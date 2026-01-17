using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;
using QLKDPhongTro.Presentation.ViewModels;
using QLKDPhongTro.Presentation.Utils;

namespace QLKDPhongTro.Presentation.Views.Windows_User
{
    public partial class UserProfileWindow : Window
    {
        private bool _isClosing = false;

        public UserProfileWindow()
        {
            InitializeComponent();

            Loaded += UserProfileWindow_Loaded;
            Closing += UserProfileWindow_Closing;
            Closed += UserProfileWindow_Closed;
        }

        private void UserProfileWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isClosing = true;
        }

        private void UserProfileWindow_Closed(object? sender, EventArgs e)
        {
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }

        private void UserProfileWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserProfileViewModel viewModel)
            {
                viewModel.PasswordFieldsCleared += OnPasswordFieldsCleared;
            }

            Unloaded += (s, args) =>
            {
                if (DataContext is UserProfileViewModel vm)
                {
                    vm.PasswordFieldsCleared -= OnPasswordFieldsCleared;
                }
            };
        }

        private void OnPasswordFieldsCleared()
        {
            OldPasswordBox.Clear();
            NewPasswordBox.Clear();
            ConfirmPasswordBox.Clear();
        }

        private void OldPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserProfileViewModel viewModel && sender is System.Windows.Controls.PasswordBox passwordBox)
            {
                viewModel.OldPassword = passwordBox.Password;
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserProfileViewModel viewModel && sender is System.Windows.Controls.PasswordBox passwordBox)
            {
                viewModel.NewPassword = passwordBox.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserProfileViewModel viewModel && sender is System.Windows.Controls.PasswordBox passwordBox)
            {
                viewModel.ConfirmPassword = passwordBox.Password;
            }
        }



        private string FindProjectDirectory()
        {
            try
            {
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                
                if (string.IsNullOrEmpty(assemblyDirectory))
                {
                    assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }
                
                var currentDir = new DirectoryInfo(assemblyDirectory);
                
                while (currentDir != null)
                {
                    var csprojFiles = currentDir.GetFiles("*.csproj", SearchOption.TopDirectoryOnly);
                    if (csprojFiles.Length > 0)
                    {
                        var resourcesPath = Path.Combine(currentDir.FullName, "Resources", "Images");
                        if (Directory.Exists(resourcesPath) || currentDir.Name == "QLKDPhongTro.Presentation")
                        {
                            return currentDir.FullName;
                        }
                    }
                    currentDir = currentDir.Parent;
                }
                
                currentDir = new DirectoryInfo(assemblyDirectory);
                while (currentDir != null)
                {
                    if (currentDir.Name == "QLKDPhongTro.Presentation")
                    {
                        return currentDir.FullName;
                    }
                    currentDir = currentDir.Parent;
                }
                
                var fallbackDir = new DirectoryInfo(assemblyDirectory);
                for (int i = 0; i < 3 && fallbackDir != null; i++)
                {
                    fallbackDir = fallbackDir.Parent;
                }
                
                if (fallbackDir != null)
                {
                    return fallbackDir.FullName;
                }
            }
            catch
            {
                // Ignore error
            }
            
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
