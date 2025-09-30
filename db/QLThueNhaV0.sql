CREATE DATABASE QLThueNhaV0;
GO

USE QLThueNhaV0;
GO

CREATE TABLE Admin (
    MaAdmin INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) UNIQUE NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100),
    SoDienThoai NVARCHAR(15)
);

CREATE TABLE Nha (
    MaNha INT IDENTITY(1,1) PRIMARY KEY,
    DiaChi NVARCHAR(255) NOT NULL,
    TongSoPhong INT CHECK (TongSoPhong BETWEEN 1 AND 10),
    GhiChu NVARCHAR(255)
);

CREATE TABLE Phong (
    MaPhong INT IDENTITY(1,1) PRIMARY KEY,
    MaNha INT FOREIGN KEY REFERENCES Nha(MaNha) ON DELETE CASCADE,
    TenPhong NVARCHAR(50) NOT NULL,
    DienTich DECIMAL(5,2) CHECK (DienTich > 0),
    GiaCoBan DECIMAL(18,0) CHECK (GiaCoBan >= 0),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Trống', N'Đang thuê')),
    GhiChu NVARCHAR(255)
);

CREATE TABLE NguoiThue (
    MaNguoiThue INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai NVARCHAR(15) NOT NULL,
    CCCD NVARCHAR(20) UNIQUE,
    NgayBatDau DATE,
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Đang ở', N'Đã trả phòng')),
    GhiChu NVARCHAR(255)
);

CREATE TABLE HopDong (
    MaHopDong INT IDENTITY(1,1) PRIMARY KEY,
    MaNguoiThue INT FOREIGN KEY REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE,
    MaPhong INT FOREIGN KEY REFERENCES Phong(MaPhong) ON DELETE CASCADE,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    TienCoc DECIMAL(18,0) CHECK (TienCoc >= 0),
    FileHopDong NVARCHAR(255),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Hiệu lực', N'Hết hạn', N'Hủy'))
);

CREATE TABLE ThanhToan (
    MaThanhToan INT IDENTITY(1,1) PRIMARY KEY,
    MaHopDong INT FOREIGN KEY REFERENCES HopDong(MaHopDong) ON DELETE CASCADE,
    ThangNam CHAR(7) NOT NULL,  
    TienThue DECIMAL(18,0) DEFAULT 0,
    TienDien DECIMAL(18,0) DEFAULT 0,
    TienNuoc DECIMAL(18,0) DEFAULT 0,
    TienInternet DECIMAL(18,0) DEFAULT 0,
    TienVeSinh DECIMAL(18,0) DEFAULT 0,
    TienGiuXe DECIMAL(18,0) DEFAULT 0,
    ChiPhiKhac DECIMAL(18,0) DEFAULT 0,
    TongTien AS (TienThue + TienDien + TienNuoc + TienInternet + TienVeSinh + TienGiuXe + ChiPhiKhac) PERSISTED,
    TrangThaiThanhToan NVARCHAR(20) CHECK (TrangThaiThanhToan IN (N'Đã trả', N'Chưa trả')),
    NgayThanhToan DATE
);

CREATE TABLE BaoTri_SuCo (
    MaSuCo INT IDENTITY(1,1) PRIMARY KEY,
    MaPhong INT FOREIGN KEY REFERENCES Phong(MaPhong) ON DELETE CASCADE,
    MoTaSuCo NVARCHAR(255) NOT NULL,
    NgayBaoCao DATE DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) CHECK (TrangThai IN (N'Chưa xử lý', N'Đang xử lý', N'Hoàn tất')),
    ChiPhi DECIMAL(18,0) DEFAULT 0
);

CREATE TABLE TaiSanNguoiThue (
    MaTaiSan INT IDENTITY(1,1) PRIMARY KEY,
    MaNguoiThue INT FOREIGN KEY REFERENCES NguoiThue(MaNguoiThue) ON DELETE CASCADE,
    LoaiTaiSan NVARCHAR(20) CHECK (LoaiTaiSan IN (N'Xe', N'Thú cưng')),
    MoTa NVARCHAR(255),
    PhiPhuThu DECIMAL(18,0) DEFAULT 0
);

