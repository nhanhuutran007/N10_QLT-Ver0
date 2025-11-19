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

-- Bảng Admin
CREATE TABLE IF NOT EXISTS `Admin` (
  `MaAdmin` int(11) NOT NULL AUTO_INCREMENT,
  `TenDangNhap` varchar(50) NOT NULL,
  `MatKhau` varchar(255) NOT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `SoDienThoai` varchar(15) DEFAULT NULL,
  `MaNha` int(11) NOT NULL,
  PRIMARY KEY (`MaAdmin`),
  UNIQUE KEY `TenDangNhap` (`TenDangNhap`),
  UNIQUE KEY `UK_Admin_MaNha` (`MaNha`),
  CONSTRAINT `FK_Admin_Nha` FOREIGN KEY (`MaNha`) REFERENCES `Nha` (`MaNha`) ON DELETE RESTRICT
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

