using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class PaymentFormViewModel : ViewModelBase
    {
        private readonly FinancialController _financialController;

        #region Properties

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
                SetProperty(ref _selectedContract, value);
                // Auto-fill amount when contract is selected
                if (value != null)
                {
                    // Lấy giá phòng từ thông tin hợp đồng hoặc phòng
                    // Tạm thời set mặc định
                    Amount = 2500000; // Có thể thay bằng giá thực tế từ database
                }
            }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private DateTime _paymentDate = DateTime.Now;
        public DateTime PaymentDate
        {
            get => _paymentDate;
            set => SetProperty(ref _paymentDate, value);
        }

        private string _paymentMethod = "Tiền mặt";
        public string PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value);
        }

        private string _notes;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        private string _thangNam = DateTime.Now.ToString("MM/yyyy");
        public string ThangNam
        {
            get => _thangNam;
            set => SetProperty(ref _thangNam, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        #endregion

        #region Commands

        public ICommand SavePaymentCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadDataCommand { get; }

        #endregion

        #region Events

        public event EventHandler<string> ShowMessageRequested;
        public event EventHandler PaymentSaved;

        #endregion

        public PaymentFormViewModel()
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
            SavePaymentCommand = new RelayCommand(async () => await SavePaymentAsync());
            CancelCommand = new RelayCommand(() => CancelPayment());
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

                // Lấy danh sách hợp đồng đang hoạt động
                var activeContracts = await _financialController.GetActiveContractDtosAsync();
                Contracts.Clear();

                foreach (var contract in activeContracts)
                {
                    Contracts.Add(contract);
                }

                if (Contracts.Count > 0)
                {
                    SelectedContract = Contracts[0];
                }

                ShowMessage("Đã tải danh sách hợp đồng thành công!", "Thành công", MessageBoxImage.Information);
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

        public bool ValidatePayment()
        {
            if (SelectedContract == null)
            {
                ShowMessage("Vui lòng chọn hợp đồng", "Lỗi", MessageBoxImage.Warning);
                return false;
            }

            if (Amount <= 0)
            {
                ShowMessage("Số tiền phải lớn hơn 0", "Lỗi", MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(ThangNam) || !System.Text.RegularExpressions.Regex.IsMatch(ThangNam, @"^(0[1-9]|1[0-2])/\d{4}$"))
            {
                ShowMessage("Tháng năm phải có định dạng MM/yyyy", "Lỗi", MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public async Task SavePaymentAsync()
        {
            try
            {
                if (!ValidatePayment()) return;

                var paymentDto = new CreatePaymentDto
                {
                    MaHopDong = SelectedContract.MaHopDong,
                    ThangNam = ThangNam,
                    TienThue = Amount,
                    TienDien = 0, // Có thể thêm fields cho các loại chi phí khác
                    TienNuoc = 0,
                    TienInternet = 0,
                    TienVeSinh = 0,
                    TienGiuXe = 0,
                    ChiPhiKhac = 0
                };

                var result = await _financialController.CreatePaymentAsync(paymentDto);

                if (result.IsValid)
                {
                    ShowMessage("Ghi nhận thanh toán thành công!", "Thành công", MessageBoxImage.Information);
                    PaymentSaved?.Invoke(this, EventArgs.Empty);

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
                ShowMessage($"Lỗi khi lưu thanh toán: {ex.Message}", "Lỗi", MessageBoxImage.Error);
            }
        }

        #endregion

        #region Private Methods

        private void CancelPayment()
        {
            // Reset form khi hủy
            ResetForm();
        }

        private void ResetForm()
        {
            Amount = 0;
            ThangNam = DateTime.Now.ToString("MM/yyyy");
            PaymentDate = DateTime.Now;
            PaymentMethod = "Tiền mặt";
            Notes = string.Empty;

            if (Contracts.Count > 0)
            {
                SelectedContract = Contracts[0];
            }
        }

        private void ShowMessage(string message, string title, MessageBoxImage icon)
        {
            ShowMessageRequested?.Invoke(this, message);
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        #endregion
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

        private void ShowMessage(string message, string title, MessageBoxImage icon)
        {
            ShowMessageRequested?.Invoke(this, message);
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        #endregion
    }
}