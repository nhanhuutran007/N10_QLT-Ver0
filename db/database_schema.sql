-- =============================================
-- Script tạo cơ sở dữ liệu quản lý phòng trọ
-- =============================================

-- Tạo database (nếu cần)
-- CREATE DATABASE QLKDPhongTro;
-- USE QLKDPhongTro;

-- =============================================
-- 1. Bảng tblDangNhap (Bảng đăng nhập)
-- =============================================
CREATE TABLE tblDangNhap (
    Acc VARCHAR(50) PRIMARY KEY,
    TenDangNhap VARCHAR(100) NOT NULL UNIQUE,
    MatKhau VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE
);

-- =============================================
-- 2. Bảng tblPhongTro (Bảng phòng trọ)
-- =============================================
CREATE TABLE tblPhongTro (
    MaPhongTro VARCHAR(20) PRIMARY KEY,
    ToaNha VARCHAR(50) NOT NULL,
    Tang INT NOT NULL,
    SoPhong VARCHAR(10) NOT NULL,
    GiaPhong DECIMAL(15,2) NOT NULL,
    MoTa NVARCHAR(500),
    TrangThai VARCHAR(20) DEFAULT 'Trống' CHECK (TrangThai IN ('Trống', 'Đã thuê', 'Bảo trì'))
);

-- =============================================
-- 3. Bảng tblKhachThue (Bảng khách thuê)
-- =============================================
CREATE TABLE tblKhachThue (
    MaKhachThue VARCHAR(20) PRIMARY KEY,
    MaPhongTro VARCHAR(20),
    HoTen NVARCHAR(100) NOT NULL,
    GioiTinh VARCHAR(10) NOT NULL CHECK (GioiTinh IN ('Nam', 'Nữ', 'Khác')),
    NgaySinh DATE NOT NULL,
    QueQuan NVARCHAR(200),
    SoDienThoai VARCHAR(15) NOT NULL UNIQUE,
    CCCD VARCHAR(20) NOT NULL UNIQUE,
    NgayTao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaPhongTro) REFERENCES tblPhongTro(MaPhongTro)
);

-- =============================================
-- 4. Bảng tblHopDong (Bảng hợp đồng)
-- =============================================
CREATE TABLE tblHopDong (
    MaHopDong VARCHAR(20) PRIMARY KEY,
    MaPhongTro VARCHAR(20) NOT NULL,
    MaKhachThue VARCHAR(20) NOT NULL,
    HoTenNguoiKy NVARCHAR(100) NOT NULL,
    TienCoc DECIMAL(15,2) NOT NULL,
    NgayKy DATE NOT NULL,
    NgayHetHan DATE NOT NULL,
    GhiChu NVARCHAR(500),
    TrangThai VARCHAR(20) DEFAULT 'Có hiệu lực' CHECK (TrangThai IN ('Có hiệu lực', 'Hết hạn', 'Hủy bỏ')),
    FOREIGN KEY (MaPhongTro) REFERENCES tblPhongTro(MaPhongTro),
    FOREIGN KEY (MaKhachThue) REFERENCES tblKhachThue(MaKhachThue)
);

-- =============================================
-- 5. Bảng tblDienNuoc (Bảng điện nước)
-- =============================================
CREATE TABLE tblDienNuoc (
    MaDienNuoc VARCHAR(20) PRIMARY KEY,
    MaPhongTro VARCHAR(20) NOT NULL,
    ThangNam VARCHAR(7) NOT NULL, -- Format: YYYY-MM
    SoDienCu INT NOT NULL DEFAULT 0,
    SoDienMoi INT NOT NULL,
    TieuThuDien AS (SoDienMoi - SoDienCu) PERSISTED,
    TienDien DECIMAL(15,2) NOT NULL,
    SoNuocCu INT NOT NULL DEFAULT 0,
    SoNuocMoi INT NOT NULL,
    TieuThuNuoc AS (SoNuocMoi - SoNuocCu) PERSISTED,
    TienNuoc DECIMAL(15,2) NOT NULL,
    DonGiaDien DECIMAL(10,2) NOT NULL,
    DonGiaNuoc DECIMAL(10,2) NOT NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaPhongTro) REFERENCES tblPhongTro(MaPhongTro),
    UNIQUE (MaPhongTro, ThangNam)
);

-- =============================================
-- 6. Bảng tblHoaDon (Bảng hóa đơn)
-- =============================================
CREATE TABLE tblHoaDon (
    MaHoaDon VARCHAR(20) PRIMARY KEY,
    MaPhongTro VARCHAR(20) NOT NULL,
    ThangNam VARCHAR(7) NOT NULL, -- Format: YYYY-MM
    TienVeSinh DECIMAL(15,2) DEFAULT 0,
    Internet DECIMAL(15,2) DEFAULT 0,
    DichVuKhac DECIMAL(15,2) DEFAULT 0,
    KhuyenMai DECIMAL(15,2) DEFAULT 0,
    GhiChu NVARCHAR(500),
    TrangThai VARCHAR(20) DEFAULT 'Chưa thanh toán' CHECK (TrangThai IN ('Chưa thanh toán', 'Đã thanh toán', 'Quá hạn')),
    NgayTao DATETIME DEFAULT GETDATE(),
    NgayThanhToan DATETIME,
    FOREIGN KEY (MaPhongTro) REFERENCES tblPhongTro(MaPhongTro),
    UNIQUE (MaPhongTro, ThangNam)
);

