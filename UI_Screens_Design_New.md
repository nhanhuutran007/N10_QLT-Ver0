# UI Screens Design

Tài liệu mô tả các màn hình (Window) trong ứng dụng quản lý phòng trọ. Mỗi mục gồm:

- **Main Flow**: luồng chính, hành động người dùng.
- **User interface**: các thành phần UI chính, kèm gợi ý màn hình cần chụp.
- **Validation Rule / Data Mapping**: ràng buộc dữ liệu, binding và cách dữ liệu được lưu/truy vấn.
> Lưu ý: Nội dung chỉ mô tả các chức năng/thành phần **thực sự tồn tại trong XAML**, không suy đoán thêm.


---

# Đăng nhập hệ thống (LoginWindow)

## Main Flow

| **Screen**      | **Đăng nhập** |
|-----------------|---------------|
| **Description** | Màn hình đăng nhập vào hệ thống. |
| **Screen Access** | Màn hình đăng nhập là màn hình đầu tiên khi người dùng khởi động hệ thống. |

## User Interface

- **Khu vực bên trái**: panel giới thiệu, hình ảnh minh hoạ, mô tả tính năng.
- **Khu vực bên phải (form)**:
  - `TextBox` Email.
  - `PasswordBox` Mật khẩu.
  - `CheckBox` Hiển thị mật khẩu.
  - `Button` Đăng nhập.
  - `Button/Hyperlink` Quên mật khẩu.
  - `Hyperlink` Tạo tài khoản mới.

- **Screenshot**: chụp **toàn bộ cửa sổ LoginWindow** khi form đăng nhập được hiển thị đầy đủ (cả panel giới thiệu và form bên phải).

## Validation Rule / Data Mapping

| **Field** | **Type**   | **Description** |
|----------|------------|-----------------|
| Email    | TextBox    | Địa chỉ email đăng ký dùng để đăng nhập. Bắt buộc nhập, binding vào thuộc tính `Email` trong ViewModel. |
| Mật khẩu | PasswordBox| Mật khẩu đăng nhập, bắt buộc nhập. Binding/được đọc từ `PasswordBox` và truyền vào logic đăng nhập trong ViewModel. |

- Nút `Đăng nhập` gọi command xử lý đăng nhập với `Email` và `Mật khẩu`.
- Nút `Quên mật khẩu` mở `ForgotPasswordEmailWindow`.
- Link `Tạo tài khoản mới` mở `RegisterWindow`.


---

# Đăng ký tài khoản (RegisterWindow)

## Main Flow

| **Screen**      | Đăng ký tài khoản |
|-----------------|-------------------|
| **Description** | Màn hình tạo tài khoản người dùng mới cho hệ thống. |
| **Screen Access** | Được mở từ màn hình Đăng nhập khi người dùng chọn liên kết "Tạo tài khoản mới". |

## User Interface

- **Bên trái**: panel giới thiệu, mô tả tính năng hệ thống.
- **Bên phải (form)**:
  - `TextBox` Email.
  - `TextBox` Tên đăng nhập.
  - `TextBox` Mã nhà.
  - `PasswordBox` Mật khẩu.
  - `PasswordBox` Xác nhận mật khẩu.
  - `CheckBox` Hiển thị mật khẩu.
  - Các dòng ghi chú yêu cầu độ mạnh mật khẩu.
  - `Button` Đăng ký.
  - Link quay lại `Đăng nhập`.

- **Screenshot**: chụp **toàn bộ cửa sổ RegisterWindow**, tập trung vào khu vực form nhập thông tin đăng ký.

## Validation Rule / Data Mapping

| **Field**        | **Type**    | **Description** |
|------------------|------------|-----------------|
| Email            | TextBox    | Địa chỉ email đăng ký tài khoản. Bắt buộc nhập, binding vào thuộc tính `Email` trong ViewModel. |
| Tên đăng nhập    | TextBox    | Tên người dùng hiển thị/đăng nhập, bắt buộc nhập, binding `UserName`. |
| Mã nhà           | TextBox    | Mã định danh nhà trọ mà tài khoản quản lý, binding `HouseId`. Có thể được kiểm tra tồn tại trong hệ thống. |
| Mật khẩu         | PasswordBox| Mật khẩu đăng ký, bắt buộc nhập, được đọc từ `PasswordBox` và kiểm tra độ mạnh theo ghi chú hiển thị. |
| Xác nhận mật khẩu| PasswordBox| Nhập lại mật khẩu để xác nhận, được so sánh với trường Mật khẩu trong ViewModel. |

- Nút `Đăng ký` gọi command tạo tài khoản mới nếu các trường hợp lệ.
- Link `Đăng nhập` điều hướng người dùng quay lại `LoginWindow`.


---

# Quên mật khẩu - nhập email (ForgotPasswordEmailWindow)

## Main Flow

| **Screen**      | Quên mật khẩu - nhập email |
|-----------------|-----------------------------|
| **Description** | Màn hình yêu cầu người dùng nhập email để nhận OTP hoặc link đặt lại mật khẩu. |
| **Screen Access** | Được mở từ `LoginWindow` khi bấm "Quên mật khẩu". |

## User Interface

- `TextBlock` mô tả: Nhập email đã đăng ký.
- `TextBox` Email.
- Nút `Gửi mã` hoặc `Tiếp tục`.
- Nút `Đóng`/`Quay lại đăng nhập`.

- **Screenshot**: chụp **ForgotPasswordEmailWindow** với một email đã được nhập.

## Validation Rule / Data Mapping

| **Field** | **Type** | **Description** |
|----------|---------|-----------------|
| Email    | TextBox | Email đã đăng ký tài khoản; bắt buộc nhập, validate định dạng email. |
| Gửi mã   | Button  | Gọi command gửi OTP/link đặt lại mật khẩu tới email; chuyển sang màn OTP nếu thành công. |
| Đóng     | Button  | Đóng popup, quay về `LoginWindow`. |

---

# Đăng nhập OTP (OtpLoginWindow)

## Main Flow

| **Screen**      | Đăng nhập OTP |
|-----------------|----------------|
| **Description** | Màn hình cho phép người dùng nhập mã OTP được gửi qua email để xác thực. |
| **Screen Access** | Được mở sau bước nhập email quên mật khẩu hoặc đăng nhập hai lớp (nếu có). |

## User Interface

- `TextBlock` hiển thị email/địa chỉ nhận mã.
- `TextBox`/khung nhập mã OTP (có thể chia 4–6 ô nhỏ tuỳ XAML).
- Nút `Xác nhận`.
- Nút `Gửi lại mã`.
- Nút `Hủy`.

- **Screenshot**: chụp **OtpLoginWindow** khi người dùng đã nhập một mã OTP ví dụ.

## Validation Rule / Data Mapping

| **Field**    | **Type** | **Description** |
|-------------|---------|-----------------|
| OTP         | TextBox(s) | Nhập mã xác thực được gửi qua email; validate độ dài/định dạng. |
| Xác nhận    | Button  | Gọi command kiểm tra mã OTP; nếu đúng thì cho phép tiếp tục (đăng nhập/đặt lại mật khẩu). |
| Gửi lại mã  | Button  | Gọi command gửi lại OTP mới; có thể kèm giới hạn thời gian. |
| Hủy         | Button  | Huỷ quy trình OTP và quay lại màn trước. |

---

# Đặt lại mật khẩu (ResetPasswordWindow)

## Main Flow

| **Screen**      | Đặt lại mật khẩu |
|-----------------|------------------|
| **Description** | Màn hình cho phép người dùng đặt mật khẩu mới sau khi xác thực OTP. |
| **Screen Access** | Được mở sau khi OTP hợp lệ trong quy trình quên mật khẩu. |

## User Interface

- `PasswordBox` Mật khẩu mới.
- `PasswordBox` Nhập lại mật khẩu mới.
- Nút `Lưu mật khẩu`.
- Nút `Hủy`.

- **Screenshot**: chụp **ResetPasswordWindow** khi người dùng nhập mật khẩu mới/nhập lại.

## Validation Rule / Data Mapping

| **Field**            | **Type**     | **Description** |
|----------------------|-------------|-----------------|
| Mật khẩu mới         | PasswordBox | Nhập mật khẩu mới, có thể kèm validate (độ dài, ký tự đặc biệt...). |
| Nhập lại mật khẩu mới| PasswordBox | Phải trùng với "Mật khẩu mới"; validate khi bấm lưu. |
| Lưu mật khẩu         | Button      | Gọi command đặt lại mật khẩu; nếu thành công có thể chuyển về `LoginWindow`. |
| Hủy                  | Button      | Huỷ thao tác đặt lại mật khẩu, đóng cửa sổ. |

---

# Bảng điều khiển (DashWindow)

## Main Flow

| **Screen**      | Bảng điều khiển |
|-----------------|-----------------|
| **Description** | Màn hình tổng quan sau khi đăng nhập, hiển thị giao diện chính và điều hướng tới các chức năng quản lý. |
| **Screen Access** | Được hiển thị sau khi đăng nhập thành công từ `LoginWindow`. |

