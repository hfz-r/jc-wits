using System.Windows;
using ESD.JC_Main.ViewModels;

namespace ESD.JC_Main.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void PasswordBoxText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LoginWindowViewModel.Password = PasswordBoxText.Password;
        }
    }
}
