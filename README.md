# 🏠 N10_QLT-Ver0 - Phần Mềm Quản Lý Thuê Trọ

Dự án công nghệ phần mềm **PHẦN MỀM QUẢN LÝ THUÊ TRỌ** của nhóm 10.

## 📋 Tổng quan

Phần mềm quản lý nhà trọ toàn diện được phát triển theo kiến trúc **MVVM (Model-View-ViewModel)** với WPF, sử dụng **CommunityToolkit.Mvvm** cho data binding và command handling. Hệ thống hỗ trợ quản lý đầy đủ các hoạt động của nhà trọ từ quản lý phòng, khách thuê, hợp đồng, thanh toán đến bảo trì sự cố.

## 🏗️ Kiến trúc hệ thống

### Kiến trúc 3 lớp (3-Layer Architecture)

```
┌─────────────────────────────────────────┐
│      Presentation Layer (View)         │
│  - WPF Views & ViewModels               │
│  - UI Components & Converters           │
└─────────────────┬───────────────────────┘
                  │ depends on
┌─────────────────▼───────────────────────┐
│      Business Layer (Controller)       │
│  - Controllers & Services               │
│  - Business Logic & DTOs                │
└─────────────────┬───────────────────────┘
                  │ depends on
┌─────────────────▼───────────────────────┐
│      Data Layer (Model)                 │
│  - Models & Repositories                │
│  - Database Access & Utilities          │
└─────────────────────────────────────────┘
```

## 📁 Cấu trúc dự án

### 1. 🗄️ DataLayer (Model)

**Vị trí**: `QLKDPhongTro.DataLayer/`  
**Chức năng**: Xử lý dữ liệu và truy cập database

#### Cấu trúc chi tiết:

```
DataLayer/
├── Models/                          # Các model dữ liệu
│   ├── User.cs                      # Model người dùng/admin
│   ├── House.cs                     # Model nhà trọ
│   ├── RentedRoom.cs                # Model phòng trọ
│   ├── Tenant.cs                    # Model khách thuê
│   ├── Contract.cs                  # Model hợp đồng
│   ├── Payment.cs                   # Model thanh toán
│   ├── MaintenanceIncident.cs       # Model sự cố bảo trì
│   ├── TenantAsset.cs               # Model tài sản khách thuê
│   ├── RoomTenantInfo.cs            # Model thông tin phòng-khách
│   └── TenantStayInfo.cs            # Model thông tin lưu trú
│
├── Repositories/                    # Repositories xử lý CRUD
│   ├── IUserRepository.cs           # Interface User Repository
│   ├── UserRepository.cs            # Implementation User Repository
│   ├── IHouseRepository.cs          # Interface House Repository
│   ├── HouseRepository.cs           # Implementation House Repository
│   ├── IRentedRoomRepository.cs     # Interface RentedRoom Repository
│   ├── RentedRoomRepository.cs      # Implementation RentedRoom Repository
│   ├── ITenantRepository.cs         # Interface Tenant Repository
│   ├── TenantRepository.cs          # Implementation Tenant Repository
│   ├── IContractRepository.cs       # Interface Contract Repository
│   ├── ContractRepository.cs        # Implementation Contract Repository
│   ├── IPaymentRepository.cs        # Interface Payment Repository
│   ├── PaymentRepository.cs         # Implementation Payment Repository
│   ├── IMaintenanceRepository.cs    # Interface Maintenance Repository
│   ├── MaintenanceRepository.cs     # Implementation Maintenance Repository
│   └── ConnectDB.cs                 # Database connection helper
│
└── Utils/                           # Utilities & Helpers
    ├── PasswordHelper.cs            # Mã hóa/giải mã mật khẩu
    ├── EmailService.cs              # Service gửi email
    ├── OtpHelper.cs                 # Xử lý OTP authentication
    ├── ContractGenerator.cs         # Tạo file hợp đồng Word
    └── QrCodeHelper.cs              # Tạo mã QR thanh toán
```

**Trách nhiệm**:
- ✅ Định nghĩa các model dữ liệu
- ✅ Thực hiện các thao tác CRUD với MySQL database
- ✅ Xử lý kết nối database
- ✅ Mã hóa/giải mã mật khẩu (SHA-256)
- ✅ Gửi email OTP và thông báo
- ✅ Tạo file hợp đồng từ template Word
- ✅ Tạo mã QR cho thanh toán

