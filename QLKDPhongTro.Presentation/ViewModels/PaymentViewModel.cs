using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using QLKDPhongTro.Presentation.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.Presentation.Views.Windows;
// using QLKDPhongTro.DataLayer.Repositories; // Tránh tham chiếu trực tiếp repo ở Presentation

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class PaymentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly FinancialController _financialController;

        // Nguồn dữ liệu đầy đủ và trang hiện tại (tương tự sự cố bảo trì)
        private List<PaymentItem> _allPayments = new();
        private ObservableCollection<PaymentItem> _payments = new();
        public ObservableCollection<PaymentItem> Payments
        {
            get => _payments;
            set
            {
                _payments = value;
                OnPropertyChanged(nameof(Payments));
            }
        }

        // Phân trang
        private int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set { _pageSize = value <= 0 ? 5 : value; OnPropertyChanged(nameof(PageSize)); ApplySortAndPage(); }
        }
        private int _pageIndex = 1; // 1-based
        public int PageIndex
        {
            get => _pageIndex;
            set { _pageIndex = value < 1 ? 1 : value; OnPropertyChanged(nameof(PageIndex)); ApplySortAndPage(); }
        }
        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = value < 1 ? 1 : value; OnPropertyChanged(nameof(TotalPages)); }
        }

        // Sắp xếp
        private string _sortOrder = "newest"; // newest | oldest
        public string SortOrder
        {
            get => _sortOrder;
            set { _sortOrder = value; OnPropertyChanged(nameof(SortOrder)); ApplySortAndPage(); }
        }

        // Tìm kiếm
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value ?? string.Empty; OnPropertyChanged(nameof(SearchText)); PageIndex = 1; ApplySortAndPage(); }
        }

        private string _paginationText = string.Empty;
        public string PaginationText
        {
            get => _paginationText;
            set { _paginationText = value; OnPropertyChanged(nameof(PaginationText)); }
        }

        // Chọn dòng hiện tại
        private PaymentItem? _selectedPayment;
        public PaymentItem? SelectedPayment
        {
            get => _selectedPayment;
            set { _selectedPayment = value; OnPropertyChanged(nameof(SelectedPayment)); }
        }

        // Lệnh phân trang và CRUD
        public ICommand PrevPageCommand { get; }
        public ICommand NextPageCommand { get; }

        private PaymentItem? _editingItem;
        private PaymentItem? _originalItem;

        // Commands
        public ICommand ViewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ShowAddPaymentCommand { get; }

        // Danh sách lựa chọn trạng thái khi chỉnh sửa
        public IList<string> EditStatusChoices { get; } = new List<string> { "Chưa trả", "Trả một phần", "Đã trả" };

        public PaymentViewModel()
        {
            _financialController = FinancialController.CreateDefault();

            _ = LoadDataAsync();

            // Initialize commands
            ViewCommand = new RelayCommand<PaymentItem>(ViewPayment);
            EditCommand = new RelayCommand<PaymentItem>(EditPayment);
            SaveCommand = new RelayCommand<PaymentItem>(async p => await SavePayment(p));
            CancelCommand = new RelayCommand<PaymentItem>(CancelEdit);
            DeleteCommand = new RelayCommand<PaymentItem>(async p => await DeletePayment(p));
            ShowAddPaymentCommand = new RelayCommand(ShowAddPayment);

            PrevPageCommand = new RelayCommand(
                () => { if (PageIndex > 1) PageIndex--; },
                () => PageIndex > 1);
            NextPageCommand = new RelayCommand(
                () => { if (PageIndex < TotalPages) PageIndex++; },
                () => PageIndex < TotalPages);
        }

        public async Task LoadDataAsync()
        {
            try
            {
                var paymentDtos = await _financialController.GetAllPaymentsAsync();
                _allPayments = paymentDtos.Select(MapToItem).ToList();
                PageIndex = 1;
                ApplySortAndPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ShowAddPayment()
        {
            // 1. Khởi tạo cửa sổ thêm mới
            var addWindow = new AddBillingInfoWindow();

            // 2. Hiển thị dạng Dialog (chờ người dùng thao tác xong)
            bool? result = addWindow.ShowDialog();

            // 3. Nếu form trả về True (Lưu thành công) -> Reload danh sách
            if (result == true)
            {
                _ = LoadDataAsync();
            }
        }
        private async void ViewPayment(PaymentItem? payment)
        {
            if (payment != null)
            {
                try
                {
                    var id = int.Parse(payment.Id);
                    var invoiceData = await _financialController.GetInvoiceDetailAsync(id);

                    if (invoiceData != null)
                    {
                        var invoiceWindow = new InvoiceDetailView(invoiceData)
                        {
                            Owner = Application.Current.MainWindow
                        };
                        invoiceWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin hóa đơn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi tải thông tin hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ApplySortAndPage()
        {
            IEnumerable<PaymentItem> query = _allPayments;
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim().ToLowerInvariant();
                query = query.Where(x =>
                    // Mã thanh toán
                    (!string.IsNullOrEmpty(x.Id) && x.Id.ToLowerInvariant().Contains(keyword))
                    // Ngày hiển thị dd/MM/yyyy
                    || x.Date.ToString("dd/MM/yyyy").ToLowerInvariant().Contains(keyword)
                    // Tổng tiền (số thuần)
                    || x.TotalAmount.ToString().ToLowerInvariant().Contains(keyword)
                    // Tổng tiền định dạng N0 với phân tách hàng nghìn
                    || x.TotalAmount.ToString("N0").ToLowerInvariant().Contains(keyword)
                    // Trạng thái (Đã thanh toán/Chưa thanh toán)
                    || (!string.IsNullOrEmpty(x.Status) && x.Status.ToLowerInvariant().Contains(keyword))
                    // Tên phòng
                    || (!string.IsNullOrEmpty(x.RoomName) && x.RoomName.ToLowerInvariant().Contains(keyword))
                );
            }

            if (SortOrder == "newest")
                query = query.OrderByDescending(x => x.Date);
            else
                query = query.OrderBy(x => x.Date);

            var total = query.Count();
            TotalPages = (int)Math.Ceiling(total / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;

            var skip = (PageIndex - 1) * PageSize;
            var pageItems = query.Skip(skip).Take(PageSize).ToList();

            // Clear và thêm items mới
            // KHÔNG đăng ký event handler cho EditableStatus để tránh auto-update
            // Chỉ update khi người dùng nhấn nút Save
            Payments.Clear();
            foreach (var item in pageItems)
            {
                Payments.Add(item);
            }

            var from = total == 0 ? 0 : skip + 1;
            var to = skip + pageItems.Count;
            PaginationText = $"Hiển thị {from} đến {to} trong {total}";
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        // Đã tắt auto-update khi thay đổi ComboBox để tránh mất trạng thái
        // Chỉ update khi người dùng nhấn nút Save
        // private async void OnPaymentItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        // {
        //     if (sender is PaymentItem item && e.PropertyName == nameof(PaymentItem.EditableStatus))
        //     {
        //         await UpdatePaymentStatusAsync(item);
        //     }
        // }

        // private async Task UpdatePaymentStatusAsync(PaymentItem item)
        // {
        //     try
        //     {
        //         if (string.IsNullOrWhiteSpace(item.EditableStatus)) return;

        //         var id = int.Parse(item.Id);
        //         var result = await _financialController.UpdatePaymentStatusAsync(id, item.EditableStatus);
        //         if (!result.IsValid)
        //         {
        //             MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //             return;
        //         }

        //         // Đồng bộ lại hiển thị theo DTO trả về
        //         if (result.Data is QLKDPhongTro.BusinessLayer.DTOs.PaymentDto updated)
        //         {
        //             item.Status = updated.TrangThaiThanhToan;
        //             item.Date = updated.NgayThanhToan ?? item.Date;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show($"Lỗi khi cập nhật trạng thái: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //     }
        // }

        private static PaymentItem MapToItem(PaymentDto dto)
        {
            return new PaymentItem
            {
                Id = dto.MaThanhToan.ToString(),
                Date = dto.NgayThanhToan ?? ParseThangNamToDate(dto.ThangNam),
                TotalAmount = dto.TongTien,
                Status = MapStatus(dto.TrangThaiThanhToan),
                RoomName = dto.TenPhong ?? string.Empty,
                PaymentMethod = string.Empty,
                AmountPaid = dto.SoTienDaTra ?? 0,
                PartialPaymentInput = dto.SoTienDaTra.HasValue && dto.SoTienDaTra > 0 ? dto.SoTienDaTra : null
            };
        }

        private static string MapStatus(string? trangThai)
        {
            // Đồng bộ với các trạng thái hợp lệ
            if (string.Equals(trangThai, "Đã trả", StringComparison.OrdinalIgnoreCase)) return "Đã trả";
            if (string.Equals(trangThai, "Trả một phần", StringComparison.OrdinalIgnoreCase)) return "Trả một phần";
            if (string.Equals(trangThai, "Chưa trả", StringComparison.OrdinalIgnoreCase)) return "Chưa trả";
            return "Chưa trả";
        }

        private static DateTime ParseThangNamToDate(string thangNam)
        {
            try
            {
                var parts = thangNam.Split('/');
                if (parts.Length == 2 && int.TryParse(parts[0], out int m) && int.TryParse(parts[1], out int y))
                {
                    return new DateTime(y, m, 1);
                }
            }
            catch { }
            return DateTime.Now;
        }


        private void EditPayment(PaymentItem? payment)
        {
            if (payment != null)
            {
                // Tắt chế độ sửa của dòng khác (nếu có) để tránh lỗi
                if (_editingItem != null && _editingItem != payment)
                {
                    CancelEdit(_editingItem);
                }

                _editingItem = payment;

                // Lưu trạng thái gốc để restore nếu bấm Hủy
                var initialStatus = string.Equals(payment.Status, "Đã trả", StringComparison.OrdinalIgnoreCase) ? "Đã trả" : "Chưa trả";

                _originalItem = new PaymentItem
                {
                    Id = payment.Id,
                    TotalAmount = payment.TotalAmount,
                    AmountPaid = payment.AmountPaid,
                    PartialPaymentInput = payment.PartialPaymentInput,
                    PaymentMethod = payment.PaymentMethod,
                    Status = payment.Status,
                    EditableStatus = initialStatus
                };

                // Kích hoạt chế độ sửa
                payment.EditableStatus = initialStatus;
                payment.PartialPaymentInput = payment.AmountPaid > 0 ? payment.AmountPaid : (decimal?)null;
                payment.IsEditing = true; // Property này sẽ kích hoạt Trigger ẩn hiện nút bấm
            }
        }

        private async Task SavePayment(PaymentItem? payment)
        {
            if (payment != null && _editingItem == payment)
            {
                // Validate data
                if (string.IsNullOrWhiteSpace(payment.EditableStatus))
                {
                    MessageBox.Show("Vui lòng chọn trạng thái thanh toán!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    // Cập nhật trực tiếp bản ghi Payment trong DB theo thông tin đã chỉnh sửa
                    var id = int.Parse(payment.Id);
                    // Đảm bảo giá trị EditableStatus không null hoặc empty
                    var desired = payment.EditableStatus?.Trim() ?? payment.Status;
                    decimal? partialAmount = null;

                    var isValidStatus = EditStatusChoices.Any(status => status.Equals(desired, StringComparison.OrdinalIgnoreCase));
                    if (!isValidStatus)
                    {
                        MessageBox.Show("Trạng thái không hợp lệ. Vui lòng chọn trong danh sách cho phép.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (string.Equals(desired, "Trả một phần", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!payment.PartialPaymentInput.HasValue || payment.PartialPaymentInput <= 0)
                        {
                            MessageBox.Show("Vui lòng nhập số tiền đã thanh toán khi chọn 'Trả một phần'.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        partialAmount = payment.PartialPaymentInput;
                    }

                    var result = await _financialController.UpdatePaymentStatusAsync(id, desired, partialAmount);
                    if (!result.IsValid)
                    {
                        MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        // Khôi phục giá trị cũ nếu update thất bại
                        if (_originalItem != null)
                        {
                            payment.EditableStatus = _originalItem.Status;
                        }
                        return;
                    }

                    // Sau khi update thành công, reload dữ liệu từ database để đảm bảo đồng bộ
                    // Điều này đảm bảo rằng giá trị trong UI khớp với giá trị trong database
                    payment.IsEditing = false;
                    _editingItem = null;
                    _originalItem = null;
                    
                    // Reload dữ liệu từ database để đảm bảo đồng bộ
                    await LoadDataAsync();
                    
                    MessageBox.Show($"Đã lưu thành công thanh toán: {payment.Id}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    // Khôi phục giá trị cũ nếu có lỗi
                    if (_originalItem != null)
                    {
                        payment.EditableStatus = _originalItem.Status;
                    }
                }
            }

        }

        private void CancelEdit(PaymentItem? payment)
        {
            if (payment != null && _editingItem == payment && _originalItem != null)
            {
                // Restore original values
                payment.Id = _originalItem.Id;
                payment.Date = _originalItem.Date;
                payment.TotalAmount = _originalItem.TotalAmount;
                payment.Status = _originalItem.Status;
                payment.PaymentMethod = _originalItem.PaymentMethod;
                payment.AmountPaid = _originalItem.AmountPaid;
                payment.PartialPaymentInput = _originalItem.PartialPaymentInput;
                // Khôi phục EditableStatus về giá trị ban đầu
                payment.EditableStatus = _originalItem.Status;
                payment.IsEditing = false;
                
                _editingItem = null;
                _originalItem = null;
                
                OnPropertyChanged(nameof(Payments));
                MessageBox.Show("Đã hủy chỉnh sửa", "Hủy", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task DeletePayment(PaymentItem? payment)
        {
            if (payment != null)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn xóa thanh toán: {payment.Id}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var id = int.Parse(payment.Id);
                        var deleteResult = await _financialController.DeletePaymentAsync(id);
                        if (deleteResult.IsValid)
                        {
                            _allPayments.Remove(payment);
                            ApplySortAndPage();
                            MessageBox.Show($"Đã xóa thành công thanh toán: {payment.Id}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show(deleteResult.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PaymentItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _id = string.Empty;
        private DateTime _date;
        private decimal _totalAmount;
        private string _status = string.Empty;
        private string _editableStatus = string.Empty;
        private string _paymentMethod = string.Empty;
        private bool _isEditing;
        private string _roomName = string.Empty;
        private decimal _amountPaid;
        private decimal? _partialPaymentInput;

        public string Id 
        { 
            get => _id; 
            set 
            { 
                _id = value; 
                OnPropertyChanged(nameof(Id)); 
            } 
        }
        
        public DateTime Date 
        { 
            get => _date; 
            set 
            { 
                _date = value; 
                OnPropertyChanged(nameof(Date)); 
            } 
        }
        
        public decimal TotalAmount 
        { 
            get => _totalAmount; 
            set 
            { 
                _totalAmount = value; 
                OnPropertyChanged(nameof(TotalAmount)); 
                OnPropertyChanged(nameof(RemainingAmount));
                OnPropertyChanged(nameof(HasRemainingAmount));
            } 
        }
        
        public string Status 
        { 
            get => _status; 
            set 
            { 
                _status = value; 
                OnPropertyChanged(nameof(Status)); 
                OnPropertyChanged(nameof(IsPartialStatus));
                OnPropertyChanged(nameof(HasRemainingAmount));
            } 
        }

        // Trạng thái dùng cho ComboBox chỉnh sửa (Đã trả/Chưa trả)
        public string EditableStatus
        {
            get => _editableStatus;
            set
            {
                if (_editableStatus == value) return;
                _editableStatus = value;
                OnPropertyChanged(nameof(EditableStatus));

                if (!string.Equals(_editableStatus, "Trả một phần", StringComparison.OrdinalIgnoreCase))
                {
                    PartialPaymentInput = null;
                }
            }
        }
        
        public string PaymentMethod 
        { 
            get => _paymentMethod; 
            set 
            { 
                _paymentMethod = value; 
                OnPropertyChanged(nameof(PaymentMethod)); 
            } 
        }

        public string RoomName
        {
            get => _roomName;
            set
            {
                _roomName = value;
                OnPropertyChanged(nameof(RoomName));
            }
        }

        public decimal AmountPaid
        {
            get => _amountPaid;
            set
            {
                _amountPaid = value;
                OnPropertyChanged(nameof(AmountPaid));
                OnPropertyChanged(nameof(RemainingAmount));
                OnPropertyChanged(nameof(HasPaidAmount));
                OnPropertyChanged(nameof(HasRemainingAmount));
            }
        }

        public decimal RemainingAmount => Math.Max(0, TotalAmount - AmountPaid);
        public bool HasPaidAmount => AmountPaid > 0;
        // Chỉ hiển thị tiền còn lại khi đang ở trạng thái "Trả một phần" và còn nợ
        public bool HasRemainingAmount => IsPartialStatus && RemainingAmount > 0;

        public decimal? PartialPaymentInput
        {
            get => _partialPaymentInput;
            set
            {
                _partialPaymentInput = value;
                OnPropertyChanged(nameof(PartialPaymentInput));
            }
        }

        public bool IsPartialStatus => string.Equals(Status, "Trả một phần", StringComparison.OrdinalIgnoreCase);

        public bool IsEditing 
        { 
            get => _isEditing; 
            set 
            { 
                _isEditing = value; 
                OnPropertyChanged(nameof(IsEditing)); 
            } 
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
