using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.Contracts;
//using DFIS.EntitiesDAL.EDMX;
//using DFIS.Transport.Domain.DTOs;
//using DFIS.Transport.Domain.Inputs;
//using DFIS.Transport.Repositories.TransportDriverManagementRepo;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using System.Web;
using System.Collections;
using OfficeOpenXml;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Transport.BusinessLogics.TransportSealBLL
{
    public class TransportSealBLL : ITransportSealBLL
    {
        private readonly IGenericRepository<TransportSeal> _generalRepo;
        private readonly IGenericRepository<TransportExecution> _transportExecutionRepo;
        private readonly IGenericRepository<MasterLocation> _masterLocationRepo;
        private readonly IGenericRepository<MasterTruckSealStock> _masterTruckSealStockRepo;
        private readonly IGenericRepository<MasterUser> _masterUserRepo;
        private readonly IGenericRepository<TransportOrder> _transportOrderRepo;

        public TransportSealBLL(IGenericRepository<TransportSeal> generalRepo, 
            IGenericRepository<TransportExecution> transportExecutionRepo,
            IGenericRepository<MasterLocation> masterLocationRepo,
            IGenericRepository<MasterTruckSealStock> masterTruckSealStockRepo,
            IGenericRepository<MasterUser> masterUserRepo,
            IGenericRepository<TransportOrder> transportOrderRepo)
        {
            _generalRepo = generalRepo;
            _transportExecutionRepo = transportExecutionRepo;
            _masterLocationRepo = masterLocationRepo;
            _masterTruckSealStockRepo = masterTruckSealStockRepo;
            _masterUserRepo = masterUserRepo;
            _transportOrderRepo = transportOrderRepo;
    }
       
        public List<TransportOrderDTO> GetTransportOrderByTransactionNo(string id)
        {
            TOMContextDB context = new TOMContextDB();

            var dbResult = (from a in context.TransportExecutions
                join b in context.TransportOrders on a.IDTransportExecution equals b.IDTransportExecution
                join c in context.MasterLocations on b.SenderIDLocation equals c.IDLocation
                join d in context.MasterLocations on b.ReceiverIDLocation equals d.IDLocation
                            //where a.TransportNo.ToUpper() == id.ToUpper() && b.LoadStartTime != null && b.LoadFinishTime != null && b.UnloadStartTime != null && b.UnloadFinishTime != null
                            where a.TransportNo.ToUpper() == id.ToUpper()
                select new TransportOrderDTO()
                {
                    IDTransportOrder = b.IDTransportOrder,
                    STONo = b.STONo,
                    SenderIDLocation = b.SenderIDLocation,
                    ReceiverIDLocation = b.ReceiverIDLocation,
                    SenderLocationName = c.LocationName,
                    ReceiverLocationName = d.LocationName,
                    IDTransportExecution = a.IDTransportExecution,
                    LoadStartTime = b.LoadStartTime,
                    LoadFinishTime = b.LoadFinishTime,
                    UnloadStartTime = b.UnloadStartTime,
                    UnloadFinishTime = b.UnloadFinishTime,
                    LoadBoxperWorkingTime = b.LoadBoxperWorkingTime,
                    UnloadBoxperWorkingTime = b.UnloadBoxperWorkingTime,
                    TransportMode = a.TransportMode
                }).ToList();
            return dbResult;
        }

        public List<TransportSealDTO> GetDatas(TransportSealInput input)
        {
            var queryFilter = PredicateHelper.True<TransportSeal>();
            var ctx = new TOMContextDB();

            var dbRes = ctx.TransportExecutions.Where(f => f.TransportNo.Equals(input.TransportNo)).FirstOrDefault();

            if (dbRes == null && !String.IsNullOrEmpty(input.TransportNo))
            {
                return null;
            }

            if (!String.IsNullOrEmpty(input.TransportNo)) queryFilter = queryFilter.And(m => m.IDTransportExecution == dbRes.IDTransportExecution);

            if (!String.IsNullOrEmpty(input.SealNumber)) queryFilter = queryFilter.And(m => m.SealNumber == input.SealNumber);

            //if (!String.IsNullOrEmpty(input.IDLocation)) queryFilter = queryFilter.And(m => m.IDLocation == input.IDLocation);

            queryFilter = queryFilter.And(m => m.IsActive);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression, input.SortExpression2 }, input.SortOrder);

            var dbResult = Mapper.Map<List<TransportSealDTO>>(_generalRepo.Get(queryFilter).ToList());
            var listLocation = _masterLocationRepo.GetAll().Where(x => x.IsActive).ToList();

            foreach (var data in dbResult)
            {
                data.CreatedByFullName = _masterUserRepo.GetByID(data.CreatedBy).FullName.ToString();
                data.LocationName = listLocation.Where(x => x.IDLocation == data.IDLocation).Select(x => x.LocationName).Single().ToString();
                data.TN = data.TransportExecution.TransportNo;
            }

            dbResult = dbResult.OrderBy(f => f.SealNumber).ThenBy(f => f.SealActivity).ToList();
            return dbResult;
        }

        public List<TransportSealDTO> GetValidateDataSealDtos(TransportSealDTO input)
        {
            var queryFilter = PredicateHelper.True<TransportSeal>();

            if (!String.IsNullOrEmpty(input.SealNumber))
                queryFilter = queryFilter.And(m => m.SealNumber == input.SealNumber);
            queryFilter = queryFilter.And(m => m.IsActive);

            var dbResult = _generalRepo.Get(queryFilter).ToList();
            return Mapper.Map<List<TransportSealDTO>>(dbResult);
        }

        public TransportSealDTO EditData(TransportSealDTO input)
        {
            //var dbInput = Mapper.Map<TransportSeal>(input);
            using (var dbContext = new TOMContextDB())
            {
                TransportSeal validateTn =
                    dbContext.TransportSeals.FirstOrDefault(x => x.IDTransportSeal == input.IDTransportSeal);
                try
                {
                    if (validateTn != null)
                    {
                        validateTn.Remarks = input.Remarks;
                        validateTn.SealTime = input.SealTime;
                        validateTn.UpdatedBy = input.UpdatedBy;
                        validateTn.UpdatedDate = DateTime.Now;
                        validateTn.IsActive = input.IsActive;
                        //validateTn.rema
                        dbContext.SaveChanges();
                    }
                }
                catch (ExceptionBase ex)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.UnhandledException);
                }
            }
            return input;
        }

        public int SaveSealDto(TransportSealDTO input)
        {
            using (var dbContext = new TOMContextDB())
            {
                var validateTn = dbContext.TransportExecutions
                    .FirstOrDefault(x => x.IDTransportExecution == input.IDTransportExecution);

                if (validateTn == null)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.NoAvailableKey);
                }
                var newInsertData = Mapper.Map<TransportSeal>(input);

                newInsertData.UpdatedBy = newInsertData.CreatedBy;
                newInsertData.CreatedDate = DateTime.Now;
                newInsertData.UpdatedDate = DateTime.Now;
                newInsertData.IsActive = true;

                dbContext.TransportSeals.Add(newInsertData);

                return dbContext.SaveChanges();
            }
        }

        public List<string> Upload(HttpPostedFileBase input, string userid)
        {
            List<string> listError = new List<string>();
            var ctx = new TOMContextDB();

            var stream = input.InputStream;
            var filename = input.FileName;

            var listTransportExecution = _transportExecutionRepo.GetAll().Where(x => x.IsActive);
            var listMasterLocation = _masterLocationRepo.GetAll().Where(x => x.IsActive);
            var listSealNumber = _masterTruckSealStockRepo.GetAll().Where(x => x.IsActive);
            
            var currentdatetime = DateTime.Now;
            
            try
            {
                //region VALIDATE ALL DATA BEFORE INSERTING INTO DATABASE
                listError = ValidateData(stream, listMasterLocation, listTransportExecution, listSealNumber);
                //endregion

                if (listError.Count == 0)
                {
                    using (ExcelPackage xlPackage = new ExcelPackage(stream))
                    {
                        var sheet = xlPackage.Workbook.Worksheets[1];

                        var totalRows = sheet.Dimension.End.Row;
                        
                        //pengecekan data kembar
                        var ListData = new List<TransportSealDTO>();
                        for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                        {

                            if (sheet.Cells[rowNum, 1].Text.Trim().Equals("")) break;
                            var transportSeal = new TransportSealDTO();
                            transportSeal.IDTransportSeal = rowNum; // rownumber
                            //transportSeal.IDTransportExecution = Convert.ToInt32(listTransportExecution.Where(x => x.TransportNo.ToUpper() == sheet.Cells[rowNum, 1].Text).Select(x => x.IDTransportExecution).Single());
                            transportSeal.SealNumber = sheet.Cells[rowNum, 2].Text;
                            transportSeal.SealActivity = sheet.Cells[rowNum, 3].Text;
                            /*
                            transportSeal.IDLocation = listMasterLocation.Where(x => x.LocationName.ToUpper() == sheet.Cells[rowNum, 4].Text.ToUpper()).Select(x => x.IDLocation).Single().ToString();
                            transportSeal.SealTime = DateTime.FromOADate(Double.Parse("" + sheet.Cells[rowNum, 5].Value));
                            transportSeal.IsActive = true;
                            transportSeal.Remarks = sheet.Cells[rowNum, 6].Text; ;
                            transportSeal.CreatedBy = userid;
                            transportSeal.CreatedDate = currentdatetime;
                            transportSeal.UpdatedBy = userid;
                            transportSeal.UpdatedDate = currentdatetime;
                            */
                            ListData.Add(transportSeal);
                        }

                        var dbResult = (from ts in ListData
                                       join ts2 in ctx.TransportSeals on new { ts.SealNumber, ts.SealActivity } equals new { ts2.SealNumber, ts2.SealActivity }
                                       select ts).Distinct().ToList();
                        
                        if (dbResult != null)
                        {
                            foreach (var result in dbResult)
                            {                                
                                listError.Add("Row # " + result.IDTransportSeal + " already exists");
                                return listError;
                            }
                        }
                        

                        for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                        {
                            var transportSeal = new TransportSeal();
                            var result = listTransportExecution.Where(x => x.TransportNo.ToUpper() == sheet.Cells[rowNum, 1].Text).FirstOrDefault();
                            if (result == null) break;
                            transportSeal.IDTransportExecution = Convert.ToInt32(result.IDTransportExecution);                            
                            transportSeal.SealNumber = sheet.Cells[rowNum, 2].Text;
                            transportSeal.SealActivity = sheet.Cells[rowNum, 3].Text;
                            transportSeal.IDLocation = listMasterLocation.Where(x => x.LocationName.ToUpper() == sheet.Cells[rowNum, 4].Text.ToUpper()).Select(x => x.IDLocation).Single().ToString();
                            transportSeal.SealTime = DateTime.FromOADate(Double.Parse(""+sheet.Cells[rowNum, 5].Value));
                            transportSeal.IsActive = true;
                            transportSeal.Remarks = sheet.Cells[rowNum, 6].Text; ;
                            transportSeal.CreatedBy = userid;
                            transportSeal.CreatedDate = currentdatetime;
                            transportSeal.UpdatedBy = userid;
                            transportSeal.UpdatedDate = currentdatetime;

                            _generalRepo.Insert(transportSeal);
                            _generalRepo.Save();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                listError.Add(e.Message);
            }
            return listError;
        }

        public List<string> ValidateData(System.IO.Stream stream, IEnumerable<MasterLocation> listMasterLocation, IEnumerable<TransportExecution> listTransportExecution, IEnumerable<MasterTruckSealStock> listSealNumber)
        {
            List<string> listError = new List<string>();

            using (ExcelPackage xlPackage = new ExcelPackage(stream))
            {
                var sheet = xlPackage.Workbook.Worksheets[1];

                var totalRows = sheet.Dimension.End.Row;
                var totalColumns = sheet.Dimension.End.Column;

                for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                {
                    var textError = "";
                    var listErrorPerRow = new List<string>();

                    if (sheet.Cells[rowNum, 1].Text.Trim().Equals("")) break;

                    var isExistTN = listTransportExecution.ToList().Where(x => x.TransportNo.ToUpper() == sheet.Cells[rowNum, 1].Text.Trim().ToUpper()).ToList();
                    if (isExistTN.Count == 0) listErrorPerRow.Add(sheet.Cells[rowNum, 1].Text.Trim() == "" ? stringErrorEmpty("Transaction Number", rowNum) : stringErrorIsNotExist("Transaction Number", rowNum));

                    if (sheet.Cells[rowNum, 2].Text != "")
                    {
                        var isExistSN = listSealNumber.ToList().Where(x => Int32.Parse(x.SealNumberFrom) <= Int32.Parse(sheet.Cells[rowNum, 2].Text) && Int32.Parse(x.SealNumberTo) >= Int32.Parse(sheet.Cells[rowNum, 2].Text)).ToList();
                        listErrorPerRow.Add(isExistSN.Count == 0 ? stringErrorIsNotExist("Seal Number", rowNum) : "");
                    } else
                    {
                        listErrorPerRow.Add(stringErrorEmpty("Seal Number", rowNum));
                    }
                    
                    listErrorPerRow.Add(sheet.Cells[rowNum, 3].Text.Trim() == "" ? stringErrorEmpty("Activity", rowNum) : "");

                    var isExistML = listMasterLocation.ToList().Where(x => x.LocationName.ToUpper() == sheet.Cells[rowNum, 4].Text.Trim().ToUpper()).ToList();
                    if (isExistML.Count == 0) listErrorPerRow.Add(sheet.Cells[rowNum, 4].Text.Trim() == "" ? stringErrorEmpty("Location", rowNum) : stringErrorIsNotExist("Location", rowNum));

                    listErrorPerRow.Add(sheet.Cells[rowNum, 5].Text.Trim() == "" ? stringErrorEmpty("Date and Time", rowNum) : "");

                    listErrorPerRow.RemoveAll(x => string.IsNullOrEmpty(x));

                    textError = listErrorPerRow.Count > 0 ? "Row # " + rowNum + ": " + String.Join(", ", listErrorPerRow) + "." : "";

                    if(textError != "") listError.Add(textError);
                }
            }
            
            return listError;
        }

        public string stringErrorEmpty(string colName, int rowNum) {
            return colName + " can't be empty";
        }

        public string stringErrorIsNotExist(string colName, int rowNum)
        {
            return colName + " not found";
        }

        public bool DeleteSealNumber(string transportationNumber, string sealNumber)
        {
            try
            {
                var records = _generalRepo.GetAll().Where(x => x.TransportExecution.TransportNo == transportationNumber && x.SealNumber == sealNumber).ToList();

                foreach (var data in records)
                {
                    _generalRepo.Delete(data.IDTransportSeal);
                    _generalRepo.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<MasterLocationDTO> GetLocationByTN(string TN)
        {
            List<MasterLocationDTO> result = new List<MasterLocationDTO>();
            var IDTE = _transportExecutionRepo.Get(x => x.TransportNo == TN);
            if (IDTE.Count() > 0)
            {
                var listLocationFromTOSender = _transportOrderRepo.GetAll().Where(x => x.IDTransportExecution == IDTE.Single().IDTransportExecution).Select(x => x.ActualSenderIDLocation).ToList();
                var listLocationFromTOReceiver = _transportOrderRepo.GetAll().Where(x => x.IDTransportExecution == IDTE.Single().IDTransportExecution).Select(x => x.ActualReceiverIDLocation).ToList();
                var listMasterLocation = _masterLocationRepo.Get(x => x.IsActive);
                var listLocation = listLocationFromTOReceiver.Concat(listLocationFromTOSender).Distinct();
                foreach (var record in listLocation)
                {
                    var tempRecord = new MasterLocationDTO();
                    tempRecord.IDLocation = record;
                    tempRecord.LocationName = listMasterLocation.Where(x => x.IDLocation == record).Select(x => x.LocationName).Count() > 0 ? listMasterLocation.Where(x => x.IDLocation == record).Single().LocationName : "";
                    result.Add(tempRecord);
                }
            }
            return result;
        }
    }
}
