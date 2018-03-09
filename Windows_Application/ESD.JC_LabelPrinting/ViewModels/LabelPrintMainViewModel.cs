using ESD.JC_Infrastructure.Controls;
using ESD.JC_LabelPrinting.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TDSFramework;

namespace ESD.JC_LabelPrinting.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class LabelPrintMainViewModel : BindableBase, IConfirmNavigationRequest
    {
        #region Properties

        ProgressDialogHelper hlp;
        BackgroundWorker worker;

        private string _bin;
        public string BIN
        {
            get { return _bin; }
            set
            {
                SetProperty(ref _bin, value);
                RaisePropertyChanged("BIN");
                _printLblCommand.RaiseCanExecuteChanged();
            }
        }

        private string _sapNo;
        public string SAPNO
        {
            get { return _sapNo; }
            set
            {
                SetProperty(ref _sapNo, value);
                RaisePropertyChanged("SAPNO");
                _printLblCommand.RaiseCanExecuteChanged();
            }
        }

        private string _legacyNo;
        public string LEGACYNO
        {
            get { return _legacyNo; }
            set
            {
                SetProperty(ref _legacyNo, value);
                RaisePropertyChanged("LEGACYNO");
                _printLblCommand.RaiseCanExecuteChanged();
            }
        }

        private string _en;
        public string EN
        {
            get { return _en; }
            set
            {
                SetProperty(ref _en, value);
                RaisePropertyChanged("EN");
                _printLblCommand.RaiseCanExecuteChanged();
            }
        }

        private string _ms;
        public string MS
        {
            get { return _ms; }
            set
            {
                SetProperty(ref _ms, value);
                RaisePropertyChanged("MS");
                _printLblCommand.RaiseCanExecuteChanged();
            }
        }

        private string _bun;
        public string BUN
        {
            get { return _bun; }
            set
            {
                SetProperty(ref _bun, value);
                RaisePropertyChanged("BUN");
                _printLblCommand.RaiseCanExecuteChanged();
            }
        }

        private decimal? _qty;
        public decimal? QTY
        {
            get { return _qty; }
            set
            {
                SetProperty(ref _qty, value);
                RaisePropertyChanged("QTY");
                _printLblCommand.RaiseCanExecuteChanged();
            }
        }

        private string _vendorID;
        public string VENDORID
        {
            get { return _vendorID; }
            set
            {
                SetProperty(ref _vendorID, value);
                RaisePropertyChanged("VENDORID");
                _printLblCommand.RaiseCanExecuteChanged();
            }
        }

        private string _SendState;
        public string SendState
        {
            get { return _SendState; }
            set { SetProperty(ref _SendState, value); }
        }

        public CommandBindingCollection CommandBindings
        {
            get
            {
                return _CommandBindings;
            }
        }

        #endregion

        private const string NormalStateKey = "Normal";
        private const string SavingStateKey = "Saving";
        private const string SavedStateKey = "Saved";

        private IEventAggregator EventAggregator;

        private CommandBindingCollection _CommandBindings;
        private DelegateCommand<object> _printLblCommand;
        private InteractionRequest<Confirmation> confirmExitInteractionRequest;

        public LabelPrintMainViewModel(IEventAggregator EventAggregator)
        {
            SendState = NormalStateKey;

            this.EventAggregator = EventAggregator;

            hlp = new ProgressDialogHelper(EventAggregator);
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;

            _CommandBindings = new CommandBindingCollection();
            _printLblCommand = new DelegateCommand<object>(Print, CanPrint);
            confirmExitInteractionRequest = new InteractionRequest<Confirmation>();

            InitializeCommandBindings();
        }

        public ICommand PrintLblCommand
        {
            get { return this._printLblCommand; }
        }
        public IInteractionRequest ConfirmExitInteractionRequest
        {
            get { return this.confirmExitInteractionRequest; }
        }

        private void InitializeCommandBindings()
        {
            //Create a command binding for the Save command
            CommandBinding saveBinding = new CommandBinding(ApplicationCommands.Paste, PasteExecuted, PasteCanExecute);

            //Register the binding to the class 
            CommandManager.RegisterClassCommandBinding(typeof(LabelPrintMainViewModel), saveBinding);
            CommandBindings.Add(saveBinding);
        }
       
        private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                List<string[]> rowData = ClipboardHelper.ParseClipboardData();

                if (rowData.First() != null)
                {
                    var dt = rowData.First();
                    if (dt.All(x => string.IsNullOrEmpty(x)))
                    {
                        throw new Exception("Nothing to be pasted.");
                    }
                    else
                    {
                        BIN = dt.Count() > 0 ? dt[0] : string.Empty;
                        SAPNO = dt.Count() > 1 ? dt[1] : string.Empty;
                        LEGACYNO = dt.Count() > 2 ? dt[2] : string.Empty;
                        EN = dt.Count() > 3 ? dt[3] : string.Empty;
                        MS = dt.Count() > 4 ? dt[4] : string.Empty;
                        BUN = dt.Count() > 5 ? dt[5] : string.Empty;
                        QTY = string.IsNullOrEmpty(dt[6]) ? (decimal?)null : dt.Count() > 6 ? decimal.Parse(dt[6]) : (decimal?)null;

                        SendState = SavingStateKey;
                    }
                }
            }
            catch (Exception ex)
            {
                SendState = NormalStateKey;
                MessageBox.Show(ex.Message, "Notification", MessageBoxButton.OK);
            }

            _printLblCommand.RaiseCanExecuteChanged();
        }

        private void PasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
   
        private void Print(object obj)
        {
            if (MessageBox.Show("Confirm to generate the label?", "Print", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SendState = SavingStateKey;

                hlp.SetDialogDescription("Printing...");

                worker.DoWork += delegate (object s, DoWorkEventArgs args)
                {
                    StringBuilder strErrorPallet = new StringBuilder();
                    System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                    pd.PrinterSettings = new PrinterSettings();
                    pd.PrinterSettings.PrinterName = Properties.Settings.Default.PrinterPort;

                    try
                    {
                        string fileName = string.Empty;
                        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        fileName = path + @"\JC-WITS_GR_Label_Manual.prn";

                        StreamReader txtReader = new StreamReader(fileName, false);
                        string xTemp = txtReader.ReadToEnd();
                        txtReader.Close();
                        var parts = new Dictionary<int, string>[2];

                        StringBuilder strPallet = new StringBuilder();
                        StringBuilder strPalletTemplate = new StringBuilder();

                        strPallet.Append(string.Empty);
                        strPalletTemplate.Append(string.Empty);
                        strPalletTemplate.Append(xTemp);

                        strPallet = new StringBuilder();
                        strPallet.Append(strPalletTemplate.ToString());
                        strPallet.Replace("<SAPNO>", SAPNO);
                        strPallet.Replace("<LEGACYNO>", LEGACYNO);
                        strPallet.Replace("<BIN>", BIN);
                        strPallet.Replace("<QTY>", QTY.Value.ToString());
                        strPallet.Replace("<BUN>", BUN);
                        strPallet.Replace("<VENDORID>", VENDORID);

                        for (int x = 0; x < 2; x++)
                        {
                            parts[x] = new Dictionary<int, string>();

                            string input = string.Empty;
                            string lbl = string.Empty;
                            switch (x)
                            {
                                case 0:
                                    input = EN;
                                    lbl = "<EN";
                                    break;
                                case 1:
                                    input = MS;
                                    lbl = "<MS";
                                    break;
                            }

                            string[] words = input.Split(' ');

                            string part = string.Empty;
                            int partCounter = 0;
                            foreach (var word in words)
                            {
                                if (part.Length + word.Length < 33)
                                {
                                    part += string.IsNullOrEmpty(part) ? word : " " + word;
                                }
                                else
                                {
                                    parts[x].Add(partCounter, part);
                                    part = word;
                                    partCounter++;
                                }
                            }
                            parts[x].Add(partCounter, part);

                            if (partCounter == 0)
                                parts[x].Add((partCounter + 1), string.Empty);

                            foreach (var i in parts[x])
                            {
                                string index = lbl + (i.Key + 1).ToString() + ">";
                                strPallet.Replace(index, i.Value);
                            }
                        }

                        if (RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, strPallet.ToString()) == false)
                        {
                        }

                        if (worker.CancellationPending)
                        {
                            args.Cancel = true;
                            return;
                        }

                        hlp.Initialize();
                    }
                    catch (Exception ex)
                    {
                        SendState = NormalStateKey;
                        MessageBox.Show(ex.Message);
                    }
                };

                worker.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
                {
                    hlp.pgdialog.Close();
                    SendState = SavedStateKey;
                };

                worker.RunWorkerAsync();
                hlp.pgdialog.Visibility = Visibility.Visible;
            }
        }

        private bool CanPrint(object arg)
        {
            if (!string.IsNullOrEmpty(BIN) && !string.IsNullOrEmpty(SAPNO) &&
                !string.IsNullOrEmpty(LEGACYNO) && !string.IsNullOrEmpty(EN) &&
                !string.IsNullOrEmpty(MS) && !string.IsNullOrEmpty(BUN) &&
                !string.IsNullOrEmpty(VENDORID) && QTY.HasValue)
            {
                return true;
            }

            return false;
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (this.SendState == NormalStateKey)
            {
                this.confirmExitInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you sure you want to navigate away from this window?",
                        Title = "Confirm"
                    },
                    c => { continuationCallback(c.Confirmed); });
            }
            else
            {
                continuationCallback(true);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
