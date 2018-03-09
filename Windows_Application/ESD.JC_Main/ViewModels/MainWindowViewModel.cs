using Prism.Events;
using Prism.Mvvm;
using ESD.JC_Infrastructure.Events;
using ESD.JC_Main.LoginServices;
using Prism.Commands;
using System.Windows.Input;
using System.Threading;
using System.Windows;
using Prism.Interactivity.InteractionRequest;
using System;

namespace ESD.JC_Main.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Properties   

        private string _AuthenticatedUser = string.Empty;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set { SetProperty(ref _AuthenticatedUser, value); }
        }

        private string _InteractionResultMessage;
        public string InteractionResultMessage
        {
            get { return _InteractionResultMessage; }
            set
            {
                SetProperty(ref _InteractionResultMessage, value);
                this.RaisePropertyChanged("InteractionResultMessage");
            }
        }

        private IEventAggregator _EventAggregator;
        private InteractionRequest<Confirmation> confirmLogoutInteractionRequest;

        #endregion

        public MainWindowViewModel(IEventAggregator EventAggregator)
        {
            LogoutCommand = new DelegateCommand<Window>(Logout);
            confirmLogoutInteractionRequest = new InteractionRequest<Confirmation>();

            _EventAggregator = EventAggregator;
            _EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);
        }

        public ICommand LogoutCommand { get; private set; }
        public IInteractionRequest ConfirmLogoutInteractionRequest
        {
            get { return this.confirmLogoutInteractionRequest; }
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
        }

        private void Logout(Window window)
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            if (customPrincipal != null)
            {
                this.confirmLogoutInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Due to security concerned, you will be casting out from this application. Is that ok?",
                        Title = "Logout"
                    },
                    returned => 
                    {
                        InteractionResultMessage = returned.Confirmed ? Confirmed(window, customPrincipal) : "NOT OK!";
                    });
            }
        }

        private string Confirmed(Window window, CustomPrincipal customPrincipal)
        {
            customPrincipal.Identity = new AnonymousIdentity();
            window.Close();
            Environment.Exit(0);

            return "OK!";
        }
    }
}