### 2. 💼 BusinessLayer (Controller)

**Vị trí**: `QLKDPhongTro.BusinessLayer/`  
**Chức năng**: Xử lý logic nghiệp vụ và điều khiển

#### Cấu trúc chi tiết:

```
BusinessLayer/
├── Controllers/                     # Controllers xử lý logic nghiệp vụ
│   ├── AuthController.cs            # Xác thực & phân quyền
│   ├── HouseController.cs           # Quản lý nhà trọ
│   ├── RentedRoomController.cs      # Quản lý phòng trọ
│   ├── TenantController.cs          # Quản lý khách thuê
│   ├── ContractController.cs        # Quản lý hợp đồng
│   ├── FinancialController.cs       # Quản lý tài chính
│   └── MaintenanceController.cs     # Quản lý bảo trì sự cố
│
├── Services/                        # Services xử lý nghiệp vụ phức tạp
│   ├── GoogleSheetsService.cs       # Đồng bộ dữ liệu từ Google Sheets
│   ├── GoogleFormService.cs         # Xử lý Google Form responses
│   ├── OcrService.cs                # OCR đọc số điện từ ảnh (Tesseract)
│   ├── YoloMeterReadingService.cs   # AI đọc đồng hồ điện (YOLO)
│   └── DebtProcessingService.cs     # Xử lý công nợ tự động
│
└── DTOs/                            # Data Transfer Objects
    ├── LoginResult.cs               # DTO kết quả đăng nhập
    ├── RegisterResult.cs            # DTO kết quả đăng ký
    ├── ValidationResult.cs          # DTO kết quả validation
    ├── HouseDto.cs                  # DTO cho House
    ├── RentedRoomDto.cs             # DTO cho RentedRoom
    ├── TenantDto.cs                 # DTO cho Tenant
    ├── ContractDto.cs               # DTO cho Contract
    ├── PaymentDto.cs                # DTO cho Payment
    └── ... (33 DTOs tổng cộng)
```

**Trách nhiệm**:
- ✅ Xử lý logic nghiệp vụ phức tạp
- ✅ Validation dữ liệu đầu vào
- ✅ Điều phối giữa View và Model
- ✅ Xử lý authentication và authorization
- ✅ Quản lý nhà trọ, phòng, khách thuê, hợp đồng
- ✅ Tính toán tài chính và công nợ
- ✅ Đồng bộ dữ liệu từ Google Sheets/Forms
- ✅ OCR và AI đọc số điện tự động

### 3. 🎨 Presentation (View)

**Vị trí**: `QLKDPhongTro.Presentation/`  
**Chức năng**: Giao diện người dùng và tương tác

#### Cấu trúc chi tiết:

