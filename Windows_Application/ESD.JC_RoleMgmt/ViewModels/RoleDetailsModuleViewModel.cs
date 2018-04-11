using DataLayer;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Events;
using System.Collections.ObjectModel;
using ESD.JC_RoleMgmt.Services;
using Prism.Commands;
using System.Linq;

namespace ESD.JC_RoleMgmt.ViewModels
{
    public class RoleDetailsModuleViewModel : BindableBase
    {
        public string ViewName
        {
            get { return "Module Access"; }
        }

        public ObservableCollection<ModuleAccessCtrlTransaction> modCollection { get; set; }

        private ObservableCollection<ModuleAccessCtrlTrnxExt> _moduleCollectionExt;
        public ObservableCollection<ModuleAccessCtrlTrnxExt> moduleCollectionExt
        {
            get { return _moduleCollectionExt; }
            set
            {
                SetProperty(ref _moduleCollectionExt, value);
                RaisePropertyChanged("moduleCollectionExt");
            }
        }

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IRoleServices roleServices;

        public RoleDetailsModuleViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IRoleServices roleServices)
        {
            this.regionManager = regionManager;
            this.roleServices = roleServices;
            this.eventAggregator = eventAggregator;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
        }

        public DelegateCommand OnLoadedCommand { get; private set; }

        private void OnLoaded()
        {
            moduleCollectionExt = new ObservableCollection<ModuleAccessCtrlTrnxExt>();

            foreach (var item in modCollection.Where(x => x.IsAllow))
            {
                moduleCollectionExt.Add(new ModuleAccessCtrlTrnxExt
                {
                    ID = item.ModuleAccessCtrl.ID,
                    Module = item.ModuleAccessCtrl.Module,
                    IsChecked = item.IsAllow
                });
            }

            RaisePropertyChanged("moduleCollectionExt");
        }
    }

    public class ModuleAccessCtrlTrnxExt
    {
        public long ID { get; set; }
        public string Module { get; set; }
        public bool IsChecked { get; set; }
    }
}