## User Interface

- `SidebarControl`: thanh điều hướng chính (phòng, khách thuê, hợp đồng, tài chính,...).
- `TopbarControl`: thanh trên cùng (tìm kiếm, profile dropdown,...).
- Khu vực nội dung chính hiển thị **carousel 2 ảnh** (Anh1.jpg, Anh2.jpg).

- **Screenshot**: chụp **toàn bộ DashWindow**, đảm bảo thấy rõ sidebar bên trái, topbar phía trên và vùng carousel hình ảnh ở trung tâm.

## Validation Rule / Data Mapping

| **Field**       | **Type**      | **Description** |
|-----------------|--------------|-----------------|
| SidebarControl  | UserControl  | Điều hướng sang các màn hình quản lý (phòng, khách thuê, hợp đồng, tài chính, báo cáo,...). Binding vào ViewModel chung để xử lý điều hướng. |
| TopbarControl   | UserControl  | Thanh trên cùng chứa ô tìm kiếm, menu người dùng, các nút tiện ích; binding vào ViewModel để thực hiện tìm kiếm/hành động nhanh. |
| Carousel ảnh    | Image/ItemsControl | Hiển thị danh sách ảnh tĩnh (Anh1.jpg, Anh2.jpg) theo cấu hình trong XAML/Resource hoặc ViewModel. |


---

# Danh sách phòng (RoomWindow)

## Main Flow

| **Screen**      | Danh sách phòng |
|-----------------|------------------|
| **Description** | Màn hình quản lý phòng, hiển thị danh sách tất cả các phòng cùng thông tin cơ bản. |
| **Screen Access** | Được mở từ `DashWindow` hoặc từ menu điều hướng trong `SidebarControl`. |

## User Interface

- `SidebarControl` (bên trái) và `TopbarControl` (trên cùng).
- Thanh tiêu đề và mô tả màn hình.
- Khu vực bộ lọc:
  - Ô `TextBox` tìm kiếm phòng.
  - `ComboBox` sắp xếp (theo tiêu chí được binding).
  - `Button` Thêm phòng.
- `DataGrid` danh sách phòng hiển thị các cột chính (theo XAML):
  - Mã phòng (`MaPhong`).
  - Tên phòng (`TenPhong`).
  - Diện tích (`DienTich`).
  - Giá cơ bản / Giá thuê (`GiaCoBan`).
  - Trạng thái (`TrangThai`).
  - Các cột thao tác (xem/chỉnh sửa,...).
- Thanh phân trang: số trang hiện tại, nút chuyển trang.

- **Screenshot**: chụp **toàn bộ RoomWindow khi DataGrid có dữ liệu**, cho thấy rõ bộ lọc, bảng phòng và phân trang.

## Validation Rule / Data Mapping

| **Field**      | **Type**   | **Description** |
|----------------|-----------|-----------------|
| Tìm kiếm phòng | TextBox   | Nhập từ khoá để lọc danh sách phòng theo tên/mã; binding vào thuộc tính `SearchText` trong ViewModel, lọc lại danh sách khi thay đổi. |
| Sắp xếp        | ComboBox  | Chọn tiêu chí sắp xếp (ví dụ theo tên, giá, trạng thái); binding `SortOrder` để thay đổi thứ tự hiển thị. |
| Thêm phòng     | Button    | Mở `AddRoomWindow` để thêm phòng mới, gọi command tương ứng trong ViewModel. |
| MaPhong        | DataGrid Column | Cột hiển thị mã phòng, binding `MaPhong` từ đối tượng phòng trong `ItemsSource`. |
| TenPhong       | DataGrid Column | Cột hiển thị tên phòng, binding `TenPhong`. |
| DienTich       | DataGrid Column | Cột hiển thị diện tích phòng, binding `DienTich`. |
| GiaCoBan       | DataGrid Column | Cột hiển thị giá cơ bản/giá thuê, binding `GiaCoBan`. |
| TrangThai      | DataGrid Column | Cột hiển thị trạng thái phòng (Trống, Đang thuê, Đang bảo trì,...), binding `TrangThai`. |
| Thao tác       | DataGrid Column (Buttons) | Các nút xem chi tiết/chỉnh sửa trên từng dòng, gọi command để mở `ViewRoomWindow` hoặc `EditRoomWindow` với phòng tương ứng. |


---

# Thêm phòng (AddRoomWindow)

## Main Flow

| **Screen**      | Thêm phòng |
|-----------------|------------|
| **Description** | Màn hình tạo mới một phòng trong hệ thống. |
| **Screen Access** | Được mở từ `RoomWindow` khi người dùng chọn nút `Thêm phòng`. |

## User Interface

- Header:
  - Tiêu đề binding `Title` (ví dụ: "Thêm phòng").
  - Nút `✕` để đóng.
- Form chính:
  - `TextBox` Tên phòng (`NewRoom.TenPhong`).
  - `TextBox` Diện tích (`NewRoom.DienTich`).
  - `TextBox` Giá thuê (`NewRoom.GiaCoBan`, `StringFormat=N0`).
  - `TextBox` Giá bằng chữ (`NewRoom.GiaBangChu`).
  - `ComboBox` Trạng thái (`StatusOptions`, SelectedValue=`NewRoom.TrangThai`).
  - `TextBox` Tiện nghi (`NewRoom.TrangThietBi`, multiline, danh sách phân tách dấu phẩy).
  - `TextBox` Ghi chú (`NewRoom.GhiChu`, multiline).
- Footer:
  - `Button` Hủy bỏ.
  - `Button` Lưu (text lấy từ `ButtonContent`).

- **Screenshot**: chụp **toàn bộ AddRoomWindow** với đầy đủ các trường nhập, ưu tiên khi có dữ liệu mẫu.

## Validation Rule / Data Mapping

| **Field**      | **Type** | **Description** |
|----------------|---------|-----------------|
| Tên phòng      | TextBox | Tên phòng, bắt buộc nhập, binding `NewRoom.TenPhong`. |
| Diện tích      | TextBox | Diện tích phòng (m²), bắt buộc nhập, binding `NewRoom.DienTich`. |
| Giá thuê       | TextBox | Giá thuê phòng (VNĐ), bắt buộc nhập; binding `NewRoom.GiaCoBan` với `StringFormat=N0`. |
| Giá bằng chữ   | TextBox | Giá thuê hiển thị dạng chữ, binding `NewRoom.GiaBangChu`. |
| Trạng thái     | ComboBox| Chọn trạng thái phòng, ItemsSource=`StatusOptions`, SelectedValue=`NewRoom.TrangThai`. |
| Tiện nghi      | TextBox | Danh sách tiện nghi, nhập dạng văn bản, binding `NewRoom.TrangThietBi`. |
| Ghi chú        | TextBox | Ghi chú bổ sung về phòng, binding `NewRoom.GhiChu`. |
| Lưu            | Button  | Gọi `SaveCommand` trong ViewModel để lưu phòng mới sau khi validate các field bắt buộc. |
| Hủy bỏ         | Button  | Gọi `CancelAddEditCommand` để đóng cửa sổ mà không lưu. |


---

# Sửa phòng (EditRoomWindow)

## Main Flow

| **Screen**      | Sửa phòng |
|-----------------|-----------|
| **Description** | Màn hình chỉnh sửa thông tin một phòng đã tồn tại. |
| **Screen Access** | Được mở từ `RoomWindow` hoặc `ViewRoomWindow` khi người dùng chọn chức năng chỉnh sửa phòng. |

## User Interface

- Giao diện gần như giống `AddRoomWindow`:
  - Header hiển thị `Title` tương ứng (ví dụ: "Chỉnh sửa phòng").
  - Các trường: Tên phòng, Diện tích, Giá thuê, Giá bằng chữ, Trạng thái, Tiện nghi, Ghi chú.
  - Footer có `Hủy bỏ` và `Lưu`.

- **Screenshot**: chụp **EditRoomWindow** với dữ liệu thật của một phòng đang tồn tại.

## Validation Rule / Data Mapping

| **Field**    | **Type** | **Description** |
|--------------|---------|-----------------|
| Tên phòng    | TextBox | Binding `NewRoom.TenPhong` với giá trị phòng hiện tại; bắt buộc nhập. |
| Diện tích    | TextBox | Binding `NewRoom.DienTich`; bắt buộc nhập. |
| Giá thuê     | TextBox | Binding `NewRoom.GiaCoBan` với format số; bắt buộc nhập. |
| Giá bằng chữ | TextBox | Binding `NewRoom.GiaBangChu`. |
| Trạng thái   | ComboBox| Binding `NewRoom.TrangThai`, ItemsSource=`StatusOptions`. |
| Tiện nghi    | TextBox | Binding `NewRoom.TrangThietBi`. |
| Ghi chú      | TextBox | Binding `NewRoom.GhiChu`. |
| Lưu          | Button  | Gọi `SaveCommand` để cập nhật phòng trong data source nếu dữ liệu hợp lệ. |
| Hủy bỏ       | Button  | Gọi `CancelAddEditCommand` để huỷ chỉnh sửa và đóng cửa sổ. |


