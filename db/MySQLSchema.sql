SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";

-- 1. KHỞI TẠO DATABASE
CREATE DATABASE IF NOT EXISTS `githubio_QLT_Ver1` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `githubio_QLT_Ver1`;

-- =======================================================
-- 2. CẤU TRÚC BẢNG (SCHEMA)
-- =======================================================

-- Bảng Nhà
CREATE TABLE IF NOT EXISTS `Nha` (
  `MaNha` int(11) NOT NULL AUTO_INCREMENT,
  `DiaChi` varchar(255) NOT NULL,
  `TongSoPhong` int(11) DEFAULT NULL CHECK (`TongSoPhong` > 0),
  `GhiChu` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`MaNha`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Bảng Admin mở rộng
CREATE TABLE IF NOT EXISTS `Admin` (
  `MaAdmin` int(11) NOT NULL AUTO_INCREMENT,
  `TenDangNhap` varchar(50) NOT NULL,
  `MatKhau` varchar(255) NOT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `SoDienThoai` varchar(15) DEFAULT NULL,

  -- Thông tin cá nhân bổ sung
  `HoTen` varchar(100) DEFAULT NULL,
  `NgaySinh` date DEFAULT NULL,
  `CCCD` varchar(20) DEFAULT NULL,
  `NgayCap` date DEFAULT NULL,
  `NoiCap` varchar(100) DEFAULT NULL,
  `DiaChi` varchar(255) DEFAULT NULL,

  -- Quan hệ nhà
  `MaNha` int(11) NOT NULL,

  PRIMARY KEY (`MaAdmin`),
  UNIQUE KEY `TenDangNhap` (`TenDangNhap`),

  -- Mỗi nhà chỉ có 1 admin phụ trách
  UNIQUE KEY `UK_Admin_MaNha` (`MaNha`),

  CONSTRAINT `FK_Admin_Nha` FOREIGN KEY (`MaNha`) 
        REFERENCES `Nha` (`MaNha`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


-- Bảng Phòng
CREATE TABLE IF NOT EXISTS `Phong` (
  `MaPhong` int(11) NOT NULL AUTO_INCREMENT,
  `MaNha` int(11) DEFAULT NULL,
  `TenPhong` varchar(50) NOT NULL,
  `DienTich` decimal(5,2) DEFAULT NULL CHECK (`DienTich` > 0),
  `GiaCoBan` decimal(18,0) DEFAULT 0 CHECK (`GiaCoBan` >= 0),
  `TrangThai` enum('Trống','Đang thuê','Dự kiến','Bảo trì') DEFAULT 'Trống',
  `GhiChu` varchar(255) DEFAULT NULL,
  `NgayCoTheChoThue` date DEFAULT NULL COMMENT 'Ngày dự kiến phòng sẵn sàng',
  `GiaBangChu` varchar(255) DEFAULT NULL,
  `TrangThietBi` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`MaPhong`),
  KEY `MaNha` (`MaNha`),
  CONSTRAINT `FK_Phong_Nha` FOREIGN KEY (`MaNha`) REFERENCES `Nha` (`MaNha`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Bảng Người Thuê (CÓ THÊM CỘT MA_PHONG THEO YÊU CẦU)
CREATE TABLE IF NOT EXISTS `NguoiThue` (
  `MaNguoiThue` int(11) NOT NULL AUTO_INCREMENT,
  `MaPhong` int(11) DEFAULT NULL COMMENT 'Liên kết phòng đang ở (NULL nếu đã trả)',
  `HoTen` varchar(100) NOT NULL,
  `SoDienThoai` varchar(15) NOT NULL,
  `CCCD` varchar(20) DEFAULT NULL UNIQUE,
  `Email` varchar(100) DEFAULT NULL,
  `GioiTinh` enum('Nam','Nữ','Khác') DEFAULT NULL,
  `NgayBatDau` date DEFAULT NULL,
  `TrangThai` enum('Đang ở','Đã trả phòng','Sắp trả phòng') DEFAULT 'Đang ở',
  `GhiChu` varchar(255) DEFAULT NULL,
  `NgaySinh` date DEFAULT NULL,
  `DiaChiThuongTru` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`MaNguoiThue`),
  KEY `MaPhong` (`MaPhong`),
  CONSTRAINT `FK_NguoiThue_Phong` FOREIGN KEY (`MaPhong`) REFERENCES `Phong` (`MaPhong`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Bảng Hợp Đồng
CREATE TABLE IF NOT EXISTS `HopDong` (
  `MaHopDong` int(11) NOT NULL AUTO_INCREMENT,
  `MaNguoiThue` int(11) DEFAULT NULL,
  `MaPhong` int(11) DEFAULT NULL,
  `NgayBatDau` date NOT NULL,
  `NgayKetThuc` date NOT NULL,
  `TienCoc` decimal(18,0) DEFAULT 0 CHECK (`TienCoc` >= 0),
  `FileHopDong` varchar(255) DEFAULT NULL,
  `TrangThai` enum('Hiệu lực','Hết hạn','Hủy','Sắp hết hạn') DEFAULT 'Hiệu lực',
  PRIMARY KEY (`MaHopDong`),
  KEY `MaNguoiThue` (`MaNguoiThue`),
  KEY `MaPhong` (`MaPhong`),
  CONSTRAINT `FK_HopDong_NguoiThue` FOREIGN KEY (`MaNguoiThue`) REFERENCES `NguoiThue` (`MaNguoiThue`) ON DELETE CASCADE,
  CONSTRAINT `FK_HopDong_Phong` FOREIGN KEY (`MaPhong`) REFERENCES `Phong` (`MaPhong`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
-- Bảng Hợp Đồng
CREATE TABLE IF NOT EXISTS `HopDong` (
  `MaHopDong` int(11) NOT NULL AUTO_INCREMENT,
  `MaNguoiThue` int(11) DEFAULT NULL,
  `MaPhong` int(11) DEFAULT NULL,
  `NgayBatDau` date NOT NULL,
  `NgayKetThuc` date NOT NULL,
  `TienCoc` decimal(18,0) DEFAULT 0 CHECK (`TienCoc` >= 0),
  `FileHopDong` varchar(255) DEFAULT NULL,
  `TrangThai` enum('Hiệu lực','Hết hạn','Hủy','Sắp hết hạn') DEFAULT 'Hiệu lực',
  PRIMARY KEY (`MaHopDong`),
  KEY `MaNguoiThue` (`MaNguoiThue`),
  KEY `MaPhong` (`MaPhong`),
  CONSTRAINT `FK_HopDong_NguoiThue` FOREIGN KEY (`MaNguoiThue`) REFERENCES `NguoiThue` (`MaNguoiThue`) ON DELETE CASCADE,
  CONSTRAINT `FK_HopDong_Phong` FOREIGN KEY (`MaPhong`) REFERENCES `Phong` (`MaPhong`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Bảng Tài Sản Người Thuê
CREATE TABLE IF NOT EXISTS `TaiSanNguoiThue` (
  `MaTaiSan` int(11) NOT NULL AUTO_INCREMENT,
  `MaNguoiThue` int(11) DEFAULT NULL,
  `LoaiTaiSan` enum('Xe','Thú cưng','Khác') DEFAULT 'Xe',
  `MoTa` varchar(255) DEFAULT NULL,
  `PhiPhuThu` decimal(18,0) DEFAULT 0,
  PRIMARY KEY (`MaTaiSan`),
  KEY `MaNguoiThue` (`MaNguoiThue`),
  CONSTRAINT `FK_TaiSan_NguoiThue` FOREIGN KEY (`MaNguoiThue`) REFERENCES `NguoiThue` (`MaNguoiThue`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Bảng Thanh Toán (UPDATE LOGIC TÍNH TOÁN & TRẠNG THÁI)
CREATE TABLE IF NOT EXISTS `ThanhToan` (
  `MaThanhToan` int(11) NOT NULL AUTO_INCREMENT,
  `MaHopDong` int(11) DEFAULT NULL,
  `ThangNam` char(7) NOT NULL,
  
  -- Các khoản thu
  `TienThue` decimal(18,0) DEFAULT 0,
  `TienDien` decimal(18,0) DEFAULT 0,
  `TienNuoc` decimal(18,0) DEFAULT 0,
  `TienInternet` decimal(18,0) DEFAULT 0,
  `TienVeSinh` decimal(18,0) DEFAULT 0,
  `TienGiuXe` decimal(18,0) DEFAULT 0,
  `ChiPhiKhac` decimal(18,0) DEFAULT 0,
  
  -- Chỉ số điện nước
  `DonGiaDien` decimal(18,0) DEFAULT 3500,
  `DonGiaNuoc` decimal(18,0) DEFAULT 100000,
  `ChiSoDienCu` decimal(18,2) DEFAULT 0,
  `ChiSoDienMoi` decimal(18,2) DEFAULT 0,
  `SoDien` decimal(18,2) DEFAULT 0,
  `SoNuoc` decimal(18,2) DEFAULT 0,
  
  -- [AUTO] Tự động tính Tổng tiền
  `TongTien` decimal(18,0) GENERATED ALWAYS AS (
     COALESCE(`TienThue`,0) + COALESCE(`TienDien`,0) + COALESCE(`TienNuoc`,0) + 
     COALESCE(`TienInternet`,0) + COALESCE(`TienVeSinh`,0) + COALESCE(`TienGiuXe`,0) + COALESCE(`ChiPhiKhac`,0)
  ) STORED,

  -- [UPDATE] Quản lý công nợ
  `SoTienDaTra` decimal(18,0) DEFAULT 0 COMMENT 'Số tiền khách thực đóng',
  `ConLai` decimal(18,0) GENERATED ALWAYS AS (`TongTien` - `SoTienDaTra`) STORED COMMENT 'Tự động tính nợ',

  -- [UPDATE] Enum trạng thái mới
  `TrangThaiThanhToan` enum('Chưa trả','Trả một phần','Đã trả') DEFAULT 'Chưa trả',
  `NgayThanhToan` date DEFAULT NULL,
  `GhiChu` varchar(500) DEFAULT NULL,
  
  PRIMARY KEY (`MaThanhToan`),
  UNIQUE KEY `UK_HopDong_Thang` (`MaHopDong`, `ThangNam`),
  KEY `MaHopDong` (`MaHopDong`),
  CONSTRAINT `FK_ThanhToan_HopDong` FOREIGN KEY (`MaHopDong`) REFERENCES `HopDong` (`MaHopDong`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Bảng Bảo Trì
CREATE TABLE IF NOT EXISTS `BaoTri_SuCo` (
  `MaSuCo` int(11) NOT NULL AUTO_INCREMENT,
  `MaPhong` int(11) DEFAULT NULL,
  `MoTaSuCo` varchar(255) NOT NULL,
  `NgayBaoCao` date DEFAULT (curdate()),
  `TrangThai` enum('Chưa xử lý','Đang xử lý','Hoàn tất') DEFAULT 'Chưa xử lý',
  `ChiPhi` decimal(18,0) DEFAULT 0,
  PRIMARY KEY (`MaSuCo`),
  KEY `MaPhong` (`MaPhong`),
  CONSTRAINT `FK_BaoTri_Phong` FOREIGN KEY (`MaPhong`) REFERENCES `Phong` (`MaPhong`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Bảng Google Form Log (Hỗ trợ AI đọc số điện)
CREATE TABLE IF NOT EXISTS `GoogleFormLog` (
  `MaLog` int(11) NOT NULL AUTO_INCREMENT,
  `RoomName` varchar(50) NOT NULL,
  `ElectricImageUrl` varchar(500) DEFAULT NULL,
  `WaterImageUrl` varchar(500) DEFAULT NULL,
  `SubmittedValue` decimal(18,2) DEFAULT NULL,
  `ExtractedValue` decimal(18,2) DEFAULT NULL,
  `IsValid` bit(1) DEFAULT b'0',
  `ErrorMessage` varchar(500) DEFAULT NULL,
  `Timestamp` datetime DEFAULT current_timestamp(),
  `Processed` bit(1) DEFAULT b'0',
  PRIMARY KEY (`MaLog`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


-- =======================================================
-- 3. DỮ LIỆU MẪU (DATA) - CHUẨN HÓA 5 PHÒNG
-- =======================================================

-- 3.1. Nhà & Admin
INSERT INTO `Nha` (`DiaChi`, `TongSoPhong`, `GhiChu`) VALUES
('15/2 Đường CMT8, Quận 10, TP.HCM', 5, 'Nhà trọ Happy House - Cổng vân tay');

INSERT INTO `Admin` (`TenDangNhap`, `MatKhau`, `Email`, `SoDienThoai`, `MaNha`) VALUES
('admin', '123456', 'admin@happyhouse.vn', '0909999888', 1);

-- 3.2. Phòng (P.101 -> P.301)
INSERT INTO `Phong` (`MaNha`, `TenPhong`, `DienTich`, `GiaCoBan`, `TrangThai`, `GhiChu`, `TrangThietBi`, `NgayCoTheChoThue`) VALUES
(1, 'P.101', 25.0, 4000000, 'Đang thuê', 'Phòng tầng trệt, cửa sổ lớn', 'Máy lạnh, Tủ lạnh, Giường', NULL),
(1, 'P.102', 22.0, 3500000, 'Trống', 'Khách cũ mới trả, cần dọn vệ sinh', 'Máy lạnh, Nóng lạnh', '2025-11-20'),
(1, 'P.201', 28.0, 4500000, 'Đang thuê', 'Phòng VIP, ban công', 'Full nội thất', NULL),
(1, 'P.202', 28.0, 4500000, 'Bảo trì', 'Đang chờ thợ sửa máy lạnh', 'Full nội thất', '2025-11-25'),
(1, 'P.301', 35.0, 5500000, 'Đang thuê', 'Phòng đôi, ở ghép', '2 Giường, Tủ lớn, Bếp', NULL);

-- 3.3. Người Thuê (Gán đúng vào phòng)
INSERT INTO `NguoiThue` (`MaPhong`, `HoTen`, `SoDienThoai`, `CCCD`, `Email`, `GioiTinh`, `NgayBatDau`, `TrangThai`, `DiaChiThuongTru`) VALUES
(1, 'Nguyễn Văn An', '0911222333', '079199000001', 'an.nguyen@mail.com', 'Nam', '2025-01-10', 'Đang ở', 'Hà Nội'),
(3, 'Trần Thị Bình', '0922333444', '079199000002', 'binh.tran@mail.com', 'Nữ', '2025-03-15', 'Đang ở', 'Đà Nẵng'),
(NULL, 'Lê Văn Cường', '0933444555', '079199000003', 'cuong.le@mail.com', 'Nam', '2024-12-01', 'Đã trả phòng', 'TP.HCM'),
(5, 'Phạm Thị Dung', '0944555666', '079199000004', 'dung.pham@mail.com', 'Nữ', '2025-06-01', 'Đang ở', 'Cần Thơ'),
(5, 'Hoàng Văn Em', '0955666777', '079199000005', 'em.hoang@mail.com', 'Nam', '2025-06-01', 'Đang ở', 'Vĩnh Long');

-- 3.4. Hợp Đồng
INSERT INTO `HopDong` (`MaNguoiThue`, `MaPhong`, `NgayBatDau`, `NgayKetThuc`, `TienCoc`, `TrangThai`) VALUES
(1, 1, '2025-01-10', '2026-01-10', 4000000, 'Hiệu lực'),
(3, 2, '2024-12-01', '2025-11-01', 3500000, 'Hết hạn'),
(2, 3, '2025-03-15', '2026-03-15', 4500000, 'Hiệu lực'),
(4, 5, '2025-06-01', '2026-06-01', 5500000, 'Hiệu lực');

-- 3.5. Tài Sản
INSERT INTO `TaiSanNguoiThue` (`MaNguoiThue`, `LoaiTaiSan`, `MoTa`, `PhiPhuThu`) VALUES
(1, 'Xe', 'Honda AirBlade - 59C1-123.45', 100000),
(2, 'Xe', 'Vision - 59E1-678.90', 100000),
(2, 'Thú cưng', 'Mèo Anh lông ngắn', 50000),
(4, 'Xe', 'SH Mode - 59T2-999.99', 150000),
(5, 'Xe', 'Wave Alpha - 59X3-111.11', 100000);

-- 3.6. Bảo trì
INSERT INTO `BaoTri_SuCo` (`MaPhong`, `MoTaSuCo`, `NgayBaoCao`, `TrangThai`, `ChiPhi`) VALUES
(4, 'Máy lạnh chảy nước, không lạnh', '2025-11-15', 'Đang xử lý', 0),
(1, 'Thay bóng đèn toilet', '2025-10-10', 'Hoàn tất', 50000);

-- 3.7. Thanh Toán (DEMO TÍNH TOÁN)
-- Kịch bản 1: Đã trả đủ
INSERT INTO `ThanhToan` (`MaHopDong`, `ThangNam`, `TienThue`, `TienDien`, `TienNuoc`, `TienInternet`, `TienVeSinh`, `TienGiuXe`, `SoTienDaTra`, `TrangThaiThanhToan`, `NgayThanhToan`) 
VALUES (1, '11/2025', 4000000, 350000, 100000, 100000, 50000, 100000, 4700000, 'Đã trả', '2025-11-05');

-- Kịch bản 2: Trả thiếu (Hệ thống tự tính nợ 2tr)
INSERT INTO `ThanhToan` (`MaHopDong`, `ThangNam`, `TienThue`, `TienDien`, `TienNuoc`, `ChiPhiKhac`, `SoTienDaTra`, `TrangThaiThanhToan`, `GhiChu`) 
VALUES (3, '11/2025', 4500000, 500000, 150000, 50000, 3500000, 'Trả một phần', 'Khất lại 2tr');

-- Kịch bản 3: Chưa đóng
INSERT INTO `ThanhToan` (`MaHopDong`, `ThangNam`, `TienThue`, `TienDien`, `TienNuoc`, `SoTienDaTra`, `TrangThaiThanhToan`) 
VALUES (4, '11/2025', 5500000, 800000, 200000, 0, 'Chưa trả');