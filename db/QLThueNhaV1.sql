USE [master]
GO

/****** Object:  Database [QLThueNhaV1]    Script Date: 10/03/2025 ******/
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QLThueNhaV1')
BEGIN
    ALTER DATABASE [QLThueNhaV1] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [QLThueNhaV1];
END
GO

CREATE DATABASE [QLThueNhaV1]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'QLThueNhaV1', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\QLThueNhaV1.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'QLThueNhaV1_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\QLThueNhaV1_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO

ALTER DATABASE [QLThueNhaV1] SET COMPATIBILITY_LEVEL = 160
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
BEGIN
    EXEC [QLThueNhaV1].[dbo].[sp_fulltext_database] @action = 'enable'
END
GO

ALTER DATABASE [QLThueNhaV1] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET ANSI_NULLS ON 
GO
ALTER DATABASE [QLThueNhaV1] SET ANSI_PADDING ON 
GO
ALTER DATABASE [QLThueNhaV1] SET ANSI_WARNINGS ON 
GO
ALTER DATABASE [QLThueNhaV1] SET ARITHABORT ON 
GO
ALTER DATABASE [QLThueNhaV1] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [QLThueNhaV1] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET CURSOR_DEFAULT GLOBAL 
GO
ALTER DATABASE [QLThueNhaV1] SET CONCAT_NULL_YIELDS_NULL ON 
GO
ALTER DATABASE [QLThueNhaV1] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET QUOTED_IDENTIFIER ON 
GO
ALTER DATABASE [QLThueNhaV1] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET ENABLE_BROKER 
GO
ALTER DATABASE [QLThueNhaV1] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [QLThueNhaV1] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET RECOVERY FULL 
GO
ALTER DATABASE [QLThueNhaV1] SET MULTI_USER 
GO
ALTER DATABASE [QLThueNhaV1] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [QLThueNhaV1] SET DB_CHAINING OFF 
GO
ALTER DATABASE [QLThueNhaV1] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [QLThueNhaV1] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [QLThueNhaV1] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [QLThueNhaV1] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [QLThueNhaV1] SET QUERY_STORE = ON
GO
ALTER DATABASE [QLThueNhaV1] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200)
GO

USE [QLThueNhaV1]
GO

