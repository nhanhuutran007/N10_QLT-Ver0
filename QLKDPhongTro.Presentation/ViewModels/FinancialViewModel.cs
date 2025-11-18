using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Commands;
using QLKDPhongTro.BusinessLayer.Services;
using QLKDPhongTro.Presentation.Views.Windows;
using QLKDPhongTro.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class FinancialViewModel : INotifyPropertyChanged
    {
        private readonly FinancialController? _financialController;
        private readonly DebtProcessingService _debtProcessingService;
        private ObservableCollection<FinancialRecordDto> _financialRecords = new();
        private ObservableCollection<FinancialRecordDto> _allRecords = new();
        private string _searchText = string.Empty;
        private string _selectedSortOption = "Mới nhất";
        private string _selectedFilterType = "Tất cả";
        private string _selectedFilterStatus = "Tất cả";
        private int _currentPage = 1;
        private const int _itemsPerPage = 9;
        private string _currentView = "AllRecords";
        private ObservableCollection<DebtReportDto> _debts = new();
        private FinancialStatsDto? _financialStats;
        private bool _isLoading = false;
        private bool _isFiltering = false;
        private readonly SemaphoreSlim _loadSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _filterSemaphore = new SemaphoreSlim(1, 1);
        private bool _isProcessingGoogleForm = false;
        // Thêm property để lưu trữ thông tin chi tiết sự cố
        private DebtDiscrepancyInfo? _selectedDebtDiscrepancy;
        public DebtDiscrepancyInfo? SelectedDebtDiscrepancy
        {
            get => _selectedDebtDiscrepancy;
            set
            {
                _selectedDebtDiscrepancy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDiscrepancyInfo));
            }
        }

        public bool HasDiscrepancyInfo => SelectedDebtDiscrepancy != null;

        #region Properties

        public ObservableCollection<FinancialRecordDto> FinancialRecords
        {
            get => _financialRecords;
            set
            {
                _financialRecords = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged();
                _currentPage = 1;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(300).ConfigureAwait(false);
                        if (_searchText == value)
                        {
                            await FilterRecordsAsync().ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error filtering records: {ex.Message}");
                    }
                });
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption == value) return;
                _selectedSortOption = value;
                OnPropertyChanged();
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await ApplySortAndFilterAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error applying sort/filter: {ex.Message}");
                    }
                });
            }
        }

        public string SelectedFilterType
        {
            get => _selectedFilterType;
            set
            {
                if (_selectedFilterType == value) return;
                _selectedFilterType = value;
                OnPropertyChanged();
                _currentPage = 1;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await ApplySortAndFilterAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error applying filter: {ex.Message}");
                    }
                });
            }
        }

        public string SelectedFilterStatus
        {
            get => _selectedFilterStatus;
            set
            {
                if (_selectedFilterStatus == value) return;
                _selectedFilterStatus = value;
                OnPropertyChanged();
                _currentPage = 1;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await ApplySortAndFilterAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error applying filter: {ex.Message}");
                    }
                });
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CanGoToPreviousPage));
                OnPropertyChanged(nameof(CanGoToNextPage));
                OnPropertyChanged(nameof(PageInfo));
                _ = ApplySortAndFilterAsync();
            }
        }

        public int TotalPages => (int)Math.Ceiling((double)_allRecords.Count / _itemsPerPage);
        public bool CanGoToPreviousPage => _currentPage > 1;
        public bool CanGoToNextPage => _currentPage < TotalPages;
        public string PageInfo => $"Hiển thị {((_currentPage - 1) * _itemsPerPage) + 1} đến {Math.Min(_currentPage * _itemsPerPage, _allRecords.Count)} trong {_allRecords.Count}";

        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public decimal TotalRevenue { get; private set; }
        public decimal TotalExpense { get; private set; }
        public decimal TotalProfit { get; private set; }
        public decimal TotalDebt { get; private set; }

        public List<int> PageNumbers
        {
            get
            {
                var pages = new List<int>();
                if (TotalPages == 0) return pages;

                var maxPages = Math.Min(TotalPages, 5);
                var startPage = Math.Max(1, _currentPage - 2);
                var endPage = Math.Min(TotalPages, startPage + maxPages - 1);

                for (int i = startPage; i <= endPage; i++)
                {
                    pages.Add(i);
                }
                return pages;
            }
        }

        public string CurrentView
        {
            get => _currentView;
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsAllRecordsView));
                    OnPropertyChanged(nameof(IsDebtsView));
                    OnPropertyChanged(nameof(IsReportsView));
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await LoadViewDataAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error loading view data: {ex.Message}");
                        }
                    });
                }
            }
        }

        public bool IsAllRecordsView => _currentView == "AllRecords";
        public bool IsDebtsView => _currentView == "Debts";
        public bool IsReportsView => _currentView == "Reports";

        public ObservableCollection<DebtReportDto> Debts
        {
            get => _debts;
            set
            {
                _debts = value;
                OnPropertyChanged();
            }
        }

        public FinancialStatsDto? FinancialStats
        {
            get => _financialStats;
            set
            {
                _financialStats = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand ManualInputRentCommand { get; }
        public ICommand ScanImageCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand ViewDetailCommand { get; }
        public ICommand MarkAsPaidCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ProcessDebtsFromGoogleFormCommand { get; }

        // Thêm command mới để xem chi tiết sự cố
        public ICommand InspectDiscrepancyCommand { get; }
        public ICommand ResolveDiscrepancyCommand { get; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<string>? ShowMessageRequested;

        #endregion

        public FinancialViewModel()
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
            }

            _debtProcessingService = new DebtProcessingService();

            // Khởi tạo commands
            ManualInputRentCommand = new RelayCommand(() => { });
            ScanImageCommand = new RelayCommand(() => { });
            LoadDataCommand = new RelayCommand(async () => await LoadDataAsync());
            ViewDetailCommand = new RelayCommand<FinancialRecordDto>(async (record) => await ViewDetailAsync(record));
            MarkAsPaidCommand = new RelayCommand<FinancialRecordDto>(async (record) => await MarkAsPaidAsync(record));
            DeleteCommand = new RelayCommand<FinancialRecordDto>(async (record) => await DeleteAsync(record));
            EditCommand = new RelayCommand<FinancialRecordDto>(async (record) => await EditAsync(record));
            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
            ExportCommand = new RelayCommand(() => { });
            ProcessDebtsFromGoogleFormCommand = new RelayCommand(async () => await ProcessDebtsFromGoogleFormAsync());
            PreviousPageCommand = new RelayCommand(() => { if (CanGoToPreviousPage) CurrentPage--; }, () => CanGoToPreviousPage);
            NextPageCommand = new RelayCommand(() => { if (CanGoToNextPage) CurrentPage++; }, () => CanGoToNextPage);

            // Thêm commands mới
            InspectDiscrepancyCommand = new RelayCommand<DebtReportDto>(async (debt) => await InspectDiscrepancyAsync(debt));
            ResolveDiscrepancyCommand = new RelayCommand(async () => await ResolveDiscrepancyAsync());
        }

        #region Public Methods

        public async Task LoadDataAsync()
        {
            if (!await _loadSemaphore.WaitAsync(0).ConfigureAwait(false))
            {
                return;
            }

            try
            {
                IsLoading = true;

                if (_financialController == null)
                {
                    await LoadSampleData().ConfigureAwait(false);
                    return;
                }

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

                try
                {
                    var payments = await _financialController.GetAllPaymentsAsync().ConfigureAwait(false);
                    cts.Token.ThrowIfCancellationRequested();

                    var records = new ObservableCollection<FinancialRecordDto>();

                    foreach (var payment in payments)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        var record = MapPaymentToFinancialRecord(payment);
                        records.Add(record);
                    }

                    var updateDispatcher = Application.Current?.Dispatcher;
                    if (updateDispatcher != null && !updateDispatcher.CheckAccess())
                    {
                        await updateDispatcher.InvokeAsync(() =>
                        {
                            _allRecords = records;
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                    else
                    {
                        _allRecords = records;
                    }

                    await CalculateStatisticsAsync().ConfigureAwait(false);
                    await ApplySortAndFilterAsync().ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException("Kết nối database quá thời gian chờ. Vui lòng kiểm tra kết nối mạng.");
                }
            }
            catch (TimeoutException ex)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        ShowMessageRequested?.Invoke(this, ex.Message);
                    });
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, ex.Message);
                }
                await LoadSampleData().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        ShowMessageRequested?.Invoke(this, $"Lỗi khi tải dữ liệu: {ex.Message}");
                    });
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, $"Lỗi khi tải dữ liệu: {ex.Message}");
                }
                await LoadSampleData().ConfigureAwait(false);
            }
            finally
            {
                IsLoading = false;
                _loadSemaphore.Release();
            }
        }

        /// <summary>
        /// Xử lý sự cố khi có sai lệch giữa giá trị nhập tay và OCR
        /// </summary>
        private async Task HandleDiscrepancyAsync(DebtCalculationResult debtResult)
        {
            try
            {
                // Service (DebtProcessingService) đã cache thông tin này rồi.
                // ViewModel không cần tạo lại. Nó chỉ cần biết là có lỗi.

                // Tải MeterReadingResult (nếu chưa có) để đảm bảo cache đầy đủ
                if (debtResult.IsDiscrepancy)
                {
                    var info = await _debtProcessingService.GetDiscrepancyInfoAsync(debtResult.RoomName);
                    if (info.MeterReadingResult == null)
                    {
                        info.MeterReadingResult = await GetMeterReadingResultAsync(debtResult);
                    }
                }

                // Hiển thị thông báo cho admin
                ShowMessageRequested?.Invoke(this,
                    $"Phát hiện sai lệch ở phòng {debtResult.RoomName}: " +
                    $"Nhập tay: {debtResult.ManualValue} | OCR: {debtResult.OcrValue}");
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi xử lý sự cố: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy kết quả đọc chỉ số từ ảnh
        /// </summary>
        private async Task<MeterReadingResult> GetMeterReadingResultAsync(DebtCalculationResult debtResult)
        {
            if (!string.IsNullOrEmpty(debtResult.OriginalImagePath))
            {
                var ocrService = new OcrService();
                return await ocrService.AnalyzeImageAsync(debtResult.OriginalImagePath, MeterType.Electricity);
            }

            // Fallback nếu không có ảnh (mặc dù logic sự cố yêu cầu phải có ảnh)
            return new MeterReadingResult
            {
                Value = debtResult.OcrValue,
                RawText = debtResult.OcrValue.ToString(),
                Confidence = 0.5f // Giả định độ tin cậy trung bình
            };
        }

        /// <summary>
        /// Mở cửa sổ kiểm tra chi tiết sự cố
        /// </summary>
        public async Task OpenDiscrepancyInspectionAsync(DebtReportDto debt)
        {
            // FIX: Kiểm tra 'TrangThaiThanhToan'
            if (debt == null || debt.TrangThaiThanhToan != "Cảnh báo") return;

            try
            {
                IsLoading = true;

                // Lấy thông tin chi tiết sự cố
                // FIX: Dùng 'TenPhong'
                var discrepancyInfo = await _debtProcessingService.GetDiscrepancyInfoAsync(debt.TenPhong);
                if (discrepancyInfo != null)
                {
                    // Tạo ViewModel cho cửa sổ kiểm tra
                    var inspectVM = new MeterReadingInspectionViewModel(
                        discrepancyInfo.MeterReadingResult as MeterReadingResult,
                        discrepancyInfo.OriginalImagePath);

                    // Mở cửa sổ kiểm tra
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var inspectionWindow = new MeterReadingInspectionWindow(inspectVM)
                        {
                            Owner = Application.Current.MainWindow,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };

                        // Xử lý khi admin xác nhận
                        inspectionWindow.Closed += async (s, e) =>
                        {
                            if (inspectVM.IsConfirmed)
                            {
                                await ResolveDiscrepancyWithConfirmedValue(debt, inspectVM.FinalValue);
                            }
                        };

                        inspectionWindow.ShowDialog();
                    });
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, "Không tìm thấy thông tin chi tiết sự cố.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi mở cửa sổ kiểm tra: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Giải quyết sự cố với giá trị đã xác nhận
        /// </summary>
        private async Task ResolveDiscrepancyWithConfirmedValue(DebtReportDto debt, decimal confirmedValue)
        {
            try
            {
                IsLoading = true;

                // FIX: Dùng 'TenPhong'
                var result = await _debtProcessingService.ResolveDiscrepancyAsync(debt.TenPhong, confirmedValue);
                if (result.IsSuccess)
                {
                    ShowMessageRequested?.Invoke(this,
                        $"Đã giải quyết sự cố cho phòng {debt.TenPhong}. Giá trị xác nhận: {confirmedValue}");

                    // Cập nhật UI
                    debt.TrangThaiThanhToan = "Đã xác nhận";
                    debt.GhiChu = $"Đã xác nhận: {confirmedValue}";
                    debt.ComparisonInfo = $"Xác nhận: {confirmedValue}";

                    // Refresh dữ liệu
                    await LoadViewDataAsync();
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, $"Lỗi khi giải quyết sự cố: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ProcessDebtsFromGoogleFormAsync()
        {
            _isProcessingGoogleForm = true;
            try
            {
                IsLoading = true;
                ShowMessageRequested?.Invoke(this, "Đang xử lý và đối chiếu dữ liệu từ Google Form...");

                // Xử lý công nợ từ Google Form
                var results = await _debtProcessingService.ProcessDebtsAsync();

                // Phân loại kết quả
                var validDebts = results.Where(r => r.IsProcessed && !r.IsDiscrepancy).ToList();
                var discrepancyDebts = results.Where(r => r.IsProcessed && r.IsDiscrepancy).ToList();
                var errorDebts = results.Where(r => !r.IsProcessed).ToList();

                // Xử lý các trường hợp có sai lệch
                foreach (var discrepancy in discrepancyDebts)
                {
                    await HandleDiscrepancyAsync(discrepancy);
                }

                // Tạo danh sách báo cáo cho UI
                var reportList = new ObservableCollection<DebtReportDto>();

                // 1. Thêm các khoản có sai lệch (Ưu tiên hiển thị)
                foreach (var item in discrepancyDebts)
                {
                    reportList.Add(new DebtReportDto
                    {
                        TenPhong = item.RoomName,
                        ThangNam = DateTime.Now.ToString("MM/yyyy"),
                        TongTien = item.TotalDebt,
                        TrangThaiThanhToan = "Cảnh báo",
                        GhiChu = item.WarningNote,
                        SoDienThoai = item.Email, // Giữ nguyên số điện thoại thực từ database
                        ComparisonInfo = $"Nhập tay: {item.ManualValue} | OCR: {item.OcrValue}" // HIỂN THỊ ĐÚNG THÔNG TIN SO SÁNH
                    });
                }

                // 2. Thêm các khoản hợp lệ
                foreach (var item in validDebts)
                {
                    reportList.Add(new DebtReportDto
                    {
                        TenPhong = item.RoomName,
                        ThangNam = DateTime.Now.ToString("MM/yyyy"),
                        TongTien = item.TotalDebt,
                        TrangThaiThanhToan = "Chờ duyệt",
                        GhiChu = "Số liệu khớp",
                        SoDienThoai = item.Email, // Giữ nguyên số điện thoại thực từ database
                        //ComparisonInfo = $"Giá trị: {item.CurrentElectricValue}" // HIỂN THỊ ĐÚNG THÔNG TIN
                        ComparisonInfo = $"Giá trị: {item.OcrValue}"
                    });
                }

                // Cập nhật UI
                Debts = reportList;
                CurrentView = "Debts";

                // Thông báo kết quả
                string summary = $"Xử lý xong Google Form:\n" +
                                 $"✅ {validDebts.Count} phòng khớp số liệu\n" +
                                 $"⚠️ {discrepancyDebts.Count} phòng có sai lệch (Cần kiểm tra)\n" +
                                 $"❌ {errorDebts.Count} lỗi xử lý";

                ShowMessageRequested?.Invoke(this, summary);

                // Tự động tạo payment records cho các bản ghi hợp lệ
                if (validDebts.Count > 0)
                {
                    var createdCount = await _debtProcessingService.CreatePaymentRecordsFromDebtsAsync(validDebts);
                    ShowMessageRequested?.Invoke(this, $"Đã tự động tạo {createdCount} bản ghi thanh toán hợp lệ.");
                }

                // Thông báo về các sự cố cần xử lý
                if (discrepancyDebts.Count > 0)
                {
                    ShowMessageRequested?.Invoke(this,
                        $"Có {discrepancyDebts.Count} sự cố cần kiểm tra. Vui lòng chuyển đến tab 'Quản lý công nợ' để xử lý.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi xử lý Google Form: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                _isProcessingGoogleForm = false;
            }
        }

        // Phương thức kiểm tra sự cố
        private async Task InspectDiscrepancyAsync(DebtReportDto? debt)
        {
            // FIX: Kiểm tra 'TrangThaiThanhToan'
            if (debt == null || debt.TrangThaiThanhToan != "Cảnh báo") return;

            try
            {
                IsLoading = true;

                // Lấy thông tin chi tiết sự cố từ service
                // FIX: Dùng 'TenPhong'
                var discrepancyInfo = await _debtProcessingService.GetDiscrepancyInfoAsync(debt.TenPhong);
                if (discrepancyInfo != null)
                {
                    SelectedDebtDiscrepancy = discrepancyInfo;

                    // Hiển thị cửa sổ kiểm tra chi tiết
                    var inspectVM = new MeterReadingInspectionViewModel(
                        discrepancyInfo.MeterReadingResult as MeterReadingResult,
                        discrepancyInfo.OriginalImagePath);

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var window = new MeterReadingInspectionWindow(inspectVM);
                        window.Owner = Application.Current.MainWindow;

                        // Xử lý khi cửa sổ đóng
                        window.Closed += async (s, e) =>
                        {
                            if (inspectVM.IsConfirmed)
                            {
                                // Cập nhật giá trị đã xác nhận
                                await ResolveDiscrepancyWithValue(debt, inspectVM.FinalValue);
                            }
                        };

                        window.ShowDialog();
                    });
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, "Không tìm thấy thông tin chi tiết sự cố.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi kiểm tra sự cố: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Phương thức giải quyết sự cố với giá trị cụ thể
        private async Task ResolveDiscrepancyWithValue(DebtReportDto debt, decimal confirmedValue)
        {
            try
            {
                IsLoading = true;

                // FIX: Dùng 'TenPhong'
                var result = await _debtProcessingService.ResolveDiscrepancyAsync(debt.TenPhong, confirmedValue);
                if (result.IsSuccess)
                {
                    ShowMessageRequested?.Invoke(this, $"Đã giải quyết sự cố cho phòng {debt.TenPhong} với giá trị {confirmedValue}");

                    // Cập nhật UI
                    debt.TrangThaiThanhToan = "Đã xác nhận";
                    debt.GhiChu = $"Đã xác nhận: {confirmedValue}";
                    debt.ComparisonInfo = $"Xác nhận: {confirmedValue}";

                    // Refresh data
                    await LoadViewDataAsync();

                    SelectedDebtDiscrepancy = null;
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, $"Lỗi khi giải quyết sự cố: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Phương thức giải quyết sự cố từ panel thông tin
        private async Task ResolveDiscrepancyAsync()
        {
            if (SelectedDebtDiscrepancy == null) return;

            try
            {
                IsLoading = true;

                // Sử dụng giá trị đã xác nhận hoặc giá trị thủ công làm mặc định
                decimal valueToUse = SelectedDebtDiscrepancy.ConfirmedValue > 0 ?
                    SelectedDebtDiscrepancy.ConfirmedValue : SelectedDebtDiscrepancy.ManualValue;

                var result = await _debtProcessingService.ResolveDiscrepancyAsync(
                    SelectedDebtDiscrepancy.RoomName, valueToUse);

                if (result.IsSuccess)
                {
                    ShowMessageRequested?.Invoke(this, "Đã giải quyết sự cố thành công!");

                    // Cập nhật lại danh sách
                    await LoadViewDataAsync();

                    // Reset selected discrepancy
                    SelectedDebtDiscrepancy = null;
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, $"Lỗi khi giải quyết sự cố: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private Task FilterRecordsAsync()
        {
            return ApplySortAndFilterAsync();
        }

        private async Task ApplySortAndFilterAsync()
        {
            if (!await _filterSemaphore.WaitAsync(0).ConfigureAwait(false))
            {
                return;
            }

            try
            {
                _isFiltering = true;
                var filtered = _allRecords.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    filtered = filtered.Where(r =>
                        r.TenPhong.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        r.LoaiGiaoDich.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        r.ChiTietGiaoDich.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        r.ThangNam.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }

                if (_selectedFilterType != "Tất cả")
                {
                    filtered = filtered.Where(r => r.LoaiGiaoDich == _selectedFilterType);
                }

                if (_selectedFilterStatus != "Tất cả")
                {
                    filtered = filtered.Where(r => r.TrangThaiText == _selectedFilterStatus);
                }

                filtered = _selectedSortOption switch
                {
                    "Mới nhất" => filtered.OrderByDescending(r => r.KyHan),
                    "Cũ nhất" => filtered.OrderBy(r => r.KyHan),
                    "Theo Số tiền (Cao - Thấp)" => filtered.OrderByDescending(r => r.TongTien),
                    "Theo Số tiền (Thấp - Cao)" => filtered.OrderBy(r => r.TongTien),
                    _ => filtered.OrderByDescending(r => r.KyHan)
                };

                var filteredList = filtered.ToList();

                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        try
                        {
                            _allRecords = new ObservableCollection<FinancialRecordDto>(filteredList);

                            var pagedRecords = filteredList
                                .Skip((_currentPage - 1) * _itemsPerPage)
                                .Take(_itemsPerPage)
                                .ToList();

                            FinancialRecords = new ObservableCollection<FinancialRecordDto>(pagedRecords);

                            OnPropertyChanged(nameof(TotalPages));
                            OnPropertyChanged(nameof(CanGoToPreviousPage));
                            OnPropertyChanged(nameof(CanGoToNextPage));
                            OnPropertyChanged(nameof(PageInfo));
                            OnPropertyChanged(nameof(PageNumbers));
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error updating UI in ApplySortAndFilterAsync: {ex.Message}");
                        }
                    }, System.Windows.Threading.DispatcherPriority.Background);
                }
                else
                {
                    try
                    {
                        _allRecords = new ObservableCollection<FinancialRecordDto>(filteredList);

                        var pagedRecords = filteredList
                            .Skip((_currentPage - 1) * _itemsPerPage)
                            .Take(_itemsPerPage)
                            .ToList();

                        FinancialRecords = new ObservableCollection<FinancialRecordDto>(pagedRecords);

                        OnPropertyChanged(nameof(TotalPages));
                        OnPropertyChanged(nameof(CanGoToPreviousPage));
                        OnPropertyChanged(nameof(CanGoToNextPage));
                        OnPropertyChanged(nameof(PageInfo));
                        OnPropertyChanged(nameof(PageNumbers));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error updating UI in ApplySortAndFilterAsync: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi lọc/sắp xếp: {ex.Message}");
            }
            finally
            {
                _isFiltering = false;
                _filterSemaphore.Release();
            }
        }

        private async Task ViewDetailAsync(FinancialRecordDto? record)
        {
            if (record == null)
            {
                ShowMessageRequested?.Invoke(this, "Không có dữ liệu để xem chi tiết.");
                return;
            }

            try
            {
                if (_financialController != null)
                {
                    var invoiceData = await _financialController.GetInvoiceDetailAsync(record.MaThanhToan).ConfigureAwait(false);
                    if (invoiceData != null)
                    {
                        var dispatcher = Application.Current?.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            await dispatcher.InvokeAsync(() =>
                            {
                                var invoiceWindow = new InvoiceDetailView(invoiceData)
                                {
                                    Owner = Application.Current?.MainWindow
                                };
                                invoiceWindow.ShowDialog();
                            });
                        }
                        else
                        {
                            var invoiceWindow = new InvoiceDetailView(invoiceData)
                            {
                                Owner = Application.Current?.MainWindow
                            };
                            invoiceWindow.ShowDialog();
                        }
                    }
                    else
                    {
                        ShowMessageRequested?.Invoke(this, "Không tìm thấy thông tin chi tiết.");
                    }
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi xem chi tiết: {ex.Message}");
            }
        }

        private async Task MarkAsPaidAsync(FinancialRecordDto? record)
        {
            if (record == null)
            {
                ShowMessageRequested?.Invoke(this, "Không có dữ liệu để cập nhật.");
                return;
            }

            if (_financialController == null)
            {
                ShowMessageRequested?.Invoke(this, "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau.");
                return;
            }

            var dispatcher = Application.Current?.Dispatcher;
            MessageBoxResult result;

            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                result = await dispatcher.InvokeAsync(() => MessageBox.Show(
                    $"Bạn có chắc chắn muốn đánh dấu thanh toán này là 'Đã trả'?\n\nPhòng: {record.TenPhong}\nSố tiền: {record.TongTien:N0} VNĐ",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question));
            }
            else
            {
                result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn đánh dấu thanh toán này là 'Đã trả'?\n\nPhòng: {record.TenPhong}\nSố tiền: {record.TongTien:N0} VNĐ",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
            }

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var validationResult = await _financialController.UpdatePaymentStatusAsync(record.MaThanhToan, "Đã trả").ConfigureAwait(false);
                    if (validationResult.IsValid)
                    {
                        ShowMessageRequested?.Invoke(this, validationResult.Message);
                        await LoadDataAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        ShowMessageRequested?.Invoke(this, validationResult.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageRequested?.Invoke(this, $"Lỗi khi cập nhật: {ex.Message}");
                }
            }
        }

        private async Task DeleteAsync(FinancialRecordDto? record)
        {
            if (record == null)
            {
                ShowMessageRequested?.Invoke(this, "Không có dữ liệu để xóa.");
                return;
            }

            if (_financialController == null)
            {
                ShowMessageRequested?.Invoke(this, "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau.");
                return;
            }

            var dispatcher = Application.Current?.Dispatcher;
            MessageBoxResult result;

            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                result = await dispatcher.InvokeAsync(() => MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa thanh toán này?\n\nPhòng: {record.TenPhong}\nSố tiền: {record.TongTien:N0} VNĐ\n\nHành động này không thể hoàn tác!",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning));
            }
            else
            {
                result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa thanh toán này?\n\nPhòng: {record.TenPhong}\nSố tiền: {record.TongTien:N0} VNĐ\n\nHành động này không thể hoàn tác!",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
            }

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var validationResult = await _financialController.DeletePaymentAsync(record.MaThanhToan).ConfigureAwait(false);
                    if (validationResult.IsValid)
                    {
                        ShowMessageRequested?.Invoke(this, validationResult.Message);
                        await LoadDataAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        ShowMessageRequested?.Invoke(this, validationResult.Message);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageRequested?.Invoke(this, $"Lỗi khi xóa: {ex.Message}");
                }
            }
        }

        private async Task EditAsync(FinancialRecordDto? record)
        {
            if (record == null)
            {
                ShowMessageRequested?.Invoke(this, "Không có dữ liệu để chỉnh sửa.");
                return;
            }

            // FIX: Đã loại bỏ nhánh logic xử lý "Cảnh báo"
            // EditCommand chỉ dành cho FinancialRecordDto

            // Logic cũ cho các trường hợp khác
            if (_financialController == null)
            {
                ShowMessageRequested?.Invoke(this, "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau.");
                return;
            }

            try
            {
                var payment = await _financialController.GetPaymentByIdAsync(record.MaThanhToan).ConfigureAwait(false);
                if (payment != null)
                {
                    var dispatcher = Application.Current?.Dispatcher;
                    bool? dialogResult = false;

                    if (dispatcher != null && !dispatcher.CheckAccess())
                    {
                        dialogResult = await dispatcher.InvokeAsync(() =>
                        {
                            var editDialog = new ManualInputView();
                            editDialog.Owner = Application.Current?.MainWindow;
                            return editDialog.ShowDialog();
                        });
                    }
                    else
                    {
                        var editDialog = new ManualInputView();
                        editDialog.Owner = Application.Current?.MainWindow;
                        dialogResult = editDialog.ShowDialog();
                    }

                    if (dialogResult == true)
                    {
                        await LoadDataAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, "Không tìm thấy thông tin thanh toán để chỉnh sửa.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi mở form chỉnh sửa: {ex.Message}");
            }
        }

        // FIX: Đã loại bỏ phương thức EditDebtWarningAsync vì logic của nó bị sai
        // (trộn lẫn DTO) và chức năng của nó đã được xử lý bởi InspectDiscrepancyAsync

        private async Task CalculateStatisticsAsync()
        {
            try
            {
                decimal revenue = 0, expense = 0, debt = 0, profit = 0;
                FinancialStatsDto? stats = null;

                if (_financialController == null)
                {
                    revenue = _allRecords.Where(r => r.TrangThaiThanhToan == "Đã trả").Sum(r => r.TongTien);
                    expense = _allRecords.Sum(r => r.TongTien) - revenue;
                    debt = _allRecords.Where(r => r.TrangThaiThanhToan == "Chưa trả").Sum(r => r.TongTien);
                    profit = revenue - expense;
                }
                else
                {
                    stats = await _financialController.GetFinancialStatsAsync().ConfigureAwait(false);
                    revenue = stats.TongThuNhap;
                    expense = stats.TongChiPhi;
                    debt = stats.TongCongNo;
                    profit = stats.LoiNhuan;
                }

                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        TotalRevenue = revenue;
                        TotalExpense = expense;
                        TotalDebt = debt;
                        TotalProfit = profit;
                        if (stats != null)
                        {
                            FinancialStats = stats;
                        }

                        OnPropertyChanged(nameof(TotalRevenue));
                        OnPropertyChanged(nameof(TotalExpense));
                        OnPropertyChanged(nameof(TotalProfit));
                        OnPropertyChanged(nameof(TotalDebt));
                    });
                }
                else
                {
                    TotalRevenue = revenue;
                    TotalExpense = expense;
                    TotalDebt = debt;
                    TotalProfit = profit;
                    if (stats != null)
                    {
                        FinancialStats = stats;
                    }

                    OnPropertyChanged(nameof(TotalRevenue));
                    OnPropertyChanged(nameof(TotalExpense));
                    OnPropertyChanged(nameof(TotalProfit));
                    OnPropertyChanged(nameof(TotalDebt));
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi tính thống kê: {ex.Message}");
            }
        }

        private async Task LoadViewDataAsync()
        {
            if (_isProcessingGoogleForm && _currentView == "Debts")
            {
                return;
            }
            try
            {
                if (_currentView == "Debts")
                {
                    if (_financialController != null)
                    {
                        var debts = await _financialController.GetDebtReportAsync().ConfigureAwait(false);
                        var dispatcher = Application.Current?.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            await dispatcher.InvokeAsync(() =>
                            {
                                Debts = new ObservableCollection<DebtReportDto>(debts);
                            });
                        }
                        else
                        {
                            Debts = new ObservableCollection<DebtReportDto>(debts);
                        }
                    }
                    else
                    {
                        var dispatcher = Application.Current?.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            await dispatcher.InvokeAsync(() =>
                            {
                                Debts = new ObservableCollection<DebtReportDto>();
                            });
                        }
                        else
                        {
                            Debts = new ObservableCollection<DebtReportDto>();
                        }
                    }
                }
                else if (_currentView == "Reports")
                {
                    await CalculateStatisticsAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }

        #endregion

        #region Private Helpers

        private FinancialRecordDto MapPaymentToFinancialRecord(PaymentDto payment)
        {
            string loaiGiaoDich = "Tiền Thuê";
            string chiTiet = $"Tiền thuê: {payment.TienThue:N0} VNĐ";

            if (payment.TienDien > 0 || payment.TienNuoc > 0)
            {
                loaiGiaoDich = "Chỉ Số Điện/Nước";
                chiTiet = $"Điện: {payment.TienDien:N0} VNĐ, Nước: {payment.TienNuoc:N0} VNĐ";
            }
            else if (payment.ChiPhiKhac > 0 || payment.TienInternet > 0 || payment.TienVeSinh > 0 || payment.TienGiuXe > 0)
            {
                loaiGiaoDich = "Chi Phí";
                var chiPhiList = new List<string>();
                if (payment.TienInternet > 0) chiPhiList.Add($"Internet: {payment.TienInternet:N0}");
                if (payment.TienVeSinh > 0) chiPhiList.Add($"Vệ sinh: {payment.TienVeSinh:N0}");
                if (payment.TienGiuXe > 0) chiPhiList.Add($"Giữ xe: {payment.TienGiuXe:N0}");
                if (payment.ChiPhiKhac > 0) chiPhiList.Add($"Khác: {payment.ChiPhiKhac:N0}");
                chiTiet = string.Join(", ", chiPhiList);
            }

            string trangThai = "BinhThuong";
            string trangThaiText = "Bình thường";
            string trangThaiColor = "#7D8FA9";
            double progressValue = 0;

            if (payment.TrangThaiThanhToan == "Đã trả")
            {
                trangThai = "HoanThanh";
                trangThaiText = "Hoàn thành";
                trangThaiColor = "#319DFF";
                progressValue = 100;
            }
            else if (payment.TrangThaiThanhToan == "Chưa trả")
            {
                if (!string.IsNullOrEmpty(payment.ThangNam))
                {
                    if (TryParseThangNam(payment.ThangNam, out DateTime kyHan))
                    {
                        var monthsOverdue = (DateTime.Now.Year - kyHan.Year) * 12 + (DateTime.Now.Month - kyHan.Month);
                        if (monthsOverdue > 1)
                        {
                            trangThai = "QuaHan";
                            trangThaiText = "Quá hạn";
                            trangThaiColor = "#FF0000";
                            progressValue = 0;
                        }
                        else if (monthsOverdue == 1)
                        {
                            trangThai = "CanhBao";
                            trangThaiText = "Cảnh báo";
                            trangThaiColor = "#FF316A";
                            progressValue = 50;
                        }
                        else
                        {
                            progressValue = 75;
                        }
                    }
                }
            }

            DateTime kyHanDate = DateTime.Now;
            if (!string.IsNullOrEmpty(payment.ThangNam))
            {
                if (!TryParseThangNam(payment.ThangNam, out kyHanDate))
                {
                    kyHanDate = DateTime.Now;
                }
            }

            return new FinancialRecordDto
            {
                MaThanhToan = payment.MaThanhToan,
                MaHopDong = payment.MaHopDong ?? 0,
                TenPhong = payment.TenPhong ?? "Không xác định",
                KyHan = kyHanDate,
                LoaiGiaoDich = loaiGiaoDich,
                TongTien = payment.TongTien,
                ChiTietGiaoDich = chiTiet,
                TrangThaiText = trangThaiText,
                TrangThaiColor = trangThaiColor,
                ProgressValue = progressValue,
                TrangThai = trangThai,
                ThangNam = payment.ThangNam,
                TrangThaiThanhToan = payment.TrangThaiThanhToan,
                NgayThanhToan = payment.NgayThanhToan
            };
        }

        private static bool TryParseThangNam(string thangNam, out DateTime result)
        {
            result = DateTime.Now;
            if (string.IsNullOrEmpty(thangNam) || thangNam.Length != 7)
                return false;

            var parts = thangNam.Split('/');
            if (parts.Length != 2)
                return false;

            if (int.TryParse(parts[0], out int month) && int.TryParse(parts[1], out int year))
            {
                if (month >= 1 && month <= 12 && year >= 2000 && year <= 2100)
                {
                    result = new DateTime(year, month, 1);
                    return true;
                }
            }

            return false;
        }

        private async Task LoadSampleData()
        {
            var dispatcher = Application.Current?.Dispatcher;

            var sampleRecords = new ObservableCollection<FinancialRecordDto>
            {
                new()
                {
                    MaThanhToan = 1,
                    TenPhong = "P101",
                    KyHan = new DateTime(2024, 11, 1),
                    LoaiGiaoDich = "Tiền Thuê",
                    TongTien = 3500000,
                    ChiTietGiaoDich = "Tiền thuê: 3.500.000 VNĐ",
                    TrangThaiText = "Hoàn thành",
                    TrangThaiColor = "#319DFF",
                    ProgressValue = 100,
                    TrangThai = "HoanThanh",
                    ThangNam = "11/2024",
                    TrangThaiThanhToan = "Đã trả"
                },
                new()
                {
                    MaThanhToan = 2,
                    TenPhong = "P102",
                    KyHan = new DateTime(2024, 11, 1),
                    LoaiGiaoDich = "Chỉ Số Điện/Nước",
                    TongTien = 500000,
                    ChiTietGiaoDich = "Điện: 300.000 VNĐ, Nước: 200.000 VNĐ",
                    TrangThaiText = "Cảnh báo",
                    TrangThaiColor = "#FF316A",
                    ProgressValue = 50,
                    TrangThai = "CanhBao",
                    ThangNam = "11/2024",
                    TrangThaiThanhToan = "Chưa trả"
                },
                new()
                {
                    MaThanhToan = 3,
                    TenPhong = "A101",
                    KyHan = new DateTime(2024, 10, 1),
                    LoaiGiaoDich = "Chi Phí",
                    TongTien = 200000,
                    ChiTietGiaoDich = "Internet: 100.000, Vệ sinh: 50.000, Giữ xe: 50.000",
                    TrangThaiText = "Quá hạn",
                    TrangThaiColor = "#FF0000",
                    ProgressValue = 0,
                    TrangThai = "QuaHan",
                    ThangNam = "10/2024",
                    TrangThaiThanhToan = "Chưa trả"
                }
            };

            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                await dispatcher.InvokeAsync(() =>
                {
                    _allRecords = sampleRecords;
                });
            }
            else
            {
                _allRecords = sampleRecords;
            }

            await ApplySortAndFilterAsync().ConfigureAwait(false);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    // FIX: Đã xóa class 'DebtDiscrepancyInfo' trùng lặp.
    // ViewModel sẽ sử dụng class được định nghĩa trong DebtProcessingService.cs
}