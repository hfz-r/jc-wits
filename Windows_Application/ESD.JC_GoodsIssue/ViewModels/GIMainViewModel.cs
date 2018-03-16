using DataLayer;
using ESD.JC_GoodsIssue.Services;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

        private CollectionViewSource cvs { get; set; }

        private const string giDetailsViewName = "GIDetailsView";

        private IEventAggregator EventAggregator;
        private IRegionManager RegionManager;
        private IGIServices GIServices;
        private DelegateCommand<object> _ExportCommand;
        private InteractionRequest<Confirmation> confirmDeleteInteractionRequest;

        public GIMainViewModel(IEventAggregator _EventAggregator, IRegionManager _RegionManager, IGIServices _GIServices)
        {
            RegionManager = _RegionManager;
            GIServices = _GIServices;
            EventAggregator = _EventAggregator;
            EventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);
            EventAggregator.GetEvent<CollectionViewSourceEvent>().Subscribe(InitCollectionViewSource);

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            _ExportCommand = new DelegateCommand<object>(Export);
            OpenGIDetailsCommand = new DelegateCommand<GITransaction>(OpenGIDetails);
            confirmDeleteInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenGIDetailsCommand { get; private set; }
        public ICommand ExportCommand
        {
            get { return this._ExportCommand; }
        }
        public IInteractionRequest ConfirmDeleteInteractionRequest
        {
            get { return this.confirmDeleteInteractionRequest; }
        }

        private void OnLoaded()
        {
            GoodsIssues = new ObservableCollection<GITransaction>();
            foreach(var obj in GIServices.GetAll(true).ToList())
            {
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

        private void Export(object ignored)
        {
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

            storage.ColumnsHeaders.Add("ID");
            storage.ColumnsHeaders.Add("SAP No.");
            storage.ColumnsHeaders.Add("Text");
            storage.ColumnsHeaders.Add("Quantity");
            storage.ColumnsHeaders.Add("Transfer Type");
            storage.ColumnsHeaders.Add("Production No.");
            storage.ColumnsHeaders.Add("Location To");
            storage.ColumnsHeaders.Add("Location From");

            ObservableCollection<ExportCLassModel> exportObj = new ObservableCollection<ExportCLassModel>();
            foreach (var gr in GoodsIssues)
            {
                exportObj.Add(new ExportCLassModel
                {
                    ID = gr.ID,
                    SAPNo = gr.GoodsReceive.Material,
                    Text = gr.Text,
                    Quantity = gr.Quantity,
                    TransferType = gr.TransferType,
                    ProductionNo = gr.ProductionNo,
                    LocationTo = (gr.Location1 != null) ? gr.Location1.LocationDesc : string.Empty,
                    LocationFrom = (gr.Location != null) ? gr.Location.LocationDesc : string.Empty
                });
            }

            if (exportObj != null)
            {
                storage.InsertRecords(exportObj.ToArray());

                this.confirmDeleteInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "File Export Success. Are you want to open the file?",
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
                Application excel = new Application();
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
    }

    [DelimitedRecord("")]
    public class ExportCLassModel
    {
        [FieldOrder(1)]
        public long ID { get; set; }

        [FieldOrder(2)]
        public string SAPNo { get; set; }

        [FieldOrder(3)]
        public string Text { get; set; }

        [FieldOrder(4)]
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal Quantity { get; set; }

        [FieldOrder(5)]
        public string TransferType { get; set; }

        [FieldOrder(6)]
        public string ProductionNo { get; set; }

        [FieldOrder(7)]
        public string LocationTo { get; set; }

        [FieldOrder(8)]
        public string LocationFrom { get; set; }
    }

}
