-- Create and use the database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'database_v2')
BEGIN
    CREATE DATABASE database_v2;
END;
GO

USE database_v2;
GO

-- ===================== Bảng Admin =====================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Admin]') AND type in (N'U'))
CREATE TABLE Admin (
  MaAdmin INT IDENTITY(1,1) PRIMARY KEY,
  TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
  MatKhau VARCHAR(255) NOT NULL,
  Email VARCHAR(100),
  SoDienThoai VARCHAR(15)
);

IF NOT EXISTS (SELECT 1 FROM Admin WHERE TenDangNhap = 'admin')
INSERT INTO Admin (TenDangNhap, MatKhau, Email, SoDienThoai)
VALUES ('admin', 'admin123', 'admin@example.com', '0901000001');
GO

-- ===================== Bảng Nha =====================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Nha]') AND type in (N'U'))
CREATE TABLE Nha (
  MaNha INT IDENTITY(1,1) PRIMARY KEY,
  DiaChi NVARCHAR(255) NOT NULL,
  TongSoPhong INT CHECK (TongSoPhong BETWEEN 1 AND 10), 
  GhiChu NVARCHAR(255)
);

IF NOT EXISTS (SELECT 1 FROM Nha WHERE DiaChi = N'123 Đường A, Quận 1, TP.HCM')
INSERT INTO Nha (DiaChi, TongSoPhong, GhiChu)
VALUES
(N'123 Đường A, Quận 1, TP.HCM', 5, N'Nhà trung tâm'),
(N'456 Đường B, Quận 7, TP.HCM', 6, N'Gần khu công nghệ'),
(N'789 Đường C, Bình Thạnh', 4, N'Khu yên tĩnh'),
(N'12 Nguyễn Văn Linh, Quận 7', 8, N'Gần siêu thị'),
(N'99 Lý Thường Kiệt, Quận 10', 10, N'Gần trường học');
GO

-- ===================== Bảng Phong (Đã cập nhật) =====================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Phong]') AND type in (N'U'))
CREATE TABLE Phong (
  MaPhong INT IDENTITY(1,1) PRIMARY KEY,
  MaNha INT,
  TenPhong NVARCHAR(50) NOT NULL,
  DienTich DECIMAL(5,2) CHECK (DienTich > 0),
  GiaCoBan DECIMAL(18,0) DEFAULT 0 CHECK (GiaCoBan >= 0),
  TrangThai NVARCHAR(20) DEFAULT N'Trống' CHECK (TrangThai IN (N'Đang thuê', N'Trống')),
  GhiChu NVARCHAR(255),
  
  -- Cột mới từ RentedRoom
  GiaBangChu NVARCHAR(255) NULL,
  TrangThietBi NVARCHAR(500) NULL,

  FOREIGN KEY (MaNha) REFERENCES Nha(MaNha) ON DELETE CASCADE
);

IF NOT EXISTS (SELECT 1 FROM Phong WHERE MaNha = 1 AND TenPhong = N'P101')
INSERT INTO Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, GiaBangChu, TrangThietBi)
VALUES
(1, N'P101', 18.5, 3000000, N'Trống', N'Có cửa sổ', N'Ba triệu đồng', N'Điều hòa, Nóng lạnh'),
(1, N'P102', 20.0, 3500000, N'Trống', N'Gần cầu thang', N'Ba triệu năm trăm nghìn đồng', N'Điều hòa'),
(2, N'P201', 22.0, 3800000, N'Trống', N'Ban công nhỏ', N'Ba triệu tám trăm nghìn đồng', N'Full nội thất'),
(3, N'P301', 25.0, 4000000, N'Trống', N'Có ban công lớn', N'Bốn triệu đồng', N'Không có'),
(4, N'P401', 28.0, 4500000, N'Trống', N'Phòng mới xây', N'Bốn triệu năm trăm nghìn đồng', N'Điều hòa, Tủ lạnh');
GO

