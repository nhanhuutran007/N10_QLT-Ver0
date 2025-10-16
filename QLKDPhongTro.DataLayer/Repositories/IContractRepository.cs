// QLKDPhongTro.DataLayer/Repositories/IHopDongRepository.cs
using QLKDPhongTro.DataLayer.Models;
using System.Collections.Generic;

namespace QLKDPhongTro.DataLayer.Repositories
{
    public interface IContractRepository
    {
        List<HopDong> GetAllHopDong();
        void AddHopDong(HopDong hopDong);
        void UpdateHopDong(HopDong hopDong);
        void DeleteHopDong(int maHopDong);
        List<HopDong> GetExpiringContracts(int days); // Lấy hợp đồng sắp hết hạn
    }
}