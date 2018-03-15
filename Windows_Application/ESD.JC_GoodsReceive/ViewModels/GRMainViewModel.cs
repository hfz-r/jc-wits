using DataLayer;
using ESD.JC_GoodsReceive.Services;
using ESD.JC_Infrastructure.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using System.Reflection;
using TDSFramework;
using Microsoft.Win32;
using FileHelpers;
using FileHelpers.ExcelNPOIStorage;
using Microsoft.Office.Interop.Excel;
using ESD.JC_Infrastructure;
using FileHelpers.Events;
using System.Threading;

namespace ESD.JC_GoodsReceive.ViewModels
{
    public class GRMainViewModel : BindableBase
    {
        #region Properties

        private ICollectionView _GoodReceives;
        public ICollectionView GoodReceives
        {
            get { return _GoodReceives; }
            set { SetProperty(ref _GoodReceives, value); }
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

        private DataGridCellInfo _cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return _cellInfo; }
            set
            {
                _cellInfo = value;
            }
        }

        private string _FilterTextBox;
        public string FilterTextBox
        {
            get { return _FilterTextBox; }
            set
            {
                SetProperty(ref _FilterTextBox, value);
                if (GoodReceives != null)
                    CollectionViewSource.GetDefaultView(GoodReceives).Refresh();
            }
        }

        private List<bool> _CountChecked = new List<bool>();
        public List<bool> CountChecked
        {
            get { return _CountChecked; }
            set { SetProperty(ref _CountChecked, value); }
        }

        private object _ImportBtn;
        public object ImportBtn
        {
            get { return _ImportBtn; }
            set
            {
                SetProperty(ref _ImportBtn, value);
                RaisePropertyChanged("ImportBtn");
            }
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

        #endregion

        private const string grDetailsViewName = "GRDetailsView";

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IGRServices grServices;

        public ObservableCollection<GoodsReceive> grCollection { get; private set; }
        public ObservableCollection<GoodsReceive> tempCollection { get; private set; }

        private DelegateCommand<object> _ImportGRCommand;
        private DelegateCommand<object> _ExportGRCommand;
        private DelegateCommand<object> _PrintLblCommand;
        private DelegateCommand<object> _IsSelected;
        private DelegateCommand _checkedAllCommand;
        private DelegateCommand _unCheckedAllCommand;
        private InteractionRequest<Confirmation> confirmDeleteInteractionRequest;

