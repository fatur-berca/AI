using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using OfficeOpenXml;
using TOM.Master.Domain.Inputs;
using System.Diagnostics;
using DFIS.Contracts;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal;
using DFIS.Utils;

namespace TOM.Master.BusinessLogics
{
    public class MasterCostBLL: IMasterCostBLL
    {
        private readonly IMasterCostRepo _masterCostRepo;
        private readonly IMasterVendorTOMRepo _masterVendorTOMRepo;
        private readonly IMasterLocationRepo _masterLocationRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IGenericRepository<MasterCost> _generalRepo;

        public MasterCostBLL(IGenericRepository<MasterCost> generalRepo, IMasterCostRepo masterCostRepo, IMasterVendorTOMRepo masterVendorTOMRepo, IMasterLocationRepo masterLocationRepo, IMasterListRepo masterListRepo)
        {
            _masterCostRepo = masterCostRepo;
            _masterVendorTOMRepo = masterVendorTOMRepo;
            _masterLocationRepo = masterLocationRepo;
            _masterListRepo = masterListRepo;
            _generalRepo = generalRepo;
        }

        public List<MasterListDTO> GetList()
        {
            List<MasterListDTO> tempList = new List<MasterListDTO>();
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("TransportationMode")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("ViaRoute")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("VehicleType")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("OrderType")));
            return tempList;
        }

        public List<MasterCostDTO> GetALLMasterCostByCostType(List<string> costType)
        {
            return Mapper.Map<List<MasterCost>, List<MasterCostDTO>>(_masterCostRepo.SelectAllCostByListCostType(costType));
        }
        public List<MasterCostDTO> GetAllMasterCost(MasterCostInput input)
        {
            var masterCostList = Mapper.Map<List<MasterCost>, List<MasterCostDTO>>(_masterCostRepo.GetAllMasterCost(input));
            foreach (var item in masterCostList)
            {
                item.MasterLocation = Mapper.Map<MasterLocationDTO>(_masterLocationRepo.GetMasterLocationByID(item.SenderIDLocation));
                item.MasterLocation1 = Mapper.Map<MasterLocationDTO>(_masterLocationRepo.GetMasterLocationByID(item.ThroughIDLocation));
                item.MasterLocation2 = Mapper.Map<MasterLocationDTO>(_masterLocationRepo.GetMasterLocationByID(item.ReceiverIDLocation));
                item.MasterTransportVendor = Mapper.Map<MasterVendorTOMDTO>(_masterVendorTOMRepo.Get(c => c.IDVendor == item.IDVendor).FirstOrDefault());
            }
            return masterCostList;
        }
        public List<MasterLocationDTO> GetALLMasterLocation()
        {
            List<MasterLocationDTO> tempList = new List<MasterLocationDTO>();
            tempList.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Warehouse")));
            tempList.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Agent")));
            tempList.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Factory")));
            return tempList;
        }

        public List<MasterVendorTOMDTO> GetALLVendor()
        {
            return Mapper.Map<List<MasterVendor>, List<MasterVendorTOMDTO>>(_masterVendorTOMRepo.GetAllMasterVendorActive());
        }

        public int SaveData(MasterCost input, bool status)
        {
            var result = _masterCostRepo.SaveData(input, status);
            if(result == 0)
            {
                result = _masterCostRepo.CommitSave();
            }
            return result;
        }

        public List<string> Upload(HttpPostedFileBase input, string userId)
        {
            var stream = input.InputStream;
            List<string> listError = new List<string>();
            List<MasterCost> listSave = new List<MasterCost>();
            List<MasterCostDTO> listUpdateData = new List<MasterCostDTO>();
            using (ExcelPackage xlPackage = new ExcelPackage(stream))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets.First();
                string dataTypeUpload = myWorksheet.Cells[2, 1].Text;                
                List<MasterCostDTO> latestEffectiveStartDate = _masterCostRepo.ListMasterCostWithLatestEffectiveStartDate(dataTypeUpload);//digunakan untuk mengecek data dengan effective date paling terakhir
                List<string> errorField = CheckFieldFileUpload(myWorksheet.Cells[1,1,1,15], dataTypeUpload);
                //Debug.WriteLine("Error Field:" + errorField.Count);
                if (errorField.Count > 0)
                    listError.AddRange(errorField);

                var totalRows = myWorksheet.Dimension.End.Row;
                for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                {
                    try
                    {
                        listError.AddRange(CheckFieldFileUpload(myWorksheet.Cells[rowNum,1,rowNum,15], dataTypeUpload));
                        listError.AddRange(CheckDataUpload(myWorksheet.Cells[rowNum, 1, rowNum, 15], rowNum, dataTypeUpload));
                        if(listError.Count == 0)
                            SetMasterCostFileUpload(myWorksheet.Cells[rowNum, 1, rowNum, 15], rowNum, userId, dataTypeUpload, latestEffectiveStartDate, ref listSave, ref listUpdateData, ref listError);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                if (listError.Count == 0)
                {
                    for(int i=0; i<listSave.Count; i++ )
                    {
                        var mcost = _masterCostRepo.SaveData(listSave[i], true);
                        if (mcost == 1)
                        {                            
                            listError.Add("Data error in rows - "+(i+1));
                            break;
                        }
                        _masterCostRepo.CommitSave();
                            
                        /*if (listUpdateData[i] != null)
                            _masterCostRepo.SaveDataUpload(Mapper.Map<MasterCostDTO, MasterCost>(listUpdateData[i]), false);
                        _masterCostRepo.SaveDataUpload(listSave[i], true);*/
                    }
                }
            }
            return listError;
        }

        public List<string> CheckFieldFileUpload(ExcelRange row, string costType)
        {
            List<string> listError = new List<string>();
            if (row[1, 2].Text.ToLower() != "vendor name")
                listError.Add("Header-2-Vendor Name");
            if (costType.ToLower() == "km based")
            {
                if (row[1, 3].Text.ToLower() != "minimum km")
                    listError.Add("Header-" + costType + "-3-Minimum KM");
                if (row[1, 4].Text.ToLower() != "based price")
                    listError.Add("Header-" + costType + "-4-Based Price");
                if (row[1, 5].Text.ToLower() != "discount price")
                    listError.Add("Header-" + costType + "-5-Discount Price");
                if (row[1, 6].Text.ToLower() != "effective start date")
                    listError.Add("Header-" + costType + "-6-Effective Start Date");
            }
            else if (costType.ToLower() == "box based")
            {
                if (row[1, 3].Text.ToLower() != "minimum box")
                    listError.Add("Header-" + costType + "-3-Minimum Box");
                if (row[1, 4].Text.ToLower() != "based price")
                    listError.Add("Header-" + costType + "-4-Based Price");
                if (row[1, 5].Text.ToLower() != "sender")
                    listError.Add("Header-" + costType + "-5-Sender");
                if (row[1, 6].Text.ToLower() != "through")
                    listError.Add("Header-" + costType + "-6-Through");
                if (row[1, 7].Text.ToLower() != "receiver")
                    listError.Add("Header-" + costType + "-7-Receiver");
                if (row[1, 8].Text.ToLower() != "effective start date")
                    listError.Add("Header-" + costType + "-8-Effective Start Date");
            }
            else if (costType.ToLower() == "spsi")
            {
                if (row[1, 3].Text.ToLower() != "based price")
                    listError.Add("Header-" + costType + "-3-Based Price");
                if (row[1, 4].Text.ToLower() != "receiver")
                    listError.Add("Header-" + costType + "-4-Receiver");
                if (row[1, 5].Text.ToLower() != "additional unit price")
                    listError.Add("Header-" + costType + "-5-Additional Unit Price");
                if (row[1, 6].Text.ToLower() != "effective start date")
                    listError.Add("Header-" + costType + "-6-Effective Start Date");
            }
            else if (costType.ToLower() == "trip based")
            {
                if (row[1, 3].Text.ToLower() != "based price")
                    listError.Add("Header-" + costType + "-3-Based Price");
                if (row[1, 4].Text.ToLower() != "vehicle type")
                    listError.Add("Header-" + costType + "-4-Vehicle Type");
                if (row[1, 5].Text.ToLower() != "sender")
                    listError.Add("Header-" + costType + "-5-Sender");
                if (row[1, 6].Text.ToLower() != "through")
                    listError.Add("Header-" + costType + "-6-Through");
                if (row[1, 7].Text.ToLower() != "receiver")
                    listError.Add("Header-" + costType + "-7-Receiver");
                if (row[1, 8].Text.ToLower() != "via")
                    listError.Add("Header-" + costType + "-8-Via");
                if (row[1, 9].Text.ToLower() != "effective start date")
                    listError.Add("Header-" + costType + "-9-Effective Start Date");
            }
            else if (costType.ToLower() == "asdp")
            {
                if (row[1, 3].Text.ToLower() != "based price")
                    listError.Add("Header-" + costType + "-3-Based Price");
                if (row[1, 4].Text.ToLower() != "vehicle type")
                    listError.Add("Header-" + costType + "-4-Vehicle Type");
                if (row[1, 5].Text.ToLower() != "sender")
                    listError.Add("Header-" + costType + "-5-Sender");
                if (row[1, 6].Text.ToLower() != "receiver")
                    listError.Add("Header-" + costType + "-6-Receiver");
                if (row[1, 7].Text.ToLower() != "effective start date")
                    listError.Add("Header-" + costType + "-9-Effective Start Date");
            }
            else
            {
                listError.Add("Type-" + costType);
            }
            return listError;
        }

        public List<string> CheckDataUpload(ExcelRange row, int rowNumber, string costType)
        {
            //pertama cek data di excelnya ada atau tidak
            //kedua cek referensi/fk master location(kalau ada)
            //ketiga cek apakah textnya bisa di parsing jadi angka/tanggal
            List<MasterLocationDTO> listLocation = GetALLMasterLocation();//digunakan untuk mengecek lokasi yang diupload ada di database tidak
            List<string> listError = new List<string>();
            Decimal parseDecimal;
            DateTime parseDate;

            if (String.IsNullOrEmpty(row[rowNumber, 2].Text) && String.IsNullOrEmpty(row[rowNumber, 3].Text) && String.IsNullOrEmpty(row[rowNumber, 4].Text) &&
                String.IsNullOrEmpty(row[rowNumber, 5].Text) && String.IsNullOrEmpty(row[rowNumber, 6].Text) && String.IsNullOrEmpty(row[rowNumber, 7].Text) &&
                String.IsNullOrEmpty(row[rowNumber, 8].Text) && String.IsNullOrEmpty(row[rowNumber, 9].Text) && String.IsNullOrEmpty(row[rowNumber, 10].Text))
            {
                return listError;
            }
            else
            {
                if (String.IsNullOrEmpty(row[rowNumber, 2].Text))
                    listError.Add("DataEmpty-" + rowNumber + "-Vendor Name");
                if (costType.ToLower() == "km based")
                {
                    if (!String.IsNullOrEmpty(row[rowNumber, 3].Text))
                    {
                        bool parse = Decimal.TryParse(row[rowNumber, 3].Text, out parseDecimal);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Minimum KM-Number");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Minimum KM");

                    if (!String.IsNullOrEmpty(row[rowNumber, 4].Text))
                    {
                        bool parse = Decimal.TryParse(row[rowNumber, 4].Text, out parseDecimal);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Based Price-Number");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Based Price");

                    if (!String.IsNullOrEmpty(row[rowNumber, 5].Text))
                    {
                        bool parse = Decimal.TryParse(row[rowNumber, 5].Text, out parseDecimal);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Discount Price-Number");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Discount Price");

                    if (!String.IsNullOrEmpty(row[rowNumber, 6].Text))
                    {
                        bool parse = DateTime.TryParse(row[rowNumber, 6].Text, out parseDate);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Effective Start Date-Date");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Effective Start Date");
                }
                else if (costType.ToLower() == "box based")
                {
                    if (!String.IsNullOrEmpty(row[rowNumber, 3].Text))
                    {
                        bool parse = Decimal.TryParse(row[rowNumber, 3].Text, out parseDecimal);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Minimum Box-Number");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Minimum Box");

                    if (!String.IsNullOrEmpty(row[rowNumber, 4].Text))
                    {
                        bool parse = Decimal.TryParse(row[rowNumber, 4].Text, out parseDecimal);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Based Price-Number");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Based Price");

                    if (!String.IsNullOrEmpty(row[rowNumber, 5].Text))
                    {
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == row[rowNumber, 5].Text);
                        if (result == null)
                            listError.Add("Location-" + rowNumber + "-" + row[rowNumber, 5].Text + "-Sender");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Sender");

                    if (!String.IsNullOrEmpty(row[rowNumber, 6].Text))
                    {
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == row[rowNumber, 6].Text);
                        if (result == null)
                            listError.Add("Location-" + rowNumber + "-" + row[rowNumber, 6].Text + "-Through");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Through");

                    if (!String.IsNullOrEmpty(row[rowNumber, 7].Text))
                    {
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == row[rowNumber, 7].Text);
                        if (result == null)
                            listError.Add("Location-" + rowNumber + "-" + row[rowNumber, 7].Text + "-Receiver");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Receiver");

                    if (!String.IsNullOrEmpty(row[rowNumber, 8].Text))
                    {
                        bool parse = DateTime.TryParse(row[rowNumber, 8].Text, out parseDate);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Effective Start Date-Date");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Effective Start Date");
                }
                else if (costType.ToLower() == "spsi")
                {
                    if (!String.IsNullOrEmpty(row[rowNumber, 3].Text))
                    {
                        bool parse = Decimal.TryParse(row[rowNumber, 3].Text, out parseDecimal);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Based Price-Number");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Based Price");

                    if (!String.IsNullOrEmpty(row[rowNumber, 4].Text))
                    {
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == row[rowNumber, 4].Text);
                        if (result == null)
                            listError.Add("Location-" + rowNumber + "-" + row[rowNumber, 4].Text + "-Receiver");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Receiver");

                    if (!String.IsNullOrEmpty(row[rowNumber, 5].Text))
                    {
                        bool parse = Decimal.TryParse(row[rowNumber, 5].Text, out parseDecimal);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Additional Unit Price-Number");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Additional Unit Price");

                    if (!String.IsNullOrEmpty(row[rowNumber, 6].Text))
                    {
                        bool parse = DateTime.TryParse(row[rowNumber, 6].Text, out parseDate);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Effective Start Date-Date");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Effective Start Date");
                }
                else if (costType.ToLower() == "trip based")
                {
                    if (String.IsNullOrEmpty(row[rowNumber, 3].Text))
                        listError.Add("DataEmpty-" + rowNumber + "-Based Price");

                    if (String.IsNullOrEmpty(row[rowNumber, 4].Text))
                        listError.Add("DataEmpty-" + rowNumber + "-Vehicle Type");

                    if (!String.IsNullOrEmpty(row[rowNumber, 5].Text))
                    {
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == row[rowNumber, 5].Text);
                        if (result == null)
                            listError.Add("Location-" + rowNumber + "-" + row[rowNumber, 5].Text + "-Sender");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Sender");

                    if (!String.IsNullOrEmpty(row[rowNumber, 6].Text))
                    {
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == row[rowNumber, 6].Text);
                        if (result == null)
                            listError.Add("Location-" + rowNumber + "-" + row[rowNumber, 6].Text + "-Through");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Through");

                    if (!String.IsNullOrEmpty(row[rowNumber, 7].Text))
                    {
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == row[rowNumber, 7].Text);
                        if (result == null)
                            listError.Add("Location-" + rowNumber + "-" + row[rowNumber, 7].Text + "-Receiver");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Receiver");

                    if (String.IsNullOrEmpty(row[rowNumber, 8].Text))
                        listError.Add("DataEmpty-" + rowNumber + "-Via");

                    if (!String.IsNullOrEmpty(row[rowNumber, 9].Text))
                    {
                        bool parse = DateTime.TryParse(row[rowNumber, 9].Text, out parseDate);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Effective Start Date-Date");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Effective Start Date");
                }
                else if (costType.ToLower() == "asdp")
                {
                    if (!String.IsNullOrEmpty(row[rowNumber, 3].Text))
                    {
                        bool parse = Decimal.TryParse(row[rowNumber, 3].Text, out parseDecimal);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Based Price-Number");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Based Price");

                    if (String.IsNullOrEmpty(row[rowNumber, 4].Text))
                        listError.Add("DataEmpty-" + rowNumber + "-Vehicle Type");

                    if (!String.IsNullOrEmpty(row[rowNumber, 5].Text))
                    {
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == row[rowNumber, 5].Text);
                        if (result == null)
                            listError.Add("Location-" + rowNumber + "-" + row[rowNumber, 5].Text + "-Sender");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Sender");

                    if (!String.IsNullOrEmpty(row[rowNumber, 6].Text))
                    {
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == row[rowNumber, 6].Text);
                        if (result == null)
                            listError.Add("Location-" + rowNumber + "-" + row[rowNumber, 6].Text + "-Receiver");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Receiver");

                    if (!String.IsNullOrEmpty(row[rowNumber, 7].Text))
                    {
                        bool parse = DateTime.TryParse(row[rowNumber, 7].Text, out parseDate);
                        if (!parse)
                            listError.Add("Parse-" + rowNumber + "-Effective Start Date-Date");
                    }
                    else
                        listError.Add("DataEmpty-" + rowNumber + "-Effective Start Date");
                }
            }
            return listError;
        }

        public void SetMasterCostFileUpload(ExcelRange row, int rowNumber, string userid, string costType, List<MasterCostDTO> latestEffectiveStartDate, ref List<MasterCost> listSave, ref List<MasterCostDTO> listUpdateData, ref List<string> listError)
        {
            List<MasterLocationDTO> listLocation = GetALLMasterLocation();
            MasterCost tempSave = new MasterCost();
            MasterCostDTO tempUpdate = new MasterCostDTO();
            if (String.IsNullOrEmpty(row[rowNumber, 2].Text) && String.IsNullOrEmpty(row[rowNumber, 3].Text) && String.IsNullOrEmpty(row[rowNumber, 4].Text) &&
                String.IsNullOrEmpty(row[rowNumber, 5].Text) && String.IsNullOrEmpty(row[rowNumber, 6].Text) && String.IsNullOrEmpty(row[rowNumber, 7].Text) &&
                String.IsNullOrEmpty(row[rowNumber, 8].Text) && String.IsNullOrEmpty(row[rowNumber, 9].Text) && String.IsNullOrEmpty(row[rowNumber, 10].Text))
            {

            }
            else
            {
                tempSave.CostType = costType;
                var VendorName = row[rowNumber, 2].Text;
                var vendor = _masterVendorTOMRepo.Get(x => x.VendorName == VendorName && x.VendorCategory == tempSave.CostType).FirstOrDefault();
                if (vendor != null)
                {
                    if(vendor.ParentVendor == null)
                    {
                        tempSave.IDVendor = vendor.IDVendor;
                    }
                    else
                    {
                        listError.Add("IsNotParent-" + rowNumber + "-" + row[rowNumber, 2].Text + "-Vendor Name");
                    }
                }
                else
                {
                    listError.Add("DataNotFound-" + rowNumber + "-" + row[rowNumber, 2].Text + "-Vendor Name");
                }
                if (costType.ToLower() == "km based")
                {
                    var date = row[rowNumber, 6].Text;
                    var temp = row[rowNumber, 3].Text.Split('.');
                    if (row[rowNumber, 3].Text.IndexOf(',') >= 0)
                    {
                        temp = row[rowNumber, 3].Text.Split(',');
                    }
                    tempSave.MinimumKM = Decimal.Parse(temp[0]);
                    temp = row[rowNumber, 4].Text.Split('.');
                    if (row[rowNumber, 4].Text.IndexOf(',') >= 0)
                    {
                        temp = row[rowNumber, 4].Text.Split(',');
                    }
                    tempSave.BasedPrice = Decimal.Parse(temp[0]);
                    temp = row[rowNumber, 5].Text.Split('.');
                    if (row[rowNumber, 5].Text.IndexOf(',') >= 0)
                    {
                        temp = row[rowNumber, 5].Text.Split(',');
                    }
                    tempSave.DiscountPrice = Decimal.Parse(temp[0]);
                    tempSave.EffectiveStartDate = DateTime.Parse(date.ToString());
                    //apakah minimum km juga termasuk sebagai pembanding?
                    //tempUpdate = latestEffectiveStartDate.Find(x => x.CostType == costType && x.IDVendor == Int32.Parse(row[rowNumber, 2].Text));
                    //if (tempUpdate != null)
                    //{
                    //    tempUpdate.EffectiveEndDate = tempSave.EffectiveStartDate.Date.AddDays(-1);
                    //}
                }
                else if (costType.ToLower() == "box based")
                {
                    var date = row[rowNumber, 8].Text;
                    var temp = row[rowNumber, 3].Text.Split('.');
                    if (row[rowNumber, 3].Text.IndexOf(',') >= 0)
                    {
                        temp = row[rowNumber, 3].Text.Split(',');
                    }
                    tempSave.MinimumBox = int.Parse(temp[0]);
                    temp = row[rowNumber, 4].Text.Split('.');
                    if (row[rowNumber, 4].Text.IndexOf(',') >= 0)
                    {
                        temp = row[rowNumber, 4].Text.Split(',');
                    }
                    tempSave.BasedPrice = Decimal.Parse(temp[0]);
                    tempSave.SenderIDLocation = row[rowNumber, 5].Text;
                    tempSave.ThroughIDLocation = row[rowNumber, 6].Text;
                    tempSave.ReceiverIDLocation = row[rowNumber, 7].Text;
                    tempSave.EffectiveStartDate = DateTime.Parse(date.ToString());
                    //tempUpdate = latestEffectiveStartDate.Find(x => x.CostType == costType && x.IDVendor == Int32.Parse(row[rowNumber, 2].Text) && x.ThroughIDLocation == row[rowNumber, 3].Text);

                    MasterLocationDTO result = listLocation.Find(x => x.LocationName == tempSave.SenderIDLocation);

                    if (result != null)
                    {
                        tempSave.SenderIDLocation = result.IDLocation;
                    }
                    else
                    {
                        listError.Add("Location-" + rowNumber + "-" + tempSave.SenderIDLocation + "-Sender");
                    }

                    MasterLocationDTO result2 = listLocation.Find(x => x.LocationName == tempSave.ThroughIDLocation);

                    if (result2 != null)
                    {
                        tempSave.ThroughIDLocation = result2.IDLocation;
                    }
                    else
                    {
                        listError.Add("Location-" + rowNumber + "-" + tempSave.ThroughIDLocation + "-Through");
                    }

                    MasterLocationDTO result3 = listLocation.Find(x => x.LocationName == tempSave.ReceiverIDLocation);

                    if (result3 != null)
                    {
                        tempSave.ReceiverIDLocation = result3.IDLocation;
                    }
                    else
                    {
                        listError.Add("Location-" + rowNumber + "-" + tempSave.ReceiverIDLocation + "-Receiver");
                    }

                    //if (tempUpdate != null)
                    //{
                    //    tempUpdate.EffectiveEndDate = tempSave.EffectiveStartDate.Date.AddDays(-1);
                    //}
                }
                else if (costType.ToLower() == "spsi")
                {
                    var date = row[rowNumber, 6].Text;
                    var temp = row[rowNumber, 3].Text.Split('.');
                    if (row[rowNumber, 3].Text.IndexOf(',') >= 0)
                    {
                        temp = row[rowNumber, 3].Text.Split(',');
                    }
                    tempSave.BasedPrice = Decimal.Parse(temp[0]);
                    tempSave.ReceiverIDLocation = row[rowNumber, 4].Text;
                    temp = row[rowNumber, 5].Text.Split('.');
                    if (row[rowNumber, 5].Text.IndexOf(',') >= 0)
                    {
                        temp = row[rowNumber, 5].Text.Split(',');
                    }
                    tempSave.AdditionalUnitPrice = Decimal.Parse(temp[0]);
                    tempSave.EffectiveStartDate = DateTime.Parse(date.ToString());

                    MasterLocationDTO result3 = listLocation.Find(x => x.LocationName == tempSave.ReceiverIDLocation);

                    if (result3 != null)
                    {
                        tempSave.ReceiverIDLocation = result3.IDLocation;
                    }
                    else
                    {
                        listError.Add("Location-" + rowNumber + "-" + tempSave.ReceiverIDLocation + "-Receiver");
                    }
                }
                else if (costType.ToLower() == "trip based")
                {
                    var date = row[rowNumber, 9].Text;
                    var temp = row[rowNumber, 3].Text.Split('.');
                    if (row[rowNumber, 3].Text.IndexOf(',') >= 0)
                    {
                        temp = row[rowNumber, 10].Text.Split(',');
                    }
                    //tempSave.TransportationMode = row[rowNumber, 3].Text;
                    tempSave.VehicleType = row[rowNumber, 4].Text;
                    //tempSave.OrderType = row[rowNumber, 5].Text;
                    tempSave.SenderIDLocation = row[rowNumber, 5].Text;
                    tempSave.ThroughIDLocation = row[rowNumber, 6].Text;
                    tempSave.ReceiverIDLocation = row[rowNumber, 7].Text;
                    tempSave.Via = row[rowNumber, 8].Text;
                    tempSave.BasedPrice = Decimal.Parse(temp[0]);
                    tempSave.EffectiveStartDate = DateTime.Parse(date.ToString());
                    //tempUpdate = latestEffectiveStartDate.Find(x => x.CostType == costType && x.IDVendor == Int32.Parse(row[rowNumber, 2].Text) && x.VehicleType == row[rowNumber, 4].Text && x.OrderType == row[rowNumber, 5].Text && x.SenderIDLocation == row[rowNumber, 6].Text && x.ThroughIDLocation == row[rowNumber, 7].Text && x.ReceiverIDLocation == row[rowNumber, 8].Text && x.Via == row[rowNumber, 9].Text);

                    MasterLocationDTO result = listLocation.Find(x => x.LocationName == tempSave.SenderIDLocation);

                    if (result != null)
                    {
                        tempSave.SenderIDLocation = result.IDLocation;
                    }
                    else
                    {
                        listError.Add("Location-" + rowNumber + "-" + tempSave.SenderIDLocation + "-Sender");
                    }

                    MasterLocationDTO result2 = listLocation.Find(x => x.LocationName == tempSave.ThroughIDLocation);

                    if (result2 != null)
                    {
                        tempSave.ThroughIDLocation = result2.IDLocation;
                    }
                    else
                    {
                        listError.Add("Location-" + rowNumber + "-" + tempSave.ThroughIDLocation + "-Through");
                    }

                    MasterLocationDTO result3 = listLocation.Find(x => x.LocationName == tempSave.ReceiverIDLocation);

                    if (result3 != null)
                    {
                        tempSave.ReceiverIDLocation = result3.IDLocation;
                    }
                    else
                    {
                        listError.Add("Location-" + rowNumber + "-" + tempSave.ReceiverIDLocation + "-Receiver");
                    }

                    //if (tempUpdate != null)
                    //{
                    //    tempUpdate.EffectiveEndDate = tempSave.EffectiveStartDate.Date.AddDays(-1);
                    //}
                }
                else if (costType.ToLower() == "asdp")
                {
                    var date = row[rowNumber, 7].Text;
                    var temp = row[rowNumber, 3].Text.Split('.');
                    if (row[rowNumber, 3].Text.IndexOf(',') >= 0)
                    {
                        temp = row[rowNumber, 3].Text.Split(',');
                    }
                    tempSave.BasedPrice = Decimal.Parse(temp[0]);
                    tempSave.VehicleType = row[rowNumber, 4].Text;
                    tempSave.SenderIDLocation = row[rowNumber, 5].Text;
                    tempSave.ReceiverIDLocation = row[rowNumber, 6].Text;
                    tempSave.EffectiveStartDate = DateTime.Parse(date.ToString());
                    //tempUpdate = latestEffectiveStartDate.Find(x => x.CostType == costType && x.IDVendor == Int32.Parse(row[rowNumber, 2].Text) && x.VehicleType == row[rowNumber, 3].Text && x.OrderType == row[rowNumber, 4].Text && x.SenderIDLocation == row[rowNumber, 5].Text && x.ThroughIDLocation == row[rowNumber, 6].Text && x.ReceiverIDLocation == row[rowNumber, 7].Text);

                    MasterLocationDTO result = listLocation.Find(x => x.LocationName == tempSave.SenderIDLocation);

                    if (result != null)
                    {
                        tempSave.SenderIDLocation = result.IDLocation;
                    }
                    else
                    {
                        listError.Add("Location-" + rowNumber + "-" + tempSave.SenderIDLocation + "-Sender");
                    }

                    MasterLocationDTO result3 = listLocation.Find(x => x.LocationName == tempSave.ReceiverIDLocation);

                    if (result3 != null)
                    {
                        tempSave.ReceiverIDLocation = result3.IDLocation;
                    }
                    else
                    {
                        listError.Add("Location-" + rowNumber + "-" + tempSave.ReceiverIDLocation + "-Receiver");
                    }

                    //if (tempUpdate != null)
                    //{
                    //    tempUpdate.EffectiveEndDate = tempSave.EffectiveStartDate.Date.AddDays(-1);
                    //}
                }

                tempSave.EffectiveEndDate = new DateTime(2999, 12, 31);

                tempSave.IsActive = true;
                tempSave.CreatedBy = userid;
                tempSave.UpdatedBy = userid;
                if (tempUpdate != null)
                    tempUpdate.UpdatedBy = userid;
                listSave.Add(tempSave);
                listUpdateData.Add(tempUpdate);
            }
        }
        public List<MasterList> GetTypeList()
        {
            List<MasterList> tempList = new List<MasterList>();            
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterList>>(_masterListRepo.GetMasterListByFieldName("VendorCategory")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterList>>(_masterListRepo.GetMasterListByFieldName("DataTypeMstCost")));
            return tempList;
        }

        public bool DelData(int keyID)
        {
            var context = new TOMContextDB();
            var getData = context.MasterCosts
                .Where(m => m.IDCost == keyID);

            if (getData != null)
            {
                _masterCostRepo.Delete(keyID);
                _masterCostRepo.Save();

                return true;
            }
            else
            {
                return false;
            }
        }

        public int SaveData(MasterCostDTO input, bool status)
        {
            return SaveData(MappingHelper.Map<MasterCost>(input), status);
        }

        public void InsertOrUpdate(MasterCostDTO value, bool canUpdate = true)
        {
            // filters out the same record
            var existingRecords = _generalRepo.Get(v =>
            (value.IDCost == v.IDCost)
            ||
            (value.IDVendor == v.IDVendor && 
            value.SenderIDLocation == v.SenderIDLocation && 
            value.ReceiverIDLocation == v.ReceiverIDLocation &&
            value.ThroughIDLocation == v.ThroughIDLocation &&
            value.VehicleType == v.VehicleType &&
            value.Via == v.Via
            ));

            // create date range helper
            DateRangeHelper hlp = new DateRangeHelper();
            foreach (var rec in existingRecords)
                hlp.Add(new DateRange(rec.EffectiveStartDate, rec.EffectiveEndDate, rec));

            value.UpdatedDate = DateTime.Now;
            if (existingRecords.Count() == 0)
            {
                // new data mode (vendor is new)
                _generalRepo.Insert(Mapper.Map<MasterCost>(value));
            }
            else
            {
                // update mode (vendor is same)
                var record = existingRecords.Where(mtl => mtl.IDCost == value.IDCost).FirstOrDefault();
                if (record == null)
                {
                    // get all intersections
                    var itx = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate));
                    if (itx.Count == 0)
                    {
                        // no intersection, happily insert the data :D
                        _generalRepo.Insert(Mapper.Map<MasterCost>(value));
                    }
                    else if (itx.Count == 1)
                    {
                        // the new data can only intersects the inf ended data
                        var itr = itx.ElementAt(0);
                        if (itr.SourceRange.DateEnd.ToString("yyyy-MM-dd") != "2999-12-31")
                            throw new EffectiveDateConflictException();
                        // but only when the new data starts after the start date of the intersected data
                        if (itr.SourceRange.DateStart >= value.EffectiveStartDate)
                            throw new EffectiveDateConflictException();
                        // update the old data
                        if (itr.SourceRange.Tag is MasterCost)
                        {
                            var oldValue = itr.SourceRange.Tag as MasterCost;
                            oldValue.EffectiveEndDate = value.EffectiveStartDate - new TimeSpan(1, 0, 0, 0);
                            _generalRepo.Update(oldValue);
                        }
                        // insert the new data
                        _generalRepo.Insert(Mapper.Map<MasterCost>(value));
                    }
                    else
                        throw new EffectiveDateConflictException();

                }
                else
                {
                    // cannot intersect with anything but itself
                    var recs = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate));
                    if ((recs.Count == 1 && recs.ElementAt(0).SourceRange.Tag is MasterCost &&
                        (recs.ElementAt(0).SourceRange.Tag as MasterCost).IDCost != value.IDCost)
                        ||
                        recs.Count > 1)
                        throw new EffectiveDateConflictException();
                    // pure update
                    value.IDCost = record.IDCost;
                    MappingHelper.Map(value, record);
                    _generalRepo.Update(record);
                }
            }


            _generalRepo.Save();
        }

        #region IImporterBLL implementation Template
        bool _importBegin = false;
        public void Import(IEnumerable<MasterCostDTO> newData)
        {
            BeginImport();
            try
            {
                foreach (var d in newData) ImportRow(d);
                FinalizeImport();
            }
            catch (Exception ex)
            {
                CancelImport();
                throw ex;
            }
        }
        public void ImportRow(MasterCostDTO data)
        {
            InsertOrUpdate(data, false);
        }
        public void BeginImport()
        {
            if (_importBegin) return;
            _importBegin = true;

            _generalRepo.BeginTransaction();
        }
        public void FinalizeImport()
        {
            if (!_importBegin) return;
            _generalRepo.EndTransaction();
        }
        public void CancelImport()
        {
            if (!_importBegin) return;
            _generalRepo.EndTransaction(false);
        }
        #endregion
    }
}
