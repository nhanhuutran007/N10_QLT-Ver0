using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using QLKDPhongTro.Presentation.Commands;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class PaymentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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

        private PaymentItem? _editingItem;
        private PaymentItem? _originalItem;

        // Commands
        public ICommand ViewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }

        public PaymentViewModel()
        {
            LoadSampleData();
            
            // Initialize commands
            ViewCommand = new RelayCommand<PaymentItem>(ViewPayment);
            EditCommand = new RelayCommand<PaymentItem>(EditPayment);
            SaveCommand = new RelayCommand<PaymentItem>(SavePayment);
            CancelCommand = new RelayCommand<PaymentItem>(CancelEdit);
            DeleteCommand = new RelayCommand<PaymentItem>(DeletePayment);
        }

        private void LoadSampleData()
        {
            Payments = new ObservableCollection<PaymentItem>
            {
                new PaymentItem { Id = "PAY001", Date = DateTime.Now.AddDays(-5), TotalAmount = 4500000, Status = "paid", PaymentMethod = "Chuyển khoản" },
                new PaymentItem { Id = "PAY002", Date = DateTime.Now.AddDays(-3), TotalAmount = 3800000, Status = "pending", PaymentMethod = "Tiền mặt" },
                new PaymentItem { Id = "PAY003", Date = DateTime.Now.AddDays(-1), TotalAmount = 5200000, Status = "unpaid", PaymentMethod = "Chuyển khoản" },
                new PaymentItem { Id = "PAY004", Date = DateTime.Now.AddDays(-7), TotalAmount = 4100000, Status = "paid", PaymentMethod = "Tiền mặt" },
                new PaymentItem { Id = "PAY005", Date = DateTime.Now.AddDays(-2), TotalAmount = 3900000, Status = "failed", PaymentMethod = "Chuyển khoản" }
            };
        }

        private void ViewPayment(PaymentItem? payment)
        {
            if (payment != null)
            {
                MessageBox.Show($"Xem chi tiết thanh toán: {payment.Id}", "Thông tin", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditPayment(PaymentItem? payment)
        {
            if (payment != null)
            {
                _editingItem = payment;
                _originalItem = new PaymentItem
                {
                    Id = payment.Id,
                    Date = payment.Date,
                    TotalAmount = payment.TotalAmount,
                    Status = payment.Status,
                    PaymentMethod = payment.PaymentMethod
                };
                
                // Set editing mode
                payment.IsEditing = true;
                OnPropertyChanged(nameof(Payments));
                
                MessageBox.Show($"Bắt đầu chỉnh sửa: {payment.Id}", "Chỉnh sửa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SavePayment(PaymentItem? payment)
        {
            if (payment != null && _editingItem == payment)
            {
                // Validate data
                if (string.IsNullOrWhiteSpace(payment.Status) || string.IsNullOrWhiteSpace(payment.PaymentMethod))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Save to database (simulate)
                try
                {
                    // TODO: Implement actual database save logic here
                    // await PaymentService.UpdatePaymentAsync(payment);
                    
                    payment.IsEditing = false;
                    _editingItem = null;
                    _originalItem = null;
                    
                    MessageBox.Show($"Đã lưu thành công thanh toán: {payment.Id}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                payment.IsEditing = false;
                
                _editingItem = null;
                _originalItem = null;
                
                OnPropertyChanged(nameof(Payments));
                MessageBox.Show("Đã hủy chỉnh sửa", "Hủy", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeletePayment(PaymentItem? payment)
        {
            if (payment != null)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn xóa thanh toán: {payment.Id}?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // TODO: Implement actual database delete logic here
                        // await PaymentService.DeletePaymentAsync(payment.Id);
                        
                        Payments.Remove(payment);
                        MessageBox.Show($"Đã xóa thành công thanh toán: {payment.Id}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
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
        private string _paymentMethod = string.Empty;
        private bool _isEditing;

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
            } 
        }
        
        public string Status 
        { 
            get => _status; 
            set 
            { 
                _status = value; 
                OnPropertyChanged(nameof(Status)); 
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
