using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.Reporting.WinForms;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.Report;
using Project.FC2J.UI.Helpers.Excel;
using Project.FC2J.UI.Helpers.Reports;
using DataTable = System.Data.DataTable;

namespace Project.FC2J.UI.Reports
{
    /// <summary>
    /// Interaction logic for ReportViewerScreen.xaml
    /// </summary>
    public partial class ReportViewerScreen : Window
    {
        private int _reportType = 0;
        private IExcelHelper _excelHelper;
        private IReportEndpoint _reportEndpoint;

        public ReportViewerScreen(IReportEndpoint reportEndpoint, IExcelHelper excelHelper)
        {
            InitializeComponent();
            _reportEndpoint = reportEndpoint;
            _excelHelper = excelHelper;
            OnPageSettings();
            InventoryDate.SelectedDate = DateTime.Today;
        }

        private void OnPageSettings()
        {
            var pageSetting = new PageSettings
            {
                Margins = new Margins
                {
                    Top = 0,
                    Bottom = 0,
                    Left = 0,
                    Right = 0
                }
            };

            var paperSize = new PaperSize
            {
                RawKind = (int)PaperKind.Letter
            };

            pageSetting.PaperSize = paperSize;
            ReportViewer.SetPageSettings(pageSetting);
            ReportViewer.RefreshReport();
            ReportViewer.Reset();
            //03Feb2020

        }


        private async void Generate_OnClick(object sender, RoutedEventArgs e)
        {
            Generate.IsEnabled = false;
            OverlayLoading.Visibility = Visibility.Visible;

            ReportViewer.Reset();
            System.Data.DataTable dt = new System.Data.DataTable();
            _reportType = 2;
            switch (_reportType)
            {
                case 1: //Sales Order
                    dt = _reportEndpoint.ToDataTable(await _reportEndpoint.GetSalesReport());
                    break;
                case 2: //Daily Inventory
                    await OnGenerateInventory();
                    Generate.IsEnabled = true;
                    OverlayLoading.Visibility = Visibility.Collapsed;
                    return;
            }

            ReportDataSource ds1 = new ReportDataSource("DataSet1",dt);
            ReportViewer.LocalReport.DataSources.Add(ds1);
            switch (_reportType)
            {
                case 1: //Sales Order
                    ReportViewer.LocalReport.ReportPath = "Reports/SalesOrder.rdlc";
                    break;
                case 2: //Daily Inventory
                    //ReportViewer.LocalReport.ReportPath = "Reports/DailyInventory.rdlc";
                    break;
            }

            ReportViewer.LocalReport.Refresh();
            ReportViewer.RefreshReport();
            Generate.IsEnabled = true;
        }

