-- Script tạo database và bảng cho hệ thống quản lý phòng trọ
-- Tạo database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'QLKDPhongTro')
BEGIN
    CREATE DATABASE QLKDPhongTro;
END
GO

USE QLKDPhongTro;
GO

-- Tạo bảng tblDangNhap (Bảng đăng nhập)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblDangNhap]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblDangNhap](
        [MaDangNhap] [nvarchar](50) NOT NULL,
        [TenDangNhap] [nvarchar](100) NOT NULL,
        [MatKhau] [nvarchar](255) NOT NULL,
        [Email] [nvarchar](100) NOT NULL,
        [NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
        [TrangThai] [bit] NOT NULL DEFAULT 1,
        CONSTRAINT [PK_tblDangNhap] PRIMARY KEY CLUSTERED ([MaDangNhap] ASC)
    );
END
GO

-- Tạo bảng tblPhongTro
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblPhongTro]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblPhongTro](
        [MaPhong] [nvarchar](20) NOT NULL,
        [TenPhong] [nvarchar](50) NOT NULL,
        [DienTich] [float] NOT NULL,
        [GiaThue] [decimal](18, 0) NOT NULL,
        [TrangThai] [nvarchar](20) NOT NULL DEFAULT 'Trống',
        [MoTa] [nvarchar](500) NULL,
        [NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_tblPhongTro] PRIMARY KEY CLUSTERED ([MaPhong] ASC)
    );
END
GO

-- Tạo bảng tblKhachThue
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblKhachThue]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblKhachThue](
        [MaKhach] [nvarchar](20) NOT NULL,
        [HoTen] [nvarchar](100) NOT NULL,
        [CCCD] [nvarchar](20) NOT NULL,
        [SDT] [nvarchar](15) NOT NULL,
        [DiaChi] [nvarchar](200) NULL,
        [NgaySinh] [date] NULL,
        [GioiTinh] [nvarchar](10) NULL,
        [NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_tblKhachThue] PRIMARY KEY CLUSTERED ([MaKhach] ASC)
    );
END
GO

-- Tạo bảng tblHopDong
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblHopDong]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblHopDong](
        [MaHopDong] [nvarchar](20) NOT NULL,
        [MaKhach] [nvarchar](20) NOT NULL,
        [MaPhong] [nvarchar](20) NOT NULL,
        [NgayBatDau] [date] NOT NULL,
        [NgayKetThuc] [date] NOT NULL,
        [GiaThue] [decimal](18, 0) NOT NULL,
        [TienCoc] [decimal](18, 0) NOT NULL,
        [TrangThai] [nvarchar](20) NOT NULL DEFAULT 'Hoạt động',
        [NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_tblHopDong] PRIMARY KEY CLUSTERED ([MaHopDong] ASC),
        CONSTRAINT [FK_tblHopDong_tblKhachThue] FOREIGN KEY([MaKhach]) REFERENCES [dbo].[tblKhachThue] ([MaKhach]),
        CONSTRAINT [FK_tblHopDong_tblPhongTro] FOREIGN KEY([MaPhong]) REFERENCES [dbo].[tblPhongTro] ([MaPhong])
    );
END
GO

-- Tạo bảng tblDienNuoc
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblDienNuoc]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblDienNuoc](
        [MaDienNuoc] [nvarchar](20) NOT NULL,
        [MaPhong] [nvarchar](20) NOT NULL,
        [Thang] [int] NOT NULL,
        [Nam] [int] NOT NULL,
        [SoDienCu] [int] NOT NULL,
        [SoDienMoi] [int] NOT NULL,
        [SoNuocCu] [int] NOT NULL,
        [SoNuocMoi] [int] NOT NULL,
        [GiaDien] [decimal](18, 0) NOT NULL,
        [GiaNuoc] [decimal](18, 0) NOT NULL,
        [TongTien] [decimal](18, 0) NOT NULL,
        [TrangThai] [nvarchar](20) NOT NULL DEFAULT 'Chưa thanh toán',
        [NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_tblDienNuoc] PRIMARY KEY CLUSTERED ([MaDienNuoc] ASC),
        CONSTRAINT [FK_tblDienNuoc_tblPhongTro] FOREIGN KEY([MaPhong]) REFERENCES [dbo].[tblPhongTro] ([MaPhong])
    );