        public GRMainViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IGRServices grServices)
        {
            this.regionManager = regionManager;
            this.grServices = grServices;
            this.eventAggregator = eventAggregator;

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            OpenGRDetailsCommand = new DelegateCommand<GoodsReceive>(OpenGRDetails);
            XOKCommand = new DelegateCommand(OnLoaded);
            OKCommand = new DelegateCommand(OKImport);

            _ImportGRCommand = new DelegateCommand<object>(ImportGR);
            _ExportGRCommand = new DelegateCommand<object>(ExportGR);
            _PrintLblCommand = new DelegateCommand<object>(PrintLabel, CanPrint);
            _IsSelected = new DelegateCommand<object>(CheckBoxIsSelected);
            _checkedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(true);
            });
            _unCheckedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(false);
            });

            confirmDeleteInteractionRequest = new InteractionRequest<Confirmation>();
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenGRDetailsCommand { get; private set; }
        public ICommand OKCommand { get; private set; }
        public ICommand XOKCommand { get; private set; }
        public ICommand ImportGRCommand
        {
            get { return this._ImportGRCommand; }
        }
        public ICommand ExportGRCommand
        {
            get { return this._ExportGRCommand; }
        }
        public ICommand PrintLblCommand
        {
            get { return this._PrintLblCommand; }
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
        public IInteractionRequest ConfirmDeleteInteractionRequest
        {
            get { return this.confirmDeleteInteractionRequest; }
        }

        private void OnLoaded()
        {
            ImportBtn = null;

            tempCollection = new ObservableCollection<GoodsReceive>();
            grCollection = new ObservableCollection<GoodsReceive>();
            foreach (var obj in grServices.GetAll())
            {
                if (obj.QtyReceived == null)
                    obj.Ok = false;
                else
                {
                    if (obj.QtyReceived == obj.Quantity)
                        obj.Ok = true;
                    else if (obj.QtyReceived < obj.Quantity)
                        obj.Ok = null;
                }
                
                obj.IsChecked = false;
                grCollection.Add(obj);
            }

            GoodReceives = new ListCollectionView(grCollection);
            GoodReceives.SortDescriptions.Add(new SortDescription("PurchaseOrder", ListSortDirection.Ascending));

            CollectionViewSource.GetDefaultView(GoodReceives).Filter = Filter;
        }

        private bool Filter(object item)
        {
            if (string.IsNullOrEmpty(FilterTextBox))
                return true;

            var gr = (GoodsReceive)item;

            return (gr.PurchaseOrder.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                    gr.Material.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase) ||
                    gr.MaterialShortText.StartsWith(FilterTextBox, StringComparison.OrdinalIgnoreCase));
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

            RaisePropertyChanged("GoodReceives");

            _PrintLblCommand.RaiseCanExecuteChanged();
        }

        private void ImportGR(object ignored)
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".csv|.xls|.xlsx";
            dlg.Filter = "CSV files (*.csv)|*.csv|Excel documents (*.xls, *.xlsx)|*.xls;*.xlsx";

            tempCollection = new ObservableCollection<GoodsReceive>();

            if (dlg.ShowDialog() == true)
            {
                var file = new FileInfo(dlg.FileName);
                if (file.Extension == ".csv")
                {
                    ReadCSVFile(file.FullName);
                }
                else
                {
                    ReadExcelFile(file.FullName);
                }
            }
        }

        private void ReadExcelFile(string fullName)
        {
            try
            {
                ExcelNPOIStorage storage = new ExcelNPOIStorage(typeof(ImportCLassModel), 1, 0);
                storage.FileName = fullName;

                var records = storage.ExtractRecords();
                foreach (ImportCLassModel rec in records)
                {
                    if (string.IsNullOrEmpty(rec.PurchaseOrder) ||
                        string.IsNullOrEmpty(rec.Vendor) ||
                        string.IsNullOrEmpty(rec.Material) ||
                        string.IsNullOrEmpty(rec.MaterialShortText) ||
                        string.IsNullOrEmpty(rec.StorageBin) ||
                        rec.DocumentDate == null)
                    {
                        throw new Exception("Null Or Empty Value Found.");
                    }

                    PopulateRecords(rec, tempCollection);
                }

                if (tempCollection.Count() > 0)
                {
                    ImportBtn = true;

                    GoodReceives = new ListCollectionView(tempCollection);
                    GoodReceives.SortDescriptions.Add(new SortDescription("PurchaseOrder", ListSortDirection.Ascending));

                    CollectionViewSource.GetDefaultView(GoodReceives).Filter = Filter;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read contents:\n\n" + ex.Message, "Error");
            }
        }

        private void OKImport()
        {
            this.confirmDeleteInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "Are you confirm you want to save this?",
                        Title = "Confirm"
                    },
                     c =>
                     {
                         if (c.Confirmed)
                         {
                            if (Save())
                                 OnLoaded();
                         }
                     });
        }

        private void ReadCSVFile(string fullName)
        {
            try
            {
                var engine = new DelimitedFileEngine<ImportCLassModel>(Encoding.UTF8);
                engine.AfterReadRecord += AfterReadCSVEvent;
                engine.Options.Delimiter = ";";

                var records = engine.ReadFile(fullName);
                foreach (ImportCLassModel rec in records)
                {
                    if (string.IsNullOrEmpty(rec.PurchaseOrder) ||
                       string.IsNullOrEmpty(rec.Vendor) ||
                       string.IsNullOrEmpty(rec.Material) ||
                       string.IsNullOrEmpty(rec.MaterialShortText) ||
                       string.IsNullOrEmpty(rec.StorageBin) ||
                       rec.DocumentDate == null)
                    {
                        throw new Exception("Null Or Empty Value Found.");
                    }

                    PopulateRecords(rec, tempCollection);
                }

                if (tempCollection.Count() > 0)
                {
                    ImportBtn = true;

                    GoodReceives = new ListCollectionView(tempCollection);
                    GoodReceives.SortDescriptions.Add(new SortDescription("PurchaseOrder", ListSortDirection.Ascending));

                    CollectionViewSource.GetDefaultView(GoodReceives).Filter = Filter;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read contents:\n\n" + ex.Message, "Error");
            }
        }

        private void AfterReadCSVEvent(EngineBase engine, AfterReadEventArgs<ImportCLassModel> e)
        {
            if (e.Record.PurchaseOrder.Length > 11)
                throw new Exception("Line " + e.LineNumber + ": PurchaseOrder is too long");

            if (e.Record.Material.Length > 8)
                throw new Exception("Line " + e.LineNumber + ": Material is too long");

            if (e.Record.MaterialShortText.Length > 41)
                throw new Exception("Line " + e.LineNumber + ": MaterialShortText is too long");

            if (e.Record.StorageBin.Length > 6)
                throw new Exception("Line " + e.LineNumber + ": StorageBin is too long");
        }

        private bool Save()
        {
            bool ok = false;
            try
            {
                List<GoodsReceive> toAddList = new List<GoodsReceive>();
                List<GoodsReceive> toUpdateList = new List<GoodsReceive>();

                foreach (GoodsReceive gr in tempCollection)
                {
                    if (gr.IsChecked == true)
                    {
                        var obj = grServices.GetGRBySAPNo(gr.Material);
                        {
                            if (obj != null)
                            {
                                Update(ref obj, gr);
                                toUpdateList.Add(obj);
                            }
                            else
                            {
                                toAddList.Add(gr);
                            }
                        }
                    }
                }

                if (toAddList.Count() > 0)
                    ok = grServices.Save(toAddList, "Save");
                if (toUpdateList.Count() > 0)
                    ok = grServices.Save(toUpdateList, "Update");

                return ok;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Update(ref GoodsReceive obj, GoodsReceive gr)
        {
            obj.Vendor = gr.Vendor;
            obj.Material = gr.Material;
            obj.MaterialShortText = gr.MaterialShortText;
            obj.Ok = gr.Ok;
            obj.Quantity = gr.Quantity;
            obj.StorageLoc = gr.StorageLoc;
            obj.Plant = gr.Plant;
            obj.StorageBin = gr.StorageBin;
            obj.DocumentDate = gr.DocumentDate;
            obj.PostingDate = DateTime.Now;
            obj.ModifiedOn = DateTime.Now;
            obj.ModifiedBy = AuthenticatedUser;
        }

        private void PopulateRecords(ImportCLassModel rec, ObservableCollection<GoodsReceive> temp)
        {
            if (grCollection.Any(x => x.PurchaseOrder == rec.PurchaseOrder) == false)
            {
                temp.Add(new GoodsReceive
                {
                    PurchaseOrder = rec.PurchaseOrder,
                    Vendor = rec.Vendor,
                    Material = rec.Material,
                    MaterialShortText = rec.MaterialShortText,
                    Ok = rec.Ok,
                    Quantity = rec.Quantity,
                    Eun = rec.Eun,
                    MvmtType = rec.MvmtType,
                    StorageLoc = rec.StorageLoc,
                    Plant = rec.Plant,
                    StorageBin = rec.StorageBin,
                    DocumentDate = rec.DocumentDate,
                    PostingDate = DateTime.Now,
                    CreatedOn = DateTime.Now,
                    CreatedBy = AuthenticatedUser,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = AuthenticatedUser,
                    IsChecked = true
                });
            }
        }

        private void ExportGR(object ignored)
        {
            ExcelNPOIStorage storage = new ExcelNPOIStorage(typeof(ImportCLassModel), 0, 0);

            string path = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Export GR");

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
                string.Concat(@"\GoodsReceiveExport_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm", 
                System.Globalization.CultureInfo.InvariantCulture)) + 
                ".xlsx";

            storage.ColumnsHeaders.Add("Document Date");
            storage.ColumnsHeaders.Add("PO Number");
            storage.ColumnsHeaders.Add("Vendor");
            storage.ColumnsHeaders.Add("Material");
            storage.ColumnsHeaders.Add("Material Short Text");
            storage.ColumnsHeaders.Add("OK");
            storage.ColumnsHeaders.Add("Quantity");
            storage.ColumnsHeaders.Add("Storage Loc");
            storage.ColumnsHeaders.Add("Plant");
            storage.ColumnsHeaders.Add("Storage Bin");

            ObservableCollection<ImportCLassModel> importObj = new ObservableCollection<ImportCLassModel>();
            foreach (var gr in grCollection)
            {
                importObj.Add(new ImportCLassModel
                {
                    DocumentDate = gr.DocumentDate.GetValueOrDefault(),
                    PurchaseOrder = gr.PurchaseOrder,
                    Vendor = gr.Vendor,
                    Material = gr.Material,
                    MaterialShortText = gr.MaterialShortText,
                    Ok = gr.Ok,
                    Quantity = gr.Quantity,
                    Eun = gr.Eun,
                    MvmtType = gr.MvmtType,
                    StorageLoc = gr.StorageLoc,
                    Plant = gr.Plant,
                    StorageBin = gr.StorageBin
                });
            }

            if (importObj != null)
            {
                storage.InsertRecords(importObj.ToArray());

                this.confirmDeleteInteractionRequest.Raise(
                    new Confirmation
                    {
                        Content = "File Export Success. Are you want to open the file?",
                        Title = "Notification"
                    },
                    c => { InteractionResultMessage = c.Confirmed ? OpenExportedFile(storage.FileName) : "NOT OK!"; });
            }
        }

        private string OpenExportedFile(string fileName)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = true;

                Workbook wb = excel.Workbooks.Open(fileName);
            }
            catch
            {
                throw new Exception();
            }

            return "OK";
        }

        private void PrintLabel(object ignored)
        {
            if (grCollection.Any(x => x.IsChecked == true))
            {
                if (MessageBox.Show("Confirm to generate the label?", "Print", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var listObj = grCollection.Where(x => x.IsChecked == true).ToList();

                    StringBuilder strErrorPallet = new StringBuilder();
                    System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                    pd.PrinterSettings = new PrinterSettings();
                    pd.PrinterSettings.PrinterName = Properties.Settings.Default.PrinterPort;

                    try
                    {
                        string fileName = string.Empty;
                        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        fileName = path + @"\JC-WITS_Label.prn";

                        StreamReader txtReader = new StreamReader(fileName, false);
                        string xTemp = txtReader.ReadToEnd();
                        txtReader.Close();
                        var parts = new Dictionary<int, string>[2];

                        foreach (var item in listObj)
                        {
                            StringBuilder strPallet = new StringBuilder();
                            StringBuilder strPalletTemplate = new StringBuilder();

                            strPallet.Append(string.Empty);
                            strPalletTemplate.Append(string.Empty);
                            strPalletTemplate.Append(xTemp);

                            strPallet = new StringBuilder();
                            strPallet.Append(strPalletTemplate.ToString());
                            strPallet.Replace("<SAPNO>", item.Material);
                            strPallet.Replace("<LEGACYNO>", item.Material);
                            strPallet.Replace("<BIN>", item.StorageBin);
                            strPallet.Replace("<QTY>", item.Quantity.ToString());
                            strPallet.Replace("<BUN>", item.Eun);

                            if (item.Eun == "KG")
                                strPallet.Replace("<QRCODE>", item.Material + ";" + item.Quantity);
                            else
                                strPallet.Replace("<QRCODE>", item.Material);

                            //start EN & MS String Builder
                            for (int x = 0; x < 2; x++)
                            {
                                parts[x] = new Dictionary<int, string>();

                                string input = string.Empty;
                                string lbl = string.Empty;
                                switch (x)
                                {
                                    case 0:
                                        input = item.MaterialShortText;
                                        lbl = "<EN";
                                        break;
                                    case 1:
                                        input = item.MaterialShortText;
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
                            //end EN & MS String Builder

                            if (RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, strPallet.ToString()) == false)
                            {
                                strErrorPallet.Append(item.PurchaseOrder + ", ");
                            }
                        }

                        if (strErrorPallet.Length > 0)
                        {
                            strErrorPallet.Remove(strErrorPallet.Length - 2, 2);
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

        private bool CanPrint(object ignored)
        {
            if (ImportBtn != null)
            {
                return false;
            }

            return (CountChecked == null && CountChecked.Count.Equals(0)) ? false : (CountChecked.Where(x => x == true).Count() > 0);
        }

        private void OpenGRDetails(GoodsReceive gr)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", gr.ID);

            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(grDetailsViewName + parameters, UriKind.Relative));
        }

        private void SetIsSelectedProperty(bool isSelected)
        {
            ObservableCollection<GoodsReceive> tempObj = new ObservableCollection<GoodsReceive>();
            if (tempCollection != null && tempCollection.Count() > 0)
                tempObj = tempCollection;
            else
                tempObj = grCollection;

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

            _PrintLblCommand.RaiseCanExecuteChanged();

            GoodReceives = new ListCollectionView(tempObj);
            GoodReceives.SortDescriptions.Add(new SortDescription("PurchaseOrder", ListSortDirection.Ascending));

            CollectionViewSource.GetDefaultView(GoodReceives).Filter = Filter;
        }
    }

    [DelimitedRecord("")]
    public class ImportCLassModel
    {
        [FieldOrder(1)]
        public DateTime? DocumentDate { get; set; }

        [FieldOrder(2)]
        public string PurchaseOrder { get; set; }

        [FieldOrder(3)]
        public string Vendor { get; set; }

        [FieldOrder(4)]
        public string Material { get; set; }

        [FieldOrder(5)]
        public string MaterialShortText { get; set; }

        [FieldOrder(6)]
        public bool? Ok { get; set; }

        [FieldOrder(7)]
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal Quantity { get; set; }

        [FieldOrder(8)]
        public string Eun { get; set; }

        [FieldOrder(9)]
        public string MvmtType { get; set; }

        [FieldOrder(10)]
        public string StorageLoc { get; set; }

        [FieldOrder(11)]
        public int Plant { get; set; }

        [FieldOrder(12)]
        public string StorageBin { get; set; }
    }
}
