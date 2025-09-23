using System;
using System.Windows.Forms;
using QLKDPhongTro.Controllers;
using QLKDPhongTro.Models;

namespace QLKDPhongTro.Views
{
    public partial class frmFormApp : Form
    {
        public frmFormApp()
        {
            InitializeComponent();
        }

        private void frmFormApp_Load(object sender, EventArgs e)
        {
            // Hiển thị thông tin user đã đăng nhập
            if (AuthController.CurrentUser != null)
            {
                this.Text = $"Quản lý kinh doanh Phòng Trọ - Chào mừng {AuthController.CurrentUser.TenDangNhap}";
                
                // Có thể thêm thông tin user vào form chính
                // Ví dụ: hiển thị tên user ở góc trên bên phải
            }
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}