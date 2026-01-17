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
(1, 'admin', 'admin123', 'nhanhuutran007@gmail.com', '0909999888', 'Phạm Đức Mạnh', '1980-06-19', '918391391', '2025-10-30', 'Thanh Hóa', 'Bồ Đào Nha, Châu Âu', 1, 'TRAN HUU NHAN', '08699182255', '/Resources/Images/QR_20251126121439.jpg'),
(2, 'trong', 'ronaldo1\r\n', 'phuocvinh@gmail.com', '', NULL, NULL, NULL, NULL, NULL, NULL, 301, NULL, NULL, NULL),
(7, 'khoa', '3havZdjce1W5YXf+kAC24O8qseGV+Al9+uCwtMy4d4Q=', 'phdk1601@gmail.com', '', NULL, NULL, NULL, NULL, NULL, NULL, 2, NULL, NULL, NULL),
(8, 'manh', 'loqrKLDzMTi7Q8rxzxCjP8u9V5HKwfoUo3FywD2WjXE=', 'mpham3421@gmail.com', '', NULL, NULL, NULL, NULL, NULL, NULL, 36, NULL, NULL, NULL);

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
(37, 28, 'Hư tivi', '2025-09-12', '2025-12-09', 'Chưa xử lý', 0),
(38, 28, 'Hư tivi', '2025-09-12', '2025-12-09', 'Chưa xử lý', 0),
(39, 28, 'đau đụng', '2025-09-12', '2025-12-19', 'Chưa xử lý', 0),
(40, 28, 'đau đụng', '2025-09-12', '2025-12-19', 'Chưa xử lý', 0),
(41, 30, 'Hư đèn', '2025-09-12', '2025-12-17', 'Chưa xử lý', 0),
(42, 30, 'Hư đèn', '2025-09-12', '2025-12-17', 'Chưa xử lý', 0);

