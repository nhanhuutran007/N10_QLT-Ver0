-- ============================================================
-- Script kiểm tra và debug dữ liệu hợp đồng
-- Email: nhanhuutran007@gmail.com
-- ============================================================

-- 1. Kiểm tra User có tồn tại không
SELECT 'Kiểm tra User' AS Step;
SELECT * FROM User WHERE Email = 'nhanhuutran007@gmail.com';

-- 2. Kiểm tra NguoiThue (Tenant) có tồn tại không
SELECT 'Kiểm tra NguoiThue' AS Step;
SELECT * FROM NguoiThue WHERE Email = 'nhanhuutran007@gmail.com';

-- 3. Kiểm tra HopDong (Contract) liên kết với NguoiThue
SELECT 'Kiểm tra HopDong' AS Step;
SELECT 
    hd.*,
    nt.HoTen,
    nt.Email,
    p.TenPhong,
    p.GiaCoBan
FROM HopDong hd
INNER JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
WHERE nt.Email = 'nhanhuutran007@gmail.com';

-- 4. Kiểm tra HopDong với trạng thái "Hiệu lực"
SELECT 'Kiểm tra HopDong Hiệu lực' AS Step;
SELECT 
    hd.MaHopDong,
    hd.MaNguoiThue,
    hd.TrangThai,
    nt.Email
FROM HopDong hd
INNER JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
WHERE nt.Email = 'nhanhuutran007@gmail.com'
  AND hd.TrangThai = 'Hiệu lực';

-- 5. Kiểm tra tất cả HopDong (bất kể trạng thái)
SELECT 'Tất cả HopDong của User' AS Step;
SELECT 
    hd.MaHopDong,
    hd.MaNguoiThue,
    hd.MaPhong,
    hd.NgayBatDau,
    hd.NgayKetThuc,
    hd.TienCoc,
    hd.TrangThai,
    nt.HoTen,
    nt.Email,
    p.TenPhong,
    p.GiaCoBan
FROM NguoiThue nt
LEFT JOIN HopDong hd ON nt.MaNguoiThue = hd.MaNguoiThue
LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
WHERE nt.Email = 'nhanhuutran007@gmail.com';

-- ============================================================
-- HƯỚNG DẪN DEBUG:
-- ============================================================
-- Chạy từng SELECT ở trên và kiểm tra kết quả:
--
-- Nếu Step 1 không có kết quả:
--   → User chưa tồn tại, cần tạo tài khoản User
--
-- Nếu Step 2 không có kết quả:
--   → NguoiThue chưa tồn tại, cần chạy script test_user_housing_data.sql
--   → Chạy OPTION 1 hoặc OPTION 2 trong file đó
--
-- Nếu Step 3 không có kết quả:
--   → HopDong chưa được tạo, kiểm tra lại script test
--   → Có thể do @MaNguoiThue không được set đúng
--
-- Nếu Step 4 không có kết quả nhưng Step 5 có:
--   → HopDong tồn tại nhưng TrangThai không phải "Hiệu lực"
--   → Cần UPDATE TrangThai về "Hiệu lực"
--
-- ============================================================