---

# Danh sách khách thuê (TenantManagementWindow)

## Main Flow

| **Screen**      | Danh sách khách thuê |
|-----------------|-----------------------|
| **Description** | Màn hình quản lý khách thuê, hiển thị danh sách tất cả khách thuê và cho phép thao tác xem/sửa/xoá. |
| **Screen Access** | Được mở từ `DashWindow` hoặc menu trong `SidebarControl`. |

## User Interface

- Thanh sidebar/topbar giống các màn hình quản lý khác.
- Ô tìm kiếm khách thuê (`SearchText`).
- `ComboBox` sort (mới nhất, cũ nhất,... theo ViewModel).
- `Button` Thêm khách thuê.
- `DataGrid` danh sách khách thuê:
  - Mã khách thuê (`MaKhachThue`).
  - Họ tên (`HoTen`).
  - Số điện thoại (`SoDienThoai`).
  - Ngày sinh (`NgaySinh`).
  - Giới tính (`GioiTinh`).
  - Trạng thái (`TrangThai`).
  - Cột thao tác: Xem, Sửa, Xóa.
- Phân trang (PageIndex, Prev/Next,...).

- **Screenshot**: chụp **toàn bộ TenantManagementWindow** khi DataGrid hiển thị dữ liệu nhiều dòng (để thấy rõ layout danh sách và action).

## Validation Rule / Data Mapping

| **Field**          | **Type**        | **Description** |
|--------------------|----------------|-----------------|
| Tìm kiếm khách thuê| TextBox        | Nhập từ khoá để lọc danh sách khách theo tên/số điện thoại; binding `SearchText`. |
| Sắp xếp            | ComboBox       | Chọn tiêu chí sắp xếp; binding `SortOrder`. |
| Thêm khách thuê    | Button         | Mở `AddTenantWindow` để thêm khách thuê mới. |
| MaKhachThue        | DataGrid Column| Cột mã khách thuê, binding `MaKhachThue`. |
| HoTen              | DataGrid Column| Cột họ tên khách thuê, binding `HoTen`. |
| SoDienThoai        | DataGrid Column| Cột số điện thoại, binding `SoDienThoai`. |
| NgaySinh           | DataGrid Column| Cột ngày sinh, binding `NgaySinh`. |
| GioiTinh           | DataGrid Column| Cột giới tính, binding `GioiTinh`. |
| TrangThai          | DataGrid Column| Cột trạng thái khách thuê, binding `TrangThai`. |
| Thao tác           | DataGrid Column (Buttons) | Các nút Xem/Sửa/Xóa khách thuê; gọi command tương ứng trên ViewModel với item hiện tại. |


---

# Thêm / Sửa khách thuê (AddTenantWindow)

## Main Flow

| **Screen**      | Thêm / Sửa khách thuê |
|-----------------|------------------------|
| **Description** | Màn hình dùng để tạo mới hoặc chỉnh sửa thông tin một khách thuê. |
| **Screen Access** | Được mở từ `TenantManagementWindow` khi chọn thêm khách thuê mới hoặc chỉnh sửa một khách đã có. |

## User Interface

- Header hiển thị `Title` và mô tả "Vui lòng nhập đầy đủ thông tin bên dưới".
- Form chia thành nhiều hàng:
  - `ComboBox` Phòng (ItemsSource=`Rooms`, hiển thị `TenPhong`).
  - `TextBox` Họ tên (`NewTenant.HoTen`).
  - `TextBox` CCCD (`NewTenant.CCCD`).
  - `DatePicker` Ngày sinh (`NewTenant.NgaySinh`).
  - `ComboBox` Giới tính (`GenderOptions`, `NewTenant.GioiTinh`).
  - `TextBox` Số điện thoại (`NewTenant.SoDienThoai`).
  - `TextBox` Nghề nghiệp (`NewTenant.NgheNghiep`).
  - `ComboBox` Trạng thái người thuê (`TenantStatuses`, `NewTenant.TrangThai`).
  - `TextBox` Nơi cấp, `DatePicker` Ngày cấp CCCD.
  - `TextBox` Email, Địa chỉ.
  - `TextBox` Ghi chú (multiline).
- Footer: nút `Hủy bỏ` và `Lưu`.

- **Screenshot**: chụp **AddTenantWindow** ở trạng thái điền mẫu đầy đủ tất cả trường.

## Validation Rule / Data Mapping

| **Field**           | **Type**    | **Description** |
|---------------------|------------|-----------------|
| Phòng               | ComboBox   | Chọn phòng mà khách sẽ ở; ItemsSource=`Rooms`, SelectedItem binding tới phòng của `NewTenant`. |
| Họ tên              | TextBox    | Họ tên khách thuê, bắt buộc nhập, binding `NewTenant.HoTen`. |
| CCCD                | TextBox    | Số CCCD/hộ chiếu, bắt buộc nhập, binding `NewTenant.CCCD`. |
| Ngày sinh           | DatePicker | Ngày sinh của khách, bắt buộc nhập, binding `NewTenant.NgaySinh`. |
| Giới tính           | ComboBox   | Chọn giới tính, ItemsSource=`GenderOptions`, binding `NewTenant.GioiTinh`. |
| Số điện thoại       | TextBox    | Số điện thoại liên hệ, bắt buộc nhập, binding `NewTenant.SoDienThoai`. |
| Nghề nghiệp         | TextBox    | Nghề nghiệp, binding `NewTenant.NgheNghiep`. |
| Trạng thái người thuê| ComboBox  | Trạng thái khách thuê (đang ở, đã rời đi,...), ItemsSource=`TenantStatuses`, binding `NewTenant.TrangThai`. |
| Nơi cấp CCCD        | TextBox    | Nơi cấp giấy tờ, binding thuộc tính tương ứng trong `NewTenant`. |
| Ngày cấp CCCD       | DatePicker | Ngày cấp giấy tờ, binding thuộc tính tương ứng. |
| Email               | TextBox    | Email liên hệ, binding `NewTenant.Email` (nếu có trong model). |
| Địa chỉ             | TextBox    | Địa chỉ thường trú, binding `NewTenant.DiaChi`. |
| Ghi chú             | TextBox    | Ghi chú thêm, binding `NewTenant.GhiChu`. |
| Lưu                 | Button     | Gọi `SaveCommand` để validate và lưu thông tin khách thuê. |
| Hủy bỏ / Đóng       | Button     | Gọi `CloseButton_Click` hoặc command huỷ để đóng cửa sổ mà không lưu. |


# Chi tiết khách thuê (TenantDetailWindow)

## Main Flow

| **Screen**      | Chi tiết khách thuê |
|-----------------|----------------------|
| **Description** | Màn hình hiển thị đầy đủ thông tin cá nhân, tài sản và lưu trú của một khách thuê. |
| **Screen Access** | Được mở từ `TenantManagementWindow` khi người dùng chọn xem chi tiết khách thuê. |

## User Interface

- Header hiển thị tên khách, trạng thái, phòng đang thuê.
- **Thông tin cá nhân**:
  - Họ tên, Số điện thoại, CCCD, Ngày sinh, Địa chỉ, Giới tính, Trạng thái người thuê.
- **Tài sản mang theo**:
  - Bảng (DataGrid/ListView) hiển thị danh sách tài sản (Loại tài sản, Mô tả, Phí phụ thu).
  - Nút Thêm/Sửa/Xóa tài sản (mở `AddEditAssetWindow`).
- **Thông tin lưu trú**:
  - Phòng đang ở, Trạng thái hợp đồng, Ngày bắt đầu/kết thúc, Tiền cọc.
- **Liên hệ nhanh**:
  - Email, Điện thoại.
- Nút đóng cửa sổ.

- **Screenshot**: chụp **toàn bộ TenantDetailWindow** với data thực tế của một khách, thể hiện đủ 3 khu vực chính.

## Validation Rule / Data Mapping

| **Field**              | **Type**          | **Description** |
|------------------------|------------------|-----------------|
| Thông tin cá nhân      | TextBlocks       | Các label/binding hiển thị dữ liệu từ đối tượng tenant (Họ tên, SĐT, CCCD, Ngày sinh, Địa chỉ, Giới tính, Trạng thái). |
| Danh sách tài sản      | DataGrid/ListView| Binding collection tài sản mang theo của khách; mỗi dòng gồm loại tài sản, mô tả, phí phụ thu. |
| Thêm/Sửa/Xóa tài sản   | Buttons          | Gọi các command tương ứng để mở `AddEditAssetWindow` với chế độ thêm/sửa hoặc xoá tài sản khỏi danh sách. |
| Thông tin lưu trú      | TextBlocks       | Hiển thị phòng đang ở, trạng thái hợp đồng, ngày bắt đầu/kết thúc, tiền cọc; binding từ thông tin hợp đồng của khách. |
| Liên hệ nhanh          | Buttons/Hyperlinks| Các nút/đường dẫn nhanh để gọi điện hoặc gửi email (nếu được khai báo trong XAML). |
| Đóng                   | Button           | Đóng cửa sổ chi tiết khách thuê. |


