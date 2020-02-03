using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.Report;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Helpers.Excel;
using Project.FC2J.UI.Helpers.Reports;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;

namespace Project.FC2J.UI.Reports
{
    /// <summary>
    /// Interaction logic for SalesReport.xaml
    /// </summary>
    public partial class SalesReport : Window
    {
        private IExcelHelper _excelHelper;
        private IReportEndpoint _reportEndpoint;
        private ReportTypeEnum _reportTypeEnum;
        private string _reportTIN;

        private const string _report1 = "Month to Date Sales Report";
        private const string _report2 = "Month to Date Purchases Report";
        private const string _report3 = "Monthly Sales Report (For BIR)";
        private const string _report4 = "Monthly Purchases Report (For BIR)";
        private const string _report5 = "Customer Summary Report";
        private const int _startingColumn = 3;

        private string _mtdSalesPathFilename = Directory.GetCurrentDirectory();
        private string _selectedReport;
        private string _mtdSalesFolder;
        private string _mtdSalesFilename;
        private string _overAllTotal = string.Empty;
        private string _dateRange = string.Empty;

        public SalesReport(IReportEndpoint reportEndpoint, IExcelHelper excelHelper, string reportTIN)
        {
            InitializeComponent();
            _reportEndpoint = reportEndpoint;
            _excelHelper = excelHelper;
            _reportTIN = reportTIN;
            ReportList.SelectionChanged += new SelectionChangedEventHandler(OnMyComboBoxChanged);
            ReportLabel.Visibility = Visibility.Visible;
        }

        private async void Generate_OnClick(object sender, RoutedEventArgs e)
        {
            Generate.IsEnabled = false;
            OverlayLoading.Visibility = Visibility.Visible;
            switch (_selectedReport)
            {
                case _report1:
                    await OnGenerateMTDSalesReport();
                    break;
                case _report2:
                    await OnGenerateMTDSalesReport();
                    break;
                case _report3:
                    await OnProcessMonthly();
                    break;
                case _report4:
                    await OnProcessMonthlyPurchases();
                    break;
                case _report5:
                    await OnGenerateCustomerAccountSummaryReport();
                    break;



            }
            OverlayLoading.Visibility = Visibility.Collapsed;
            Generate.IsEnabled = true;
        }

        private async Task OnGenerateCustomerAccountSummaryReport()
        {
            var reportParameter = new ProjectReportParameter
            {
                DateFrom = Convert.ToDateTime(DateFrom.Text),
                DateTo = Convert.ToDateTime(DateTo.Text)
            };

            var dtCustomerAccountSummary = await _reportEndpoint.GetCustomerAccountSummary(reportParameter);

            if(dtCustomerAccountSummary.Rows.Count == 0)
            {
                MessageBox.Show($"Record not found", "System Information", MessageBoxButton.OK);
                return;
            }

            dtCustomerAccountSummary.TableName = "AccountSummary";

            var mainDataSet = new DataSet("Mainreport");
            mainDataSet.Tables.Add(dtCustomerAccountSummary);

            await SaveToFile(reportParameter, mainDataSet);

            GeneratedMTDSales.Text = _mtdSalesFilename;
            ViewMTDSales.Visibility = Visibility.Visible;


        }

        private async Task OnProcessMonthlyPurchases()
        {
            try
            {
                if(MonthYearDate.SelectedDate == null) return;
                DateTime firstOfNextMonth = Convert.ToDateTime(MonthYearDate.SelectedDate).AddMonths(1);
                DateTime lastOfThisMonth = firstOfNextMonth.AddDays(-1);

                var reportParameter = new ProjectReportParameter
                {
                    IsFeeds = false,
                    DateFrom = Convert.ToDateTime(MonthYearDate.SelectedDate),
                    DateTo = lastOfThisMonth
                };

                _dateRange = Convert.ToDateTime(MonthYearDate.SelectedDate).ToString("MMM-yyyy");


                var vatExempt = await _reportEndpoint.GetPurchasesReportMonthlyVatExempt(reportParameter);
                vatExempt.TableName = "VATEXEMPT";
                var vatable = await _reportEndpoint.GetPurchasesReportMonthlyVatable(reportParameter);
                vatable.TableName = "VATABLE";

                var mainDataSet = new DataSet("Mainreport");
                mainDataSet.Tables.Add(vatExempt);
                mainDataSet.Tables.Add(vatable);

                await SaveToFile(reportParameter, mainDataSet);

                DisplayForMonthly.Text = _mtdSalesFilename;
                ViewMonthly.Visibility = Visibility.Visible;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error",MessageBoxButton.OK);
            }

        }

