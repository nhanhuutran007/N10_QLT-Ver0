using QLKDPhongTro.Presentation.Services;
using QLKDPhongTro.BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using QLKDPhongTro.Presentation.ViewModels.Base;
namespace QLKDPhongTro.Presentation.ViewModels
{
    public class ScanImageViewModel : INotifyPropertyChanged
    {
        private readonly OcrService _ocrService;
        private bool _isProcessing;
        private MeterType _selectedMeterType = MeterType.Electricity;
        private ObservableCollection<ImageScanResult> _scanResults = new();

        #region Properties

        public ObservableCollection<string> SelectedImagePaths { get; } = new();
        public ObservableCollection<ImageScanResult> ScanResults
        {
            get => _scanResults;
            set
            {
                _scanResults = value;
                OnPropertyChanged();
            }
        }

        public MeterType SelectedMeterType
        {
            get => _selectedMeterType;
            set
            {
                _selectedMeterType = value;
                OnPropertyChanged();
            }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotProcessing));
            }
        }

        public bool IsNotProcessing => !IsProcessing;

        public decimal? ExtractedValue
        {
            get
            {
                var validResults = ScanResults.Where(r => r.Result?.IsValid == true).ToList();
                if (!validResults.Any()) return null;

                // Lấy giá trị có confidence cao nhất
                var bestResult = validResults.OrderByDescending(r => r.Result?.Confidence ?? 0).First();
                return bestResult.Result?.Value;
            }
        }

        #endregion

        #region Commands

        public ICommand ProcessImagesCommand { get; }
        public ICommand ClearResultsCommand { get; }

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<string>? ShowMessageRequested;
        public event EventHandler<decimal?>? ValueExtracted;

        #endregion

        public ScanImageViewModel()
        {
            _ocrService = new OcrService();
            ProcessImagesCommand = new RelayCommand(async () => await ProcessImagesAsync(), () => SelectedImagePaths.Any() && !IsProcessing);
            ClearResultsCommand = new RelayCommand(() => ClearResults());
        }

        public void SetImagePaths(IEnumerable<string> paths)
        {
            SelectedImagePaths.Clear();
            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    SelectedImagePaths.Add(path);
                }
            }
        }

        public async Task ProcessImagesAsync()
        {
            if (!SelectedImagePaths.Any())
            {
                ShowMessageRequested?.Invoke(this, "Vui lòng chọn ít nhất một ảnh");
                return;
            }

            IsProcessing = true;
            ScanResults.Clear();

            try
            {
                var imagePaths = SelectedImagePaths.ToArray();
                var results = await _ocrService.AnalyzeImagesAsync(imagePaths, SelectedMeterType);

                for (int i = 0; i < results.Length; i++)
                {
                    var result = results[i];
                    var imagePath = imagePaths[i];
                    
                    // Tạo image preview
                    var imagePreview = LoadImagePreview(imagePath);
                    
                    // Đảm bảo Result không null và có RawText
                    if (result == null)
                    {
                        result = new MeterReadingResult 
                        { 
                            Type = SelectedMeterType, 
                            Value = 0, 
                            Confidence = 0f, 
                            ErrorMessage = "Lỗi khi xử lý",
                            RawText = "Không đọc được text từ ảnh"
                        };
                    }
                    
                    ScanResults.Add(new ImageScanResult
                    {
                        ImagePath = imagePath,
                        ImageName = Path.GetFileName(imagePath),
                        Result = result,
                        Status = result.IsValid ? "Thành công" : "Không tìm thấy",
                        StatusColor = result.IsValid ? "#10D096" : "#FF316A",
                        RawOcrText = !string.IsNullOrEmpty(result.RawText) ? result.RawText : "Không đọc được text từ ảnh",
                        ImagePreview = imagePreview
                    });
                }

                var validCount = results.Count(r => r.IsValid);
                if (validCount > 0)
                {
                    ShowMessageRequested?.Invoke(this, $"Đã xử lý {validCount}/{results.Length} ảnh thành công");
                    ValueExtracted?.Invoke(this, ExtractedValue);
                }
                else
                {
                    ShowMessageRequested?.Invoke(this, "Không tìm thấy chỉ số trong ảnh. Vui lòng thử lại với ảnh rõ hơn.");
                }
            }
            catch (Exception ex)
            {
                ShowMessageRequested?.Invoke(this, $"Lỗi khi xử lý ảnh: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void ClearResults()
        {
            ScanResults.Clear();
            SelectedImagePaths.Clear();
        }

        private System.Windows.Media.Imaging.BitmapImage? LoadImagePreview(string imagePath)
        {
            try
            {
                if (!File.Exists(imagePath))
                    return null;

                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.DecodePixelWidth = 200; // Resize để preview nhỏ hơn
                bitmap.EndInit();
                bitmap.Freeze(); // Freeze để có thể dùng trong thread khác
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ImageScanResult
    {
        public string ImagePath { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public MeterReadingResult? Result { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = "#586A84";
        public string RawOcrText { get; set; } = string.Empty; // Text đã đọc từ OCR để debug
        public System.Windows.Media.Imaging.BitmapImage? ImagePreview { get; set; } // Preview ảnh
    }
}

