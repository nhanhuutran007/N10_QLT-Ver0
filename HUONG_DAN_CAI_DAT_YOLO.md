# Hướng dẫn cài đặt YOLOv9 cho đọc chỉ số đồng hồ điện

## 🎯 Tại sao dùng YOLOv9 thay vì OCR?

**OCR (Tesseract)** - Đọc toàn bộ text rồi tìm số:
- ❌ Dễ nhầm ký tự (O→0, l→1, S→5)
- ❌ Phải parse text phức tạp
- ❌ Độ chính xác thấp với đồng hồ điện

**YOLOv9 (Object Detection)** - Detect trực tiếp các chữ số:
- ✅ Detect chính xác từng chữ số (0-9)
- ✅ Không cần parse text
- ✅ Độ chính xác cao (precision 0.917, recall 0.899, mAP 0.919)
- ✅ Tự động sắp xếp và ghép số

## 📦 Cài đặt

### Bước 1: Tải YOLOv9 Model

Có 2 cách:

#### Cách 1: Sử dụng model có sẵn từ GitHub
1. Truy cập: https://github.com/sayantansikdar/yolov9n-meter-reading
2. Tải model đã train sẵn (file `.onnx` hoặc `.pt`)
3. Convert sang ONNX format nếu cần:
   ```python
   # Sử dụng ultralytics để convert
   from ultralytics import YOLO
   model = YOLO('yolov9n_meter_reading.pt')
   model.export(format='onnx')
   ```

#### Cách 2: Train model riêng
1. Chuẩn bị dataset ảnh đồng hồ điện với annotations
2. Train YOLOv9 theo hướng dẫn trong repo trên
3. Export model sang ONNX format

### Bước 2: Đặt model vào project

1. Tạo thư mục `models` trong thư mục output (bin/Debug/net8.0-windows/):
   ```
   QLKDPhongTro.Presentation/
   └── bin/
       └── Debug/
           └── net8.0-windows/
               └── models/
                   └── yolov9n_meter_reading.onnx
   ```

2. Hoặc đặt trong thư mục gốc project:
   ```
   QLKDPhongTro.Presentation/
   └── yolov9n_meter_reading.onnx
   ```

### Bước 3: Cấu hình

Service sẽ tự động:
- ✅ Tìm model trong các đường dẫn có thể
- ✅ Tự động chuyển từ OCR sang YOLO nếu có model
- ✅ Fallback về OCR nếu YOLO lỗi

## 🔧 Sử dụng

### Tự động (Recommended)
Service sẽ tự động dùng YOLO nếu có model, không cần cấu hình gì thêm!

```csharp
var service = new OcrService(); // Tự động detect YOLO model
var result = await service.AnalyzeImageAsync(imagePath, MeterType.Electricity);
```

### Kiểm tra phương thức đang dùng
```csharp
if (service.IsUsingYolo)
{
    Console.WriteLine("Đang dùng YOLOv9 - Độ chính xác cao!");
}
else
{
    Console.WriteLine("Đang dùng OCR - Cần cài YOLO model để nâng cao độ chính xác");
}
```

## 📊 So sánh hiệu suất

| Phương thức | Precision | Recall | mAP | Tốc độ |
|------------|-----------|--------|-----|--------|
| **Tesseract OCR** | ~0.6-0.7 | ~0.5-0.6 | - | Nhanh |
| **YOLOv9** | **0.917** | **0.899** | **0.919** | Trung bình |

## 🎯 Lợi ích của YOLOv9

1. **Độ chính xác cao hơn**: Detect trực tiếp chữ số, không cần parse text
2. **Xử lý leading zeros**: Nhận diện "00759" chính xác
3. **Chống nhiễu**: Ít bị ảnh hưởng bởi background phức tạp
4. **Tự động sắp xếp**: Tự động ghép các chữ số theo thứ tự

## ⚠️ Lưu ý

1. **Model size**: YOLOv9 model khá lớn (~20-50MB), cần đảm bảo có đủ dung lượng
2. **Performance**: YOLO chậm hơn OCR một chút nhưng chính xác hơn nhiều
3. **GPU**: Có thể cấu hình dùng GPU để tăng tốc (cần CUDA)

## 🔗 Tài liệu tham khảo

- [YOLOv9 Meter Reading GitHub](https://github.com/sayantansikdar/yolov9n-meter-reading)
- [ONNX Runtime Documentation](https://onnxruntime.ai/docs/)
- [YOLOv9 Paper](https://arxiv.org/abs/2402.13616)

## 🐛 Troubleshooting

### Lỗi: "Model không tìm thấy"
- Kiểm tra đường dẫn model
- Đảm bảo file `.onnx` tồn tại
- Kiểm tra tên file có đúng không

### Lỗi: "ONNX Runtime error"
- Cài đặt lại package: `Microsoft.ML.OnnxRuntime`
- Kiểm tra model format (phải là ONNX)
- Thử model khác

### Performance chậm
- Cân nhắc dùng GPU (CUDA)
- Giảm input size (hiện tại 640x640)
- Tối ưu confidence threshold