---

# Quản lý hợp đồng (ContractManagementWindow)

## Main Flow

| **Screen**      | Quản lý hợp đồng |
|-----------------|------------------|
| **Description** | Màn hình hiển thị và quản lý toàn bộ hợp đồng thuê phòng. |
| **Screen Access** | Được mở từ `DashWindow` hoặc menu điều hướng trong `SidebarControl`. |

## User Interface

- Sidebar/Topbar giống các màn quản lý khác.
- Thanh tìm kiếm và combobox sắp xếp.
- `Button` Thêm hợp đồng.
- `Button` "Tải hợp đồng sắp hết hạn".
- `Button` "Gửi cảnh báo hết hạn".
- `DataGrid` danh sách hợp đồng với các cột:
  - Mã hợp đồng (`MaHopDong`).
  - Tên phòng (`TenPhong`).
  - Tên người thuê (`TenNguoiThue`).
  - Ngày bắt đầu (`NgayBatDau`).
  - Ngày kết thúc (`NgayKetThuc`).
  - Tiền cọc (`TienCoc`).
  - Trạng thái (`TrangThai`).
  - Cột thao tác (Sửa/Xoá).
- Phân trang.

- **Screenshot**: chụp **ContractManagementWindow** với DataGrid có dữ liệu, thể hiện rõ các nút lọc hết hạn và gửi cảnh báo.

## Validation Rule / Data Mapping

| **Field**                 | **Type**        | **Description** |
|---------------------------|----------------|-----------------|
| Tìm kiếm hợp đồng         | TextBox        | Nhập từ khoá (tên phòng, người thuê, mã hợp đồng) để lọc danh sách; binding thuộc tính tìm kiếm trong ViewModel. |
| Sắp xếp                   | ComboBox       | Chọn tiêu chí sắp xếp danh sách hợp đồng; binding thuộc tính sort trong ViewModel. |
| Thêm hợp đồng             | Button         | Mở `AddContractWindow` để tạo hợp đồng mới. |
| Tải hợp đồng sắp hết hạn  | Button         | Gọi command lọc/tải danh sách hợp đồng gần hết hạn. |
| Gửi cảnh báo hết hạn      | Button         | Gửi cảnh báo tới khách thuê với các hợp đồng sắp hết hạn (theo logic trong ViewModel). |
| MaHopDong                 | DataGrid Column| Cột mã hợp đồng, binding `MaHopDong`. |
| TenPhong                  | DataGrid Column| Cột tên phòng, binding `TenPhong`. |
| TenNguoiThue              | DataGrid Column| Cột tên người thuê, binding `TenNguoiThue`. |
| NgayBatDau                | DataGrid Column| Cột ngày bắt đầu hợp đồng, binding `NgayBatDau`. |
| NgayKetThuc               | DataGrid Column| Cột ngày kết thúc, binding `NgayKetThuc`. |
| TienCoc                   | DataGrid Column| Cột tiền cọc, binding `TienCoc`. |
| TrangThai                 | DataGrid Column| Cột trạng thái hợp đồng, binding `TrangThai`. |
| Thao tác                  | DataGrid Column (Buttons) | Các nút Sửa/Xoá hợp đồng; gọi command tương ứng với bản ghi hiện tại. |


---

# Thêm hợp đồng (AddContractWindow)

## Main Flow

| **Screen**      | Thêm hợp đồng |
|-----------------|----------------|
| **Description** | Màn hình tạo mới hợp đồng thuê phòng. |
| **Screen Access** | Được mở từ `ContractManagementWindow` khi người dùng nhấn `Thêm hợp đồng`. |

## User Interface

- Các trường nhập chính:
  - `ComboBox` Phòng (`SelectedRoom`).
  - `ComboBox` Khách thuê (`SelectedTenant`).
  - `DatePicker` Ngày bắt đầu (`StartDate`).
  - `DatePicker` Ngày kết thúc (`EndDate`).
  - `TextBox` Tiền cọc (`DepositAmount`).
  - `TextBox` Điều khoản đặc biệt.
  - `TextBox` Đường dẫn lưu file hợp đồng kèm nút `Chọn...`.
- Footer: nút `Hủy` và `Lưu`.

- **Screenshot**: chụp **AddContractWindow** với các trường được điền mẫu.

## Validation Rule / Data Mapping

| **Field**           | **Type**    | **Description** |
|---------------------|------------|-----------------|
| Phòng               | ComboBox   | Chọn phòng ký hợp đồng, binding `SelectedRoom`. |
| Khách thuê          | ComboBox   | Chọn khách thuê, binding `SelectedTenant`. |
| Ngày bắt đầu        | DatePicker | Ngày bắt đầu hiệu lực hợp đồng, binding `StartDate`. |
| Ngày kết thúc       | DatePicker | Ngày kết thúc dự kiến của hợp đồng, binding `EndDate`. |
| Tiền cọc            | TextBox    | Số tiền cọc, binding `DepositAmount`. |
| Điều khoản đặc biệt | TextBox    | Ghi chú điều khoản riêng (nếu có), binding thuộc tính tương ứng trong ViewModel. |
| Đường dẫn lưu file  | TextBox    | Đường dẫn lưu file hợp đồng, binding thuộc tính path; nút `Chọn...` mở dialog chọn thư mục/tệp. |
| Lưu                 | Button     | Gọi `SaveCommand` kiểm tra dữ liệu hợp lệ và lưu hợp đồng. |
| Hủy                 | Button     | Đóng cửa sổ mà không lưu. |


---

# Chọn người đứng tên hợp đồng mới (SelectNewContractHolderWindow)

## Main Flow

| **Screen**      | Chọn người đứng tên hợp đồng mới |
|-----------------|-----------------------------------|
| **Description** | Dialog chọn khách thuê khác để thay thế người đứng tên hợp đồng khi xoá khách hiện tại. |
| **Screen Access** | Được mở khi xoá khách thuê đang là người đứng tên trong một hợp đồng. |

## User Interface

- Nội dung cảnh báo về việc cần chọn người đứng tên thay thế.
- `ListBox` danh sách khách còn lại trong phòng, mỗi item hiển thị tên, số điện thoại, trạng thái.
- Nút `Hủy` và `Xác nhận` (disable nếu chưa chọn khách).

- **Screenshot**: chụp **SelectNewContractHolderWindow** với danh sách khách và một khách đang được chọn.

## Validation Rule / Data Mapping

| **Field**          | **Type**  | **Description** |
|--------------------|----------|-----------------|
| Danh sách khách    | ListBox  | ItemsSource binding tới danh sách khách thuê còn lại trong phòng. |
| Khách được chọn    | SelectedItem | Binding tới thuộc tính `SelectedTenant` trong ViewModel. |
| Nút Xác nhận       | Button   | Chỉ `IsEnabled` khi `SelectedTenant` khác null; khi bấm sẽ cập nhật người đứng tên hợp đồng. |
| Nút Hủy            | Button   | Đóng dialog mà không thay đổi hợp đồng. |

---

# Chi tiết phòng (ViewRoomWindow)

## Main Flow

| **Screen**      | Chi tiết phòng |
|-----------------|----------------|
| **Description** | Màn hình hiển thị chi tiết một phòng, bao gồm thông tin cơ bản, người thuê hiện tại, tiện nghi và các yêu cầu bảo trì. |
| **Screen Access** | Được mở từ `RoomWindow` khi người dùng chọn xem chi tiết phòng. |

## User Interface

- Header: tiêu đề "Chi tiết phòng", tên phòng, nút `✕`.
- Phần thân chia 2 cột:

**Cột trái**
- Card thông tin phòng:
  - Tên phòng, ID, trạng thái (`SelectedRoom.TrangThai`).
  - Giá thuê (`GiaCoBan`, `GiaBangChu`).
  - Diện tích (`DienTich`).
- Card "Người thuê": danh sách `CurrentTenants` với vai trò (chủ hợp đồng/khách phụ), trạng thái khách, thông tin hợp đồng.
- Card "Tiện nghi": `SelectedRoomAmenities` dạng chip.
- Ghi chú phòng (`SelectedRoom.GhiChu`).

**Cột phải**
- Card "Trạng thái & Bảo trì": danh sách `SelectedRoomMaintenance` với mô tả sự cố, ngày báo cáo, trạng thái.

- Footer: nút `Chỉnh sửa` (mở màn hình chỉnh sửa) và `Đóng`.

- **Screenshot**: chụp **ViewRoomWindow** với phòng có ít nhất một khách thuê và vài yêu cầu bảo trì.

## Validation Rule / Data Mapping