/****** Object:  Table [dbo].[Admin]    Script Date: 10/03/2025 ******/
CREATE TABLE [dbo].[Admin](
	[MaAdmin] [int] IDENTITY(1,1) NOT NULL,
	[TenDangNhap] [nvarchar](50) NOT NULL,
	[MatKhau] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[SoDienThoai] [nvarchar](15) NULL,
	[NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
	[NgayCapNhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaAdmin] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TenDangNhap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[BaoTri_SuCo]    Script Date: 10/03/2025 ******/
CREATE TABLE [dbo].[BaoTri_SuCo](
	[MaSuCo] [int] IDENTITY(1,1) NOT NULL,
	[MaPhong] [int] NOT NULL,
	[MoTaSuCo] [nvarchar](255) NOT NULL,
	[NgayBaoCao] [date] NOT NULL DEFAULT GETDATE(),
	[NgayHoanThanh] [date] NULL,
	[TrangThai] [nvarchar](20) NOT NULL DEFAULT N'Chưa xử lý',
	[ChiPhi] [decimal](18, 2) NOT NULL DEFAULT 0,
	[NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
	[NgayCapNhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaSuCo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[HopDong]    Script Date: 10/03/2025 ******/
CREATE TABLE [dbo].[HopDong](
	[MaHopDong] [int] IDENTITY(1,1) NOT NULL,
	[MaNguoiThue] [int] NOT NULL,
	[MaPhong] [int] NOT NULL,
	[NgayBatDau] [date] NOT NULL,
	[NgayKetThuc] [date] NOT NULL,
	[TienCoc] [decimal](18, 2) NOT NULL DEFAULT 0,
	[FileHopDong] [nvarchar](255) NULL,
	[TrangThai] [nvarchar](20) NOT NULL DEFAULT N'Hiệu lực',
	[NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
	[NgayCapNhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaHopDong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[NguoiThue]    Script Date: 10/03/2025 ******/
CREATE TABLE [dbo].[NguoiThue](
	[MaNguoiThue] [int] IDENTITY(1,1) NOT NULL,
	[HoTen] [nvarchar](100) NOT NULL,
	[SoDienThoai] [nvarchar](15) NOT NULL,
	[CCCD] [nvarchar](20) NULL UNIQUE,
	[NgaySinh] [date] NULL,
	[NgayBatDau] [date] NULL,
	[TrangThai] [nvarchar](20) NOT NULL DEFAULT N'Đang ở',
	[GhiChu] [nvarchar](255) NULL,
	[NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
	[NgayCapNhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNguoiThue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Nha]    Script Date: 10/03/2025 ******/
CREATE TABLE [dbo].[Nha](
	[MaNha] [int] IDENTITY(1,1) NOT NULL,
	[DiaChi] [nvarchar](255) NOT NULL,
	[TongSoPhong] [int] NOT NULL CHECK ([TongSoPhong] >= 1 AND [TongSoPhong] <= 50),
	[GhiChu] [nvarchar](255) NULL,
	[NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
	[NgayCapNhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Phong]    Script Date: 10/03/2025 ******/
CREATE TABLE [dbo].[Phong](
	[MaPhong] [int] IDENTITY(1,1) NOT NULL,
	[MaNha] [int] NOT NULL,
	[TenPhong] [nvarchar](50) NOT NULL,
	[DienTich] [decimal](5, 2) NOT NULL CHECK ([DienTich] > 0),
	[GiaCoBan] [decimal](18, 2) NOT NULL CHECK ([GiaCoBan] >= 0),
	[TrangThai] [nvarchar](20) NOT NULL DEFAULT N'Trống',
	[GhiChu] [nvarchar](255) NULL,
	[NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
	[NgayCapNhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[TaiSanNguoiThue]    Script Date: 10/03/2025 ******/
CREATE TABLE [dbo].[TaiSanNguoiThue](
	[MaTaiSan] [int] IDENTITY(1,1) NOT NULL,
	[MaNguoiThue] [int] NOT NULL,
	[LoaiTaiSan] [nvarchar](20) NOT NULL CHECK ([LoaiTaiSan] IN (N'Xe', N'Thú cưng', N'Khác')),
	[MoTa] [nvarchar](255) NULL,
	[PhiPhuThu] [decimal](18, 2) NOT NULL DEFAULT 0,
	[NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
	[NgayCapNhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaTaiSan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ThanhToan]    Script Date: 10/03/2025 ******/
CREATE TABLE [dbo].[ThanhToan](
	[MaThanhToan] [int] IDENTITY(1,1) NOT NULL,
	[MaHopDong] [int] NOT NULL,
	[ThangNam] [char](7) NOT NULL CHECK ([ThangNam] LIKE '[0-1][0-9]/[2][0][0-9][0-9]'),  -- Format MM/YYYY
	[TienThue] [decimal](18, 2) NOT NULL DEFAULT 0,
	[TienDien] [decimal](18, 2) NOT NULL DEFAULT 0,
	[TienNuoc] [decimal](18, 2) NOT NULL DEFAULT 0,
	[TienInternet] [decimal](18, 2) NOT NULL DEFAULT 0,
	[TienVeSinh] [decimal](18, 2) NOT NULL DEFAULT 0,
	[TienGiuXe] [decimal](18, 2) NOT NULL DEFAULT 0,
	[ChiPhiKhac] [decimal](18, 2) NOT NULL DEFAULT 0,
	[TongTien]  AS (((((([TienThue]+[TienDien])+[TienNuoc])+[TienInternet])+[TienVeSinh])+[TienGiuXe])+[ChiPhiKhac]) PERSISTED,
	[TrangThaiThanhToan] [nvarchar](20) NOT NULL DEFAULT N'Chưa trả',
	[NgayThanhToan] [date] NULL,
	[NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
	[NgayCapNhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaThanhToan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Foreign Keys with CASCADE DELETE
ALTER TABLE [dbo].[BaoTri_SuCo] ADD CONSTRAINT [FK_BaoTri_SuCo_Phong] FOREIGN KEY ([MaPhong]) REFERENCES [dbo].[Phong] ([MaPhong]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[HopDong] ADD CONSTRAINT [FK_HopDong_NguoiThue] FOREIGN KEY ([MaNguoiThue]) REFERENCES [dbo].[NguoiThue] ([MaNguoiThue]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HopDong] ADD CONSTRAINT [FK_HopDong_Phong] FOREIGN KEY ([MaPhong]) REFERENCES [dbo].[Phong] ([MaPhong]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Phong] ADD CONSTRAINT [FK_Phong_Nha] FOREIGN KEY ([MaNha]) REFERENCES [dbo].[Nha] ([MaNha]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TaiSanNguoiThue] ADD CONSTRAINT [FK_TaiSanNguoiThue_NguoiThue] FOREIGN KEY ([MaNguoiThue]) REFERENCES [dbo].[NguoiThue] ([MaNguoiThue]) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ThanhToan] ADD CONSTRAINT [FK_ThanhToan_HopDong] FOREIGN KEY ([MaHopDong]) REFERENCES [dbo].[HopDong] ([MaHopDong]) ON DELETE CASCADE
GO

-- Additional Constraints
ALTER TABLE [dbo].[BaoTri_SuCo] ADD CONSTRAINT [CK_BaoTri_SuCo_TrangThai] CHECK ([TrangThai] IN (N'Chưa xử lý', N'Đang xử lý', N'Hoàn tất'))
GO

ALTER TABLE [dbo].[HopDong] ADD CONSTRAINT [CK_HopDong_TienCoc] CHECK ([TienCoc] >= 0)
GO
ALTER TABLE [dbo].[HopDong] ADD CONSTRAINT [CK_HopDong_TrangThai] CHECK ([TrangThai] IN (N'Hiệu lực', N'Hết hạn', N'Hủy'))
GO
ALTER TABLE [dbo].[HopDong] ADD CONSTRAINT [CK_HopDong_Ngay] CHECK ([NgayKetThuc] > [NgayBatDau])
GO

ALTER TABLE [dbo].[NguoiThue] ADD CONSTRAINT [CK_NguoiThue_TrangThai] CHECK ([TrangThai] IN (N'Đang ở', N'Đã trả phòng'))
GO

ALTER TABLE [dbo].[Phong] ADD CONSTRAINT [CK_Phong_TrangThai] CHECK ([TrangThai] IN (N'Trống', N'Đang thuê', N'Đang bảo trì'))
GO

ALTER TABLE [dbo].[ThanhToan] ADD CONSTRAINT [CK_ThanhToan_TrangThai] CHECK ([TrangThaiThanhToan] IN (N'Chưa trả', N'Đã trả'))
GO

-- Indexes for performance
CREATE INDEX [IX_Phong_MaNha] ON [dbo].[Phong] ([MaNha])
GO
CREATE INDEX [IX_HopDong_MaPhong] ON [dbo].[HopDong] ([MaPhong])
GO
CREATE INDEX [IX_HopDong_MaNguoiThue] ON [dbo].[HopDong] ([MaNguoiThue])
GO
CREATE INDEX [IX_ThanhToan_MaHopDong] ON [dbo].[ThanhToan] ([MaHopDong])
GO
CREATE INDEX [IX_BaoTri_SuCo_MaPhong] ON [dbo].[BaoTri_SuCo] ([MaPhong])
GO

-- Triggers for NgayCapNhat
CREATE TRIGGER [dbo].[trg_UpdateNgayCapNhat_Admin] ON [dbo].[Admin] AFTER UPDATE AS BEGIN UPDATE [dbo].[Admin] SET NgayCapNhat = GETDATE() WHERE MaAdmin IN (SELECT MaAdmin FROM inserted) END
GO
CREATE TRIGGER [dbo].[trg_UpdateNgayCapNhat_BaoTri_SuCo] ON [dbo].[BaoTri_SuCo] AFTER UPDATE AS BEGIN UPDATE [dbo].[BaoTri_SuCo] SET NgayCapNhat = GETDATE() WHERE MaSuCo IN (SELECT MaSuCo FROM inserted) END
GO
CREATE TRIGGER [dbo].[trg_UpdateNgayCapNhat_HopDong] ON [dbo].[HopDong] AFTER UPDATE AS BEGIN UPDATE [dbo].[HopDong] SET NgayCapNhat = GETDATE() WHERE MaHopDong IN (SELECT MaHopDong FROM inserted) END
GO
CREATE TRIGGER [dbo].[trg_UpdateNgayCapNhat_NguoiThue] ON [dbo].[NguoiThue] AFTER UPDATE AS BEGIN UPDATE [dbo].[NguoiThue] SET NgayCapNhat = GETDATE() WHERE MaNguoiThue IN (SELECT MaNguoiThue FROM inserted) END
GO
CREATE TRIGGER [dbo].[trg_UpdateNgayCapNhat_Nha] ON [dbo].[Nha] AFTER UPDATE AS BEGIN UPDATE [dbo].[Nha] SET NgayCapNhat = GETDATE() WHERE MaNha IN (SELECT MaNha FROM inserted) END
GO
CREATE TRIGGER [dbo].[trg_UpdateNgayCapNhat_Phong] ON [dbo].[Phong] AFTER UPDATE AS BEGIN UPDATE [dbo].[Phong] SET NgayCapNhat = GETDATE() WHERE MaPhong IN (SELECT MaPhong FROM inserted) END
GO
CREATE TRIGGER [dbo].[trg_UpdateNgayCapNhat_TaiSanNguoiThue] ON [dbo].[TaiSanNguoiThue] AFTER UPDATE AS BEGIN UPDATE [dbo].[TaiSanNguoiThue] SET NgayCapNhat = GETDATE() WHERE MaTaiSan IN (SELECT MaTaiSan FROM inserted) END
GO
CREATE TRIGGER [dbo].[trg_UpdateNgayCapNhat_ThanhToan] ON [dbo].[ThanhToan] AFTER UPDATE AS BEGIN UPDATE [dbo].[ThanhToan] SET NgayCapNhat = GETDATE() WHERE MaThanhToan IN (SELECT MaThanhToan FROM inserted) END
GO

-- Stored Procedures
CREATE PROCEDURE [dbo].[sp_GhiNhanSuCo]
    @MaPhong INT,
    @MoTaSuCo NVARCHAR(255),
    @ChiPhi DECIMAL(18,2) = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Phong WHERE MaPhong = @MaPhong)
    BEGIN
        RAISERROR(N'Phòng không tồn tại!', 16, 1);
        RETURN;
    END

    INSERT INTO BaoTri_SuCo (MaPhong, MoTaSuCo, TrangThai, ChiPhi)
    VALUES (@MaPhong, @MoTaSuCo, N'Chưa xử lý', @ChiPhi);
END;
GO

CREATE PROCEDURE [dbo].[sp_KetThucHopDong]
    @MaHopDong INT,
    @TrangThai NVARCHAR(20)  -- 'Hết hạn' hoặc 'Hủy'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaPhong INT;

    SELECT @MaPhong = MaPhong
    FROM HopDong
    WHERE MaHopDong = @MaHopDong AND TrangThai = N'Hiệu lực';

    IF @MaPhong IS NULL
    BEGIN
        RAISERROR(N'Hợp đồng không tồn tại hoặc không còn hiệu lực!', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE HopDong
        SET TrangThai = @TrangThai, NgayCapNhat = GETDATE()
        WHERE MaHopDong = @MaHopDong;

        UPDATE Phong
        SET TrangThai = N'Trống', NgayCapNhat = GETDATE()
        WHERE MaPhong = @MaPhong;

        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE [dbo].[sp_TaoHopDong]
    @MaNguoiThue INT,
    @MaPhong INT,
    @NgayBatDau DATE,
    @NgayKetThuc DATE,
    @TienCoc DECIMAL(18,2),
    @FileHopDong NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TrangThaiPhong NVARCHAR(20);

    SELECT @TrangThaiPhong = TrangThai
    FROM Phong
    WHERE MaPhong = @MaPhong;

    IF @TrangThaiPhong <> N'Trống'
    BEGIN
        RAISERROR(N'Phòng không trống, không thể tạo hợp đồng mới!', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM NguoiThue WHERE MaNguoiThue = @MaNguoiThue)
    BEGIN
        RAISERROR(N'Người thuê không tồn tại!', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO HopDong (MaNguoiThue, MaPhong, NgayBatDau, NgayKetThuc, TienCoc, FileHopDong, TrangThai)
        VALUES (@MaNguoiThue, @MaPhong, @NgayBatDau, @NgayKetThuc, @TienCoc, @FileHopDong, N'Hiệu lực');

        UPDATE Phong
        SET TrangThai = N'Đang thuê', NgayCapNhat = GETDATE()
        WHERE MaPhong = @MaPhong;

        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE [dbo].[sp_TaoThongBaoPhi]
    @MaHopDong INT,
    @ThangNam CHAR(7),  -- vd: '10/2025'
    @TienDien DECIMAL(18,2) = 0,
    @TienNuoc DECIMAL(18,2) = 0,
    @TienInternet DECIMAL(18,2) = 0,
    @TienVeSinh DECIMAL(18,2) = 0,
    @TienGiuXe DECIMAL(18,2) = 0,
    @ChiPhiKhac DECIMAL(18,2) = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GiaCoBan DECIMAL(18,2);

    SELECT @GiaCoBan = p.GiaCoBan
    FROM HopDong h
    JOIN Phong p ON h.MaPhong = p.MaPhong
    WHERE h.MaHopDong = @MaHopDong AND h.TrangThai = N'Hiệu lực';

    IF @GiaCoBan IS NULL
    BEGIN
        RAISERROR(N'Hợp đồng không hợp lệ hoặc đã hết hạn!', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM ThanhToan WHERE MaHopDong = @MaHopDong AND ThangNam = @ThangNam)
    BEGIN
        RAISERROR(N'Thông báo phí cho tháng này đã tồn tại!', 16, 1);
        RETURN;
    END

    INSERT INTO ThanhToan (MaHopDong, ThangNam, TienThue, TienDien, TienNuoc, TienInternet, TienVeSinh, TienGiuXe, ChiPhiKhac, TrangThaiThanhToan)
    VALUES (@MaHopDong, @ThangNam, @GiaCoBan, @TienDien, @TienNuoc, @TienInternet, @TienVeSinh, @TienGiuXe, @ChiPhiKhac, N'Chưa trả');
END;
GO

-- Stored Procedure for deleting Nha and renumbering Phong
CREATE PROCEDURE [dbo].[sp_XoaNhaVaRenumberPhong]
    @MaNha INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ErrorMsg NVARCHAR(4000);

    IF NOT EXISTS (SELECT 1 FROM Nha WHERE MaNha = @MaNha)
    BEGIN
        RAISERROR(N'Căn nhà không tồn tại!', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Temp table for mapping old MaPhong to new
        CREATE TABLE #PhongMapping (
            MaPhongCu INT,
            MaPhongMoi INT
        );

        -- Get all remaining rooms after delete (exclude those in this Nha)
        INSERT INTO #PhongMapping (MaPhongCu)
        SELECT MaPhong FROM Phong WHERE MaNha != @MaNha
        ORDER BY MaNha ASC, TenPhong ASC;  -- Logical order: by house then room name

        -- Assign new consecutive IDs starting from 1
        DECLARE @NewId INT = 1;
        UPDATE #PhongMapping SET MaPhongMoi = @NewId, @NewId = @NewId + 1
        FROM #PhongMapping WITH (TABLOCKX);  -- Efficient update

        -- Update referencing tables with new MaPhong
        UPDATE h SET h.MaPhong = pm.MaPhongMoi
        FROM HopDong h
        INNER JOIN #PhongMapping pm ON h.MaPhong = pm.MaPhongCu;

        UPDATE b SET b.MaPhong = pm.MaPhongMoi
        FROM BaoTri_SuCo b
        INNER JOIN #PhongMapping pm ON b.MaPhong = pm.MaPhongCu;

        -- Delete the Nha (cascades to Phong)
        DELETE FROM Nha WHERE MaNha = @MaNha;

        -- Update Phong table with new IDs (temporarily allow IDENTITY insert)
        SET IDENTITY_INSERT [dbo].[Phong] ON;
        UPDATE p SET p.MaPhong = pm.MaPhongMoi
        FROM Phong p
        INNER JOIN #PhongMapping pm ON p.MaPhong = pm.MaPhongCu;
        SET IDENTITY_INSERT [dbo].[Phong] OFF;

        -- Reseed IDENTITY to max new ID
        DECLARE @MaxNewId INT = (SELECT ISNULL(MAX(MaPhongMoi), 0) FROM #PhongMapping);
        DBCC CHECKIDENT ('[dbo].[Phong]', RESEED, @MaxNewId);

        DROP TABLE #PhongMapping;

        COMMIT TRANSACTION;
        PRINT N'Xóa căn nhà thành công và renumber mã phòng!';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @ErrorMsg = ERROR_MESSAGE();
        RAISERROR(@ErrorMsg, 16, 1);
    END CATCH
END;
GO

-- Trigger to auto renumber on delete Nha (optional, but enabled for automation)
CREATE TRIGGER [dbo].[trg_AfterDeleteNha_RenumberPhong]
ON [dbo].[Nha]
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @MaNha INT;
    SELECT @MaNha = MaNha FROM deleted;
    
    EXEC [dbo].[sp_XoaNhaVaRenumberPhong] @MaNha = @MaNha;
END;
GO

-- Insert default Admin
INSERT INTO [dbo].[Admin] (TenDangNhap, MatKhau, Email, SoDienThoai)
VALUES (N'admin', N'admin123', N'example@gmail.com', N'0123456789');
GO