-- ===================== Bảng NguoiThue (ĐÃ CẬP NHẬT) =====================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NguoiThue]') AND type in (N'U'))
CREATE TABLE NguoiThue (
  MaNguoiThue INT IDENTITY(1,1) PRIMARY KEY,
  HoTen NVARCHAR(100) NOT NULL,
  SoDienThoai VARCHAR(15) NOT NULL,
  CCCD VARCHAR(20) UNIQUE NOT NULL,
  Email VARCHAR(100) NULL,
  GioiTinh NVARCHAR(10) NULL CHECK (GioiTinh IN (N'Nam', N'Nữ', N'Khác')),
  NgheNghiep NVARCHAR(100) NULL,
  GhiChu NVARCHAR(500) NULL,
  
  -- Các trường mới từ TenantDto
  NgaySinh DATE NULL,
  NgayCap DATE NULL,
  NoiCap NVARCHAR(100) NULL,
  DiaChi NVARCHAR(255) NULL,
  
  -- Các trường audit
  NgayTao DATETIME DEFAULT GETDATE(),
  NgayCapNhat DATETIME DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT 1 FROM NguoiThue WHERE CCCD = '079123456001')
INSERT INTO NguoiThue (HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, NgaySinh, NgayCap, NoiCap, DiaChi, GhiChu)
VALUES
(N'Nguyễn Văn A', '0911000001', '079123456001', N'vana@example.com', N'Nam', N'Sinh viên', '2000-01-15', '2020-05-10', N'CA TPHCM', N'123 Nguyễn Trãi, Q1, TPHCM', N''),
(N'Trần Thị B', '0911000002', '079123456002', N'thib@example.com', N'Nữ', N'Nhân viên văn phòng', '1998-11-20', '2019-02-20', N'CA Hà Nội', N'456 Lê Lợi, Q3, TPHCM', N''),
(N'Lê Văn C', '0911000003', '079123456003', N'vanc@example.com', N'Nam', N'Kỹ sư phần mềm', '1995-07-30', '2018-10-01', N'CA Đà Nẵng', N'789 CMT8, Q10, TPHCM', N''),
(N'Phạm Thị D', '0911000004', '079123456004', N'thid@example.com', N'Nữ', N'Thiết kế', '2002-03-05', '2021-01-01', N'CA TPHCM', N'101 Hai Bà Trưng, Q1, TPHCM', N'Chuyển đi'),
(N'Huỳnh Văn E', '0911000005', '079123456005', N'vane@example.com', N'Nam', N'Tài xế', '1990-12-12', '2015-06-15', N'CA Cần Thơ', N'202 Võ Thị Sáu, Q3, TPHCM', N'');
GO
-- =========================================================================

-- ===================== Bảng HopDong =====================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HopDong]') AND type in (N'U'))
CREATE TABLE HopDong (
  MaHopDong INT IDENTITY(1,1) PRIMARY KEY,
  MaNguoiThue INT,
  MaPhong INT,
  NgayBatDau DATE NOT NULL,
  NgayKetThuc DATE NOT NULL,
  TienCoc DECIMAL(18,0) DEFAULT 0 CHECK (TienCoc >= 0),
  FileHopDong VARCHAR(255),
  TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Hiệu lực', N'Hết hạn', N'Hủy')),
  FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE,
  FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong) ON DELETE CASCADE,
  CONSTRAINT CK_NgayKetThucHopLe CHECK (NgayKetThuc > NgayBatDau)
);

IF NOT EXISTS (SELECT 1 FROM HopDong WHERE MaNguoiThue = 1 AND MaPhong = 1)
INSERT INTO HopDong (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, FileHopDong, TrangThai)
VALUES
(1, 1, '2025-01-01', '2025-07-01', 1000000, 'HD_A.pdf', N'Hiệu lực'),
(2, 2, '2025-02-01', '2025-08-01', 1500000, 'HD_B.pdf', N'Hiệu lực'),
(3, 3, '2025-03-01', '2025-09-01', 1200000, 'HD_C.pdf', N'Hiệu lực'),
(4, 4, '2025-04-01', '2025-10-01', 1000000, 'HD_D.pdf', N'Hủy'),
(5, 5, '2025-05-01', '2025-11-01', 1000000, 'HD_E.pdf', N'Hết hạn');
GO

