using System;
using System.Windows.Forms;

namespace QLKDPhongTro.Controllers
{
    /// <summary>
    /// Lớp cơ sở cho tất cả các Controller
    /// </summary>
    public abstract class BaseController
    {
        protected Form currentForm;

        public BaseController(Form form)
        {
            currentForm = form;
        }

        /// <summary>
        /// Hiển thị thông báo lỗi
        /// </summary>
        protected void ShowError(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Hiển thị thông báo thành công
        /// </summary>
        protected void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị thông báo cảnh báo
        /// </summary>
        protected void ShowWarning(string message)
        {
            MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Xác nhận trước khi thực hiện hành động
        /// </summary>
        protected bool ConfirmAction(string message)
        {
            return MessageBox.Show(message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
