using System;
using System.Diagnostics;
using System.IO;
using Xceed.Words.NET;
using Spire.Doc;
using Spire.Doc.Documents;

namespace QLKDPhongTro.BusinessLayer.Services
{
    public sealed record ContractFileResult(string DocxPath, string? PdfPath);

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
        /// Tạo file hợp đồng dựa trên mẫu .docx và tự động xuất thêm bản PDF.
        /// Trả về đường dẫn file Word và PDF (nếu chuyển đổi thành công).
        /// </summary>
        public static ContractFileResult CreateContractFile(
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

            var pdfPath = TryConvertDocxToPdf(outputDocx);
            return new ContractFileResult(outputDocx, pdfPath);
        }

        private static string? TryConvertDocxToPdf(string docxPath)
        {
            // Ưu tiên dùng LibreOffice (miễn phí) để bỏ giới hạn 3 trang.
            if (TryConvertWithLibreOffice(docxPath, out var pdfPath))
            {
                return pdfPath;
            }

            // Fallback FreeSpire (sẽ bị giới hạn 3 trang) để tránh lỗi hoàn toàn.
            try
            {
                var pdfOutputPath = Path.ChangeExtension(docxPath, ".pdf");
                using var document = new Document();
                document.LoadFromFile(docxPath);
                document.SaveToFile(pdfOutputPath, FileFormat.PDF);
                return pdfOutputPath;
            }
            catch
            {
                return null;
            }
        }

        private static bool TryConvertWithLibreOffice(string docxPath, out string? pdfPath)
        {
            pdfPath = null;
            var sofficePath = ResolveLibreOfficePath();
            if (sofficePath is null || !File.Exists(sofficePath))
            {
                return false;
            }

            try
            {
                var outputDir = Path.GetDirectoryName(docxPath)!;
                var processInfo = new ProcessStartInfo
                {
                    FileName = sofficePath,
                    Arguments = $"--headless --convert-to pdf --outdir \"{outputDir}\" \"{docxPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(processInfo);
                if (process is null)
                    return false;

                if (!process.WaitForExit(60000) || process.ExitCode != 0)
                    return false;

                var candidatePath = Path.ChangeExtension(docxPath, ".pdf");
                if (File.Exists(candidatePath))
                {
                    pdfPath = candidatePath;
                    return true;
                }
            }
            catch
            {
                // Ignore and allow fallback
            }

            return false;
        }

        private static string? ResolveLibreOfficePath()
        {
            var envPath = Environment.GetEnvironmentVariable("LIBREOFFICE_PATH");
            if (!string.IsNullOrWhiteSpace(envPath) && File.Exists(envPath))
            {
                return envPath;
            }

            var candidates = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "LibreOffice", "program", "soffice.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "LibreOffice", "program", "soffice.exe"),
                @"C:\Program Files\LibreOffice\program\soffice.exe",
                @"C:\Program Files (x86)\LibreOffice\program\soffice.exe"
            };

            foreach (var candidate in candidates)
            {
                if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
                    return candidate;
            }

            return null;
        }
    }
}
