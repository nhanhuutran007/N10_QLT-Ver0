using System;
using System.Drawing;
using System.Windows.Forms;
using QLKDPhongTro.Controllers;

namespace QLKDPhongTro.Views
{
    public partial class frmDangKy : Form
    {
        private AuthController authController;

        public frmDangKy()
        {
            InitializeComponent();
            authController = new AuthController(this);
        }

        private async void btnDangKy_Click(object sender, EventArgs e)
        {
            await authController.RegisterAsync(
                txtTenDangNhap.Text,
                txtEmail.Text,
                txtMatKhau.Text,
                txtXacNhanMatKhau.Text
            );
        }

        private void lblDangNhap_Click(object sender, EventArgs e)
        {
            authController.NavigateToLogin();
        }

        private void txtMatKhau_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordValidation();
        }

        private void UpdatePasswordValidation()
        {
            string password = txtMatKhau.Text;
            
            // Cập nhật màu sắc của các yêu cầu mật khẩu
            lblMinLength.ForeColor = password.Length >= 6 ? Color.Green : Color.Red;
            lblHasUpper.ForeColor = HasUpperCase(password) ? Color.Green : Color.Red;
            lblHasDigit.ForeColor = HasDigit(password) ? Color.Green : Color.Red;
        }

        private bool HasUpperCase(string text)
        {
            foreach (char c in text)
            {
                if (char.IsUpper(c))
                    return true;
            }
            return false;
        }

        private bool HasDigit(string text)
        {
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                    return true;
            }
            return false;
        }

        private void lblWelcome_Click(object sender, EventArgs e)
        {

        }
    }
}
