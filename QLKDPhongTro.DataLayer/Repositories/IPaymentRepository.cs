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
        /// <summary>
        /// Lấy tất cả thanh toán
        /// </summary>
        Task<List<Payment>> GetAllAsync();

        /// <summary>
        /// Lấy thanh toán theo ID
        /// </summary>
        Task<Payment?> GetByIdAsync(int maThanhToan);

        /// <summary>
        /// Tạo thanh toán mới
        /// </summary>
        Task<bool> CreateAsync(Payment payment);

        /// <summary>
        /// Cập nhật thông tin thanh toán
        /// </summary>
        Task<bool> UpdateAsync(Payment payment);

        /// <summary>
        /// Xóa thanh toán
        /// </summary>
        Task<bool> DeleteAsync(int maThanhToan);

        /// <summary>
        /// Kiểm tra thanh toán đã tồn tại chưa
        /// </summary>
        Task<bool> IsPaymentExistsAsync(int maHopDong, string thangNam);

        /// <summary>
        /// Lấy thanh toán theo hợp đồng và tháng
        /// </summary>
        Task<Payment?> GetPaymentByContractAndMonthAsync(int maHopDong, string thangNam);

        /// <summary>
        /// Lấy thanh toán gần nhất theo hợp đồng
        /// </summary>
        Task<Payment?> GetLastPaymentByContractIdAsync(int maHopDong);

        /// <summary>
        /// Lấy danh sách công nợ
        /// </summary>
        Task<List<Payment>> GetDebtsAsync(string? thangNam = null);

        /// <summary>
        /// Lấy thanh toán theo trạng thái
        /// </summary>
        Task<List<Payment>> GetPaymentsByStatusAsync(string trangThai);

        /// <summary>
        /// Lấy thống kê tài chính
        /// </summary>
        Task<FinancialStats> GetFinancialStatsAsync(int? nam = null);

        /// <summary>
        /// Lấy lịch sử giao dịch
        /// </summary>
        Task<List<Payment>> GetTransactionHistoryAsync(DateTime? tuNgay = null, DateTime? denNgay = null);

        /// <summary>
        /// Tạo thanh toán hàng tháng tự động
        /// </summary>
        Task<int> GenerateMonthlyPaymentsAsync(string thangNam);

        /// <summary>
        /// Đánh dấu thanh toán đã trả
        /// </summary>
        Task<bool> MarkAsPaidAsync(int maThanhToan, DateTime ngayThanhToan, string phuongThucThanhToan = "Tiền mặt");

        /// <summary>
        /// Lấy thanh toán theo phòng
        /// </summary>
        Task<List<Payment>> GetPaymentsByRoomAsync(int maPhong);
    }
}