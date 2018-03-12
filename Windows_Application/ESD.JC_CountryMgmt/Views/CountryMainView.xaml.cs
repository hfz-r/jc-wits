﻿using System.Windows.Controls;
using ESD.JC_CountryMgmt.ViewModels;
using System.Windows;

namespace ESD.JC_CountryMgmt.Views
{
    /// <summary>
    /// Interaction logic for CountryMainView.xaml
    /// </summary>
    public partial class CountryMainView : UserControl
    {
        private CountryMainViewViewModel m_ViewModel;

        public CountryMainView(CountryMainViewViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        private void ToolBar_Loaded(object sender, System.Windows.RoutedEventArgs e)
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

        /// <summary>
        /// Gets the view model from the data Context and assigns it to a member variable.
        /// </summary>
        private void OnMainGridDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_ViewModel = (CountryMainViewViewModel)this.DataContext;
        }
    }
}