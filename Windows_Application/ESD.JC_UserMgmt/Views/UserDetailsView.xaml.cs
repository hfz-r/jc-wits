using ESD.JC_UserMgmt.ViewModels;
using System.Windows.Controls;

namespace ESD.JC_UserMgmt.Views
{
    /// <summary>
    /// Interaction logic for UserDetailsView.xaml
    /// </summary>
    public partial class UserDetailsView : UserControl
    {
        public UserDetailsView(UserDetailsViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
