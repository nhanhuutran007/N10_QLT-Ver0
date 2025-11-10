// BusinessLayer/Services/ContractTemplateService.cs
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

        private static readonly string OutputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Contracts");


        /// <summary>
        /// Tạo file hợp đồng dựa trên mẫu .docx và trả về đường dẫn file DOCX đã lưu.
        /// File sẽ có timestamp để giữ phiên bản cũ.
        /// </summary>
        public static string CreateContractFile(string tenKhach, string tenPhong, DateTime ngayBD, DateTime ngayKT, decimal tienCoc)
        {
            if (!File.Exists(TemplatePath))
                throw new FileNotFoundException($"Template not found: {TemplatePath}");

            Directory.CreateDirectory(OutputFolder);

            var safeTenKhach = string.Concat(tenKhach.Split(Path.GetInvalidFileNameChars()));
            var outputDocx = Path.Combine(OutputFolder, $"{safeTenKhach}_{DateTime.Now:yyyyMMdd_HHmmss}.docx");

            using (var doc = DocX.Load(TemplatePath))
            {
                // Sử dụng overload đơn giản với 2 tham số (findText, replaceText)
                doc.ReplaceText("{TEN_KHACH}", tenKhach ?? "");
                doc.ReplaceText("{TEN_PHONG}", tenPhong ?? "");
                doc.ReplaceText("{NGAY_BD}", ngayBD.ToString("dd/MM/yyyy"));
                doc.ReplaceText("{NGAY_KT}", ngayKT.ToString("dd/MM/yyyy"));
                doc.ReplaceText("{TIEN_COC}", tienCoc.ToString("N0"));
                // thêm các placeholder khác nếu cần

                doc.SaveAs(outputDocx);
            }

            return outputDocx;
        }
    }
}