| **Field**                    | **Type**           | **Description** |
|------------------------------|-------------------|-----------------|
| Thông tin phòng              | TextBlocks        | Binding từ `SelectedRoom` cho tên phòng, mã phòng, trạng thái, giá thuê, diện tích, ghi chú. |
| Danh sách người thuê         | ItemsControl/List | Binding collection `CurrentTenants`, mỗi item hiển thị tên, vai trò, trạng thái khách và thông tin hợp đồng liên quan. |
| Tiện nghi phòng              | ItemsControl      | Binding collection `SelectedRoomAmenities`, hiển thị dạng chip/badge. |
| Danh sách bảo trì            | ItemsControl/List | Binding collection `SelectedRoomMaintenance`, mỗi item gồm mô tả sự cố, ngày báo cáo, trạng thái. 532→| Số yêu cầu bảo trì đang mở   | Badge/TextBlock   | Hiển thị `OpenMaintenanceCount` nếu được khai báo trong XAML. |
 533→| Nút Chỉnh sửa                | Button            | Gọi command mở `EditRoomWindow` cho phòng hiện tại. |
 534→| Nút Đóng                     | Button            | Đóng `ViewRoomWindow`. |
 535→
 536→---
 537→
 538→# Quản lý tài chính (FinancialWindow)
 539→
 540→## Main Flow
 541→
 542→| **Screen**      | Quản lý tài chính |
 543→|-----------------|-------------------|
 544→| **Description** | Màn hình quản lý thu chi, công nợ và đồng bộ dữ liệu thanh toán. |
 545→| **Screen Access** | Được mở từ `DashWindow` hoặc menu điều hướng trong `SidebarControl`. |
 546→
 547→## User Interface
 548→
 549→- Sidebar/Topbar giống các màn quản lý khác.
 550→- Thanh tab/bộ lọc chế độ xem ("Tất cả bản ghi", "Quản lý công nợ").
 551→- `Button` "Đồng bộ G.Form".
 552→- Ô tìm kiếm, `ComboBox` sắp xếp.
 553→- Khu cấu hình phí cố định:
 554→  - `TextBox` Tiền nước.
 555→  - `TextBox` Tiền internet.
 556→  - `TextBox` Tiền vệ sinh.
 557→- Danh sách bản ghi tài chính hiển thị dạng thẻ/card, mỗi card có menu ngữ cảnh với các thao tác:
 558→  - "Xem chi tiết".
 559→  - "Đánh dấu đã trả".
 560→  - "Chỉnh sửa".
 561→  - "Xóa".
 562→- Vùng hiển thị loading khi đang xử lý.
 563→
 564→- **Screenshot**: chụp **toàn bộ FinancialWindow** ở tab chính với danh sách card tài chính và khu cấu hình phí cố định.
 565→
 566→## Validation Rule / Data Mapping
 567→
 568→| **Field**             | **Type**   | **Description** |
 569→|-----------------------|-----------|-----------------|
 570→| Tìm kiếm              | TextBox   | Nhập từ khoá để lọc bản ghi tài chính theo tên phòng/khách; binding thuộc tính tìm kiếm trong ViewModel. |
 571→| Sắp xếp               | ComboBox  | Chọn tiêu chí sắp xếp (thời gian, số tiền,...); binding `SortOrder`. |
 572→| Đồng bộ G.Form        | Button    | Gọi command đồng bộ dữ liệu từ Google Form (theo logic trong ViewModel). |
 573→| Tiền nước             | TextBox   | Thiết lập mức phí nước cố định; binding property cấu hình trong ViewModel. |
 574→| Tiền internet         | TextBox   | Thiết lập phí internet cố định; binding property cấu hình. |
 575→| Tiền vệ sinh          | TextBox   | Thiết lập phí vệ sinh cố định; binding property cấu hình. |
 576→| Danh sách bản ghi     | ItemsControl | Binding collection bản ghi tài chính, mỗi card hiển thị thông tin phòng, tháng, tổng tiền, trạng thái. |
 577→| Menu ngữ cảnh trên card| ContextMenu | Các lệnh "Xem chi tiết", "Đánh dấu đã trả", "Chỉnh sửa", "Xóa" mapping tới command tương ứng với bản ghi hiện tại. |
 578→| Loading overlay       | Overlay   | Binding cờ IsLoading trong ViewModel để hiển thị tiến trình xử lý. |
 579→
 580→---
 581→
 582→# Thêm thông tin thanh toán (AddBillingInfoWindow)
 583→
 584→## Main Flow
 585→
 586→| **Screen**      | Thêm thông tin thanh toán |
 587→|-----------------|---------------------------|
 588→| **Description** | Màn hình nhập chi tiết hoá đơn thanh toán theo hợp đồng/phòng. |
 589→| **Screen Access** | Được mở từ `FinancialWindow` hoặc `PaymentListView` khi tạo bản ghi thanh toán mới. |
 590→
 591→## User Interface
 592→
 593→- `ComboBox` chọn khách/khách thuê.
 594→- `ComboBox` chọn hợp đồng hoặc phòng (MaHopDong, TenPhong).
 595→- `TextBlock` hiển thị tên phòng (read-only).
 596→- Nhóm thông tin tiền thuê và chỉ số điện:
 597→  - `TextBox` Tiền thuê.
 598→  - `TextBox` Chỉ số điện hiện tại.
 599→  - `TextBox` Đơn giá điện.
 600→- Nhóm nước/internet/vệ sinh/gửi xe/chi phí khác:
 601→  - `TextBox` Tiền nước (cố định hoặc theo đơn giá).
 602→  - `TextBox` Đơn giá nước.
 603→  - `TextBox` Internet.
 604→  - `TextBox` Vệ sinh.
 605→  - `TextBox` Gửi xe.
 606→  - `TextBox` Chi phí khác.
 607→- `DatePicker` Ngày thanh toán.
 608→- Nhóm tuỳ chọn thông báo:
 609→  - `CheckBox` Gửi thông báo trực tiếp.
 610→  - `CheckBox` Gửi thông báo qua email.
 611→- Footer: nút `Hủy` và `Thêm thông tin`.
 612→
 613→- **Screenshot**: chụp **AddBillingInfoWindow** với đầy đủ các trường được điền ví dụ.
 614→
 615→## Validation Rule / Data Mapping
 616→
 617→| **Field**             | **Type**    | **Description** |
 618→|-----------------------|------------|-----------------|
 619→| Khách/Khách thuê      | ComboBox   | Chọn khách thuê áp dụng hoá đơn; ItemsSource danh sách khách, binding SelectedItem. |
 620→| Hợp đồng/Phòng        | ComboBox   | Chọn hợp đồng hoặc phòng đang thuê, binding SelectedItem; dùng để tính toán các khoản phí. |
 621→| Tiền thuê             | TextBox    | Số tiền thuê phòng; binding property trong ViewModel, có thể lấy mặc định từ hợp đồng. |
 622→| Chỉ số điện           | TextBox    | Chỉ số công tơ điện hiện tại; binding property, phục vụ tính tiền điện. |
 623→| Đơn giá điện          | TextBox    | Đơn giá/kWh; binding. |
 624→| Tiền nước/Đơn giá nước| TextBox    | Số tiền hoặc đơn giá nước; binding; tuỳ mô hình tính. |
 625→| Internet/Vệ sinh/Gửi xe/Khác | TextBox | Các khoản phụ phí khác; binding các property tương ứng. |
 626→| Ngày thanh toán       | DatePicker | Ngày tạo bản ghi thanh toán; binding `PaymentDate`. |
 627→| Gửi thông báo trực tiếp| CheckBox  | Tuỳ chọn gửi thông báo trực tiếp; binding cờ boolean. |
 628→| Gửi thông báo email   | CheckBox   | Tuỳ chọn gửi email; binding cờ boolean. |
 629→| Thêm thông tin        | Button     | Gọi command lưu bản ghi thanh toán mới sau khi validate dữ liệu. |
 630→| Hủy                   | Button     | Đóng cửa sổ mà không lưu. |
 631→
 632→---
 633→
 634→# Danh sách thanh toán (PaymentListView)
 635→
 636→## Main Flow
 637→
 638→| **Screen**      | Danh sách thanh toán |
 639→|-----------------|-----------------------|
 640→| **Description** | Màn hình quản lý các phiếu thanh toán tiền thuê, hiển thị trạng thái đã/ chưa trả. |
 641→| **Screen Access** | Được mở từ menu quản lý tài chính hoặc từ dashboard. |
 642→
 643→## User Interface
 644→
 645→- Sidebar/Topbar giống các màn quản lý khác.
 646→- Ô tìm kiếm thanh toán.
 647→- `Button` Thêm thanh toán.
 648→- `DataGrid` danh sách thanh toán với các cột:
 649→  - Mã thanh toán.
 650→  - Phòng.
 651→  - Ngày thanh toán.
 652→  - Tổng tiền.
 653→  - Trạng thái (Đã trả/Chưa trả/Trả một phần).
 654→  - Cột thao tác: Xem chi tiết, Chỉnh sửa, Xóa.
 655→- Khi trạng thái là "Trả một phần" sẽ có thêm ô nhập số tiền đã trả.
 656→- Phân trang.
 657→
 658→- **Screenshot**: chụp **PaymentListView** khi DataGrid có đủ các trạng thái thanh toán khác nhau.
 659→
 660→## Validation Rule / Data Mapping
 661→
 662→| **Field**          | **Type**        | **Description** |
 663→|--------------------|----------------|-----------------|
 664→| Tìm kiếm           | TextBox        | Lọc danh sách thanh toán theo mã/ phòng/ khách; binding thuộc tính tìm kiếm. |
 665→| Thêm thanh toán    | Button         | Mở `AddBillingInfoWindow` hoặc màn thêm thanh toán mới. |
 666→| Mã thanh toán      | DataGrid Column| Binding `MaThanhToan`. |
 667→| Phòng              | DataGrid Column| Binding tên/mã phòng. |
 668→| Ngày thanh toán    | DataGrid Column| Binding `NgayThanhToan`. |
 669→| Tổng tiền          | DataGrid Column| Binding `TongTien`. |
 670→| Trạng thái         | DataGrid Column (ComboBox/Template) | Cho phép chọn trạng thái Đã trả/Chưa trả/Trả một phần; binding thuộc tính trạng thái. |
 671→| Số tiền đã trả (trạng thái trả một phần) | TextBox | Binding số tiền đã thanh toán; chỉ hiển thị khi trạng thái là Trả một phần. |
 672→| Thao tác           | DataGrid Column (Buttons) | Các nút xem chi tiết (mở `InvoiceDetailView`), chỉnh sửa, xóa. |

