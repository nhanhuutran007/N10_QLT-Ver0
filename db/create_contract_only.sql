-- ============================================================
-- Script tạo HopDong (Contract) cho tài khoản User đã có NguoiThue
-- Email: nhanhuutran007@gmail.com
-- ============================================================

-- Bước 1: Lấy MaNguoiThue từ email
SET @MaNguoiThue = (
    SELECT MaNguoiThue 
    FROM NguoiThue 
    WHERE Email = 'nhanhuutran007@gmail.com' 
    LIMIT 1
);

-- Bước 2: Lấy MaPhong từ NguoiThue
SET @MaPhong = (
    SELECT MaPhong 
    FROM NguoiThue 
    WHERE Email = 'nhanhuutran007@gmail.com' 
    LIMIT 1
);

-- Bước 3: Kiểm tra xem đã có hợp đồng chưa
SELECT 
    CONCAT('MaNguoiThue: ', IFNULL(@MaNguoiThue, 'NULL')) AS Info1,
    CONCAT('MaPhong: ', IFNULL(@MaPhong, 'NULL')) AS Info2;

-- Bước 4: Xóa hợp đồng cũ nếu có (tùy chọn)
-- DELETE FROM HopDong WHERE MaNguoiThue = @MaNguoiThue;

-- Bước 5: Tạo hợp đồng mới
INSERT INTO HopDong (
    MaNguoiThue,
    MaPhong,
    NgayBatDau,
    NgayKetThuc,
    TienCoc,
    TrangThai,
    GhiChu
) VALUES (
    @MaNguoiThue,
    @MaPhong,
    '2026-01-18',                         -- Ngày bắt đầu (hôm nay)
    '2027-01-18',                         -- Ngày kết thúc (1 năm)
    5000000,                              -- Tiền cọc (5 triệu)
    'Hiệu lực',                           -- Trạng thái
    'Hợp đồng test cho User'              -- Ghi chú
);

-- Bước 6: Kiểm tra kết quả
SELECT 
    hd.MaHopDong,
    hd.MaNguoiThue,
    hd.MaPhong,
    hd.NgayBatDau,
    hd.NgayKetThuc,
    hd.TienCoc,
    hd.TrangThai,
    hd.GhiChu,
    nt.HoTen,
    nt.Email,
    p.TenPhong,
    p.GiaCoBan
FROM HopDong hd
INNER JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
WHERE nt.Email = 'nhanhuutran007@gmail.com'
ORDER BY hd.MaHopDong DESC;

-- ============================================================
-- HƯỚNG DẪN:
-- ============================================================
-- 1. Chạy toàn bộ script này trong MySQL/MariaDB
-- 2. Kiểm tra kết quả ở SELECT cuối cùng
-- 3. Nếu thấy dữ liệu hợp đồng, restart ứng dụng
-- 4. Đăng nhập và vào menu "Hợp đồng"
-- ============================================================
