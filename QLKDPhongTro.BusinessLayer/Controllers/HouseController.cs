// Controllers/HouseController.cs
using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class HouseController
    {
        private readonly IHouseRepository _houseRepository;

        public HouseController(IHouseRepository houseRepository)
        {
            _houseRepository = houseRepository;
        }

        public async Task<List<HouseDto>> GetAllHousesAsync()
        {
            var houses = await _houseRepository.GetAllAsync();
            return houses.Select(h => new HouseDto
            {
                MaNha = h.MaNha,
                DiaChi = h.DiaChi,
                TinhThanh = h.TinhThanh,
                TongSoPhong = h.TongSoPhong,
                GhiChu = h.GhiChu
            }).ToList();
        }

        public async Task<bool> CreateHouseAsync(HouseDto dto)
        {
            var house = new House
            {
                MaNha = dto.MaNha,
                DiaChi = dto.DiaChi,
                TinhThanh = dto.TinhThanh,
                TongSoPhong = dto.TongSoPhong,
                GhiChu = dto.GhiChu
            };
            return await _houseRepository.CreateAsync(house);
        }

        public async Task<bool> UpdateHouseAsync(HouseDto dto)
        {
            var house = new House
            {
                MaNha = dto.MaNha,
                DiaChi = dto.DiaChi,
                TinhThanh = dto.TinhThanh,
                TongSoPhong = dto.TongSoPhong,
                GhiChu = dto.GhiChu
            };
            return await _houseRepository.UpdateAsync(house);
        }

        public async Task<bool> DeleteHouseAsync(int id)
        {
            return await _houseRepository.DeleteAsync(id);
        }
    }
}
