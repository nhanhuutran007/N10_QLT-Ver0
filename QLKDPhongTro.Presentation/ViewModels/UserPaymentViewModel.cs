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
    public partial class UserPaymentViewModel : ObservableObject
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IPaymentRepository _paymentRepository;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _hasData;

        [ObservableProperty]
        private ObservableCollection<PaymentDisplayModel> _payments = new();

        [ObservableProperty]
        private PaymentDisplayModel? _selectedPayment;

        [ObservableProperty]
        private string _tienCocBanDau = "0 VNĐ";

        [ObservableProperty]
        private string _tienCocConLai = "0 VNĐ";

        [ObservableProperty]
        private decimal _tienCocBanDauRaw;

        public UserPaymentViewModel()
        {
            _tenantRepository = new TenantRepository();
            _contractRepository = new ContractRepository();
            _paymentRepository = new PaymentRepository();
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                HasData = false;
                Payments.Clear();

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

                // 3. Get active contracts for this tenant
                var contracts = await _contractRepository.GetActiveContractsByTenantAsync(tenant.MaKhachThue);
                if (contracts == null || !contracts.Any())
                {
                    IsLoading = false;
                    return;
                }

                // 4. Get deposit information from the most recent active contract
                var activeContract = contracts
                    .Where(c => c.TrangThai == "Hiệu lực")
                    .OrderByDescending(c => c.NgayBatDau)
                    .FirstOrDefault();

                if (activeContract != null)
                {
                    TienCocBanDauRaw = activeContract.TienCoc;
                    TienCocBanDau = $"{activeContract.TienCoc:N0} VNĐ";
                    // For now, remaining deposit is the same as initial deposit
                    // You can modify this logic based on your business rules
                    TienCocConLai = $"{activeContract.TienCoc:N0} VNĐ";
                }

                // 5. Get all payments for all contracts
                var allPayments = new System.Collections.Generic.List<DataLayer.Models.Payment>();
                foreach (var contract in contracts)
                {
                    var contractPayments = await _paymentRepository.GetPaymentsByContractAsync(contract.MaHopDong);
                    if (contractPayments != null && contractPayments.Any())
                    {
                        allPayments.AddRange(contractPayments);
                    }
                }

                if (!allPayments.Any())
                {
                    IsLoading = false;
                    return;
                }

                // 6. Convert to display models and add to collection
                var paymentDisplayModels = allPayments
                    .OrderByDescending(p => p.ThangNam) // Most recent first
                    .Select(payment => CreateDisplayModel(payment))
                    .ToList();

                foreach (var payment in paymentDisplayModels)
                {
                    Payments.Add(payment);
                }

                // Auto-select the first (most recent) payment
                if (Payments.Any())
                {
                    SelectedPayment = Payments.First();
                    HasData = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private PaymentDisplayModel CreateDisplayModel(DataLayer.Models.Payment payment)
        {
            var soTienConLai = payment.TongTien - (payment.SoTienDaTra ?? 0);
            
            var model = new PaymentDisplayModel
            {
                MaThanhToan = payment.MaThanhToan,
                MaHopDong = payment.MaHopDong,
                MaThanhToanDisplay = $"#{payment.MaThanhToan}",
                ThangNam = payment.ThangNam,
                TrangThai = payment.TrangThaiThanhToan ?? "Chưa trả",
                NgayThanhToan = payment.NgayThanhToan?.ToString("dd/MM/yyyy") ?? "Chưa thanh toán",
                NgayThanhToanRaw = payment.NgayThanhToan,
                
                // Cost breakdown
                TienThue = $"{payment.TienThue ?? 0:N0} VNĐ",
                TienDien = $"{payment.TienDien ?? 0:N0} VNĐ",
                TienNuoc = $"{payment.TienNuoc ?? 0:N0} VNĐ",
                TienInternet = $"{payment.TienInternet ?? 0:N0} VNĐ",
                TienVeSinh = $"{payment.TienVeSinh ?? 0:N0} VNĐ",
                TienGiuXe = $"{payment.TienGiuXe ?? 0:N0} VNĐ",
                ChiPhiKhac = $"{payment.ChiPhiKhac ?? 0:N0} VNĐ",
                
                // Electricity and water details
                ChiSoDienCu = payment.ChiSoDienCu?.ToString("N0") ?? "0",
                ChiSoDienMoi = payment.ChiSoDienMoi?.ToString("N0") ?? "0",
                SoDien = payment.SoDien?.ToString("N0") ?? "0",
                DonGiaDien = $"{payment.DonGiaDien ?? 0:N0} VNĐ/kWh",
                SoNuoc = payment.SoNuoc?.ToString("N0") ?? "0",
                DonGiaNuoc = $"{payment.DonGiaNuoc ?? 0:N0} VNĐ/m³",
                
                // Summary
                TongTien = $"{payment.TongTien:N0} VNĐ",
                SoTienDaTra = $"{payment.SoTienDaTra ?? 0:N0} VNĐ",
                SoTienConLai = $"{soTienConLai:N0} VNĐ",
                GhiChu = string.IsNullOrWhiteSpace(payment.GhiChu) ? "Không có ghi chú" : payment.GhiChu,
                
                // Additional info
                TenPhong = payment.TenPhong ?? "N/A",
                TenKhachHang = payment.TenKhachHang ?? "N/A",
                
                // Raw values
                TongTienRaw = payment.TongTien,
                SoTienDaTraRaw = payment.SoTienDaTra ?? 0
            };

            // Set color based on status
            model.TrangThaiColor = payment.TrangThaiThanhToan switch
            {
                "Đã trả" => "#2E7D32",      // Green
                "Chưa trả" => "#C62828",    // Red
                _ => "#F57C00"              // Orange (default)
            };

            return model;
        }
    }
}
