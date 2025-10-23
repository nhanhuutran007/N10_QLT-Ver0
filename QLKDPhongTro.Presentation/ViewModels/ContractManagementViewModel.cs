using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using QLKDPhongTro.Presentation.Views.Windows;
// using QLKDPhongTro.Presentation.Commands;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
    public class ContractManagementViewModel : INotifyPropertyChanged
    {
        private string _searchText;
        private ObservableCollection<ContractViewModel> _contracts;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ContractViewModel> Contracts
        {
            get => _contracts;
            set
            {
                _contracts = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddNewContractCommand { get; }
        public ICommand DeleteContractCommand { get; }
        public ICommand EditContractCommand { get; }

        public ContractManagementViewModel()
        {
            Contracts = new ObservableCollection<ContractViewModel>();
            AddNewContractCommand = new RelayCommand(AddNewContract);
            DeleteContractCommand = new RelayCommand<ContractViewModel>(DeleteContract);
            EditContractCommand = new RelayCommand<ContractViewModel>(EditContract);
            
            LoadContracts();
        }

        public void LoadContracts()
        {
            // Load sample contract data
            Contracts.Clear();
            
            // Sample data
            Contracts.Add(new ContractViewModel
            {
                ContractName = "HD_001_2024.pdf",
                TenantName = "Nguyễn Văn A",
                CreatedDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddMonths(11),
                ContractId = 1
            });

            Contracts.Add(new ContractViewModel
            {
                ContractName = "HD_002_2024.pdf",
                TenantName = "Trần Thị B",
                CreatedDate = DateTime.Now.AddDays(-15),
                EndDate = DateTime.Now.AddMonths(5),
                ContractId = 2
            });

            Contracts.Add(new ContractViewModel
            {
                ContractName = "HD_003_2024.pdf",
                TenantName = "Lê Văn C",
                CreatedDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddMonths(8),
                ContractId = 3
            });
        }

        public void FilterContracts()
        {
            // Implement search/filter logic here
            if (string.IsNullOrEmpty(SearchText))
            {
                LoadContracts();
            }
            else
            {
                // Filter contracts based on search text
                // This is a simple implementation - you can enhance it
            }
        }

        private void AddNewContract()
        {
            // Open add contract dialog
            var addContractWindow = new AddContractWindow();
            if (addContractWindow.ShowDialog() == true)
            {
                LoadContracts(); // Refresh the list
            }
        }

        private void DeleteContract(ContractViewModel contract)
        {
            if (contract != null)
            {
                Contracts.Remove(contract);
                // TODO: Delete from database
            }
        }

        private void EditContract(ContractViewModel contract)
        {
            if (contract != null)
            {
                // Open edit contract dialog
                var editContractWindow = new EditContractWindow(contract);
                if (editContractWindow.ShowDialog() == true)
                {
                    LoadContracts(); // Refresh the list
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ContractViewModel : INotifyPropertyChanged
    {
        private string _contractName;
        private string _tenantName;
        private DateTime _createdDate;
        private DateTime _endDate;
        private int _contractId;

        public string ContractName
        {
            get => _contractName;
            set
            {
                _contractName = value;
                OnPropertyChanged();
            }
        }

        public string TenantName
        {
            get => _tenantName;
            set
            {
                _tenantName = value;
                OnPropertyChanged();
            }
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                _createdDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public int ContractId
        {
            get => _contractId;
            set
            {
                _contractId = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}