-- =============================================
-- 7. Bảng tblChiTietHoaDon (Bảng chi tiết hóa đơn)
-- =============================================
CREATE TABLE tblChiTietHoaDon (
    STT INT IDENTITY(1,1),
    MaHoaDonCT VARCHAR(20) NOT NULL,
    TenDichVu NVARCHAR(200) NOT NULL,
    ChiSoCu INT DEFAULT 0,
    ChiSoMoi INT NOT NULL,
    SoLuong AS (ChiSoMoi - ChiSoCu) PERSISTED,
    DonGia DECIMAL(15,2) NOT NULL,
    ThanhTien AS ((ChiSoMoi - ChiSoCu) * DonGia) PERSISTED,
    PRIMARY KEY (STT, MaHoaDonCT),
    FOREIGN KEY (MaHoaDonCT) REFERENCES tblHoaDon(MaHoaDon) ON DELETE CASCADE
);

-- =============================================
-- Tạo các Index để tối ưu hiệu suất
-- =============================================

-- Index cho bảng tblKhachThue
CREATE INDEX IX_tblKhachThue_MaPhongTro ON tblKhachThue(MaPhongTro);
CREATE INDEX IX_tblKhachThue_SoDienThoai ON tblKhachThue(SoDienThoai);
CREATE INDEX IX_tblKhachThue_CCCD ON tblKhachThue(CCCD);

-- Index cho bảng tblHopDong
CREATE INDEX IX_tblHopDong_MaPhongTro ON tblHopDong(MaPhongTro);
CREATE INDEX IX_tblHopDong_MaKhachThue ON tblHopDong(MaKhachThue);
CREATE INDEX IX_tblHopDong_NgayKy ON tblHopDong(NgayKy);

-- Index cho bảng tblDienNuoc
CREATE INDEX IX_tblDienNuoc_MaPhongTro ON tblDienNuoc(MaPhongTro);
CREATE INDEX IX_tblDienNuoc_ThangNam ON tblDienNuoc(ThangNam);

-- Index cho bảng tblHoaDon
CREATE INDEX IX_tblHoaDon_MaPhongTro ON tblHoaDon(MaPhongTro);
CREATE INDEX IX_tblHoaDon_ThangNam ON tblHoaDon(ThangNam);
CREATE INDEX IX_tblHoaDon_TrangThai ON tblHoaDon(TrangThai);

-- Index cho bảng tblChiTietHoaDon
CREATE INDEX IX_tblChiTietHoaDon_MaHoaDonCT ON tblChiTietHoaDon(MaHoaDonCT);

GO

-- =============================================
-- Tạo các Trigger để tự động cập nhật
-- =============================================

-- Trigger cập nhật trạng thái phòng khi có khách thuê
CREATE TRIGGER TR_tblKhachThue_UpdatePhongTrangThai
ON tblKhachThue
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Cập nhật trạng thái phòng thành "Đã thuê" khi có khách thuê
    UPDATE pt 
    SET TrangThai = 'Đã thuê'
    FROM tblPhongTro pt
    INNER JOIN inserted i ON pt.MaPhongTro = i.MaPhongTro
    WHERE i.MaPhongTro IS NOT NULL;
    
    -- Cập nhật trạng thái phòng thành "Trống" khi không còn khách thuê
    UPDATE pt 
    SET TrangThai = 'Trống'
    FROM tblPhongTro pt
    LEFT JOIN tblKhachThue kt ON pt.MaPhongTro = kt.MaPhongTro
    WHERE kt.MaPhongTro IS NULL;
END;

GO

-- =============================================
-- Thêm dữ liệu mẫu (nếu cần)
-- =============================================

-- Thêm tài khoản admin mặc định
INSERT INTO tblDangNhap (Acc, TenDangNhap, MatKhau, Email) 
VALUES ('ADMIN001', 'admin', 'admin123', 'admin@phongtro.com');

-- Thêm một số phòng trọ mẫu
INSERT INTO tblPhongTro (MaPhongTro, ToaNha, Tang, SoPhong, GiaPhong, MoTa) VALUES
('PT001', 'Tòa A', 1, '101', 2000000, 'Phòng 1 giường, có điều hòa'),
('PT002', 'Tòa A', 1, '102', 2500000, 'Phòng 2 giường, có điều hòa, tủ lạnh'),
('PT003', 'Tòa A', 2, '201', 3000000, 'Phòng 2 giường, đầy đủ tiện nghi'),
('PT004', 'Tòa B', 1, '101', 1800000, 'Phòng 1 giường, cơ bản'),
('PT005', 'Tòa B', 2, '201', 2200000, 'Phòng 1 giường, có điều hòa');

-- =============================================
-- Kết thúc script
-- =============================================
PRINT 'Đã tạo thành công cơ sở dữ liệu quản lý phòng trọ!';


