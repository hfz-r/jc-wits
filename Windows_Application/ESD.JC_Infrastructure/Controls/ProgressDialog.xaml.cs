using System;
using System.Windows;

namespace ESD.JC_Infrastructure.Controls
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }
        
        public int ProgressValue
        {
            set
            {
                this.ProgressBar.Value = value;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}
