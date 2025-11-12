using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class ManualInputViewModel : INotifyPropertyChanged
    {
        private readonly FinancialController? _financialController;
        private int? _selectedContractId;
        private string _selectedRoomName = string.Empty;
        private decimal _electricityUnitPrice;
        private decimal _electricityQuantity;
        private decimal _waterUnitPrice;
        private decimal _waterQuantity;
        private decimal _amountPaid;
        private decimal _rentAmount;

        #region Properties

        public ObservableCollection<Contract> AvailableContracts { get; } = new();
        public ObservableCollection<string> RoomNames { get; } = new();

        public int? SelectedContractId
        {
            get => _selectedContractId;
            set
            {
                _selectedContractId = value;
                OnPropertyChanged();
                UpdateRoomInfo();
            }
        }

        public string SelectedRoomName
        {
            get => _selectedRoomName;
            set
            {
                _selectedRoomName = value;
                OnPropertyChanged();
            }
        }

        public decimal ElectricityUnitPrice
        {
            get => _electricityUnitPrice;
            set
            {
                _electricityUnitPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ElectricitySubtotal));
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal ElectricityQuantity
        {
            get => _electricityQuantity;
            set
            {
                _electricityQuantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ElectricitySubtotal));
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal ElectricitySubtotal => ElectricityUnitPrice * ElectricityQuantity;

        public decimal WaterUnitPrice
        {
            get => _waterUnitPrice;
            set
            {
                _waterUnitPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WaterSubtotal));
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal WaterQuantity
        {
            get => _waterQuantity;
            set
            {
                _waterQuantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WaterSubtotal));
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal WaterSubtotal => WaterUnitPrice * WaterQuantity;

        public decimal RentAmount
        {
            get => _rentAmount;
            set
            {
                _rentAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal AmountPaid
        {
            get => _amountPaid;
            set
            {
                _amountPaid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public decimal Subtotal => RentAmount + ElectricitySubtotal + WaterSubtotal;
        public decimal Total => Subtotal - AmountPaid;

        public string CurrentMonth => DateTime.Now.ToString("MM/yyyy");

        #endregion

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadContractsCommand { get; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<bool>? CloseRequested;
        public event EventHandler<string>? ShowMessageRequested;

        #endregion

        public ManualInputViewModel()
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
                ShowMessageRequested?.Invoke(this, $"Không thể kết nối database: {ex.Message}");
            }

            SaveCommand = new RelayCommand(async () => await SaveAsync());
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(this, false));
            LoadContractsCommand = new RelayCommand(async () => await LoadContractsAsync());

            _ = LoadContractsAsync();
        }

        #region Public Methods

        public async Task LoadContractsAsync()
        {
            try
            {
                if (_financialController == null)
                {
                    LoadSampleContracts();
                    return;
                }

                var contracts = await _financialController.GetActiveContractsAsync();
                AvailableContracts.Clear();
                foreach (var contract in contracts)
                {
                    AvailableContracts.Add(contract);
                }

                if (AvailableContracts.Any())
                {
                    SelectedContractId = AvailableContracts.First().MaHopDong;
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi tải danh sách hợp đồng: {ex.Message}");
                LoadSampleContracts();
            }
        }

        public async Task SaveAsync()
        {
            if (!SelectedContractId.HasValue)
            {
                ShowMessageRequested?.Invoke(this, "Vui lòng chọn hợp đồng");
                return;
            }

            if (RentAmount <= 0)
            {
                ShowMessageRequested?.Invoke(this, "Vui lòng nhập tiền thuê");
                return;
            }

            try
            {
                if (_financialController == null)
                {
                    ShowMessageRequested?.Invoke(this, "Không thể kết nối database. Dữ liệu sẽ không được lưu.");
                    CloseRequested?.Invoke(this, true);
                    return;
                }

                var dto = new CreatePaymentDto
                {
                    MaHopDong = SelectedContractId.Value,
                    ThangNam = CurrentMonth,
                    TienThue = RentAmount,
                    TienDien = ElectricitySubtotal,
                    TienNuoc = WaterSubtotal,
                    SoDien = ElectricityQuantity,
                    SoNuoc = WaterQuantity,
                    DonGiaDien = ElectricityUnitPrice,
                    DonGiaNuoc = WaterUnitPrice
                };

                var result = await _financialController.CreatePaymentAsync(dto);

                if (result.IsValid)
                {
                    ShowMessageRequested?.Invoke(this, result.Message ?? "Ghi nhận thành công!");
                    CloseRequested?.Invoke(this, true);
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, result.Message ?? "Ghi nhận thất bại!");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi lưu: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

        private async void UpdateRoomInfo()
        {
            if (!SelectedContractId.HasValue || _financialController == null)
                return;

            try
            {
                var contract = await _financialController.GetContractByIdAsync(SelectedContractId.Value);
                if (contract != null)
                {
                    var room = await _financialController.GetRoomByIdAsync(contract.MaPhong);
                    if (room != null)
                    {
                        SelectedRoomName = room.TenPhong;
                        RentAmount = room.GiaCoBan;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi tải thông tin phòng: {ex.Message}");
            }
        }

        private void LoadSampleContracts()
        {
            AvailableContracts.Clear();
            AvailableContracts.Add(new Contract
            {
                MaHopDong = 1,
                MaPhong = 1,
                MaNguoiThue = 1,
                NgayBatDau = DateTime.Now.AddMonths(-1),
                NgayKetThuc = DateTime.Now.AddMonths(11),
                TrangThai = "Hiệu lực"
            });
            SelectedContractId = 1;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

