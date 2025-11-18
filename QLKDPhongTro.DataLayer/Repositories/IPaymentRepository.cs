using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    /// <summary>
    /// Interface cho repository xử lý dữ liệu Payment
    /// </summary>
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllAsync();
        Task<List<Payment>> GetAllByMaNhaAsync(int maNha);
        Task<Payment?> GetByIdAsync(int maThanhToan);
        Task<bool> CreateAsync(Payment payment);
        Task<bool> UpdateAsync(Payment payment);
        Task<bool> DeleteAsync(int maThanhToan);
        Task<bool> IsPaymentExistsAsync(int maHopDong, string thangNam);
        Task<Payment?> GetPaymentByContractAndMonthAsync(int maHopDong, string thangNam);

        // Hàm này bị thiếu implementation trong Class cũ, tôi đã bổ sung ở dưới
        Task<Payment?> GetLastPaymentByContractIdAsync(int maHopDong);

        Task<List<Payment>> GetDebtsAsync(string? thangNam = null);
        Task<List<Payment>> GetPaymentsByStatusAsync(string trangThai);
        Task<FinancialStats> GetFinancialStatsAsync(int? nam = null);
        Task<List<Payment>> GetTransactionHistoryAsync(DateTime? tuNgay = null, DateTime? denNgay = null);
        Task<List<Payment>> GetTransactionHistoryByMaNhaAsync(int maNha, DateTime? tuNgay = null, DateTime? denNgay = null);
        Task<int> GenerateMonthlyPaymentsAsync(string thangNam);
        Task<bool> MarkAsPaidAsync(int maThanhToan, DateTime ngayThanhToan, string phuongThucThanhToan = "Tiền mặt");
        Task<List<Payment>> GetPaymentsByRoomAsync(int maPhong);
    }
}