        private async Task OnGenerateInventory()
        {

            var date = DateTime.Now;

            try
            {
                date = Convert.ToDateTime(InventoryDate.SelectedDate);
            }
            catch (Exception e)
            {
                MessageBox.Show("Please Select Valid Date", "Invalid Date", MessageBoxButton.OK);
                return;
            }


            var categories = await _reportEndpoint.GetCategoryArrangement(); // common to all 
            var products = await _reportEndpoint.GetInventoryProducts();


            // Create 2 DataTable instances.
            var table1 = new DataTable("inventory-"+ date.ToString("ddMMMyyyy"));
            var sourceId = 7;//initial for the summary table 
            var customers = await _reportEndpoint.GetDailyInventoryCustomers(date.ToString("dd-MMMM-yyyy"), sourceId);
            var inventories = await _reportEndpoint.GetDailyInventory(date.ToString("dd-MMMM-yyyy"), sourceId);
            ProcessTableDetails(table1, date, customers, categories, products, inventories);

            var tableCoron = new DataTable("Coron-" + date.ToString("ddMMMyyyy"));
            sourceId = 0;
            customers = await _reportEndpoint.GetDailyInventoryCustomers(date.ToString("dd-MMMM-yyyy"), sourceId);
            inventories = await _reportEndpoint.GetDailyInventory(date.ToString("dd-MMMM-yyyy"), sourceId);
            ProcessTableDetails(tableCoron, date, customers, categories, products, inventories);

            var tableLubang = new DataTable("Lubang-" + date.ToString("ddMMMyyyy"));
            sourceId = 1;
            customers = await _reportEndpoint.GetDailyInventoryCustomers(date.ToString("dd-MMMM-yyyy"), sourceId);
            inventories = await _reportEndpoint.GetDailyInventory(date.ToString("dd-MMMM-yyyy"), sourceId);
            ProcessTableDetails(tableLubang, date, customers, categories, products, inventories);

            var tableSanIldefonso = new DataTable("SanIldefonso-" + date.ToString("ddMMMyyyy"));
            sourceId = 2;
            customers = await _reportEndpoint.GetDailyInventoryCustomers(date.ToString("dd-MMMM-yyyy"), sourceId);
            inventories = await _reportEndpoint.GetDailyInventory(date.ToString("dd-MMMM-yyyy"), sourceId);
            ProcessTableDetails(tableSanIldefonso, date, customers, categories, products, inventories);

            var set = new DataSet("inventory");
            set.Tables.Add(table1);
            set.Tables.Add(tableCoron);
            set.Tables.Add(tableLubang);
            set.Tables.Add(tableSanIldefonso);

            var filename = $"DailyInventory{date.ToString("ddMMMyyyy")}-GeneratedAsof{DateTime.Now.ToString("ddMMMyyyy-hhmmss")}.xlsx";
            var folder = Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("ddMMMyyyy");
            if (Directory.Exists(folder) == false)
                Directory.CreateDirectory(folder);
            _pathFilename = folder + "\\" + filename;
            await _excelHelper.ExportDSToExcel(set, _pathFilename);

            Open.Text = filename;
            OpenInventoryReport.Visibility = Visibility.Visible;
        }

