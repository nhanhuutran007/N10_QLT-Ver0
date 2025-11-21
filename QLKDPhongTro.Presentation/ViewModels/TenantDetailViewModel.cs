using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Views.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class TenantDetailViewModel : ObservableObject
    {
        private readonly TenantController _tenantController;

        public TenantDetailViewModel()
            : this(new TenantController(new TenantRepository(), new RentedRoomRepository()))
        {
        }

        public TenantDetailViewModel(TenantController controller)
        {
            _tenantController = controller;
        }

        [ObservableProperty] private TenantDto? _basicInfo;
        [ObservableProperty] private TenantStayInfoDto? _stayInfo;
        [ObservableProperty] private ObservableCollection<TenantAssetDto> _assets = new();
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private string _statusMessage = string.Empty;
        [ObservableProperty] private TenantAssetDto? _selectedAsset;

        public string[] AssetTypes { get; } = new[] { "Xe", "Thú cưng", "Khác" };

        public async Task LoadAsync(int tenantId)
        {
            try
            {
                IsLoading = true;
                var detail = await _tenantController.GetTenantDetailAsync(tenantId);
                if (detail == null)
                {
                    StatusMessage = "Không tìm thấy thông tin khách thuê.";
                    return;
                }

                BasicInfo = detail.BasicInfo;
                StayInfo = detail.StayInfo;
                Assets = new ObservableCollection<TenantAssetDto>(detail.Assets);
                StatusMessage = detail.StayInfo?.ConsistencyMessage ?? string.Empty;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task AddAsset()
        {
            if (BasicInfo == null) return;

            var newAsset = new TenantAssetDto
            {
                LoaiTaiSan = "Xe",
                MoTa = string.Empty,
                PhiPhuThu = 0
            };

            var dialog = new AddEditAssetWindow(newAsset, BasicInfo.MaKhachThue)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
            };
            if (dialog.ShowDialog() == true)
            {
                var result = await _tenantController.CreateAssetAsync(newAsset, BasicInfo.MaKhachThue);
                if (result.IsValid)
                {
                    StatusMessage = result.Message;
                    await RefreshAssetsAsync();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task EditAsset()
        {
            if (SelectedAsset == null || BasicInfo == null) return;

            var assetToEdit = new TenantAssetDto
            {
                MaTaiSan = SelectedAsset.MaTaiSan,
                LoaiTaiSan = SelectedAsset.LoaiTaiSan,
                MoTa = SelectedAsset.MoTa,
                PhiPhuThu = SelectedAsset.PhiPhuThu
            };

            var dialog = new AddEditAssetWindow(assetToEdit, BasicInfo.MaKhachThue)
            {
                Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) ?? Application.Current.MainWindow
            };
            if (dialog.ShowDialog() == true)
            {
                var result = await _tenantController.UpdateAssetAsync(assetToEdit);
                if (result.IsValid)
                {
                    StatusMessage = result.Message;
                    await RefreshAssetsAsync();
                }
                else
                {
                    MessageBox.Show(result.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task DeleteAsset()
        {
            if (SelectedAsset == null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa tài sản '{SelectedAsset.LoaiTaiSan}' này không?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var deleteResult = await _tenantController.DeleteAssetAsync(SelectedAsset.MaTaiSan);
                if (deleteResult.IsValid)
                {
                    StatusMessage = deleteResult.Message;
                    await RefreshAssetsAsync();
                }
                else
                {
                    MessageBox.Show(deleteResult.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task RefreshAssetsAsync()
        {
            if (BasicInfo == null) return;

            var detail = await _tenantController.GetTenantDetailAsync(BasicInfo.MaKhachThue);
            if (detail != null)
            {
                Assets = new ObservableCollection<TenantAssetDto>(detail.Assets);
            }
        }
    }
}

