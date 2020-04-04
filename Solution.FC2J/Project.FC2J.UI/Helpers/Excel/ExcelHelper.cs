using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Project.FC2J.UI.Helpers.Excel
{
    public class ExcelHelper : IExcelHelper
    {
        public async Task ExportDSToExcel(DataSet ds, string destination, string overAllTotal = "", string dateRange = "", ReportTypeEnum reportTypeEnum = ReportTypeEnum.Inventory, string tin = "")
        {
            await Task.Run( () =>
            {
                using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = workbook.AddWorkbookPart();
                    workbook.WorkbookPart.Workbook = new Workbook {Sheets = new Sheets()};

                    uint sheetId = 1;

                    foreach (DataTable table in ds.Tables)
                    {
                        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                        var sheetData = new SheetData();
                        sheetPart.Worksheet = new Worksheet(sheetData);

                        Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                        string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                        if (sheets.Elements<Sheet>().Count() > 0)
                        {
                            sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        var sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                        sheets.Append(sheet);

                        //setup header labels
                        overAllTotal = GetExcelSheetHeader(overAllTotal, dateRange, reportTypeEnum, sheetData, table, tin);

                        var headerRow = new Row();

                        List<String> columns = new List<string>();
                        foreach (DataColumn column in table.Columns)
                        {
                            columns.Add(column.ColumnName);

                            var cell = new Cell
                            {
                                DataType = CellValues.String,
                                CellValue = new CellValue(column.ColumnName.ToUpper())
                            };
                            headerRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(headerRow);

                        foreach (DataRow dsrow in table.Rows)
                        {
                            var newRow = new Row();
                            foreach (String col in columns)
                            {
                                var cell = new Cell();

                                ResolveCellDataType(table, col, cell, reportTypeEnum);
                                var value = cell.DataType == CellValues.Number
                                    ? dsrow[col].ToString().Replace(",","")
                                    : dsrow[col].ToString();
                                cell.CellValue = new CellValue(value); //
                                newRow.AppendChild(cell);
                            }

                            sheetData.AppendChild(newRow);
                        }
                    }
                }
            });
            
        }

        private static string GetExcelSheetHeader(string overAllTotal, string dateRange, ReportTypeEnum reportTypeEnum,
            SheetData sheetData, DataTable table, string tin)
        {
            switch (reportTypeEnum)
            {
                case ReportTypeEnum.SalesMTDFeeds:
                    if (table.TableName.Equals("CONVERTEDBYTOWN") || table.TableName.Equals("SALESPERSONNEL"))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var headerListRow = new Row();
                            var cellList = new Cell { DataType = CellValues.String };
                            if (i == 0)
                                cellList.CellValue =
                                    new CellValue($"DATE AS OF: {DateTime.Now.ToString("dd MMM yyyy")}");
                            else if (i == 1)
                                cellList.CellValue = new CellValue("");
                            else if (i == 2)
                                cellList.CellValue = new CellValue("");

                            headerListRow.AppendChild(cellList);
                            sheetData.AppendChild(headerListRow);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var headerListRow = new Row();
                            var cellList = new Cell { DataType = CellValues.String };
                            if (i == 0)
                                cellList.CellValue = new CellValue($"Volume Monitoring per Dealer");
                            else if (i == 1)
                                cellList.CellValue = new CellValue("");
                            else if (i == 2)
                                cellList.CellValue = new CellValue("");

                            headerListRow.AppendChild(cellList);
                            sheetData.AppendChild(headerListRow);
                        }
                    }
                    break;
                case ReportTypeEnum.SalesMTDVet:

                    #region ReportTypeEnum.SalesMTDVet

                    for (int i = 0; i < 4; i++)
                    {
                        Row headerListRow = new Row();
                        Cell cellList = new Cell();
                        cellList.DataType = CellValues.String;
                        switch (i)
                        {
                            case 0:
                                cellList.CellValue = new CellValue("SAN MIGUEL FOODS, INC.");
                                break;
                            case 1:
                                cellList.CellValue = new CellValue("ANIMAL HEALTH CARE");
                                break;
                            case 2:
                                cellList.CellValue = new CellValue(dateRange);
                                break;
                            case 3:
                                cellList.CellValue = new CellValue("");
                                break;
                        }

                        headerListRow.AppendChild(cellList);

                        if (i == 2)
                        {
                            for (int y = 1; y < 9; y++)
                            {
                                cellList = new Cell();
                                cellList.DataType = CellValues.String;
                                cellList.CellValue = new CellValue(string.Empty);
                                headerListRow.AppendChild(cellList);
                            }

                            cellList = new Cell();
                            cellList.DataType = CellValues.Number;
                            cellList.CellValue = new CellValue(overAllTotal);
                            headerListRow.AppendChild(cellList);
                        }

                        sheetData.AppendChild(headerListRow);
                    }

                    #endregion

                    break;
                case ReportTypeEnum.SalesMonthlyBIR:

                    if (table.TableName.Equals("VATEXEMPT"))
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            var headerListRow = new Row();
                            var cellList = new Cell();
                            cellList.DataType = CellValues.String;
                            if (i == 0)
                                cellList.CellValue = new CellValue($"{dateRange}-VAT EXEMPT");
                            else if (i == 1) cellList.CellValue = new CellValue("VAT EXEMPT");

                            headerListRow.AppendChild(cellList);

                            if (i == 1)
                            {
                                cellList = new Cell
                                {
                                    DataType = CellValues.String,
                                    CellValue = new CellValue(string.Empty)
                                };
                                headerListRow.AppendChild(cellList);

                                overAllTotal = table.AsEnumerable()
                                    .Sum(x => x.Field<double>("Amount"))
                                    .ToString("N2");
                                cellList = new Cell
                                {
                                    DataType = CellValues.String,
                                    CellValue = new CellValue(overAllTotal)
                                };
                                headerListRow.AppendChild(cellList);
                            }


                            sheetData.AppendChild(headerListRow);
                        }
                    }
                    else //VATABLE
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            var headerListRow = new Row();
                            var cellList = new Cell();
                            cellList.DataType = CellValues.String;
                            if (i == 0)
                                cellList.CellValue = new CellValue($"{dateRange}-VATABLE");
                            else if (i == 1) cellList.CellValue = new CellValue("VATABLE");

                            headerListRow.AppendChild(cellList);

                            if (i == 1)
                            {
                                cellList = new Cell
                                {
                                    DataType = CellValues.String,
                                    CellValue = new CellValue(string.Empty)
                                };
                                headerListRow.AppendChild(cellList);

                                overAllTotal = table.AsEnumerable()
                                    .Sum(x => x.Field<double>("Amount"))
                                    .ToString("N2");
                                cellList = new Cell
                                {
                                    DataType = CellValues.String,
                                    CellValue = new CellValue(overAllTotal)
                                };
                                headerListRow.AppendChild(cellList);

                                overAllTotal = table.AsEnumerable()
                                    .Sum(x => x.Field<double>("VatableSales"))
                                    .ToString("N2");
                                cellList = new Cell
                                {
                                    DataType = CellValues.String,
                                    CellValue = new CellValue(overAllTotal)
                                };
                                headerListRow.AppendChild(cellList);

                                overAllTotal = table.AsEnumerable()
                                    .Sum(x => x.Field<double>("Vat"))
                                    .ToString("N2");
                                cellList = new Cell
                                {
                                    DataType = CellValues.String,
                                    CellValue = new CellValue(overAllTotal)
                                };
                                headerListRow.AppendChild(cellList);
                            }

                            sheetData.AppendChild(headerListRow);
                        }
                    }

                    break;

                case ReportTypeEnum.PurchasesMonthlyBIR:
                    if (table.TableName.Equals("VATEXEMPT"))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var headerListRow = new Row();
                            var cellList = new Cell();
                            cellList.DataType = CellValues.String;
                            if (i == 0)
                                cellList.CellValue = new CellValue($"SAN MIGUEL FOODS, INC. - {dateRange}");
                            else if (i == 1)
                                cellList.CellValue = new CellValue($"VAT REG TIN {tin}");
                            else if (i == 2)
                                cellList.CellValue = new CellValue("FEEDS(VAT EXEMPT)");

                            headerListRow.AppendChild(cellList);
                            sheetData.AppendChild(headerListRow);
                        }

                    }
                    else //VATABLE
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var headerListRow = new Row();
                            var cellList = new Cell {DataType = CellValues.String};
                            if (i == 0)
                                cellList.CellValue = new CellValue($"SAN MIGUEL FOODS, INC. - {dateRange}");
                            else if (i == 1)
                                cellList.CellValue = new CellValue($"VAT REG TIN {tin}");
                            else if (i == 2)
                                cellList.CellValue = new CellValue("SMAHC(VATABLE)");

                            headerListRow.AppendChild(cellList);
                            sheetData.AppendChild(headerListRow);
                        }
                    }

                    break;

                case ReportTypeEnum.CustomerAccountSummary:
                    for (int i = 0; i < 2; i++)
                    {
                        var headerListRow = new Row();
                        var cellList = new Cell {DataType = CellValues.String};
                        if (i == 0)
                            cellList.CellValue = new CellValue("CUSTOMER ACCOUNT SUMMARY");
                        else if (i == 1)
                            cellList.CellValue = new CellValue(dateRange);

                        headerListRow.AppendChild(cellList);
                        sheetData.AppendChild(headerListRow);
                    }

                    break;
            }

            return overAllTotal;
        }

        private static void ResolveCellDataType(DataTable table, string col, Cell cell, ReportTypeEnum reportTypeEnum)
        {
            if (reportTypeEnum == ReportTypeEnum.Inventory)
            {
                cell.DataType = col.ToUpper().Contains("DATE") ? CellValues.String : CellValues.Number;
            }
            else if (reportTypeEnum == ReportTypeEnum.SalesMTDFeeds)
            {
                cell.DataType = col.ToUpper().Equals("CUSTOMERNAME") 
                                || col.ToUpper().Equals("TOWN")
                                || col.ToUpper().Equals("SALES PERSONNEL")
                                || col.ToUpper().Equals("POSITION")
                    ? CellValues.String : CellValues.Number;
            }
            else if (reportTypeEnum == ReportTypeEnum.PurchasesMonthlyBIR)
            {
                cell.DataType = col.Equals("purchaseDate")
                                || col.Equals("invoiceNo")
                    ? CellValues.String : CellValues.Number;
            }
            else if (reportTypeEnum == ReportTypeEnum.PurchasesMTDFeeds)
            {
                cell.DataType = col.ToUpper().Equals("SUPPLIERNAME") ? CellValues.String : CellValues.Number;
            }
            else
            {
                var columnType = table.Columns[col].DataType;
                if (columnType == typeof(Double))
                {
                    cell.DataType = CellValues.Number;
                }
                else if (col.Equals("total Price Per PO") ||
                         col.Equals("1LIT") ||
                         col.Equals("bag") ||
                         col.Equals("box") ||
                         col.Equals("btl") ||
                         col.Equals("kg") ||
                         col.Equals("kilo") ||
                         col.Equals("pck") ||
                         col.Contains("TOTAL"))
                {
                    cell.DataType = CellValues.Number;
                }
                else
                {
                    cell.DataType = CellValues.String;
                }
            }
        }
    }
}