-- =============================================
-- 5 dữ liệu mẫu tất cả các bảng
-- =============================================
-- Dữ liệu mẫu cho bảng tblKhachThue
INSERT INTO tblKhachThue (MaKhachThue, MaPhongTro, HoTen, GioiTinh, NgaySinh, QueQuan, SoDienThoai, CCCD) VALUES
('KT001', 'PT001', 'Nguyễn Văn A', 'Nam', '1995-05-15', 'Hà Nội', '0901234567', '123456789'),
('KT002', 'PT002', 'Trần Thị B', 'Nữ', '1998-08-20', 'Hồ Chí Minh', '0902345678', '987654321'),
('KT003', 'PT003', 'Lê Văn C', 'Nam', '1990-12-10', 'Đà Nẵng', '0903456789', '456789123'),
('KT004', 'PT004', 'Phạm Thị D', 'Nữ', '1992-03-25', 'Cần Thơ', '0904567890', '789123456'),
('KT005', 'PT005', 'Hoàng Văn E', 'Nam', '1988-07-30', 'Hải Phòng', '0905678901', '321654987'); 
-- Dữ liệu mẫu cho bảng tblHopDong
INSERT INTO tblHopDong (MaHopDong, MaPhongTro, MaKhachThue
, HoTenNguoiKy, TienCoc, NgayKy, NgayHetHan, GhiChu) VALUES
('HD001', 'PT001', 'KT001', 'Nguyễn Văn A', 2000000, '2023-01-01', '2023-12-31', 'Hợp đồng 1 năm'),
('HD002', 'PT002', 'KT002', 'Trần Thị B', 2500000, '2023-02-01', '2024-01-31', 'Hợp đồng 1 năm'),
('HD003', 'PT003', 'KT003', 'Lê Văn C', 3000000, '2023-03-01', '2024-02-28', 'Hợp đồng 1 năm'),
('HD004', 'PT004', 'KT004', 'Phạm Thị D', 1800000, '2023-04-01', '2024-03-31', 'Hợp đồng 1 năm'),
('HD005', 'PT005', 'KT005', 'Hoàng Văn E', 2200000, '2023-05-01', '2024-04-30', 'Hợp đồng 1 năm');
-- Dữ liệu mẫu cho bảng tblDienNuoc
INSERT INTO tblDienNuoc (MaDienNuoc, MaPhongTro, ThangNam, SoDienCu, SoDienMoi, TienDien, SoNuocCu, SoNuocMoi, TienNuoc, DonGiaDien, DonGiaNuoc) VALUES
('DN001', 'PT001', '2023-01', 100, 150, 1000000, 50, 70, 400000, 20000, 20000),
('DN002', 'PT002', '2023-01', 200, 260, 1200000, 80, 100, 400000, 20000, 20000),
('DN003', 'PT003', '2023-01', 300, 370, 1400000, 100, 130, 600000, 20000, 20000),
('DN004', 'PT004', '2023-01', 150, 190, 800000, 60, 80, 400000, 20000, 20000),
('DN005', 'PT005', '2023-01', 250, 310, 1200000, 90, 120, 600000, 20000, 20000);
-- Dữ liệu mẫu cho bảng tblHoaDon
INSERT INTO tblHoaDon (MaHoaDon, MaPhongTro, ThangNam, TienVeSinh, Internet, DichVuKhac, KhuyenMai, GhiChu, TrangThai, NgayTao, NgayThanhToan) VALUES
('HDN001', 'PT001', '2023-01', 50000, 100000, 0, 0, 'Hóa đơn tháng 1', 'Đã thanh toán', '2023-01-05', '2023-01-10'),
('HDN002', 'PT002', '2023-01', 50000, 100000, 0, 0, 'Hóa đơn tháng 1', 'Chưa thanh toán', '2023-01-05', NULL),
('HDN003', 'PT003', '2023-01', 50000, 100000, 0, 0, 'Hóa đơn tháng 1', 'Chưa thanh toán', '2023-01-05', NULL),
('HDN004', 'PT004', '2023-01', 50000, 100000, 0, 0, 'Hóa đơn tháng 1', 'Đã thanh toán', '2023-01-05', '2023-01-12'),
('HDN005', 'PT005', '2023-01', 50000, 100000, 0, 0, 'Hóa đơn tháng 1', 'Chưa thanh toán', '2023-01-05', NULL);
-- Dữ liệu mẫu cho bảng tblChiTietHoaDon
INSERT INTO tblChiTietHoaDon (MaHoaDonCT, TenDichVu, ChiSoCu, ChiSoMoi, DonGia) VALUES
('HDN001', 'Điện', 100, 150, 20000),
('HDN001', 'Nước', 50, 70, 20000),
('HDN002', 'Điện', 200, 260, 20000),
('HDN002', 'Nước', 80, 100, 20000),
('HDN003', 'Điện', 300, 370, 20000),
('HDN003', 'Nước', 100, 130, 20000),
('HDN004', 'Điện', 150, 190, 20000),
('HDN004', 'Nước', 60, 80, 20000),
('HDN005', 'Điện', 250, 310, 20000),
('HDN005', 'Nước', 90, 120, 20000);


