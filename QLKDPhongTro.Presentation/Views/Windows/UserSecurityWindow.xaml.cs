using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Utils;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class UserSecurityWindow : Window
    {
        private bool _isClosing = false;

        public UserSecurityWindow()
        {
            InitializeComponent();

            // ViewModel sẽ được tạo tự động bởi XAML
            // Đăng ký sự kiện Loaded để có thể truy cập ViewModel sau khi nó được tạo
            Loaded += UserSecurityWindow_Loaded;
            
            // Đăng ký sự kiện Closing để đánh dấu cửa sổ đang đóng
            Closing += UserSecurityWindow_Closing;
            // Đăng ký sự kiện Closed để kiểm tra và đóng ứng dụng
            Closed += UserSecurityWindow_Closed;
        }

        private void UserSecurityWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isClosing = true;
        }

        private void UserSecurityWindow_Closed(object? sender, EventArgs e)
        {
            // Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }

        private void UserSecurityWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Đăng ký sự kiện từ ViewModel để có thể xóa các ô PasswordBox
            if (DataContext is UserSecurityViewModel viewModel)
            {
                viewModel.PasswordFieldsCleared += OnPasswordFieldsCleared;
            }

            // Gỡ bỏ đăng ký sự kiện khi cửa sổ đóng lại để tránh rò rỉ bộ nhớ
            Unloaded += (s, args) =>
            {
                if (DataContext is UserSecurityViewModel vm)
                {
                    vm.PasswordFieldsCleared -= OnPasswordFieldsCleared;
                }
            };
        }

        // Handler cho TopbarControl events
        private void TopbarControl_MenuButtonClicked(object sender, EventArgs e)
        {
            // Logic chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý logic riêng của UserSecurityWindow nếu cần
        }

        private void TopbarControl_SearchTextChanged(object sender, string searchText)
        {
            // Logic search chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý search logic riêng của UserSecurityWindow nếu cần
        }

        private void TopbarControl_SettingsButtonClicked(object sender, EventArgs e)
        {
            // Logic settings chung đã được chuyển vào TopbarControl.xaml.cs
            // Chỉ xử lý settings logic riêng của UserSecurityWindow nếu cần
        }

        // Handler cho SidebarControl events
        private void SidebarControl_MenuItemClicked(object sender, string menuItem)
        {
            // Logic navigation đã được chuyển vào SidebarControl.xaml.cs
            // Chỉ xử lý logic riêng của UserSecurityWindow nếu cần
        }

        /// <summary>
        /// Xóa nội dung của các ô PasswordBox khi ViewModel yêu cầu.
        /// </summary>
        private void OnPasswordFieldsCleared()
        {
            OldPasswordBox.Clear();
            NewPasswordBox.Clear();
            ConfirmPasswordBox.Clear();
        }

        // --- CÁC PHƯƠNG THỨC NÀY LÀ CẦN THIẾT CHO PASSWORD BOX ---
        // Chúng cập nhật các thuộc tính trong ViewModel mỗi khi người dùng nhập mật khẩu.

        private void OldPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserSecurityViewModel viewModel && sender is System.Windows.Controls.PasswordBox passwordBox)
            {
                viewModel.OldPassword = passwordBox.Password;
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserSecurityViewModel viewModel && sender is System.Windows.Controls.PasswordBox passwordBox)
            {
                viewModel.NewPassword = passwordBox.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserSecurityViewModel viewModel && sender is System.Windows.Controls.PasswordBox passwordBox)
            {
                viewModel.ConfirmPassword = passwordBox.Password;
            }
        }

        // --- KHÔNG CẦN CÁC PHƯƠNG THỨC TextChanged NỮA ---
        // Binding `Mode=TwoWay` trong XAML đã tự động xử lý việc này.

        /// <summary>
        /// Xử lý sự kiện khi người dùng nhấn nút "Chọn hình" để chọn file QR code
        /// </summary>
        private void SelectQrImageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mở dialog chọn file
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Chọn hình ảnh QR code",
                    Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = true
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var selectedFilePath = openFileDialog.FileName;
                    
                    // Kiểm tra file nguồn có tồn tại không
                    if (!File.Exists(selectedFilePath))
                    {
                        MessageBox.Show("File được chọn không tồn tại!", 
                                      "Lỗi", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Error);
                        return;
                    }
                    
                    // Lấy tên file
                    var fileName = Path.GetFileName(selectedFilePath);
                    
                    // Tạo tên file mới với timestamp để tránh trùng lặp
                    var fileExtension = Path.GetExtension(fileName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    var newFileName = $"{fileNameWithoutExtension}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                    
                    // Tìm thư mục source code của project (QLKDPhongTro.Presentation)
                    string projectDirectory = FindProjectDirectory();
                    var imagesDirectory = Path.Combine(projectDirectory, "Resources", "Images");
                    
                    System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Project Directory: {projectDirectory}");
                    System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Images Directory: {imagesDirectory}");
                    
                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(imagesDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(imagesDirectory);
                            System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Đã tạo thư mục: {imagesDirectory}");
                        }
                        catch (Exception dirEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Lỗi khi tạo thư mục: {dirEx.Message}");
                            MessageBox.Show($"Không thể tạo thư mục lưu trữ: {dirEx.Message}", 
                                          "Lỗi", 
                                          MessageBoxButton.OK, 
                                          MessageBoxImage.Error);
                            return;
                        }
                    }
                    
                    // Đường dẫn đầy đủ của file đích
                    var destinationPath = Path.Combine(imagesDirectory, newFileName);
                    
                    System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Source: {selectedFilePath}");
                    System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Destination: {destinationPath}");
                    
                    // Copy file vào thư mục Images
                    try
                    {
                        // Xóa file cũ nếu đã tồn tại
                        if (File.Exists(destinationPath))
                        {
                            File.Delete(destinationPath);
                        }
                        
                        File.Copy(selectedFilePath, destinationPath, overwrite: true);
                        
                        // Kiểm tra file đã được copy thành công chưa
                        if (!File.Exists(destinationPath))
                        {
                            throw new Exception("File không được tạo sau khi copy!");
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Đã copy file thành công!");
                        
                        // Kiểm tra kích thước file
                        var sourceFileInfo = new FileInfo(selectedFilePath);
                        var destFileInfo = new FileInfo(destinationPath);
                        System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Source size: {sourceFileInfo.Length} bytes");
                        System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Destination size: {destFileInfo.Length} bytes");
                        
                        if (destFileInfo.Length == 0)
                        {
                            throw new Exception("File được tạo nhưng có kích thước 0 bytes!");
                        }
                    }
                    catch (Exception copyEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Lỗi khi copy file: {copyEx.Message}");
                        MessageBox.Show($"Không thể copy file: {copyEx.Message}\n\nĐường dẫn đích: {destinationPath}", 
                                      "Lỗi", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Error);
                        return;
                    }
                    
                    // Tạo đường dẫn tương đối để lưu vào database (format: /Resources/Images/filename)
                    var relativePath = $"/Resources/Images/{newFileName}";
                    
                    System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Relative path: {relativePath}");
                    
                    // Cập nhật LinkQr trong ViewModel
                    if (DataContext is UserSecurityViewModel viewModel)
                    {
                        viewModel.LinkQr = relativePath;
                        System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Đã cập nhật LinkQr trong ViewModel: {relativePath}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[SelectQrImage] WARNING: DataContext không phải UserSecurityViewModel!");
                    }
                    
                    MessageBox.Show($"Đã lưu hình ảnh thành công!\n\nTên file: {newFileName}\nĐường dẫn: {relativePath}\n\nVui lòng nhấn 'Cập nhật hồ sơ' để lưu vào database.", 
                                  "Thành công", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SelectQrImage] Lỗi tổng quát: {ex.Message}\nStack trace: {ex.StackTrace}");
                MessageBox.Show($"Lỗi khi chọn và lưu hình ảnh: {ex.Message}\n\nChi tiết: {ex}", 
                              "Lỗi", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Tìm thư mục source code của project (QLKDPhongTro.Presentation)
        /// </summary>
        private string FindProjectDirectory()
        {
            try
            {
                // Lấy đường dẫn của assembly hiện tại
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                
                if (string.IsNullOrEmpty(assemblyDirectory))
                {
                    // Fallback: sử dụng AppDomain.CurrentDomain.BaseDirectory
                    assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }
                
                System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Assembly Location: {assemblyLocation}");
                System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Assembly Directory: {assemblyDirectory}");
                
                // Đi lên các cấp thư mục để tìm thư mục chứa file .csproj
                var currentDir = new DirectoryInfo(assemblyDirectory);
                
                while (currentDir != null)
                {
                    // Kiểm tra xem có file .csproj trong thư mục này không
                    var csprojFiles = currentDir.GetFiles("*.csproj", SearchOption.TopDirectoryOnly);
                    if (csprojFiles.Length > 0)
                    {
                        // Kiểm tra xem có thư mục Resources/Images không
                        var resourcesPath = Path.Combine(currentDir.FullName, "Resources", "Images");
                        if (Directory.Exists(resourcesPath) || currentDir.Name == "QLKDPhongTro.Presentation")
                        {
                            System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Found project directory: {currentDir.FullName}");
                            return currentDir.FullName;
                        }
                    }
                    
                    // Đi lên một cấp
                    currentDir = currentDir.Parent;
                }
                
                // Nếu không tìm thấy, thử tìm thư mục có tên "QLKDPhongTro.Presentation"
                currentDir = new DirectoryInfo(assemblyDirectory);
                while (currentDir != null)
                {
                    if (currentDir.Name == "QLKDPhongTro.Presentation")
                    {
                        System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Found by name: {currentDir.FullName}");
                        return currentDir.FullName;
                    }
                    currentDir = currentDir.Parent;
                }
                
                // Fallback cuối cùng: sử dụng thư mục hiện tại và đi lên 3 cấp (bin/Debug/net8.0-windows -> QLKDPhongTro.Presentation)
                var fallbackDir = new DirectoryInfo(assemblyDirectory);
                for (int i = 0; i < 3 && fallbackDir != null; i++)
                {
                    fallbackDir = fallbackDir.Parent;
                }
                
                if (fallbackDir != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Using fallback: {fallbackDir.FullName}");
                    return fallbackDir.FullName;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Error: {ex.Message}");
            }
            
            // Fallback cuối cùng: sử dụng AppDomain.CurrentDomain.BaseDirectory
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            System.Diagnostics.Debug.WriteLine($"[FindProjectDirectory] Using AppDomain.BaseDirectory: {baseDir}");
            return baseDir;
        }
    }
}