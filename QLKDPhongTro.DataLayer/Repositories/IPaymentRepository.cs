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
    }

    /// <summary>
    /// Model cho thống kê tài chính
    /// </summary>
    public class FinancialStats
    {
        public decimal TongThuNhap { get; set; }
        public decimal TongChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
        public decimal TongCongNo { get; set; }
        public int SoPhongNo { get; set; }
        public decimal TangTruongThuNhap { get; set; }
        public decimal TangTruongChiPhi { get; set; }
        public decimal TyLeLoiNhuan { get; set; }
        public List<MonthlyStat> ThongKeTheoThang { get; set; } = new List<MonthlyStat>();
        public List<ExpenseCategory> PhanLoaiChiPhi { get; set; } = new List<ExpenseCategory>();
    }

    public class MonthlyStat
    {
        public string ThangNam { get; set; } = string.Empty;
        public decimal ThuNhap { get; set; }
        public decimal ChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }
    }

    public class ExpenseCategory
    {
        public string TenLoai { get; set; } = string.Empty;
        public decimal SoTien { get; set; }
        public decimal TyLe { get; set; }
    }
}