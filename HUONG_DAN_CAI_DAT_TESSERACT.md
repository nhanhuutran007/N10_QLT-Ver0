# Hướng dẫn cài đặt Tesseract OCR

## Tổng quan

Ứng dụng sử dụng Tesseract OCR để đọc chỉ số điện/nước từ ảnh. Tesseract là một công cụ OCR mã nguồn mở miễn phí.

## Cài đặt

### Bước 1: Tải Tesseract Data Files

Tesseract cần các file ngôn ngữ (language data files) để hoạt động. Bạn cần tải các file `.traineddata`:

1. Truy cập: https://github.com/tesseract-ocr/tessdata
2. Tải các file sau:
   - `eng.traineddata` (tiếng Anh - bắt buộc)
   - `vie.traineddata` (tiếng Việt - tùy chọn nhưng khuyến nghị)

### Bước 2: Đặt file vào thư mục dự án

Tạo thư mục `tessdata` trong thư mục output của ứng dụng (thường là `bin/Debug/net8.0-windows/` hoặc `bin/Release/net8.0-windows/`):

```
QLKDPhongTro.Presentation/
  bin/
    Debug/
      net8.0-windows/
        tessdata/
          eng.traineddata
          vie.traineddata
```

### Bước 3: Cấu hình trong code

File `OcrService.cs` đã được cấu hình để tự động tìm thư mục `tessdata`. Nếu bạn muốn thay đổi đường dẫn, sửa trong:

```csharp
_tesseractDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
```

## Sử dụng

Sau khi cài đặt, bạn có thể:

1. Mở ứng dụng
2. Vào "Quản lý tài chính"
3. Click "Thu nhập Chỉ số (Quét Ảnh)"
4. Chọn loại chỉ số (Điện/Nước)
5. Kéo thả hoặc chọn ảnh chứa chỉ số
6. Click "Quét ảnh"
7. Ứng dụng sẽ tự động đọc và hiển thị kết quả

## Lưu ý

- Ảnh cần rõ ràng, có độ phân giải tốt
- Chỉ số cần được chụp rõ, không bị mờ hoặc bị che
- Tesseract hoạt động tốt nhất với ảnh có độ tương phản cao
- Nếu kết quả không chính xác, thử chụp lại ảnh với ánh sáng tốt hơn

## Troubleshooting

### Lỗi: "Could not find tessdata directory"
- Đảm bảo thư mục `tessdata` tồn tại trong thư mục output
- Kiểm tra file `eng.traineddata` có trong thư mục `tessdata`

### Lỗi: "Failed to load language"
- Kiểm tra file `.traineddata` có đúng tên không
- Đảm bảo file không bị hỏng (tải lại nếu cần)

### Kết quả OCR không chính xác
- Thử với ảnh có độ phân giải cao hơn
- Đảm bảo ảnh không bị mờ hoặc bị nghiêng
- Thử với ảnh có nền trắng và chữ đen rõ ràng

## Tùy chọn: Sử dụng Azure Computer Vision (Nâng cao)

Nếu muốn độ chính xác cao hơn, bạn có thể tích hợp Azure Computer Vision API:

1. Đăng ký Azure Computer Vision
2. Lấy API Key
3. Thay thế `OcrService` bằng service sử dụng Azure API

Tuy nhiên, Azure Computer Vision là dịch vụ trả phí, trong khi Tesseract hoàn toàn miễn phí.



