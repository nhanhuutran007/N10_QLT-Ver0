SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";

-- Ensure target database exists and select it
CREATE DATABASE IF NOT EXISTS `githubio_QLT_Ver1` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `githubio_QLT_Ver1`;


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
  `MaNha` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `Admin` (`MaAdmin`, `TenDangNhap`, `MatKhau`, `Email`, `SoDienThoai`, `HoTen`, `NgaySinh`, `CCCD`, `NgayCap`, `NoiCap`, `DiaChi`, `MaNha`) VALUES
(1, 'admin', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=', 'ngochai1521@gmai.com', '0901000001', 'Nguyễn Văn A', '1985-05-20', '012345678901', '2020-01-15', 'CA TP.HCM', '123 Đường ABC, Quận 1, TP.HCM', 1),
(2, 'test', 'admin123', 'test@example.com', '0901000001', 'Test Owner', NULL, NULL, NULL, NULL, '456 Đường DEF, Quận 3, TP.HCM', 2);

CREATE TABLE `BaoTri_SuCo` (
  `MaSuCo` int(11) NOT NULL,
  `MaPhong` int(11) DEFAULT NULL,
  `MoTaSuCo` varchar(255) NOT NULL,
  `NgayBaoCao` date DEFAULT curdate(),
  `TrangThai` enum('Chưa xử lý','Đang xử lý','Hoàn tất') DEFAULT 'Chưa xử lý',
  `ChiPhi` decimal(18,0) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `BaoTri_SuCo` (`MaSuCo`, `MaPhong`, `MoTaSuCo`, `NgayBaoCao`, `TrangThai`, `ChiPhi`) VALUES
(4, 4, 'Cửa bị kẹt', '2025-11-06', 'Chưa xử lý', 0),
(5, 5, 'Nước yếu', '2025-11-06', 'Đang xử lý', 0);

CREATE TABLE `DeletedMaintenanceSignatures` (
  `Id` int(11) NOT NULL,
  `MaPhong` int(11) NOT NULL,
  `MoTaSuCo` varchar(255) NOT NULL,
  `NgayBaoCao` date NOT NULL,
  `NgayXoa` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

INSERT INTO `DeletedMaintenanceSignatures` (`Id`, `MaPhong`, `MoTaSuCo`, `NgayBaoCao`, `NgayXoa`) VALUES
(1, 1, 'Hu t? l?nh', '2025-11-05', '2025-11-06 14:12:44'),
(2, 2, 'Hu b?p di?n', '2025-11-05', '2025-11-06 14:12:46'),
(3, 1, 'Hu c?u chì', '2025-11-10', '2025-11-06 14:13:13'),
(4, 1, 'Hu vòi nu?c', '2025-11-11', '2025-11-06 14:13:51'),
(5, 1, 'Hu c?u chì', '2025-11-11', '2025-11-06 14:13:55'),
(6, 2, 'Rò r? di?n', '2025-11-06', '2025-11-06 23:24:32'),
(7, 2, 'H? vòi sen', '2025-11-13', '2025-11-13 17:42:09');

CREATE TABLE `HopDong` (
  `MaHopDong` int(11) NOT NULL,
  `MaNguoiThue` int(11) DEFAULT NULL,
  `MaPhong` int(11) DEFAULT NULL,
  `NgayBatDau` date NOT NULL,
  `NgayKetThuc` date NOT NULL,
  `TienCoc` decimal(18,0) DEFAULT 0 CHECK (`TienCoc` >= 0),
  `FileHopDong` varchar(255) DEFAULT NULL,
  `TenPhong` varchar(100) DEFAULT NULL,
  `DiaChiPhong` varchar(255) DEFAULT NULL,
  `DienTich` float DEFAULT NULL,
  `TrangThietBi` varchar(255) DEFAULT NULL,
  `GiaThue` decimal(18,2) DEFAULT NULL,
  `TenChuPhong` varchar(100) DEFAULT NULL,
  `NgaySinhChu` date DEFAULT NULL,
  `CCCDChu` varchar(50) DEFAULT NULL,
  `NgayCapChu` date DEFAULT NULL,
  `NoiCapChu` varchar(100) DEFAULT NULL,
  `DiaChiChu` varchar(255) DEFAULT NULL,
  `DienThoaiChu` varchar(20) DEFAULT NULL,
  `TenNguoiThue` varchar(100) DEFAULT NULL,
  `NgaySinhNguoiThue` date DEFAULT NULL,
  `CCCDNguoiThue` varchar(50) DEFAULT NULL,
  `NgayCapNguoiThue` date DEFAULT NULL,
  `NoiCapNguoiThue` varchar(100) DEFAULT NULL,
  `DiaChiNguoiThue` varchar(255) DEFAULT NULL,
  `DienThoaiNguoiThue` varchar(20) DEFAULT NULL,
  `NoiTaoHopDong` varchar(255) DEFAULT NULL,
  `NgayTaoHopDong` datetime DEFAULT current_timestamp(),
  `GiaBangChu` varchar(255) DEFAULT NULL,
  `NgayTraTien` varchar(50) DEFAULT NULL,
  `ThoiHanNam` int(11) DEFAULT NULL,
  `NgayGiaoNha` date DEFAULT NULL,
  `TrangThai` enum('Hiệu lực','Hết hạn','Hủy') DEFAULT NULL
) ;

INSERT INTO `HopDong` (`MaHopDong`, `MaNguoiThue`, `MaPhong`, `NgayBatDau`, `NgayKetThuc`, `TienCoc`, `FileHopDong`, `TenPhong`, `DiaChiPhong`, `DienTich`, `TrangThietBi`, `GiaThue`, `TenChuPhong`, `NgaySinhChu`, `CCCDChu`, `NgayCapChu`, `NoiCapChu`, `DiaChiChu`, `DienThoaiChu`, `TenNguoiThue`, `NgaySinhNguoiThue`, `CCCDNguoiThue`, `NgayCapNguoiThue`, `NoiCapNguoiThue`, `DiaChiNguoiThue`, `DienThoaiNguoiThue`, `NoiTaoHopDong`, `NgayTaoHopDong`, `GiaBangChu`, `NgayTraTien`, `ThoiHanNam`, `NgayGiaoNha`, `TrangThai`) VALUES
(23, 11, 8, '2025-11-15', '2026-11-15', 1000000, 'C:\\Users\\THINKPAD\\Documents\\CNPM\\Branch Hai\\N10_QLT-Ver0\\QLKDPhongTro.Presentation\\bin\\Debug\\net8.0-windows\\Contracts\\Phạm Đức Mạnh_20251115_163741.docx', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2025-11-15 16:37:42', NULL, NULL, NULL, NULL, 'Hiệu lực'),
(24, 11, 6, '2025-11-15', '2025-11-30', 900000, 'C:\\Users\\THINKPAD\\Documents\\CNPM\\Branch Hai\\N10_QLT-Ver0\\QLKDPhongTro.Presentation\\bin\\Debug\\net8.0-windows\\Contracts\\Phạm Đức Mạnh_20251115_165844.docx', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2025-11-15 16:58:44', NULL, NULL, NULL, NULL, 'Hiệu lực'),
(25, 11, 9, '2025-11-15', '2025-12-15', 890098, 'C:\\Users\\THINKPAD\\Documents\\CNPM\\Branch Hai\\N10_QLT-Ver0\\QLKDPhongTro.Presentation\\bin\\Debug\\net8.0-windows\\Contracts\\Phạm Đức Mạnh_20251115_171310.docx', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2025-11-15 17:13:10', NULL, NULL, NULL, NULL, 'Hiệu lực');

CREATE TABLE `NguoiThue` (
  `MaNguoiThue` int(11) NOT NULL,
  `HoTen` varchar(100) NOT NULL,
  `SoDienThoai` varchar(15) NOT NULL,
  `CCCD` varchar(20) DEFAULT NULL,
  `NgayBatDau` date DEFAULT NULL,
  `TrangThai` enum('Đang ở','Đã trả phòng') DEFAULT NULL,
  `GhiChu` varchar(255) DEFAULT NULL,
  `NgaySinh` date DEFAULT NULL,
  `NgayCapCCCD` date DEFAULT NULL,
  `NoiCapCCCD` varchar(255) DEFAULT NULL,
  `DiaChiThuongTru` varchar(255) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `NguoiThue` (`MaNguoiThue`, `HoTen`, `SoDienThoai`, `CCCD`, `NgayBatDau`, `TrangThai`, `GhiChu`, `NgaySinh`, `NgayCapCCCD`, `NoiCapCCCD`, `DiaChiThuongTru`, `Email`) VALUES
(11, 'Phạm Đức Mạnh', '0909090909', '90909090909', '2025-11-15', 'Đang ở', 'đẹp chai \r\n', '2001-07-10', '2025-11-15', 'Thanh Hóa', 'Thanh hóa', 'ngochai.megas@gmail.com\r\n'),
(12, 'Lê Ngọc Hải', '0985123456', '523001012', '2025-11-15', 'Đang ở', '', '1994-08-13', '2025-11-15', 'TPHCM', 'Bắc Kạn, Việt Nam', 'ngochai.megas@gmail.com');

CREATE TABLE `Nha` (
  `MaNha` int(11) NOT NULL,
  `DiaChi` varchar(255) NOT NULL,
  `TongSoPhong` int(11) DEFAULT NULL CHECK (`TongSoPhong` between 1 and 10),
  `GhiChu` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `Nha` (`MaNha`, `DiaChi`, `TongSoPhong`, `GhiChu`) VALUES
(1, '123 Đường A, Quận 1, TP.HCM', 5, 'Nhà trung tâm'),
(2, '456 Đường B, Quận 7, TP.HCM', 6, 'Gần khu công nghệ'),
(3, '789 Đường C, Bình Thạnh', 4, 'Khu yên tĩnh'),
(4, '12 Nguyễn Văn Linh, Quận 7', 8, 'Gần siêu thị'),
(5, '99 Lý Thường Kiệt, Quận 10', 10, 'Gần trường học');

CREATE TABLE `Phong` (
  `MaPhong` int(11) NOT NULL,
  `MaNha` int(11) DEFAULT NULL,
  `TenPhong` varchar(50) NOT NULL,
  `DienTich` decimal(5,2) DEFAULT NULL CHECK (`DienTich` > 0),
  `GiaCoBan` decimal(18,0) DEFAULT 0 CHECK (`GiaCoBan` >= 0),
  `TrangThai` enum('Đang thuê','Trống') DEFAULT 'Trống',
  `GhiChu` varchar(255) DEFAULT NULL,
  `GiaBangChu` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `TrangThietBi` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `Phong` (`MaPhong`, `MaNha`, `TenPhong`, `DienTich`, `GiaCoBan`, `TrangThai`, `GhiChu`, `GiaBangChu`, `TrangThietBi`) VALUES
(4, 3, 'P301', 25.00, 4000000, 'Trống', 'Có ban công lớn', '', ''),
(5, 4, 'P401', 28.00, 4500000, 'Đang thuê', 'Phòng mới xây', '', ''),
(6, NULL, 'Marid', 20.00, 2000000, 'Đang thuê', '', 'Hai tri?u d?ng', 'Máy l?nh, t? l?nh'),
(8, NULL, 'Thanh Hóa', 36.00, 36000000, 'Đang thuê', '', 'Ba mươi sáu triệu đồng', 'Nem chua, rau má'),
(9, NULL, 'MC', 100.00, 10000000, 'Đang thuê', 'Ban công', 'Mười triệu đồng', 'Máy lạnh, tủ lạnh, rau me'),
(12, NULL, 'COCONUT', 50.00, 300000, 'Trống', 'có một xe máy biển số 99493 của khách 1 ', 'BA TRIỆU ', '');

CREATE TABLE `TaiSanNguoiThue` (
  `MaTaiSan` int(11) NOT NULL,
  `MaNguoiThue` int(11) DEFAULT NULL,
  `LoaiTaiSan` enum('Xe','Thú cưng') DEFAULT NULL,
  `MoTa` varchar(255) DEFAULT NULL,
  `PhiPhuThu` decimal(18,0) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `ThanhToan` (
  `MaThanhToan` int(11) NOT NULL,
  `MaHopDong` int(11) DEFAULT NULL,
  `ThangNam` char(7) NOT NULL,
  `TienThue` decimal(18,0) DEFAULT 0,
  `TienDien` decimal(18,0) DEFAULT 0,
  `TienNuoc` decimal(18,0) DEFAULT 0,
  `TienInternet` decimal(18,0) DEFAULT 0,
  `TienVeSinh` decimal(18,0) DEFAULT 0,
  `TienGiuXe` decimal(18,0) DEFAULT 0,
  `ChiPhiKhac` decimal(18,0) DEFAULT 0,
  `DonGiaDien` decimal(18,0) DEFAULT NULL,
  `DonGiaNuoc` decimal(18,0) DEFAULT NULL,
  `SoDien` decimal(18,0) DEFAULT NULL,
  `SoNuoc` decimal(18,0) DEFAULT NULL,
  `TongTien` decimal(18,0) GENERATED ALWAYS AS (coalesce(`TienThue`,0) + coalesce(`TienDien`,0) + coalesce(`TienNuoc`,0) + coalesce(`TienInternet`,0) + coalesce(`TienVeSinh`,0) + coalesce(`TienGiuXe`,0) + coalesce(`ChiPhiKhac`,0)) STORED,
  `TrangThaiThanhToan` enum('Chưa trả','Đã trả','Trả một phần') DEFAULT 'Chưa trả',
  `NgayThanhToan` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


ALTER TABLE `Admin`
  ADD PRIMARY KEY (`MaAdmin`),
  ADD UNIQUE KEY `TenDangNhap` (`TenDangNhap`),
  ADD UNIQUE KEY `UK_Admin_MaNha` (`MaNha`);

ALTER TABLE `BaoTri_SuCo`
  ADD PRIMARY KEY (`MaSuCo`),
  ADD KEY `MaPhong` (`MaPhong`);

ALTER TABLE `DeletedMaintenanceSignatures`
  ADD PRIMARY KEY (`Id`);

ALTER TABLE `HopDong`
  ADD PRIMARY KEY (`MaHopDong`),
  ADD KEY `MaNguoiThue` (`MaNguoiThue`),
  ADD KEY `MaPhong` (`MaPhong`);

ALTER TABLE `NguoiThue`
  ADD PRIMARY KEY (`MaNguoiThue`),
  ADD UNIQUE KEY `CCCD` (`CCCD`);

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
  ADD KEY `MaHopDong` (`MaHopDong`);


ALTER TABLE `Admin`
  MODIFY `MaAdmin` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

ALTER TABLE `BaoTri_SuCo`
  MODIFY `MaSuCo` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=44;

ALTER TABLE `DeletedMaintenanceSignatures`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

ALTER TABLE `HopDong`
  MODIFY `MaHopDong` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `NguoiThue`
  MODIFY `MaNguoiThue` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

ALTER TABLE `Nha`
  MODIFY `MaNha` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

ALTER TABLE `Phong`
  MODIFY `MaPhong` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

ALTER TABLE `TaiSanNguoiThue`
  MODIFY `MaTaiSan` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

ALTER TABLE `ThanhToan`
  MODIFY `MaThanhToan` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;


ALTER TABLE `BaoTri_SuCo`
  ADD CONSTRAINT `BaoTri_SuCo_ibfk_1` FOREIGN KEY (`MaPhong`) REFERENCES `Phong` (`MaPhong`) ON DELETE CASCADE;

ALTER TABLE `HopDong`
  ADD CONSTRAINT `HopDong_ibfk_1` FOREIGN KEY (`MaNguoiThue`) REFERENCES `NguoiThue` (`MaNguoiThue`) ON DELETE CASCADE,
  ADD CONSTRAINT `HopDong_ibfk_2` FOREIGN KEY (`MaPhong`) REFERENCES `Phong` (`MaPhong`) ON DELETE CASCADE;

ALTER TABLE `Phong`
  ADD CONSTRAINT `Phong_ibfk_1` FOREIGN KEY (`MaNha`) REFERENCES `Nha` (`MaNha`) ON DELETE CASCADE;

ALTER TABLE `TaiSanNguoiThue`
  ADD CONSTRAINT `TaiSanNguoiThue_ibfk_1` FOREIGN KEY (`MaNguoiThue`) REFERENCES `NguoiThue` (`MaNguoiThue`) ON DELETE CASCADE;

ALTER TABLE `ThanhToan`
  ADD CONSTRAINT `ThanhToan_ibfk_1` FOREIGN KEY (`MaHopDong`) REFERENCES `HopDong` (`MaHopDong`) ON DELETE CASCADE;

-- Ràng buộc khóa ngoại cho Admin.MaNha -> Nha.MaNha
ALTER TABLE `Admin`
  ADD CONSTRAINT `FK_Admin_Nha` FOREIGN KEY (`MaNha`) REFERENCES `Nha` (`MaNha`) ON DELETE RESTRICT;

