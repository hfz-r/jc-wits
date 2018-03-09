using ESD.JC_Infrastructure;
using ESD.JC_FinishGoods.Views;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using ESD.JC_Infrastructure.Events;
using ESD.JC_FinishGoods.ViewModels;
using ESD.JC_Infrastructure.Controls;

namespace ESD.JC_FinishGoods.Controllers
{
    public class TabRegionController
    {
        private IUnityContainer container;
        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private ICompositeCommands _applicationCommands;

        public TabRegionController(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, ICompositeCommands _applicationCommands)
        {
            this.container = container;
            this.regionManager = regionManager;
            this._applicationCommands = _applicationCommands;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<FGUserSelectedEvent>().Subscribe(InitTabRegion, true);
        }

        private void InitTabRegion(long id)
        {
            IRegion tabRegion = regionManager.Regions[RegionNames.TabRegionFG];
            if (tabRegion == null) return;

            FGahuView ahuView = tabRegion.GetView("FGahuView") as FGahuView;
            FGfcuView fcuView = tabRegion.GetView("FGfcuView") as FGfcuView;
            if (ahuView == null && fcuView == null)
            {
                ahuView = this.container.Resolve<FGahuView>();
                fcuView = this.container.Resolve<FGfcuView>();

                tabRegion.Add(ahuView, "FGahuView");
                tabRegion.Add(fcuView, "FGfcuView");
            }
            tabRegion.Activate(ahuView);

            #region Generic Composite Buttons

            FGahuViewModel ahuViewModel = ahuView.DataContext as FGahuViewModel;
            FGfcuViewModel fcuViewModel = fcuView.DataContext as FGfcuViewModel;

            var navView = this.container.Resolve<FGNavigationItemView>();
            navView.CloseViewRequested += delegate
            {
                _applicationCommands.ImportFGCommand.UnregisterCommand(ahuViewModel.ImportFGCommand);
                _applicationCommands.ExportFGCommand.UnregisterCommand(ahuViewModel.ExportFGCommand);
                _applicationCommands.PrintLblCommand.UnregisterCommand(ahuViewModel.PrintLblCommand);
                _applicationCommands.OKCommand.UnregisterCommand(ahuViewModel.OKCommand);
                _applicationCommands.XOKCommand.UnregisterCommand(ahuViewModel.XOKCommand);

                _applicationCommands.ImportFGCommand.UnregisterCommand(fcuViewModel.ImportFGCommand);
                _applicationCommands.ExportFGCommand.UnregisterCommand(fcuViewModel.ExportFGCommand);
                _applicationCommands.PrintLblCommand.UnregisterCommand(fcuViewModel.PrintLblCommand);
                _applicationCommands.OKCommand.UnregisterCommand(fcuViewModel.OKCommand);
                _applicationCommands.XOKCommand.UnregisterCommand(fcuViewModel.XOKCommand);
            };

            _applicationCommands.ImportFGCommand.RegisterCommand(ahuViewModel.ImportFGCommand);
            _applicationCommands.ExportFGCommand.RegisterCommand(ahuViewModel.ExportFGCommand);
            _applicationCommands.PrintLblCommand.RegisterCommand(ahuViewModel.PrintLblCommand);
            _applicationCommands.OKCommand.RegisterCommand(ahuViewModel.OKCommand);
            _applicationCommands.XOKCommand.RegisterCommand(ahuViewModel.XOKCommand);

            _applicationCommands.ImportFGCommand.RegisterCommand(fcuViewModel.ImportFGCommand);
            _applicationCommands.ExportFGCommand.RegisterCommand(fcuViewModel.ExportFGCommand);
            _applicationCommands.PrintLblCommand.RegisterCommand(fcuViewModel.PrintLblCommand);
            _applicationCommands.OKCommand.RegisterCommand(fcuViewModel.OKCommand);
            _applicationCommands.XOKCommand.RegisterCommand(fcuViewModel.XOKCommand);

            #endregion Generic Composite Buttons
        }
    }
}
