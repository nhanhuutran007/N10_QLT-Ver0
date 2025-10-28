# N10_QLT-Ver0

Dự án công nghệ phần mềm **PHẦN MỀM QUẢN LÝ THUÊ TRỌ** của nhóm 10.

## Tổng quan

Dự án được phát triển theo kiến trúc **MVVM (Model-View-ViewModel)** với WPF, sử dụng **CommunityToolkit.Mvvm** cho data binding và command handling.

## Cấu trúc Layer

### 1. DataLayer (Model)

**Vị trí**: `QLKDPhongTro.DataLayer/`
**Chức năng**: Xử lý dữ liệu và truy cập database

**Cấu trúc**:

```
DataLayer/
├── Models/
│   ├── User.cs                    # Model User
│   ├── House.cs                   # Model House
│   └── RentedRoom.cs              # Model RentedRoom
├── Repositories/
│   ├── IUserRepository.cs         # Interface cho User Repository
│   ├── UserRepository.cs          # Implementation User Repository
│   ├── IHouseRepository.cs        # Interface cho House Repository
│   ├── HouseRepository.cs         # Implementation House Repository
│   ├── IRentedRoomRepository.cs   # Interface cho RentedRoom Repository
│   └── RentedRoomRepository.cs    # Implementation RentedRoom Repository
└── Utils/
    ├── PasswordHelper.cs          # Helper xử lý mật khẩu
    ├── EmailService.cs            # Service gửi email
    └── OtpHelper.cs               # Helper xử lý OTP
```

**Trách nhiệm**:

- Định nghĩa các model dữ liệu (User, House, RentedRoom)
- Thực hiện các thao tác CRUD với database
- Xử lý kết nối database
- Mã hóa/giải mã mật khẩu
- Gửi email OTP
- Xử lý OTP authentication

### 2. BusinessLayer (Controller)

**Vị trí**: `QLKDPhongTro.BusinessLayer/`
**Chức năng**: Xử lý logic nghiệp vụ và điều khiển

**Cấu trúc**:

```
BusinessLayer/
├── Controllers/
│   ├── AuthController.cs         # Controller xử lý authentication
│   ├── HouseController.cs        # Controller xử lý House
│   └── RentedRoomController.cs   # Controller xử lý RentedRoom
└── DTOs/
    ├── LoginResult.cs            # DTO kết quả đăng nhập
    ├── RegisterResult.cs         # DTO kết quả đăng ký
    ├── ValidationResult.cs       # DTO kết quả validation
    ├── HouseDto.cs               # DTO cho House
    └── RentedRoomDto.cs          # DTO cho RentedRoom
```

**Trách nhiệm**:

- Xử lý logic nghiệp vụ (authentication, house management, room management)
- Validation dữ liệu đầu vào
- Điều phối giữa View và Model
- Xử lý authentication và authorization
- Quản lý nhà trọ và phòng thuê

### 3. Presentation (View)

**Vị trí**: `QLKDPhongTro.Presentation/`
**Chức năng**: Giao diện người dùng (Views), chuyển hướng, binding dữ liệu với ViewModels

**Cấu trúc (hiện tại)**:

```
Presentation/
├── App.xaml, App.xaml.cs, AssemblyInfo.cs
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── OtpViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── ContractManagementViewModel.cs
│   ├── AddContractViewModel.cs
│   ├── RentedRoomViewModel.cs
│   ├── TenantViewModel.cs
│   └── ViewModelBase.cs
├── Views/
│   ├── Components/
│   │   ├── SidebarControl.xaml(.cs)
│   │   └── TopbarControl.xaml(.cs)
│   └── Windows/
│       ├── LoginWindow.xaml(.cs)
│       ├── RegisterWindow.xaml(.cs)
│       ├── OtpWindow.xaml(.cs)
│       ├── OtpLoginWindow.xaml          # màn hình nhập OTP khi đăng nhập (nếu dùng)
│       ├── DashWindow.xaml(.cs)
│       ├── ContractManagementWindow.xaml(.cs)
│       ├── TenantManagementWindow.xaml(.cs)
│       ├── RoomWindow.xaml(.cs)
│       ├── AddRoomWindow.xaml(.cs)
│       ├── EditRoomWindow.xaml(.cs)
│       ├── ViewRoomWindow.xaml(.cs)
│       ├── AddTenantWindow.xaml(.cs)
│       ├── AddContractWindow.xaml(.cs)
│       ├── FinancialWindow.xaml(.cs)
│       ├── ManualInputView.xaml(.cs)    # popup nhập thủ công chi phí
│       ├── ScanImageView.xaml(.cs)      # popup kéo-thả ảnh để quét
│       └── ProfileDropDown.xaml(.cs)
├── Converters/
│   ├── BoolToLoadingTextConverter.cs
│   ├── InverseBooleanConverter.cs
│   ├── IntegerValidationRule.cs
│   ├── IntToStringConverter.cs
│   ├── EmptyToVisibilityConverter .cs
│   └── StatusToColorConverter.cs
└── Resources/
    ├── Images/
    │   ├── Logo.png
    │   ├── email_icon.png
    │   ├── password_icon.png
    │   ├── avatar.jpg
    │   └── avatar1.jpg
    └── Templates/
        ├── HopDongMau.docx
        └── HopDongMau1.doc
```

