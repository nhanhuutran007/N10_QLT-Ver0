using System;
using System.Windows;
using QLKDPhongTro.Presentation.ViewModels;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class EditContractWindow : Window
    {
        private ContractViewModel _contract;

        public EditContractWindow(ContractViewModel contract)
        {
            InitializeComponent();
            _contract = contract;
            LoadContractData();
        }

        private void LoadContractData()
        {
            if (_contract != null)
            {
                ContractNameTextBox.Text = _contract.ContractName;
                TenantNameTextBox.Text = _contract.TenantName;
                StartDatePicker.SelectedDate = _contract.CreatedDate;
                EndDatePicker.SelectedDate = _contract.EndDate;
                // TODO: Load other fields from database
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(ContractNameTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập tên hợp đồng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TenantNameTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập tên người thuê!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StartDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày kết thúc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StartDatePicker.SelectedDate >= EndDatePicker.SelectedDate)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update contract data
            if (_contract != null)
            {
                _contract.ContractName = ContractNameTextBox.Text;
                _contract.TenantName = TenantNameTextBox.Text;
                _contract.CreatedDate = StartDatePicker.SelectedDate.Value;
                _contract.EndDate = EndDatePicker.SelectedDate.Value;
            }

            // TODO: Save to database
            MessageBox.Show("Hợp đồng đã được cập nhật thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
