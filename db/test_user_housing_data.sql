-- ============================================================
-- Script tạo dữ liệu test cho tài khoản User thuê phòng
-- Email: nhanhuutran007@gmail.com
-- ============================================================

-- LƯU Ý: Phòng P.109 hiện đang có trạng thái "Đang thuê"
-- Bạn có 2 lựa chọn:
--   OPTION 1: Xóa người thuê cũ của P.109 (chạy phần A)
--   OPTION 2: Chọn phòng trống khác như P.107, P.108, P.110 (chạy phần B)

-- ============================================================
-- OPTION 1: Sử dụng phòng P.109 (xóa người thuê cũ)
-- ============================================================

-- A1. Xóa người thuê cũ của phòng P.109 (nếu có)
DELETE FROM NguoiThue WHERE MaPhong = 36;

-- A2. Cập nhật trạng thái phòng về "Trống"
UPDATE Phong SET TrangThai = 'Trống' WHERE MaPhong = 36;

-- A3. Thêm người thuê mới với Email trùng với User
INSERT INTO NguoiThue (
    MaPhong,
    HoTen,
    SoDienThoai,
    CCCD,
    Email,
    GioiTinh,
    NgayBatDau,
    TrangThai,
    NgaySinh,
    DiaChi,
    NgheNghiep,
    NgayCap,
    NoiCap
) VALUES (
    36,                                    -- MaPhong = 36 (P.109)
    'Trần Hữu Nhân',                      -- HoTen từ profile
    '0869918250',                         -- SoDienThoai từ profile
    '087205004877',                       -- CCCD từ profile
    'nhanhuutran007@gmail.com',           -- Email - PHẢI TRÙNG với User.Email
    'Nam',
    '2026-01-18',                         -- NgayBatDau (hôm nay)
    'Đang ở',
    '2005-08-26',                         -- NgaySinh (8/26/2005)
    'TP. Hồ Chí Minh',
    'Sinh viên',
    '2022-03-02',                         -- NgayCap (3/2/2022)
    'Cục cảnh sát'
);

-- A4. Lấy MaNguoiThue vừa tạo
SET @MaNguoiThue = LAST_INSERT_ID();

-- A5. Tạo hợp đồng
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
    36,                                    -- P.109
    '2026-01-18',
    '2027-01-18',                         -- 1 năm
    5000000,                              -- Tiền cọc (2 tháng)
    'Hiệu lực',
    'Hợp đồng test cho User - Phòng P.109'
);

-- A6. Cập nhật trạng thái phòng thành "Đang thuê"
UPDATE Phong SET TrangThai = 'Đang thuê' WHERE MaPhong = 36;


-- ============================================================
-- OPTION 2: Sử dụng phòng trống (P.107 - 3 triệu/tháng)
-- ============================================================
-- Nếu không muốn xóa dữ liệu cũ, dùng phòng trống thay thế

-- B1. Thêm người thuê với phòng P.107 (MaPhong = 34)
INSERT INTO NguoiThue (
    MaPhong,
    HoTen,
    SoDienThoai,
    CCCD,
    Email,
    GioiTinh,
    NgayBatDau,
    TrangThai,
    NgaySinh,
    DiaChi,
    NgheNghiep,
    NgayCap,
    NoiCap
) VALUES (
    34,                                    -- MaPhong = 34 (P.107)
    'Trần Hữu Nhân',
    '0869918250',
    '087205004877',
    'nhanhuutran007@gmail.com',           -- Email - ĐIỂM QUAN TRỌNG
    'Nam',
    '2026-01-18',
    'Đang ở',
    '2005-08-26',
    'TP. Hồ Chí Minh',
    'Sinh viên',
    '2022-03-02',
    'Cục cảnh sát'
);

-- B2. Lấy MaNguoiThue vừa tạo
SET @MaNguoiThue2 = LAST_INSERT_ID();

-- B3. Tạo hợp đồng cho P.107
INSERT INTO HopDong (
    MaNguoiThue,
    MaPhong,
    NgayBatDau,
    NgayKetThuc,
    TienCoc,
    TrangThai,
    GhiChu
) VALUES (
    @MaNguoiThue2,
    34,                                    -- P.107
    '2026-01-18',
    '2027-01-18',
    6000000,                              -- Tiền cọc
    'Hiệu lực',
    'Hợp đồng test cho User - Phòng P.107'
);

-- B4. Cập nhật trạng thái phòng
UPDATE Phong SET TrangThai = 'Đang thuê' WHERE MaPhong = 34;


-- ============================================================
-- KIỂM TRA KẾT QUẢ
-- ============================================================

-- Xem thông tin người thuê vừa tạo
SELECT 
    nt.MaNguoiThue,
    nt.HoTen,
    nt.Email,
    nt.SoDienThoai,
    nt.CCCD,
    p.TenPhong,
    p.DienTich,
    p.GiaCoBan,
    p.GiaBangChu,
    p.TrangThietBi,
    p.TrangThai AS TrangThaiPhong,
    nt.TrangThai AS TrangThaiNguoiThue,
    n.DiaChi AS DiaChiNha,
    n.TinhThanh,
    n.GhiChu AS GhiChuNha,
    hd.MaHopDong,
    hd.NgayBatDau AS NgayBatDauHD,
    hd.NgayKetThuc AS NgayKetThucHD,
    hd.TienCoc,
    hd.TrangThai AS TrangThaiHopDong
FROM NguoiThue nt
INNER JOIN Phong p ON nt.MaPhong = p.MaPhong
INNER JOIN Nha n ON p.MaNha = n.MaNha
LEFT JOIN HopDong hd ON nt.MaNguoiThue = hd.MaNguoiThue
WHERE nt.Email = 'nhanhuutran007@gmail.com';

-- ============================================================
-- HƯỚNG DẪN SỬ DỤNG:
-- ============================================================
-- 1. Chọn OPTION 1 hoặc OPTION 2 (chạy phần A hoặc B, không chạy cả 2)
-- 2. Copy và chạy các câu lệnh SQL trong MySQL/MariaDB
-- 3. Chạy SELECT cuối cùng để kiểm tra kết quả
-- 4. Đăng nhập vào ứng dụng với tài khoản User
-- 5. Vào menu "Thông tin nhà ở" để xem thông tin phòng
--
-- QUAN TRỌNG:
-- - Email trong NguoiThue PHẢI là 'nhanhuutran007@gmail.com'
-- - Đây là điểm kết nối giữa User và thông tin thuê phòng
-- ============================================================

