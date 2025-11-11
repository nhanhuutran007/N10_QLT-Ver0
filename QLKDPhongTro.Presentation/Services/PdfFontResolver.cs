using System;
using System.IO;
using System.Text;
using PdfSharp.Fonts;

namespace QLKDPhongTro.Presentation.Services
{
    /// <summary>
    /// Font resolver cho PdfSharp để sử dụng system fonts từ Windows
    /// </summary>
    public class PdfFontResolver : IFontResolver
    {
        private static readonly string FontsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Tạo font key dựa trên tên font, bold và italic
            var fontKey = $"{familyName}_{isBold}_{isItalic}";
            return new FontResolverInfo(fontKey);
        }

        public byte[] GetFont(string faceName)
        {
            try
            {
                // Parse font key: "Times New Roman_True_False" hoặc "Times New Roman_False_False"
                // faceName có thể là format khác, cần xử lý linh hoạt
                bool isBold = false;
                bool isItalic = false;
                string familyName = faceName;

                // Tìm dấu _ đầu tiên để split
                var lastUnderscoreIndex = faceName.LastIndexOf('_');
                if (lastUnderscoreIndex > 0)
                {
                    var secondLastUnderscoreIndex = faceName.LastIndexOf('_', lastUnderscoreIndex - 1);
                    if (secondLastUnderscoreIndex > 0)
                    {
                        familyName = faceName.Substring(0, secondLastUnderscoreIndex);
                        var boldStr = faceName.Substring(secondLastUnderscoreIndex + 1, lastUnderscoreIndex - secondLastUnderscoreIndex - 1);
                        var italicStr = faceName.Substring(lastUnderscoreIndex + 1);
                        
                        bool.TryParse(boldStr, out isBold);
                        bool.TryParse(italicStr, out isItalic);
                    }
                }

                // Tìm file font Times New Roman
                string fontFileName = null;
                if (familyName.Equals("Times New Roman", StringComparison.OrdinalIgnoreCase) ||
                    familyName.Contains("Times") || familyName.Contains("Roman"))
                {
                    if (isBold && isItalic)
                        fontFileName = "timesbi.ttf";
                    else if (isBold)
                        fontFileName = "timesbd.ttf";
                    else if (isItalic)
                        fontFileName = "timesi.ttf";
                    else
                        fontFileName = "times.ttf";
                }
                else if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
                {
                    if (isBold && isItalic)
                        fontFileName = "arialbi.ttf";
                    else if (isBold)
                        fontFileName = "arialbd.ttf";
                    else if (isItalic)
                        fontFileName = "ariali.ttf";
                    else
                        fontFileName = "arial.ttf";
                }
                else
                {
                    // Fallback về Times New Roman
                    fontFileName = "times.ttf";
                }

                // Đọc font file (thử cả lowercase và uppercase)
                string fontPath = Path.Combine(FontsPath, fontFileName);
                if (File.Exists(fontPath))
                {
                    return File.ReadAllBytes(fontPath);
                }

                // Thử uppercase
                fontPath = Path.Combine(FontsPath, fontFileName.ToUpper());
                if (File.Exists(fontPath))
                {
                    return File.ReadAllBytes(fontPath);
                }

                // Nếu không tìm thấy, thử các biến thể khác của Times New Roman
                var alternativeFonts = new[] { "times.ttf", "TIMES.TTF", "timesi.ttf", "TIMESI.TTF" };
                foreach (var altFont in alternativeFonts)
                {
                    var altPath = Path.Combine(FontsPath, altFont);
                    if (File.Exists(altPath))
                    {
                        return File.ReadAllBytes(altPath);
                    }
                }
            }
            catch (Exception)
            {
                // Nếu có lỗi, trả về null để PdfSharp dùng font mặc định
            }

            return null;
        }
    }
}