-- ===================== Bảng TaiSanNguoiThue =====================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaiSanNguoiThue]') AND type in (N'U'))
CREATE TABLE TaiSanNguoiThue (
  MaTaiSan INT IDENTITY(1,1) PRIMARY KEY,
  MaNguoiThue INT,
  LoaiTaiSan NVARCHAR(20) CHECK (LoaiTaiSan IN (N'Xe', N'Thú cưng')),
  MoTa NVARCHAR(255),
  PhiPhuThu DECIMAL(18,0) DEFAULT 0,
  FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE
);

IF NOT EXISTS (SELECT 1 FROM TaiSanNguoiThue WHERE MaNguoiThue = 1 AND LoaiTaiSan = N'Xe')
INSERT INTO TaiSanNguoiThue (MaNguoiThue, LoaiTaiSan, MoTa, PhiPhuThu)
VALUES
(1, N'Xe', N'Xe máy Vision', 100000),
(2, N'Thú cưng', N'Mèo Anh lông ngắn', 200000),
(3, N'Xe', N'Xe máy Sirius', 100000),
(4, N'Thú cưng', N'Chó Poodle', 150000),
(5, N'Xe', N'Xe SH Mode', 200000);
GO

-- ===================== Bảng BaoTri_SuCo =====================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BaoTri_SuCo]') AND type in (N'U'))
CREATE TABLE BaoTri_SuCo (
  MaSuCo INT IDENTITY(1,1) PRIMARY KEY,
  MaPhong INT,
  MoTaSuCo NVARCHAR(255) NOT NULL,
  NgayBaoCao DATE DEFAULT (CONVERT(date, GETDATE())),
  TrangThai NVARCHAR(20) DEFAULT N'Chưa xử lý' CHECK (TrangThai IN (N'Chưa xử lý', N'Đang xử lý', N'Hoàn tất')),
  ChiPhi DECIMAL(18,0) DEFAULT 0,
  FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong) ON DELETE CASCADE
);

IF NOT EXISTS (SELECT 1 FROM BaoTri_SuCo WHERE MaPhong = 1 AND MoTaSuCo = N'Hư vòi nước')
INSERT INTO BaoTri_SuCo (MaPhong, MoTaSuCo, TrangThai, ChiPhi)
VALUES
(1, N'Hư vòi nước', N'Chưa xử lý', 0),
(2, N'Rò rỉ điện', N'Đang xử lý', 0),
(3, N'Máy lạnh hỏng', N'Hoàn tất', 300000),
(4, N'Cửa bị kẹt', N'Chưa xử lý', 0),
(5, N'Nước yếu', N'Đang xử lý', 0);
GO

