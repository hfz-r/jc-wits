using Prism.Commands;
using Prism.Mvvm;
using System.Threading;

namespace ESD.JC_Infrastructure.AccessControl
{
    public class AuthorizationViewModel : BindableBase
    {
        private bool? _visibility = true;
        public bool? Visibility
        {
            get { return _visibility; }
            set { SetProperty(ref _visibility, value); }
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

        private IModuleAccessServices moduleAccessServices;

        public AuthorizationViewModel(IModuleAccessServices moduleAccessServices)
        {
            this.moduleAccessServices = moduleAccessServices;

            OnLoadedCommand = new DelegateCommand<object>(OnLoaded);
        }

        public DelegateCommand<object> OnLoadedCommand { get; private set; }

        private void OnLoaded(object obj)
        {
            if (!string.IsNullOrEmpty(AuthenticatedUser))
            {
                Visibility = moduleAccessServices.Initialize(AuthenticatedUser, (string)obj);
                RaisePropertyChanged("Visibility");
            }
        }

    }
}
