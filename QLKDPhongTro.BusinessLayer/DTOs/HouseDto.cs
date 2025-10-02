using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class HouseDto
    {
        public int MaNha { get; set; }

        [Required(ErrorMessage ="Địa chỉ không được bỏ trống")]
        public string DiaChi { get; set; } = string.Empty;

        [Range(1,int.MaxValue, ErrorMessage = "Tổng số phòng phải lớn hơn 0")]
        public int TongSoPhong { get; set; }

        public string GhiChu { get; set; } = string.Empty;

    }
}