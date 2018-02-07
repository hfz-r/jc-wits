using System.Windows.Controls;
using ESD.JC_UserMgmt.ViewModels;
using System.Security.Permissions;
using System.Windows;

namespace ESD.JC_UserMgmt.Views
{
    /// <summary>
    /// Interaction logic for UserOperationView.xaml
    /// </summary>
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class UserOperationView : UserControl
    {
        public UserOperationView(UserOperationViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        private void PasswordBoxText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UserOperationViewModel.Password = PasswordBoxText.Password;
            //((PasswordBox)sender).Password;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }
    }
}
