using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace QLKDPhongTro.DataLayer.Repositories
{
    /// <summary>
    /// Repository xử lý dữ liệu Payment
    /// </summary>
    public class PaymentRepository : IPaymentRepository
    {
        private readonly string connectionString;

        public PaymentRepository()
        {
            connectionString = "Data Source=.;Initial Catalog=QLThueNhaV0;Integrated Security=True;TrustServerCertificate=True;Encrypt=False";
        }

        public async Task<List<Payment>> GetAllAsync()
        {
            var payments = new List<Payment>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, p.DiaChi
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    ORDER BY tt.ThangNam DESC, tt.MaThanhToan DESC", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        payments.Add(new Payment
                        {
                            MaThanhToan = reader.GetInt32(0),
                            MaHopDong = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            ThangNam = reader.GetString(2),
                            TienThue = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                            TienDien = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                            TienNuoc = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                            TienInternet = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                            TienVeSinh = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                            TienGiuXe = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                            ChiPhiKhac = reader.IsDBNull(9) ? null : reader.GetDecimal(9),
                            TongTien = reader.GetDecimal(10),
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa thanh toán" : reader.GetString(11),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16)
                        });
                    }
                }
            }
            return payments;
        }

        public async Task<Payment?> GetByIdAsync(int maThanhToan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, p.DiaChi
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    WHERE tt.MaThanhToan = @MaThanhToan", conn);
                cmd.Parameters.AddWithValue("@MaThanhToan", maThanhToan);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Payment
                        {
                            MaThanhToan = reader.GetInt32(0),
                            MaHopDong = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            ThangNam = reader.GetString(2),
                            TienThue = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                            TienDien = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                            TienNuoc = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                            TienInternet = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                            TienVeSinh = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                            TienGiuXe = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                            ChiPhiKhac = reader.IsDBNull(9) ? null : reader.GetDecimal(9),
                            TongTien = reader.GetDecimal(10),
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa thanh toán" : reader.GetString(11),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<bool> CreateAsync(Payment payment)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    INSERT INTO ThanhToan (MaHopDong, ThangNam, TienThue, TienDien, TienNuoc, TienInternet, 
                                          TienVeSinh, TienGiuXe, ChiPhiKhac, TrangThaiThanhToan, NgayThanhToan)
                    VALUES (@MaHopDong, @ThangNam, @TienThue, @TienDien, @TienNuoc, @TienInternet, 
                           @TienVeSinh, @TienGiuXe, @ChiPhiKhac, @TrangThaiThanhToan, @NgayThanhToan)", conn);

                cmd.Parameters.AddWithValue("@MaHopDong", payment.MaHopDong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ThangNam", payment.ThangNam);
                cmd.Parameters.AddWithValue("@TienThue", payment.TienThue ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienDien", payment.TienDien ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienNuoc", payment.TienNuoc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienInternet", payment.TienInternet ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienVeSinh", payment.TienVeSinh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienGiuXe", payment.TienGiuXe ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChiPhiKhac", payment.ChiPhiKhac ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThaiThanhToan", payment.TrangThaiThanhToan);
                cmd.Parameters.AddWithValue("@NgayThanhToan", payment.NgayThanhToan ?? (object)DBNull.Value);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    UPDATE ThanhToan 
                    SET MaHopDong = @MaHopDong, ThangNam = @ThangNam, TienThue = @TienThue, 
                        TienDien = @TienDien, TienNuoc = @TienNuoc, TienInternet = @TienInternet,
                        TienVeSinh = @TienVeSinh, TienGiuXe = @TienGiuXe, ChiPhiKhac = @ChiPhiKhac,
                        TrangThaiThanhToan = @TrangThaiThanhToan, NgayThanhToan = @NgayThanhToan
                    WHERE MaThanhToan = @MaThanhToan", conn);

                cmd.Parameters.AddWithValue("@MaThanhToan", payment.MaThanhToan);
                cmd.Parameters.AddWithValue("@MaHopDong", payment.MaHopDong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ThangNam", payment.ThangNam);
                cmd.Parameters.AddWithValue("@TienThue", payment.TienThue ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienDien", payment.TienDien ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienNuoc", payment.TienNuoc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienInternet", payment.TienInternet ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienVeSinh", payment.TienVeSinh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienGiuXe", payment.TienGiuXe ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChiPhiKhac", payment.ChiPhiKhac ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThaiThanhToan", payment.TrangThaiThanhToan);
                cmd.Parameters.AddWithValue("@NgayThanhToan", payment.NgayThanhToan ?? (object)DBNull.Value);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> DeleteAsync(int maThanhToan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM ThanhToan WHERE MaThanhToan = @MaThanhToan", conn);
                cmd.Parameters.AddWithValue("@MaThanhToan", maThanhToan);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> IsPaymentExistsAsync(int maHopDong, string thangNam)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM ThanhToan WHERE MaHopDong = @MaHopDong AND ThangNam = @ThangNam", conn);
                cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                cmd.Parameters.AddWithValue("@ThangNam", thangNam);

                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public async Task<Payment?> GetPaymentByContractAndMonthAsync(int maHopDong, string thangNam)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan
                    FROM ThanhToan tt
                    WHERE tt.MaHopDong = @MaHopDong AND tt.ThangNam = @ThangNam", conn);
                cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                cmd.Parameters.AddWithValue("@ThangNam", thangNam);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Payment
                        {
                            MaThanhToan = reader.GetInt32(0),
                            MaHopDong = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            ThangNam = reader.GetString(2),
                            TienThue = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                            TienDien = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                            TienNuoc = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                            TienInternet = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                            TienVeSinh = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                            TienGiuXe = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                            ChiPhiKhac = reader.IsDBNull(9) ? null : reader.GetDecimal(9),
                            TongTien = reader.GetDecimal(10),
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa thanh toán" : reader.GetString(11),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<List<Payment>> GetDebtsAsync(string? thangNam = null)
        {
            var debts = new List<Payment>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, p.DiaChi
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    WHERE tt.TrangThaiThanhToan = 'Chưa thanh toán'";

                if (!string.IsNullOrEmpty(thangNam))
                {
                    sql += " AND tt.ThangNam = @ThangNam";
                }

                sql += " ORDER BY tt.ThangNam, tt.MaThanhToan";

                var cmd = new SqlCommand(sql, conn);
                if (!string.IsNullOrEmpty(thangNam))
                {
                    cmd.Parameters.AddWithValue("@ThangNam", thangNam);
                }

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        debts.Add(new Payment
                        {
                            MaThanhToan = reader.GetInt32(0),
                            MaHopDong = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            ThangNam = reader.GetString(2),
                            TienThue = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                            TienDien = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                            TienNuoc = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                            TienInternet = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                            TienVeSinh = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                            TienGiuXe = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                            ChiPhiKhac = reader.IsDBNull(9) ? null : reader.GetDecimal(9),
                            TongTien = reader.GetDecimal(10),
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa thanh toán" : reader.GetString(11),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16)
                        });
                    }
                }
            }
            return debts;
        }

        public async Task<List<Payment>> GetPaymentsByStatusAsync(string trangThai)
        {
            var payments = new List<Payment>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, p.DiaChi
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    WHERE tt.TrangThaiThanhToan = @TrangThai
                    ORDER BY tt.ThangNam DESC, tt.MaThanhToan DESC", conn);

                cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        payments.Add(new Payment
                        {
                            MaThanhToan = reader.GetInt32(0),
                            MaHopDong = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            ThangNam = reader.GetString(2),
                            TienThue = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                            TienDien = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                            TienNuoc = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                            TienInternet = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                            TienVeSinh = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                            TienGiuXe = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                            ChiPhiKhac = reader.IsDBNull(9) ? null : reader.GetDecimal(9),
                            TongTien = reader.GetDecimal(10),
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa thanh toán" : reader.GetString(11),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16)
                        });
                    }
                }
            }
            return payments;
        }

        public async Task<FinancialStats> GetFinancialStatsAsync(int? nam = null)
        {
            var stats = new FinancialStats();
            var currentYear = nam ?? DateTime.Now.Year;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // Tổng thu nhập (các thanh toán đã thanh toán)
                var cmdIncome = new SqlCommand(@"
                    SELECT ISNULL(SUM(TongTien), 0) 
                    FROM ThanhToan 
                    WHERE TrangThaiThanhToan = 'Đã thanh toán' 
                    AND YEAR(NgayThanhToan) = @Year", conn);
                cmdIncome.Parameters.AddWithValue("@Year", currentYear);
                stats.TongThuNhap = Convert.ToDecimal(await cmdIncome.ExecuteScalarAsync());

                // Tổng chi phí
                var cmdExpense = new SqlCommand(@"
                    SELECT ISNULL(SUM(ISNULL(TienDien, 0) + ISNULL(TienNuoc, 0) + ISNULL(TienInternet, 0) + 
                           ISNULL(TienVeSinh, 0) + ISNULL(TienGiuXe, 0) + ISNULL(ChiPhiKhac, 0)), 0)
                    FROM ThanhToan 
                    WHERE TrangThaiThanhToan = 'Đã thanh toán' 
                    AND YEAR(NgayThanhToan) = @Year", conn);
                cmdExpense.Parameters.AddWithValue("@Year", currentYear);
                stats.TongChiPhi = Convert.ToDecimal(await cmdExpense.ExecuteScalarAsync());

                stats.LoiNhuan = stats.TongThuNhap - stats.TongChiPhi;

                // Tổng công nợ
                var cmdDebt = new SqlCommand(@"
                    SELECT ISNULL(SUM(TongTien), 0) 
                    FROM ThanhToan 
                    WHERE TrangThaiThanhToan = 'Chưa thanh toán'", conn);
                stats.TongCongNo = Convert.ToDecimal(await cmdDebt.ExecuteScalarAsync());

                // Số phòng nợ
                var cmdRooms = new SqlCommand(@"
                    SELECT COUNT(DISTINCT hd.MaPhong)
                    FROM ThanhToan tt
                    INNER JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    WHERE tt.TrangThaiThanhToan = 'Chưa thanh toán'", conn);
                stats.SoPhongNo = Convert.ToInt32(await cmdRooms.ExecuteScalarAsync());

                // Thống kê theo tháng
                var cmdMonthly = new SqlCommand(@"
                    SELECT ThangNam, 
                           SUM(TongTien) as ThuNhap,
                           SUM(ISNULL(TienDien, 0) + ISNULL(TienNuoc, 0) + ISNULL(TienInternet, 0) + 
                               ISNULL(TienVeSinh, 0) + ISNULL(TienGiuXe, 0) + ISNULL(ChiPhiKhac, 0)) as ChiPhi,
                           SUM(TongTien) - SUM(ISNULL(TienDien, 0) + ISNULL(TienNuoc, 0) + ISNULL(TienInternet, 0) + 
                               ISNULL(TienVeSinh, 0) + ISNULL(TienGiuXe, 0) + ISNULL(ChiPhiKhac, 0)) as LoiNhuan
                    FROM ThanhToan
                    WHERE TrangThaiThanhToan = 'Đã thanh toán' 
                    AND YEAR(NgayThanhToan) = @Year
                    GROUP BY ThangNam
                    ORDER BY ThangNam", conn);
                cmdMonthly.Parameters.AddWithValue("@Year", currentYear);

                using (var reader = await cmdMonthly.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        stats.ThongKeTheoThang.Add(new MonthlyStat
                        {
                            ThangNam = reader.GetString(0),
                            ThuNhap = reader.GetDecimal(1),
                            ChiPhi = reader.GetDecimal(2),
                            LoiNhuan = reader.GetDecimal(3)
                        });
                    }
                }

                // Phân loại chi phí
                var cmdCategories = new SqlCommand(@"
                    SELECT 
                        SUM(ISNULL(TienDien, 0)) as TienDien,
                        SUM(ISNULL(TienNuoc, 0)) as TienNuoc,
                        SUM(ISNULL(TienInternet, 0)) as TienInternet,
                        SUM(ISNULL(TienVeSinh, 0)) as TienVeSinh,
                        SUM(ISNULL(TienGiuXe, 0)) as TienGiuXe,
                        SUM(ISNULL(ChiPhiKhac, 0)) as ChiPhiKhac
                    FROM ThanhToan
                    WHERE TrangThaiThanhToan = 'Đã thanh toán'", conn);

                using (var reader = await cmdCategories.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var totalExpenses = reader.GetDecimal(0) + reader.GetDecimal(1) + reader.GetDecimal(2) +
                                          reader.GetDecimal(3) + reader.GetDecimal(4) + reader.GetDecimal(5);

                        if (totalExpenses > 0)
                        {
                            stats.PhanLoaiChiPhi.Add(new ExpenseCategory { TenLoai = "Điện", SoTien = reader.GetDecimal(0), TyLe = reader.GetDecimal(0) / totalExpenses * 100 });
                            stats.PhanLoaiChiPhi.Add(new ExpenseCategory { TenLoai = "Nước", SoTien = reader.GetDecimal(1), TyLe = reader.GetDecimal(1) / totalExpenses * 100 });
                            stats.PhanLoaiChiPhi.Add(new ExpenseCategory { TenLoai = "Internet", SoTien = reader.GetDecimal(2), TyLe = reader.GetDecimal(2) / totalExpenses * 100 });
                            stats.PhanLoaiChiPhi.Add(new ExpenseCategory { TenLoai = "Vệ sinh", SoTien = reader.GetDecimal(3), TyLe = reader.GetDecimal(3) / totalExpenses * 100 });
                            stats.PhanLoaiChiPhi.Add(new ExpenseCategory { TenLoai = "Giữ xe", SoTien = reader.GetDecimal(4), TyLe = reader.GetDecimal(4) / totalExpenses * 100 });
                            stats.PhanLoaiChiPhi.Add(new ExpenseCategory { TenLoai = "Khác", SoTien = reader.GetDecimal(5), TyLe = reader.GetDecimal(5) / totalExpenses * 100 });
                        }
                    }
                }
            }

            return stats;
        }

        public async Task<List<Payment>> GetTransactionHistoryAsync(DateTime? tuNgay = null, DateTime? denNgay = null)
        {
            var transactions = new List<Payment>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, p.DiaChi
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    WHERE tt.TrangThaiThanhToan = 'Đã thanh toán'";

                if (tuNgay.HasValue)
                {
                    sql += " AND tt.NgayThanhToan >= @TuNgay";
                }
                if (denNgay.HasValue)
                {
                    sql += " AND tt.NgayThanhToan <= @DenNgay";
                }

                sql += " ORDER BY tt.NgayThanhToan DESC";

                var cmd = new SqlCommand(sql, conn);
                if (tuNgay.HasValue)
                {
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Value);
                }
                if (denNgay.HasValue)
                {
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay.Value);
                }

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        transactions.Add(new Payment
                        {
                            MaThanhToan = reader.GetInt32(0),
                            MaHopDong = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                            ThangNam = reader.GetString(2),
                            TienThue = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                            TienDien = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                            TienNuoc = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                            TienInternet = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                            TienVeSinh = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                            TienGiuXe = reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                            ChiPhiKhac = reader.IsDBNull(9) ? null : reader.GetDecimal(9),
                            TongTien = reader.GetDecimal(10),
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa thanh toán" : reader.GetString(11),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16)
                        });
                    }
                }
            }
            return transactions;
        }

        public async Task<int> GenerateMonthlyPaymentsAsync(string thangNam)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // Lấy các hợp đồng đang hoạt động
                var cmdContracts = new SqlCommand(@"
                    SELECT hd.MaHopDong, p.GiaThue
                    FROM HopDong hd
                    INNER JOIN Phong p ON hd.MaPhong = p.MaPhong
                    WHERE hd.TrangThai = 'Đang hoạt động' 
                    AND hd.NgayKetThuc >= GETDATE()", conn);

                using (var reader = await cmdContracts.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var maHopDong = reader.GetInt32(0);
                        var giaThue = reader.GetDecimal(1);

                        // Kiểm tra đã có thanh toán cho tháng này chưa
                        var cmdCheck = new SqlCommand(@"
                            SELECT COUNT(*) 
                            FROM ThanhToan 
                            WHERE MaHopDong = @MaHopDong AND ThangNam = @ThangNam", conn);
                        cmdCheck.Parameters.AddWithValue("@MaHopDong", maHopDong);
                        cmdCheck.Parameters.AddWithValue("@ThangNam", thangNam);

                        var exists = (int)await cmdCheck.ExecuteScalarAsync() > 0;

                        if (!exists)
                        {
                            // Tạo thanh toán mới
                            var cmdInsert = new SqlCommand(@"
                                INSERT INTO ThanhToan (MaHopDong, ThangNam, TienThue, TienDien, TienNuoc, 
                                                      TienInternet, TienVeSinh, TienGiuXe, ChiPhiKhac, 
                                                      TrangThaiThanhToan, NgayThanhToan)
                                VALUES (@MaHopDong, @ThangNam, @TienThue, 0, 0, 0, 0, 0, 0, 'Chưa thanh toán', NULL)", conn);

                            cmdInsert.Parameters.AddWithValue("@MaHopDong", maHopDong);
                            cmdInsert.Parameters.AddWithValue("@ThangNam", thangNam);
                            cmdInsert.Parameters.AddWithValue("@TienThue", giaThue);

                            await cmdInsert.ExecuteNonQueryAsync();
                            count++;
                        }
                    }
                }
            }
            return count;
        }
    }
}