        private static void ProcessTableDetails(DataTable table1, DateTime date, List<DailyInventoryCustomer> customers, List<ProductInternalCategory> categories, List<InventoryProduct> products,
            List<DailyInventory> inventories)
        {
            table1.Columns.Add("DATE: " + date.ToString("MM-dd-yyyy"));
            table1.Columns.Add(" "); //string or sinulid

            //template row value
            var convertedFlag = table1.TableName.Contains("inventory-");


            #region Setup header of excel
            var rowValue = new List<string> {""};
            var isSupplyDeliveryMarked = false;

            foreach (var customer in customers)
            {
                if (customer.CustomerId.Trim().Equals("0"))
                {
                    table1.Columns.Add("Beginning Balance");
                }
                else if (customer.CustomerId.Trim().Equals("Z"))
                {
                    table1.Columns.Add("[Supply Delivery]");
                    isSupplyDeliveryMarked = true;
                }
                else
                {
                    var customerName = $"{customer.CustomerName.Trim()}_{customer.CustomerId}";
                    table1.Columns.Add(customerName); 
                }

                rowValue.Add(""); //template row value
            }

            if (isSupplyDeliveryMarked == false)
            {
                table1.Columns.Add("[Supply Delivery]");
                rowValue.Add(""); //template row value
            }

            table1.Columns.Add("Total");
            rowValue.Add(""); 

            if (convertedFlag)
                table1.Columns.Add("CONVERTED TO 50KGS");

            #endregion

            //process the rows 

            #region insert products records per category

            IEnumerable<InventoryProduct> stageProducts;

            foreach (var category in categories)
            {
                stageProducts = products.Where(item => item.Category.Equals(category.InternalCategory));
                {
                    //SMAHC
                    if (convertedFlag)
                    {
                        if (category.InternalCategory.Equals("SMAHC", StringComparison.CurrentCultureIgnoreCase))
                        {
                            //one empty
                            var addedRow = new List<string> { "TOTAL" };
                            addedRow.AddRange(rowValue);
                            table1.Rows.Add(addedRow.ToArray());

                            for (int me = 0; me < 3; me++)
                            {
                                addedRow = new List<string> { string.Empty };
                                addedRow.AddRange(rowValue);
                                table1.Rows.Add(addedRow.ToArray());
                            }
                        }
                    }


                    //insert category row with empty fields on the right
                    var stageRow = new List<string> {category.InternalCategory};
                    stageRow.AddRange(rowValue);
                    table1.Rows.Add(stageRow.ToArray());

                    foreach (var product in stageProducts)
                    {
                        //adding individual product row under the current category 
                        stageRow = new List<string> {product.Name};
                        stageRow.AddRange(rowValue);
                        if (convertedFlag)
                            stageRow.Add(product.KiloPerUnit.ToString("N2"));
                        table1.Rows.Add(stageRow.ToArray());
                    }

                    //adding new line 
                    stageRow = new List<string> {string.Empty};
                    stageRow.AddRange(rowValue);
                    table1.Rows.Add(stageRow.ToArray());
                }
            }

            #endregion

            decimal value;
            //update using the actual inventory 
            foreach (var inventory in inventories)
            {
                var dataRow = table1.AsEnumerable().FirstOrDefault(r => r[0].ToString() == inventory.ProductName);
                if (dataRow != null)
                {
                    // code
                    if (inventory.CustomerId == "0")
                    {
                        decimal.TryParse(dataRow[2].ToString(), out value);
                        dataRow[2] = (value + inventory.Quantity).ToString("N2");
                    }
                    else if (inventory.CustomerId == "Z")
                    {
                        var offset = convertedFlag ? 3 : 2;
                        decimal.TryParse(dataRow[table1.Columns.Count - offset].ToString(), out value);
                        dataRow[table1.Columns.Count - offset] = (value + inventory.Quantity).ToString("N2");
                    }
                    else
                    {
                        // need to search target column 
                        var i = -1;
                        var customerName = $"{inventory.CustomerName.Trim()}_{inventory.CustomerId}";
                        foreach (DataColumn column in table1.Columns)
                        {
                            i += 1;
                            if (column.ColumnName.Trim().Equals(customerName))
                            {
                                break;
                            }
                        }

                        decimal.TryParse(dataRow[i].ToString(), out value);
                        dataRow[i] = (value + inventory.Quantity).ToString("N2");
                    }
                }
            }

            decimal totalConverted =0 ;
            foreach (DataRow row in table1.Rows)
            {
                bool hasValue = false;
                var _total = (decimal) 0;

                for (int i = 2; i < table1.Columns.Count - 1; i++)
                {
                    if (convertedFlag)
                    {
                        if (i == 2 || i == table1.Columns.Count - 3)
                        {
                            decimal.TryParse(row[i].ToString(), out value);
                            _total += value;
                        }
                        else
                        {
                            decimal.TryParse(row[i].ToString(), out value);
                            _total -= value;
                        }
                    }
                    else
                    {
                        if (i == 2 || i == table1.Columns.Count - 2)
                        {
                            decimal.TryParse(row[i].ToString(), out value);
                            _total += value;
                        }
                        else
                        {
                            decimal.TryParse(row[i].ToString(), out value);
                            _total -= value;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(row[i].ToString()) == false)
                    {
                        hasValue = true;
                    }
                }

                if (!hasValue) continue;

                if (convertedFlag)
                {
                    //this holds the kilo per unit
                    decimal.TryParse(row[table1.Columns.Count - 1].ToString(), out value);

                    //total 
                    row[table1.Columns.Count - 2] = _total.ToString("N2");


                    if (value > 0)
                    {
                        var converted = (_total * value) / 50;
                        //convert per 50 kg
                        totalConverted += converted;
                        row[table1.Columns.Count - 1] = converted.ToString("N4");
                    }
                    else
                        row[table1.Columns.Count - 1] = string.Empty;

                }
                else
                    row[table1.Columns.Count - 1] = _total.ToString("N2");
            }

            if (!convertedFlag) return;
            {
                var totalRowConverted = table1.AsEnumerable().FirstOrDefault(r => r[0].ToString() == "TOTAL");
                if (totalRowConverted != null)
                {
                    totalRowConverted[table1.Columns.Count - 1] = totalConverted.ToString("N4");
                }
            }

        }

        private string _pathFilename = Directory.GetCurrentDirectory();

        private void OpenInventoryReport_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(_pathFilename);
        }
    }

   
}