---

# Hoá đơn chi tiết (InvoiceDetailView)

## Main Flow

| **Screen**      | Hoá đơn chi tiết |
|-----------------|------------------|
| **Description** | Màn hình hiển thị chi tiết một hoá đơn thanh toán cho khách thuê. |
| **Screen Access** | Được mở từ `PaymentListView` hoặc `FinancialWindow` khi chọn xem chi tiết. |

## User Interface

- Thông tin tiêu đề hoá đơn: logo/nhãn, mã hoá đơn, ngày tạo, ngày thanh toán, trạng thái.
- Thông tin khách thuê và phòng: tên khách, phòng, địa chỉ, thông tin liên hệ.
- Bảng chi tiết các khoản phí:
  - Tiền thuê.
  - Tiền điện (đơn giá, số kWh, thành tiền).
  - Tiền nước (đơn giá, số khối, thành tiền).
  - Internet, vệ sinh, các khoản khác.
- Tổng cộng, ghi chú.
- Khu QR thanh toán: hình ảnh QR code.
- Các nút hành động:
  - `Button` Tải xuống (Download).
  - `Button` Gửi đến khách hàng.
- Một số ô đơn giá có thể chỉnh sửa thông qua thao tác double-click (theo XAML).

- **Screenshot**: chụp **InvoiceDetailView** với hoá đơn đầy đủ thông tin và QR code hiển thị.

## Validation Rule / Data Mapping

| **Field**          | **Type**        | **Description** |
|--------------------|----------------|-----------------|
| Thông tin khách/phòng | TextBlocks   | Binding từ đối tượng hoá đơn/khách, bao gồm tên khách, phòng, liên hệ. |
| Bảng chi tiết phí  | DataGrid/ItemsControl | Binding collection các dòng chi tiết (tiền thuê, điện, nước, Internet, vệ sinh, khác) với đơn giá, số lượng, thành tiền. |
| Đơn giá chỉnh sửa  | TextBox        | Một số cột đơn giá cho phép chỉnh sửa (double-click); binding hai chiều để cập nhật lại tổng tiền. |
| Tổng cộng          | TextBlock      | Binding tổng thành tiền đã cộng các khoản phí. |
| QR thanh toán      | Image          | Hiển thị hình QR code; nguồn từ đường dẫn/byte trong ViewModel. |
| Nút Download       | Button         | Gọi command xuất hoá đơn ra file (PDF/ảnh) tuỳ implement. |
| Nút Gửi khách hàng | Button         | Gọi command gửi hoá đơn cho khách (email/nhắn tin) tuỳ implement. |

---

# Phiếu chi phí (ExpenseFormWindow)

## Main Flow

| **Screen**      | Phiếu chi phí |
|-----------------|---------------|
| **Description** | Màn hình ghi nhận các khoản chi phí liên quan tới một thanh toán hoặc phòng. |
| **Screen Access** | Được mở từ `FinancialWindow` hoặc `PaymentListView` khi thêm chi phí. |

## User Interface

- `ComboBox` chọn thanh toán liên quan (nếu có).
- `ComboBox` Loại chi phí.
- `TextBox` Số tiền.
- `DatePicker` Ngày chi.
- `TextBox` Mô tả.
- Khu hiển thị thông tin thanh toán liên quan (nếu đã chọn).
- `TextBox` Ghi chú.
- Footer: nút `Hủy` và `Lưu chi phí`.

- **Screenshot**: chụp **ExpenseFormWindow** với đầy đủ các trường và một phiếu chi phí mẫu.

## Validation Rule / Data Mapping

| **Field**        | **Type**    | **Description** |
|------------------|------------|-----------------|
| Thanh toán liên quan | ComboBox | Chọn bản ghi thanh toán để gắn chi phí; binding collection payments và SelectedItem. |
| Loại chi phí    | ComboBox   | Chọn loại chi phí (sửa chữa, vật tư, khác...); binding ItemsSource và SelectedItem. |
| Số tiền         | TextBox    | Số tiền chi; bắt buộc nhập; binding property số tiền. |
| Ngày chi        | DatePicker | Ngày phát sinh chi phí; binding property ngày. |
| Mô tả           | TextBox    | Mô tả nội dung chi phí; binding. |
| Ghi chú         | TextBox    | Ghi chú thêm; binding. |
| Lưu chi phí     | Button     | Gọi command validate và lưu phiếu chi phí. |
| Hủy             | Button     | Đóng cửa sổ mà không lưu. |

---

# Nhập tay công nợ (ManualInputView)

## Main Flow

| **Screen**      | Nhập tay công nợ |
|-----------------|------------------|
| **Description** | Màn hình nhập và điều chỉnh thủ công tiền điện, nước và công nợ cho một phòng. |
| **Screen Access** | Được mở từ `FinancialWindow` hoặc `PaymentListView` cho từng phòng/thanh toán. |

## User Interface

- Thông tin phòng/khách thuê ở phần đầu.
- Khu "Điện tiêu dùng":
  - `TextBox` Đơn giá.
  - `TextBox` Số kWh.
  - `TextBlock` Tạm tính.
- Khu "Nước sinh hoạt":
  - `TextBox` Đơn giá.
  - `TextBox` Số khối.
  - `TextBlock` Tạm tính.
- Khu "Công nợ":
  - `TextBlock` Tổng tạm tính.
  - `TextBox` Số tiền khách trả.
  - `TextBlock` Số tiền còn nợ.
- Nút `Đóng`.

- **Screenshot**: chụp **ManualInputView** với dữ liệu mẫu hiển thị rõ ba khối Điện, Nước, Công nợ.

## Validation Rule / Data Mapping

| **Field**        | **Type**   | **Description** |
|------------------|-----------|-----------------|
| Đơn giá điện     | TextBox   | Nhập đơn giá tiền điện; binding, dùng tính tiền điện. |
| Số kWh           | TextBox   | Nhập số kWh tiêu thụ; binding; dùng tính tiền điện. |
| Đơn giá nước     | TextBox   | Nhập đơn giá nước; binding; dùng tính tiền nước. |
| Số khối          | TextBox   | Nhập số khối nước; binding. |
| Tạm tính điện/nước | TextBlock| Binding giá trị tính toán từ Đơn giá * Số lượng trong ViewModel. |
| Tổng tạm tính    | TextBlock | Tổng tiền điện + nước + các khoản liên quan; binding. |
| Số tiền khách trả| TextBox   | Nhập số tiền khách thanh toán; binding; dùng tính còn nợ. |
| Số tiền còn nợ   | TextBlock | Binding kết quả công nợ sau khi trừ số tiền khách trả. |
| Đóng             | Button    | Đóng cửa sổ, lưu/không lưu tuỳ implement của ViewModel. |

---

# Danh sách bảo trì (MaintenanceListView)

## Main Flow

| **Screen**      | Danh sách bảo trì |
|-----------------|--------------------|
| **Description** | Màn hình quản lý các yêu cầu bảo trì/sự cố trong nhà trọ. |
| **Screen Access** | Được mở từ menu chính hoặc từ các màn phòng/tài chính. |

## User Interface

- Sidebar/Topbar giống các màn quản lý khác.
- Ô tìm kiếm sự cố.
- `ComboBox` sắp xếp.
- `Button` Thêm bảo trì.
- `DataGrid` danh sách bảo trì với các cột:
  - Sự cố.
  - Mã phòng.
  - Ngày báo cáo.
  - Chi phí.
  - Trạng thái (có thể chỉnh sửa trực tiếp).
  - Cột thao tác Sửa/Xóa.
