USE githubio_QLT_Ver1;

-- ===================== Bảng Admin =====================
CREATE TABLE Admin (
  MaAdmin INT AUTO_INCREMENT PRIMARY KEY,
  TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
  MatKhau VARCHAR(255) NOT NULL,
  Email VARCHAR(100),
  SoDienThoai VARCHAR(15)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO Admin (TenDangNhap, MatKhau, Email, SoDienThoai)
VALUES ('admin', 'admin123', 'admin@example.com', '0901000001');

-- ===================== Bảng Nha =====================
CREATE TABLE Nha (
  MaNha INT AUTO_INCREMENT PRIMARY KEY,
  DiaChi VARCHAR(255) NOT NULL,
  TongSoPhong INT CHECK (TongSoPhong BETWEEN 1 AND 10),
  GhiChu VARCHAR(255)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO Nha (DiaChi, TongSoPhong, GhiChu)
VALUES
('123 Đường A, Quận 1, TP.HCM', 5, 'Nhà trung tâm'),
('456 Đường B, Quận 7, TP.HCM', 6, 'Gần khu công nghệ'),
('789 Đường C, Bình Thạnh', 4, 'Khu yên tĩnh'),
('12 Nguyễn Văn Linh, Quận 7', 8, 'Gần siêu thị'),
('99 Lý Thường Kiệt, Quận 10', 10, 'Gần trường học');

-- ===================== Bảng Phong =====================
CREATE TABLE Phong (
  MaPhong INT AUTO_INCREMENT PRIMARY KEY,
  MaNha INT,
  TenPhong VARCHAR(50) NOT NULL,
  DienTich DECIMAL(5,2) CHECK (DienTich > 0),
  GiaCoBan DECIMAL(18,0) DEFAULT 0 CHECK (GiaCoBan >= 0),
  TrangThai ENUM('Đang thuê','Trống') DEFAULT 'Trống',
  GhiChu VARCHAR(255),
  FOREIGN KEY (MaNha) REFERENCES Nha(MaNha) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu)
VALUES
(1, 'P101', 18.5, 3000000, 'Trống', 'Có cửa sổ'),
(1, 'P102', 20.0, 3500000, 'Trống', 'Gần cầu thang'),
(2, 'P201', 22.0, 3800000, 'Trống', 'Ban công nhỏ'),
(3, 'P301', 25.0, 4000000, 'Trống', 'Có ban công lớn'),
(4, 'P401', 28.0, 4500000, 'Trống', 'Phòng mới xây');

-- ===================== Bảng NguoiThue =====================
CREATE TABLE NguoiThue (
  MaNguoiThue INT AUTO_INCREMENT PRIMARY KEY,
  HoTen VARCHAR(100) NOT NULL,
  SoDienThoai VARCHAR(15) NOT NULL,
  CCCD VARCHAR(20) UNIQUE,
  NgayBatDau DATE,
  TrangThai ENUM('Đang ở','Đã trả phòng'),
  GhiChu VARCHAR(255)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO NguoiThue (HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu)
VALUES
('Nguyễn Văn A', '0911000001', '079123456001', CURRENT_DATE, 'Đang ở', ''),
('Trần Thị B', '0911000002', '079123456002', CURRENT_DATE, 'Đang ở', ''),
('Lê Văn C', '0911000003', '079123456003', CURRENT_DATE, 'Đang ở', ''),
('Phạm Thị D', '0911000004', '079123456004', CURRENT_DATE, 'Đã trả phòng', 'Chuyển đi'),
('Huỳnh Văn E', '0911000005', '079123456005', CURRENT_DATE, 'Đang ở', '');

-- ===================== Bảng HopDong =====================
CREATE TABLE HopDong (
  MaHopDong INT AUTO_INCREMENT PRIMARY KEY,
  MaNguoiThue INT,
  MaPhong INT,
  NgayBatDau DATE NOT NULL,
  NgayKetThuc DATE NOT NULL,
  TienCoc DECIMAL(18,0) DEFAULT 0 CHECK (TienCoc >= 0),
  FileHopDong VARCHAR(255),
  TrangThai ENUM('Hiệu lực','Hết hạn','Hủy'),
  FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE,
  FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong) ON DELETE CASCADE,
  CHECK (NgayKetThuc > NgayBatDau)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO HopDong (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, FileHopDong, TrangThai)
VALUES
(1, 1, '2025-01-01', '2025-07-01', 1000000, 'HD_A.pdf', 'Hiệu lực'),
(2, 2, '2025-02-01', '2025-08-01', 1500000, 'HD_B.pdf', 'Hiệu lực'),
(3, 3, '2025-03-01', '2025-09-01', 1200000, 'HD_C.pdf', 'Hiệu lực'),
(4, 4, '2025-04-01', '2025-10-01', 1000000, 'HD_D.pdf', 'Hủy'),
(5, 5, '2025-05-01', '2025-11-01', 1000000, 'HD_E.pdf', 'Hết hạn');

-- ===================== Bảng TaiSanNguoiThue =====================
CREATE TABLE TaiSanNguoiThue (
  MaTaiSan INT AUTO_INCREMENT PRIMARY KEY,
  MaNguoiThue INT,
  LoaiTaiSan ENUM('Xe','Thú cưng'),
  MoTa VARCHAR(255),
  PhiPhuThu DECIMAL(18,0) DEFAULT 0,
  FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO TaiSanNguoiThue (MaNguoiThue, LoaiTaiSan, MoTa, PhiPhuThu)
VALUES
(1, 'Xe', 'Xe máy Vision', 100000),
(2, 'Thú cưng', 'Mèo Anh lông ngắn', 200000),
(3, 'Xe', 'Xe máy Sirius', 100000),
(4, 'Thú cưng', 'Chó Poodle', 150000),
(5, 'Xe', 'Xe SH Mode', 200000);

-- ===================== Bảng BaoTri_SuCo =====================
CREATE TABLE BaoTri_SuCo (
  MaSuCo INT AUTO_INCREMENT PRIMARY KEY,
  MaPhong INT,
  MoTaSuCo VARCHAR(255) NOT NULL,
  NgayBaoCao DATE DEFAULT (CURRENT_DATE),
  TrangThai ENUM('Chưa xử lý','Đang xử lý','Hoàn tất') DEFAULT 'Chưa xử lý',
  ChiPhi DECIMAL(18,0) DEFAULT 0,
  FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO BaoTri_SuCo (MaPhong, MoTaSuCo, TrangThai, ChiPhi)
VALUES
(1, 'Hư vòi nước', 'Chưa xử lý', 0),
(2, 'Rò rỉ điện', 'Đang xử lý', 0),
(3, 'Máy lạnh hỏng', 'Hoàn tất', 300000),
(4, 'Cửa bị kẹt', 'Chưa xử lý', 0),
(5, 'Nước yếu', 'Đang xử lý', 0);

-- ===================== Bảng ThanhToan =====================
CREATE TABLE ThanhToan (
  MaThanhToan INT AUTO_INCREMENT PRIMARY KEY,
  MaHopDong INT,
  ThangNam CHAR(7) NOT NULL,
  TienThue DECIMAL(18,0) DEFAULT 0,
  TienDien DECIMAL(18,0) DEFAULT 0,
  TienNuoc DECIMAL(18,0) DEFAULT 0,
  TienInternet DECIMAL(18,0) DEFAULT 0,
  TienVeSinh DECIMAL(18,0) DEFAULT 0,
  TienGiuXe DECIMAL(18,0) DEFAULT 0,
  ChiPhiKhac DECIMAL(18,0) DEFAULT 0,
  DonGiaDien DECIMAL(18,0) DEFAULT NULL,
  DonGiaNuoc DECIMAL(18,0) DEFAULT NULL,
  SoDien DECIMAL(18,0) DEFAULT NULL,
  SoNuoc DECIMAL(18,0) DEFAULT NULL,
  TongTien DECIMAL(18,0) GENERATED ALWAYS AS (
    COALESCE(TienThue, 0) + COALESCE(TienDien, 0) + COALESCE(TienNuoc, 0) + 
    COALESCE(TienInternet, 0) + COALESCE(TienVeSinh, 0) + COALESCE(TienGiuXe, 0) + COALESCE(ChiPhiKhac, 0)
  ) STORED,
  TrangThaiThanhToan ENUM('Chưa trả','Đã trả') DEFAULT 'Chưa trả',
  NgayThanhToan DATE,
  FOREIGN KEY (MaHopDong) REFERENCES HopDong(MaHopDong) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO ThanhToan (MaHopDong, ThangNam, TienThue, TienDien, TienNuoc, TienInternet, TienVeSinh, TienGiuXe, ChiPhiKhac, DonGiaDien, DonGiaNuoc, SoDien, SoNuoc, TrangThaiThanhToan, NgayThanhToan)
VALUES
-- TongTien sẽ tự động tính: 3000000 + 200000 + 100000 + 100000 + 50000 + 100000 + 0 = 3650000
(1, '01/2025', 3000000, 200000, 100000, 100000, 50000, 100000, 0, NULL, NULL, NULL, NULL, 'Đã trả', '2025-01-05'),
-- TongTien sẽ tự động tính: 3500000 + 250000 + 120000 + 100000 + 60000 + 120000 + 0 = 4200000
(2, '02/2025', 3500000, 250000, 120000, 100000, 60000, 120000, 0, NULL, NULL, NULL, NULL, 'Đã trả', '2025-02-05'),
-- TongTien sẽ tự động tính: 3800000 + 200000 + 100000 + 100000 + 50000 + 100000 + 0 = 4450000
(3, '03/2025', 3800000, 200000, 100000, 100000, 50000, 100000, 0, NULL, NULL, NULL, NULL, 'Chưa trả', NULL),
-- TongTien sẽ tự động tính: 4000000 + 250000 + 120000 + 100000 + 60000 + 120000 + 0 = 4700000
(4, '04/2025', 4000000, 250000, 120000, 100000, 60000, 120000, 0, NULL, NULL, NULL, NULL, 'Chưa trả', NULL),
-- TongTien sẽ tự động tính: 4500000 + 250000 + 120000 + 100000 + 60000 + 120000 + 0 = 5200000
(5, '05/2025', 4500000, 250000, 120000, 100000, 60000, 120000, 0, NULL, NULL, NULL, NULL, 'Đã trả', '2025-05-06');