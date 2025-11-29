using QLKDPhongTro.BusinessLayer.DTOs; // Các DTO tài chính khác
using QLKDPhongTro.Presentation.Commands;
using QLKDPhongTro.BusinessLayer.Services; // Chứa MeterReadingResult
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class MeterReadingInspectionViewModel : INotifyPropertyChanged
    {
        public bool IsConfirmed { get; private set; } = false;

        // Ảnh hiển thị (đã vẽ bounding box)
        private BitmapImage? _visualizedImage;
        public BitmapImage? VisualizedImage
        {
            get => _visualizedImage;
            set { _visualizedImage = value; OnPropertyChanged(); }
        }

        // Chuỗi thô từ AI (VD: "007590")
        private string _rawText = string.Empty;
        public string RawText
        {
            get => _rawText;
            set { _rawText = value; OnPropertyChanged(); }
        }

        // Giá trị cuối cùng để lưu (VD: 759)
        private decimal _finalValue;
        public decimal FinalValue
        {
            get => _finalValue;
            set { _finalValue = value; OnPropertyChanged(); }
        }

        private string _confidenceInfo = string.Empty;
        public string ConfidenceInfo
        {
            get => _confidenceInfo;
            set { _confidenceInfo = value; OnPropertyChanged(); }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? RequestClose;

        public MeterReadingInspectionViewModel(MeterReadingResult result, string? originalImagePath = null)
        {
            // 1. Hiển thị ảnh: Ưu tiên ảnh đã vẽ khung (Visualize), nếu không có thì dùng ảnh gốc
            if (!string.IsNullOrEmpty(result.VisualizedImageBase64))
            {
                VisualizedImage = Base64ToBitmapImage(result.VisualizedImageBase64);
            }
            else if (!string.IsNullOrEmpty(originalImagePath) && File.Exists(originalImagePath))
            {
                VisualizedImage = new BitmapImage(new Uri(originalImagePath));
            }

            // 2. Hiển thị thông tin thô
            RawText = result.RawText; // VD: "007590"
            ConfidenceInfo = $"Độ tin cậy AI: {result.Confidence:P1}";

            // 3. LOGIC XỬ LÝ SỐ LIỆU (007590 -> 759)
            FinalValue = ProcessMeterString(result.RawText);

            // Commands
            ConfirmCommand = new RelayCommand(() =>
            {
                IsConfirmed = true;
                RequestClose?.Invoke(this, EventArgs.Empty);
            });

            CancelCommand = new RelayCommand(() =>
            {
                IsConfirmed = false;
                RequestClose?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// Logic xử lý chuỗi số từ công tơ điện
        /// </summary>
        private decimal ProcessMeterString(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return 0;

            // Logic 1: Loại bỏ các số 0 ở đầu (007590 -> 7590)
            string temp = raw.TrimStart('0');

            // Logic 2: Xử lý số thập phân (Công tơ cơ thường có số cuối màu đỏ là số thập phân)
            // Nếu chuỗi dài (>= 4 ký tự) và logic của bạn là bỏ số cuối (VD: 7590 -> 759)
            // Hãy bật logic này:
            if (temp.Length >= 4)
            {
                // Giả định số cuối là số thập phân, ta chia cho 10 hoặc cắt bỏ chuỗi
                // Ví dụ: 007590 -> 7590 -> 759.0
                if (decimal.TryParse(temp, out decimal val))
                {
                    return val / 10m;
                }
            }

            // Logic mặc định nếu không thỏa điều kiện trên
            if (decimal.TryParse(temp, out decimal defaultVal))
            {
                return defaultVal;
            }

            return 0;
        }

        private BitmapImage? Base64ToBitmapImage(string base64String)
        {
            try
            {
                byte[] binaryData = Convert.FromBase64String(base64String);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(binaryData);
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }
            catch
            {
                return null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}