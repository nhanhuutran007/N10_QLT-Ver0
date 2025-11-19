SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
-- Tạo database nếu chưa có
CREATE DATABASE IF NOT EXISTS `githubio_QLT_Ver1` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `githubio_QLT_Ver1`;

-- ===================== 1. Bảng Admin =====================
CREATE TABLE IF NOT EXISTS Admin (
  MaAdmin INT AUTO_INCREMENT PRIMARY KEY,
  TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
  MatKhau VARCHAR(255) NOT NULL,
  Email VARCHAR(100),
  SoDienThoai VARCHAR(15),
  MaNha INT NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO Admin (TenDangNhap, MatKhau, Email, SoDienThoai, MaNha)
VALUES ('admin', 'admin123', 'admin@example.com', '0901000001', 1);

-- ===================== 2. Bảng Nha =====================
-- Yêu cầu 2.1: Đăng ký thông tin căn nhà (địa chỉ, tổng số phòng, ghi chú).
CREATE TABLE IF NOT EXISTS Nha (
  MaNha INT AUTO_INCREMENT PRIMARY KEY,
  DiaChi VARCHAR(255) NOT NULL,
  TongSoPhong INT CHECK (TongSoPhong BETWEEN 1 AND 50), -- Đã mở rộng giới hạn phòng
  GhiChu VARCHAR(255)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO Nha (DiaChi, TongSoPhong, GhiChu) VALUES
('123 Đường A, Quận 1, TP.HCM', 5, 'Nhà trung tâm'),
('456 Đường B, Quận 7, TP.HCM', 6, 'Gần khu công nghệ'),
('789 Đường C, Bình Thạnh', 4, 'Khu yên tĩnh'),
('12 Nguyễn Văn Linh, Quận 7', 8, 'Gần siêu thị'),
('99 Lý Thường Kiệt, Quận 10', 10, 'Gần trường học');

-- Ràng buộc Admin quản lý Nhà
ALTER TABLE Admin
  ADD CONSTRAINT FK_Admin_Nha
  FOREIGN KEY (MaNha) REFERENCES Nha(MaNha) ON DELETE RESTRICT;

-- ===================== 3. Bảng Phong =====================
-- Yêu cầu 2.1: Thông tin phòng, Trạng thái (trống/đang thuê/dự kiến), Ghi chú riêng (sửa chữa, ngày có thể thuê)
CREATE TABLE IF NOT EXISTS Phong (
  MaPhong INT AUTO_INCREMENT PRIMARY KEY,
  MaNha INT,
  TenPhong VARCHAR(50) NOT NULL,
  DienTich DECIMAL(5,2) CHECK (DienTich > 0),
  GiaCoBan DECIMAL(18,0) DEFAULT 0 CHECK (GiaCoBan >= 0),
  
  -- SỬA LOGIC: Cập nhật ENUM để khớp yêu cầu bài toán
  TrangThai ENUM('Trống', 'Đang thuê', 'Dự kiến', 'Bảo trì') DEFAULT 'Trống',
  
  GhiChu VARCHAR(255) DEFAULT NULL COMMENT 'Ghi chú chung (VD: Cửa sổ hướng Nam)',
  
  -- THÊM MỚI: Để quản lý ngày phòng sẵn sàng (khi đang bảo trì hoặc sắp trả)
  NgayCoTheChoThue DATE NULL COMMENT 'Ngày dự kiến có thể đón khách mới', 
  
  GiaBangChu VARCHAR(255) NULL,
  TrangThietBi VARCHAR(500) NULL,
  
  FOREIGN KEY (MaNha) REFERENCES Nha(MaNha) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu, NgayCoTheChoThue, GiaBangChu, TrangThietBi)
VALUES
(1, 'P101', 18.5, 3000000, 'Trống', 'Phòng sạch', CURRENT_DATE, 'Ba triệu đồng', 'Điều hòa, Nóng lạnh'),
(1, 'P102', 20.0, 3500000, 'Bảo trì', 'Đang sửa ống nước', '2025-12-01', 'Ba triệu năm trăm', 'Điều hòa'),
(2, 'P201', 22.0, 3800000, 'Dự kiến', 'Khách đã cọc giữ chỗ', '2025-11-20', 'Ba triệu tám', 'Full nội thất'),
(3, 'P301', 25.0, 4000000, 'Trống', 'Ban công lớn', CURRENT_DATE, 'Bốn triệu', 'Không có'),
(4, 'P401', 28.0, 4500000, 'Đang thuê', 'Hợp đồng dài hạn', NULL, 'Bốn triệu năm', 'Tủ lạnh');

-- ===================== 4. Bảng NguoiThue =====================
-- Yêu cầu 2.2: Thông tin người thuê, CMND/CCCD, Tình trạng thuê (đang ở/đã trả/lịch hẹn trả)
CREATE TABLE IF NOT EXISTS NguoiThue (
  MaNguoiThue INT AUTO_INCREMENT PRIMARY KEY,
  HoTen VARCHAR(100) NOT NULL,
  SoDienThoai VARCHAR(15) NOT NULL,
  CCCD VARCHAR(20) UNIQUE NOT NULL COMMENT 'Bao gồm CMND/CCCD/Hộ chiếu',
  Email VARCHAR(100) NULL,
  GioiTinh ENUM('Nam', 'Nữ', 'Khác') NULL,
  NgheNghiep VARCHAR(100) NULL,
  NgayBatDau DATE,
  
  -- SỬA LOGIC: Thêm 'Sắp trả phòng' để quản lý lịch hẹn trả
  TrangThai ENUM('Đang ở', 'Đã trả phòng', 'Sắp trả phòng') DEFAULT 'Đang ở',
  
  GhiChu VARCHAR(500) NULL,
  NgaySinh DATE NULL,
  NgayCap DATE NULL,
  NoiCap VARCHAR(100) NULL,
  DiaChi VARCHAR(255) NULL,
  NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
  NgayCapNhat DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO NguoiThue (HoTen, SoDienThoai, CCCD, Email, GioiTinh, NgayBatDau, TrangThai, GhiChu)
VALUES
('Nguyễn Văn A', '0901000001', '001200000001', 'a@email.com', 'Nam', '2025-01-01', 'Đang ở', ''),
('Trần Thị B', '0901000002', '001200000002', 'b@email.com', 'Nữ', '2025-02-01', 'Sắp trả phòng', 'Hẹn trả ngày 30/11'),
('Lê Văn C', '0901000003', '001200000003', 'c@email.com', 'Nam', '2025-03-01', 'Đang ở', ''),
('Phạm Thị D', '0901000004', '001200000004', 'd@email.com', 'Nữ', '2024-01-01', 'Đã trả phòng', 'Đã thanh toán đủ');

-- ===================== 5. Bảng HopDong =====================
-- Yêu cầu 2.2: Hợp đồng thuê (file đính kèm), thông tin thuê
CREATE TABLE IF NOT EXISTS HopDong (
  MaHopDong INT AUTO_INCREMENT PRIMARY KEY,
  MaNguoiThue INT,
  MaPhong INT,
  NgayBatDau DATE NOT NULL,
  NgayKetThuc DATE NOT NULL,
  TienCoc DECIMAL(18,0) DEFAULT 0 CHECK (TienCoc >= 0),
  
  -- File đính kèm PDF/Word
  FileHopDong VARCHAR(255) COMMENT 'Đường dẫn lưu file hợp đồng',
  
  TrangThai ENUM('Hiệu lực','Hết hạn','Hủy', 'Sắp hết hạn') DEFAULT 'Hiệu lực',
  
  FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE,
  FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong) ON DELETE CASCADE,
  CHECK (NgayKetThuc > NgayBatDau)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO HopDong (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, FileHopDong, TrangThai)
VALUES
(1, 1, '2025-01-01', '2025-07-01', 3000000, 'contracts/HD_001.pdf', 'Hiệu lực'),
(2, 2, '2025-02-01', '2025-11-30', 3500000, 'contracts/HD_002.pdf', 'Sắp hết hạn');

-- ===================== 6. Bảng TaiSanNguoiThue =====================
CREATE TABLE IF NOT EXISTS TaiSanNguoiThue (
  MaTaiSan INT AUTO_INCREMENT PRIMARY KEY,
  MaNguoiThue INT,
  LoaiTaiSan ENUM('Xe','Thú cưng', 'Khác') DEFAULT 'Xe',
  MoTa VARCHAR(255),
  PhiPhuThu DECIMAL(18,0) DEFAULT 0,
  FOREIGN KEY (MaNguoiThue) REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ===================== 7. Bảng BaoTri_SuCo =====================
-- Ghi chú bảo trì cho từng phòng
CREATE TABLE IF NOT EXISTS BaoTri_SuCo (
  MaSuCo INT AUTO_INCREMENT PRIMARY KEY,
  MaPhong INT,
  MoTaSuCo VARCHAR(255) NOT NULL,
  NgayBaoCao DATE DEFAULT (CURRENT_DATE),
  TrangThai ENUM('Chưa xử lý','Đang xử lý','Hoàn tất') DEFAULT 'Chưa xử lý',
  ChiPhi DECIMAL(18,0) DEFAULT 0,
  FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ===================== 8. Bảng ThanhToan =====================
CREATE TABLE IF NOT EXISTS ThanhToan (
  MaThanhToan INT AUTO_INCREMENT PRIMARY KEY,
  MaHopDong INT,
  ThangNam CHAR(7) NOT NULL, -- Định dạng: MM/yyyy
  
  -- CÁC KHOẢN PHÍ CỐ ĐỊNH & BIẾN ĐỔI
  TienThue DECIMAL(18,0) DEFAULT 0,
  TienInternet DECIMAL(18,0) DEFAULT 0,
  TienVeSinh DECIMAL(18,0) DEFAULT 0,
  TienGiuXe DECIMAL(18,0) DEFAULT 0,
  ChiPhiKhac DECIMAL(18,0) DEFAULT 0,

  -- ĐIỆN NƯỚC (Dữ liệu từ AI/Form hoặc nhập tay)
  DonGiaDien DECIMAL(18,0) DEFAULT 3500,
  DonGiaNuoc DECIMAL(18,0) DEFAULT 100000, 
  
  ChiSoDienCu DECIMAL(18,2) DEFAULT 0,
  ChiSoDienMoi DECIMAL(18,2) DEFAULT 0,
  
  SoDien DECIMAL(18,2) DEFAULT 0,
  SoNuoc DECIMAL(18,2) DEFAULT 1, -- Mặc định 1 người/khối nếu tính theo đầu người
  
  TienDien DECIMAL(18,0) DEFAULT 0, 
  TienNuoc DECIMAL(18,0) DEFAULT 0,
  
  -- TỔNG HỢP TÀI CHÍNH
  TongTien DECIMAL(18,0) DEFAULT 0 COMMENT 'Tổng số tiền phải thanh toán trong tháng',

  -- [CẬP NHẬT MỚI] QUẢN LÝ THANH TOÁN & CÔNG NỢ
  SoTienDaTra DECIMAL(18,0) DEFAULT 0 COMMENT 'Số tiền thực tế khách đã đóng',
  
  -- Cột ảo tự động tính công nợ (Chỉ hỗ trợ MySQL 5.7 trở lên, nếu thấp hơn thì bỏ dòng này và tính trong code)
  ConLai DECIMAL(18,0) AS (TongTien - SoTienDaTra) STORED COMMENT 'Công nợ còn thiếu',

  -- Cập nhật ENUM theo yêu cầu 2.5
  TrangThaiThanhToan ENUM('Chưa trả', 'Trả một phần', 'Đã trả') DEFAULT 'Chưa trả',
  
  NgayThanhToan DATE COMMENT 'Ngày thực hiện giao dịch gần nhất',
  NgayTao DATETIME DEFAULT CURRENT_TIMESTAMP,
  GhiChu VARCHAR(500) COMMENT 'Ghi chú (VD: Chuyển khoản, Tiền mặt...)',
  
  FOREIGN KEY (MaHopDong) REFERENCES HopDong(MaHopDong) ON DELETE CASCADE,
  UNIQUE (MaHopDong, ThangNam) -- Đảm bảo mỗi hợp đồng chỉ có 1 phiếu thu mỗi tháng
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ===================== 9. Bảng GoogleFormLog =====================
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