using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class RentedRoomDto
    {
        public int MaPhong { get; set; }

        [Required(ErrorMessage = "Tên phòng không được bỏ trống")]
        public string TenPhong { get; set; } = string.Empty;

        [Range(5, double.MaxValue, ErrorMessage = "Diện tích tối thiểu là 4 m2")]
        public double DienTich { get; set; }

        [Range(500000, double.MaxValue, ErrorMessage = "Giá cơ bản phải lớn hơn 5000000")]
        public double GiaCoBan { get; set; }

        public string TrangThai { get; set; } = string.Empty; // Trống/Đang thuê/Dự kiến
        public string GhiChu { get; set; } = string.Empty;
    }
}