using QLKDPhongTro.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; // Đã chuyển sang MySQL

namespace QLKDPhongTro.DataLayer.Repositories
{
    /// <summary>
    /// Repository xử lý dữ liệu Payment
    /// </summary>
    public class PaymentRepository : IPaymentRepository
    {
        // Sử dụng ConnectDB chung để quản lý connection string
        private string connectionString => ConnectDB.GetConnectionString();

        public async Task<List<Payment>> GetAllAsync()
        {
            var payments = new List<Payment>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand(@"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, n.DiaChi,
                           tt.DonGiaDien, tt.DonGiaNuoc, tt.SoDien, tt.SoNuoc
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    LEFT JOIN Nha n ON p.MaNha = n.MaNha
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
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa trả" : GetTrangThaiThanhToan(reader.GetString(11)),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                            DonGiaDien = reader.IsDBNull(17) ? null : reader.GetDecimal(17),
                            DonGiaNuoc = reader.IsDBNull(18) ? null : reader.GetDecimal(18),
                            SoDien = reader.IsDBNull(19) ? null : reader.GetDecimal(19),
                            SoNuoc = reader.IsDBNull(20) ? null : reader.GetDecimal(20)
                        });
                    }
                }
            }
            return payments;
        }

        public async Task<Payment?> GetByIdAsync(int maThanhToan)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand(@"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, n.DiaChi,
                           tt.DonGiaDien, tt.DonGiaNuoc, tt.SoDien, tt.SoNuoc
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    LEFT JOIN Nha n ON p.MaNha = n.MaNha
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
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa trả" : GetTrangThaiThanhToan(reader.GetString(11)),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                            DonGiaDien = reader.IsDBNull(17) ? null : reader.GetDecimal(17),
                            DonGiaNuoc = reader.IsDBNull(18) ? null : reader.GetDecimal(18),
                            SoDien = reader.IsDBNull(19) ? null : reader.GetDecimal(19),
                            SoNuoc = reader.IsDBNull(20) ? null : reader.GetDecimal(20)
                        };
                    }
                }
            }
            return null;
        }

        public async Task<bool> CreateAsync(Payment payment)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand(@"
                    INSERT INTO ThanhToan (MaHopDong, ThangNam, TienThue, TienDien, TienNuoc, TienInternet, 
                                          TienVeSinh, TienGiuXe, ChiPhiKhac, TrangThaiThanhToan, NgayThanhToan,
                                          DonGiaDien, DonGiaNuoc, SoDien, SoNuoc)
                    VALUES (@MaHopDong, @ThangNam, @TienThue, @TienDien, @TienNuoc, @TienInternet, 
                           @TienVeSinh, @TienGiuXe, @ChiPhiKhac, @TrangThaiThanhToan, @NgayThanhToan,
                           @DonGiaDien, @DonGiaNuoc, @SoDien, @SoNuoc)", conn);

                cmd.Parameters.AddWithValue("@MaHopDong", payment.MaHopDong ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ThangNam", payment.ThangNam);
                cmd.Parameters.AddWithValue("@TienThue", payment.TienThue ?? 0);
                cmd.Parameters.AddWithValue("@TienDien", payment.TienDien ?? 0);
                cmd.Parameters.AddWithValue("@TienNuoc", payment.TienNuoc ?? 0);
                cmd.Parameters.AddWithValue("@TienInternet", payment.TienInternet ?? 0);
                cmd.Parameters.AddWithValue("@TienVeSinh", payment.TienVeSinh ?? 0);
                cmd.Parameters.AddWithValue("@TienGiuXe", payment.TienGiuXe ?? 0);
                cmd.Parameters.AddWithValue("@ChiPhiKhac", payment.ChiPhiKhac ?? 0);
                cmd.Parameters.AddWithValue("@TrangThaiThanhToan", payment.TrangThaiThanhToan);
                cmd.Parameters.AddWithValue("@NgayThanhToan", payment.NgayThanhToan ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DonGiaDien", payment.DonGiaDien ?? 0);
                cmd.Parameters.AddWithValue("@DonGiaNuoc", payment.DonGiaNuoc ?? 0);
                cmd.Parameters.AddWithValue("@SoDien", payment.SoDien ?? 0);
                cmd.Parameters.AddWithValue("@SoNuoc", payment.SoNuoc ?? 0);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            try
            {
                // Sử dụng ConnectDB.CreateConnectionAsync() để tự động set charset utf8mb4
                using (MySqlConnection conn = await ConnectDB.CreateConnectionAsync())
                {
                    
                    // Kiểm tra giá trị ENUM hiện tại trong database
                    var checkEnumCmd = new MySqlCommand("SELECT COLUMN_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ThanhToan' AND COLUMN_NAME = 'TrangThaiThanhToan'", conn);
                    var enumDefinition = await checkEnumCmd.ExecuteScalarAsync() as string;
                    System.Diagnostics.Debug.WriteLine($"UpdateAsync: ENUM definition in DB = '{enumDefinition}'");
                    
                    // Debug logging
                    System.Diagnostics.Debug.WriteLine($"UpdateAsync: MaThanhToan={payment.MaThanhToan}, TrangThaiThanhToan={payment.TrangThaiThanhToan}");
                    
                    // Đảm bảo giá trị ENUM được set trực tiếp với giá trị chính xác
                    // MySQL ENUM('Chưa trả','Đã trả') - giá trị phải khớp chính xác
                    var cmd = new MySqlCommand(@"
                        UPDATE ThanhToan 
                        SET MaHopDong = @MaHopDong, ThangNam = @ThangNam, TienThue = @TienThue, 
                            TienDien = @TienDien, TienNuoc = @TienNuoc, TienInternet = @TienInternet,
                            TienVeSinh = @TienVeSinh, TienGiuXe = @TienGiuXe, ChiPhiKhac = @ChiPhiKhac,
                            TrangThaiThanhToan = @TrangThaiThanhToan, NgayThanhToan = @NgayThanhToan,
                            DonGiaDien = @DonGiaDien, DonGiaNuoc = @DonGiaNuoc, SoDien = @SoDien, SoNuoc = @SoNuoc
                        WHERE MaThanhToan = @MaThanhToan", conn);

                    cmd.Parameters.AddWithValue("@MaThanhToan", payment.MaThanhToan);
                    cmd.Parameters.AddWithValue("@MaHopDong", payment.MaHopDong ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ThangNam", payment.ThangNam);
                    cmd.Parameters.AddWithValue("@TienThue", payment.TienThue ?? 0);
                    cmd.Parameters.AddWithValue("@TienDien", payment.TienDien ?? 0);
                    cmd.Parameters.AddWithValue("@TienNuoc", payment.TienNuoc ?? 0);
                    cmd.Parameters.AddWithValue("@TienInternet", payment.TienInternet ?? 0);
                    cmd.Parameters.AddWithValue("@TienVeSinh", payment.TienVeSinh ?? 0);
                    cmd.Parameters.AddWithValue("@TienGiuXe", payment.TienGiuXe ?? 0);
                    cmd.Parameters.AddWithValue("@ChiPhiKhac", payment.ChiPhiKhac ?? 0);
                    
                    // Đảm bảo giá trị TrangThaiThanhToan khớp với ENUM trong database
                    // MySQL ENUM('Chưa trả','Đã trả') - giá trị phải khớp chính xác
                    string trangThai;
                    if (string.IsNullOrWhiteSpace(payment.TrangThaiThanhToan))
                    {
                        trangThai = "Chưa trả";
                        System.Diagnostics.Debug.WriteLine($"UpdateAsync: TrangThaiThanhToan is null/empty, defaulting to 'Chưa trả'");
                    }
                    else
                    {
                        trangThai = payment.TrangThaiThanhToan.Trim();
                        // Chuẩn hóa giá trị để khớp với ENUM trong database - so sánh chính xác
                        // ENUM values: 'Chưa trả','Đã trả' (theo MySQLSchema.sql)
                        if (string.Equals(trangThai, "Đã trả", StringComparison.OrdinalIgnoreCase))
                        {
                            trangThai = "Đã trả";
                        }
                        else if (string.Equals(trangThai, "Chưa trả", StringComparison.OrdinalIgnoreCase))
                        {
                            trangThai = "Chưa trả";
                        }
                        else
                        {
                            // Nếu không khớp, mặc định là "Chưa trả"
                            System.Diagnostics.Debug.WriteLine($"UpdateAsync: Warning - TrangThaiThanhToan value '{trangThai}' không khớp, defaulting to 'Chưa trả'");
                            trangThai = "Chưa trả";
                        }
                    }
                    
                    // Debug logging - kiểm tra byte length và encoding
                    var bytes = System.Text.Encoding.UTF8.GetBytes(trangThai);
                    System.Diagnostics.Debug.WriteLine($"UpdateAsync: Final TrangThaiThanhToan value = '{trangThai}' (Length: {trangThai.Length}, Bytes: {bytes.Length})");
                    System.Diagnostics.Debug.WriteLine($"UpdateAsync: Byte array: [{string.Join(", ", bytes)}]");
                    
                    // Đảm bảo giá trị không bao giờ là null hoặc empty
                    if (string.IsNullOrWhiteSpace(trangThai))
                    {
                        trangThai = "Chưa trả";
                        System.Diagnostics.Debug.WriteLine($"UpdateAsync: TrangThai was null/empty, set to 'Chưa trả'");
                    }
                    
                    // Verify giá trị khớp với ENUM definition
                    // ENUM('Chưa trả','Đã trả') - giá trị phải khớp chính xác
                    if (trangThai != "Chưa trả" && trangThai != "Đã trả")
                    {
                        System.Diagnostics.Debug.WriteLine($"UpdateAsync: ERROR - TrangThai '{trangThai}' không khớp với ENUM values!");
                        trangThai = "Chưa trả"; // Fallback
                    }
                    
                    // Với MySQL ENUM, cần set giá trị chính xác
                    // MySQL ENUM sẽ tự động convert string thành ENUM value nếu khớp
                    // Nếu giá trị không khớp, MySQL sẽ set thành empty string hoặc NULL
                    // Đảm bảo giá trị khớp chính xác với ENUM definition: 'Chưa trả','Đã trả'
                    // Sử dụng MySqlParameter với explicit encoding để đảm bảo MySQL nhận diện đúng
                    var trangThaiParam = new MySqlParameter("@TrangThaiThanhToan", MySqlDbType.VarChar)
                    {
                        Value = trangThai,
                        Size = 20
                    };
                    cmd.Parameters.Add(trangThaiParam);
                    
                    System.Diagnostics.Debug.WriteLine($"UpdateAsync: Setting parameter @TrangThaiThanhToan = '{trangThai}' (Type: VarChar, Size: 20)");
                    
                    cmd.Parameters.AddWithValue("@NgayThanhToan", payment.NgayThanhToan ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DonGiaDien", payment.DonGiaDien ?? 0);
                    cmd.Parameters.AddWithValue("@DonGiaNuoc", payment.DonGiaNuoc ?? 0);
                    cmd.Parameters.AddWithValue("@SoDien", payment.SoDien ?? 0);
                    cmd.Parameters.AddWithValue("@SoNuoc", payment.SoNuoc ?? 0);

                    // Thực thi query và kiểm tra kết quả
                    var result = await cmd.ExecuteNonQueryAsync();
                    
                    // Debug logging
                    System.Diagnostics.Debug.WriteLine($"UpdateAsync: Result={result}, RowsAffected={result}");
                    
                    if (result > 0)
                    {
                        // Verify giá trị đã được lưu đúng chưa
                        var verifyCmd = new MySqlCommand("SELECT TrangThaiThanhToan FROM ThanhToan WHERE MaThanhToan = @MaThanhToan", conn);
                        verifyCmd.Parameters.AddWithValue("@MaThanhToan", payment.MaThanhToan);
                        var verifyResult = await verifyCmd.ExecuteScalarAsync();
                        var verifyValue = verifyResult?.ToString() ?? "NULL";
                        System.Diagnostics.Debug.WriteLine($"UpdateAsync: Verified value in DB = '{verifyValue}' (Type: {verifyResult?.GetType().Name ?? "NULL"})");
                        
                        // Nếu giá trị trong DB không khớp với giá trị mong muốn, log warning
                        if (verifyValue != trangThai && verifyValue != "NULL" && !string.IsNullOrWhiteSpace(verifyValue))
                        {
                            System.Diagnostics.Debug.WriteLine($"UpdateAsync: WARNING - Value mismatch! Expected: '{trangThai}', Got: '{verifyValue}'");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"UpdateAsync: ERROR - Update failed! Rows affected: {result}");
                    }
                    
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                System.Diagnostics.Debug.WriteLine($"UpdateAsync Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int maThanhToan)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("DELETE FROM ThanhToan WHERE MaThanhToan = @MaThanhToan", conn);
                cmd.Parameters.AddWithValue("@MaThanhToan", maThanhToan);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<bool> IsPaymentExistsAsync(int maHopDong, string thangNam)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM ThanhToan WHERE MaHopDong = @MaHopDong AND ThangNam = @ThangNam", conn);
                cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                cmd.Parameters.AddWithValue("@ThangNam", thangNam);

                var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return count > 0;
            }
        }

        public async Task<Payment?> GetPaymentByContractAndMonthAsync(int maHopDong, string thangNam)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand(@"
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
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa trả" : (string.IsNullOrWhiteSpace(reader.GetString(11)) ? "Chưa trả" : reader.GetString(11).Trim()),
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
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, n.DiaChi,
                           tt.DonGiaDien, tt.DonGiaNuoc, tt.SoDien, tt.SoNuoc
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    LEFT JOIN Nha n ON p.MaNha = n.MaNha
                    WHERE tt.TrangThaiThanhToan = 'Chưa trả'";

                if (!string.IsNullOrEmpty(thangNam))
                {
                    sql += " AND tt.ThangNam = @ThangNam";
                }

                sql += " ORDER BY tt.ThangNam, tt.MaThanhToan";

                var cmd = new MySqlCommand(sql, conn);
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
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa trả" : GetTrangThaiThanhToan(reader.GetString(11)),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                            DonGiaDien = reader.IsDBNull(17) ? null : reader.GetDecimal(17),
                            DonGiaNuoc = reader.IsDBNull(18) ? null : reader.GetDecimal(18),
                            SoDien = reader.IsDBNull(19) ? null : reader.GetDecimal(19),
                            SoNuoc = reader.IsDBNull(20) ? null : reader.GetDecimal(20)
                        });
                    }
                }
            }
            return debts;
        }

        public async Task<List<Payment>> GetPaymentsByStatusAsync(string trangThai)
        {
            var payments = new List<Payment>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand(@"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, n.DiaChi,
                           tt.DonGiaDien, tt.DonGiaNuoc, tt.SoDien, tt.SoNuoc
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    LEFT JOIN Nha n ON p.MaNha = n.MaNha
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
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa trả" : GetTrangThaiThanhToan(reader.GetString(11)),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                            DonGiaDien = reader.IsDBNull(17) ? null : reader.GetDecimal(17),
                            DonGiaNuoc = reader.IsDBNull(18) ? null : reader.GetDecimal(18),
                            SoDien = reader.IsDBNull(19) ? null : reader.GetDecimal(19),
                            SoNuoc = reader.IsDBNull(20) ? null : reader.GetDecimal(20)
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

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // Tổng thu nhập (các thanh toán đã thanh toán)
                var cmdIncome = new MySqlCommand(@"
                    SELECT IFNULL(SUM(TongTien), 0) 
                    FROM ThanhToan 
                    WHERE TrangThaiThanhToan = 'Đã trả' 
                    AND YEAR(NgayThanhToan) = @Year", conn);
                cmdIncome.Parameters.AddWithValue("@Year", currentYear);
                stats.TongThuNhap = Convert.ToDecimal(await cmdIncome.ExecuteScalarAsync());

                // Tổng chi phí
                var cmdExpense = new MySqlCommand(@"
                    SELECT IFNULL(SUM(IFNULL(TienDien, 0) + IFNULL(TienNuoc, 0) + IFNULL(TienInternet, 0) + 
                           IFNULL(TienVeSinh, 0) + IFNULL(TienGiuXe, 0) + IFNULL(ChiPhiKhac, 0)), 0)
                    FROM ThanhToan 
                    WHERE TrangThaiThanhToan = 'Đã trả' 
                    AND YEAR(NgayThanhToan) = @Year", conn);
                cmdExpense.Parameters.AddWithValue("@Year", currentYear);
                stats.TongChiPhi = Convert.ToDecimal(await cmdExpense.ExecuteScalarAsync());

                stats.LoiNhuan = stats.TongThuNhap - stats.TongChiPhi;

                // Tổng công nợ
                var cmdDebt = new MySqlCommand(@"
                    SELECT IFNULL(SUM(TongTien), 0) 
                    FROM ThanhToan 
                    WHERE TrangThaiThanhToan = 'Chưa trả'", conn);
                stats.TongCongNo = Convert.ToDecimal(await cmdDebt.ExecuteScalarAsync());

                // Số phòng nợ
                var cmdRooms = new MySqlCommand(@"
                    SELECT COUNT(DISTINCT hd.MaPhong)
                    FROM ThanhToan tt
                    INNER JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    WHERE tt.TrangThaiThanhToan = 'Chưa trả'", conn);
                stats.SoPhongNo = Convert.ToInt32(await cmdRooms.ExecuteScalarAsync());

                // Thống kê theo tháng
                var cmdMonthly = new MySqlCommand(@"
                    SELECT ThangNam, 
                           SUM(TongTien) as ThuNhap,
                           SUM(COALESCE(TienDien, 0) + COALESCE(TienNuoc, 0) + COALESCE(TienInternet, 0) + 
                               COALESCE(TienVeSinh, 0) + COALESCE(TienGiuXe, 0) + COALESCE(ChiPhiKhac, 0)) as ChiPhi,
                           SUM(TongTien) - SUM(COALESCE(TienDien, 0) + COALESCE(TienNuoc, 0) + COALESCE(TienInternet, 0) + 
                               COALESCE(TienVeSinh, 0) + COALESCE(TienGiuXe, 0) + COALESCE(ChiPhiKhac, 0)) as LoiNhuan
                    FROM ThanhToan
                    WHERE TrangThaiThanhToan = 'Đã trả' 
                    AND YEAR(NgayThanhToan) = @Year
                    GROUP BY ThangNam
                    ORDER BY ThangNam", conn);
                cmdMonthly.Parameters.AddWithValue("@Year", currentYear);

                using (var reader = await cmdMonthly.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        stats.ThongKeTheoThang.Add(new MonthlyStats
                        {
                            ThangNam = reader.GetString(0),
                            ThuNhap = reader.GetDecimal(1),
                            ChiPhi = reader.GetDecimal(2),
                            LoiNhuan = reader.GetDecimal(3)
                        });
                    }
                }

                // Phân loại chi phí
                var cmdCategories = new MySqlCommand(@"
                    SELECT 
                        SUM(COALESCE(TienDien, 0)) as TienDien,
                        SUM(COALESCE(TienNuoc, 0)) as TienNuoc,
                        SUM(COALESCE(TienInternet, 0)) as TienInternet,
                        SUM(COALESCE(TienVeSinh, 0)) as TienVeSinh,
                        SUM(COALESCE(TienGiuXe, 0)) as TienGiuXe,
                        SUM(COALESCE(ChiPhiKhac, 0)) as ChiPhiKhac
                    FROM ThanhToan
                    WHERE TrangThaiThanhToan = 'Đã trả'", conn);

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
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var sql = @"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, n.DiaChi
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    LEFT JOIN Nha n ON p.MaNha = n.MaNha
                    WHERE tt.TrangThaiThanhToan = 'Đã trả'";

                if (tuNgay.HasValue)
                {
                    sql += " AND tt.NgayThanhToan >= @TuNgay";
                }
                if (denNgay.HasValue)
                {
                    sql += " AND tt.NgayThanhToan <= @DenNgay";
                }

                sql += " ORDER BY tt.NgayThanhToan DESC";

                var cmd = new MySqlCommand(sql, conn);
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
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa trả" : GetTrangThaiThanhToan(reader.GetString(11)),
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
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // Lấy các hợp đồng đang hoạt động
                var cmdContracts = new MySqlCommand(@"
                    SELECT hd.MaHopDong, p.GiaCoBan
                    FROM HopDong hd
                    INNER JOIN Phong p ON hd.MaPhong = p.MaPhong
                    WHERE hd.TrangThai = 'Hiệu lực' 
                    AND hd.NgayKetThuc >= NOW()", conn);

                using (var reader = await cmdContracts.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var maHopDong = reader.GetInt32(0);
                        var giaThue = reader.GetDecimal(1);

                        // Kiểm tra đã có thanh toán cho tháng này chưa
                        var cmdCheck = new MySqlCommand(@"
                            SELECT COUNT(*) 
                            FROM ThanhToan 
                            WHERE MaHopDong = @MaHopDong AND ThangNam = @ThangNam", conn);
                        cmdCheck.Parameters.AddWithValue("@MaHopDong", maHopDong);
                        cmdCheck.Parameters.AddWithValue("@ThangNam", thangNam);

                        var exists = Convert.ToInt32(await cmdCheck.ExecuteScalarAsync()) > 0;

                        if (!exists)
                        {
                            // Tạo thanh toán mới
                            var cmdInsert = new MySqlCommand(@"
                                INSERT INTO ThanhToan (MaHopDong, ThangNam, TienThue, TienDien, TienNuoc, 
                                                      TienInternet, TienVeSinh, TienGiuXe, ChiPhiKhac, 
                                                      TrangThaiThanhToan, NgayThanhToan)
                                VALUES (@MaHopDong, @ThangNam, @TienThue, 0, 0, 0, 0, 0, 0, 'Chưa trả', NULL)", conn);

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

        public async Task<bool> MarkAsPaidAsync(int maThanhToan, DateTime ngayThanhToan, string phuongThucThanhToan = "Tiền mặt")
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand(@"
                    UPDATE ThanhToan 
                    SET TrangThaiThanhToan = 'Đã trả', 
                        NgayThanhToan = @NgayThanhToan
                    WHERE MaThanhToan = @MaThanhToan", conn);

                cmd.Parameters.AddWithValue("@MaThanhToan", maThanhToan);
                cmd.Parameters.AddWithValue("@NgayThanhToan", ngayThanhToan);

                var result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<List<Payment>> GetPaymentsByRoomAsync(int maPhong)
        {
            var payments = new List<Payment>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand(@"
                    SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam, tt.TienThue, tt.TienDien, tt.TienNuoc, 
                           tt.TienInternet, tt.TienVeSinh, tt.TienGiuXe, tt.ChiPhiKhac, tt.TongTien, 
                           tt.TrangThaiThanhToan, tt.NgayThanhToan,
                           nt.HoTen, p.TenPhong, nt.SoDienThoai, n.DiaChi,
                           tt.DonGiaDien, tt.DonGiaNuoc, tt.SoDien, tt.SoNuoc
                    FROM ThanhToan tt
                    LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
                    LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
                    LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
                    LEFT JOIN Nha n ON p.MaNha = n.MaNha
                    WHERE hd.MaPhong = @MaPhong
                    ORDER BY tt.ThangNam DESC", conn);

                cmd.Parameters.AddWithValue("@MaPhong", maPhong);

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
                            TrangThaiThanhToan = reader.IsDBNull(11) ? "Chưa trả" : GetTrangThaiThanhToan(reader.GetString(11)),
                            NgayThanhToan = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                            TenKhachHang = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            TenPhong = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                            SoDienThoai = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                            DiaChi = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                            DonGiaDien = reader.IsDBNull(17) ? null : reader.GetDecimal(17),
                            DonGiaNuoc = reader.IsDBNull(18) ? null : reader.GetDecimal(18),
                            SoDien = reader.IsDBNull(19) ? null : reader.GetDecimal(19),
                            SoNuoc = reader.IsDBNull(20) ? null : reader.GetDecimal(20)
                        });
                    }
                }
            }
            return payments;
        }

        /// <summary>
        /// Helper method để xử lý giá trị TrangThaiThanhToan từ database
        /// </summary>
        private static string GetTrangThaiThanhToan(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Chưa trả";
            }
            
            var trimmed = value.Trim();
            // Chuẩn hóa giá trị để khớp với ENUM
            if (trimmed.Equals("Đã trả", StringComparison.OrdinalIgnoreCase) || 
                trimmed.Contains("Đã") || trimmed.Contains("đã"))
            {
                return "Đã trả";
            }
            else if (trimmed.Equals("Chưa trả", StringComparison.OrdinalIgnoreCase) || 
                     trimmed.Contains("Chưa") || trimmed.Contains("chưa"))
            {
                return "Chưa trả";
            }
            else
            {
                // Mặc định là "Chưa trả" nếu không khớp
                return "Chưa trả";
            }
        }
    }
}