```
Presentation/
├── App.xaml, App.xaml.cs            # Application entry point
├── AssemblyInfo.cs                  # Assembly information
│
├── ViewModels/                      # ViewModels (MVVM pattern)
│   ├── LoginViewModel.cs            # VM đăng nhập
│   ├── RegisterViewModel.cs         # VM đăng ký
│   ├── OtpViewModel.cs              # VM xác thực OTP
│   ├── ForgotPasswordEmailViewModel.cs  # VM quên mật khẩu
│   ├── ForgotPasswordOtpViewModel.cs    # VM OTP quên mật khẩu
│   ├── ResetPasswordViewModel.cs    # VM đặt lại mật khẩu
│   ├── DashboardViewModel.cs        # VM trang chủ
│   ├── RentedRoomViewModel.cs       # VM quản lý phòng
│   ├── TenantViewModel.cs           # VM quản lý khách thuê
│   ├── TenantDetailViewModel.cs     # VM chi tiết khách thuê
│   ├── ContractManagementViewModel.cs   # VM quản lý hợp đồng
│   ├── AddContractViewModel.cs      # VM thêm hợp đồng
│   ├── FinancialViewModel.cs        # VM tài chính
│   ├── FinancialDashboardViewModel.cs   # VM dashboard tài chính
│   ├── PaymentViewModel.cs          # VM thanh toán
│   ├── PaymentFormViewModel.cs      # VM form thanh toán
│   ├── EditPaymentViewModel.cs      # VM sửa thanh toán
│   ├── MaintenanceListViewModel.cs  # VM danh sách bảo trì
│   ├── ManualInputViewModel.cs      # VM nhập thủ công
│   ├── ScanImageViewModel.cs        # VM quét ảnh điện
│   ├── MeterReadingInspectionViewModel.cs  # VM kiểm tra số điện
│   ├── ManualDebtViewModel.cs       # VM quản lý công nợ
│   ├── UserSecurityViewModel.cs     # VM bảo mật tài khoản
│   └── IOtpEntryViewModel.cs        # Interface OTP entry
│
├── Views/
│   ├── Components/                  # Reusable UI Components
│   │   ├── SidebarControl.xaml(.cs)     # Sidebar navigation
│   │   └── TopbarControl.xaml(.cs)      # Top bar với user info
│   │
│   └── Windows/                     # Application Windows
│       ├── LoginWindow.xaml(.cs)            # Màn hình đăng nhập
│       ├── RegisterWindow.xaml(.cs)         # Màn hình đăng ký
│       ├── OtpLoginWindow.xaml(.cs)         # Màn hình OTP đăng nhập
│       ├── ForgotPasswordEmailWindow.xaml   # Màn hình quên mật khẩu
│       ├── ResetPasswordWindow.xaml         # Màn hình đặt lại mật khẩu
│       ├── DashWindow.xaml(.cs)             # Dashboard chính
│       ├── HouseInfoWindow.xaml             # Thông tin nhà trọ
│       │
│       ├── RoomWindow.xaml(.cs)             # Quản lý phòng
│       ├── AddRoomWindow.xaml(.cs)          # Thêm phòng mới
│       ├── EditRoomWindow.xaml(.cs)         # Sửa thông tin phòng
│       ├── ViewRoomWindow.xaml(.cs)         # Xem chi tiết phòng
│       │
│       ├── TenantManagementWindow.xaml(.cs) # Quản lý khách thuê
│       ├── AddTenantWindow.xaml(.cs)        # Thêm khách thuê
│       ├── TenantDetailWindow.xaml(.cs)     # Chi tiết khách thuê
│       ├── DeleteTenantConfirmWindow.xaml   # Xác nhận xóa khách
│       ├── SelectNewContractHolderWindow.xaml  # Chọn người thuê mới
│       ├── AddEditAssetWindow.xaml          # Quản lý tài sản khách
│       │
│       ├── ContractManagementWindow.xaml(.cs)  # Quản lý hợp đồng
│       ├── AddContractWindow.xaml(.cs)      # Thêm hợp đồng mới
│       │
│       ├── FinancialWindow.xaml(.cs)        # Quản lý tài chính
│       ├── PaymentListView.xaml             # Danh sách thanh toán
│       ├── PaymentFormWindow.xaml           # Form tạo hóa đơn
│       ├── EditPaymentDialog.xaml           # Sửa thanh toán
│       ├── InvoiceDetailView.xaml           # Chi tiết hóa đơn
│       ├── ManualInputView.xaml(.cs)        # Nhập thủ công số điện
│       ├── ScanImageView.xaml(.cs)          # Quét ảnh đồng hồ điện
│       ├── MeterReadingInspectionWindow.xaml  # Kiểm tra số điện
│       ├── ManualDebtWindow.xaml            # Quản lý công nợ
│       ├── ExpenseFormWindow.xaml           # Form chi phí
│       ├── AddBillingInfoWindow.xaml        # Thêm thông tin thanh toán
│       │
│       ├── MaintenanceListView.xaml         # Danh sách bảo trì sự cố
│       │
│       ├── UserSecurityWindow.xaml          # Bảo mật tài khoản
│       ├── ProfileDropDown.xaml(.cs)        # Dropdown profile
│       ├── QrPopupWindow.xaml               # Popup hiển thị QR
│       ├── ChatWindow.xaml                  # Chat (future feature)
│       └── ReportWindow.xaml                # Báo cáo thống kê
│
├── Converters/                      # Value Converters cho XAML binding
│   ├── BoolToVisibilityConverter.cs     # Bool → Visibility
│   ├── BoolToLoadingTextConverter.cs    # Bool → Loading text
│   ├── InverseBooleanConverter.cs       # Đảo ngược bool
│   ├── IntToStringConverter.cs          # Int → String
│   ├── IntegerValidationRule.cs         # Validation số nguyên
│   ├── EmptyToVisibilityConverter.cs    # Empty → Visibility
│   └── StatusToColorConverter.cs        # Status → Color
│
└── Resources/                       # Tài nguyên ứng dụng
    ├── Images/                      # Hình ảnh
    │   ├── Logo.png                 # Logo ứng dụng
    │   ├── email_icon.png           # Icon email
    │   ├── password_icon.png        # Icon password
    │   ├── avatar.jpg               # Avatar mặc định
    │   └── avatar1.jpg              # Avatar khác
    │
    └── Templates/                   # Templates
        ├── HopDongMau.docx          # Template hợp đồng Word
        └── HopDongMau1.doc          # Template hợp đồng khác
```

