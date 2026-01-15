using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class ManualDebtViewModel : ObservableObject
    {
        private readonly FinancialController _financialController;

        [ObservableProperty] private ObservableCollection<ContractDto> _contracts = new();
        [ObservableProperty] private ContractDto? _selectedContract;
        [ObservableProperty] private string _thangNam = DateTime.Now.ToString("MM/yyyy");
        [ObservableProperty] private decimal _soDienThangNay;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private bool _isSaving;

        public ManualDebtViewModel(FinancialController financialController)
        {
            _financialController = financialController;
            _ = LoadContractsAsync();
        }

        private async Task LoadContractsAsync()
        {
            try
            {
                var contracts = await _financialController.GetActiveContractDtosAsync();
                Contracts = new ObservableCollection<ContractDto>(contracts);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi khi tải danh sách hợp đồng: {ex.Message}";
                MessageBox.Show(StatusMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (SelectedContract == null)
            {
                MessageBox.Show("Vui lòng chọn phòng/hợp đồng.", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ThangNam))
            {
                MessageBox.Show("Vui lòng nhập Tháng/Năm (MM/yyyy).", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(ThangNam.Trim(), @"^(0[1-9]|1[0-2])/\d{4}$"))
            {
                MessageBox.Show("Tháng năm phải có định dạng MM/yyyy.", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SoDienThangNay < 0)
            {
                MessageBox.Show("Số điện tháng này không được âm.", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsSaving = true;

                // Lấy thông tin hợp đồng để xác định tiền thuê
                var contract = await _financialController.GetContractByIdAsync(SelectedContract.MaHopDong);
                if (contract == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin hợp đồng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ưu tiên giá thuê trong hợp đồng, nếu 0 thì lấy giá cơ bản của phòng
                decimal tienThue = contract.GiaThue;
                if (tienThue <= 0)
                {
                    var room = await _financialController.GetRoomByIdAsync(contract.MaPhong);
                    tienThue = room?.GiaCoBan ?? 0;
                }

                var dto = new CreatePaymentDto
                {
                    MaHopDong = SelectedContract.MaHopDong,
                    ThangNam = ThangNam.Trim(),
                    TienThue = tienThue,
                    // Các trường khác để 0, FinancialController.CreatePaymentAsync sẽ tự tính điện, nước...
                    SoDien = SoDienThangNay,
                    TienNuoc = 0,
                    TienDien = 0,
                    TienInternet = 0,
                    TienVeSinh = 0,
                    TienGiuXe = 0,
                    ChiPhiKhac = 0
                };

                var result = await _financialController.CreatePaymentAsync(dto);
                if (result.IsValid)
                {
                    MessageBox.Show(result.Message, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseAllManualDebtWindows();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo công nợ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAllManualDebtWindows();
        }

        private void CloseAllManualDebtWindows()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w.GetType().Name == "ManualDebtWindow")
                {
                    w.Close();
                }
            }
        }
    }
}


