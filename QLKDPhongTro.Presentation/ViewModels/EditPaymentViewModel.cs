using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Commands;
using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DataLayer = QLKDPhongTro.DataLayer;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class EditPaymentViewModel : INotifyPropertyChanged
    {
        private readonly FinancialController? _financialController;
        private readonly int _maThanhToan;
        private PaymentDto? _originalPayment;

        private decimal _tienThue;
        private decimal _tienDien;
        private decimal _tienNuoc;
        private decimal _tienInternet;
        private decimal _tienVeSinh;
        private decimal _tienGiuXe;
        private decimal _chiPhiKhac;
        private decimal? _soDienThangTruoc;
        private decimal? _soDienThangNay;
        private bool _isLoading;

        #region Properties

        public int MaThanhToan => _maThanhToan;
        public string TenPhong { get; private set; } = string.Empty;
        public string ThangNam { get; private set; } = string.Empty;

        public decimal TienThue
        {
            get => _tienThue;
            set
            {
                _tienThue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TongTien));
            }
        }

        public decimal TienDien
        {
            get => _tienDien;
            set
            {
                _tienDien = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TongTien));
            }
        }

        public decimal TienNuoc
        {
            get => _tienNuoc;
            set
            {
                _tienNuoc = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TongTien));
            }
        }

        public decimal TienInternet
        {
            get => _tienInternet;
            set
            {
                _tienInternet = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TongTien));
            }
        }

        public decimal TienVeSinh
        {
            get => _tienVeSinh;
            set
            {
                _tienVeSinh = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TongTien));
            }
        }

        public decimal TienGiuXe
        {
            get => _tienGiuXe;
            set
            {
                _tienGiuXe = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TongTien));
            }
        }

        public decimal ChiPhiKhac
        {
            get => _chiPhiKhac;
            set
            {
                _chiPhiKhac = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TongTien));
            }
        }

        public decimal? SoDienThangTruoc
        {
            get => _soDienThangTruoc;
            set
            {
                _soDienThangTruoc = value;
                OnPropertyChanged();
                RecalculateElectricity();
            }
        }

        public decimal? SoDienThangNay
        {
            get => _soDienThangNay;
            set
            {
                _soDienThangNay = value;
                OnPropertyChanged();
                RecalculateElectricity();
            }
        }

        public decimal TongTien => TienThue + TienDien + TienNuoc + TienInternet + TienVeSinh + TienGiuXe + ChiPhiKhac;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<bool>? CloseRequested;
        public event EventHandler<string>? ShowMessageRequested;

        #endregion

        public EditPaymentViewModel(int maThanhToan)
        {
            _maThanhToan = maThanhToan;

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
                ShowMessageRequested?.Invoke(this, $"Không thể kết nối database: {ex.Message}");
            }

            SaveCommand = new RelayCommand(async () => await SaveAsync());
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(this, false));

            _ = LoadPaymentDataAsync();
        }

        private async Task LoadPaymentDataAsync()
        {
            if (_financialController == null)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        ShowMessageRequested?.Invoke(this, "Không thể kết nối đến cơ sở dữ liệu.");
                    });
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, "Không thể kết nối đến cơ sở dữ liệu.");
                }
                return;
            }

            try
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() => { IsLoading = true; });
                }
                else
                {
                    IsLoading = true;
                }

                var payment = await _financialController.GetPaymentByIdAsync(_maThanhToan).ConfigureAwait(false);
                if (payment == null)
                {
                    if (dispatcher != null && !dispatcher.CheckAccess())
                    {
                        await dispatcher.InvokeAsync(() =>
                        {
                            ShowMessageRequested?.Invoke(this, "Không tìm thấy thông tin thanh toán.");
                            CloseRequested?.Invoke(this, false);
                        });
                    }
                    else
                    {
                        ShowMessageRequested?.Invoke(this, "Không tìm thấy thông tin thanh toán.");
                        CloseRequested?.Invoke(this, false);
                    }
                    return;
                }

                _originalPayment = payment;

                // Load chỉ số điện từ Payment entity trực tiếp
                var paymentRepository = new PaymentRepository();
                var paymentEntity = await paymentRepository.GetByIdAsync(_maThanhToan).ConfigureAwait(false);

                // Load data vào UI - phải chạy trên UI thread
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        LoadPaymentToUI(payment, paymentEntity);
                        IsLoading = false;
                    });
                }
                else
                {
                    LoadPaymentToUI(payment, paymentEntity);
                    IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        ShowMessageRequested?.Invoke(this, $"Lỗi khi tải dữ liệu: {ex.Message}");
                        IsLoading = false;
                    });
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, $"Lỗi khi tải dữ liệu: {ex.Message}");
                    IsLoading = false;
                }
            }
        }

        private void LoadPaymentToUI(PaymentDto payment, DataLayer.Models.Payment? paymentEntity)
        {
            TenPhong = payment.TenPhong ?? "Không xác định";
            ThangNam = payment.ThangNam ?? DateTime.Now.ToString("MM/yyyy");

            TienThue = payment.TienThue ?? 0;
            TienDien = payment.TienDien ?? 0;
            TienNuoc = payment.TienNuoc ?? 0;
            TienInternet = payment.TienInternet ?? 0;
            TienVeSinh = payment.TienVeSinh ?? 0;
            TienGiuXe = payment.TienGiuXe ?? 0;
            ChiPhiKhac = payment.ChiPhiKhac ?? 0;

            // Load chỉ số điện từ entity
            if (paymentEntity != null)
            {
                SoDienThangTruoc = paymentEntity.ChiSoDienCu;
                SoDienThangNay = paymentEntity.ChiSoDienMoi;
            }

            OnPropertyChanged(nameof(TenPhong));
            OnPropertyChanged(nameof(ThangNam));
        }

        private void RecalculateElectricity()
        {
            if (SoDienThangNay.HasValue && SoDienThangTruoc.HasValue)
            {
                decimal soDienTieuThu = SoDienThangNay.Value - SoDienThangTruoc.Value;
                if (soDienTieuThu >= 0)
                {
                    // Sử dụng đơn giá mặc định 3500 VND/kWh
                    TienDien = soDienTieuThu * 3500m;
                }
            }
        }

        private async Task SaveAsync()
        {
            var dispatcher = Application.Current?.Dispatcher;
            
            if (_financialController == null)
            {
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        ShowMessageRequested?.Invoke(this, "Không thể kết nối đến cơ sở dữ liệu.");
                    });
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, "Không thể kết nối đến cơ sở dữ liệu.");
                }
                return;
            }

            try
            {
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() => { IsLoading = true; });
                }
                else
                {
                    IsLoading = true;
                }

                // Sử dụng UpdateInvoiceUnitPricesAsync để cập nhật payment
                var success = await _financialController.UpdateInvoiceUnitPricesAsync(
                    _maThanhToan,
                    SoDienThangTruoc,
                    SoDienThangNay,
                    TienThue,
                    TienInternet,
                    TienVeSinh,
                    TienGiuXe,
                    ChiPhiKhac
                ).ConfigureAwait(false);

                // Tất cả các thao tác UI phải chạy trên UI thread
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        if (success)
                        {
                            ShowMessageRequested?.Invoke(this, "Cập nhật thanh toán thành công!");
                            CloseRequested?.Invoke(this, true);
                        }
                        else
                        {
                            ShowMessageRequested?.Invoke(this, "Cập nhật thanh toán thất bại.");
                        }
                        IsLoading = false;
                    });
                }
                else
                {
                    if (success)
                    {
                        ShowMessageRequested?.Invoke(this, "Cập nhật thanh toán thành công!");
                        CloseRequested?.Invoke(this, true);
                    }
                    else
                    {
                        ShowMessageRequested?.Invoke(this, "Cập nhật thanh toán thất bại.");
                    }
                    IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    await dispatcher.InvokeAsync(() =>
                    {
                        ShowMessageRequested?.Invoke(this, $"Lỗi khi lưu: {ex.Message}");
                        IsLoading = false;
                    });
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, $"Lỗi khi lưu: {ex.Message}");
                    IsLoading = false;
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

