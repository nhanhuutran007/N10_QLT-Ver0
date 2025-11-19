using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class FinancialDashboardViewModel : BaseViewModel
    {
        private readonly FinancialController? _financialController;
        private readonly bool _useSampleData = false;

        #region Properties

        // 1. Thống kê tổng quan
        private FinancialStatsDto _financialStats = new();
        public FinancialStatsDto FinancialStats
        {
            get => _financialStats;
            set
            {
                _financialStats = value;
                OnPropertyChanged();
            }
        }

        // 2. Dữ liệu thống kê theo tháng (Nguồn dữ liệu gốc cho biểu đồ)
        private ObservableCollection<MonthlyStatsDto> _monthlyStats = new();
        public ObservableCollection<MonthlyStatsDto> MonthlyStats
        {
            get => _monthlyStats;
            set
            {
                _monthlyStats = value;
                OnPropertyChanged();

                // Khi dữ liệu tháng thay đổi, thông báo cho View cập nhật lại các biểu đồ
                OnPropertyChanged(nameof(RevenueDataPoints));
                OnPropertyChanged(nameof(ExpenseDataPoints));
                OnPropertyChanged(nameof(ProfitDataPoints));
            }
        }

        // 3. Các Property dữ liệu biểu đồ (Đã được thêm mới để fix lỗi)
        public ObservableCollection<ChartDataPoint> RevenueDataPoints =>
            MonthlyStats != null
                ? new ObservableCollection<ChartDataPoint>(MonthlyStats.Select(m => new ChartDataPoint { Label = m.ThangNam, Value = m.ThuNhap }))
                : new ObservableCollection<ChartDataPoint>();

        public ObservableCollection<ChartDataPoint> ExpenseDataPoints =>
            MonthlyStats != null
                ? new ObservableCollection<ChartDataPoint>(MonthlyStats.Select(m => new ChartDataPoint { Label = m.ThangNam, Value = m.ChiPhi }))
                : new ObservableCollection<ChartDataPoint>();

        public ObservableCollection<ChartDataPoint> ProfitDataPoints =>
            MonthlyStats != null
                ? new ObservableCollection<ChartDataPoint>(MonthlyStats.Select(m => new ChartDataPoint { Label = m.ThangNam, Value = m.LoiNhuan }))
                : new ObservableCollection<ChartDataPoint>();


        // 4. Dữ liệu biểu đồ thực tế theo ngày thanh toán
        private ObservableCollection<MonthlyStatsDto> _paidMonthlyStats = new();
        public ObservableCollection<MonthlyStatsDto> PaidMonthlyStats
        {
            get => _paidMonthlyStats;
            private set
            {
                _paidMonthlyStats = value;
                OnPropertyChanged();
                // Đảm bảo MaxMonthlyRevenue luôn > 0 để biểu đồ hiển thị đúng
                var maxValue = _paidMonthlyStats != null && _paidMonthlyStats.Any() 
                    ? _paidMonthlyStats.Max(m => m.ThuNhap) 
                    : 0;
                // Nếu max = 0, đặt giá trị mặc định là 1 để tránh chia cho 0
                MaxMonthlyRevenue = maxValue > 0 ? maxValue : 1;
            }
        }

        private decimal _maxMonthlyRevenue = 1; // Giá trị mặc định để tránh chia cho 0
        public decimal MaxMonthlyRevenue
        {
            get => _maxMonthlyRevenue;
            private set
            {
                // Đảm bảo giá trị luôn >= 1
                _maxMonthlyRevenue = value >= 1 ? value : 1;
                OnPropertyChanged();
            }
        }

        // 5. Danh sách công nợ
        private ObservableCollection<DebtReportDto> _debts = new();
        public ObservableCollection<DebtReportDto> Debts
        {
            get => _debts;
            set
            {
                _debts = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDebts));
                OnPropertyChanged(nameof(TotalDebtsCount));
                OnPropertyChanged(nameof(TotalDebtAmount));
            }
        }

        // 6. Lịch sử giao dịch
        private ObservableCollection<TransactionHistoryDto> _transactionHistory = new();
        public ObservableCollection<TransactionHistoryDto> TransactionHistory
        {
            get => _transactionHistory;
            set
            {
                _transactionHistory = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasTransactions));
                OnPropertyChanged(nameof(TotalTransactionsCount));
                OnPropertyChanged(nameof(TotalTransactionAmount));
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private RecentTenantInfoDto? _recentTenant;
        public RecentTenantInfoDto? RecentTenant
        {
            get => _recentTenant;
            set
            {
                _recentTenant = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<RecentTenantInfoDto> _recentTenants = new();
        public ObservableCollection<RecentTenantInfoDto> RecentTenants
        {
            get => _recentTenants;
            set
            {
                _recentTenants = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        public bool IsNotLoading => !IsLoading;

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public DebtReportDto? SelectedDebt { get; set; }

        // 7. Chỉ số tăng trưởng
        private decimal _revenueGrowthPercent;
        public decimal RevenueGrowthPercent
        {
            get => _revenueGrowthPercent;
            set
            {
                _revenueGrowthPercent = value;
                OnPropertyChanged();
                RevenueGrowthText = FormatGrowthText(_revenueGrowthPercent);
                RevenueGrowthColor = _revenueGrowthPercent > 0 ? "#0A7D5A" : _revenueGrowthPercent < 0 ? "#FF316A" : "#586A84";
            }
        }

        private string _revenueGrowthText = string.Empty;
        public string RevenueGrowthText
        {
            get => _revenueGrowthText;
            private set
            {
                _revenueGrowthText = value;
                OnPropertyChanged();
            }
        }

        private string _revenueGrowthColor = "#586A84";
        public string RevenueGrowthColor
        {
            get => _revenueGrowthColor;
            private set
            {
                _revenueGrowthColor = value;
                OnPropertyChanged();
            }
        }

        private decimal _latestPaidMonthRevenue;
        public decimal LatestPaidMonthRevenue
        {
            get => _latestPaidMonthRevenue;
            private set
            {
                _latestPaidMonthRevenue = value;
                OnPropertyChanged();
            }
        }

        private string _latestPaidMonthLabel = DateTime.Now.Month.ToString().PadLeft(2, '0');
        public string LatestPaidMonthLabel
        {
            get => _latestPaidMonthLabel;
            private set
            {
                _latestPaidMonthLabel = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand LoadDataCommand { get; }
        public ICommand ShowPaymentFormCommand { get; }
        public ICommand ShowExpenseFormCommand { get; }
        public ICommand PayDebtCommand { get; }
        public ICommand FilterTransactionsCommand { get; }
        public ICommand ExportReportCommand { get; }
        public ICommand AutoGenerateDebtsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ClearDateFilterCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ToggleChartViewCommand { get; }

        #endregion

        #region Events

        public event EventHandler? ShowPaymentFormRequested;
        public event EventHandler? ShowExpenseFormRequested;
        public event EventHandler<string>? ShowMessageRequested;
        public event EventHandler? DataRefreshed;

        #endregion

        public FinancialDashboardViewModel()
        {
            try
            {
                var paymentRepository = new PaymentRepository();
                var contractRepository = new ContractRepository();
                var roomRepository = new RentedRoomRepository();
                var tenantRepository = new TenantRepository();

                _financialController = new FinancialController(
                    paymentRepository,
                    contractRepository,
                    roomRepository,
                    tenantRepository);
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Không thể kết nối database: {ex.Message}. Đang sử dụng dữ liệu mẫu.");
                _useSampleData = true;
            }

            // Khởi tạo commands
            LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());
            ShowPaymentFormCommand = new RelayCommand(() => ShowPaymentFormRequested?.Invoke(this, EventArgs.Empty));
            ShowExpenseFormCommand = new RelayCommand(() => ShowExpenseFormRequested?.Invoke(this, EventArgs.Empty));
            PayDebtCommand = new RelayCommand<DebtReportDto?>(async (debt) => await PayDebtAsync(debt));
            FilterTransactionsCommand = new RelayCommand(async () => await FilterTransactionsAsync());
            ExportReportCommand = new RelayCommand(async () => await ExportReportAsync());
            AutoGenerateDebtsCommand = new RelayCommand(async () => await AutoGenerateDebtsAsync());
            RefreshCommand = new RelayCommand(async () => await RefreshDataAsync());
            ClearSearchCommand = new RelayCommand(ClearSearch);
            ClearDateFilterCommand = new RelayCommand(ClearDateFilter);
            SearchCommand = new RelayCommand(async () => await SearchDebtsAsync());
            ToggleChartViewCommand = new RelayCommand(ToggleChartView);

            // Load dữ liệu ban đầu
            _ = InitializeAsync();
        }

        #region Public Methods

        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        public async Task RefreshDataAsync()
        {
            await LoadDataAsync();
            DataRefreshed?.Invoke(this, EventArgs.Empty);
        }

        public async Task LoadDataAsync()
        {
            await LoadDataInternalAsync(true);
        }

        public async Task SearchDebtsAsync()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                await LoadDataAsync();
                return;
            }

            try
            {
                IsLoading = true;

                // Filter trên dữ liệu hiện tại (tạm thời)
                var filtered = _debts
                    .Where(d => (d.TenPhong?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true)
                             || (d.TenKhachHang?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true)
                             || (d.SoDienThoai?.Contains(SearchText) == true)
                             || (d.ThangNam?.Contains(SearchText) == true))
                    .ToList();

                Debts = new ObservableCollection<DebtReportDto>(filtered);
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi tìm kiếm: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task PayDebtAsync(DebtReportDto? debt)
        {
            if (debt == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Xác nhận thanh toán cho phòng {debt.TenPhong}?\n" +
                    $"Người thuê: {debt.TenKhachHang}\n" +
                    $"Số tiền: {debt.TongTien:N0} VNĐ\n" +
                    $"Tháng: {debt.ThangNam}",
                    "Xác nhận thanh toán",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    IsLoading = true;
                    if (_financialController == null)
                    {
                        ShowMessageRequested?.Invoke(this, "Không có kết nối Controller. Đang ở chế độ dữ liệu mẫu.");
                        return;
                    }

                    var vr = await _financialController.UpdatePaymentStatusAsync(debt.MaThanhToan, "Đã trả");
                    if (vr.IsValid)
                    {
                        ShowMessageRequested?.Invoke(this, vr.Message);
                        await RefreshDataAsync();
                    }
                    else
                    {
                        ShowMessageRequested?.Invoke(this, vr.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi thanh toán: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task FilterTransactionsAsync()
        {
            try
            {
                IsLoading = true;
                if (_useSampleData || _financialController == null)
                {
                    await LoadSampleTransactions();
                    ShowMessageRequested?.Invoke(this, $"Đã lọc {TransactionHistory.Count} giao dịch (dữ liệu mẫu)");
                }
                else
                {
                    var list = await _financialController.GetTransactionHistoryAsync(FromDate, ToDate);
                    TransactionHistory = new ObservableCollection<TransactionHistoryDto>(list);
                    ShowMessageRequested?.Invoke(this, $"Đã lọc {TransactionHistory.Count} giao dịch");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi lọc giao dịch: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task AutoGenerateDebtsAsync()
        {
            try
            {
                var result = MessageBox.Show(
                    "Tính năng này sẽ tạo công nợ tự động cho tất cả hợp đồng đang hoạt động.\n" +
                    "Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận tạo công nợ tự động",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    IsLoading = true;

                    // Thêm dữ liệu mẫu mới
                    var newDebt = new DebtReportDto
                    {
                        MaThanhToan = _debts.Count + 1,
                        TenPhong = "P201",
                        TenKhachHang = "Người thuê mới",
                        SoDienThoai = "0900000000",
                        ThangNam = DateTime.Now.ToString("MM/yyyy"),
                        TongTien = 3000000
                    };

                    var updatedDebts = _debts.ToList();
                    updatedDebts.Add(newDebt);
                    Debts = new ObservableCollection<DebtReportDto>(updatedDebts);

                    ShowMessageRequested?.Invoke(this, "Đã tạo 1 công nợ tự động");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi tạo công nợ tự động: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task ExportReportAsync()
        {
            try
            {
                var reportData = new
                {
                    ThoiGian = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    TongThuNhap = FinancialStats.TongThuNhap,
                    TongChiPhi = FinancialStats.TongChiPhi,
                    LoiNhuan = FinancialStats.LoiNhuan,
                    TongCongNo = FinancialStats.TongCongNo,
                    SoPhongNo = FinancialStats.SoPhongNo,
                    SoCongNo = Debts.Count,
                    SoGiaoDich = TransactionHistory.Count
                };

                var reportInfo =
                    $"BÁO CÁO TÀI CHÍNH\n" +
                    $"Thời gian: {reportData.ThoiGian}\n" +
                    $"Tổng thu nhập: {reportData.TongThuNhap:N0} VNĐ\n" +
                    $"Tổng chi phí: {reportData.TongChiPhi:N0} VNĐ\n" +
                    $"Lợi nhuận: {reportData.LoiNhuan:N0} VNĐ\n" +
                    $"Tổng công nợ: {reportData.TongCongNo:N0} VNĐ\n" +
                    $"Số phòng nợ: {reportData.SoPhongNo}\n" +
                    $"Số công nợ: {reportData.SoCongNo}\n" +
                    $"Số giao dịch: {reportData.SoGiaoDich}";

                MessageBox.Show(reportInfo, "Báo Cáo Tài Chính", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowMessageRequested?.Invoke(this, "Báo cáo đã được tạo thành công!");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi xuất báo cáo: {ex.Message}");
            }
        }

        public void ClearSearch()
        {
            SearchText = string.Empty;
            _ = LoadDataAsync();
        }

        public void ClearDateFilter()
        {
            FromDate = null;
            ToDate = null;
            _ = FilterTransactionsAsync();
        }

        public void ToggleChartView()
        {
            // Có thể mở rộng để chuyển đổi giữa các loại biểu đồ
            ShowMessageRequested?.Invoke(this, "Chuyển đổi chế độ xem biểu đồ");
        }

        #endregion

        #region Private Helpers

        private async Task LoadDataInternalAsync(bool showMessage = true)
        {
            try
            {
                IsLoading = true;
                if (_useSampleData || _financialController == null)
                {
                    await LoadSampleData();
                    if (showMessage)
                        ShowMessageRequested?.Invoke(this, "Đang sử dụng dữ liệu mẫu.");
                    return;
                }

                // Tải dữ liệu thực từ Controller (toàn thời gian)
                var stats = await _financialController.GetFinancialStatsAsync(null);
                FinancialStats = stats;

                // Lấy thống kê theo tháng cho năm hiện tại để vẽ biểu đồ
                var yearStats = await _financialController.GetFinancialStatsAsync(DateTime.Now.Year);
                MonthlyStats = new ObservableCollection<MonthlyStatsDto>(yearStats.ThongKeTheoThang ?? new List<MonthlyStatsDto>());

                var debts = await _financialController.GetDebtReportAsync(null);
                Debts = new ObservableCollection<DebtReportDto>(debts);

                var transactions = await _financialController.GetTransactionHistoryAsync(null, null);
                TransactionHistory = new ObservableCollection<TransactionHistoryDto>(transactions);

                await CalculateRevenueGrowthPercentByPaidDateAsync();

                RecentTenant = await _financialController.GetMostRecentTenantInfoAsync();
                var top3 = await _financialController.GetMostRecentTenantsInfoAsync(3);
                RecentTenants = new ObservableCollection<RecentTenantInfoDto>(top3);
            }
            catch (Exception ex)
            {
                await LoadSampleData();
                ShowMessageRequested?.Invoke(this, $"Lỗi khi tải dữ liệu: {ex.Message}. Đang sử dụng dữ liệu mẫu.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadSampleData()
        {
            // Tạo dữ liệu mẫu
            FinancialStats = new FinancialStatsDto
            {
                TongThuNhap = 12500000,
                TongChiPhi = 3500000,
                LoiNhuan = 9000000,
                TongCongNo = 10500000,
                SoPhongNo = 3
            };

            var sampleDebts = new List<DebtReportDto>
            {
                new() { MaThanhToan = 1, TenPhong = "P101", TenKhachHang = "Nguyễn Văn A", SoDienThoai = "0123456789", ThangNam = "11/2024", TongTien = 3500000 },
                new() { MaThanhToan = 2, TenPhong = "P102", TenKhachHang = "Trần Thị B", SoDienThoai = "0987654321", ThangNam = "11/2024", TongTien = 3200000 },
                new() { MaThanhToan = 3, TenPhong = "A101", TenKhachHang = "Lê Văn C", SoDienThoai = "0912345678", ThangNam = "11/2024", TongTien = 3800000 }
            };

            Debts = new ObservableCollection<DebtReportDto>(sampleDebts);

            await LoadSampleTransactions();
            RecentTenant = new RecentTenantInfoDto { MaNguoiThue = 1, HoTen = "Nguyễn Văn A", TienCoc = 1000000m, TrangThai = "Đang ở" };
            RecentTenants = new ObservableCollection<RecentTenantInfoDto>(new List<RecentTenantInfoDto>
            {
                new() { MaNguoiThue = 1, HoTen = "Nguyễn Văn A", TienCoc = 1000000m, TrangThai = "Đang ở" },
                new() { MaNguoiThue = 2, HoTen = "Trần Thị B", TienCoc = 1500000m, TrangThai = "Đang ở" },
                new() { MaNguoiThue = 3, HoTen = "Lê Văn C", TienCoc = 1200000m, TrangThai = "Đang ở" }
            });
            RevenueGrowthPercent = 0;
            await Task.CompletedTask;
        }

        private async Task LoadSampleTransactions()
        {
            var sampleTransactions = new List<TransactionHistoryDto>
            {
                new() { MaThanhToan = 1, TenPhong = "P101", TenKhachHang = "Nguyễn Văn A", MoTa = "Thu tiền thuê phòng P101", SoTien = 3500000, ThoiGian = DateTime.Now.AddDays(-5), LoaiGiaoDich = "Thu tiền thuê" },
                new() { MaThanhToan = 2, TenPhong = "P102", TenKhachHang = "Trần Thị B", MoTa = "Thu tiền thuê phòng P102", SoTien = 3200000, ThoiGian = DateTime.Now.AddDays(-3), LoaiGiaoDich = "Thu tiền thuê" },
                new() { MaThanhToan = 3, TenPhong = "P101", TenKhachHang = "Nguyễn Văn A", MoTa = "Chi phí bảo trì điện nước", SoTien = -2000000, ThoiGian = DateTime.Now.AddDays(-1), LoaiGiaoDich = "Chi phí bảo trì" }
            };

            TransactionHistory = new ObservableCollection<TransactionHistoryDto>(sampleTransactions);
            await Task.CompletedTask;
        }

        private async Task CalculateRevenueGrowthPercentByPaidDateAsync()
        {
            try
            {
                if ((TransactionHistory == null || TransactionHistory.Count == 0) && _financialController != null)
                {
                    var transactions = await _financialController.GetTransactionHistoryAsync(null, null);
                    TransactionHistory = new ObservableCollection<TransactionHistoryDto>(transactions);
                }

                var lastTwoMonths = TransactionHistory
                    .GroupBy(t => new { y = t.ThoiGian.Year, m = t.ThoiGian.Month })
                    .Select(g => new { g.Key.y, g.Key.m, Sum = g.Sum(x => x.SoTien) })
                    .OrderByDescending(x => x.y).ThenByDescending(x => x.m)
                    .Take(2)
                    .ToList();

                var year = DateTime.Now.Year;
                var paidByMonth = TransactionHistory
                    .Where(t => t.ThoiGian.Year == year)
                    .GroupBy(t => t.ThoiGian.Month)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.SoTien));

                var list = new List<MonthlyStatsDto>();
                for (int m = 1; m <= 12; m++)
                {
                    paidByMonth.TryGetValue(m, out var sum);
                    list.Add(new MonthlyStatsDto
                    {
                        ThangNam = $"{m.ToString().PadLeft(2, '0')}/{year}",
                        ThuNhap = sum,
                        ChiPhi = 0,
                        LoiNhuan = sum
                    });
                }
                PaidMonthlyStats = new ObservableCollection<MonthlyStatsDto>(list);

                if (lastTwoMonths.Count >= 1)
                {
                    LatestPaidMonthRevenue = lastTwoMonths[0].Sum;
                    LatestPaidMonthLabel = lastTwoMonths[0].m.ToString().PadLeft(2, '0');
                }
                else
                {
                    LatestPaidMonthRevenue = 0;
                    LatestPaidMonthLabel = DateTime.Now.Month.ToString().PadLeft(2, '0');
                }

                if (lastTwoMonths.Count < 2)
                {
                    RevenueGrowthPercent = 0;
                    return;
                }

                var current = lastTwoMonths[0].Sum;
                var previous = lastTwoMonths[1].Sum;

                RevenueGrowthPercent = previous == 0 ? 0 : ((current - previous) / previous) * 100m;
            }
            catch
            {
                RevenueGrowthPercent = 0;
            }
        }

        private static string FormatGrowthText(decimal percent)
        {
            if (percent > 0)
                return $"{percent:0.##}% tăng so với tháng trước";
            if (percent < 0)
                return $"{Math.Abs(percent):0.##}% giảm so với tháng trước";
            return "0% so với tháng trước";
        }

        #endregion

        #region Helper Properties

        public bool HasDebts => Debts.Any();
        public bool HasTransactions => TransactionHistory.Any();
        public int TotalDebtsCount => Debts.Count;
        public int TotalTransactionsCount => TransactionHistory.Count;
        public decimal TotalDebtAmount => Debts.Sum(d => d.TongTien);
        public decimal TotalTransactionAmount => TransactionHistory.Sum(t => t.SoTien);

        #endregion
    }

    // Lớp hỗ trợ cho dữ liệu biểu đồ
    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;

        public void Execute(object? parameter) => _execute((T?)parameter);
    }
}