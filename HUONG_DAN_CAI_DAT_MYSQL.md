# Hướng dẫn cài đặt và chạy ứng dụng Quản Lý Phòng Trọ

## Tình trạng hiện tại

Ứng dụng đang sử dụng MySQL database. Bạn có 3 lựa chọn:

---

## Lựa chọn 1: Sử dụng MySQL Server có sẵn (Khuyến nghị nếu có quyền truy cập)

Ứng dụng đã được cấu hình sẵn để kết nối đến MySQL server:
- **Server**: `host80.vietnix.vn`
- **Database**: `githubio_QLT_Ver1`
- **Port**: `3306`

**Nếu bạn có quyền truy cập server này**, chỉ cần:
1. Đảm bảo có kết nối internet
2. Chạy ứng dụng trực tiếp - nó sẽ tự động kết nối

---

## Lựa chọn 2: Cài đặt MySQL Local (Khuyến nghị cho phát triển)

### Bước 1: Tải và cài đặt MySQL

**Windows:**
1. Tải MySQL Community Server từ: https://dev.mysql.com/downloads/mysql/
2. Chọn phiên bản phù hợp (khuyến nghị MySQL 8.0 trở lên)
3. Cài đặt với các tùy chọn mặc định
4. Ghi nhớ **root password** bạn đặt trong quá trình cài đặt

**Hoặc sử dụng XAMPP (Dễ hơn):**
1. Tải XAMPP từ: https://www.apachefriends.org/
2. Cài đặt và khởi động MySQL từ XAMPP Control Panel

### Bước 2: Tạo Database và Import Schema

1. Mở MySQL Command Line Client hoặc MySQL Workbench
2. Tạo database mới:
```sql
CREATE DATABASE githubio_QLT_Ver1 CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

3. Import file schema:
```bash
# Sử dụng MySQL Command Line
mysql -u root -p githubio_QLT_Ver1 < db/MySQLSchema.sql
```

Hoặc trong MySQL Workbench:
- File → Open SQL Script → Chọn file `db/MySQLSchema.sql`
- Execute

### Bước 3: Cập nhật Connection String

Mở file `QLKDPhongTro.DataLayer/Repositories/ConnectDB.cs` và sửa:

```csharp
public static string GetConnectionString()
{
    if (_connectionString != null)
        return _connectionString;

    // ===== CẤU HÌNH KẾT NỐI MYSQL LOCAL =====
    string server = "localhost";  // hoặc "127.0.0.1"
    string database = "githubio_QLT_Ver1";
    string username = "root";  // hoặc user bạn tạo
    string password = "your_password_here";  // Mật khẩu bạn đặt
    string port = "3306";
    
    _connectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};SslMode=None;CharSet=utf8mb4;";
    
    return _connectionString;
}
```

**Lưu ý:** 
- Thay `your_password_here` bằng mật khẩu root của bạn
- Nếu dùng XAMPP, mật khẩu mặc định thường là rỗng (`""`)

---

## Lựa chọn 3: Chạy với Dữ liệu Mẫu (Không cần MySQL)

Ứng dụng đã được thiết kế để tự động sử dụng **dữ liệu mẫu** khi không kết nối được database.

### Cách hoạt động:

1. Khi khởi động, ứng dụng sẽ thử kết nối MySQL
2. Nếu **không kết nối được**, nó sẽ tự động chuyển sang dữ liệu mẫu
3. Bạn vẫn có thể sử dụng ứng dụng bình thường, nhưng dữ liệu sẽ không được lưu

### Dữ liệu mẫu bao gồm:
- ✅ Danh sách phòng
- ✅ Danh sách người thuê
- ✅ Hợp đồng
- ✅ Bản ghi tài chính
- ✅ Công nợ

**Lưu ý:** 
- Dữ liệu mẫu chỉ hiển thị, không lưu vào database
- Mỗi lần khởi động lại sẽ reset về dữ liệu mẫu ban đầu

---

## Kiểm tra kết nối

Sau khi cấu hình, bạn có thể kiểm tra:

1. **Chạy ứng dụng** - Nếu thấy dữ liệu hiển thị → Thành công!
2. **Kiểm tra Console/Log** - Nếu có thông báo "Không thể kết nối database" → Đang dùng dữ liệu mẫu

---

## Troubleshooting

### Lỗi: "Unable to connect to any of the specified MySQL hosts"
- Kiểm tra MySQL đã khởi động chưa (XAMPP Control Panel)
- Kiểm tra firewall có chặn port 3306 không
- Kiểm tra username/password đúng chưa

### Lỗi: "Access denied for user"
- Kiểm tra username và password
- Đảm bảo user có quyền truy cập database

### Lỗi: "Unknown database"
- Đảm bảo đã tạo database `githubio_QLT_Ver1`
- Đảm bảo đã import schema từ file `MySQLSchema.sql`

---

## Tóm tắt nhanh

**Nếu muốn chạy ngay (không cần MySQL):**
- ✅ Chỉ cần build và chạy - ứng dụng sẽ tự động dùng dữ liệu mẫu

**Nếu muốn lưu dữ liệu thật:**
- 📥 Cài MySQL (XAMPP hoặc MySQL Server)
- 📥 Tạo database và import schema
- ⚙️ Cập nhật connection string trong `ConnectDB.cs`

---

## Liên hệ hỗ trợ

Nếu gặp vấn đề, vui lòng kiểm tra:
1. File log/console output
2. Connection string trong `ConnectDB.cs`
3. Trạng thái MySQL service