**Trách nhiệm**:
- ✅ Hiển thị giao diện người dùng
- ✅ Xử lý tương tác người dùng
- ✅ Data binding với ViewModels
- ✅ Navigation giữa các màn hình
- ✅ Quản lý trạng thái UI và loading states
- ✅ Validation input từ người dùng

## 🔄 Luồng xử lý dữ liệu

### 1. Đăng nhập với OTP:

```
User Input (LoginWindow)
    ↓
LoginViewModel validates input
    ↓
AuthController.LoginAsync()
    ↓
UserRepository checks credentials
    ↓
EmailService sends OTP
    ↓
User enters OTP (OtpLoginWindow)
    ↓
OtpViewModel validates OTP
    ↓
AuthController verifies OTP
    ↓
Navigate to DashWindow
```

### 2. Quản lý thanh toán với OCR:

```
User uploads image (ScanImageView)
    ↓
ScanImageViewModel processes image
    ↓
OcrService/YoloMeterReadingService extracts meter reading
    ↓
PaymentFormViewModel calculates payment
    ↓
FinancialController creates payment
    ↓
PaymentRepository saves to database
    ↓
Display invoice (InvoiceDetailView)
```

### 3. Đồng bộ bảo trì từ Google Sheets:

```
MaintenanceListViewModel triggers sync
    ↓
MaintenanceController.SyncFromGoogleSheetsAsync()
    ↓
GoogleSheetsService reads CSV data
    ↓
Parse timestamp, room, description, repair date
    ↓
MaintenanceRepository checks duplicates
    ↓
Save new incidents to database
    ↓
EmailService sends notifications to tenants
    ↓
Refresh maintenance list
```

## ✨ Tính năng chính

### 🔐 Authentication & Authorization
- ✅ Đăng ký tài khoản với validation đầy đủ
- ✅ Đăng nhập với xác thực 2 bước (OTP qua email)
- ✅ Quên mật khẩu và đặt lại mật khẩu
- ✅ Quản lý bảo mật tài khoản
- ✅ Mã hóa mật khẩu SHA-256

### 🏘️ Quản lý nhà trọ
- ✅ Dashboard tổng quan với thống kê
- ✅ Quản lý thông tin nhà trọ
- ✅ Quản lý phòng trọ (CRUD)
- ✅ Theo dõi trạng thái phòng (Trống/Đang thuê/Bảo trì/Dự kiến)
- ✅ Quản lý trang thiết bị phòng

### 👥 Quản lý khách thuê
- ✅ Thêm/Sửa/Xóa khách thuê
- ✅ Quản lý thông tin CCCD, liên hệ
- ✅ Quản lý tài sản khách thuê (xe, thú cưng)
- ✅ Lịch sử lưu trú
- ✅ Xác nhận xóa khách thuê an toàn

### 📝 Quản lý hợp đồng
- ✅ Tạo hợp đồng từ template Word
- ✅ Quản lý hợp đồng (Hiệu lực/Hết hạn/Sắp hết hạn)
- ✅ Tự động điền thông tin vào hợp đồng
- ✅ Lưu trữ file hợp đồng PDF
- ✅ Quản lý tiền cọc

### 💰 Quản lý tài chính
- ✅ Tạo hóa đơn thanh toán tự động
- ✅ Quản lý các khoản thu (tiền phòng, điện, nước, internet, vệ sinh, giữ xe)
- ✅ **OCR đọc số điện từ ảnh** (Tesseract)
- ✅ **AI đọc đồng hồ điện** (YOLO)
- ✅ Nhập thủ công số điện/nước
- ✅ Kiểm tra và xác nhận số điện
- ✅ Tính toán tự động tiền điện/nước
- ✅ Quản lý công nợ
- ✅ Lịch sử thanh toán
- ✅ Tạo mã QR thanh toán
- ✅ Dashboard tài chính với biểu đồ

