# N10_QLT-Ver0
Dự án công nghệ phần mềm PHẦN MỀM QUẢN LÝ THUÊ TRỌ của nhóm 10.

# Kiến trúc MVC cho QLKDPhongTro

## Tổng quan
Dự án đã được tái cấu trúc theo mô hình MVC (Model-View-Controller) với các layer sau:

## Cấu trúc Layer

### 1. DataLayer (Model)
**Vị trí**: `QLKDPhongTro.DataLayer/`
**Chức năng**: Xử lý dữ liệu và truy cập database

**Cấu trúc**:
```
DataLayer/
├── Models/
│   └── User.cs                    # Model User
├── Repositories/
│   ├── IUserRepository.cs        # Interface cho User Repository
│   └── UserRepository.cs         # Implementation User Repository
└── Utils/
    └── PasswordHelper.cs          # Helper xử lý mật khẩu
```

**Trách nhiệm**:
- Định nghĩa các model dữ liệu
- Thực hiện các thao tác CRUD với database
- Xử lý kết nối database
- Mã hóa/giải mã mật khẩu

### 2. BusinessLayer (Controller)
**Vị trí**: `QLKDPhongTro.BusinessLayer/`
**Chức năng**: Xử lý logic nghiệp vụ và điều khiển

**Cấu trúc**:
```
BusinessLayer/
├── Controllers/
│   └── AuthController.cs         # Controller xử lý authentication
├── Models/
│   └── User.cs                   # Model User cho Business Layer
└── DTOs/
    ├── LoginResult.cs            # DTO kết quả đăng nhập
    ├── RegisterResult.cs         # DTO kết quả đăng ký
    └── ValidationResult.cs       # DTO kết quả validation
```

**Trách nhiệm**:
- Xử lý logic nghiệp vụ
- Validation dữ liệu
- Điều phối giữa View và Model
- Xử lý authentication và authorization

### 3. Presentation (View)
**Vị trí**: `QLKDPhongTro.Presentation/`
**Chức năng**: Giao diện người dùng và ViewModels

**Cấu trúc**:
```
Presentation/
├── ViewModels/
│   ├── LoginViewModel.cs         # ViewModel cho đăng nhập
│   ├── RegisterViewModel.cs      # ViewModel cho đăng ký
│   └── ViewModelBase.cs          # Base class cho ViewModels
├── Views/
│   └── Windows/
│       ├── LoginWindow.xaml      # Giao diện đăng nhập
│       ├── LoginWindow.xaml.cs   # Code-behind đăng nhập
│       ├── RegisterWindow.xaml   # Giao diện đăng ký
│       └── RegisterWindow.xaml.cs # Code-behind đăng ký
└── Converters/
    ├── BoolToLoadingTextConverter.cs
    └── InverseBooleanConverter.cs
```

**Trách nhiệm**:
- Hiển thị giao diện người dùng
- Xử lý tương tác người dùng
- Binding dữ liệu với ViewModels
- Navigation giữa các màn hình

## Luồng xử lý

### Đăng nhập:
1. **View** (LoginWindow) → User nhập thông tin
2. **ViewModel** (LoginViewModel) → Nhận dữ liệu từ View
3. **Controller** (AuthController) → Xử lý logic đăng nhập
4. **Model** (UserRepository) → Truy vấn database
5. **Controller** → Trả về kết quả
6. **ViewModel** → Cập nhật UI
7. **View** → Hiển thị kết quả

### Đăng ký:
1. **View** (RegisterWindow) → User nhập thông tin
2. **ViewModel** (RegisterViewModel) → Nhận dữ liệu từ View
3. **Controller** (AuthController) → Validation và xử lý logic
4. **Model** (UserRepository) → Lưu dữ liệu vào database
5. **Controller** → Trả về kết quả
6. **ViewModel** → Cập nhật UI
7. **View** → Hiển thị kết quả

## Lợi ích của kiến trúc mới

1. **Tách biệt trách nhiệm**: Mỗi layer có trách nhiệm riêng biệt
2. **Dễ bảo trì**: Thay đổi logic nghiệp vụ không ảnh hưởng đến UI
3. **Tái sử dụng**: Business logic có thể được sử dụng cho nhiều UI khác nhau
4. **Testing**: Dễ dàng unit test từng layer riêng biệt
5. **Mở rộng**: Dễ dàng thêm tính năng mới mà không ảnh hưởng code hiện tại

## Dependencies

- **Presentation** → **BusinessLayer** → **DataLayer**
- Presentation chỉ giao tiếp với BusinessLayer
- BusinessLayer giao tiếp với DataLayer
- DataLayer chỉ xử lý dữ liệu

## Cách sử dụng

1. **Thêm tính năng mới**: Tạo Controller trong BusinessLayer
2. **Thay đổi database**: Chỉ cần sửa DataLayer
3. **Thay đổi UI**: Chỉ cần sửa Presentation layer
4. **Thay đổi logic nghiệp vụ**: Chỉ cần sửa BusinessLayer
