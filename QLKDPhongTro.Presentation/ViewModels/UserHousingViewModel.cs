using CommunityToolkit.Mvvm.ComponentModel;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class UserHousingViewModel : ObservableObject
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IRentedRoomRepository _roomRepository;
        private readonly IHouseRepository _houseRepository;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _hasData;

        [ObservableProperty]
        private string _tenNha = string.Empty;

        [ObservableProperty]
        private string _diaChiNha = string.Empty;

        [ObservableProperty]
        private string _tinhThanh = string.Empty;

        [ObservableProperty]
        private string _ghiChuNha = string.Empty;

        [ObservableProperty]
        private string _tenPhong = string.Empty;

        [ObservableProperty]
        private string _dienTich = string.Empty;

        [ObservableProperty]
        private string _giaCoBan = string.Empty;

        [ObservableProperty]
        private string _giaBangChu = string.Empty;

        [ObservableProperty]
        private string _trangThaiPhong = string.Empty;

        [ObservableProperty]
        private string _trangThietBi = string.Empty;

        public UserHousingViewModel()
        {
            _tenantRepository = new TenantRepository();
            _roomRepository = new RentedRoomRepository();
            _houseRepository = new HouseRepository();
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                HasData = false;

                // 1. Get current user's email
                var currentUser = AuthController.CurrentUser;
                if (currentUser == null || string.IsNullOrEmpty(currentUser.Email))
                {
                    IsLoading = false;
                    return;
                }

                // 2. Find Tenant by Email
                var tenant = await _tenantRepository.GetByEmailAsync(currentUser.Email);
                if (tenant == null || tenant.MaPhong == null)
                {
                     // Fallback: Try to get data based on MaNha directly linked to user (if any, though User.MaNha is usually for Admin)
                     // Regular users are linked to room via Tenant Record.
                     IsLoading = false;
                     return;
                }

                // 3. Get Room Details
                var room = await _roomRepository.GetByIdAsync(tenant.MaPhong.Value);
                if (room == null)
                {
                    IsLoading = false;
                    return;
                }

                // 4. Get House Details
                var house = await _houseRepository.GetByIdAsync(room.MaNha);
                if (house == null)
                {
                    IsLoading = false;
                    return;
                }

                // 5. Populate Data
                TenNha = $"Nhà #{house.MaNha}"; // Or add a Name field to House if it exists, otherwise use Address/ID
                DiaChiNha = house.DiaChi;
                TinhThanh = house.TinhThanh;
                GhiChuNha = house.GhiChu;

                TenPhong = room.TenPhong;
                DienTich = $"{room.DienTich:N2} m²";
                GiaCoBan = $"{room.GiaCoBan:N0} VNĐ";
                GiaBangChu = room.GiaBangChu;
                TrangThaiPhong = room.TrangThai;
                TrangThietBi = room.TrangThietBi;

                HasData = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin nhà ở: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
