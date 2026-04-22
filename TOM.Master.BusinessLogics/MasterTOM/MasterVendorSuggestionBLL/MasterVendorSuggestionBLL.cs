using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using DFIS.Utils;
using OfficeOpenXml;
using DFIS.Contracts;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public class MasterVendorSuggestionBLL : IMasterVendorSuggestionBLL
    {
        private readonly IGenericRepository<MasterVendor> _generalRepo;
        private readonly IMasterVendorSuggestionRepo _masterVendorSuggestionRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IMasterLocationRepo _masterLocationRepo;
        private readonly IMasterVendorTOMRepo _masterVendorTOMRepo;

        public MasterVendorSuggestionBLL(IGenericRepository<MasterVendor> generalRepo, IMasterVendorSuggestionRepo masterVendorSuggestionRepo, IMasterListRepo masterListRepo, IMasterLocationRepo masterLocationRepo, IMasterVendorTOMRepo masterVendorTOMRepo)
        {
            _generalRepo = generalRepo;
            _masterVendorSuggestionRepo = masterVendorSuggestionRepo;
            _masterListRepo = masterListRepo;
            _masterLocationRepo = masterLocationRepo;
            _masterVendorTOMRepo = masterVendorTOMRepo;
        }

        public List<MasterListDTO> GetList()
        {
            List<MasterListDTO> tempList = new List<MasterListDTO>();
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("OrderType")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("TransportationCategory")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("TransportationMode")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("VehicleType")));
            return tempList.OrderBy(x => x.FieldName).ToList();
        }

        public List<MasterLocationDTO> GetLocation()
        {
            /*
            List<MasterLocationDTO> tempList = new List<MasterLocationDTO>();
            tempList.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Warehouse")));
            tempList.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Agent")));
            tempList.AddRange(Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(_masterLocationRepo.GetAllMasterLocationActiveByType("Factory")));
            */
            TOMContextDB ctx = new TOMContextDB();
            var tempList = (from l in ctx.MasterLocations
                            join c in ctx.MasterConfigurations on l.Type equals c.Value
                            where c.PageName == "TOMLocationFilter"
                            select l).ToList();
            return Mapper.Map<List<MasterLocation>, List<MasterLocationDTO>>(tempList);
        }
        public MasterVendorSuggestionDTO GetByID(int id)
        {
            return Mapper.Map<MasterVendorSuggestionDTO>(_masterVendorSuggestionRepo.Get(v => v.IDVendorSuggestion == id).FirstOrDefault());
        }
        public List<MasterVendorTOMDTO> GetVendorByCriteria(MasterVendorSuggestionDTO Criteria)
        {
            TOMContextDB ctx = new TOMContextDB();
            var queryFilter = PredicateHelper.True<MasterVendor>();

            if (!String.IsNullOrEmpty(Criteria.TransportationCategory))
            {
                queryFilter = queryFilter.And(f => f.VendorCategory == Criteria.TransportationCategory);
            }

            if (!String.IsNullOrEmpty(Criteria.TransportationMode))
            {
                queryFilter = queryFilter.And(f => f.TransportationMode == Criteria.TransportationMode);
            }

            queryFilter = queryFilter.And(f => f.IsActive && f.ParentVendor == null);

            return Mapper.Map<List<MasterVendorTOMDTO>>(_generalRepo.Get(queryFilter)).ToList();
            //ctx.MasterVendorSuggestions.Where( f => f.IsActive)
            //return Mapper.Map<List<MasterTransportVendor>, List<MasterVendorTOMDTO>>(_masterVendorTOMRepo.GetAllMasterVendorActive());
        }
        public List<MasterVendorSuggestionDTO> GetALLMasterVendorSuggestion(MasterVendorSuggestionInput criteria)
        {
            var vsug = Mapper.Map<List<MasterVendorSuggestion>, List<MasterVendorSuggestionDTO>>(_masterVendorSuggestionRepo.GetAllMasterVendorSuggestion(criteria).ToList());
            foreach (var v in vsug)
            {
                v.MasterLocation = Mapper.Map<MasterLocationDTO>(_masterLocationRepo.GetMasterLocationByID(v.StartLocation));
                v.MasterLocation1 = Mapper.Map<MasterLocationDTO>(_masterLocationRepo.GetMasterLocationByID(v.ReceiverIDLocation));
                v.MasterTransportVendor = Mapper.Map<MasterVendorTOMDTO>(_masterVendorTOMRepo.Get(c => c.IDVendor == v.IDVendorSuggestion).FirstOrDefault());
            }
            return vsug;
        }

        public void SaveData(MasterVendorSuggestion input, bool status)
        {
            _masterVendorSuggestionRepo.SaveData(input, status);
        }

        public MasterVendorSuggestion ValidateData(MasterVendorSuggestion Input)
        {
            TOMContextDB ctx = new TOMContextDB();
            return ctx.MasterVendorSuggestions
                        .Where(f => f.StartLocation == Input.StartLocation && f.ReceiverIDLocation == Input.ReceiverIDLocation
                                && f.OrderType == Input.OrderType && f.TransportationCategory == Input.TransportationCategory
                                && f.TransportationMode == Input.TransportationMode && f.VehicleType == Input.VehicleType).FirstOrDefault();
        }

        public List<string> Upload(HttpPostedFileBase input, string userId)
        {
            List<MasterLocationDTO> listLocation = GetLocation();//digunakan untuk mengecek lokasi yang diupload ada di database tidak
            var stream = input.InputStream;
            List<string> listKolomError = new List<string>();
            List<MasterVendorSuggestion> listSave = new List<MasterVendorSuggestion>();
            using (ExcelPackage xlPackage = new ExcelPackage(stream))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets.First();
                var totalRows = myWorksheet.Dimension.End.Row;
                var rowNumbList = new Boolean[totalRows];
                for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                {
                    MasterVendorSuggestion masterVendorSuggest = new MasterVendorSuggestion();
                    try
                    {
                        masterVendorSuggest.StartLocation = myWorksheet.Cells[rowNum, 1].Text;
                        masterVendorSuggest.ReceiverIDLocation = myWorksheet.Cells[rowNum, 2].Text;
                        masterVendorSuggest.OrderType = myWorksheet.Cells[rowNum, 3].Text;
                        masterVendorSuggest.TransportationCategory = myWorksheet.Cells[rowNum, 4].Text;
                        masterVendorSuggest.TransportationMode = myWorksheet.Cells[rowNum, 5].Text;
                        masterVendorSuggest.VehicleType = myWorksheet.Cells[rowNum, 6].Text;
                        var vendor = _masterVendorTOMRepo.GetAll().Where(x => x.VendorName == myWorksheet.Cells[rowNum, 7].Text).FirstOrDefault();
                        masterVendorSuggest.SuggestedVendor = vendor != null ? vendor.IDVendor : 0;
                        masterVendorSuggest.CreatedBy = userId;
                        masterVendorSuggest.UpdatedBy = userId;
                        MasterLocationDTO result = listLocation.Find(x => x.LocationName == masterVendorSuggest.StartLocation);
                        string tempError;
                        if (result == null)
                        {
                            tempError = "StartLocation-" + masterVendorSuggest.StartLocation + "-" + rowNum;
                            listKolomError.Add(tempError);
                        }
                        MasterLocationDTO result2 = listLocation.Find(x => x.LocationName == masterVendorSuggest.ReceiverIDLocation);
                        if (result2 == null)
                        {
                            tempError = "Receiver-" + masterVendorSuggest.ReceiverIDLocation + "-" + rowNum;
                            listKolomError.Add(tempError);
                        }
                        MasterVendorSuggestion result3 = _masterVendorSuggestionRepo.GetAll().Where(x => x.StartLocation == masterVendorSuggest.StartLocation
                        && x.ReceiverIDLocation == masterVendorSuggest.ReceiverIDLocation && x.OrderType == masterVendorSuggest.OrderType
                        && x.TransportationCategory == masterVendorSuggest.TransportationCategory && x.TransportationMode == masterVendorSuggest.TransportationMode
                        && x.VehicleType == masterVendorSuggest.VehicleType).FirstOrDefault();
                        rowNumbList[rowNum - 1] = true;
                        if (result3 != null)
                        {
                            rowNumbList[rowNum - 1] = false;
                            if (result3.IsActive)
                            {
                                tempError = "IsActive-" + masterVendorSuggest.IsActive + "-" + rowNum;
                                listKolomError.Add(tempError);
                            }
                            else
                            {
                                masterVendorSuggest.IsActive = true;
                                masterVendorSuggest.IDVendorSuggestion = result3.IDVendorSuggestion;
                            }
                        }
                        listSave.Add(masterVendorSuggest);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                if (listKolomError.Count == 0)
                {
                    var index = 1;
                    foreach (MasterVendorSuggestion temp in listSave)
                    {
                        _masterVendorSuggestionRepo.SaveData(temp, rowNumbList[index]);
                    }
                }
            }
            return listKolomError;
        }

        #region IImporterBLL implementation Template
        bool _importBegin = false;
        public void Import(IEnumerable<MasterVendorSuggestionDTO> newData)
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
        public void ImportRow(MasterVendorSuggestionDTO data)
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
        public void InsertOrUpdate(MasterVendorSuggestionDTO value, bool canUpdate = true)
        {
            // filters out the same record
            var existingRecords = _masterVendorSuggestionRepo.Get(v =>
            (value.IDVendorSuggestion == v.IDVendorSuggestion)
            ||
            (value.StartLocation == v.StartLocation
            && value.ReceiverIDLocation == v.ReceiverIDLocation
            && value.OrderType == v.OrderType
            && value.TransportationCategory == v.TransportationCategory
            && value.TransportationMode == v.TransportationMode
            && value.VehicleType == v.VehicleType)
               );

            if (existingRecords.Count() == 0)
            {
                // insert the new data
                _masterVendorSuggestionRepo.Insert(MappingHelper.Map<MasterVendorSuggestion>(value));
            }
            else
            {
                var record = existingRecords.ElementAt(0);

                if (record.IsActive && record.IDVendorSuggestion != value.IDVendorSuggestion)
                {
                    throw new Exception("Data already exists!");
                }
                // update the data
                value.IDVendorSuggestion = record.IDVendorSuggestion;
                value.UpdatedDate = DateTime.Now;
                MappingHelper.Map(value, record);
                _masterVendorSuggestionRepo.Update(record);
            }
            _masterVendorSuggestionRepo.Save();
        }
    }
}