-- ===================== Bảng ThanhToan (Logic tính toán tự động) =====================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ThanhToan]') AND type in (N'U'))
CREATE TABLE ThanhToan (
  MaThanhToan INT IDENTITY(1,1) PRIMARY KEY,
  MaHopDong INT,
  ThangNam CHAR(7) NOT NULL,
  TienThue DECIMAL(18,0) DEFAULT 0,
  TienInternet DECIMAL(18,0) DEFAULT 0,
  TienVeSinh DECIMAL(18,0) DEFAULT 0,
  TienGiuXe DECIMAL(18,0) DEFAULT 0,
  ChiPhiKhac DECIMAL(18,0) DEFAULT 0,
  DonGiaDien DECIMAL(18,0) DEFAULT 3500,
  DonGiaNuoc DECIMAL(18,0) DEFAULT 100000, 
  ChiSoDienCu DECIMAL(18,2) DEFAULT NULL,
  ChiSoDienMoi DECIMAL(18,2) DEFAULT NULL,
  SoNuoc DECIMAL(18,2) DEFAULT 1,
  SoDien AS (CASE 
                  WHEN ChiSoDienMoi IS NOT NULL AND ChiSoDienCu IS NOT NULL AND ChiSoDienMoi >= ChiSoDienCu 
                  THEN ChiSoDienMoi - ChiSoDienCu 
                  ELSE NULL 
                END),
  TienDien AS (CAST(CASE 
                          WHEN (ChiSoDienMoi - ChiSoDienCu) > 0 
                          THEN (ChiSoDienMoi - ChiSoDienCu) * ISNULL(DonGiaDien, 3500) 
                          ELSE 0 
                        END AS DECIMAL(18,0))) PERSISTED,
  TienNuoc AS (CAST(ISNULL(DonGiaNuoc, 100000) AS DECIMAL(18,0))) PERSISTED,
  TongTien AS (
    COALESCE(TienThue, 0) + 
    COALESCE(TienDien, 0) + 
    COALESCE(TienNuoc, 0) + 
    COALESCE(TienInternet, 0) + 
    COALESCE(TienVeSinh, 0) + 
    COALESCE(TienGiuXe, 0) + 
    COALESCE(ChiPhiKhac, 0)
  ) PERSISTED,
  TrangThaiThanhToan NVARCHAR(20) DEFAULT N'Chưa trả' CHECK (TrangThaiThanhToan IN (N'Chưa trả', N'Đã trả')),
  NgayThanhToan DATE,
  NgayTao DATETIME DEFAULT GETDATE(),
  GhiChu NVARCHAR(500),
  FOREIGN KEY (MaHopDong) REFERENCES HopDong(MaHopDong) ON DELETE CASCADE,
  UNIQUE (MaHopDong, ThangNam),
  CONSTRAINT CK_ChiSoDienHopLe CHECK (ChiSoDienMoi IS NULL OR ChiSoDienCu IS NULL OR ChiSoDienMoi >= ChiSoDienCu)
);
GO

IF NOT EXISTS (SELECT 1 FROM ThanhToan WHERE MaHopDong = 1 AND ThangNam = '01/2025')
INSERT INTO ThanhToan (
    MaHopDong, ThangNam, TienThue, TienInternet, TienVeSinh, TienGiuXe, 
    ChiSoDienCu, ChiSoDienMoi, ChiPhiKhac, TrangThaiThanhToan, NgayThanhToan, GhiChu
)
VALUES
(1, '01/2025', 3000000, 100000, 50000, 100000, 100, 150, 0, N'Đã trả', '2025-01-05', N'Thanh toán đầy đủ'),
(2, '02/2025', 3500000, 100000, 60000, 120000, 150, 200, 0, N'Đã trả', '2025-02-05', N'Khách hàng mới'),
(3, '03/2025', 3800000, 100000, 50000, 100000, 130, 180, 50000, N'Chưa trả', NULL, N'Có chi phí phát sinh'),
(4, '04/2025', 4000000, 100000, 60000, 120000, 180, 220, 0, N'Chưa trả', NULL, N'Chờ xác nhận'),
(5, '05/2025', 4500000, 100000, 60000, 120000, 200, 250, 100000, N'Đã trả', '2025-05-06', N'Có chi phí sửa chữa');
GO

-- ===================== Bảng GoogleFormLog =====================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GoogleFormLog]') AND type in (N'U'))
CREATE TABLE GoogleFormLog (
    MaLog INT IDENTITY(1,1) PRIMARY KEY,
    RoomName VARCHAR(50) NOT NULL,
    ElectricImageUrl VARCHAR(500),
    SubmittedValue DECIMAL(18,2),
    ExtractedValue DECIMAL(18,2),
    IsValid BIT DEFAULT 0, 
    ErrorMessage VARCHAR(500),
    Timestamp DATETIME DEFAULT GETDATE(),
    Processed BIT DEFAULT 0
);
GO
USE database_v2;
GO

