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
- Định nghĩa các model dữ liệu
- Thực hiện các thao tác CRUD với database
- Xử lý kết nối database
- Mã hóa/giải mã mật khẩu
- Gửi email và xử lý OTP

### 2. BusinessLayer (Controller)
**Vị trí**: `QLKDPhongTro.BusinessLayer/`
**Chức năng**: Xử lý logic nghiệp vụ và điều khiển

**Cấu trúc**:
```
BusinessLayer/
├── Controllers/
│   ├── AuthController.cs          # Controller xử lý authentication
│   ├── HouseController.cs         # Controller xử lý quản lý nhà
│   └── RentedRoomController.cs    # Controller xử lý quản lý phòng thuê
├── DTOs/
│   ├── LoginResult.cs             # DTO kết quả đăng nhập
│   ├── RegisterResult.cs          # DTO kết quả đăng ký
│   ├── ValidationResult.cs        # DTO kết quả validation
│   ├── HouseDto.cs                # DTO cho House
│   └── RentedRoomDto.cs           # DTO cho RentedRoom
└── Models/
    └── (Các model business logic)
```

**Trách nhiệm**:
- Xử lý logic nghiệp vụ
- Validation dữ liệu
- Điều phối giữa View và Model
- Xử lý authentication và authorization
- Quản lý nhà và phòng thuê

### 3. Presentation (View)
**Vị trí**: `QLKDPhongTro.Presentation/`
**Chức năng**: Giao diện người dùng và ViewModels

**Cấu trúc**:
```
Presentation/
├── ViewModels/
│   ├── ViewModelBase.cs           # Base class cho ViewModels
│   ├── LoginViewModel.cs          # ViewModel cho đăng nhập
│   ├── RegisterViewModel.cs       # ViewModel cho đăng ký
│   ├── OtpViewModel.cs            # ViewModel cho xác thực OTP
│   ├── DashboardViewModel.cs      # ViewModel cho dashboard
│   └── RentedRoomViewModel.cs     # ViewModel cho quản lý phòng
├── Views/
│   ├── Windows/
│   │   ├── LoginWindow.xaml       # Giao diện đăng nhập
│   │   ├── LoginWindow.xaml.cs   # Code-behind đăng nhập
│   │   ├── RegisterWindow.xaml   # Giao diện đăng ký
│   │   ├── RegisterWindow.xaml.cs # Code-behind đăng ký
│   │   ├── OtpWindow.xaml         # Giao diện xác thực OTP
│   │   ├── OtpWindow.xaml.cs      # Code-behind OTP
│   │   ├── DashWindow.xaml        # Giao diện dashboard
│   │   ├── DashWindow.xaml.cs     # Code-behind dashboard
│   │   ├── RoomManagementWindow.xaml # Giao diện quản lý phòng
│   │   ├── RoomManagementWindow.xaml.cs # Code-behind quản lý phòng
│   │   ├── AddRoomWindow.xaml     # Giao diện thêm phòng
│   │   ├── AddRoomWindow.xaml.cs  # Code-behind thêm phòng
│   │   ├── EditRoomWindow.xaml    # Giao diện sửa phòng
│   │   ├── EditRoomWindow.xaml.cs # Code-behind sửa phòng
│   │   ├── ViewRoomWindow.xaml    # Giao diện xem phòng
│   │   └── ViewRoomWindow.xaml.cs # Code-behind xem phòng
│   └── Components/
│       ├── SidebarControl.xaml    # Component sidebar
│       └── SidebarControl.xaml.cs # Code-behind sidebar
├── Converters/
│   ├── BoolToLoadingTextConverter.cs
│   ├── IntegerValidationRule.cs
│   ├── IntToStringConverter.cs
│   └── InverseBooleanConverter.cs
└── Resources/
    └── Images/
        ├── Logo.png
        ├── email_icon.png
        └── password_icon.png
```

**Trách nhiệm**:
- Hiển thị giao diện người dùng
- Xử lý tương tác người dùng
- Binding dữ liệu với ViewModels
- Navigation giữa các màn hình
- Quản lý trạng thái UI

## Luồng xử lý

### Đăng nhập với OTP:
1. **View** (LoginWindow) → User nhập thông tin
2. **ViewModel** (LoginViewModel) → Nhận dữ liệu từ View
3. **Controller** (AuthController) → Xử lý logic đăng nhập
4. **Model** (UserRepository) → Gửi OTP qua email
5. **View** (OtpWindow) → User nhập mã OTP
6. **ViewModel** (OtpViewModel) → Xác thực OTP
7. **Controller** → Trả về kết quả
8. **View** (DashWindow) → Chuyển đến Dashboard

