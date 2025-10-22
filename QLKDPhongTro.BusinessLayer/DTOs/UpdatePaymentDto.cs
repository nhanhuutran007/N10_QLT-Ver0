using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class UpdatePaymentDTO
    {
        [Required]
        public int MaThanhToan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền điện không được âm")]
        public decimal? TienDien { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền nước không được âm")]
        public decimal? TienNuoc { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền internet không được âm")]
        public decimal? TienInternet { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền vệ sinh không được âm")]
        public decimal? TienVeSinh { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền giữ xe không được âm")]
        public decimal? TienGiuXe { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chi phí khác không được âm")]
        public decimal? ChiPhiKhac { get; set; }
    }
}