        private async Task OnProcessMonthly()
        {
            if(MonthYearDate.SelectedDate == null) return;
            DateTime firstOfNextMonth = Convert.ToDateTime(MonthYearDate.SelectedDate).AddMonths(1);
            DateTime lastOfThisMonth = firstOfNextMonth.AddDays(-1);

            var reportParameter = new ProjectReportParameter
            {
                IsFeeds = false,
                DateFrom = Convert.ToDateTime(MonthYearDate.SelectedDate),
                DateTo = lastOfThisMonth
            };

            _dateRange = Convert.ToDateTime(MonthYearDate.SelectedDate).ToString("MMM-yyyy");


            var vatExempt = await _reportEndpoint.GetSalesReportMonthlyBIR(reportParameter);
            vatExempt.TableName = "VATEXEMPT";
            var vatable = await _reportEndpoint.GetSalesReportMonthlyBIRVatable(reportParameter);
            vatable.TableName = "VATABLE";

            var mainDataSet = new DataSet("Mainreport");
            mainDataSet.Tables.Add(vatExempt);
            mainDataSet.Tables.Add(vatable);

            await SaveToFile(reportParameter, mainDataSet);

            DisplayForMonthly.Text = _mtdSalesFilename;
            ViewMonthly.Visibility = Visibility.Visible;

        }

        

