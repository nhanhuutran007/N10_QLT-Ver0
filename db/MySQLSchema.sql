SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;


CREATE TABLE `Admin` (
  `MaAdmin` int(11) NOT NULL,
  `TenDangNhap` varchar(50) NOT NULL,
  `MatKhau` varchar(255) NOT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `SoDienThoai` varchar(15) DEFAULT NULL,
  `HoTen` varchar(100) DEFAULT NULL,
  `NgaySinh` date DEFAULT NULL,
  `CCCD` varchar(20) DEFAULT NULL,
  `NgayCap` date DEFAULT NULL,
  `NoiCap` varchar(100) DEFAULT NULL,
  `DiaChi` varchar(255) DEFAULT NULL,
  `MaNha` int(11) NOT NULL,
  `TenTK` varchar(255) DEFAULT NULL,
  `SoTK` varchar(50) DEFAULT NULL,
  `LinkQr` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `Admin` (`MaAdmin`, `TenDangNhap`, `MatKhau`, `Email`, `SoDienThoai`, `HoTen`, `NgaySinh`, `CCCD`, `NgayCap`, `NoiCap`, `DiaChi`, `MaNha`, `TenTK`, `SoTK`, `LinkQr`) VALUES
(1, 'admin', 'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', 'nhanhuutran009@gmail.com', '0909999888', 'Phạm Đức Mạnh', '1980-06-19', '918391391', '2025-10-30', 'Thanh Hóa', 'Bồ Đào Nha, Châu Âu', 1, 'TRAN HUU NHAN', '08699182255', '/Resources/Images/QR_20251126121439.jpg'),
(2, 'trong', 'AlrF/WArcM+aP2AQPy9ctTAdJq+NKSVHwbnLaCa6hZk=', 'phuocvinh@gmail.com', '', NULL, NULL, NULL, NULL, NULL, NULL, 301, NULL, NULL, NULL);

CREATE TABLE `BaoTri_SuCo` (
  `MaSuCo` int(11) NOT NULL,
  `MaPhong` int(11) DEFAULT NULL,
  `MoTaSuCo` varchar(255) NOT NULL,
  `NgayBaoCao` date DEFAULT curdate(),
  `NgayCoTheSua` date DEFAULT NULL,
  `TrangThai` enum('Chưa xử lý','Đang xử lý','Hoàn tất') DEFAULT 'Chưa xử lý',
  `ChiPhi` decimal(18,0) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `BaoTri_SuCo` (`MaSuCo`, `MaPhong`, `MoTaSuCo`, `NgayBaoCao`, `NgayCoTheSua`, `TrangThai`, `ChiPhi`) VALUES
(29, 18, 'Bị đẹp trai', '2025-12-04', '2025-12-04', 'Chưa xử lý', 0);

CREATE TABLE `DeletedMaintenanceSignatures` (
  `Id` int(11) NOT NULL,
  `MaPhong` int(11) NOT NULL,
  `MoTaSuCo` varchar(255) NOT NULL,
  `NgayBaoCao` date NOT NULL,
  `NgayCoTheSua` date NOT NULL,
  `NgayXoa` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `GoogleFormLog` (
  `MaLog` int(11) NOT NULL,
  `RoomName` varchar(50) NOT NULL,
  `ElectricImageUrl` varchar(500) DEFAULT NULL,
  `SubmittedValue` decimal(18,2) DEFAULT NULL,
  `ExtractedValue` decimal(18,2) DEFAULT NULL,
  `IsValid` bit(1) DEFAULT b'0',
  `ErrorMessage` varchar(500) DEFAULT NULL,
  `Timestamp` datetime DEFAULT current_timestamp(),
  `Processed` bit(1) DEFAULT b'0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `HopDong` (
  `MaHopDong` int(11) NOT NULL,
  `MaNguoiThue` int(11) DEFAULT NULL,
  `MaPhong` int(11) DEFAULT NULL,
  `NgayBatDau` date NOT NULL,
  `NgayKetThuc` date NOT NULL,
  `TienCoc` decimal(18,0) DEFAULT 0 CHECK (`TienCoc` >= 0),
  `FileHopDong` varchar(255) DEFAULT NULL,
  `TrangThai` enum('Hiệu lực','Hết hạn','Hủy','Sắp hết hạn') DEFAULT 'Hiệu lực',
  `GhiChu` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `HopDong` (`MaHopDong`, `MaNguoiThue`, `MaPhong`, `NgayBatDau`, `NgayKetThuc`, `TienCoc`, `FileHopDong`, `TrangThai`, `GhiChu`) VALUES
(27, 34, 18, '2025-12-04', '2026-12-04', 3000000, 'C:\\Users\\STRONGERMANY\\Documents\\HopDongPhongTro\\Beckham_COCONUT_20251204_154944.pdf', 'Hiệu lực', NULL),
(28, 33, 19, '2025-12-04', '2026-12-04', 1500000, 'C:\\Users\\STRONGERMANY\\Documents\\HopDongPhongTro\\Ronaldo_PINEAPPLE_20251204_155709.pdf', 'Hiệu lực', 'Hợp đồng mới được tạo sau khi xóa người thuê cũ (HD#26)');

CREATE TABLE `NguoiThue` (
  `MaNguoiThue` int(11) NOT NULL,
  `MaPhong` int(11) DEFAULT NULL COMMENT 'Liên kết phòng đang ở (NULL nếu đã trả)',
  `HoTen` varchar(100) NOT NULL,
  `SoDienThoai` varchar(15) NOT NULL,
  `CCCD` varchar(20) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `GioiTinh` enum('Nam','Nữ','Khác') DEFAULT NULL,
  `NgayBatDau` date DEFAULT NULL,
  `TrangThai` varchar(50) DEFAULT 'Đang ở',
  `GhiChu` varchar(255) DEFAULT NULL,
  `NgaySinh` date DEFAULT NULL,
  `DiaChi` varchar(255) DEFAULT NULL,
  `NgheNghiep` varchar(100) DEFAULT NULL,
  `NgayCap` date DEFAULT NULL,
  `NoiCap` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `NguoiThue` (`MaNguoiThue`, `MaPhong`, `HoTen`, `SoDienThoai`, `CCCD`, `Email`, `GioiTinh`, `NgayBatDau`, `TrangThai`, `GhiChu`, `NgaySinh`, `DiaChi`, `NgheNghiep`, `NgayCap`, `NoiCap`) VALUES
(33, 19, 'Ronaldo', '', '9283923823', '', 'Nam', '2025-12-04', 'Đang ở', '', '2025-12-04', '', '', '2025-12-04', ''),
(34, 18, 'Beckham', '0998188238', '293283293192', 'smsfn@gmail.com', 'Nam', '2025-12-04', 'Đang ở', '', '2005-12-04', 'Việt Nam', 'Bác Sĩ', '2025-12-04', 'Hoàng Sa');

CREATE TABLE `Nha` (
  `MaNha` int(11) NOT NULL,
  `DiaChi` varchar(255) NOT NULL,
  `TongSoPhong` int(11) DEFAULT NULL CHECK (`TongSoPhong` > 0),
  `GhiChu` varchar(255) DEFAULT NULL,
  `TinhThanh` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `Nha` (`MaNha`, `DiaChi`, `TongSoPhong`, `GhiChu`, `TinhThanh`) VALUES
(1, '15/2 Đường CMT8, Quận 10, TP.HCM', 5, 'Nhà trọ Happy House - Cổng vân tay', 'TPHCM'),
(301, 'Chưa cập nhật', 1, NULL, '');

CREATE TABLE `Phong` (
  `MaPhong` int(11) NOT NULL,
  `MaNha` int(11) DEFAULT NULL,
  `TenPhong` varchar(50) NOT NULL,
  `DienTich` decimal(5,2) DEFAULT NULL CHECK (`DienTich` > 0),
  `GiaCoBan` decimal(18,0) DEFAULT 0 CHECK (`GiaCoBan` >= 0),
  `TrangThai` enum('Đang thuê','Trống','Đang bảo trì','Dự kiến') DEFAULT 'Trống',
  `GhiChu` varchar(255) DEFAULT NULL,
  `NgayCoTheChoThue` date DEFAULT NULL COMMENT 'Ngày dự kiến phòng sẵn sàng',
  `GiaBangChu` varchar(255) DEFAULT NULL,
  `TrangThietBi` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `Phong` (`MaPhong`, `MaNha`, `TenPhong`, `DienTich`, `GiaCoBan`, `TrangThai`, `GhiChu`, `NgayCoTheChoThue`, `GiaBangChu`, `TrangThietBi`) VALUES
(18, 1, 'COCONUT', 50.00, 3000000, 'Đang thuê', 'Có 1 xe máy', NULL, 'Ba triệu đồng', ''),
(19, 1, 'PINEAPPLE', 15.00, 1500000, 'Đang thuê', '', NULL, 'Một triệu năm trăm nghìn đồng', 'Máy lạnh, máy giặt'),
(20, 1, 'BANANA', 45.00, 3000000, 'Trống', '', NULL, 'Ba trieu dong ', 'TV, tu lanh ');

CREATE TABLE `TaiSanNguoiThue` (
  `MaTaiSan` int(11) NOT NULL,
  `MaNguoiThue` int(11) DEFAULT NULL,
  `LoaiTaiSan` enum('Xe','Thú cưng','Khác') DEFAULT 'Xe',
  `MoTa` varchar(255) DEFAULT NULL,
  `PhiPhuThu` decimal(18,0) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `ThanhToan` (
  `MaThanhToan` int(11) NOT NULL,
  `MaHopDong` int(11) DEFAULT NULL,
  `ThangNam` varchar(20) DEFAULT NULL,
  `TienThue` decimal(18,0) DEFAULT 0,
  `TienDien` decimal(18,0) DEFAULT 0,
  `TienNuoc` decimal(18,0) DEFAULT 0,
  `TienInternet` decimal(18,0) DEFAULT 0,
  `TienVeSinh` decimal(18,0) DEFAULT 0,
  `TienGiuXe` decimal(18,0) DEFAULT 0,
  `ChiPhiKhac` decimal(18,0) DEFAULT 0,
  `DonGiaDien` decimal(18,0) DEFAULT 0,
  `DonGiaNuoc` decimal(18,0) DEFAULT 0,
  `ChiSoDienCu` double DEFAULT 0,
  `ChiSoDienMoi` double DEFAULT 0,
  `SoDien` double DEFAULT 0,
  `SoNuoc` double DEFAULT 0,
  `TongTien` decimal(18,0) DEFAULT 0,
  `SoTienDaTra` decimal(18,0) DEFAULT 0,
  `ConLai` decimal(18,0) DEFAULT 0,
  `TrangThaiThanhToan` varchar(50) DEFAULT 'Chưa đóng',
  `NgayThanhToan` datetime DEFAULT NULL,
  `GhiChu` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


ALTER TABLE `Admin`
  ADD PRIMARY KEY (`MaAdmin`),
  ADD UNIQUE KEY `TenDangNhap` (`TenDangNhap`),
  ADD UNIQUE KEY `UK_Admin_MaNha` (`MaNha`);

ALTER TABLE `BaoTri_SuCo`
  ADD PRIMARY KEY (`MaSuCo`),
  ADD KEY `MaPhong` (`MaPhong`);

ALTER TABLE `DeletedMaintenanceSignatures`
  ADD PRIMARY KEY (`Id`);

ALTER TABLE `GoogleFormLog`
  ADD PRIMARY KEY (`MaLog`);

ALTER TABLE `HopDong`
  ADD PRIMARY KEY (`MaHopDong`),
  ADD KEY `MaNguoiThue` (`MaNguoiThue`),
  ADD KEY `MaPhong` (`MaPhong`);

ALTER TABLE `NguoiThue`
  ADD PRIMARY KEY (`MaNguoiThue`),
  ADD UNIQUE KEY `CCCD` (`CCCD`),
  ADD KEY `MaPhong` (`MaPhong`);

ALTER TABLE `Nha`
  ADD PRIMARY KEY (`MaNha`);

ALTER TABLE `Phong`
  ADD PRIMARY KEY (`MaPhong`),
  ADD KEY `MaNha` (`MaNha`);

ALTER TABLE `TaiSanNguoiThue`
  ADD PRIMARY KEY (`MaTaiSan`),
  ADD KEY `MaNguoiThue` (`MaNguoiThue`);

ALTER TABLE `ThanhToan`
  ADD PRIMARY KEY (`MaThanhToan`),
  ADD KEY `FK_ThanhToan_HopDong` (`MaHopDong`);


ALTER TABLE `Admin`
  MODIFY `MaAdmin` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

ALTER TABLE `BaoTri_SuCo`
  MODIFY `MaSuCo` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

ALTER TABLE `DeletedMaintenanceSignatures`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `GoogleFormLog`
  MODIFY `MaLog` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `HopDong`
  MODIFY `MaHopDong` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;

ALTER TABLE `NguoiThue`
  MODIFY `MaNguoiThue` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=35;

ALTER TABLE `Nha`
  MODIFY `MaNha` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=302;

ALTER TABLE `Phong`
  MODIFY `MaPhong` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

ALTER TABLE `TaiSanNguoiThue`
  MODIFY `MaTaiSan` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

ALTER TABLE `ThanhToan`
  MODIFY `MaThanhToan` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;


ALTER TABLE `Admin`
  ADD CONSTRAINT `FK_Admin_Nha` FOREIGN KEY (`MaNha`) REFERENCES `Nha` (`MaNha`);

ALTER TABLE `BaoTri_SuCo`
  ADD CONSTRAINT `FK_BaoTri_Phong` FOREIGN KEY (`MaPhong`) REFERENCES `Phong` (`MaPhong`) ON DELETE CASCADE;

ALTER TABLE `HopDong`
  ADD CONSTRAINT `FK_HopDong_NguoiThue` FOREIGN KEY (`MaNguoiThue`) REFERENCES `NguoiThue` (`MaNguoiThue`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_HopDong_Phong` FOREIGN KEY (`MaPhong`) REFERENCES `Phong` (`MaPhong`) ON DELETE CASCADE;

ALTER TABLE `NguoiThue`
  ADD CONSTRAINT `FK_NguoiThue_Phong` FOREIGN KEY (`MaPhong`) REFERENCES `Phong` (`MaPhong`) ON DELETE SET NULL;

ALTER TABLE `Phong`
  ADD CONSTRAINT `FK_Phong_Nha` FOREIGN KEY (`MaNha`) REFERENCES `Nha` (`MaNha`) ON DELETE CASCADE;

ALTER TABLE `TaiSanNguoiThue`
  ADD CONSTRAINT `FK_TaiSan_NguoiThue` FOREIGN KEY (`MaNguoiThue`) REFERENCES `NguoiThue` (`MaNguoiThue`) ON DELETE CASCADE;

ALTER TABLE `ThanhToan`
  ADD CONSTRAINT `FK_ThanhToan_HopDong` FOREIGN KEY (`MaHopDong`) REFERENCES `HopDong` (`MaHopDong`) ON DELETE CASCADE ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