CREATE TRIGGER trg_CapNhatTrangThaiPhong
ON HopDong
AFTER INSERT
AS
BEGIN
    UPDATE Phong
    SET TrangThai = N'Đang thuê'
    WHERE MaPhong IN (SELECT MaPhong FROM inserted);
END;

CREATE TRIGGER trg_HopDongHetHan
ON HopDong
AFTER UPDATE
AS
BEGIN
    UPDATE Phong
    SET TrangThai = N'Trống'
    WHERE MaPhong IN (
        SELECT i.MaPhong 
        FROM inserted i 
        WHERE i.TrangThai = N'Hết hạn' OR i.TrangThai = N'Hủy'
    );
END;

ALTER TABLE HopDong
ADD CONSTRAINT CK_HopDong_Ngay CHECK (NgayKetThuc > NgayBatDau);

CREATE TRIGGER trg_DefaultThanhToan
ON ThanhToan
AFTER INSERT
AS
BEGIN
    UPDATE ThanhToan
    SET TrangThaiThanhToan = N'Chưa trả'
    WHERE MaThanhToan IN (SELECT MaThanhToan FROM inserted)
      AND TrangThaiThanhToan IS NULL;
END;

CREATE PROCEDURE sp_TaoHopDong
    @MaNguoiThue INT,
    @MaPhong INT,
    @NgayBatDau DATE,
    @NgayKetThuc DATE,
    @TienCoc DECIMAL(18,0),
    @FileHopDong NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TrangThaiPhong NVARCHAR(20);

    -- Kiểm tra phòng có đang trống không
    SELECT @TrangThaiPhong = TrangThai
    FROM Phong
    WHERE MaPhong = @MaPhong;

    IF @TrangThaiPhong <> N'Trống'
    BEGIN
        RAISERROR(N'Phòng không trống, không thể tạo hợp đồng mới!', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Thêm hợp đồng
        INSERT INTO HopDong (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, FileHopDong, TrangThai)
        VALUES (@MaNguoiThue, @MaPhong, @NgayBatDau, @NgayKetThuc, @TienCoc, @FileHopDong, N'Hiệu lực');

        -- Cập nhật trạng thái phòng
        UPDATE Phong
        SET TrangThai = N'Đang thuê'
        WHERE MaPhong = @MaPhong;

        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;

CREATE PROCEDURE sp_KetThucHopDong
    @MaHopDong INT,
    @TrangThai NVARCHAR(20)  -- 'Hết hạn' hoặc 'Hủy'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaPhong INT;

    SELECT @MaPhong = MaPhong
    FROM HopDong
    WHERE MaHopDong = @MaHopDong;

    IF @MaPhong IS NULL
    BEGIN
        RAISERROR(N'Hợp đồng không tồn tại!', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE HopDong
        SET TrangThai = @TrangThai
        WHERE MaHopDong = @MaHopDong;

        UPDATE Phong
        SET TrangThai = N'Trống'
        WHERE MaPhong = @MaPhong;

        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;

CREATE PROCEDURE sp_TaoThongBaoPhi
    @MaHopDong INT,
    @ThangNam CHAR(7) -- vd: '09/2025'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GiaCoBan DECIMAL(18,0);

    SELECT @GiaCoBan = p.GiaCoBan
    FROM HopDong h
    JOIN Phong p ON h.MaPhong = p.MaPhong
    WHERE h.MaHopDong = @MaHopDong
      AND h.TrangThai = N'Hiệu lực';

    IF @GiaCoBan IS NULL
    BEGIN
        RAISERROR(N'Hợp đồng không hợp lệ hoặc đã hết hạn!', 16, 1);
        RETURN;
    END

    INSERT INTO ThanhToan (MaHopDong, ThangNam, TienThue, TrangThaiThanhToan)
    VALUES (@MaHopDong, @ThangNam, @GiaCoBan, N'Chưa trả');
END;

CREATE PROCEDURE sp_GhiNhanSuCo
    @MaPhong INT,
    @MoTaSuCo NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Phong WHERE MaPhong = @MaPhong)
    BEGIN
        RAISERROR(N'Phòng không tồn tại!', 16, 1);
        RETURN;
    END

    INSERT INTO BaoTri_SuCo (MaPhong, MoTaSuCo, NgayBaoCao, TrangThai, ChiPhi)
    VALUES (@MaPhong, @MoTaSuCo, GETDATE(), N'Chưa xử lý', 0);
END;