END
GO

-- Tạo bảng tblHoaDon
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblHoaDon]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblHoaDon](
        [MaHoaDon] [nvarchar](20) NOT NULL,
        [MaKhach] [nvarchar](20) NOT NULL,
        [MaPhong] [nvarchar](20) NOT NULL,
        [Thang] [int] NOT NULL,
        [Nam] [int] NOT NULL,
        [TongTien] [decimal](18, 0) NOT NULL,
        [TrangThai] [nvarchar](20) NOT NULL DEFAULT 'Chưa thanh toán',
        [NgayTao] [datetime] NOT NULL DEFAULT GETDATE(),
        [NgayThanhToan] [datetime] NULL,
        CONSTRAINT [PK_tblHoaDon] PRIMARY KEY CLUSTERED ([MaHoaDon] ASC),
        CONSTRAINT [FK_tblHoaDon_tblKhachThue] FOREIGN KEY([MaKhach]) REFERENCES [dbo].[tblKhachThue] ([MaKhach]),
        CONSTRAINT [FK_tblHoaDon_tblPhongTro] FOREIGN KEY([MaPhong]) REFERENCES [dbo].[tblPhongTro] ([MaPhong])
    );
END
GO

-- Tạo bảng tblChiTietHoaDon
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblChiTietHoaDon]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblChiTietHoaDon](
        [MaChiTiet] [nvarchar](20) NOT NULL,
        [MaHoaDon] [nvarchar](20) NOT NULL,
        [LoaiChiPhi] [nvarchar](50) NOT NULL,
        [SoLuong] [int] NOT NULL,
        [DonGia] [decimal](18, 0) NOT NULL,
        [ThanhTien] [decimal](18, 0) NOT NULL,
        [MoTa] [nvarchar](200) NULL,
        CONSTRAINT [PK_tblChiTietHoaDon] PRIMARY KEY CLUSTERED ([MaChiTiet] ASC),
        CONSTRAINT [FK_tblChiTietHoaDon_tblHoaDon] FOREIGN KEY([MaHoaDon]) REFERENCES [dbo].[tblHoaDon] ([MaHoaDon])
    );
END
GO

-- Tạo Index để tối ưu hiệu suất
CREATE NONCLUSTERED INDEX [IX_tblDangNhap_TenDangNhap] ON [dbo].[tblDangNhap] ([TenDangNhap]);
CREATE NONCLUSTERED INDEX [IX_tblDangNhap_Email] ON [dbo].[tblDangNhap] ([Email]);
CREATE NONCLUSTERED INDEX [IX_tblPhongTro_TrangThai] ON [dbo].[tblPhongTro] ([TrangThai]);
CREATE NONCLUSTERED INDEX [IX_tblHopDong_MaKhach] ON [dbo].[tblHopDong] ([MaKhach]);
CREATE NONCLUSTERED INDEX [IX_tblHopDong_MaPhong] ON [dbo].[tblHopDong] ([MaPhong]);

-- Thêm dữ liệu mẫu cho bảng tblDangNhap
INSERT INTO [dbo].[tblDangNhap] ([MaDangNhap], [TenDangNhap], [MatKhau], [Email], [NgayTao], [TrangThai])
VALUES 
('ADMIN001', 'admin', 'admin123', 'admin@example.com', GETDATE(), 1),
('USER001', 'user1', 'user123', 'user1@example.com', GETDATE(), 1);

PRINT 'Database và các bảng đã được tạo thành công!';

