using DataLayer;
using ESD.JC_GoodsIssue.Services;
using ESD.JC_GoodsReceive.ViewModels;
using ESD.JC_Infrastructure;
using ESD.JC_Infrastructure.Events;
using FileHelpers;
using FileHelpers.ExcelNPOIStorage;
using Microsoft.Office.Interop.Excel;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ESD.JC_GoodsIssue.ViewModels
{
    public class GIMainViewModel : BindableBase
    {
        private ObservableCollection<GITransaction> _GoodsIssues;
        public ObservableCollection<GITransaction> GoodsIssues
        {
            get { return _GoodsIssues; }
            set
            {
                SetProperty(ref _GoodsIssues, value);
                RaisePropertyChanged("GoodsIssues");
            }
        }

        private GITransaction _SelectedGI;
        public GITransaction SelectedGI
        {
            get { return _SelectedGI; }
            set { SetProperty(ref _SelectedGI, value); }
        }

        private DataGridCellInfo _cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return _cellInfo; }
            set
            {
                _cellInfo = value;
                if (_cellInfo != null)
                {
                    _ExportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isEnableAutoRefresh;
        public bool IsEnableAutoRefresh
        {
            get { return _isEnableAutoRefresh; }
            set
            {
                SetProperty(ref _isEnableAutoRefresh, value);
                RaisePropertyChanged("IsEnableAutoRefresh");

                if (_isEnableAutoRefresh)
                    ExecuteTimer();
                else
                    StopTimer();
            }
        }

        private double _ProgressValue;
        public double ProgressValue
        {
            get
            {
                return _ProgressValue;
            }
            set
            {
                if (_ProgressValue == value)
                    return;
                _ProgressValue = value;

                RaisePropertyChanged("ProgressValue");
            }
        }

        private string _AuthenticatedUser = string.Empty;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set { SetProperty(ref _AuthenticatedUser, value); }
        }

        private string _FilterTextBox;
        public string FilterTextBox
        {
            get { return _FilterTextBox; }
            set
            {
                SetProperty(ref _FilterTextBox, value);
                RaisePropertyChanged("FilterTextBox");

                cvs.Filter += new FilterEventHandler(FilterGI);
            }
        }

        private List<bool> _CountChecked = new List<bool>();
        public List<bool> CountChecked
        {
            get { return _CountChecked; }
            set { SetProperty(ref _CountChecked, value); }
        }

        private CollectionViewSource cvs { get; set; }

        private const string giDetailsViewName = "GIDetailsView";
        public ObservableCollection<CheckedListItem<OkCategory>> OkFilter { get; private set; }

        private IEventAggregator EventAggregator;
        private IRegionManager RegionManager;
        private IGIServices GIServices;
        private IGITransactionServices GITransactionServices;
        private IGITimerSevices timerServices;
        private DelegateCommand<object> _DeleteCommand;
        private DelegateCommand<object> _IsSelected;
        private DelegateCommand _checkedAllCommand;
        private DelegateCommand _unCheckedAllCommand;
        private DelegateCommand<object> _ExportCommand;
        private InteractionRequest<Confirmation> confirmDeleteInteractionRequest;

        public GIMainViewModel(IEventAggregator _EventAggregator, IRegionManager _RegionManager, IGIServices _GIServices, IGITimerSevices _timerServices, IGITransactionServices _GITransactionServices)
        {
            RegionManager = _RegionManager;
            GIServices = _GIServices;
            GITransactionServices = _GITransactionServices;
            timerServices = _timerServices;
            EventAggregator = _EventAggregator;
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);
            EventAggregator.GetEvent<CollectionViewSourceEvent>().Subscribe(InitCollectionViewSource);
            EventAggregator.GetEvent<GI_ItemMessageEvent>().Subscribe(ConsumeItemMessage);

            IsEnableAutoRefresh = true;
            OnLoadedCommand = new DelegateCommand(OnLoaded);
            _ExportCommand = new DelegateCommand<object>(Export);
            _DeleteCommand = new DelegateCommand<object>(Delete, CanDelete);
            _IsSelected = new DelegateCommand<object>(CheckBoxIsSelected);
            _checkedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(true);
            });
            _unCheckedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(false);
            });
            OpenGIDetailsCommand = new DelegateCommand<GITransaction>(OpenGIDetails);
            confirmDeleteInteractionRequest = new InteractionRequest<Confirmation>();
        }

        #region Commands

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenGIDetailsCommand { get; private set; }
        public ICommand DeleteCommand
        {
            get { return this._DeleteCommand; }
        }
        public ICommand IsSelected
        {
            get { return this._IsSelected; }
        }
        public ICommand CheckedAllCommand
        {
            get { return _checkedAllCommand; }
        }
        public ICommand UnCheckedAllCommand
        {
            get { return _unCheckedAllCommand; }
        }
        public ICommand ExportCommand
        {
            get { return this._ExportCommand; }
        }
        public IInteractionRequest ConfirmDeleteInteractionRequest
        {
            get { return this.confirmDeleteInteractionRequest; }
        }

        #endregion Commands

        private void OnLoaded()
        {
            GoodsIssues = new ObservableCollection<GITransaction>();
            foreach(var obj in GIServices.GetAll(true).ToList())
            {
                if (obj.TransferType.Contains("POST"))
                {
                    obj.TransferType = "Transfer Posting";
                }
                else
                {
                    obj.TransferType = "Transfer to Production";
                }

                GoodsIssues.Add(obj);
            }            
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
        }

        private void InitCollectionViewSource(CollectionViewSource obj)
        {
            cvs = obj;
        }

        private void FilterGI(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterTextBox))
                return;

            var gi = e.Item as GITransaction;
            if (gi == null)
                e.Accepted = false;
            else if ((gi.GoodsReceive.Material.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) || 
                      gi.Text.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase)) != true)
                e.Accepted = false;
        }

        private void OpenGIDetails(GITransaction gi)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", SelectedGI.ID);

            this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(giDetailsViewName + parameters, UriKind.Relative));
        }
        private void SetIsSelectedProperty(bool isSelected)
        {
            ObservableCollection<GITransaction> tempObj = new ObservableCollection<GITransaction>();
            if (GoodsIssues != null && GoodsIssues.Count() > 0)
                tempObj = GoodsIssues;

            foreach (var gr in tempObj)
            {
                gr.IsChecked = isSelected;
            }

            switch (isSelected)
            {
                case true:
                    CountChecked.AddRange(Enumerable.Repeat(true, tempObj.Count));
                    break;
                case false:
                    CountChecked = new List<bool>();
                    break;
            }
            
            _DeleteCommand.RaiseCanExecuteChanged();            
        }

        private void CheckBoxIsSelected(object IsChecked)
        {
            bool chk = (bool)IsChecked;
            if (chk)
                CountChecked.Add(chk);
            else
            {
                if (CountChecked.Where(x => x == true).Count() > 0)
                    CountChecked.Remove(true);
            }

            RaisePropertyChanged("GoodsReceive");
            
            _DeleteCommand.RaiseCanExecuteChanged();
        }

        private void Delete(object ignored)
        {
            if (GoodsIssues.Any(x => x.IsChecked == true))
            {
                if (MessageBox.Show("Confirm to delete?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var listObj = GoodsIssues.Where(x => x.IsChecked == true).ToList();

                    try
                    {
                        foreach (var item in listObj)
                        {
                            GITransactionServices.Delete(item.ID);
                        }
                        OnLoaded();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private bool CanDelete(object ignored)
        {
            return (CountChecked == null && CountChecked.Count.Equals(0)) ? false : (CountChecked.Where(x => x == true).Count() > 0);
        }

        private void Export(object ignored)
        {
            if (GoodsIssues != null && GoodsIssues.Count == 0)
            {
                MessageBox.Show("There is no records to export");
                return;
            }

            ExcelNPOIStorage storage = new ExcelNPOIStorage(typeof(ExportCLassModel), 0, 0);

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Export GI");

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (IOException ie)
                {
                    Console.WriteLine("IO Error: " + ie.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("General Error: " + e.Message);
                }
            }

            storage.FileName = path +
                string.Concat(@"\GoodsIssueExport_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm",
                System.Globalization.CultureInfo.InvariantCulture)) +
                ".xlsx";

            storage.ColumnsHeaders.Add("SAP No.");
            storage.ColumnsHeaders.Add("Text");
            storage.ColumnsHeaders.Add("Quantity");
            storage.ColumnsHeaders.Add("Eun");
            storage.ColumnsHeaders.Add("Transfer Type");
            storage.ColumnsHeaders.Add("Production No.");
            storage.ColumnsHeaders.Add("Location To");
            storage.ColumnsHeaders.Add("Location From");
            storage.ColumnsHeaders.Add("Issued By");
            storage.ColumnsHeaders.Add("Issued On");

            ObservableCollection<ExportCLassModel> exportObj = new ObservableCollection<ExportCLassModel>();
            foreach (var gr in GoodsIssues)
            {
                exportObj.Add(new ExportCLassModel
                {
                    SAPNo = gr.GoodsReceive.Material,
                    Text = gr.Text,
                    Quantity = gr.Quantity,
                    Eun = gr.GoodsReceive.Eun,
                    TransferType = gr.TransferType.Contains("POST") ? "Transfer Posting" : "Transfer to Production",
                    ProductionNo = gr.ProductionNo,
                    LocationTo = (gr.Location1 != null) ? gr.Location1.LocationDesc : string.Empty,
                    LocationFrom = (gr.Location != null) ? gr.Location.LocationDesc : string.Empty,
                    IssuedBy = gr.CreatedBy,
                    IssuedOn = Convert.ToString(gr.CreatedOn)
                });
            }

            if (exportObj != null)
            {
                storage.InsertRecords(exportObj.ToArray());

                this.confirmDeleteInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Exported successfully. Open file?",
                        Title = "Notification"
                    },
                     c =>
                     {
                         if (c.Confirmed)
                         {
                             if (OpenExportedFile(storage.FileName))
                                 OnLoaded();
                         }
                     });
            }
        }

        private bool OpenExportedFile(string fileName)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = true;

                Workbook wb = excel.Workbooks.Open(fileName);
            }
            catch
            {
                return false;
                throw new Exception();
            }

            return true;
        }
        #region Timer

        private void ExecuteTimer()
        {
            timerServices.StartTimerExecute();
        }

        private void StopTimer()
        {
            timerServices.StopTimerExecute();
        }

        private void ConsumeItemMessage(GIItemMessage msg)
        {
            if (msg == null)
                return;

            if (msg.HasValue)
            {
                ProgressValue = msg.PercentageValue;

                if (msg.State == "Completed")
                {
                    #region refresh grid
                    OnLoaded();
                    #endregion refresh grid

                    StopTimer();
                    ExecuteTimer();
                }
            }
        }

        #endregion Timer
    }

    [DelimitedRecord("")]
    public class ExportCLassModel
    {
        [FieldOrder(2)]
        public string SAPNo { get; set; }

        [FieldOrder(3)]
        public string Text { get; set; }

        [FieldOrder(4)]
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal Quantity { get; set; }

        [FieldOrder(6)]
        public string Eun { get; set; }

        [FieldOrder(7)]
        public string TransferType { get; set; }

        [FieldOrder(8)]
        public string ProductionNo { get; set; }

        [FieldOrder(9)]
        public string LocationTo { get; set; }

        [FieldOrder(10)]
        public string LocationFrom { get; set; }

        [FieldOrder(11)]
        public string IssuedBy { get; set; }

        [FieldOrder(12)]
        public string IssuedOn { get; set; }
    }

}