- Phân trang.

- **Screenshot**: chụp **MaintenanceListView** với nhiều dòng dữ liệu và một vài dòng đang chỉnh sửa trạng thái/chi phí.

## Validation Rule / Data Mapping

| **Field**        | **Type**        | **Description** |
|------------------|----------------|-----------------|
| Tìm kiếm         | TextBox        | Lọc danh sách sự cố theo từ khoá; binding thuộc tính tìm kiếm. |
| Sắp xếp          | ComboBox       | Chọn tiêu chí sắp xếp; binding sort. |
| Thêm bảo trì     | Button         | Mở form thêm sự cố bảo trì mới (tuỳ implement). |
| Sự cố            | DataGrid Column| Binding mô tả sự cố. |
| Mã phòng         | DataGrid Column| Binding mã phòng liên quan. |
| Ngày báo cáo     | DataGrid Column| Binding ngày báo cáo. |
| Chi phí          | DataGrid Column (TextBox) | Cho phép nhập/chỉnh sửa chi phí; binding hai chiều. |
| Trạng thái       | DataGrid Column (ComboBox/TextBox) | Cho phép chỉnh sửa trạng thái xử lý; binding thuộc tính trạng thái. |
| Thao tác         | DataGrid Column (Buttons) | Các nút Sửa/Xoá gọi command tương ứng. |

---

# Thông tin nhà (HouseInfoWindow)

## Main Flow

| **Screen**      | Thông tin nhà |
|-----------------|---------------|
| **Description** | Màn hình hiển thị và cập nhật thông tin cơ bản của nhà trọ. |
| **Screen Access** | Được mở từ menu profile (ProfileDropDown) hoặc màn hình thiết lập. |

## User Interface

- `TextBox` Mã nhà (read-only).
- `TextBox` Địa chỉ.
- `TextBox` Tỉnh/Thành phố.
- `TextBox` Tổng số phòng.
- `TextBox` Ghi chú.
- `Button` Lưu thông tin.
- Nút đóng (icon `✕`).

- **Screenshot**: chụp **HouseInfoWindow** với dữ liệu thực tế của một nhà trọ.

## Validation Rule / Data Mapping

| **Field**        | **Type** | **Description** |
|------------------|---------|-----------------|
| Mã nhà           | TextBox | Mã nhà hiển thị chỉ đọc; binding ID nhà trong ViewModel. |
| Địa chỉ          | TextBox | Địa chỉ chi tiết; binding property `DiaChi`. |
| Tỉnh/Thành phố   | TextBox | Tỉnh/thành; binding property tương ứng. |
| Tổng số phòng    | TextBox | Số lượng phòng trong nhà; binding. |
| Ghi chú          | TextBox | Ghi chú chung về nhà trọ; binding. |
| Lưu thông tin    | Button  | Gọi command cập nhật thông tin nhà. |
| Đóng             | Button  | Đóng cửa sổ. |

---

# Quét ảnh chỉ số (ScanImageView)

## Main Flow

| **Screen**      | Quét ảnh chỉ số |
|-----------------|-----------------|
| **Description** | Màn hình tải hoặc chụp ảnh công tơ điện/nước để hệ thống xử lý và nhận diện chỉ số. |
| **Screen Access** | Được mở từ `FinancialWindow` hoặc một nút "Quét chỉ số" trong màn hình công nợ. |

## User Interface

- Khu vực chọn ảnh:
  - `Button` Chọn ảnh từ máy.
  - (Tuỳ XAML) khu drag & drop hoặc preview.
- Vùng hiển thị ảnh xem trước (Preview Image).
- Khu kết quả AI (sau khi xử lý):
  - `TextBlock` mô tả trạng thái.
  - `TextBox` chỉ số đọc được (có thể sửa tay).
- Nút `Gửi xử lý AI` / `Nhận diện`.
- Nút `Đồng ý` để áp dụng kết quả sang màn hình gọi đến.
- Nút `Đóng`.

- **Screenshot**: chụp **ScanImageView** khi đã chọn một ảnh công tơ và hiện kết quả nhận diện.

## Validation Rule / Data Mapping

| **Field**         | **Type** | **Description** |
|-------------------|---------|-----------------|
| Chọn ảnh          | Button  | Mở dialog chọn file ảnh, binding command mở file và cập nhật nguồn ảnh trong ViewModel. |
| Ảnh xem trước     | Image   | Nguồn binding tới đường dẫn/bitmap ảnh đã chọn. |
| Gửi xử lý AI      | Button  | Gọi command gửi ảnh tới dịch vụ AI/logic nội bộ để nhận diện chỉ số; cập nhật kết quả về ViewModel. |
| Kết quả chỉ số    | TextBox | Binding hai chiều với chỉ số đọc được; cho phép người dùng chỉnh lại nếu AI đọc sai. |
| ĐỒNG Ý            | Button  | Ghi nhận kết quả chỉ số và trả về cho màn hình trước (hoặc cập nhật vào context thanh toán). |
| Đóng              | Button  | Đóng cửa sổ quét ảnh. |

---

# Kiểm tra ảnh chỉ số (MeterReadingInspectionWindow)

## Main Flow

| **Screen**      | Kiểm tra ảnh chỉ số |
|-----------------|---------------------|
| **Description** | Màn hình đối chiếu kết quả đọc chỉ số từ AI với thông tin hiển thị trên ảnh, cho phép người dùng xác nhận/chỉnh sửa. |
| **Screen Access** | Được mở từ `ScanImageView` sau khi AI trả kết quả, hoặc từ danh sách kiểm định. |

## User Interface

- Panel hiển thị ảnh công tơ (có thể phóng to).
- Panel bên phải hiển thị:
  - `TextBlock` thông tin phòng/khách.
  - `TextBox` chỉ số đọc được.
  - `TextBox` chỉ số cũ.
  - `TextBlock` hoặc `TextBox` chênh lệch.
- `CheckBox` "Xác nhận đúng".
- `TextBox` Ghi chú.
- Nút `Lưu`/`Xác nhận` và `Đóng`.

- **Screenshot**: chụp **MeterReadingInspectionWindow** với ảnh công tơ và giá trị chỉ số mới, cũ, chênh lệch rõ ràng.

## Validation Rule / Data Mapping

| **Field**          | **Type** | **Description** |
|--------------------|---------|-----------------|
| Ảnh công tơ        | Image   | Binding ảnh đã quét từ `ScanImageView`. |
| Chỉ số đọc được    | TextBox | Kết quả từ AI, binding hai chiều cho phép chỉnh sửa. |
| Chỉ số cũ          | TextBlock/TextBox | Giá trị chỉ số trước đó, binding từ lịch sử. |
| Chênh lệch         | TextBlock | Binding giá trị tính từ chỉ số mới - chỉ số cũ. |
| Xác nhận đúng      | CheckBox | Cờ xác nhận kết quả; binding boolean. |
| Ghi chú            | TextBox | Ghi chú kiểm định; binding. |
| Lưu/Xác nhận       | Button  | Gọi command lưu kết quả kiểm định và cập nhật công nợ/chỉ số. |
| Đóng               | Button  | Đóng cửa sổ. |

---

# Popup QR thanh toán (QrPopupWindow)

## Main Flow

| **Screen**      | Popup QR thanh toán |
|-----------------|----------------------|
| **Description** | Popup hiển thị mã QR để khách thanh toán nhanh. |
| **Screen Access** | Được mở từ `InvoiceDetailView` hoặc màn tài chính khi người dùng chọn "Xem QR". |

## User Interface

- `Image` hiển thị QR code.
- `TextBlock` hiển thị số tiền hoặc nội dung chuyển khoản.
- `TextBlock` hiển thị ghi chú (VD: nội dung bắt buộc khi chuyển khoản).
- Nút `Copy` nội dung thanh toán (nếu có trong XAML).
- Nút `Đóng`.

- **Screenshot**: chụp **QrPopupWindow** với QR rõ nét và nội dung chuyển khoản tương ứng.

## Validation Rule / Data Mapping

| **Field**          | **Type** | **Description** |
|--------------------|---------|-----------------|
| QR Image           | Image   | Binding tới nguồn ảnh QR được generate từ thông tin thanh toán. |
| Nội dung thanh toán| TextBlock | Binding text nội dung chuyển khoản (ví dụ: mã hoá đơn, phòng,...). |
| Số tiền            | TextBlock | Binding số tiền tương ứng. |
| Copy nội dung      | Button  | Gọi command copy nội dung thanh toán vào clipboard. |
| Đóng               | Button  | Đóng popup. |

---

# Cửa sổ chat AI (ChatWindow)

## Main Flow

| **Screen**      | Cửa sổ chat AI |
|-----------------|----------------|
| **Description** | Màn hình chat giữa người dùng và AI hỗ trợ quản lý phòng trọ/tài chính. |
| **Screen Access** | Được mở từ biểu tượng/chat icon trên `DashWindow` hoặc menu trợ giúp. |

## User Interface

