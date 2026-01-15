using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class RentedRoomDto
    {
        public int MaPhong { get; set; } // Chỉ bắt buộc khi cập nhật
        public int MaNha { get; set; }

        [Required(ErrorMessage = "Tên phòng không được bỏ trống")]
        public string TenPhong { get; set; } = string.Empty;

        [Range(5, double.MaxValue, ErrorMessage = "Diện tích tối thiểu là 5 m2")]
        public double DienTich { get; set; }

        [Range(typeof(decimal), "500000", "79228162514264337593543950335", ErrorMessage = "Giá cơ bản phải lớn hơn 500000")]
        public decimal GiaCoBan { get; set; }

        public string TrangThai { get; set; } = string.Empty;

        public string GhiChu { get; set; } = string.Empty;

        public string GiaBangChu { get; set; } = string.Empty;
        public string TrangThietBi { get; set; } = string.Empty;
    }
}