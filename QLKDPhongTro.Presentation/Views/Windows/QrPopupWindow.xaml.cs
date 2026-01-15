using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class QrPopupWindow : Window
    {
        public QrPopupWindow()
        {
            InitializeComponent();
            this.KeyDown += QrPopupWindow_KeyDown;
            this.MouseDown += QrPopupWindow_MouseDown;
        }

        private void QrPopupWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void QrPopupWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
