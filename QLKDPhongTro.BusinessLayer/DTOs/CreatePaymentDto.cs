using System.ComponentModel.DataAnnotations;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Mã hợp đồng là bắt buộc")]
        public int MaHopDong { get; set; }

        [Required(ErrorMessage = "Tháng năm là bắt buộc")]
        [RegularExpression(@"^(0[1-9]|1[0-2])/\d{4}$", ErrorMessage = "Tháng năm phải có định dạng MM/yyyy")]
        public string ThangNam { get; set; }

        [Required(ErrorMessage = "Tiền thuê là bắt buộc")]
        [Range(1, double.MaxValue, ErrorMessage = "Tiền thuê phải lớn hơn 0")]
        public decimal TienThue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền điện không được âm")]
        public decimal TienDien { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền nước không được âm")]
        public decimal TienNuoc { get; set; }

        // Chỉ số điện nước - mặc định là 0
        [Range(0, double.MaxValue, ErrorMessage = "Chỉ số điện không được âm")]
        public decimal SoDien { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Chỉ số nước không được âm")]
        public decimal SoNuoc { get; set; } = 0;

        // Đơn giá điện nước - mặc định là 0
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá điện không được âm")]
        public decimal DonGiaDien { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá nước không được âm")]
        public decimal DonGiaNuoc { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Tiền internet không được âm")]
        public decimal TienInternet { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền vệ sinh không được âm")]
        public decimal TienVeSinh { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền giữ xe không được âm")]
        public decimal TienGiuXe { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chi phí khác không được âm")]
        public decimal ChiPhiKhac { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chỉ số điện tháng trước không được âm")]
        public decimal SoDienThangTruoc { get; set; } = 0; // Thêm trường này
    }
}