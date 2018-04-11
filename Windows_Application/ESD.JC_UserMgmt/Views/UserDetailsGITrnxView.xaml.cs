using System.Windows.Controls;
using ESD.JC_UserMgmt.ViewModels;

namespace ESD.JC_UserMgmt.Views
{
    /// <summary>
    /// Interaction logic for UserDetailsGITrnxView.xaml
    /// </summary>
    public partial class UserDetailsGITrnxView : UserControl
    {
        public UserDetailsGITrnxView(UserDetailsGITrnxViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