**Trách nhiệm**:

- Hiển thị giao diện người dùng (Login, Register, OTP, Dashboard, Room Management)
- Xử lý tương tác người dùng
- Binding dữ liệu với ViewModels
- Navigation giữa các màn hình
- Quản lý trạng thái UI và loading states

## Luồng xử lý

### Đăng nhập với OTP:

1. **View** (LoginWindow) → User nhập thông tin
2. **ViewModel** (LoginViewModel) → Nhận dữ liệu từ View
3. **Controller** (AuthController) → Xử lý logic đăng nhập
4. **Model** (UserRepository) → Truy vấn database
5. **Controller** → Gửi OTP qua email
6. **View** (OtpWindow) → User nhập mã OTP
7. **ViewModel** (OtpViewModel) → Xác thực OTP
8. **Controller** → Trả về kết quả
9. **View** (DashWindow) → Chuyển đến Dashboard

### Đăng ký tài khoản:

1. **View** (RegisterWindow) → User nhập thông tin
2. **ViewModel** (RegisterViewModel) → Validation dữ liệu
3. **Model** (UserRepository) → Lưu tài khoản vào database
4. **ViewModel** → Hiển thị thông báo thành công
5. **View** → Chuyển về màn hình đăng nhập

### Quản lý phòng:

1. **View** (RoomManagementWindow) → User xem danh sách phòng
2. **ViewModel** (RentedRoomViewModel) → Load dữ liệu phòng
3. **Controller** (RentedRoomController) → Xử lý logic nghiệp vụ
4. **Model** (RentedRoomRepository) → Truy vấn database
5. **View** → Hiển thị danh sách phòng

## Tính năng chính

### Authentication & Authorization:

- **Đăng ký tài khoản**: Tạo tài khoản mới với validation đầy đủ
- **Đăng nhập với OTP**: Xác thực 2 bước qua email OTP
- **Quản lý mật khẩu**: Mã hóa bảo mật với PasswordHelper

### Quản lý nhà trọ:

- **Dashboard**: Tổng quan hệ thống
- **Quản lý phòng**: CRUD operations cho phòng thuê
- **Thêm/Sửa/Xem phòng**: Giao diện thân thiện
- **Sidebar navigation**: Điều hướng dễ dàng

### UI/UX Features:

- **Responsive design**: Giao diện thích ứng
- **Modern styling**: Thiết kế hiện đại với WPF
- **Loading states**: Trạng thái loading cho UX tốt
- **Validation**: Kiểm tra dữ liệu đầu vào
- **Error handling**: Xử lý lỗi thân thiện

## Lợi ích của kiến trúc MVVM

1. **Tách biệt trách nhiệm**: Mỗi layer có trách nhiệm riêng biệt
2. **Dễ bảo trì**: Thay đổi logic nghiệp vụ không ảnh hưởng đến UI
3. **Tái sử dụng**: Business logic có thể được sử dụng cho nhiều UI khác nhau
4. **Testing**: Dễ dàng unit test từng layer riêng biệt
5. **Mở rộng**: Dễ dàng thêm tính năng mới mà không ảnh hưởng code hiện tại
6. **Security**: OTP authentication và password hashing
7. **Scalability**: Kiến trúc có thể mở rộng cho nhiều tính năng khác

## Dependencies

```
Presentation Layer
    ↓ (depends on)
BusinessLayer
    ↓ (depends on)
DataLayer
```

- **Presentation** → **BusinessLayer** → **DataLayer**
- Presentation chỉ giao tiếp với BusinessLayer
- BusinessLayer giao tiếp với DataLayer
- DataLayer chỉ xử lý dữ liệu

## Công nghệ sử dụng

- **.NET 8.0**: Framework chính
- **WPF**: Giao diện người dùng
- **CommunityToolkit.Mvvm**: MVVM framework
- **SQL Server**: Database
- **Entity Framework**: ORM (nếu cần)
- **System.Net.Mail**: Gửi email

## Cách sử dụng

### Development:

1. **Thêm tính năng mới**: Tạo Controller trong BusinessLayer
2. **Thay đổi database**: Chỉ cần sửa DataLayer
3. **Thay đổi UI**: Chỉ cần sửa Presentation layer
4. **Thay đổi logic nghiệp vụ**: Chỉ cần sửa BusinessLayer

### Database Setup:

1. **SQL Server**: Cài đặt SQL Server LocalDB hoặc SQL Server Express
2. **Database**: Tạo database từ file `db/QLThueNhaV0.sql`
3. **Connection String**: Cập nhật trong `UserRepository.cs`

### Build & Run:

1. **Prerequisites**: .NET 8.0 SDK
2. **Dependencies**: CommunityToolkit.Mvvm, System.Data.SqlClient
3. **Build**: `dotnet build QLKDPhongTro.sln`
4. **Run**: `dotnet run --project QLKDPhongTro.Presentation`

### Configuration:

- **Email Service**: Cấu hình SMTP trong `EmailService.cs`
- **Database**: Cập nhật connection string
- **OTP Settings**: Điều chỉnh thời gian hết hạn OTP
