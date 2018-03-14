using ESD.JC_Infrastructure.Controls;
using ESD.JC_Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Threading;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class FGMainViewModel : BindableBase
    {
        private ICompositeCommands _applicationCommands;
        public ICompositeCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }

        private string _FilterTextBox;
        public string FilterTextBox
        {
            get { return _FilterTextBox; }
            set
            {
                SetProperty(ref _FilterTextBox, value);
                EventAggregator.GetEvent<FilterTextBoxEvent>().Publish(_FilterTextBox);
            }
        }

        private object _ImportBtn = null;
        public object ImportBtn
        {
            get { return _ImportBtn; }
            set { SetProperty(ref _ImportBtn, value); }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                    return Thread.CurrentPrincipal.Identity.Name;

                return "Unauthorized User";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.IsAuthenticated;
            }
        }

        private IEventAggregator EventAggregator;

        public FGMainViewModel(ICompositeCommands applicationCommands, IEventAggregator EventAggregator)
        {
            ApplicationCommands = applicationCommands;
            this.EventAggregator = EventAggregator;
            this.EventAggregator.GetEvent<ObjectEvent>().Subscribe(InitImportButton);

            OnLoadedCommand = new DelegateCommand(OnLoaded);
        }

        public DelegateCommand OnLoadedCommand { get; private set; }

        private void OnLoaded()
        {
            if (!IsAuthenticated)
                return;
            
            EventAggregator.GetEvent<FGUserSelectedEvent>().Publish(0);
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Publish(AuthenticatedUser);
        }

        private void InitImportButton(object obj)
        {
            ImportBtn = obj;
            RaisePropertyChanged("ImportBtn");
        }
    }
}
