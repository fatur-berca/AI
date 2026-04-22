using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using OfficeOpenXml;
using DFIS.Universal.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Transport.Repositories;
using TOM.Master.Repositories;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.BusinessLogics
{
    public class TransportTruckArrivalBLL : ITransportTruckArrivalBLL
    {
        private readonly ITransportTruckArrivalRepo _transportTruckArrivalRepo;
        private readonly ITransportTruckArrivalTempRepo _transportTruckArrivalTempRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IMasterConfigurationRepo _masterConfigurationRepo;

        public TransportTruckArrivalBLL(ITransportTruckArrivalRepo transportTruckArrivalRepo,
            IMasterListRepo masterListRepo, IMasterConfigurationRepo masterConfigurationRepo, ITransportTruckArrivalTempRepo transportTruckArrivalTempRepo)
        {
            _transportTruckArrivalRepo = transportTruckArrivalRepo;
            _masterListRepo = masterListRepo;
            _masterConfigurationRepo = masterConfigurationRepo;
            _transportTruckArrivalTempRepo = transportTruckArrivalTempRepo;
        }

        public List<MasterListDTO> GetAllETACategory()
        {
            return Mapper.Map<List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("ETACategory").ToList());
        }

        public List<TransportTruckArrivalDTO> GetAllTransportTruckArrival(TransportTruckArrivalInput filter)
        {
            return _transportTruckArrivalRepo.GetAllTransportTruckArrival(filter);
        }

        public MasterConfigurationDTO GetMaxSpeed()
        {
            return Mapper.Map<MasterConfigurationDTO>(
                _masterConfigurationRepo
                    .GetMasterConfigurationByPageNameDescription("TruckArrival", "MaxVehicleSpeed"));
        }

        public void UpdateCalculate(TransportTruckArrival save, string userid)
        {
            _transportTruckArrivalRepo.UpdateCalculate(save, userid);
        }

        public void DeleteTruckArrivalTemp()
        {
            _transportTruckArrivalTempRepo.DeleteTruckArrivalTemp();
            _transportTruckArrivalTempRepo.ReseedTruckArrivalTemp();
        }

        public void SaveUpload(string fileLocation)
        {
            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(fileLocation)))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here
                var totalRows = myWorksheet.Dimension.End.Row;
                for (int rowNum = 5; rowNum <= totalRows; rowNum++) //selet starting row here
                {
                    int val;
                    Decimal val2;
                    try
                    {
                        string noPol = myWorksheet.Cells[rowNum, 1].Text;
                        DateTime date = DateTime.Parse(myWorksheet.Cells[rowNum, 2].Text);
                        string longitude = myWorksheet.Cells[rowNum, 3].Text;
                        string latitude = myWorksheet.Cells[rowNum, 4].Text;

                        string location = myWorksheet.Cells[rowNum, 5].Text;
                        string locationName = myWorksheet.Cells[rowNum, 6].Text;
                        int speed = int.TryParse(myWorksheet.Cells[rowNum, 7].Text, out val) ? val : 0;
                        int altitude = int.TryParse(myWorksheet.Cells[rowNum, 8].Text, out val) ? val : 0;

                        int satellite = int.TryParse(myWorksheet.Cells[rowNum, 9].Text, out val) ? val : 0;
                        Decimal mileage = Decimal.TryParse(myWorksheet.Cells[rowNum, 10].Text, out val2) ? val2 : 0;
                        string acc = myWorksheet.Cells[rowNum, 11].Text;
                        string sos = myWorksheet.Cells[rowNum, 12].Text;

                        string rear = myWorksheet.Cells[rowNum, 13].Text;
                        string left = myWorksheet.Cells[rowNum, 14].Text;
                        string right = myWorksheet.Cells[rowNum, 15].Text;
                        string resource = myWorksheet.Cells[rowNum, 16].Text;

                        TruckArrivalTemp truckArrivalTemp = new TruckArrivalTemp();
                        truckArrivalTemp.PoliceNumber = noPol;
                        truckArrivalTemp.Date = date;
                        truckArrivalTemp.Longitude = longitude;
                        truckArrivalTemp.Latitude = latitude;
                        truckArrivalTemp.Location = location;
                        truckArrivalTemp.Speed = speed;
                        truckArrivalTemp.Satellite = satellite;
                        truckArrivalTemp.SOS = sos;
                        truckArrivalTemp.DoorRear = rear;
                        truckArrivalTemp.DoorLeft = left;
                        truckArrivalTemp.DoorRight = right;
                        _transportTruckArrivalTempRepo.SaveData(truckArrivalTemp);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        public void SaveUploadXLS(string fileLocation)
        {
            int row = 0;
            using (var stream = File.Open(fileLocation, FileMode.Open, FileAccess.Read))
            {
                int val;
                Decimal val2;
                throw new NotImplementedException("Please fix the NuGet package and properly refer");
                #warning Please fix the NuGet package and properly refer
                /*
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())//baca row
                        {
                            if (row > 3)
                            {
                                string noPol = reader.GetString(0) != null ? reader.GetString(0) : "";
                                DateTime date = reader.GetDateTime(1);
                                string longitude = reader.GetDouble(2).ToString();
                                string latitude = reader.GetDouble(3).ToString();

                                string location = reader.GetString(4) != null ? reader.GetString(4) : "";
                                string locationName = reader.GetString(5) != null ? reader.GetString(5) : ""; ;
                                int speed = int.TryParse(reader.GetDouble(6).ToString(), out val) ? val : 0;
                                int altitude = int.TryParse(reader.GetDouble(7).ToString(), out val) ? val : 0;

                                int satellite = int.TryParse(reader.GetDouble(8).ToString(), out val) ? val : 0;
                                Decimal mileage = decimal.TryParse(reader.GetDouble(9).ToString(), out val2) ? val2 : 0;
                                string acc = reader.GetString(10) != null ? reader.GetString(10) : "";
                                string sos = reader.GetString(11) != null ? reader.GetString(11) : "";

                                string rear = reader.GetString(12) != null ? reader.GetString(12) : "";
                                string left = reader.GetString(13) != null ? reader.GetString(13) : "";
                                string right = reader.GetString(14) != null ? reader.GetString(14) : "";
                                string resource = reader.GetString(15) != null ? reader.GetString(15) : "";

                                TruckArrivalTemp truckArrivalTemp = new TruckArrivalTemp();
                                truckArrivalTemp.PoliceNumber = noPol;
                                truckArrivalTemp.Date = date;
                                truckArrivalTemp.Longitude = longitude;
                                truckArrivalTemp.Latitude = latitude;
                                truckArrivalTemp.Location = location;
                                truckArrivalTemp.Speed = speed;
                                truckArrivalTemp.Satellite = satellite;
                                truckArrivalTemp.SOS = sos;
                                truckArrivalTemp.DoorRear = rear;
                                truckArrivalTemp.DoorLeft = left;
                                truckArrivalTemp.DoorRight = right;
                                _transportTruckArrivalTempRepo.SaveData(truckArrivalTemp);
                            }
                            row++;
                        }
                    } while (reader.NextResult());//baca sheet
                }
                */
            }
        }

        public void MoveFile(string source, string des)
        {
            var fileName = Path.GetFileName(source);
            var destFile = Path.Combine(des, fileName);
            File.Move(source, destFile);
        }

        public MemoryStream ReportExport(TransportTruckArrivalInput filter)
        {
            List<string> field = new List<string>();
            field.Add("PoliceNumber");
            field.Add("GPNo");
            field.Add("Date");
            field.Add("FinishAddress");
            field.Add("ETACategory");
            field.Add("ETACategoryCalculate");
            return ExportDataSet(GetAllTransportTruckArrival(filter), field);
        }

        private MemoryStream ExportDataSet(List<TransportTruckArrivalDTO> data, List<string> field)
        {
            MemoryStream memoryStream = new MemoryStream();

            throw new NotImplementedException("Please fix the NuGet package and properly refer");
            #warning Please fix the NuGet package and properly refer
            /*
            using (var workbook = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = workbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet
                {
                    Id = workbook.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Truck Arrival"
                };
                sheets.Append(sheet);
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                Row headerRow = new Row();
                headerRow.RowIndex = (int)1;//
                List<String> columns = new List<string>();
                int colIndexHeader = 1;//
                foreach (string column in field)
                {
                    columns.Add(column);
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellReference = getColumnName(colIndexHeader) + 1;//
                    cell.CellValue = new CellValue(column);
                    headerRow.AppendChild(cell);
                    colIndexHeader++;//
                }
                sheetData.AppendChild(headerRow);
                int rowIndexDetail = 2;//
                foreach (TransportTruckArrivalDTO dsrow in data)
                {
                    Row newRow = new Row();
                    Type type = dsrow.GetType();
                    int colIndexDetail = 1;//
                    newRow.RowIndex = (uint)rowIndexDetail;//
                    foreach (String col in columns)
                    {
                        Cell cell = new Cell();
                        PropertyInfo propertyInfo = type.GetProperty(col);
                        if (propertyInfo.GetValue(dsrow, null) != null)
                        {
                            if (propertyInfo.PropertyType == typeof(string))
                            {
                                cell.DataType = CellValues.String;
                                cell.CellValue = new CellValue(propertyInfo.GetValue(dsrow, null).ToString());
                            }
                            else if (propertyInfo.PropertyType == typeof(Nullable<decimal>) ||
                                     propertyInfo.PropertyType == typeof(Nullable<int>))
                            {
                                cell.DataType = CellValues.Number;
                                cell.CellValue = new CellValue(propertyInfo.GetValue(dsrow, null).ToString());
                            }
                            else if (propertyInfo.PropertyType == typeof(DateTime))
                            {
                                cell.DataType = CellValues.String;
                                DateTime date = Convert.ToDateTime(propertyInfo.GetValue(dsrow, null).ToString());
                                cell.CellValue = new CellValue(date.ToString("dd-MMM-yyyy"));
                            }
                            else
                            {
                                cell.DataType = CellValues.String;
                                cell.CellValue = new CellValue(propertyInfo.GetValue(dsrow, null).ToString());
                            }
                        }
                        else
                        {
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue("");
                        }
                        cell.CellReference = getColumnName(colIndexDetail) + rowIndexDetail;//
                        newRow.AppendChild(cell);
                        colIndexDetail++;//
                    }
                    sheetData.AppendChild(newRow);
                    rowIndexDetail++;//
                }

                worksheetPart.Worksheet.Save();
                workbookPart.Workbook.Save();
                memoryStream.Seek(0, SeekOrigin.Begin);
                workbook.Close();
            }
            */
            return memoryStream;
        }

        private static string getColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = String.Empty;
            while (dividend > 0)
            {
                var modifier = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modifier) + columnName;
                dividend = Convert.ToInt32((dividend - modifier) / 26);
            }
            return columnName;
        }
    }
}
