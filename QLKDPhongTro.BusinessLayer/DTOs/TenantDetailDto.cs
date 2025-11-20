using System.Collections.Generic;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class TenantDetailDto
    {
        public TenantDto BasicInfo { get; set; } = new TenantDto();
        public List<TenantAssetDto> Assets { get; set; } = new List<TenantAssetDto>();
        public TenantStayInfoDto? StayInfo { get; set; }
    }
}

