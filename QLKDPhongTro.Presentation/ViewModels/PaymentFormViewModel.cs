using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels.Base; 

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class PaymentFormViewModel : ViewModelBase
    {
        private readonly FinancialController _financialController;
        private readonly Dictionary<int, List<ContractDto>> _contractsByTenant = new();

        #region Properties Binding

        private ObservableCollection<CustomerOption> _customers = new ObservableCollection<CustomerOption>();
        public ObservableCollection<CustomerOption> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        private CustomerOption _selectedCustomer;
        public CustomerOption SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (SetProperty(ref _selectedCustomer, value))
                {
                    LoadContractsForCustomer(value);
                }
            }
        }

        private ObservableCollection<ContractDto> _contracts = new ObservableCollection<ContractDto>();
        public ObservableCollection<ContractDto> Contracts
        {
            get => _contracts;
            set => SetProperty(ref _contracts, value);
        }

        private ContractDto _selectedContract;
        public ContractDto SelectedContract
        {
            get => _selectedContract;
            set
            {
                // 1. Cập nhật hợp đồng được chọn
                SetProperty(ref _selectedContract, value);

                // 2. Logic tự động điền thông tin khi chọn hợp đồng
                if (value != null)
                {
                    // Tự động điền Giá thuê
                    Amount = value.GiaThue;

                    // Tự động điền Tên phòng (Lấy từ DTO hoặc tìm theo logic)
                    // Giả sử ContractDto có sẵn TenPhong. Nếu không, cần gọi Controller lấy Room theo MaPhong
                    RoomName = value.TenPhong ?? "Không xác định";
                }
                else
                {
                    RoomName = string.Empty;
                    Amount = 0;
                }
            }
        }

        // Thêm Property riêng cho Tên Phòng để Binding ổn định hơn
        private string _roomName;
        public string RoomName
        {
            get => _roomName;
            set => SetProperty(ref _roomName, value);
        }

        // ... Các properties khác giữ nguyên (Amount, SoDien, SoNuoc...)
        private decimal _amount;
        public decimal Amount { get => _amount; set => SetProperty(ref _amount, value); }

        private decimal _soDien;
        public decimal SoDien { get => _soDien; set => SetProperty(ref _soDien, value); }

        private decimal _donGiaDien = 3500;
        public decimal DonGiaDien { get => _donGiaDien; set => SetProperty(ref _donGiaDien, value); }

        // Nước khoán
        private decimal _soNuoc = 1;
        public decimal SoNuoc { get => _soNuoc; set => SetProperty(ref _soNuoc, value); }

        private decimal _donGiaNuoc = 100000;
        public decimal DonGiaNuoc { get => _donGiaNuoc; set => SetProperty(ref _donGiaNuoc, value); }

        // Dịch vụ
        public decimal TienInternet { get; set; }
        public decimal TienVeSinh { get; set; }
        public decimal TienGiuXe { get; set; }
        public decimal ChiPhiKhac { get; set; }

        private DateTime _paymentDate = DateTime.Now;
        public DateTime PaymentDate { get => _paymentDate; set => SetProperty(ref _paymentDate, value); }

        private bool _isLoading;
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        #endregion

        #region Commands
        public ICommand SavePaymentCommand { get; }
        public ICommand CancelCommand { get; }
        public event EventHandler PaymentSaved;
        #endregion

        public PaymentFormViewModel()
        {
            _financialController = FinancialController.CreateDefault();
            SavePaymentCommand = new RelayCommand(async () => await SavePaymentAsync());
            CancelCommand = new RelayCommand<Window>((w) => w?.Close());
            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                var activeContracts = await _financialController.GetActiveContractDtosAsync();
                _contractsByTenant.Clear();

                foreach (var contract in activeContracts)
                {
                    var tenantId = contract.MaNguoiThue;
                    if (!_contractsByTenant.TryGetValue(tenantId, out var list))
                    {
                        list = new List<ContractDto>();
                        _contractsByTenant[tenantId] = list;
                    }
                    list.Add(contract);
                }

                var tenants = await _financialController.GetAllTenantsAsync();
                var tenantDict = tenants.ToDictionary(t => t.MaKhachThue, t => t.HoTen);

                Customers.Clear();
                foreach (var kvp in _contractsByTenant)
                {
                    string name = tenantDict.TryGetValue(kvp.Key, out var hoTen)
                        ? hoTen
                        : $"Khách #{kvp.Key}";
                    Customers.Add(new CustomerOption
                    {
                        MaNguoiThue = kvp.Key,
                        HoTen = name
                    });
                }

                SelectedCustomer = Customers.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void LoadContractsForCustomer(CustomerOption customer)
        {
            Contracts.Clear();

            if (customer != null && _contractsByTenant.TryGetValue(customer.MaNguoiThue, out var contractList))
            {
                foreach (var contract in contractList)
                {
                    Contracts.Add(contract);
                }

                SelectedContract = Contracts.FirstOrDefault();
            }
            else
            {
                SelectedContract = null;
                RoomName = string.Empty;
                Amount = 0;
            }
        }

        public async Task SavePaymentAsync()
        {
            if (SelectedContract == null)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng!", "Cảnh báo");
                return;
            }

            try
            {
                decimal thanhTienDien = SoDien * DonGiaDien;
                decimal thanhTienNuoc = SoNuoc * DonGiaNuoc;

                var paymentDto = new CreatePaymentDto
                {
                    MaHopDong = SelectedContract.MaHopDong,
                    ThangNam = PaymentDate.ToString("MM/yyyy"),
                    TienThue = Amount,

                    SoDien = SoDien,
                    DonGiaDien = DonGiaDien,
                    TienDien = thanhTienDien,
                    SoDienThangTruoc = 0,

                    SoNuoc = SoNuoc,
                    DonGiaNuoc = DonGiaNuoc,
                    TienNuoc = thanhTienNuoc,

                    TienInternet = TienInternet,
                    TienVeSinh = TienVeSinh,
                    TienGiuXe = TienGiuXe,
                    ChiPhiKhac = ChiPhiKhac
                };

                var result = await _financialController.CreatePaymentAsync(paymentDto);

                if (result.IsValid)
                {
                    MessageBox.Show("Thêm thanh toán thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    PaymentSaved?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show($"Lỗi: {result.Message}", "Lỗi Validate", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public sealed class CustomerOption
    {
        public int MaNguoiThue { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public override string ToString() => HoTen;
    }

    public class ExpenseFormViewModel : ViewModelBase
    {
        private readonly FinancialController _financialController;

        #region Properties

        private ObservableCollection<string> _expenseTypes = new ObservableCollection<string>
        {
            "Điện",
            "Nước",
            "Internet",
            "Vệ sinh",
            "Bảo trì",
            "Vật tư",
            "Khác"
        };
        public ObservableCollection<string> ExpenseTypes
        {
            get => _expenseTypes;
            set => SetProperty(ref _expenseTypes, value);
        }

        private ObservableCollection<PaymentDto> _payments = new ObservableCollection<PaymentDto>();
        public ObservableCollection<PaymentDto> Payments
        {
            get => _payments;
            set => SetProperty(ref _payments, value);
        }

        private PaymentDto _selectedPayment;
        public PaymentDto SelectedPayment
        {
            get => _selectedPayment;
            set => SetProperty(ref _selectedPayment, value);
        }

        private string _selectedExpenseType;
        public string SelectedExpenseType
        {
            get => _selectedExpenseType;
            set => SetProperty(ref _selectedExpenseType, value);
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private DateTime _expenseDate = DateTime.Now;
        public DateTime ExpenseDate
        {
            get => _expenseDate;
            set => SetProperty(ref _expenseDate, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _notes;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        #endregion

        #region Commands

        public ICommand SaveExpenseCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadDataCommand { get; }

        #endregion

        #region Events

        public event EventHandler<string> ShowMessageRequested;
        public event EventHandler ExpenseSaved;

        #endregion

        public ExpenseFormViewModel()
        {
            // Khởi tạo controller với các repositories
            var paymentRepository = new PaymentRepository();
            var contractRepository = new ContractRepository();
            var roomRepository = new RentedRoomRepository();
            var tenantRepository = new TenantRepository();

            _financialController = new FinancialController(
                paymentRepository,
                contractRepository,
                roomRepository,
                tenantRepository);

            // Khởi tạo commands
            SaveExpenseCommand = new RelayCommand(async () => await SaveExpenseAsync());
            CancelCommand = new RelayCommand(() => CancelExpense());
            LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());

            // Load dữ liệu ban đầu
            _ = LoadDataAsync();
        }

        #region Public Methods

        public async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                // Lấy danh sách thanh toán để áp dụng chi phí
                var payments = await _financialController.GetAllPaymentsAsync();
                Payments.Clear();

                foreach (var payment in payments)
                {
                    Payments.Add(payment);
                }

                if (ExpenseTypes.Count > 0)
                {
                    SelectedExpenseType = ExpenseTypes[0];
                }

                ShowMessage("Đã tải dữ liệu thành công!", "Thành công", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public bool ValidateExpense()
        {
            if (SelectedPayment == null)
            {
                ShowMessage("Vui lòng chọn thanh toán để áp dụng chi phí", "Lỗi", MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(SelectedExpenseType))
            {
                ShowMessage("Vui lòng chọn loại chi phí", "Lỗi", MessageBoxImage.Warning);
                return false;
            }

            if (Amount <= 0)
            {
                ShowMessage("Số tiền phải lớn hơn 0", "Lỗi", MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(Description))
            {
                ShowMessage("Vui lòng nhập mô tả chi phí", "Lỗi", MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public async Task SaveExpenseAsync()
        {
            try
            {
                if (!ValidateExpense()) return;

                var expenseDto = new ExpenseDto
                {
                    MaThanhToan = SelectedPayment.MaThanhToan,
                    LoaiChiPhi = SelectedExpenseType,
                    SoTien = Amount,
                    MoTa = Description,
                    NgayTao = ExpenseDate
                };

                var result = await _financialController.CreateExpenseAsync(expenseDto);

                if (result.IsValid)
                {
                    ShowMessage("Ghi nhận chi phí thành công!", "Thành công", MessageBoxImage.Information);
                    ExpenseSaved?.Invoke(this, EventArgs.Empty);

                    // Reset form sau khi lưu thành công
                    ResetForm();
                }
                else
                {
                    ShowMessage($"Lỗi: {result.Message}", "Lỗi", MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Lỗi khi lưu chi phí: {ex.Message}", "Lỗi", MessageBoxImage.Error);
            }
        }

        #endregion

        #region Private Methods

        private void CancelExpense()
        {
            // Reset form khi hủy
            ResetForm();
        }

        private void ResetForm()
        {
            Amount = 0;
            Description = string.Empty;
            Notes = string.Empty;
            ExpenseDate = DateTime.Now;

            if (ExpenseTypes.Count > 0)
            {
                SelectedExpenseType = ExpenseTypes[0];
            }

            if (Payments.Count > 0)
            {
                SelectedPayment = Payments[0];
            }
        }

        // Sửa lại: Bỏ qua title và icon, chỉ gửi message
        private void ShowMessage(string message, string title, MessageBoxImage icon)
        {
            ShowMessageRequested?.Invoke(this, message);
            // MessageBox.Show(message, title, MessageBoxButton.OK, icon); // View sẽ xử lý việc hiển thị MessageBox
        }

        #endregion
    }
}