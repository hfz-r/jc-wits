using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Reflection;
using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using ESD.JC_Main.LoginServices;
using ESD.JC_Main.Views;
using ESD.JC_Infrastructure.Events;
using System.Threading;

namespace ESD.JC_Main.ViewModels
{
    public class LoginWindowViewModel : BindableBase
    {
        #region Properties

        private string _Versioning = "v " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public string Versioning
        {
            get { return _Versioning; }
            set { SetProperty(ref _Versioning, value); }
        }

        private string _Username = string.Empty;
        public string Username
        {
            get { return _Username; }
            set { SetProperty(ref _Username, value); }
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

        public static string Password = string.Empty;

        ProgressDialog pgdialog;
        BackgroundWorker worker;

        private IEventAggregator _EventAggregator;
        private IAuthenticationService _AuthenticationService;

        #endregion

        #region Constructor 

        public LoginWindowViewModel(IEventAggregator EventAggregator, IAuthenticationService AuthenticationService)
        {
            _AuthenticationService = AuthenticationService;
            _EventAggregator = EventAggregator;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            LoginCommand = new DelegateCommand<Window>(Login);
            CloseCommand = new DelegateCommand<Window>(Close);
        }

        #endregion

        #region Command

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        #endregion

        #region Event Handler

        private void OnLoaded()
        {
            Username = string.Empty;
            Password = string.Empty;
        }

        #endregion

        #region Methods
       
        private void Login(Window window)
        {
            if (string.IsNullOrEmpty(Username))
            {
                MessageBox.Show("Username is required.", "Notification", MessageBoxButton.OK);
                return;
            }
            else if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Password is required.", "Notification", MessageBoxButton.OK);
                return;
            }

            pgdialog = new ProgressDialog();

            System.Windows.Threading.Dispatcher _Dispatcher = pgdialog.Dispatcher;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += delegate (object s, DoWorkEventArgs args)
            {
                try
                {
                    _AuthenticationService.Authentication(Username, Password);

                    RaisePropertyChanged("IsAuthenticated");
                    RaisePropertyChanged("AuthenticatedUser");

                    for (int x = 1; x < 100; x++)
                    {
                        if (worker.CancellationPending)
                        {
                            args.Cancel = true;
                            return;
                        }

                        Thread.Sleep(10);

                        UpdateProgressDelegate update = new UpdateProgressDelegate(UpdateProgressText);
                        _Dispatcher.BeginInvoke(update, x / 10);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
                }
            };

            worker.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
            {
                pgdialog.Close();

                if (IsAuthenticated)
                {
                    _EventAggregator.GetEvent<AuthenticatedUserEvent>().Publish(AuthenticatedUser);
                    window.Close();
                }
            };

            worker.RunWorkerAsync();
            pgdialog.ShowDialog();
        }

        public delegate void UpdateProgressDelegate(int percentage);

        public void UpdateProgressText(int percentage)
        {
            pgdialog.ProgressValue = percentage;
        }

        private void Close(Window window)
        {
            Environment.Exit(0);
        }

        #endregion
    }
}