- Khu danh sách tin nhắn (chat history), hiển thị từng bong bóng chat (user/AI).
- `TextBox` nhập tin nhắn.
- Nút `Gửi`.
- (Tuỳ XAML) nút chèn mẫu câu hỏi nhanh, hoặc icon đính kèm.

- **Screenshot**: chụp **ChatWindow** với vài đoạn hội thoại mẫu giữa người dùng và AI.

## Validation Rule / Data Mapping

| **Field**        | **Type**        | **Description** |
|------------------|----------------|-----------------|
| Danh sách tin nhắn | ItemsControl/ListBox | Binding collection các message (role, content, time). |
| Ô nhập tin nhắn  | TextBox        | Binding text input; clear sau khi gửi. |
| Gửi              | Button         | Gọi command gửi message lên backend/AI và thêm vào danh sách tin nhắn. |
| Tin nhắn AI      | DataTemplate   | Template hiển thị nội dung trả lời của AI; binding từ collection. |

---

# Báo cáo & Thống kê (ReportWindow)

## Main Flow

| **Screen**      | Báo cáo & Thống kê |
|-----------------|---------------------|
| **Description** | Màn hình hiển thị biểu đồ, số liệu tổng hợp về doanh thu, công nợ, tỉ lệ lấp đầy phòng. |
| **Screen Access** | Được mở từ `DashWindow` hoặc menu báo cáo. |

## User Interface

- Bộ lọc báo cáo:
  - `DatePicker` từ ngày/đến ngày hoặc `ComboBox` chọn khoảng thời gian.
  - `ComboBox` chọn loại báo cáo (doanh thu, công nợ, công suất phòng,...).
- Khu thống kê tổng quan (cards):
  - Tổng doanh thu.
  - Tổng công nợ.
  - Số phòng đang được thuê.
- Khu biểu đồ (chart control theo XAML):
  - Biểu đồ cột hoặc đường theo thời gian.
- `Button` Xuất báo cáo (Excel/PDF).

- **Screenshot**: chụp **ReportWindow** với filter đã chọn và một biểu đồ đang hiển thị.

## Validation Rule / Data Mapping

| **Field**        | **Type**        | **Description** |
|------------------|----------------|-----------------|
| Khoảng thời gian | DatePicker/ComboBox | Binding tham số filter để load dữ liệu báo cáo. |
| Loại báo cáo     | ComboBox       | Binding loại báo cáo; thay đổi sẽ reload dữ liệu. |
| Thống kê tổng quan| ItemsControl/Cards | Binding các chỉ số tổng hợp (doanh thu, công nợ, phòng thuê). |
| Biểu đồ          | ChartControl   | Binding series dữ liệu thống kê theo thời gian. |
| Xuất báo cáo     | Button         | Gọi command export dữ liệu ra file (chi tiết trong ViewModel). |

---

# Thông tin & Bảo mật người dùng (UserSecurityWindow)

## Main Flow

| **Screen**      | Thông tin & Bảo mật người dùng |
|-----------------|---------------------------------|
| **Description** | Màn hình cho phép người dùng xem và cập nhật thông tin tài khoản, đổi mật khẩu. |
| **Screen Access** | Được mở từ menu profile (avatar hoặc tên người dùng). |

## User Interface

- Khu thông tin tài khoản:
  - `TextBox` Họ tên.
  - `TextBox` Email (read-only hoặc editable tùy XAML).
  - `TextBox` Số điện thoại.
- Khu đổi mật khẩu:
  - `PasswordBox` Mật khẩu hiện tại.
  - `PasswordBox` Mật khẩu mới.
  - `PasswordBox` Nhập lại mật khẩu mới.
- Nút `Lưu` thông tin.
- Nút `Đổi mật khẩu`.
- Nút `Đóng`.

- **Screenshot**: chụp **UserSecurityWindow** ở trạng thái hiển thị cả thông tin tài khoản và form đổi mật khẩu.

## Validation Rule / Data Mapping

| **Field**            | **Type**     | **Description** |
|----------------------|-------------|-----------------|
| Họ tên               | TextBox     | Binding hai chiều với tên hiển thị của người dùng. |
| Email                | TextBox     | Binding email; có thể chỉ đọc nếu không cho phép đổi. |
| Số điện thoại        | TextBox     | Binding số điện thoại; có thể có validate định dạng. |
| Mật khẩu hiện tại    | PasswordBox | Nhập mật khẩu hiện tại khi đổi mật khẩu. |
| Mật khẩu mới         | PasswordBox | Nhập mật khẩu mới; có thể kiểm tra độ mạnh. |
| Nhập lại mật khẩu mới| PasswordBox | Xác nhận mật khẩu mới phải trùng với trường trên. |
| Lưu thông tin        | Button      | Gọi command lưu cập nhật hồ sơ. |
| Đổi mật khẩu         | Button      | Gọi command đổi mật khẩu; kiểm tra mật khẩu hiện tại và xác nhận mật khẩu mới. |
| Đóng                 | Button      | Đóng cửa sổ. |

---

# Quên mật khẩu - nhập email (ForgotPasswordEmailWindow)

## Main Flow

| **Screen**      | Quên mật khẩu - nhập email |
|-----------------|-----------------------------|
| **Description** | Màn hình yêu cầu người dùng nhập email để nhận OTP hoặc link đặt lại mật khẩu. |
| **Screen Access** | Được mở từ `LoginWindow` khi bấm "Quên mật khẩu". |

## User Interface

- `TextBlock` mô tả: Nhập email đã đăng ký.
- `TextBox` Email.
- Nút `Gửi mã` hoặc `Tiếp tục`.
- Nút `Đóng`/`Quay lại đăng nhập`.

- **Screenshot**: chụp **ForgotPasswordEmailWindow** với một email đã được nhập.

## Validation Rule / Data Mapping

| **Field** | **Type** | **Description** |
|----------|---------|-----------------|
| Email    | TextBox | Email đã đăng ký tài khoản; bắt buộc nhập, validate định dạng email. |
| Gửi mã   | Button  | Gọi command gửi OTP/link đặt lại mật khẩu tới email; chuyển sang màn OTP nếu thành công. |
| Đóng     | Button  | Đóng popup, quay về `LoginWindow`. |

---

# Đăng nhập OTP (OtpLoginWindow)

## Main Flow

| **Screen**      | Đăng nhập OTP |
|-----------------|----------------|
| **Description** | Màn hình cho phép người dùng nhập mã OTP được gửi qua email để xác thực. |
| **Screen Access** | Được mở sau bước nhập email quên mật khẩu hoặc đăng nhập hai lớp (nếu có). |

## User Interface

- `TextBlock` hiển thị email/địa chỉ nhận mã.
- `TextBox`/khung nhập mã OTP (có thể chia 4–6 ô nhỏ tuỳ XAML).
- Nút `Xác nhận`.
- Nút `Gửi lại mã`.
- Nút `Hủy`.

- **Screenshot**: chụp **OtpLoginWindow** khi người dùng đã nhập một mã OTP ví dụ.

## Validation Rule / Data Mapping

| **Field**    | **Type** | **Description** |
|-------------|---------|-----------------|
| OTP         | TextBox(s) | Nhập mã xác thực được gửi qua email; validate độ dài/định dạng. |
| Xác nhận    | Button  | Gọi command kiểm tra mã OTP; nếu đúng thì cho phép tiếp tục (đăng nhập/đặt lại mật khẩu). |
| Gửi lại mã  | Button  | Gọi command gửi lại OTP mới; có thể kèm giới hạn thời gian. |
| Hủy         | Button  | Huỷ quy trình OTP và quay lại màn trước. |

---

# Đặt lại mật khẩu (ResetPasswordWindow)

## Main Flow

| **Screen**      | Đặt lại mật khẩu |
|-----------------|------------------|
| **Description** | Màn hình cho phép người dùng đặt mật khẩu mới sau khi xác thực OTP. |
| **Screen Access** | Được mở sau khi OTP hợp lệ trong quy trình quên mật khẩu. |

## User Interface

- `PasswordBox` Mật khẩu mới.
- `PasswordBox` Nhập lại mật khẩu mới.
- Nút `Lưu mật khẩu`.
- Nút `Hủy`.

- **Screenshot**: chụp **ResetPasswordWindow** khi người dùng nhập mật khẩu mới/nhập lại.

## Validation Rule / Data Mapping

| **Field**            | **Type**     | **Description** |
|----------------------|-------------|-----------------|
| Mật khẩu mới         | PasswordBox | Nhập mật khẩu mới, có thể kèm validate (độ dài, ký tự đặc biệt...). |
| Nhập lại mật khẩu mới| PasswordBox | Phải trùng với "Mật khẩu mới"; validate khi bấm lưu. |
| Lưu mật khẩu         | Button      | Gọi command đặt lại mật khẩu; nếu thành công có thể chuyển về `LoginWindow`. |
| Hủy                  | Button      | Huỷ thao tác đặt lại mật khẩu, đóng cửa sổ. |

---
