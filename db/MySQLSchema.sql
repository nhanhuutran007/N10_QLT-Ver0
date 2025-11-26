USE githubio_QLT_Ver1;

-- ===================== Bảng Admin =====================
CREATE TABLE IF NOT EXISTS Admin (
  MaAdmin INT AUTO_INCREMENT PRIMARY KEY,
  TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
  MatKhau VARCHAR(255) NOT NULL,
  Email VARCHAR(100),
  SoDienThoai VARCHAR(15),
  HoTen VARCHAR(100),
  NgaySinh DATE NULL,
  CCCD VARCHAR(20),
  NgayCap DATE NULL,
  NoiCap VARCHAR(100),
  DiaChi VARCHAR(255),
  MaNha INT NOT NULL UNIQUE,
  TenTK VARCHAR(255) NULL,
  SoTK VARCHAR(50) NULL,
  LinkQr VARCHAR(500) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Kiểm tra tồn tại trước khi Insert để tránh lỗi duplicate khi chạy lại script
INSERT IGNORE INTO Admin (TenDangNhap, MatKhau, Email, SoDienThoai, HoTen, NgaySinh, CCCD, NgayCap, NoiCap, DiaChi, MaNha, TenTK, SoTK, LinkQr)
VALUES (
  'admin',
  'admin123',
  'admin@example.com',
  '0901000001',
  'Nguyễn Văn A',
  '1985-05-20',
  '012345678901',
  '2020-01-15',
  'Công an TP.HCM',
  '123 Đường A, Quận 1, TP.HCM',
  1,
  NULL,
  NULL,
  NULL
);

-- ===================== Bảng Nha =====================
CREATE TABLE IF NOT EXISTS Nha (
  MaNha INT AUTO_INCREMENT PRIMARY KEY,
  DiaChi VARCHAR(255) NOT NULL,
  TinhThanh VARCHAR(100) NOT NULL,
  TongSoPhong INT CHECK (TongSoPhong BETWEEN 1 AND 10),
  GhiChu VARCHAR(255)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO Nha (DiaChi, TinhThanh, TongSoPhong, GhiChu)
VALUES
('123 Đường A, Quận 1, TP.HCM', 'TP.HCM', 5, 'Nhà trung tâm'),
('456 Đường B, Quận 7, TP.HCM', 'TP.HCM', 6, 'Gần khu công nghệ'),
('789 Đường C, Bình Thạnh', 'TP.HCM', 4, 'Khu yên tĩnh'),
('12 Nguyễn Văn Linh, Quận 7', 'TP.HCM', 8, 'Gần siêu thị'),
('99 Lý Thường Kiệt, Quận 10', 'TP.HCM', 10, 'Gần trường học');

-- Thêm khóa ngoại cho Admin sau khi bảng Nha đã tồn tại
ALTER TABLE Admin
  ADD CONSTRAINT FK_Admin_Nha
  FOREIGN KEY (MaNha) REFERENCES Nha(MaNha)
  ON DELETE RESTRICT;

-- ===================== Bảng Phong (Đã Bổ Sung) =====================
CREATE TABLE IF NOT EXISTS Phong (
  MaPhong INT AUTO_INCREMENT PRIMARY KEY,
  MaNha INT,
  TenPhong VARCHAR(50) NOT NULL,
  DienTich DECIMAL(5,2) CHECK (DienTich > 0),
  GiaCoBan DECIMAL(18,0) DEFAULT 0 CHECK (GiaCoBan >= 0),
  TrangThai ENUM('Đang thuê','Trống') DEFAULT 'Trống',
  GhiChu VARCHAR(255),
  -- Bổ sung các cột mới
  GiaBangChu VARCHAR(255) NULL,
  TrangThietBi VARCHAR(500) NULL,
  FOREIGN KEY (MaNha) REFERENCES Nha(MaNha) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, GiaBangChu, TrangThietBi)
VALUES
(1, 'P101', 18.5, 3000000, 'Trống', 'Có cửa sổ', 'Ba triệu đồng', 'Điều hòa, Nóng lạnh'),
(1, 'P102', 20.0, 3500000, 'Trống', 'Gần cầu thang', 'Ba triệu năm trăm nghìn đồng', 'Điều hòa'),
(2, 'P201', 22.0, 3800000, 'Trống', 'Ban công nhỏ', 'Ba triệu tám trăm nghìn đồng', 'Full nội thất'),
(3, 'P301', 25.0, 4000000, 'Trống', 'Có ban công lớn', 'Bốn triệu đồng', 'Không có'),
(4, 'P401', 28.0, 4500000, 'Trống', 'Phòng mới xây', 'Bốn triệu năm trăm nghìn đồng', 'Điều hòa, Tủ lạnh');

-- ===================== Bảng NguoiThue (Đã Bổ Sung) =====================
CREATE TABLE IF NOT EXISTS NguoiThue (
  MaNguoiThue INT AUTO_INCREMENT PRIMARY KEY,
  HoTen VARCHAR(100) NOT NULL,
  SoDienThoai VARCHAR(15) NOT NULL,
  CCCD VARCHAR(20) UNIQUE NOT NULL,
  Email VARCHAR(100) NULL, -- Bổ sung
  GioiTinh ENUM('Nam', 'Nữ', 'Khác') NULL, -- Bổ sung
  NgheNghiep VARCHAR(100) NULL, -- Bổ sung
  NgayBatDau DATE,
  TrangThai ENUM('Đang ở','Đã trả phòng') DEFAULT 'Đang ở',
  GhiChu VARCHAR(500) NULL,
  -- Các trường mới chi tiết
  NgaySinh DATE NULL, -- Bổ sung
  NgayCap DATE NULL, -- Bổ sung
  NoiCap VARCHAR(100) NULL, -- Bổ sung
  DiaChi VARCHAR(255) NULL, -- Bổ sung
  -- Audit
  NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP, -- Bổ sung
  NgayCapNhat DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP -- Bổ sung
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


INSERT IGNORE INTO NguoiThue (HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgheNghiep, NgaySinh, NgayCap, NoiCap, DiaChi, NgayBatDau, TrangThai, GhiChu)
VALUES
('Nguyễn Văn A', '0911000001', '079123456001', 'vana@example.com', 'Nam', 'Sinh viên', '2000-01-15', '2020-05-10', 'CA TPHCM', '123 Nguyễn Trãi, Q1, TPHCM', CURRENT_DATE, 'Đang ở', ''),
('Trần Thị B', '0911000002', '079123456002', 'thib@example.com', 'Nữ', 'Nhân viên văn phòng', '1998-11-20', '2019-02-20', 'CA Hà Nội', '456 Lê Lợi, Q3, TPHCM', CURRENT_DATE, 'Đang ở', ''),
('Lê Văn C', '0911000003', '079123456003', 'vanc@example.com', 'Nam', 'Kỹ sư phần mềm', '1995-07-30', '2018-10-01', 'CA Đà Nẵng', '789 CMT8, Q10, TPHCM', CURRENT_DATE, 'Đang ở', ''),
('Phạm Thị D', '0911000004', '079123456004', 'thid@example.com', 'Nữ', 'Thiết kế', '2002-03-05', '2021-01-01', 'CA TPHCM', '101 Hai Bà Trưng, Q1, TPHCM', CURRENT_DATE, 'Đã trả phòng', 'Chuyển đi'),
('Huỳnh Văn E', '0911000005', '079123456005', 'vane@example.com', 'Nam', 'Tài xế', '1990-12-12', '2015-06-15', 'CA Cần Thơ', '202 Võ Thị Sáu, Q3, TPHCM', CURRENT_DATE, 'Đang ở', '');

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

-- ===================== Bảng TaiSanNguoiThue =====================
CREATE TABLE IF NOT EXISTS TaiSanNguoiThue (
  MaTaiSan INT AUTO_INCREMENT PRIMARY KEY,
  MaNguoiThue INT,
  LoaiTaiSan ENUM('Xe','Thú cưng'),
  MoTa VARCHAR(255),
  PhiPhuThu DECIMAL(18,0) DEFAULT 0,
  FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO TaiSanNguoiThue (MaNguoiThue, LoaiTaiSan, MoTa, PhiPhuThu)
VALUES
(1, 'Xe', 'Xe máy Vision', 100000),
(2, 'Thú cưng', 'Mèo Anh lông ngắn', 200000),
(3, 'Xe', 'Xe máy Sirius', 100000),
(4, 'Thú cưng', 'Chó Poodle', 150000),
(5, 'Xe', 'Xe SH Mode', 200000);

-- ===================== Bảng BaoTri_SuCo =====================
CREATE TABLE IF NOT EXISTS BaoTri_SuCo (
  MaSuCo INT AUTO_INCREMENT PRIMARY KEY,
  MaPhong INT,
  MoTaSuCo VARCHAR(255) NOT NULL,
  NgayBaoCao DATE DEFAULT (CURRENT_DATE),
  TrangThai ENUM('Chưa xử lý','Đang xử lý','Hoàn tất') DEFAULT 'Chưa xử lý',
  ChiPhi DECIMAL(18,0) DEFAULT 0,
  FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO BaoTri_SuCo (MaPhong, MoTaSuCo, TrangThai, ChiPhi)
VALUES
(1, 'Hư vòi nước', 'Chưa xử lý', 0),
(2, 'Rò rỉ điện', 'Đang xử lý', 0),
(3, 'Máy lạnh hỏng', 'Hoàn tất', 300000),
(4, 'Cửa bị kẹt', 'Chưa xử lý', 0),
(5, 'Nước yếu', 'Đang xử lý', 0);

-- ===================== Bảng ThanhToan (Đã Cập Nhật Logic Mới) =====================
-- Logic: Sử dụng cột thường thay vì Generated Columns để C# tự tính toán và cập nhật
CREATE TABLE IF NOT EXISTS ThanhToan (
  MaThanhToan INT AUTO_INCREMENT PRIMARY KEY,
  MaHopDong INT,
  ThangNam CHAR(7) NOT NULL, -- Định dạng MM/yyyy

  -- Các khoản tiền
  TienThue DECIMAL(18,0) DEFAULT 0,
  TienInternet DECIMAL(18,0) DEFAULT 0,
  TienVeSinh DECIMAL(18,0) DEFAULT 0,
  TienGiuXe DECIMAL(18,0) DEFAULT 0,
  ChiPhiKhac DECIMAL(18,0) DEFAULT 0,

  -- Điện Nước
  DonGiaDien DECIMAL(18,0) DEFAULT 3500,
  DonGiaNuoc DECIMAL(18,0) DEFAULT 100000, 
  
  ChiSoDienCu DECIMAL(18,2) DEFAULT 0, -- Bổ sung: Lấy từ tháng trước
  ChiSoDienMoi DECIMAL(18,2) DEFAULT 0, -- Bổ sung: Lấy từ Google Form
  
  SoDien DECIMAL(18,2) DEFAULT 0, -- Logic tính ở App: Mới - Cũ
  SoNuoc DECIMAL(18,2) DEFAULT 1,
  
  -- Các cột tiền (Cột thường, không phải generated)
  TienDien DECIMAL(18,0) DEFAULT 0, 
  TienNuoc DECIMAL(18,0) DEFAULT 0,
  TongTien DECIMAL(18,0) DEFAULT 0,

  TrangThaiThanhToan ENUM('Chưa trả','Đã trả') DEFAULT 'Chưa trả',
  NgayThanhToan DATE,
  NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
  GhiChu VARCHAR(500),
  
  FOREIGN KEY (MaHopDong) REFERENCES HopDong(MaHopDong) ON DELETE CASCADE,
  UNIQUE (MaHopDong, ThangNam)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO ThanhToan (MaHopDong, ThangNam, TienThue, TienInternet, TienVeSinh, TienGiuXe, ChiPhiKhac, DonGiaDien, DonGiaNuoc, ChiSoDienCu, ChiSoDienMoi, SoDien, TienDien, TienNuoc, TongTien, TrangThaiThanhToan, NgayThanhToan, GhiChu)
VALUES
(1, '01/2025', 3000000, 100000, 50000, 100000, 0, 3500, 100000, 100, 150, 50, 175000, 100000, 3425000, 'Đã trả', '2025-01-05', 'Thanh toán đầy đủ'),
(2, '02/2025', 3500000, 100000, 60000, 120000, 0, 3500, 100000, 150, 200, 50, 175000, 100000, 4055000, 'Đã trả', '2025-02-05', 'Khách hàng mới'),
(3, '03/2025', 3800000, 100000, 50000, 100000, 50000, 3500, 100000, 130, 180, 50, 175000, 100000, 4375000, 'Chưa trả', NULL, 'Có chi phí phát sinh'),
(4, '04/2025', 4000000, 100000, 60000, 120000, 0, 3500, 100000, 180, 220, 40, 140000, 100000, 4520000, 'Chưa trả', NULL, 'Chờ xác nhận'),
(5, '05/2025', 4500000, 100000, 60000, 120000, 100000, 3500, 100000, 200, 250, 50, 175000, 100000, 5155000, 'Đã trả', '2025-05-06', 'Có chi phí sửa chữa');

-- ===================== Bảng GoogleFormLog (Bổ Sung Mới) =====================
CREATE TABLE IF NOT EXISTS GoogleFormLog (
    MaLog INT AUTO_INCREMENT PRIMARY KEY,
    RoomName VARCHAR(50) NOT NULL,
    ElectricImageUrl VARCHAR(500),
    SubmittedValue DECIMAL(18,2),
    ExtractedValue DECIMAL(18,2),
    IsValid BIT DEFAULT 0, 
    ErrorMessage VARCHAR(500),
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    Processed BIT DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;