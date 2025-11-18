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
        private readonly IHouseRepository _houseRepository;

        public RentedRoomController(IRentedRoomRepository rentedRoomRepository)
        {
            _rentedRoomRepository = rentedRoomRepository;
            _houseRepository = new HouseRepository();
        }

        public async Task<List<RentedRoomDto>> GetAllRoomsAsync()
        {
            List<DataLayer.Models.RentedRoom> rooms;
            var currentUser = AuthController.CurrentUser;
            if (currentUser != null && currentUser.MaNha > 0)
            {
                rooms = await _rentedRoomRepository.GetAllByMaNhaAsync(currentUser.MaNha);
            }
            else
            {
                rooms = await _rentedRoomRepository.GetAllAsync();
            }
            return rooms.Select(r => new RentedRoomDto
            {
                MaPhong = r.MaPhong,
                MaNha = r.MaNha,
                TenPhong = r.TenPhong,
                DienTich = (double)r.DienTich,
                GiaCoBan = r.GiaCoBan,
                TrangThai = r.TrangThai,
                GhiChu = r.GhiChu, 
                GiaBangChu = r.GiaBangChu,
                TrangThietBi = r.TrangThietBi,
            }).ToList();
        }

        /// <summary>
        /// Kiểm tra xem tài khoản hiện tại có được phép tạo thêm phòng không
        /// Trả về null nếu được phép, hoặc message lỗi nếu không.
        /// </summary>
        public async Task<string?> CheckCanCreateRoomAsync()
        {
            return await ValidateCreateRoomAsync();
        }

        private async Task<string?> ValidateCreateRoomAsync()
        {
            // Chặn tạo phòng nếu tài khoản chưa được gán mã nhà
            if (AuthController.CurrentUser == null || (AuthController.CurrentUser?.MaNha ?? 0) <= 0)
            {
                return "Tài khoản chưa được gán mã nhà";
            }

            var maNha = AuthController.CurrentUser.MaNha;

            // Lấy thông tin nhà để biết tổng số phòng tối đa
            var house = await _houseRepository.GetByIdAsync(maNha);
            if (house != null && house.TongSoPhong > 0)
            {
                var roomsOfHouse = await _rentedRoomRepository.GetAllByMaNhaAsync(maNha);
                var currentCount = roomsOfHouse?.Count ?? 0;

                if (currentCount >= house.TongSoPhong)
                {
                    return $"Nhà hiện tại đã đủ {house.TongSoPhong} phòng, không thể tạo thêm.";
                }
            }

            return null;
        }

        public async Task<string> CreateRoomAsync(RentedRoomDto dto)
        {
            var validationMessage = await ValidateCreateRoomAsync();
            if (validationMessage != null)
            {
                return validationMessage;
            }

            var maNha = AuthController.CurrentUser!.MaNha;

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
                TrangThietBi = dto.TrangThietBi,
                MaNha = maNha
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