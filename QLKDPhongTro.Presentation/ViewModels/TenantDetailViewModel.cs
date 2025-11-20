using CommunityToolkit.Mvvm.ComponentModel;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
    }
}

