using System;
using System.IO;
using Xceed.Words.NET;

namespace QLKDPhongTro.BusinessLayer.Services
{
    public static class ContractTemplateService
    {
        private static readonly string TemplatePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            "Templates",
            "HopDongMau.docx"
        );

        private static readonly string OutputFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Contracts"
        );

        /// <summary>
        /// Tạo file hợp đồng dựa trên mẫu .docx và trả về đường dẫn file DOCX đã lưu.
        /// File sẽ có timestamp để giữ phiên bản cũ.
        /// </summary>
        public static string CreateContractFile(
            // Thông tin chung
            string NoiTaoHD, DateTime NgayTaoHD,

            // Bên A (chủ nhà)
            string TenA, DateTime NgaySinhA, string CCCDA, DateTime NgayCapA, string NoiCapA,
            string DiaChiA, string DienThoaiA,

            // Bên B (người thuê)
            string TenB, DateTime NgaySinhB, string CCCDB, DateTime NgayCapB, string NoiCapB,
            string DiaChiB, string DienThoaiB,

            // Phòng & hợp đồng
            string TenPhong, string DiaChiPhong, decimal DienTich, string TrangThietBi,
            decimal GiaThue, string GiaBangChu, string NgayTraTien, int ThoiHanNam, DateTime NgayGiaoNha
        )
        {
            if (!File.Exists(TemplatePath))
                throw new FileNotFoundException($"Không tìm thấy file mẫu hợp đồng: {TemplatePath}");

            Directory.CreateDirectory(OutputFolder);

            var safeTenKhach = string.Concat(TenB.Split(Path.GetInvalidFileNameChars()));
            var outputDocx = Path.Combine(OutputFolder, $"{safeTenKhach}_{DateTime.Now:yyyyMMdd_HHmmss}.docx");

            using (var doc = DocX.Load(TemplatePath))
            {
                // ====== Thông tin chung ======
                doc.ReplaceText("{NoiTaoHD}", NoiTaoHD ?? "");
                doc.ReplaceText("{Ngay}", NgayTaoHD.Day.ToString());
                doc.ReplaceText("{Thang}", NgayTaoHD.Month.ToString());
                doc.ReplaceText("{Nam}", NgayTaoHD.Year.ToString());

                // ====== Bên A ======
                doc.ReplaceText("{TenA}", TenA ?? "");
                doc.ReplaceText("{NgaySinhA}", NgaySinhA.ToString("dd/MM/yyyy"));
                doc.ReplaceText("{CCCDA}", CCCDA ?? "");
                doc.ReplaceText("{NgayCapA}", NgayCapA.ToString("dd/MM/yyyy"));
                doc.ReplaceText("{NoiCapA}", NoiCapA ?? "");
                doc.ReplaceText("{DiaChiA}", DiaChiA ?? "");
                doc.ReplaceText("{DienThoaiA}", DienThoaiA ?? "");

                // ====== Bên B ======
                doc.ReplaceText("{TenB}", TenB ?? "");
                doc.ReplaceText("{NgaySinhB}", NgaySinhB.ToString("dd/MM/yyyy"));
                doc.ReplaceText("{CCCDB}", CCCDB ?? "");
                doc.ReplaceText("{NgayCapB}", NgayCapB.ToString("dd/MM/yyyy"));
                doc.ReplaceText("{NoiCapB}", NoiCapB ?? "");
                doc.ReplaceText("{DiaChiB}", DiaChiB ?? "");
                doc.ReplaceText("{DienThoaiB}", DienThoaiB ?? "");

                // ====== Thông tin phòng ======
                doc.ReplaceText("{TENPHONG}", TenPhong ?? "");
                doc.ReplaceText("{DIACHIPHONG}", DiaChiPhong ?? "");
                doc.ReplaceText("{DIENTICH}", DienTich.ToString("N2"));
                doc.ReplaceText("{TRANGTHIETBI}", TrangThietBi ?? "");

                // ====== Giá và điều khoản ======
                doc.ReplaceText("{GIATHUE}", GiaThue.ToString("N0"));
                doc.ReplaceText("{GIABANGCHU}", GiaBangChu ?? "");
                doc.ReplaceText("{NGAYTRATIEN}", NgayTraTien ?? "");
                doc.ReplaceText("{THOIHAN}", ThoiHanNam.ToString());
                doc.ReplaceText("{NGAYGIAONHA}", NgayGiaoNha.ToString("dd/MM/yyyy"));

                doc.SaveAs(outputDocx);
            }

            return outputDocx;
        }
    }
}
