using System;
using System.Windows.Forms;
using QLKDPhongTro.Controllers;

namespace QLKDPhongTro.Views
{
    public partial class frmDoiMK : Form
    {
        private AuthController authController;

        public frmDoiMK()
        {
            InitializeComponent();
            authController = new AuthController(this);
        }

        private void btnDongY_Click(object sender, EventArgs e)
        {
            // TODO: Implement change password logic
            MessageBox.Show("Chức năng đổi mật khẩu đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}