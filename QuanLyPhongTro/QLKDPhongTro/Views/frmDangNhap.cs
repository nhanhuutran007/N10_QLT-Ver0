using System;
using System.Windows.Forms;
using QLKDPhongTro.Controllers;

namespace QLKDPhongTro.Views
{
    public partial class frmDangNhap : Form
    {
        private AuthController authController;

        public frmDangNhap()
        {
            InitializeComponent();
            authController = new AuthController(this);
        }

        private async void btnDangNhap_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra thông tin đầu vào
                if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenDangNhap.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMatKhau.Focus();
                    return;
                }

                // Thực hiện đăng nhập
                bool loginSuccess = await authController.LoginAsync(txtTenDangNhap.Text, txtMatKhau.Text);

                if (loginSuccess)
                {
                    // Đăng nhập thành công, form sẽ được đóng trong AuthController
                    // Không cần làm gì thêm ở đây
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblDangKy_Click(object sender, EventArgs e)
        {
            authController.NavigateToRegister();
        }

        private void txtMatKhau_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Cho phép đăng nhập khi nhấn Enter
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnDangNhap_Click(sender, e);
            }
        }

        private void txtTenDangNhap_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Cho phép đăng nhập khi nhấn Enter
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnDangNhap_Click(sender, e);
            }
        }

        private void chkHienThiMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle hiển thị mật khẩu
            if (chkHienThiMatKhau.Checked)
            {
                txtMatKhau.PasswordChar = '\0'; // Hiển thị mật khẩu
            }
            else
            {
                txtMatKhau.PasswordChar = '•'; // Ẩn mật khẩu
            }
        }

        private void lblQuenMatKhau_Click(object sender, EventArgs e)
        {
            // Xử lý quên mật khẩu
            MessageBox.Show("Tính năng quên mật khẩu đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lblWelcome_Click(object sender, EventArgs e)
        {

        }

        private void lblQuenMatKhau_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}