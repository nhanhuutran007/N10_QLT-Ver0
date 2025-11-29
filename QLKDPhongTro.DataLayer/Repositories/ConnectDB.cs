using MySql.Data.MySqlClient;

namespace QLKDPhongTro.DataLayer.Repositories
{
    /// <summary>
    /// Class quản lý kết nối database chung cho tất cả repositories
    /// </summary>
    public static class ConnectDB
    {
        private static string? _connectionString;

        /// <summary>
        /// Lấy connection string cho MySQL database
        /// </summary>
        public static string GetConnectionString()
        {
            if (_connectionString != null)
                return _connectionString;

            //// ===== CẤU HÌNH KẾT NỐI MYSQL =====
            string server = "host80.vietnix.vn";
            string database = "githubio_QLT_Ver1";
            string username = "githubio_admin";
            string password = "nhanhuutran007";
            string port = "3306";

            //// Thêm charset=utf8mb4 để đảm bảo MySQL nhận diện đúng ký tự tiếng Việt trong ENUM
            //_connectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};SslMode=Preferred;CharSet=utf8mb4;";
            // Cấu hình cho Localhost (XAMPP/WAMP/MySQL Workbench)
            //string server = "localhost";          // Hoặc dùng "127.0.0.1" đều được
            //string database = "githubio_qlt_ver2"; // Tên database mới theo script Ver2
            //string username = "root";             // Mặc định của Localhost là 'root', không phải 'admin'
            //string password = "";                 // Mặc định thường để trống (chuỗi rỗng)
            //string port = "3306";                 // Cổng mặc định

            // Tạo chuỗi kết nối đầy đủ (Connection String)
            _connectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};Charset=utf8mb4;";
            return _connectionString;
        }

        /// <summary>
        /// Tạo và mở connection với charset utf8mb4 đã được set
        /// </summary>
        public static async Task<MySqlConnection> CreateConnectionAsync()
        {
            var connection = new MySqlConnection(GetConnectionString());
            await connection.OpenAsync();

            // Đảm bảo connection sử dụng charset utf8mb4 để nhận diện đúng ký tự tiếng Việt
            // Chỉ set charset, không set COLLATE để tránh lỗi với các cột có CHARACTER SET khác
            var setCharsetCmd = new MySqlCommand("SET NAMES utf8mb4", connection);
            await setCharsetCmd.ExecuteNonQueryAsync();

            return connection;
        }

        /// <summary>
        /// Reset connection string (dùng khi cần thay đổi cấu hình)
        /// </summary>
        public static void ResetConnectionString()
        {
            _connectionString = null;
        }
    }
}


