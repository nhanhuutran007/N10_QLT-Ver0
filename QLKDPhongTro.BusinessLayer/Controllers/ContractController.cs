using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Models;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QLKDPhongTro.Presentation.Utils;
using System.Windows;


namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class ContractController
    {
        private readonly IContractRepository _repository;


        public ContractController(IContractRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ContractDto>> GetAllHopDongAsync()
        {
            var entities = await _repository.GetAllHopDongAsync();
            return entities.Select(e => new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                FileHopDong = e.FileHopDong,
                TrangThai = e.TrangThai,
                // Bỏ TenNguoiThue và TenPhong vì model Contract không có
                TenNguoiThue = "", // Cần lấy từ repository khác
                TenPhong = "" // Cần lấy từ repository khác
            }).ToList();
        }

        public async Task<ContractDto?> GetByIdAsync(int maHopDong)
        {
            var entity = await _repository.GetByIdAsync(maHopDong);
            if (entity == null) return null;

            return new ContractDto
            {
                MaHopDong = entity.MaHopDong,
                MaNguoiThue = entity.MaNguoiThue,
                MaPhong = entity.MaPhong,
                NgayBatDau = entity.NgayBatDau,
                NgayKetThuc = entity.NgayKetThuc,
                TienCoc = entity.TienCoc,
                FileHopDong = entity.FileHopDong,
                TrangThai = entity.TrangThai,
                TenNguoiThue = "", // Cần lấy từ repository khác
                TenPhong = "" // Cần lấy từ repository khác
            };
        }

        public async Task<List<ContractDto>> GetActiveContractsAsync()
        {
            var entities = await _repository.GetActiveContractsAsync();
            return entities.Select(e => new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                FileHopDong = e.FileHopDong,
                TrangThai = e.TrangThai,
                TenNguoiThue = "", // Cần lấy từ repository khác
                TenPhong = "" // Cần lấy từ repository khác
            }).ToList();
        }

        public async Task CreateHopDongAsync(ContractDto dto)
        {
            var entity = new Contract
            {
                MaNguoiThue = dto.MaNguoiThue,
                MaPhong = dto.MaPhong,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                TienCoc = dto.TienCoc,
                FileHopDong = dto.FileHopDong,
                TrangThai = dto.TrangThai
            };
            await _repository.AddHopDongAsync(entity);
        }

        public async Task UpdateHopDongAsync(ContractDto dto)
        {
            var entity = new Contract
            {
                MaHopDong = dto.MaHopDong,
                MaNguoiThue = dto.MaNguoiThue,
                MaPhong = dto.MaPhong,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                TienCoc = dto.TienCoc,
                FileHopDong = dto.FileHopDong,
                TrangThai = dto.TrangThai
            };
            await _repository.UpdateHopDongAsync(entity);
        }

        public async Task DeleteHopDongAsync(int id)
        {
            await _repository.DeleteHopDongAsync(id);
        }

        public async Task<List<ContractDto>> GetExpiringContractsAsync(int days)
        {
            var entities = await _repository.GetExpiringContractsAsync(days);
            return entities.Select(e => new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                FileHopDong = e.FileHopDong,
                TrangThai = e.TrangThai,
                TenNguoiThue = "", // Cần lấy từ repository khác
                TenPhong = "" // Cần lấy từ repository khác
            }).ToList();
        }
        // 🔹 Gửi email cảnh báo cho hợp đồng sắp hết hạn

        public async Task<bool> SendExpiryWarningEmailsAsync(int days)
        {
            var expiringContracts = await GetExpiringContractsAsync(days);

            if (expiringContracts == null || expiringContracts.Count == 0)
                return false;

            int success = 0, failed = 0;

            foreach (var contract in expiringContracts)
            {
                try
                {
                    await EmailService.SendEmailAsync(
                        "ngochai1521@gmail.com",
                        "Thông báo sắp hết hạn hợp đồng",
                        $"Hợp đồng của bạn sẽ hết hạn trong {days} ngày tới. Vui lòng liên hệ để gia hạn."
                    );
                    success++;
                }
                catch
                {
                    failed++;
                }
            }

            return true;
        }





    }
}