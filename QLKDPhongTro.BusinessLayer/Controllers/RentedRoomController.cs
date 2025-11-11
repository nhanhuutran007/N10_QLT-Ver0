using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class RentedRoomController
    {
        private readonly IRentedRoomRepository _rentedRoomRepository;

        public RentedRoomController(IRentedRoomRepository rentedRoomRepository)
        {
            _rentedRoomRepository = rentedRoomRepository;
        }

        public async Task<List<RentedRoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _rentedRoomRepository.GetAllAsync();
            return rooms.Select(r => new RentedRoomDto
            {
                MaPhong = r.MaPhong,
                TenPhong = r.TenPhong,
                DienTich = (double)r.DienTich,
                GiaCoBan = r.GiaCoBan,
                TrangThai = r.TrangThai,
                GhiChu = r.GhiChu, 
                GiaBangChu = r.GiaBangChu,
                TrangThietBi = r.TrangThietBi,
            }).ToList();
        }

        public async Task<string> CreateRoomAsync(RentedRoomDto dto)
        {
            // Không kiểm tra mã phòng trùng nữa vì database tự động tạo
            var room = new RentedRoom
            {
                // Không set MaPhong - để database tự động tăng
                TenPhong = dto.TenPhong,
                DienTich = (decimal)dto.DienTich,
                GiaCoBan = dto.GiaCoBan,
                TrangThai = dto.TrangThai,
                GhiChu = dto.GhiChu,
                GiaBangChu = dto.GiaBangChu,
                TrangThietBi = dto.TrangThietBi
            };

            var result = await _rentedRoomRepository.CreateAsync(room);
            return result ? "Thêm phòng thành công!" : "Có lỗi xảy ra khi thêm phòng!";
        }

        public async Task<bool> UpdateRoomAsync(RentedRoomDto dto)
        {
            var room = new RentedRoom
            {
                MaPhong = dto.MaPhong, // Giữ mã phòng khi cập nhật
                TenPhong = dto.TenPhong,
                DienTich = (decimal)dto.DienTich,
                GiaCoBan = dto.GiaCoBan,
                TrangThai = dto.TrangThai,
                GhiChu = dto.GhiChu,
                GiaBangChu = dto.GiaBangChu,
                TrangThietBi = dto.TrangThietBi
            };
            return await _rentedRoomRepository.UpdateAsync(room);
        }
        public async Task<bool> DeleteRoomAsync(int id)
        {
            return await _rentedRoomRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateRoomStatusAsync(int id, string status)
        {
            return await _rentedRoomRepository.UpdateStatusAsync(id, status);
        }
    }
}