-- 1. Xóa bảng ThanhToan cũ (để tạo lại cấu trúc đúng)
IF OBJECT_ID('dbo.ThanhToan', 'U') IS NOT NULL
    DROP TABLE dbo.ThanhToan;
GO

-- 2. Tạo lại bảng ThanhToan (Bỏ Computed Columns, dùng cột thường)
CREATE TABLE ThanhToan (
  MaThanhToan INT IDENTITY(1,1) PRIMARY KEY,
  MaHopDong INT,
  ThangNam CHAR(7) NOT NULL, -- Định dạng MM/yyyy
  
  -- Các khoản tiền (C# sẽ tính và đẩy vào đây)
  TienThue DECIMAL(18,0) DEFAULT 0,
  TienInternet DECIMAL(18,0) DEFAULT 0,
  TienVeSinh DECIMAL(18,0) DEFAULT 0,
  TienGiuXe DECIMAL(18,0) DEFAULT 0,
  ChiPhiKhac DECIMAL(18,0) DEFAULT 0,
  
  -- Điện Nước
  DonGiaDien DECIMAL(18,0) DEFAULT 3500,
  DonGiaNuoc DECIMAL(18,0) DEFAULT 100000, 
  
  ChiSoDienCu DECIMAL(18,2) DEFAULT 0, -- Lấy từ tháng trước
  ChiSoDienMoi DECIMAL(18,2) DEFAULT 0, -- Lấy từ Google Form
  
  SoDien DECIMAL(18,2) DEFAULT 0, -- C# tính: Mới - Cũ
  SoNuoc DECIMAL(18,2) DEFAULT 1,
  
  -- Cột thường (Không dùng AS ... PERSISTED nữa)
  TienDien DECIMAL(18,0) DEFAULT 0, 
  TienNuoc DECIMAL(18,0) DEFAULT 0,
  TongTien DECIMAL(18,0) DEFAULT 0,

  TrangThaiThanhToan NVARCHAR(20) DEFAULT N'Chưa trả' CHECK (TrangThaiThanhToan IN (N'Chưa trả', N'Đã trả')),
  NgayThanhToan DATE,
  NgayTao DATETIME DEFAULT GETDATE(),
  GhiChu NVARCHAR(500),
  
  FOREIGN KEY (MaHopDong) REFERENCES HopDong(MaHopDong) ON DELETE CASCADE,
  -- Ràng buộc mỗi hợp đồng chỉ có 1 hóa đơn trong 1 tháng
  UNIQUE (MaHopDong, ThangNam) 
);
GO

-- 3. DỮ LIỆU MẪU ĐỂ TEST (QUAN TRỌNG)
-- Để test logic "Lấy số điện tháng trước", ta cần tạo giả một hóa đơn của tháng trước.
-- Giả sử tháng hiện tại là tháng 11/2025, ta tạo dữ liệu tháng 10/2025.

-- Đảm bảo có hợp đồng cho P101 (MaPhong = 1, MaHopDong = 1)
-- Tạo hóa đơn Tháng 10/2025 cho P101 với Chỉ số điện Mới là 1000
INSERT INTO ThanhToan (
    MaHopDong, ThangNam, 
    TienThue, TienDien, TienNuoc, TongTien,
    ChiSoDienCu, ChiSoDienMoi, SoDien,
    TrangThaiThanhToan, GhiChu
)
VALUES (
    1, '10/2025', 
    3000000, 350000, 100000, 3450000,
    900, 1000, 100, -- Số mới tháng 10 là 1000. Khi chạy tháng 11, số Cũ sẽ là 1000.
    N'Đã trả', N'Dữ liệu mẫu tháng trước'
);
GO