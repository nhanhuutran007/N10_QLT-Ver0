IF DB_ID(N'QLThueNhaV1') IS NULL
BEGIN
	CREATE DATABASE [QLThueNhaV1];
END
GO
USE [QLThueNhaV1]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[MaAdmin] [int] IDENTITY(1,1) NOT NULL,
	[TenDangNhap] [nvarchar](50) NOT NULL,
	[MatKhau] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[SoDienThoai] [nvarchar](15) NULL,
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
/****** Object:  Table [dbo].[BaoTri_SuCo]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BaoTri_SuCo](
	[MaSuCo] [int] IDENTITY(1,1) NOT NULL,
	[MaPhong] [int] NULL,
	[MoTaSuCo] [nvarchar](255) NOT NULL,
	[NgayBaoCao] [date] NULL,
	[TrangThai] [nvarchar](20) NULL,
	[ChiPhi] [decimal](18, 0) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaSuCo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HopDong]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HopDong](
	[MaHopDong] [int] IDENTITY(1,1) NOT NULL,
	[MaNguoiThue] [int] NULL,
	[MaPhong] [int] NULL,
	[NgayBatDau] [date] NOT NULL,
	[NgayKetThuc] [date] NOT NULL,
	[TienCoc] [decimal](18, 0) NULL,
	[FileHopDong] [nvarchar](255) NULL,
	[TrangThai] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaHopDong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NguoiThue]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NguoiThue](
	[MaNguoiThue] [int] IDENTITY(1,1) NOT NULL,
	[HoTen] [nvarchar](100) NOT NULL,
	[SoDienThoai] [nvarchar](15) NOT NULL,
	[CCCD] [nvarchar](20) NULL,
	[NgayBatDau] [date] NULL,
	[TrangThai] [nvarchar](20) NULL,
	[GhiChu] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNguoiThue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[CCCD] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Nha]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Nha](
	[MaNha] [int] IDENTITY(1,1) NOT NULL,
	[DiaChi] [nvarchar](255) NOT NULL,
	[TongSoPhong] [int] NULL,
	[GhiChu] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Phong]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Phong](
	[MaPhong] [int] IDENTITY(1,1) NOT NULL,
	[MaNha] [int] NULL,
	[TenPhong] [nvarchar](50) NOT NULL,
	[DienTich] [decimal](5, 2) NULL,
	[GiaCoBan] [decimal](18, 0) NULL,
	[TrangThai] [nvarchar](20) NULL,
	[GhiChu] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaPhong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaiSanNguoiThue]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaiSanNguoiThue](
	[MaTaiSan] [int] IDENTITY(1,1) NOT NULL,
	[MaNguoiThue] [int] NULL,
	[LoaiTaiSan] [nvarchar](20) NULL,
	[MoTa] [nvarchar](255) NULL,
	[PhiPhuThu] [decimal](18, 0) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaTaiSan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThanhToan]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThanhToan](
	[MaThanhToan] [int] IDENTITY(1,1) NOT NULL,
	[MaHopDong] [int] NULL,
	[ThangNam] [char](7) NOT NULL,
	[TienThue] [decimal](18, 0) NULL,
	[TienDien] [decimal](18, 0) NULL,
	[TienNuoc] [decimal](18, 0) NULL,
	[TienInternet] [decimal](18, 0) NULL,
	[TienVeSinh] [decimal](18, 0) NULL,
	[TienGiuXe] [decimal](18, 0) NULL,
	[ChiPhiKhac] [decimal](18, 0) NULL,
	[TongTien]  AS (((((([TienThue]+[TienDien])+[TienNuoc])+[TienInternet])+[TienVeSinh])+[TienGiuXe])+[ChiPhiKhac]) PERSISTED,
	[TrangThaiThanhToan] [nvarchar](20) NULL,
	[NgayThanhToan] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaThanhToan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BaoTri_SuCo] ADD  DEFAULT (getdate()) FOR [NgayBaoCao]
GO
ALTER TABLE [dbo].[BaoTri_SuCo] ADD  DEFAULT ((0)) FOR [ChiPhi]
GO
ALTER TABLE [dbo].[TaiSanNguoiThue] ADD  DEFAULT ((0)) FOR [PhiPhuThu]
GO
ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT ((0)) FOR [TienThue]
GO
ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT ((0)) FOR [TienDien]
GO
ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT ((0)) FOR [TienNuoc]
GO
ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT ((0)) FOR [TienInternet]
GO
ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT ((0)) FOR [TienVeSinh]
GO
ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT ((0)) FOR [TienGiuXe]
GO
ALTER TABLE [dbo].[ThanhToan] ADD  DEFAULT ((0)) FOR [ChiPhiKhac]
GO
ALTER TABLE [dbo].[BaoTri_SuCo]  WITH CHECK ADD FOREIGN KEY([MaPhong])
REFERENCES [dbo].[Phong] ([MaPhong])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HopDong]  WITH CHECK ADD FOREIGN KEY([MaNguoiThue])
REFERENCES [dbo].[NguoiThue] ([MaNguoiThue])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HopDong]  WITH CHECK ADD FOREIGN KEY([MaPhong])
REFERENCES [dbo].[Phong] ([MaPhong])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Phong]  WITH CHECK ADD FOREIGN KEY([MaNha])
REFERENCES [dbo].[Nha] ([MaNha])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TaiSanNguoiThue]  WITH CHECK ADD FOREIGN KEY([MaNguoiThue])
REFERENCES [dbo].[NguoiThue] ([MaNguoiThue])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ThanhToan]  WITH CHECK ADD FOREIGN KEY([MaHopDong])
REFERENCES [dbo].[HopDong] ([MaHopDong])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BaoTri_SuCo]  WITH CHECK ADD CHECK  (([TrangThai]=N'Hoàn tất' OR [TrangThai]=N'Đang xử lý' OR [TrangThai]=N'Chưa xử lý'))
GO
ALTER TABLE [dbo].[HopDong]  WITH CHECK ADD CHECK  (([TienCoc]>=(0)))
GO
ALTER TABLE [dbo].[HopDong]  WITH CHECK ADD CHECK  (([TrangThai]=N'Hủy' OR [TrangThai]=N'Hết hạn' OR [TrangThai]=N'Hiệu lực'))
GO
ALTER TABLE [dbo].[HopDong]  WITH CHECK ADD  CONSTRAINT [CK_HopDong_Ngay] CHECK  (([NgayKetThuc]>[NgayBatDau]))
GO
ALTER TABLE [dbo].[HopDong] CHECK CONSTRAINT [CK_HopDong_Ngay]
GO
ALTER TABLE [dbo].[NguoiThue]  WITH CHECK ADD CHECK  (([TrangThai]=N'Đã trả phòng' OR [TrangThai]=N'Đang ở'))
GO
ALTER TABLE [dbo].[Nha]  WITH CHECK ADD CHECK  (([TongSoPhong]>=(1) AND [TongSoPhong]<=(10)))
GO
ALTER TABLE [dbo].[Phong]  WITH CHECK ADD CHECK  (([DienTich]>(0)))
GO
ALTER TABLE [dbo].[Phong]  WITH CHECK ADD CHECK  (([GiaCoBan]>=(0)))
GO
ALTER TABLE [dbo].[Phong]  WITH CHECK ADD CHECK  (([TrangThai]=N'Đang thuê' OR [TrangThai]=N'Trống'))
GO
ALTER TABLE [dbo].[TaiSanNguoiThue]  WITH CHECK ADD CHECK  (([LoaiTaiSan]=N'Thú cưng' OR [LoaiTaiSan]=N'Xe'))
GO
ALTER TABLE [dbo].[ThanhToan]  WITH CHECK ADD CHECK  (([TrangThaiThanhToan]=N'Chưa trả' OR [TrangThaiThanhToan]=N'Đã trả'))
GO
/****** Object:  StoredProcedure [dbo].[sp_GhiNhanSuCo]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GhiNhanSuCo]
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
GO
/****** Object:  StoredProcedure [dbo].[sp_KetThucHopDong]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
GO
/****** Object:  StoredProcedure [dbo].[sp_TaoHopDong]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TaoHopDong]
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
GO
/****** Object:  StoredProcedure [dbo].[sp_TaoThongBaoPhi]    Script Date: 10/20/2025 1:28:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_TaoThongBaoPhi]
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
GO

/* =============================================
   Thủ tục khởi tạo dữ liệu mẫu cho CSDL
   - Chỉ chèn khi bảng đang trống để tránh trùng lặp
   - Tuân thủ khóa ngoại và CHECK constraints
   ============================================= */
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_KhoiTaoCSDL_QLThueNhaV1]
AS
BEGIN
	SET NOCOUNT ON;

	-- Admin
	IF NOT EXISTS (SELECT 1 FROM dbo.[Admin])
	BEGIN
		INSERT INTO dbo.[Admin] (TenDangNhap, MatKhau, Email, SoDienThoai)
		VALUES
		(N'admin1', N'123456', N'admin1@example.com', N'0901000001'),
		(N'admin2', N'123456', N'admin2@example.com', N'0901000002'),
		(N'admin3', N'123456', N'admin3@example.com', N'0901000003'),
		(N'admin4', N'123456', N'admin4@example.com', N'0901000004'),
		(N'admin5', N'123456', N'admin5@example.com', N'0901000005');
	END

	-- Nha, Phong, NguoiThue, HopDong, TaiSanNguoiThue, BaoTri_SuCo, ThanhToan
	IF NOT EXISTS (SELECT 1 FROM dbo.Nha)
	BEGIN
		DECLARE @MaNha1 INT, @MaNha2 INT;
		INSERT INTO dbo.Nha (DiaChi, TongSoPhong, GhiChu)
		VALUES (N'123 Đường A, Quận 1, TP.HCM', 5, N'Nhà trung tâm'),
		       (N'456 Đường B, Quận 7, TP.HCM', 6, N'Gần khu công nghệ');

		-- Lấy id theo thứ tự vừa chèn
		SELECT @MaNha2 = MAX(MaNha) FROM dbo.Nha;
		SELECT @MaNha1 = MIN(MaNha) FROM dbo.Nha;

		-- Phong: tạo 6 phòng (4 ở nhà 1, 2 ở nhà 2)
		DECLARE @MaPhong1 INT, @MaPhong2 INT, @MaPhong3 INT, @MaPhong4 INT, @MaPhong5 INT, @MaPhong6 INT;

		INSERT INTO dbo.Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu)
		VALUES (@MaNha1, N'P101', 18.5, 3000000, N'Trống', N'Có cửa sổ');
		SET @MaPhong1 = SCOPE_IDENTITY();

		INSERT INTO dbo.Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu)
		VALUES (@MaNha1, N'P102', 20.0, 3500000, N'Trống', N'Gần cầu thang');
		SET @MaPhong2 = SCOPE_IDENTITY();

		INSERT INTO dbo.Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu)
		VALUES (@MaNha1, N'P201', 22.0, 3800000, N'Trống', N'Ban công nhỏ');
		SET @MaPhong3 = SCOPE_IDENTITY();

		INSERT INTO dbo.Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu)
		VALUES (@MaNha1, N'P202', 24.0, 4000000, N'Trống', N'Ban công lớn');
		SET @MaPhong4 = SCOPE_IDENTITY();

		INSERT INTO dbo.Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu)
		VALUES (@MaNha2, N'A101', 19.0, 3200000, N'Trống', N'Yên tĩnh');
		SET @MaPhong5 = SCOPE_IDENTITY();

		INSERT INTO dbo.Phong (MaNha, TenPhong, DienTich, GiaCoBan, TrangThai, GhiChu)
		VALUES (@MaNha2, N'A201', 23.0, 4200000, N'Trống', N'Sáng sủa');
		SET @MaPhong6 = SCOPE_IDENTITY();

		-- NguoiThue: 5 người
		DECLARE @MaNguoiThue1 INT, @MaNguoiThue2 INT, @MaNguoiThue3 INT, @MaNguoiThue4 INT, @MaNguoiThue5 INT;
		INSERT INTO dbo.NguoiThue (HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu)
		VALUES (N'Nguyễn Văn A', N'0911000001', N'079123456001', GETDATE(), N'Đang ở', NULL);
		SET @MaNguoiThue1 = SCOPE_IDENTITY();
		INSERT INTO dbo.NguoiThue (HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu)
		VALUES (N'Trần Thị B', N'0911000002', N'079123456002', GETDATE(), N'Đang ở', NULL);
		SET @MaNguoiThue2 = SCOPE_IDENTITY();
		INSERT INTO dbo.NguoiThue (HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu)
		VALUES (N'Lê Văn C', N'0911000003', N'079123456003', GETDATE(), N'Đang ở', NULL);
		SET @MaNguoiThue3 = SCOPE_IDENTITY();
		INSERT INTO dbo.NguoiThue (HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu)
		VALUES (N'Phạm Thị D', N'0911000004', N'079123456004', GETDATE(), N'Đã trả phòng', N'Chuyển đi');
		SET @MaNguoiThue4 = SCOPE_IDENTITY();
		INSERT INTO dbo.NguoiThue (HoTen, SoDienThoai, CCCD, NgayBatDau, TrangThai, GhiChu)
		VALUES (N'Huỳnh Văn E', N'0911000005', N'079123456005', GETDATE(), N'Đang ở', NULL);
		SET @MaNguoiThue5 = SCOPE_IDENTITY();

		-- Tạo 3 hợp đồng bằng thủ tục để tự động cập nhật trạng thái phòng
		DECLARE @NgayBD DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
		DECLARE @NgayKT DATE = DATEADD(MONTH, 6, @NgayBD);

		EXEC dbo.sp_TaoHopDong @MaNguoiThue = @MaNguoiThue1, @MaPhong = @MaPhong1, @NgayBatDau = @NgayBD, @NgayKetThuc = @NgayKT, @TienCoc = 1000000, @FileHopDong = N'HD_A.pdf';
		EXEC dbo.sp_TaoHopDong @MaNguoiThue = @MaNguoiThue2, @MaPhong = @MaPhong2, @NgayBatDau = @NgayBD, @NgayKetThuc = @NgayKT, @TienCoc = 1500000, @FileHopDong = N'HD_B.pdf';
		EXEC dbo.sp_TaoHopDong @MaNguoiThue = @MaNguoiThue3, @MaPhong = @MaPhong5, @NgayBatDau = @NgayBD, @NgayKetThuc = @NgayKT, @TienCoc = 1200000, @FileHopDong = N'HD_C.pdf';

		-- Lấy mã hợp đồng tương ứng để tạo thông báo phí
		DECLARE @MaHopDong1 INT, @MaHopDong2 INT, @MaHopDong3 INT;
		SELECT TOP 1 @MaHopDong1 = MaHopDong FROM dbo.HopDong WHERE MaNguoiThue = @MaNguoiThue1 AND MaPhong = @MaPhong1 ORDER BY MaHopDong DESC;
		SELECT TOP 1 @MaHopDong2 = MaHopDong FROM dbo.HopDong WHERE MaNguoiThue = @MaNguoiThue2 AND MaPhong = @MaPhong2 ORDER BY MaHopDong DESC;
		SELECT TOP 1 @MaHopDong3 = MaHopDong FROM dbo.HopDong WHERE MaNguoiThue = @MaNguoiThue3 AND MaPhong = @MaPhong5 ORDER BY MaHopDong DESC;

		-- Tạo thông báo phí cho tháng hiện tại
		DECLARE @ThangNam CHAR(7) = RIGHT('0' + CAST(MONTH(GETDATE()) AS VARCHAR(2)), 2) + '/' + CAST(YEAR(GETDATE()) AS CHAR(4));
		IF @MaHopDong1 IS NOT NULL EXEC dbo.sp_TaoThongBaoPhi @MaHopDong = @MaHopDong1, @ThangNam = @ThangNam;
		IF @MaHopDong2 IS NOT NULL EXEC dbo.sp_TaoThongBaoPhi @MaHopDong = @MaHopDong2, @ThangNam = @ThangNam;
		IF @MaHopDong3 IS NOT NULL EXEC dbo.sp_TaoThongBaoPhi @MaHopDong = @MaHopDong3, @ThangNam = @ThangNam;

		-- Ghi nhận một vài sự cố
		INSERT INTO dbo.BaoTri_SuCo (MaPhong, MoTaSuCo, NgayBaoCao, TrangThai, ChiPhi)
		VALUES (@MaPhong3, N'Hư vòi nước', GETDATE(), N'Chưa xử lý', 0),
		       (@MaPhong4, N'Rò rỉ điện nhẹ', GETDATE(), N'Đang xử lý', 0);

		-- Tài sản người thuê
		INSERT INTO dbo.TaiSanNguoiThue (MaNguoiThue, LoaiTaiSan, MoTa, PhiPhuThu)
		VALUES (@MaNguoiThue1, N'Xe', N'Xe máy Vision', 100000),
		       (@MaNguoiThue2, N'Thú cưng', N'Mèo Anh lông ngắn', 200000),
		       (@MaNguoiThue3, N'Xe', N'Xe máy Sirius', 100000);
	END
END
GO

-- Thực thi thủ tục khởi tạo dữ liệu (idempotent: chỉ thêm khi trống)
EXEC dbo.sp_KhoiTaoCSDL_QLThueNhaV1;
GO
