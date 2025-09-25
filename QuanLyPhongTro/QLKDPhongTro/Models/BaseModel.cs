using System;

namespace QLKDPhongTro.Models
{
    /// <summary>
    /// Lớp cơ sở cho tất cả các Model
    /// </summary>
    public abstract class BaseModel
    {
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