CREATE TABLE `DeletedMaintenanceSignatures` (
  `Id` int(11) NOT NULL,
  `MaPhong` int(11) NOT NULL,
  `MoTaSuCo` varchar(255) NOT NULL,
  `NgayBaoCao` date NOT NULL,
  `NgayCoTheSua` date NOT NULL,
  `NgayXoa` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `DeletedMaintenanceSignatures` (`Id`, `MaPhong`, `MoTaSuCo`, `NgayBaoCao`, `NgayCoTheSua`, `NgayXoa`) VALUES
(1, 18, 'Bị lỗi hệ thống', '2025-12-05', '0000-00-00', '2025-12-04 20:46:27'),
(2, 18, 'Bị đẹp trai', '2025-12-04', '0000-00-00', '2025-12-04 20:46:44'),
(3, 18, 'Bị đẹp trai', '2025-04-12', '0000-00-00', '2025-12-04 20:46:46'),
(4, 18, 'Bị lỗi hệ thống', '2025-04-12', '0000-00-00', '2025-12-04 20:46:50');

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
(37, 41, 29, '2025-12-09', '2026-12-09', 1700000, 'C:\\Users\\STRONGERMANY\\Documents\\HopDongPhongTro\\Lê Quang Minh_P.102_20251209_142612.pdf', 'Hiệu lực', NULL),
(39, 43, 31, '2025-11-01', '2025-12-01', 1200000, 'C:\\Users\\THINKPAD\\Documents\\HopDongPhongTro\\Trần Văn Nam_P.104_20251209_145621.pdf', 'Hiệu lực', NULL),
(40, 45, 33, '2025-12-09', '2026-01-08', 3800000, 'C:\\Users\\THINKPAD\\Documents\\HopDongPhongTro\\Lê Hoài Phong_P.106_20251209_153822.pdf', 'Hiệu lực', NULL),
(41, 42, 30, '2025-12-09', '2026-12-09', 5000000, 'C:\\Users\\User\\Documents\\HopDongPhongTro\\Nguyễn Minh Khôi_P.103_20251209_172043.pdf', 'Hiệu lực', NULL),
(43, 53, 28, '2025-12-09', '2026-12-09', 9000000, 'C:\\Users\\User\\Documents\\HopDongPhongTro\\Đăng Khôi_P.101_20251209_183416.pdf', 'Hiệu lực', 'Hợp đồng mới được tạo sau khi xóa người thuê cũ (HD#42)');

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
(41, 29, 'Lê Quang Minh', '0985123236', '098238232311', 'quangminh@gmail.com', 'Nam', '2025-12-09', 'Đang ở', '', '1990-12-23', 'Thôn Thiện Tây, Xã Gia Bi, Tỉnh Quảng Bình', 'Bác sĩ', '2019-01-28', 'Quảng Bình'),
(42, 30, 'Nguyễn Minh Khôi', '0638954836', '064783956783', 'ỉwhgojs@gmail.com', 'Nam', '2025-12-09', 'Đang ở', '', '1991-03-06', 'Hà Nội', 'Kỹ sư phần mềm', '2025-12-09', 'hà nội '),
(43, 31, 'Trần Văn Nam', '0905566778', '034567891234', 'lengochai.megas@gmail.com', 'Nam', '2025-10-02', 'Đang ở', '', '1992-03-11', 'P. Trường An, TP. Huế', 'Kỹ sư xây dựng', '2017-12-12', 'Thừa Thiên Huế'),
(44, 31, 'Phạm Thu Trang', '0987654433', '023456789123', 'thutrang@gmail.com', 'Nữ', '2025-10-25', 'Đã trả phòng', '', '1997-09-15', 'P. Nghĩa Tân, Cầu Giấy, Hà Nội', 'Giáo viên', '2020-04-10', 'Hà Nội'),
(45, 33, 'Lê Hoài Phong', '0912233445', '067891234567', 'molitran1@gmail.com', 'Nam', '2025-09-28', 'Đang ở', '', '1989-07-04', 'P. Phước Long B, TP. Thủ Đức', 'Lập trình viên', '2016-03-22', 'TP.HCM'),
(47, 35, 'Đỗ Văn Lực', '0901122334', '089123456712', 'vanluc@gmail.com', 'Nam', '2025-11-28', 'Đang ở', '', '1991-11-12', 'P. Hòa Khánh Bắc, Liên Chiểu, Đà Nẵng', 'Tài xế', '2018-08-30', 'Đà Nẵng'),
(48, 35, 'Võ Thị Kim Ngân', '0988112233', '045612378912', 'kimngan@gmail.com', 'Nữ', '2025-09-12', 'Đang ở', '', '1996-06-21', 'P. 7, TP. Vũng Tàu', 'Thiết kế đồ họa', '2020-01-19', 'Bà Rịa - Vũng Tàu'),
(50, 37, 'Ngô Minh Châu', '0945566771', '054321987654', 'minhchau@gmail.com', 'Nữ', '2025-11-05', 'Đang ở', '', '1998-04-18', 'TT. Châu Thành, An Giang', 'Dược sĩ', '2021-05-24', 'An Giang'),
(51, 37, 'Trịnh Quốc Khánh', '0932445566', '065432198765', 'quockhanh@gmail.com', 'Nam', '2025-09-07', 'Đang ở', '', '1990-08-25', 'P. Bình Hưng Hòa, Bình Tân, TP.HCM', 'Thợ điện', '2018-02-17', 'TP.HCM'),
(52, 29, 'Nguyễn Thị Hồng', '0912345678', '012345678912', 'hongnguyen@gmail.com', 'Nữ', '2025-11-15', 'Đang ở', '', '1994-05-20', 'P. Bến Nghé, Q1, TP.HCM', 'Nhân viên văn phòng', '2018-10-02', 'TP.HCM'),
(53, 28, 'Đăng Khôi', '0949207456', '123456789101', 'phdk2602@gmail.com', 'Nam', '2025-12-09', 'Đang ở', '', '1900-12-09', 'Tây Ninh', 'IT', '2025-12-09', 'Tây Ninh');

CREATE TABLE `Nha` (
  `MaNha` int(11) NOT NULL,
  `DiaChi` varchar(255) NOT NULL,
  `TongSoPhong` int(11) DEFAULT NULL CHECK (`TongSoPhong` > 0),
  `GhiChu` varchar(255) DEFAULT NULL,
  `TinhThanh` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `Nha` (`MaNha`, `DiaChi`, `TongSoPhong`, `GhiChu`, `TinhThanh`) VALUES
(1, '15/2 Đường CMT8, Quận 10, TP.HCM', 10, 'Nhà trọ Happy House - Cổng vân tay', 'TPHCM'),
(2, 'Chưa cập nhật', 1, NULL, ''),
(3, 'Chưa cập nhật', 1, NULL, ''),
(36, 'Chưa cập nhật', 10, NULL, ''),
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
(28, 1, 'P.101', 50.00, 9000000, 'Đang thuê', '', NULL, 'Chín triệu đồng ', ''),
(29, 1, 'P.102', 78.00, 1700000, 'Đang thuê', '', NULL, 'Một triệu bảy trăm nghìn đồng', 'Tủ lạnh, bếp, máy đun siêu tốc'),
(30, 1, 'P.103', 40.00, 5000000, 'Đang thuê', '', NULL, 'Năm triệu đồng ', 'máy lạnh, tủ lạnh , máy giặt'),
(31, 1, 'P.104', 30.00, 1200000, 'Đang thuê', '', NULL, 'Một triệu hai trăm nghìn đồng', 'Máy lạnh, tivi'),
(32, 1, 'P.105', 30.00, 4000000, 'Trống', '', NULL, 'Bốn triệu ', 'Bếp , giường'),
(33, 1, 'P.106', 80.00, 3800000, 'Đang thuê', '', NULL, 'Ba triệu tám trăm nghìn đồng', 'Máy giặt, máy rửa chén'),
(34, 1, 'P.107', 40.00, 3000000, 'Trống', '', NULL, 'Ba triệu đồng ', 'tivi , điện thoại'),
(35, 1, 'P.108', 40.00, 2200000, 'Trống', '', NULL, 'Hai triệu hai trăm nghìn đồng', 'Ban công, bồn tắm'),
(36, 1, 'P.109', 20.00, 2500000, 'Đang thuê', '', NULL, 'Hai triệu năm trăm nghìn đồng ', 'quạt , tủ lạnh , máy giặt'),
(37, 1, 'P.110', 100.00, 6000000, 'Trống', '', NULL, 'Sáu triệu đồng', 'Phòng Penthouse');

CREATE TABLE `TaiSanNguoiThue` (
  `MaTaiSan` int(11) NOT NULL,
  `MaNguoiThue` int(11) DEFAULT NULL,
  `LoaiTaiSan` enum('Xe','Thú cưng','Khác') DEFAULT 'Xe',
  `MoTa` varchar(255) DEFAULT NULL,
  `PhiPhuThu` decimal(18,0) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `TaiSanNguoiThue` (`MaTaiSan`, `MaNguoiThue`, `LoaiTaiSan`, `MoTa`, `PhiPhuThu`) VALUES
(14, 41, 'Xe', '', 0);

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

CREATE TABLE `User` (
  `MaUser` int(11) NOT NULL,
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

INSERT INTO `ThanhToan` (`MaThanhToan`, `MaHopDong`, `ThangNam`, `TienThue`, `TienDien`, `TienNuoc`, `TienInternet`, `TienVeSinh`, `TienGiuXe`, `ChiPhiKhac`, `DonGiaDien`, `DonGiaNuoc`, `ChiSoDienCu`, `ChiSoDienMoi`, `SoDien`, `SoNuoc`, `TongTien`, `SoTienDaTra`, `ConLai`, `TrangThaiThanhToan`, `NgayThanhToan`, `GhiChu`) VALUES
(25, 37, '11/2025', 3500000, 350000, 200000, 100000, 60000, 120000, 0, 3500, 100000, 1200, 1300, 100, 2, 4330000, 4330000, 0, 'Đã trả', '2025-11-05 00:00:00', 'Thanh toán đúng hạn'),
(26, 39, '11/2025', 4000000, 525000, 300000, 100000, 60000, 240000, 0, 3500, 100000, 2500, 2650, 150, 3, 5225000, 5225000, 0, 'Đã trả', '2025-11-05 00:00:00', 'Đang chờ khách chuyển khoản'),
(27, 40, '11/2025', 2800000, 175000, 100000, 100000, 60000, 120000, 0, 3500, 100000, 850, 900, 50, 1, 3355000, 3355000, 0, 'Đã trả', '2025-11-05 00:00:00', 'Khách xin nợ lại tiền điện nước'),
(28, 41, '11/2025', 5000000, 1050000, 400000, 100000, 60000, 0, 50000, 3500, 100000, 3000, 3300, 300, 4, 6660000, 6660000, 0, 'Đã trả', '2025-11-03 00:00:00', 'Phí khác là tiền sửa khóa'),
(35, 43, '12/2025', 9000000, 2653000, 100000, 100000, 60000, 0, 0, 3500, 100000, 0, 758, 758, 1, 11913000, 0, 0, 'Chưa trả', NULL, 'DISCREPANCY: Manual=758|OCR=759|Status=PENDING');


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

ALTER TABLE `User`
  ADD PRIMARY KEY (`MaUser`),
  ADD UNIQUE KEY `TenDangNhap` (`TenDangNhap`),
  ADD KEY `FK_User_Nha` (`MaNha`);


ALTER TABLE `Admin`
  MODIFY `MaAdmin` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

ALTER TABLE `BaoTri_SuCo`
  MODIFY `MaSuCo` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=43;

ALTER TABLE `DeletedMaintenanceSignatures`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

ALTER TABLE `GoogleFormLog`
  MODIFY `MaLog` int(11) NOT NULL AUTO_INCREMENT;

ALTER TABLE `HopDong`
  MODIFY `MaHopDong` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=44;

ALTER TABLE `NguoiThue`
  MODIFY `MaNguoiThue` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=54;

ALTER TABLE `Nha`
  MODIFY `MaNha` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=302;

ALTER TABLE `Phong`
  MODIFY `MaPhong` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=38;

ALTER TABLE `TaiSanNguoiThue`
  MODIFY `MaTaiSan` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

ALTER TABLE `ThanhToan`
  MODIFY `MaThanhToan` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=36;

ALTER TABLE `User`
  MODIFY `MaUser` int(11) NOT NULL AUTO_INCREMENT;


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

ALTER TABLE `User`
  ADD CONSTRAINT `FK_User_Nha` FOREIGN KEY (`MaNha`) REFERENCES `Nha` (`MaNha`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
