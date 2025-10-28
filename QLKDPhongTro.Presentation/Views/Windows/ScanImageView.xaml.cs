using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ScanImageView : Window
    {
        public IReadOnlyList<string> SelectedFilePaths { get; private set; } = new List<string>();

        public ScanImageView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                // Chỉ nhận các tệp hình ảnh phổ biến
                var allowedExt = new HashSet<string>(new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" }, System.StringComparer.OrdinalIgnoreCase);
                var files = paths.Where(p => File.Exists(p) && allowedExt.Contains(Path.GetExtension(p))).ToList();
                if (files.Count > 0)
                {
                    SelectedFilePaths = files;
                    this.DialogResult = true;
                    this.Close();
                }
            }
        }

        private void DropZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn ảnh để quét",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp",
                Multiselect = true
            };

            if (dlg.ShowDialog(this) == true)
            {
                SelectedFilePaths = dlg.FileNames.ToList();
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}