        private async Task OnGenerateMTDSalesReport()
        {

            var isMerged = false;
            var isFirstAddress2 = true;

            var reportParameter = new ProjectReportParameter
            {
                IsFeeds = Convert.ToBoolean(IsFeeds.IsChecked),
                Address2 = "SAN JOSE DEL MONTE",
                InternalCategory = "PREMIUM",
                DateFrom = Convert.ToDateTime(DateFrom.Text),
                DateTo = Convert.ToDateTime(DateTo.Text)
            };

            var mainTable = new DataTable { TableName = "NOT-CONVERTED" };
            var dtConverted = new DataTable { TableName = "CONVERTED" };
            var dtConvertedByTown = new DataTable { TableName = "CONVERTEDBYTOWN" };
            var dtSalesPersonnel = new DataTable { TableName = "SALESPERSONNEL" };

            string aggregateLabel = "";
            var targetTotals = "";

            if (reportParameter.IsFeeds)
            {

                #region MyRegion

                List<ProjectCustomerAddress2> address2;
                List<Personnel> personnel;
                if (_selectedReport == _report1)
                {
                    address2 = await _reportEndpoint.GetCustomerAddress2();
                    personnel = await _reportEndpoint.GetPersonnel();
                    SetupTable(dtConvertedByTown, address2, dtConverted);
                    SetupPersonnelTable(dtSalesPersonnel, personnel, dtConverted);
                }
                else
                {
                    address2 = new List<ProjectCustomerAddress2>
                    {
                        new ProjectCustomerAddress2 {Address2 = "SUPPLIER"}
                    };
                }

                #endregion

                //first
                aggregateLabel = "TOTAL HOGS";
                var categories = GetListProductInternalCategories("PREMIUM,EXPERT YELLOW,EXPERT WHITE,BONANZA,JUMBO").ToList();

                targetTotals = "";
                foreach (var category in categories)
                {
                    targetTotals += $"TOTAL-{category.InternalCategory.MakeOneWord()}";
                }

                await PopulateDataTable(address2, reportParameter, categories, isMerged, mainTable, dtConvertedByTown, dtSalesPersonnel, isFirstAddress2);
                SetTotalForCategories(mainTable, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);

                isMerged = false;
                isFirstAddress2 = true;
                await PopulateDataTable(address2, reportParameter, categories, isMerged, dtConverted, dtConvertedByTown, dtSalesPersonnel, isFirstAddress2, true);
                SetTotalForCategories(dtConverted, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);


                
                //2nd 
                aggregateLabel = "TOTAL TRADE";
                categories = GetListProductInternalCategories("PUREBLEND").ToList();
                targetTotals = "";
                foreach (var category in categories)
                {
                    targetTotals += $"TOTAL-{category.InternalCategory.MakeOneWord()}";
                }

                targetTotals += "TOTAL HOGS";

                isMerged = true;
                isFirstAddress2 = true;
                await PopulateDataTable(address2, reportParameter, categories, isMerged, mainTable, dtConvertedByTown, dtSalesPersonnel, isFirstAddress2, false,true);
                SetTotalForCategories(mainTable, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);

                isMerged = true;
                isFirstAddress2 = true;
                await PopulateDataTable(address2, reportParameter, categories, isMerged, dtConverted, dtConvertedByTown, dtSalesPersonnel, isFirstAddress2, true, true);
                SetTotalForCategories(dtConverted, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);



                //3rd
                aggregateLabel = "TOTAL GAMEFOWL";
                categories = GetListProductInternalCategories("GRAINS,INTEGRA").ToList();
                targetTotals = "";
                foreach (var category in categories)
                {
                    targetTotals += $"TOTAL-{category.InternalCategory.MakeOneWord()}";
                }

                isMerged = true;
                isFirstAddress2 = true;
                await PopulateDataTable(address2, reportParameter, categories, isMerged, mainTable, dtConvertedByTown, dtSalesPersonnel, isFirstAddress2, false, true);
                SetTotalForCategories(mainTable, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);

                isMerged = true;
                isFirstAddress2 = true;
                await PopulateDataTable(address2, reportParameter, categories, isMerged, dtConverted, dtConvertedByTown, dtSalesPersonnel, isFirstAddress2, true, true);
                SetTotalForCategories(dtConverted, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);


                //4th
                aggregateLabel = "TOTAL FARM";
                categories = GetListProductInternalCategories("ESSENTIAL BROILER,PIC / ESSENTIALS").ToList();
                targetTotals = "";
                foreach (var category in categories)
                {
                    targetTotals += $"TOTAL-{category.InternalCategory.MakeOneWord()}";
                }

                isMerged = true;
                isFirstAddress2 = true;
                await PopulateDataTable(address2, reportParameter, categories, isMerged, mainTable, dtConvertedByTown, dtSalesPersonnel, isFirstAddress2, false, true);
                SetTotalForCategories(mainTable, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);

                isMerged = true;
                isFirstAddress2 = true;
                await PopulateDataTable(address2, reportParameter, categories, isMerged, dtConverted, dtConvertedByTown, dtSalesPersonnel, isFirstAddress2, true, true);
                SetTotalForCategories(dtConverted, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);


                //finally
                aggregateLabel = "OVERALL TOTAL";
                targetTotals = "TOTAL TRADETOTAL GAMEFOWLTOTAL FARM";
                SetTotalForCategories(mainTable, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);

                SetTotalForCategories(dtConverted, dtConvertedByTown, dtSalesPersonnel, aggregateLabel, targetTotals);


            }
            else
            {
                mainTable = _selectedReport == _report1 ? await _reportEndpoint.GetSalesReportSMAHC(reportParameter) : await _reportEndpoint.GetPurchasesReportSMAHC(reportParameter);

                var currentPO = "";
                var totalPOPrice = 0m;
                DataRow rowToUpdate = null;
                var overAllTotal = 0m;

                foreach (DataRow row in mainTable.Rows)
                {
                    if (currentPO != row[2].ToString())
                    {
                        if (rowToUpdate != null)
                        {
                            rowToUpdate[9] = totalPOPrice;
                        }

                        rowToUpdate = row;
                        currentPO = row[2].ToString();
                        totalPOPrice = Convert.ToDecimal(row[8]);
                    }
                    else
                    {
                        totalPOPrice += Convert.ToDecimal(row[8]);
                        rowToUpdate = row;
                    }

                    overAllTotal += Convert.ToDecimal(row[8]);

                }

                _overAllTotal = overAllTotal.ToString();

                if (rowToUpdate != null)
                {
                    rowToUpdate[9] = totalPOPrice;
                }
                
                mainTable.TableName = "SMAHC";
            }


            var mainDataSet = new DataSet("Mainreport");
            if (_selectedReport == _report1 && reportParameter.IsFeeds)
            {
                mainTable.Columns.Remove("PersonnelId");
            }
            mainDataSet.Tables.Add(mainTable);

            if (reportParameter.IsFeeds)
            {
                if (_selectedReport == _report1 && reportParameter.IsFeeds)
                {
                    dtConverted.Columns.Remove("PersonnelId");
                    dtSalesPersonnel.Columns.Remove("ID");
                }
                mainDataSet.Tables.Add(dtConverted);

                if (_selectedReport == _report1 && reportParameter.IsFeeds)
                {
                    mainDataSet.Tables.Add(dtConvertedByTown);
                    mainDataSet.Tables.Add(dtSalesPersonnel);
                }
            }

            await SaveToFile(reportParameter, mainDataSet);

            GeneratedMTDSales.Text = _mtdSalesFilename;
            ViewMTDSales.Visibility = Visibility.Visible;
        }

