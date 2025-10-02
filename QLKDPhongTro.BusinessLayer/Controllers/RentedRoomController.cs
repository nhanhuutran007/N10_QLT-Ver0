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
            return rooms.Select(r =>
            {
                var dto = new RentedRoomDto
                {
                    MaPhong = r.MaPhong,
                    TenPhong = r.TenPhong,
                    DienTich = r.DienTich,
                    GiaCoBan = r.GiaCoBan,
                    TrangThai = r.TrangThai,
                    GhiChu = r.GhiChu
                };
                // Lấy SoGiuong từ GhiChu nếu có
                if (!string.IsNullOrEmpty(r.GhiChu) && r.GhiChu.StartsWith("Số giường:"))
                {
                    if (int.TryParse(r.GhiChu.Replace("Số giường:", "").Trim(), out int soGiuong))
                        dto.SoGiuong = soGiuong;
                }
                return dto;
            }).ToList();
        }

        public async Task<string> CreateRoomAsync(RentedRoomDto dto)
        {
            // Kiểm tra trùng mã phòng
            var existingRoom = await _rentedRoomRepository.GetByIdAsync(dto.MaPhong);
            if (existingRoom != null)
                return "Mã phòng đã tồn tại!";

            var room = new RentedRoom
            {
                MaPhong = dto.MaPhong,
                TenPhong = dto.TenPhong,
                DienTich = dto.DienTich,
                GiaCoBan = dto.GiaCoBan,
                TrangThai = dto.TrangThai,
                GhiChu = $"Số giường: {dto.SoGiuong}" // Lưu SoGiuong vào GhiChu
            };
            var result = await _rentedRoomRepository.CreateAsync(room);
            return result ? "Thêm phòng thành công!" : "Có lỗi xảy ra khi thêm phòng!";
        }

        public async Task<bool> UpdateRoomAsync(RentedRoomDto dto)
        {
            var room = new RentedRoom
            {
                MaPhong = dto.MaPhong,
                TenPhong = dto.TenPhong,
                DienTich = dto.DienTich,
                GiaCoBan = dto.GiaCoBan,
                TrangThai = dto.TrangThai,
                GhiChu = $"Số giường: {dto.SoGiuong}" // Lưu SoGiuong vào GhiChu
            };
            return await _rentedRoomRepository.UpdateAsync(room);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            return await _rentedRoomRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateRoomStatusAsync(int id, string status)
        {
            var room = await _rentedRoomRepository.GetByIdAsync(id);
            if (room == null) return false;

            room.TrangThai = status;
            return await _rentedRoomRepository.UpdateAsync(room);
        }
    }
}