### 🔧 Quản lý bảo trì sự cố
- ✅ Danh sách sự cố bảo trì
- ✅ **Đồng bộ tự động từ Google Sheets** (Form báo cáo sự cố)
- ✅ Quản lý trạng thái (Chưa xử lý/Đang xử lý/Hoàn tất)
- ✅ Theo dõi chi phí sửa chữa
- ✅ Ngày báo cáo và ngày dự kiến sửa
- ✅ **Gửi email thông báo tự động** cho khách thuê
- ✅ Tìm kiếm và lọc sự cố
- ✅ Phân trang danh sách

### 📧 Hệ thống Email
- ✅ Gửi OTP đăng nhập
- ✅ Gửi OTP quên mật khẩu
- ✅ Thông báo sự cố bảo trì
- ✅ Email template HTML đẹp mắt

### 🔗 Tích hợp Google Services
- ✅ Đồng bộ dữ liệu từ Google Sheets
- ✅ Xử lý Google Form responses
- ✅ Parse CSV từ Google Sheets
- ✅ Hỗ trợ timestamp và date formats

### 🎨 UI/UX Features
- ✅ Modern WPF design với Material Design
- ✅ Responsive layout
- ✅ Sidebar navigation
- ✅ Loading states và progress indicators
- ✅ Validation với error messages
- ✅ Popup và dialog windows
- ✅ Data grid với sorting và filtering
- ✅ DatePicker và ComboBox custom styles

## 🛠️ Công nghệ sử dụng

### Framework & Libraries
- **.NET 8.0** - Framework chính
- **WPF (Windows Presentation Foundation)** - UI Framework
- **CommunityToolkit.Mvvm** - MVVM framework
- **MySQL** - Database (chuyển từ SQL Server)
- **MySql.Data** - MySQL connector

### AI & OCR
- **Tesseract OCR** - Đọc số điện từ ảnh
- **YOLO (You Only Look Once)** - AI đọc đồng hồ điện

### External Services
- **Google Sheets API** - Đồng bộ dữ liệu
- **Google Forms** - Thu thập báo cáo sự cố
- **System.Net.Mail** - Gửi email SMTP

### Document Processing
- **DocumentFormat.OpenXml** - Xử lý file Word
- **QRCoder** - Tạo mã QR

### Other Tools
- **System.Drawing** - Xử lý hình ảnh
- **System.Net.Http** - HTTP client

## 📊 Database Schema

### Bảng chính:
- **Admin** - Thông tin quản trị viên
- **Nha** - Thông tin nhà trọ
- **Phong** - Thông tin phòng trọ
- **NguoiThue** - Thông tin khách thuê
- **HopDong** - Hợp đồng thuê
- **ThanhToan** - Thanh toán
- **BaoTri_SuCo** - Bảo trì sự cố
- **TaiSanNguoiThue** - Tài sản khách thuê
- **DeletedMaintenanceSignatures** - Lưu vết sự cố đã xóa
- **GoogleFormLog** - Log từ Google Form

### Quan hệ:
```
Admin ──┬── Nha
        │
Nha ────┼── Phong ──┬── NguoiThue ──┬── HopDong ──── ThanhToan
        │           │                │
        │           │                └── TaiSanNguoiThue
        │           │
        │           └── BaoTri_SuCo
```

## 🚀 Cài đặt và chạy

### Yêu cầu hệ thống:
- **.NET 8.0 SDK** hoặc cao hơn
- **MySQL Server** (hoặc MariaDB)
- **Visual Studio 2022** (khuyến nghị) hoặc VS Code
- **Windows 10/11** (WPF chỉ chạy trên Windows)

### Bước 1: Clone repository
```bash
git clone https://github.com/yourusername/N10_QLT-Ver0.git
cd N10_QLT-Ver0
```

### Bước 2: Cài đặt Database
1. Cài đặt MySQL Server
2. Tạo database mới:
```sql
CREATE DATABASE qlthuetra CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```
3. Import schema từ file `db/MySQLSchema.sql`

### Bước 3: Cấu hình Connection String
Cập nhật connection string trong `QLKDPhongTro.DataLayer/Repositories/ConnectDB.cs`:
```csharp
private static string _connectionString = "Server=localhost;Database=qlthuetra;Uid=root;Pwd=yourpassword;";
```

