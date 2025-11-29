namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// Thiết lập các khoản phí cố định sử dụng khi tạo công nợ từ Google Form.
    /// </summary>
    public class DebtFeeSettings
    {
        public decimal ElectricityFee { get; set; } = 3_500m;
        public decimal WaterFee { get; set; } = 100_000m;
        public decimal InternetFee { get; set; } = 100_000m;
        public decimal SanitationFee { get; set; } = 60_000m;

        /// <summary>
        /// Phí thu thêm cho mỗi phương tiện vượt quá số lượng miễn phí.
        /// </summary>
        public decimal AdditionalVehicleFee { get; set; } = 100_000m;

        /// <summary>
        /// Số lượng phương tiện được miễn phí (ví dụ 1 xe đầu tiên miễn phí).
        /// </summary>
        public int FreeVehicleCount { get; set; } = 1;
    }
}

