using System;
using System.Configuration;

namespace QLKDPhongTro.Config
{
    /// <summary>
    /// Cấu hình kết nối cơ sở dữ liệu
    /// </summary>
    public static class DatabaseConfig
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
                    ?? "Data Source=.;Initial Catalog=QuanLyPhongTro;Integrated Security=True";
            }
        }
    }
}
