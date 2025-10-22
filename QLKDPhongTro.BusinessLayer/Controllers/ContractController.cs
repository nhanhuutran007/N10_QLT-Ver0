using QLKDPhongTro.BusinessLayer.DTOs;
using QLKDPhongTro.DataLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QLKDPhongTro.BusinessLayer.Controllers
{
    public class ContractController
    {
        private readonly ContractRepository _repository;

        public ContractController(ContractRepository repository)
        {
            _repository = repository;
        }

        public List<ContractDto> GetAllHopDong()
        {
            var entities = _repository.GetAllHopDong();
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
                TenNguoiThue = e.TenNguoiThue,
                TenPhong = e.TenPhong
            }).ToList();
        }

        public void CreateHopDong(ContractDto dto)
        {
            var entity = new HopDong
            {
                MaNguoiThue = dto.MaNguoiThue,
                MaPhong = dto.MaPhong,
                NgayBatDau = dto.NgayBatDau,
                NgayKetThuc = dto.NgayKetThuc,
                TienCoc = dto.TienCoc,
                FileHopDong = dto.FileHopDong,
                TrangThai = dto.TrangThai
            };
            _repository.AddHopDong(entity);
        }

        public void UpdateHopDong(ContractDto dto)
        {
            var entity = new HopDong
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
            _repository.UpdateHopDong(entity);
        }

        public void DeleteHopDong(int id)
        {
            _repository.DeleteHopDong(id);
        }

        public List<ContractDto> GetExpiringContracts(int days)
        {
            var entities = _repository.GetExpiringContracts(days);
            return entities.Select(e => new ContractDto
            {
                MaHopDong = e.MaHopDong,
                MaNguoiThue = e.MaNguoiThue,
                MaPhong = e.MaPhong,
                NgayBatDau = e.NgayBatDau,
                NgayKetThuc = e.NgayKetThuc,
                TienCoc = e.TienCoc,
                TrangThai = e.TrangThai,
                TenNguoiThue = e.TenNguoiThue,
                TenPhong = e.TenPhong
            }).ToList();
        }
    }
}
