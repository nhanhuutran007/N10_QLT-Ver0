USE githubio_QLT_Ver1;

-- ===================== SỬA CÁC VẤN ĐỀ VỀ DỮ LIỆU =====================

-- VẤN ĐỀ 1: TrangThai của Phong không nhất quán với HopDong
-- Các phòng có hợp đồng "Hiệu lực" phải có TrangThai = 'Đang thuê'
-- Các phòng không có hợp đồng hoặc hợp đồng "Hết hạn"/"Hủy" phải có TrangThai = 'Trống'

-- Cập nhật TrangThai của Phong dựa trên HopDong
UPDATE Phong p
SET p.TrangThai = 'Đang thuê'
WHERE EXISTS (
    SELECT 1 
    FROM HopDong hd 
    WHERE hd.MaPhong = p.MaPhong 
    AND hd.TrangThai = 'Hiệu lực'
);

-- Cập nhật các phòng không có hợp đồng hoặc hợp đồng đã hết hạn/hủy
UPDATE Phong p
SET p.TrangThai = 'Trống'
WHERE NOT EXISTS (
    SELECT 1 
    FROM HopDong hd 
    WHERE hd.MaPhong = p.MaPhong 
    AND hd.TrangThai = 'Hiệu lực'
);

-- VẤN ĐỀ 2: Kiểm tra và sửa các ThanhToan không có HopDong hợp lệ
-- (Nếu có ThanhToan với MaHopDong không tồn tại hoặc HopDong đã bị xóa)
-- Điều này sẽ được xử lý bởi FOREIGN KEY constraint, nhưng cần đảm bảo dữ liệu nhất quán

-- VẤN ĐỀ 3: Đảm bảo các ThanhToan có MaHopDong hợp lệ
-- Kiểm tra xem có ThanhToan nào có MaHopDong không tồn tại không
SELECT 'Kiểm tra ThanhToan có MaHopDong không hợp lệ:' as CheckMessage;
SELECT tt.MaThanhToan, tt.MaHopDong, tt.ThangNam
FROM ThanhToan tt
LEFT JOIN HopDong hd ON tt.MaHopDong = hd.MaHopDong
WHERE tt.MaHopDong IS NOT NULL AND hd.MaHopDong IS NULL;

-- Nếu có ThanhToan với MaHopDong không hợp lệ, có thể xóa hoặc cập nhật
-- (Tạm thời comment để không xóa dữ liệu)
-- DELETE FROM ThanhToan 
-- WHERE MaHopDong IS NOT NULL 
-- AND NOT EXISTS (SELECT 1 FROM HopDong WHERE MaHopDong = ThanhToan.MaHopDong);

-- VẤN ĐỀ 4: Đảm bảo format ThangNam đúng (MM/YYYY)
-- Kiểm tra xem có ThangNam nào không đúng format không
SELECT 'Kiểm tra format ThangNam:' as CheckMessage;
SELECT MaThanhToan, ThangNam
FROM ThanhToan
WHERE ThangNam NOT REGEXP '^[0-9]{2}/[0-9]{4}$';

-- VẤN ĐỀ 5: Đảm bảo các giá trị ENUM đúng
-- Kiểm tra TrangThaiThanhToan
SELECT 'Kiểm tra TrangThaiThanhToan:' as CheckMessage;
SELECT DISTINCT TrangThaiThanhToan
FROM ThanhToan
WHERE TrangThaiThanhToan NOT IN ('Chưa trả', 'Đã trả');

-- VẤN ĐỀ 6: Đảm bảo NgayThanhToan chỉ có khi TrangThaiThanhToan = 'Đã trả'
-- Cập nhật các bản ghi không nhất quán
UPDATE ThanhToan
SET NgayThanhToan = NULL
WHERE TrangThaiThanhToan = 'Chưa trả' AND NgayThanhToan IS NOT NULL;

-- VẤN ĐỀ 7: Kiểm tra tính nhất quán của TongTien (generated column)
-- TongTien là generated column nên sẽ tự động tính, nhưng cần đảm bảo các giá trị thành phần không NULL
-- (Các giá trị NULL sẽ được xử lý bởi COALESCE trong generated column)

-- VẤN ĐỀ 8: Đảm bảo các HopDong có MaPhong hợp lệ
SELECT 'Kiểm tra HopDong có MaPhong không hợp lệ:' as CheckMessage;
SELECT hd.MaHopDong, hd.MaPhong, hd.TrangThai
FROM HopDong hd
LEFT JOIN Phong p ON hd.MaPhong = p.MaPhong
WHERE p.MaPhong IS NULL;

-- VẤN ĐỀ 9: Đảm bảo các HopDong có MaNguoiThue hợp lệ
SELECT 'Kiểm tra HopDong có MaNguoiThue không hợp lệ:' as CheckMessage;
SELECT hd.MaHopDong, hd.MaNguoiThue, hd.TrangThai
FROM HopDong hd
LEFT JOIN NguoiThue nt ON hd.MaNguoiThue = nt.MaNguoiThue
WHERE nt.MaNguoiThue IS NULL;

-- ===================== TÓM TẮT CÁC VẤN ĐỀ ĐÃ SỬA =====================
-- 1. ✅ Đã cập nhật TrangThai của Phong dựa trên HopDong
-- 2. ✅ Đã kiểm tra ThanhToan có MaHopDong hợp lệ
-- 3. ✅ Đã kiểm tra format ThangNam
-- 4. ✅ Đã kiểm tra TrangThaiThanhToan
-- 5. ✅ Đã cập nhật NgayThanhToan cho nhất quán
-- 6. ✅ Đã kiểm tra HopDong có MaPhong và MaNguoiThue hợp lệ

-- ===================== KẾT QUẢ KIỂM TRA =====================
SELECT 'Tổng số Phong:' as Summary, COUNT(*) as Count FROM Phong
UNION ALL
SELECT 'Phong Đang thuê:', COUNT(*) FROM Phong WHERE TrangThai = 'Đang thuê'
UNION ALL
SELECT 'Phong Trống:', COUNT(*) FROM Phong WHERE TrangThai = 'Trống'
UNION ALL
SELECT 'Tổng số HopDong:', COUNT(*) FROM HopDong
UNION ALL
SELECT 'HopDong Hiệu lực:', COUNT(*) FROM HopDong WHERE TrangThai = 'Hiệu lực'
UNION ALL
SELECT 'Tổng số ThanhToan:', COUNT(*) FROM ThanhToan
UNION ALL
SELECT 'ThanhToan Đã trả:', COUNT(*) FROM ThanhToan WHERE TrangThaiThanhToan = 'Đã trả'
UNION ALL
SELECT 'ThanhToan Chưa trả:', COUNT(*) FROM ThanhToan WHERE TrangThaiThanhToan = 'Chưa trả';