### Đăng ký tài khoản:
1. **View** (RegisterWindow) → User nhập thông tin
2. **ViewModel** (RegisterViewModel) → Validation dữ liệu
3. **Model** (UserRepository) → Tạo tài khoản trực tiếp
4. **ViewModel** → Hiển thị thông báo thành công
5. **View** (LoginWindow) → Chuyển về màn hình đăng nhập

### Quản lý phòng:
1. **View** (RoomManagementWindow) → Hiển thị danh sách phòng
2. **ViewModel** (RentedRoomViewModel) → Load dữ liệu phòng
3. **Controller** (RentedRoomController) → Xử lý logic nghiệp vụ
4. **Model** (RentedRoomRepository) → Truy vấn database
5. **View** → Cập nhật danh sách phòng

## Tính năng chính

### 🔐 Authentication & Authorization
- **Đăng nhập với OTP**: Xác thực 2 bước qua email
- **Đăng ký tài khoản**: Tạo tài khoản mới với validation
- **Mã hóa mật khẩu**: Sử dụng PasswordHelper để bảo mật
- **Quản lý session**: Theo dõi trạng thái đăng nhập

### 🏠 Quản lý nhà và phòng
- **Dashboard**: Tổng quan hệ thống
- **Quản lý phòng**: CRUD operations cho phòng thuê
- **Thêm/Sửa/Xem phòng**: Giao diện chi tiết cho từng phòng
- **Sidebar navigation**: Điều hướng dễ dàng

### 🎨 Giao diện người dùng
- **Modern UI**: Thiết kế hiện đại với WPF
- **Responsive design**: Tương thích nhiều kích thước màn hình
- **Data binding**: Sử dụng CommunityToolkit.Mvvm
- **Custom controls**: Các component tái sử dụng

### 📧 Email & OTP
- **EmailService**: Gửi email thông báo
- **OtpHelper**: Tạo và xác thực mã OTP
- **Security**: Bảo mật thông tin người dùng

## Lợi ích của kiến trúc MVVM

1. **Tách biệt trách nhiệm**: Mỗi layer có trách nhiệm riêng biệt
2. **Dễ bảo trì**: Thay đổi logic nghiệp vụ không ảnh hưởng đến UI
3. **Tái sử dụng**: Business logic có thể được sử dụng cho nhiều UI khác nhau
4. **Testing**: Dễ dàng unit test từng layer riêng biệt
5. **Mở rộng**: Dễ dàng thêm tính năng mới mà không ảnh hưởng code hiện tại
6. **Data Binding**: Tự động cập nhật UI khi dữ liệu thay đổi
7. **Command Pattern**: Xử lý user interaction một cách rõ ràng

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

### 1. Thêm tính năng mới:
- Tạo Controller trong BusinessLayer
- Tạo ViewModel trong Presentation
- Tạo View (Window/UserControl) trong Presentation

### 2. Thay đổi database:
- Chỉ cần sửa DataLayer
- Cập nhật Repository interfaces
- Không ảnh hưởng đến BusinessLayer và Presentation

### 3. Thay đổi UI:
- Chỉ cần sửa Presentation layer
- Sử dụng Data Binding để kết nối với ViewModel
- Không ảnh hưởng đến BusinessLayer

### 4. Thay đổi logic nghiệp vụ:
- Chỉ cần sửa BusinessLayer
- Cập nhật Controllers và DTOs
- Không ảnh hưởng đến Presentation

## Cài đặt và chạy

1. **Clone repository**:
   ```bash
   git clone [repository-url]
   cd N10_QLT-Ver0
   ```

2. **Restore packages**:
   ```bash
   dotnet restore
   ```

3. **Build solution**:
   ```bash
   dotnet build
   ```

4. **Chạy ứng dụng**:
   ```bash
   dotnet run --project QLKDPhongTro.Presentation
   ```

## Database Setup

1. Tạo database `QLThueNhaV0` trong SQL Server
2. Chạy script `db/QLThueNhaV0.sql` để tạo tables
3. Cập nhật connection string trong `UserRepository.cs`

## Đóng góp

1. Fork repository
2. Tạo feature branch
3. Commit changes
4. Push to branch
5. Tạo Pull Request

## License

Dự án được phát triển bởi nhóm 10 - N10_QLT-Ver0
