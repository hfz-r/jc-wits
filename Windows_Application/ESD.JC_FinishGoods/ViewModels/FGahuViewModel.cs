﻿using DataLayer;
using ESD.JC_Infrastructure.Controls;
using ESD.JC_FinishGoods.Services;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using ESD.JC_Infrastructure.Events;
using System.Windows.Data;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Linq;
using FileHelpers.ExcelNPOIStorage;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using FileHelpers;
using Microsoft.Office.Interop.Excel;
using ESD.JC_Infrastructure;
using System.Text;
using System.Drawing.Printing;
using System.Reflection;
using TDSFramework;

namespace ESD.JC_FinishGoods.ViewModels
{
    public class FGahuViewModel : BindableBase, IActiveAware
    {
        #region Properties

        public string ViewName
        {
            get { return "Air Handling Unit (AHU)"; }
        }

        private ICollectionView _AHU;
        public ICollectionView AHU
        {
            get { return _AHU; }
            set { SetProperty(ref _AHU, value); }
        }

        private DataGridCellInfo _cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return _cellInfo; }
            set { SetProperty(ref _cellInfo, value); }
        }

        private List<bool> _CountChecked = new List<bool>();
        public List<bool> CountChecked
        {
            get { return _CountChecked; }
            set { SetProperty(ref _CountChecked, value); }
        }

        private bool _canPrint = true;
        public bool CanPrint
        {
            get { return _canPrint; }
            set { SetProperty(ref _canPrint, value); }
        }

        private string _AuthenticatedUser = string.Empty;
        public string AuthenticatedUser
        {
            get { return _AuthenticatedUser; }
            set { SetProperty(ref _AuthenticatedUser, value); }
        }

        #endregion Properties

        private const string ahuDetailsViewName = "FGahuDetailsView";

        XSSFWorkbook ahuWorkbook;

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private IAHUServices ahuServices;

        public ObservableCollection<AHU> ahuCollection { get; private set; }
        public ObservableCollection<AHU> tempCollection { get; private set; }

        public DelegateCommand ImportFGCommand;
        public DelegateCommand ExportFGCommand;
        public DelegateCommand PrintLblCommand;
        public DelegateCommand OKCommand;
        public DelegateCommand XOKCommand;

        private DelegateCommand<object> _IsSelected;
        private DelegateCommand _checkedAllCommand;
        private DelegateCommand _unCheckedAllCommand;

        public FGahuViewModel(ICompositeCommands applicationCommands, IRegionManager regionManager, IEventAggregator eventAggregator, IAHUServices ahuServices)
        {
            this.regionManager = regionManager;
            this.ahuServices = ahuServices;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<AuthenticatedUserEvent>().Subscribe(InitAuthenticatedUser);

            OnLoadedCommand = new DelegateCommand(OnLoaded);
            OpenAHUDetailsCommand = new DelegateCommand<AHU>(OpenAHUDetails);
            _IsSelected = new DelegateCommand<object>(CheckBoxIsSelected);
            _checkedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(true);
            });
            _unCheckedAllCommand = new DelegateCommand(() =>
            {
                SetIsSelectedProperty(false);
            });

            ImportFGCommand = new DelegateCommand(Import);
            ExportFGCommand = new DelegateCommand(Export);
            PrintLblCommand = new DelegateCommand(PrintLabel).ObservesCanExecute(() => CanPrint);
            OKCommand = new DelegateCommand(OKImport);
            XOKCommand = new DelegateCommand(OnLoaded);
        }

        public DelegateCommand OnLoadedCommand { get; private set; }
        public ICommand OpenAHUDetailsCommand { get; private set; }
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

        private void OnLoaded()
        {
            this.eventAggregator.GetEvent<ObjectEvent>().Publish(null);

            tempCollection = new ObservableCollection<AHU>();
            ahuCollection = new ObservableCollection<AHU>();
            foreach (var obj in ahuServices.GetAll(false))
            {
                if (obj.SectionReceived == null)
                    obj.ShipStatus = false;
                else
                {
                    if (obj.SectionReceived == obj.Section)
                        obj.ShipStatus = true;
                    else if (obj.SectionReceived < obj.Section)
                        obj.ShipStatus = null;
                }

                obj.IsChecked = false;
                ahuCollection.Add(obj);
            }

            AHU = new ListCollectionView(ahuCollection);
            AHU.SortDescriptions.Add(new SortDescription("Project", ListSortDirection.Ascending));

            //CollectionViewSource.GetDefaultView(AHU).Filter = Filter;
        }

        private void InitAuthenticatedUser(string user)
        {
            AuthenticatedUser = user;
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

            RaisePropertyChanged("AHU");

            PrintLblCommand.RaiseCanExecuteChanged();
        }

        private void SetIsSelectedProperty(bool isSelected)
        {
            ObservableCollection<AHU> tempObj = new ObservableCollection<AHU>();
            if (tempCollection != null && tempCollection.Count() > 0)
                tempObj = tempCollection;
            else
                tempObj = ahuCollection;

            foreach (var ahu in tempObj)
            {
                ahu.IsChecked = isSelected;
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

            PrintLblCommand.RaiseCanExecuteChanged();

            AHU = new ListCollectionView(tempObj);
            AHU.SortDescriptions.Add(new SortDescription("Project", ListSortDirection.Ascending));

            //CollectionViewSource.GetDefaultView(AHU).Filter = Filter;
        }

        private void Import()
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xls|.xlsx";
            dlg.Filter = "Excel documents (*.xls, *.xlsx, *.xlsm)|*.xls;*.xlsx;*.xlsm";

            tempCollection = new ObservableCollection<AHU>();

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var file = new FileInfo(dlg.FileName);
                    if (!string.IsNullOrEmpty(file.Extension))
                    {
                        using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                        {
                            ahuWorkbook = new XSSFWorkbook(fs);
                        }

                        ReadExcelFile();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Import Failed.", MessageBoxButton.OK);
                }
            }
        }

        private void ReadExcelFile()
        {
            try
            {
                ISheet sheet = ahuWorkbook.GetSheetAt(0);
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

                #region Cells Comparison

                while (rows.MoveNext())
                {
                    IRow row = (XSSFRow)rows.Current;
                    if (row.RowNum == 0)
                        continue;
                    if (row.RowNum == 1)
                        continue;
                    if (row.RowNum == 2)
                        continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank))
                        continue;
                    if (row.Cells.Any(d => d.CellType == CellType.Error))
                        continue;

                    AHUImportCLassModel obj = new AHUImportCLassModel();
                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        ICell cell = row.GetCell(i);

                        switch (i)
                        {
                            case 1:
                                if (cell == null)
                                    continue;
                                else
                                    obj.SerialNo = SetCellValue(cell);
                                break;

                            case 2:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Project = SetCellValue(cell);
                                break;

                            case 3:
                                if (cell == null)
                                    continue;
                                else
                                    obj.UnitTag = SetCellValue(cell);
                                break;

                            case 6:
                                if (cell == null)
                                    continue;
                                else
                                    obj.SalesOrder = SetCellValue(cell);
                                break;

                            case 7:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Item = (cell.CellType.ToString() == "String" ? Convert.ToDecimal(cell.StringCellValue) : Convert.ToDecimal(cell.NumericCellValue));
                                break;

                            case 8:
                                if (cell == null)
                                    continue;
                                else
                                    obj.PartNo = SetCellValue(cell);
                                break;

                            case 9:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Model = SetCellValue(cell);
                                break;

                            case 12:
                                if (cell == null)
                                    continue;
                                else
                                    obj.CoolingCoil1a = SetCellValue(cell);
                                break;

                            case 14:
                                if (cell == null)
                                    continue;
                                else
                                    obj.CoolingCoil1b = SetCellValue(cell);
                                break;

                            case 15:
                                if (cell == null)
                                    continue;
                                else
                                    obj.CoolingCoil1c = SetCellValue(cell);
                                break;

                            case 16:
                                if (cell == null)
                                    continue;
                                else
                                    obj.CoolingCoil1d = SetCellValue(cell);
                                break;

                            case 18:
                                if (cell == null)
                                    continue;
                                else
                                    obj.CoolingCoil2a = SetCellValue(cell);
                                break;

                            case 20:
                                if (cell == null)
                                    continue;
                                else
                                    obj.CoolingCoil2b = SetCellValue(cell);
                                break;

                            case 21:
                                if (cell == null)
                                    continue;
                                else
                                    obj.CoolingCoil2c = SetCellValue(cell);
                                break;

                            case 22:
                                if (cell == null)
                                    continue;
                                else
                                    obj.CoolingCoil2d = SetCellValue(cell);
                                break;

                            case 24:
                                if (cell == null)
                                    continue;
                                else
                                    obj.HeatingCoil1a = SetCellValue(cell);
                                break;

                            case 26:
                                if (cell == null)
                                    continue;
                                else
                                    obj.HeatingCoil1b = SetCellValue(cell);
                                break;

                            case 27:
                                if (cell == null)
                                    continue;
                                else
                                    obj.HeatingCoil1c = SetCellValue(cell);
                                break;

                            case 28:
                                if (cell == null)
                                    continue;
                                else
                                    obj.HeatingCoil1d = SetCellValue(cell);
                                break;

                            case 30:
                                if (cell == null)
                                    continue;
                                else
                                    obj.HeatingCoil2a = SetCellValue(cell);
                                break;

                            case 32:
                                if (cell == null)
                                    continue;
                                else
                                    obj.HeatingCoil2b = SetCellValue(cell);
                                break;

                            case 33:
                                if (cell == null)
                                    continue;
                                else
                                    obj.HeatingCoil2c = SetCellValue(cell);
                                break;

                            case 34:
                                if (cell == null)
                                    continue;
                                else
                                    obj.HeatingCoil2d = SetCellValue(cell);
                                break;

                            case 40:
                                if (cell == null)
                                    continue;
                                else
                                    if (cell.CellType.ToString() == "Formula")
                                    if (cell.CachedFormulaResultType.ToString() == "Error")
                                        continue;
                                    else
                                        obj.FanType = SetFormulaCellValue(cell);
                                else
                                    obj.FanType = SetCellValue(cell);
                                break;

                            case 42:
                                if (cell == null)
                                    continue;
                                else
                                    obj.FanMotor = SetCellValue(cell);
                                break;

                            case 43:
                                if (cell == null)
                                    continue;
                                else
                                    obj.MotorPole = SetCellValue(cell);
                                break;

                            case 44:
                                if (cell == null)
                                    continue;
                                else
                                    obj.PowerSupply = SetCellValue(cell);
                                break;

                            case 45:
                                if (cell == null)
                                    continue;
                                else
                                    obj.FanRPM = SetCellValue(cell);
                                break;

                            case 47:
                                if (cell == null)
                                    continue;
                                else
                                    obj.FanPulley = SetCellValue(cell);
                                break;

                            case 48:
                                if (cell == null)
                                    continue;
                                else
                                    obj.MotorPulley = SetCellValue(cell);
                                break;

                            case 49:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Belt = SetCellValue(cell);
                                break;

                            case 59:
                                if (cell == null)
                                    continue;
                                else
                                    obj.Section = (cell.CellType.ToString() == "String" ? Convert.ToInt32(cell.StringCellValue) : Convert.ToInt32(cell.NumericCellValue));
                                break;

                            case 91:
                                if (cell == null)
                                    continue;
                                if (cell.CellType.ToString() == "Formula")
                                    if (cell.CachedFormulaResultType.ToString() == "Error")
                                        continue;
                                    else
                                        obj.Heater = SetFormulaCellValue(cell);
                                else
                                    obj.Heater = SetCellValue(cell);
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(obj.SerialNo) ||
                        !string.IsNullOrEmpty(obj.Project))
                        PopulateRecords(obj, tempCollection);
                }
                #endregion Cells Comparison

                if (tempCollection.Count() > 0)
                {
                    this.eventAggregator.GetEvent<ObjectEvent>().Publish(true);

                    AHU = new ListCollectionView(tempCollection);
                    AHU.SortDescriptions.Add(new SortDescription("Project", ListSortDirection.Ascending));

                    //CollectionViewSource.GetDefaultView(AHU).Filter = Filter;
                }
                else
                    throw new Exception("Data not available.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to read contents:\n\n" + ex.Message, "Error");
            }
        }

        public string SetCellValue(ICell cell)
        {
            return (cell.CellType.ToString() == "String" ? cell.StringCellValue : cell.NumericCellValue.ToString());
        }

        public string SetFormulaCellValue(ICell cell)
        {
            return cell.CachedFormulaResultType.ToString() == "String" ? cell.StringCellValue : cell.NumericCellValue.ToString();
        }

        private void PopulateRecords(AHUImportCLassModel rec, ObservableCollection<AHU> temp)
        {
            if (ahuCollection.Any(x => x.SerialNo == rec.SerialNo) == false)
            {
                temp.Add(new AHU
                {
                    Project = rec.Project,
                    UnitTag = rec.UnitTag,
                    PartNo = rec.PartNo,
                    Model = rec.Model,
                    PowerSupply = rec.PowerSupply,
                    FanType = rec.FanType,
                    MotorPole = rec.MotorPole,
                    FanMotor = rec.FanMotor + "  " + "KW",
                    FanRPM = rec.FanRPM,
                    FanPulley = rec.FanPulley,
                    MotorPulley = rec.MotorPulley,
                    Belt = rec.Belt,
                    Section = rec.Section,
                    CoolingCoil1 = (!string.IsNullOrEmpty(rec.CoolingCoil1a) ? rec.CoolingCoil1a : "-") + "R" + " / " +
                                   (!string.IsNullOrEmpty(rec.CoolingCoil1b) ? rec.CoolingCoil1b : "-") + "FPI" + " / " +
                                   (!string.IsNullOrEmpty(rec.CoolingCoil1c) ? rec.CoolingCoil1c : "-") + " / " +
                                   (!string.IsNullOrEmpty(rec.CoolingCoil1d) ? rec.CoolingCoil1d : "-") + "H",
                    CoolingCoil2 = (!string.IsNullOrEmpty(rec.CoolingCoil2a) ? rec.CoolingCoil2a : "-") + "R" + " / " +
                                   (!string.IsNullOrEmpty(rec.CoolingCoil2b) ? rec.CoolingCoil2b : "-") + "FPI" + " / " +
                                   (!string.IsNullOrEmpty(rec.CoolingCoil2c) ? rec.CoolingCoil2c : "-") + " / " +
                                   (!string.IsNullOrEmpty(rec.CoolingCoil2d) ? rec.CoolingCoil2d : "-") + "H",
                    HeatingCoil1 = (!string.IsNullOrEmpty(rec.HeatingCoil1a) ? rec.HeatingCoil1a : "-") + "R" + " / " +
                                   (!string.IsNullOrEmpty(rec.HeatingCoil1b) ? rec.HeatingCoil1b : "-") + "FPI" + " / " +
                                   (!string.IsNullOrEmpty(rec.HeatingCoil1c) ? rec.HeatingCoil1c : "-") + " / " +
                                   (!string.IsNullOrEmpty(rec.HeatingCoil1d) ? rec.HeatingCoil1d : "-") + "H",
                    HeatingCoil2 = (!string.IsNullOrEmpty(rec.HeatingCoil2a) ? rec.HeatingCoil2a : "-") + "R" + " / " +
                                   (!string.IsNullOrEmpty(rec.HeatingCoil2b) ? rec.HeatingCoil2b : "-") + "FPI" + " / " +
                                   (!string.IsNullOrEmpty(rec.HeatingCoil2c) ? rec.HeatingCoil2c : "-") + " / " +
                                   (!string.IsNullOrEmpty(rec.HeatingCoil2d) ? rec.HeatingCoil2d : "-") + "H",
                    Heater = rec.Heater,
                    SalesOrder = rec.SalesOrder,
                    Item = rec.Item,
                    SerialNo = rec.SerialNo,
                    CreatedOn = DateTime.Now,
                    CreatedBy = AuthenticatedUser,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = AuthenticatedUser,
                    IsChecked = true
                });
            }
        }

        private void Export()
        {
            ExcelNPOIStorage storage = new ExcelNPOIStorage(typeof(AHUImportCLassModel), 0, 0);

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Export FG");

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
                string.Concat(@"\AHUExport_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm",
                System.Globalization.CultureInfo.InvariantCulture)) +
                ".xlsx";

            storage.ColumnsHeaders.Add("Project");
            storage.ColumnsHeaders.Add("Unit Tag");
            storage.ColumnsHeaders.Add("Part No.");
            storage.ColumnsHeaders.Add("Model");
            storage.ColumnsHeaders.Add("Section");
            storage.ColumnsHeaders.Add("Item");
            storage.ColumnsHeaders.Add("Serial No.");
            storage.ColumnsHeaders.Add("Section Received");

            ObservableCollection<AHUImportCLassModel> importObj = new ObservableCollection<AHUImportCLassModel>();
            foreach (var ahu in ahuCollection)
            {
                importObj.Add(new AHUImportCLassModel
                {
                    Project = ahu.Project,
                    UnitTag = ahu.UnitTag,
                    PartNo = ahu.PartNo,
                    Model = ahu.Model,
                    Section = ahu.Section,
                    Item = ahu.Item,
                    SerialNo = ahu.SerialNo,
                    SectionReceived = ahu.SectionReceived.GetValueOrDefault()
                });
            }

            if (importObj != null)
            {
                storage.InsertRecords(importObj.ToArray());
            }
            if (MessageBox.Show("File Export Success. Are you want to open the file?", "Notification", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenExportedFile(storage.FileName);
            }
        }

        private void OpenExportedFile(string fileName)
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
        }

        private void OKImport()
        {
            try
            {
                if (MessageBox.Show("Are you confirm you want to save this?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (Save())
                        OnLoaded();
                    else
                        throw new Exception("Save Failed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Import Failed.", MessageBoxButton.OK);
            }
        }

        private bool Save()
        {
            bool ok = false;
            try
            {
                List<AHU> toAddList = new List<AHU>();
                List<AHU> toUpdateList = new List<AHU>();

                foreach (AHU ahu in tempCollection)
                {
                    if (ahu.IsChecked == true)
                    {
                        var obj = ahuServices.GetAHUBySerialNo(ahu.SerialNo);
                        {
                            if (obj != null)
                            {
                                Update(ref obj, ahu);
                                toUpdateList.Add(obj);
                            }
                            else
                            {
                                toAddList.Add(ahu);
                            }
                        }
                    }
                }

                if (toAddList.Count() > 0)
                    ok = ahuServices.Save(toAddList, "Save");
                if (toUpdateList.Count() > 0)
                    ok = ahuServices.Save(toUpdateList, "Update");

                return ok;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void Update(ref AHU obj, AHU ahu)
        {
            obj.Project = ahu.Project;
            obj.UnitTag = ahu.UnitTag;
            obj.PartNo = ahu.PartNo;
            obj.Model = ahu.Model;
            obj.PowerSupply = ahu.PowerSupply;
            obj.FanType = ahu.FanType;
            obj.MotorPole = ahu.MotorPole;
            obj.FanMotor = ahu.FanMotor;
            obj.FanRPM = ahu.FanRPM;
            obj.FanPulley = ahu.FanPulley;
            obj.MotorPulley = ahu.MotorPulley;
            obj.Belt = ahu.Belt;
            obj.Section = ahu.Section;
            obj.CoolingCoil1 = ahu.CoolingCoil1;
            obj.CoolingCoil2 = ahu.CoolingCoil2;
            obj.HeatingCoil1 = ahu.HeatingCoil1;
            obj.HeatingCoil2 = ahu.HeatingCoil1;
            obj.Heater = ahu.Heater;
            obj.SalesOrder = ahu.SalesOrder;
            obj.Item = ahu.Item;
            obj.ModifiedOn = DateTime.Now;
            obj.ModifiedBy = AuthenticatedUser;
        }

        private void PrintLabel()
        {
            if (ahuCollection.Any(x => x.IsChecked == true))
            {
                if (MessageBox.Show("Confirm to generate the label?", "Print", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var listObj = ahuCollection.Where(x => x.IsChecked == true).ToList();

                    StringBuilder strErrorPallet = new StringBuilder();
                    System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                    pd.PrinterSettings = new PrinterSettings();
                    pd.PrinterSettings.PrinterName = Properties.Settings.Default.PrinterPort;

                    try
                    {
                        string fileName = string.Empty;
                        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        fileName = path + @"\JC-FG_AHU.prn";

                        StreamReader txtReader = new StreamReader(fileName, Encoding.Default, false);
                        string xTemp = txtReader.ReadToEnd();
                        txtReader.Close();

                        foreach (var item in listObj)
                        {
                            StringBuilder strPallet = new StringBuilder();
                            StringBuilder strPalletTemplate = new StringBuilder();

                            strPallet.Append(string.Empty);
                            strPalletTemplate.Append(string.Empty);
                            strPalletTemplate.Append(xTemp);

                            strPallet = new StringBuilder();
                            strPallet.Append(strPalletTemplate.ToString());
                            strPallet.Replace("<Project>", item.Project);
                            strPallet.Replace("<UnitTag>", item.UnitTag);
                            strPallet.Replace("<PartNo>", item.PartNo);
                            strPallet.Replace("<Model>", item.Model);
                            strPallet.Replace("<PowerSupply>", item.PowerSupply);
                            strPallet.Replace("<FanType>", item.FanType);
                            strPallet.Replace("<MotorPole>", item.MotorPole);
                            strPallet.Replace("<FanMotor>", item.FanMotor);
                            strPallet.Replace("<FanRPM>", item.FanRPM);
                            strPallet.Replace("<FanPulley>", item.FanPulley);
                            strPallet.Replace("<MotorPulley>", item.MotorPulley);
                            strPallet.Replace("<Belt>", item.Belt);
                            strPallet.Replace("<CoolingCoil1>", item.CoolingCoil1);
                            strPallet.Replace("<CoolingCoil2>", item.CoolingCoil2);
                            strPallet.Replace("<HeatingCoil1>", item.HeatingCoil1);
                            strPallet.Replace("<HeatingCoil2>", item.HeatingCoil2);
                            strPallet.Replace("<Heater>", item.Heater);
                            strPallet.Replace("<SalesOrder>", item.SalesOrder);
                            strPallet.Replace("<Section>", item.Section.ToString());
                            strPallet.Replace("<Item>", item.Item.ToString());
                            strPallet.Replace("0123456789ABCDE", item.SerialNo);

                            if (RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, strPallet.ToString()) == false)
                            {
                                strErrorPallet.Append(item.SerialNo + ", ");
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

        private void OpenAHUDetails(AHU ahu)
        {
            var parameters = new NavigationParameters();
            parameters.Add("AuthenticatedUser", AuthenticatedUser);
            parameters.Add("ID", ahu.ID);

            this.regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(ahuDetailsViewName + parameters, UriKind.Relative));
        }

        #region Composite Buttons

        bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }

        private void OnIsActiveChanged()
        {
            ImportFGCommand.IsActive = IsActive;
            ExportFGCommand.IsActive = IsActive;
            PrintLblCommand.IsActive = IsActive;
            OKCommand.IsActive = IsActive;
            XOKCommand.IsActive = IsActive;

            IsActiveChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler IsActiveChanged;

        #endregion Composite Buttons
    }

    [DelimitedRecord("")]
    public class AHUImportCLassModel
    {
        [FieldOrder(1)]
        public string Project { get; set; }

        [FieldOrder(2)]
        public string UnitTag { get; set; }

        [FieldOrder(3)]
        public string PartNo { get; set; }

        [FieldOrder(4)]
        public string Model { get; set; }

        [FieldOrder(9)]
        public string PowerSupply { get; set; }

        [FieldOrder(10)]
        public string FanType { get; set; }

        [FieldOrder(11)]
        public string MotorPole { get; set; }

        [FieldOrder(12)]
        public string FanMotor { get; set; }

        [FieldOrder(13)]
        public string FanRPM { get; set; }

        [FieldOrder(14)]
        public string FanPulley { get; set; }

        [FieldOrder(15)]
        public string MotorPulley { get; set; }

        [FieldOrder(16)]
        public string Belt { get; set; }

        [FieldOrder(5)]
        public int? Section { get; set; }

        [FieldOrder(17)]
        public string CoolingCoil1a { get; set; }

        [FieldOrder(18)]
        public string CoolingCoil1b { get; set; }

        [FieldOrder(19)]
        public string CoolingCoil1c { get; set; }

        [FieldOrder(20)]
        public string CoolingCoil1d { get; set; }

        [FieldOrder(21)]
        public string CoolingCoil2a { get; set; }

        [FieldOrder(22)]
        public string CoolingCoil2b { get; set; }

        [FieldOrder(23)]
        public string CoolingCoil2c { get; set; }

        [FieldOrder(24)]
        public string CoolingCoil2d { get; set; }

        [FieldOrder(25)]
        public string HeatingCoil1a { get; set; }

        [FieldOrder(26)]
        public string HeatingCoil1b { get; set; }

        [FieldOrder(27)]
        public string HeatingCoil1c { get; set; }

        [FieldOrder(28)]
        public string HeatingCoil1d { get; set; }

        [FieldOrder(29)]
        public string HeatingCoil2a { get; set; }

        [FieldOrder(30)]
        public string HeatingCoil2b { get; set; }

        [FieldOrder(31)]
        public string HeatingCoil2c { get; set; }

        [FieldOrder(32)]
        public string HeatingCoil2d { get; set; }

        [FieldOrder(33)]
        public string Heater { get; set; }

        [FieldOrder(34)]
        public string SalesOrder { get; set; }

        [FieldOrder(6)]
        public decimal Item { get; set; }

        [FieldOrder(7)]
        public string SerialNo { get; set; }

        [FieldOrder(8)]
        public int? SectionReceived { get; set; }
    }
}