        private void UpdateRowValue(DataTable dtConvertedByTown, string address, string column, double value)
        {
            try
            {
                if (dtConvertedByTown.Columns.Count > 0)
                {
                    var expression = $"TOWN='{address.Replace("'", "''")}'";
                    var rowToUpdate = dtConvertedByTown.Select(expression).FirstOrDefault();
                    if (rowToUpdate == null) return;
                    rowToUpdate[column] = value;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void UpdatePersonnelRowValue(DataTable dtSalesPersonnel, int personnelId, string category, float value)
        {
            try
            {
                if (dtSalesPersonnel.Columns.Count > 0)
                {
                    var expression = $"ID={personnelId}";
                    var rowToUpdate = dtSalesPersonnel.Select(expression).FirstOrDefault();
                    if (rowToUpdate == null) return;
                    if (rowToUpdate.IsNull(category) == false)
                        rowToUpdate[category] = Convert.ToInt64(rowToUpdate[category])  + value;
                    else
                        rowToUpdate[category] = value;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void SetupPersonnelTable(DataTable dtSalesPersonnel, List<Personnel> personnel, DataTable dtSource)
        {

            dtSalesPersonnel.Columns.Add("ID", typeof(String)); //Shall be removed on the Generation of report
            dtSalesPersonnel.Columns.Add("SALES PERSONNEL", typeof(String));
            dtSalesPersonnel.Columns.Add("POSITION", typeof(String));
            dtSalesPersonnel.Columns.Add("TOTAL-PREMIUM", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL-EXPERTYELLOW", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL-EXPERTWHITE", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL-BONANZA", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL-JUMBO", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL HOGS", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL-PUREBLEND", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL TRADE", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL-GRAINS", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL-INTEGRA", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL GAMEFOWL", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL-ESSENTIALBROILER", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL-PICESSENTIALS", typeof(Double));
            dtSalesPersonnel.Columns.Add("TOTAL FARM", typeof(Double));
            dtSalesPersonnel.Columns.Add("OVERALL TOTAL", typeof(Double));

            var pos = 0;
            dtSalesPersonnel.Rows.Clear();
            foreach (var customerPersonnel in personnel)
            {
                var dr = dtSalesPersonnel.NewRow(); //Create New Row
                dr[0] = customerPersonnel.Id; // Set Column Value
                dr[1] = customerPersonnel.Name; // Set Column Value
                dr[2] = customerPersonnel.Position; // Set Column Value
                dtSalesPersonnel.Rows.InsertAt(dr, pos); // InsertAt specified position
                pos++;
            }

            Console.WriteLine(pos);

        }
        private void SetupTable(DataTable dtConvertedByTown, List<ProjectCustomerAddress2> address2, DataTable dtSource)
        {
            //dtConvertedByTown.Columns.Add(currentCategory);
            //var dr = mainTable.NewRow(); //Create New Row
            //dr[1] = currentAddress; // Set Column Value
            //mainTable.Rows.InsertAt(dr, 0); // InsertAt specified position

            //DataColumn workCol = workTable.Columns.Add("CustID", typeof(Int32));
            //workCol.AllowDBNull = false;
            //workCol.Unique = true;
            //workTable.Columns.Add("CustFName", typeof(String));
            //workTable.Columns.Add("Purchases", typeof(Double));

            //aggregateLabel = "TOTAL HOGS";
            //var categories = GetListProductInternalCategories("PREMIUM,EXPERT YELLOW,EXPERT WHITE,BONANZA,JUMBO").ToList();

            //aggregateLabel = "TOTAL TRADE";
            //categories = GetListProductInternalCategories("PUREBLEND").ToList();

            //aggregateLabel = "TOTAL GAMEFOWL";
            //categories = GetListProductInternalCategories("GRAINS,INTEGRA").ToList();

            //aggregateLabel = "TOTAL FARM";
            //categories = GetListProductInternalCategories("ESSENTIAL BROILER,PIC / ESSENTIALS").ToList();


            //aggregateLabel = "OVERALL TOTAL";


            dtConvertedByTown.Columns.Add("TOWN", typeof(String));
            dtConvertedByTown.Columns.Add("TOTAL-PREMIUM", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL-EXPERTYELLOW", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL-EXPERTWHITE", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL-BONANZA", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL-JUMBO", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL HOGS", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL-PUREBLEND", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL TRADE", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL-GRAINS", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL-INTEGRA", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL GAMEFOWL", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL-ESSENTIALBROILER", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL-PICESSENTIALS", typeof(Double));
            dtConvertedByTown.Columns.Add("TOTAL FARM", typeof(Double));
            dtConvertedByTown.Columns.Add("OVERALL TOTAL", typeof(Double));

            var pos = 0;
            dtConvertedByTown.Rows.Clear();
            foreach (var customerAddress2 in address2)
            {
                var dr = dtConvertedByTown.NewRow(); //Create New Row
                dr[0] = customerAddress2.Address2; // Set Column Value
                dtConvertedByTown.Rows.InsertAt(dr, pos); // InsertAt specified position
                pos++;
            }

            Console.WriteLine(pos);

        }

        private void SetTotalForCategories(DataTable mainTable, DataTable dtConvertedByTown, DataTable dtSalesPersonnel, string aggregateLabel, string targetTotals)
        {
            mainTable.Columns.Add(aggregateLabel);

            string expression;
            DataRow rowToUpdate = null;  
            float rowTotal = 0;
            
            foreach (DataRow row in mainTable.Rows)
            {
                
                try
                {
                    expression = _selectedReport == _report1 ?  $"customerName='{row[1].ToString().Replace("'","''")}'" : $"supplierName='{row[1].ToString().Replace("'", "''")}'";
                    rowToUpdate = mainTable.Select(expression).FirstOrDefault();
                    if (rowToUpdate == null) continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                rowTotal = 0;

                
                for (var i = _startingColumn; i < mainTable.Columns.Count; i++)
                {
                    if(targetTotals.Contains(mainTable.Columns[i].ColumnName))
                    {
                        if(row.IsNull(mainTable.Columns[i].ColumnName) == false)
                            rowTotal += Convert.ToSingle(row[i]);
                    }
                }

                rowToUpdate[aggregateLabel] = rowTotal;
                if (!mainTable.TableName.Equals("CONVERTED")) continue;
                var value = row[0];
                if (value == DBNull.Value)
                {
                    UpdateRowValue(dtConvertedByTown, address: row[1].ToString(), column: aggregateLabel,value: rowTotal);
                }

            }

            if (_selectedReport != _report1 || !mainTable.TableName.Equals("CONVERTED")) return;

            foreach (DataRow row in dtSalesPersonnel.Rows)
            {

                try
                {
                    expression = $"ID={Convert.ToSingle(row[0])}";
                    rowToUpdate = dtSalesPersonnel.Select(expression).FirstOrDefault();
                    if (rowToUpdate == null) continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                rowTotal = 0;

                for (var i = _startingColumn + 1; i < dtSalesPersonnel.Columns.Count; i++)
                {
                    if (targetTotals.Contains(dtSalesPersonnel.Columns[i].ColumnName))
                    {
                        if (row.IsNull(dtSalesPersonnel.Columns[i].ColumnName) == false)
                            rowTotal += Convert.ToSingle(row[i]);
                    }
                }

                rowToUpdate[aggregateLabel] = rowTotal;

            }

        }

        private IEnumerable<ProductInternalCategory> GetListProductInternalCategories(string value)
        {
            var result = new List<ProductInternalCategory>();
            var categories = value.Split(',');
            foreach (var s in categories)
            {
                result.Add(new ProductInternalCategory{InternalCategory = s});
            }
            return result;
        }

        private async Task SaveToFile(ProjectReportParameter reportParameter, DataSet mainDataSet)
        {
            var reportType = reportParameter.IsFeeds ? "FEEDS" : "VET-MED";
            if(_selectedReport==_report1)
            {
                _mtdSalesFilename =
                    $"MTD-SalesReport-{reportType}-AsOf-{DateTime.Now.ToString("ddMMMyyyy-hhmmss")}.xlsx";
            }
            else if (_selectedReport == _report2)
            {
                _mtdSalesFilename =
                    $"MTD-PurchaseReport-{reportType}-AsOf-{DateTime.Now.ToString("ddMMMyyyy-hhmmss")}.xlsx";

            }
            else if (_selectedReport == _report3)
            {
                _mtdSalesFilename =
                    $"MonthlySalesReport-BIR-AsOf-{DateTime.Now.ToString("ddMMMyyyy-hhmmss")}.xlsx";
            }
            else if (_selectedReport == _report4)
            {
                _mtdSalesFilename =
                    $"MonthlyPurchaseReport-BIR-AsOf-{DateTime.Now.ToString("ddMMMyyyy-hhmmss")}.xlsx";

            }
            else if (_selectedReport == _report5)
            {
                _mtdSalesFilename =
                    $"CustomerAccountSummaryReport-AsOf-{DateTime.Now.ToString("ddMMMyyyy-hhmmss")}.xlsx";

            }

            if (_selectedReport == _report1 || _selectedReport == _report2 || _selectedReport == _report5)
            {
                _mtdSalesFolder = Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("ddMMMyyyy");
            }
            else
            {
                _mtdSalesFolder = Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("MMMyyyy");
            }

            if (Directory.Exists(_mtdSalesFolder) == false)
                Directory.CreateDirectory(_mtdSalesFolder);


            _mtdSalesPathFilename = _mtdSalesFolder + "\\" + _mtdSalesFilename;
            await _excelHelper.ExportDSToExcel(mainDataSet, _mtdSalesPathFilename,  _overAllTotal, _dateRange, _reportTypeEnum, _reportTIN);

        }

        private async Task PopulateDataTable(List<ProjectCustomerAddress2> address2, ProjectReportParameter reportParameter, List<ProductInternalCategory> categories,
            bool isMerged, DataTable mainTable,DataTable dtConvertedByTown, DataTable dtSalesPersonnel, bool isFirstAddress2, bool isConverted=false, bool isOverrideIsFirstCategory = false)
        {
            DataRow rowToUpdate;
            DataTable reportTable;
            string currentAddress;
            bool isFirstCategory;
            string currentCategory;
            string expression;

            foreach (var customerAddress2 in address2)
            {
                reportParameter.Address2 = customerAddress2.Address2;
                currentAddress = reportParameter.Address2 == string.Empty ? "BLANK ADDRESS2" : reportParameter.Address2;
                isFirstCategory = true;
                if (isOverrideIsFirstCategory)
                    isFirstCategory = false;

                foreach (var category in categories)
                {
                    reportParameter.InternalCategory = category.InternalCategory;
                    reportTable = await SetReportTable(reportParameter, isConverted);
                    currentCategory = $"TOTAL-{category.InternalCategory.MakeOneWord()}";

                    if (reportTable.Rows.Count > 0)
                    {
                        try
                        {
                            if (isMerged == false)
                            {
                                mainTable.Merge(reportTable);
                                mainTable.Columns.Add(currentCategory);
                                var dr = mainTable.NewRow(); //Create New Row
                                dr[1] = currentAddress; // Set Column Value
                                mainTable.Rows.InsertAt(dr, 0); // InsertAt specified position
                                isMerged = true;

                                float rowTotal = 0;
                                var colTotal = new float[reportTable.Columns.Count];
                                foreach (DataRow row in reportTable.Rows)
                                {
                                    expression = _selectedReport == _report1 ? $"customerId='{row[0]}'" : $"supplierId='{row[0]}'";
                                    rowToUpdate = mainTable.Select(expression).FirstOrDefault();
                                    rowTotal = 0;

                                    if (rowToUpdate == null) continue;
                                    for (var i = _startingColumn; i < reportTable.Columns.Count; i++)
                                    {
                                        rowTotal += Convert.ToSingle(row[i]);
                                        colTotal[i] += Convert.ToSingle(row[i]);

                                        if (mainTable.TableName.Equals("CONVERTED"))
                                            UpdatePersonnelRowValue(dtSalesPersonnel, personnelId: Convert.ToInt32(row[2]), category: currentCategory, value: Convert.ToSingle(row[i]));

                                    }

                                    rowToUpdate[currentCategory] = rowTotal;
                                    
                                    
                                }

                                expression = _selectedReport == _report1 ? $"customerName='{currentAddress}'" : $"supplierName='{currentAddress}'";
                                rowToUpdate = mainTable.Select(expression).FirstOrDefault();
                                if (rowToUpdate != null)
                                {
                                    rowTotal = 0;
                                    for (var i = _startingColumn; i < colTotal.Length; i++)
                                    {
                                        rowToUpdate[reportTable.Columns[i].ColumnName] = colTotal[i];
                                        rowTotal += Convert.ToSingle(colTotal[i]);
                                    }

                                    rowToUpdate[currentCategory] = rowTotal;
                                    if (mainTable.TableName.Equals("CONVERTED"))
                                        UpdateRowValue(dtConvertedByTown, address: currentAddress, column: currentCategory, value: rowTotal);
                                }
                            }

                            else
                            {
                                if (isFirstAddress2)
                                {
                                    for (var i = _startingColumn; i < reportTable.Columns.Count; i++)
                                    {
                                        mainTable.Columns.Add((string) reportTable.Columns[i].ColumnName);
                                    }

                                    mainTable.Columns.Add(currentCategory);

                                    //populate the new added columns
                                    InsertAndArrangeDataTable(reportTable, mainTable, currentAddress, currentCategory, dtConvertedByTown, dtSalesPersonnel);

                                }
                                else
                                {
                                    //shall add new rows if detected the new category is currently processing it
                                    if (isFirstCategory)
                                    {
                                        var dr = mainTable.NewRow(); //Create New Row
                                        dr[1] = currentAddress; // Set Column Value
                                        mainTable.Rows.Add(dr);

                                        for (var i = 0; i < reportTable.Rows.Count; i++)
                                        {
                                            dr = mainTable.NewRow();
                                            dr[0] = reportTable.Rows[i][0];
                                            dr[1] = reportTable.Rows[i][1];
                                            dr[2] = reportTable.Rows[i][2];
                                            mainTable.Rows.Add(dr);
                                        }
                                    }

                                    //populate the new added columns
                                    InsertAndArrangeDataTable(reportTable, mainTable, currentAddress, currentCategory, dtConvertedByTown, dtSalesPersonnel);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        if (isFirstCategory)
                            isFirstCategory = false;
                    }
                }

               
                if (isFirstAddress2)
                    isFirstAddress2 = false;
            }
        }

        

        private async Task<DataTable> SetReportTable(ProjectReportParameter reportParameter, bool isConverted)
        {
            DataTable reportTable;

            if(_selectedReport==_report1)
            {
                if (isConverted)
                {
                    reportTable = await _reportEndpoint.GetMTDSalesReportConverted(reportParameter);
                }
                else
                {
                    reportTable = await _reportEndpoint.GetMonthToDateSalesReport(reportParameter);
                }

            }
            else
            {
                if (isConverted)
                {
                    reportTable = await _reportEndpoint.GetPurchaseReportMTDConverted(reportParameter);
                }
                else
                {
                    reportTable = await _reportEndpoint.GetPurchaseReportMTD(reportParameter);
                }

            }
            return reportTable;
        }

        private void InsertAndArrangeDataTable(DataTable reportTable, DataTable mainTable, string currentAddress, string currentCategory, DataTable dtConvertedByTown, DataTable dtSalesPersonnel)
        {
            string expression;
            DataRow rowToUpdate;
            float rowTotal = 0;
            var colTotal = new float[reportTable.Columns.Count];
            foreach (DataRow row in reportTable.Rows)
            {
                expression = _selectedReport == _report1 ? $"customerId='{row[0]}'" : $"supplierId='{row[0]}'";
                rowToUpdate = mainTable.Select(expression).FirstOrDefault();
                rowTotal = 0;

                if (rowToUpdate == null) continue;
                for (var i = _startingColumn; i < reportTable.Columns.Count; i++)
                {
                    rowToUpdate[reportTable.Columns[i].ColumnName] = row[i];
                    rowTotal += Convert.ToSingle(row[i]);
                    colTotal[i] += Convert.ToSingle(row[i]);

                    if (mainTable.TableName.Equals("CONVERTED"))
                        UpdatePersonnelRowValue(dtSalesPersonnel, personnelId: Convert.ToInt32(row[2]), category: currentCategory, value: Convert.ToSingle(row[i]));

                }

                rowToUpdate[currentCategory] = rowTotal;

            }

            expression = _selectedReport == _report1 ? $"customerName='{currentAddress}'" : $"supplierName='{currentAddress}'";
            rowToUpdate = mainTable.Select(expression).FirstOrDefault();
            if (rowToUpdate != null)
            {
                rowTotal = 0;
                for (var i = _startingColumn; i < colTotal.Length; i++)
                {
                    rowToUpdate[reportTable.Columns[i].ColumnName] = decimal.Round(Convert.ToDecimal(colTotal[i]), 2, MidpointRounding.AwayFromZero);
                    rowTotal += Convert.ToSingle(colTotal[i]);
                }
                rowToUpdate[currentCategory] = rowTotal;
                if (mainTable.TableName.Equals("CONVERTED"))
                    UpdateRowValue(dtConvertedByTown, address: currentAddress, column: currentCategory, value: rowTotal);

            }
        }

        private void OnMyComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == null) return;
            _selectedReport = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;

            ReportLabel.Visibility = Visibility.Collapsed;
            Report1Parameter.Visibility = Visibility.Collapsed;
            Report2Parameter.Visibility = Visibility.Collapsed;
            Generate.IsEnabled = false;

            switch (_selectedReport)
            {
                case _report1:
                    _reportTypeEnum = ReportTypeEnum.SalesMTDFeeds;
                    DateFrom.Text = $"01-{DateTime.Now.ToString("MMM-yyyy")}";
                    DateTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    _dateRange = $"{DateFrom.Text} till {DateTo.Text}";

                    IsFeeds.IsChecked = true;
                    Report1Parameter.Visibility = Visibility.Visible;
                    ViewMTDSales.Visibility = Visibility.Collapsed;
                    Generate.IsEnabled = true;
                    break;
                case _report2:
                    _reportTypeEnum = ReportTypeEnum.PurchasesMTDFeeds;
                    DateFrom.Text = $"01-{DateTime.Now.ToString("MMM-yyyy")}";
                    DateTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    _dateRange = $"{DateFrom.Text} till {DateTo.Text}";

                    IsFeeds.IsChecked = true;
                    Report1Parameter.Visibility = Visibility.Visible;
                    ViewMTDSales.Visibility = Visibility.Collapsed;
                    Generate.IsEnabled = true;
                    break;

                case _report3:
                    _reportTypeEnum = ReportTypeEnum.SalesMonthlyBIR;
                    Report2Parameter.Visibility = Visibility.Visible;
                    Generate.IsEnabled = MonthYearDate.SelectedDate != null;
                    ViewMonthly.Visibility = Visibility.Collapsed;
                    break;

                case _report4:
                    _reportTypeEnum = ReportTypeEnum.PurchasesMonthlyBIR;
                    Report2Parameter.Visibility = Visibility.Visible;
                    Generate.IsEnabled = MonthYearDate.SelectedDate != null;
                    ViewMonthly.Visibility = Visibility.Collapsed;
                    break;

                case _report5:
                    _reportTypeEnum = ReportTypeEnum.CustomerAccountSummary;
                    DateFrom.Text = $"01-{DateTime.Now.AddMonths(-2).ToString("MMM-yyyy")}";
                    DateTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    _dateRange = $"{DateFrom.Text} till {DateTo.Text}";

                    IsFeeds.Visibility = Visibility.Collapsed;
                    Report1Parameter.Visibility = Visibility.Visible;
                    ViewMTDSales.Visibility = Visibility.Collapsed;
                    Generate.IsEnabled = true;
                    break;
            }

        }

        private void ViewMTDSales_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(_mtdSalesPathFilename);
        }

        private void MonthYearDate_OnCalendarClosed(object sender, RoutedEventArgs e)
        {
            Generate.IsEnabled = MonthYearDate.SelectedDate != null;
        }

        private void IsFeeds_Checked(object sender, RoutedEventArgs e)
        {
            switch (_selectedReport)
            {
                case (_report1):
                    _reportTypeEnum = Convert.ToBoolean(IsFeeds.IsChecked) ? ReportTypeEnum.SalesMTDFeeds : ReportTypeEnum.SalesMTDVet;
                    break;
                case (_report2):
                    _reportTypeEnum = Convert.ToBoolean(IsFeeds.IsChecked) ? ReportTypeEnum.PurchasesMTDFeeds : ReportTypeEnum.PurchasesMTDVet;
                    break;
            }

        }

      
    }
}
