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
                // Sử dụng overload đơn giản với 2 tham số (findText, replaceText)
                doc.ReplaceText("{TEN_KHACH}", TenB ?? "");
                doc.ReplaceText("{TEN_PHONG}", TenPhong ?? "");
                // TODO: Thêm các tham số NgayBatDau, NgayKetThuc, TienCoc vào method signature
                // doc.ReplaceText("{NGAY_BD}", NgayBatDau.ToString("dd/MM/yyyy"));
                // doc.ReplaceText("{NGAY_KT}", NgayKetThuc.ToString("dd/MM/yyyy"));
                // doc.ReplaceText("{TIEN_COC}", TienCoc.ToString("N0"));
                // thêm các placeholder khác nếu cần

                doc.SaveAs(outputDocx);
            }

            return outputDocx;
        }
    }
}
