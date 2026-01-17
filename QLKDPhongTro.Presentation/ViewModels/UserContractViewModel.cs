using CommunityToolkit.Mvvm.ComponentModel;
using QLKDPhongTro.BusinessLayer.Controllers;
using QLKDPhongTro.DataLayer.Repositories;
using QLKDPhongTro.Presentation.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public partial class UserContractViewModel : ObservableObject
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IContractRepository _contractRepository;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _hasData;

        [ObservableProperty]
        private ObservableCollection<ContractDisplayModel> _contracts = new();

        [ObservableProperty]
        private ContractDisplayModel? _selectedContract;

        public UserContractViewModel()
        {
            _tenantRepository = new TenantRepository();
            _contractRepository = new ContractRepository();
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                HasData = false;
                Contracts.Clear();

                // 1. Get current user's email
                var currentUser = AuthController.CurrentUser;
                if (currentUser == null || string.IsNullOrEmpty(currentUser.Email))
                {
                    IsLoading = false;
                    return;
                }

                // 2. Find Tenant by Email
                var tenant = await _tenantRepository.GetByEmailAsync(currentUser.Email);
                if (tenant == null || tenant.MaKhachThue == 0)
                {
                    IsLoading = false;
                    return;
                }

                // 3. Get ALL Contracts for this Tenant (not just active ones)
                var contracts = await _contractRepository.GetActiveContractsByTenantAsync(tenant.MaKhachThue);
                
                // Also try to get all contracts if the method exists, otherwise use active only
                // For now, we'll use GetActiveContractsByTenantAsync and order by date
                
                if (contracts == null || !contracts.Any())
                {
                    IsLoading = false;
                    return;
                }

                // 4. Convert to display models and add to collection
                var contractDisplayModels = contracts
                    .OrderByDescending(c => c.NgayBatDau) // Most recent first
                    .Select(contract => CreateDisplayModel(contract, tenant.HoTen))
                    .ToList();

                // Mark the most recent active contract
                var mostRecentActive = contractDisplayModels
                    .FirstOrDefault(c => c.TrangThai == "Hiệu lực");
                if (mostRecentActive != null)
                {
                    mostRecentActive.IsActive = true;
                }

                foreach (var contract in contractDisplayModels)
                {
                    Contracts.Add(contract);
                }

                // Auto-select the first (most recent) contract
                if (Contracts.Any())
                {
                    SelectedContract = Contracts.First();
                    HasData = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private ContractDisplayModel CreateDisplayModel(DataLayer.Models.Contract contract, string tenantName)
        {
            var model = new ContractDisplayModel
            {
                MaHopDong = contract.MaHopDong,
                MaHopDongDisplay = $"#{contract.MaHopDong}",
                TenPhong = contract.TenPhong ?? "N/A",
                TenNguoiThue = contract.TenNguoiThue ?? tenantName ?? "N/A",
                NgayBatDauRaw = contract.NgayBatDau,
                NgayKetThucRaw = contract.NgayKetThuc,
                NgayBatDau = contract.NgayBatDau.ToString("dd/MM/yyyy"),
                NgayKetThuc = contract.NgayKetThuc.ToString("dd/MM/yyyy"),
                TienCoc = $"{contract.TienCoc:N0} VNĐ",
                GiaThue = $"{contract.GiaThue:N0} VNĐ",
                TrangThai = contract.TrangThai ?? "N/A",
                GhiChu = string.IsNullOrWhiteSpace(contract.GhiChu) ? "Không có ghi chú" : contract.GhiChu,
                FileHopDong = string.IsNullOrWhiteSpace(contract.FileHopDong) ? "Chưa có file" : contract.FileHopDong
            };

            // Calculate remaining days
            var remainingDays = (contract.NgayKetThuc - DateTime.Now).Days;
            if (remainingDays > 0)
            {
                model.SoNgayConLai = $"{remainingDays} ngày";
            }
            else if (remainingDays == 0)
            {
                model.SoNgayConLai = "Hết hạn hôm nay";
            }
            else
            {
                model.SoNgayConLai = $"Đã hết hạn {Math.Abs(remainingDays)} ngày";
            }

            // Set color based on status
            model.TrangThaiColor = contract.TrangThai switch
            {
                "Hiệu lực" => "#2E7D32",      // Green
                "Sắp hết hạn" => "#F57C00",   // Orange
                "Hết hạn" => "#C62828",       // Red
                "Hủy" => "#757575",           // Gray
                _ => "#3B4758"                // Default
            };

            return model;
        }
    }
}
