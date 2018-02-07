using DataLayer;
using ESD.JC_GoodsReceive.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System;
using System.ComponentModel;

namespace ESD.JC_GoodsReceive.Views
{
    /// <summary>
    /// Interaction logic for GRMainView.xaml
    /// </summary>
    public partial class GRMainView : UserControl
    {
        public GRMainView(GRMainViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        private void headerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < dgGrid.Items.Count; i++)
            //{
            //    CheckBox cb = new CheckBox();

            //    var x = dgGrid.Items[i];
            //}
            //foreach (GoodsReceive gc in dgGrid.ItemsSource)
            //{
            //    gc.IsChecked = true;
            //}
        }
    }
}