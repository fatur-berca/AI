using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System.Linq;

namespace hms_tom_dev.Util
{
    public class DataTableToExcel
    {
        public void createExcel(string YourExcelfileName, string sheetName)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(YourExcelfileName, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = (string.IsNullOrEmpty(sheetName)) ? "Export" : sheetName
                };

                sheets.Append(sheet);

                workbookPart.Workbook.Save();
            }
        }

        public void generateCustomeExcel(DataTable YoutdTName, string YourExcelfileName, int start, int end)
        {
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(YourExcelfileName, true))
            {
                WorkbookPart workbook = spreadsheet.WorkbookPart;
                //create a reference to Sheet1

                //var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();

                //SheetData data = new SheetData();

                WorksheetPart worksheet = workbook.WorksheetParts.Last();
                SheetData data = worksheet.Worksheet.GetFirstChild<SheetData>();

                WorkbookStylesPart stylesPart = workbook.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = GenerateStyleSheet();
                stylesPart.Stylesheet.Save();

                //add column names to the first row
                Row header = new Row();
                header.RowIndex = (UInt32)start;

                foreach (DataColumn column in YoutdTName.Columns)
                {
                    Cell headerCell = createTextCell(YoutdTName.Columns.IndexOf(column) + 1, start, column.ColumnName,6);

                    header.AppendChild(headerCell);
                }
                data.AppendChild(header);

                //loop through each data row
                DataRow contentRow = default(DataRow);
                int j = 0;
                for (int i = start - 1; i <= end - 1; i++)
                {
                    contentRow = YoutdTName.Rows[j];
                    data.AppendChild(createContentRow(contentRow, i + 2));
                    j++;
                }
            }
        }

        public void generateExcels(DataTable YoutdTName, string YourExcelfileName, string sheetName = null, bool isWithUrl = false)
        {
            generateExcels(YoutdTName, YourExcelfileName, null, sheetName, isWithUrl);
        }
        public void generateExcels(DataTable YoutdTName, string YourExcelfileName, string[] headerNames, string sheetName = null, bool isWithUrl = false)
        {
            createExcel(YourExcelfileName, sheetName);

            //populate the data into the spreadsheet
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(YourExcelfileName, true))
            {
                WorkbookPart workbook = spreadsheet.WorkbookPart;
                //create a reference to Sheet1

                //var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();

                //SheetData data = new SheetData();

                WorksheetPart worksheet = workbook.WorksheetParts.Last();
                SheetData data = worksheet.Worksheet.GetFirstChild<SheetData>();

                WorkbookStylesPart stylesPart = workbook.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = GenerateStyleSheet();
                stylesPart.Stylesheet.Save();

                //add column names to the first row
                Row header = new Row();
                header.RowIndex = (int)1;

                var linkColumn = 0;

                foreach (DataColumn column in YoutdTName.Columns)
                {
                    var idx = YoutdTName.Columns.IndexOf(column);
                    Cell headerCell = createTextCell(idx + 1, 1, headerNames != null && idx < headerNames.Length ? headerNames[idx] : column.ColumnName,6);

                    if (isWithUrl && column.ColumnName == "HMS Memo File")
                    {
                        linkColumn = idx;
                    }

                    header.AppendChild(headerCell);
                }
                data.AppendChild(header);

                //loop through each data row
                DataRow contentRow = default(DataRow);
                for (int i = 0; i <= YoutdTName.Rows.Count - 1; i++)
                {
                    contentRow = YoutdTName.Rows[i];
                    data.AppendChild(createContentRow(contentRow, i + 2, isWithUrl, linkColumn));
                }
            }
        }

        private static string getColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modifier = 0;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = Convert.ToInt32((dividend - modifier) / 26);
            }

            return columnName;
        }

        private static Cell createTextCell(int columnIndex, int rowIndex, object cellValue, UInt32? styleIndex)
        {
            Cell cell = new Cell();
            cell.CellReference = getColumnName(columnIndex) + rowIndex;

            if (cellValue is int || cellValue is Decimal)
            {
                cell.DataType = CellValues.Number;
                cell.CellValue = new CellValue(cellValue.ToString());
            }
            else if (cellValue is DateTime)
            {
                cell.DataType = CellValues.Date;
                cell.CellValue = new CellValue(((DateTime)cellValue).ToString("dd-MMM-yyyy"));
            }
            else
            {
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(cellValue.ToString());
            }
            if(styleIndex != null)
                cell.StyleIndex = styleIndex;
            //InlineString inlineString = new InlineString();
            //Text t = new Text();

            //t.Text = cellValue.ToString();
            //inlineString.AppendChild(t);
            //cell.AppendChild(inlineString);

            return cell;
        }

        private static Row createContentRow(DataRow dataRow, int rowIndex, bool isWithUrl = false, int linkColumn = 0)
        {
            Row row = new Row { RowIndex = (UInt32)rowIndex };

            for (int i = 0; i <= dataRow.Table.Columns.Count - 1; i++)
            {
                Cell dataCell = createTextCell(i + 1, rowIndex, dataRow[i], null);

                if(isWithUrl && linkColumn == i)
                {
                    CellFormula cellFormula1 = new CellFormula();

                    var url = ConfigurationManager.AppSettings["WebRootUrl"] + "Assets/Uploads/TransportLostClaimDamage/" + dataRow[i];
                    cellFormula1.Text = @"HYPERLINK(""" + url + "\"" + "," + "\"" + dataRow[i] + "\")";
                    dataCell.Append(cellFormula1);
                    dataCell.StyleIndex = 3;
                }

                row.AppendChild(dataCell);
            }
            return row;
        }

        private Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(
                new Fonts(
                    new Font(                                                               // Index 0 - The default font.
                        new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 1 - The bold font.
                        new Bold(),
                        new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 2 - The Italic font.
                        new Italic(),
                        new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 3 - White Font
                        new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "ffffff" } },
                        new FontName() { Val = "Calibri" })
                ),
                new Fills(
                    new Fill(                                                           // Index 0 - The default fill.
                        new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(                                                           // Index 1 - The default fill of gray 125 (required)
                        new PatternFill() { PatternType = PatternValues.Gray125 }),
                    new Fill(                                                           // Index 2 - The red fill.
                        new PatternFill(
                                new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "B21B1B" } }
                            )
                            { PatternType = PatternValues.Solid }),
                    new Fill(                                                           // Index 3 - The yellow fill.
                        new PatternFill(
                                new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "F7EC40" } }
                            )
                            { PatternType = PatternValues.Solid }),
                    new Fill(                                                           // Index 4 - The green fill.
                        new PatternFill(
                                new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "3BBB25" } }
                            )
                            { PatternType = PatternValues.Solid })
                ),
                new Borders(
                    new Border(                                                         // Index 0 - The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),
                    new Border(                                                         // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                        new LeftBorder(
                                new Color() { Rgb = new HexBinaryValue() { Value = "000000" } }
                            )
                            { Style = BorderStyleValues.Medium },
                        new RightBorder(
                                new Color() { Rgb = new HexBinaryValue() { Value = "000000" } }
                            )
                            { Style = BorderStyleValues.Medium },
                        new TopBorder(
                                new Color() { Rgb = new HexBinaryValue() { Value = "000000" } }
                            )
                            { Style = BorderStyleValues.Medium },
                        new BottomBorder(
                                new Color() { Rgb = new HexBinaryValue() { Value = "000000" } }
                            )
                            { Style = BorderStyleValues.Medium },
                        new DiagonalBorder())
                ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },                        // Index 0 - The default cell style.  If a cell does not have a style index applied it will use this style combination instead
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 0, ApplyFont = true },      // Index 1 - Bold 
                    new CellFormat() { FontId = 3, FillId = 2, BorderId = 0, ApplyFont = true },      // Index 2 - red
                    new CellFormat() { FontId = 1, FillId = 3, BorderId = 0, ApplyFont = true },      // Index 3 - yellow 
                    new CellFormat() { FontId = 3, FillId = 4, BorderId = 0, ApplyFont = true },      // Index 4 - green 
                    new CellFormat(                                                                   // Index 5 - Alignment
                            new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                        )
                        { FontId = 0, FillId = 0, BorderId = 0, ApplyAlignment = true },
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 1, ApplyBorder = true }      // Index 6 - Bold & Border
                )
            ); // return
        }
    }
}