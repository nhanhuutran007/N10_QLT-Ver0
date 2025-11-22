using System.Collections.Generic;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    /// <summary>
    /// Kết quả xóa người thuê, bao gồm thông tin về việc có cần tạo hợp đồng mới không
    /// </summary>
    public class DeleteTenantResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Có cần tạo hợp đồng mới không (khi người bị xóa là người đứng tên hợp đồng và phòng còn người thuê)
        /// </summary>
        public bool RequiresNewContract { get; set; }
        
        /// <summary>
        /// Mã phòng của người thuê bị xóa
        /// </summary>
        public int? MaPhong { get; set; }
        
        /// <summary>
        /// Danh sách người thuê còn lại trong phòng (nếu cần tạo hợp đồng mới)
        /// </summary>
        public List<RoomTenantDto> RemainingTenants { get; set; } = new List<RoomTenantDto>();
        
        /// <summary>
        /// Thông tin hợp đồng cũ (nếu có)
        /// </summary>
        public ContractDto? OldContract { get; set; }
    }
}