### Bước 4: Cấu hình Email Service
Cập nhật SMTP settings trong `QLKDPhongTro.DataLayer/Utils/EmailService.cs`:
```csharp
SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
{
    Credentials = new NetworkCredential("your-email@gmail.com", "your-app-password"),
    EnableSsl = true
};
```

### Bước 5: Build và chạy
```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build QLKDPhongTro.sln

# Run application
dotnet run --project QLKDPhongTro.Presentation
```

Hoặc mở `QLKDPhongTro.sln` trong Visual Studio và nhấn F5.

## 📖 Hướng dẫn sử dụng

### Đăng nhập lần đầu:
1. Chạy ứng dụng
2. Click "Đăng ký" để tạo tài khoản admin
3. Nhập thông tin và xác thực OTP qua email
4. Đăng nhập với tài khoản vừa tạo
5. Nhập OTP đăng nhập

### Quản lý phòng trọ:
1. Vào "Quản lý phòng" từ sidebar
2. Thêm phòng mới với thông tin đầy đủ
3. Cập nhật trạng thái phòng
4. Quản lý trang thiết bị

### Tạo hợp đồng:
1. Thêm khách thuê mới
2. Vào "Quản lý hợp đồng"
3. Tạo hợp đồng mới, chọn phòng và khách thuê
4. Hệ thống tự động tạo file Word từ template
5. In hoặc lưu file PDF

### Quản lý thanh toán:
1. Vào "Quản lý tài chính"
2. Tạo hóa đơn mới cho tháng
3. Nhập số điện/nước:
   - Chụp ảnh đồng hồ → OCR tự động đọc
   - Hoặc nhập thủ công
4. Kiểm tra và xác nhận
5. Tạo mã QR thanh toán
6. Gửi hóa đơn cho khách

### Quản lý bảo trì:
1. Khách thuê điền Google Form báo cáo sự cố
2. Hệ thống tự động đồng bộ từ Google Sheets
3. Gửi email thông báo cho khách thuê
4. Admin cập nhật trạng thái và chi phí
5. Theo dõi tiến độ sửa chữa

## 🎯 Lợi ích của kiến trúc MVVM

1. **Separation of Concerns** - Tách biệt trách nhiệm rõ ràng
2. **Maintainability** - Dễ bảo trì và mở rộng
3. **Testability** - Dễ dàng unit test
4. **Reusability** - Tái sử dụng business logic
5. **Scalability** - Mở rộng linh hoạt
6. **Team Collaboration** - Nhiều người làm việc song song

## 📝 Development Guidelines

### Thêm tính năng mới:

1. **Tạo Model** trong `DataLayer/Models/`
2. **Tạo Repository** trong `DataLayer/Repositories/`
3. **Tạo Controller** trong `BusinessLayer/Controllers/`
4. **Tạo ViewModel** trong `Presentation/ViewModels/`
5. **Tạo View** trong `Presentation/Views/Windows/`
6. **Update Database** schema nếu cần

### Code Style:
- Sử dụng **async/await** cho tất cả database operations
- Implement **INotifyPropertyChanged** trong ViewModels
- Sử dụng **RelayCommand** cho commands
- Validation đầy đủ ở cả ViewModel và Controller
- Error handling với try-catch
- Logging với Debug.WriteLine

## 🐛 Troubleshooting

### Lỗi kết nối database:
- Kiểm tra MySQL service đang chạy
- Kiểm tra connection string
- Kiểm tra firewall

### Lỗi gửi email:
- Kiểm tra SMTP credentials
- Bật "Less secure app access" cho Gmail
- Hoặc sử dụng App Password

### Lỗi OCR không đọc được:
- Kiểm tra ảnh rõ nét
- Đảm bảo Tesseract đã được cài đặt
- Thử nhập thủ công

### Lỗi Google Sheets sync:
- Kiểm tra Spreadsheet ID
- Kiểm tra quyền truy cập (public)
- Kiểm tra format dữ liệu

## 📄 License

This project is developed for educational purposes as part of Software Engineering course.

## 👥 Team Members - Nhóm 10

- Phạm Tấn Mạnh - Leader
- Trần Hữu Nhân - Developer
- Nguyễn Đăng Khoa - Developer
- Phạm Ngọc Hải - Developer

## 📞 Contact

For questions or support, please contact:nhanhuunhan009@gmail.com

---

**Phát triển bởi Nhóm 10 - Công nghệ phần mềm**
