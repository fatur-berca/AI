using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using AutoMapper;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.Transport.BusinessLogics.TransportSealBLL;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Transport;
using hms_tom_dev.Util;
using TOM.Master.BusinessLogics;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using System.Web;
using hms_tom_dev.Helper;
using TOM.Master.Domain.DTOs;

namespace hms_tom_dev.Controllers
{
    public class TransportationSealController : BaseController
    {
        private readonly IMasterUserLocationMappingBLL _mstuserlocationmappingbll;
        private readonly ITransportSealBLL _transportSealBLL;
        private readonly IMasterTruckSealBLL _masterTruckSealBll;

        public TransportationSealController(IMasterUserLocationMappingBLL mstuserlocationmappingbll, ITransportSealBLL transportSealBLL, IMasterTruckSealBLL masterTruckSealBll)
        {
            _mstuserlocationmappingbll = mstuserlocationmappingbll;
            _transportSealBLL = transportSealBLL;
            _masterTruckSealBll = masterTruckSealBll;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportationSeal));
        }
        //
        // GET: /TransportationSeal/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditIndex()
        {
            return View();
        }

        public ActionResult GetLocationByTN()
        {
            string TN = Request.Params["TN"];
            var dbResult = _transportSealBLL.GetLocationByTN(TN);
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportOrderByNo()
        {
            var id = Request.Params["TransportNo"];
            var dbResult = _transportSealBLL.GetTransportOrderByTransactionNo(id);
            var viewMapper = Mapper.Map<List<TransportOrderViewModel>>(dbResult);
            return Json(viewMapper, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult CheckAvailableSealNumber()
        //{
        //    var sealNumber = Request.Params["SealNumber"];
        //    var sealActivity = Request.Params["SealActivity"];
        //    var dbResult = _masterTruckSealBll.CheckAvailSealNumber(sealNumber, sealActivity);

        //    return Json(dbResult, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetData(TransportSealInput criteria)
        {
            var dbResult = new List<TransportSealDTO>();
            if (!String.IsNullOrEmpty(criteria.TransportNo) || !String.IsNullOrEmpty(criteria.SealNumber)) dbResult = _transportSealBLL.GetDatas(criteria);
            var viewModel = Mapper.Map<List<TransportSealViewModel>>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveData(TransportSealInput input)
        {
            // check valid sealnumber
            var dbResult = new TransportSealViewModel();


            input.CreatedBy = GetUserId();
            try
            {
                var validateSealNumber = _masterTruckSealBll.CheckAvailSealNumber(input.SealNumber);
                if (!validateSealNumber)
                {
                    dbResult.ResponseType = Enums.ResponseType.Invalid.ToString();
                    dbResult.Message = "Seal number not found";
                    return Json(dbResult, JsonRequestBehavior.AllowGet);
                }

                var items = Mapper.Map<TransportSealDTO>(input);
                var validateSealRecord = _transportSealBLL.GetValidateDataSealDtos(items);

                if (validateSealRecord.Count > 0)
                {
                    //jika sudah ada satu data (otomatis data seal), cek seal activity input , jika seal maka suggest untuk input unseal, tapi jika unseal maka lolos
                    if (validateSealRecord.Count == 1)
                    {
                        if (input.SealActivity == "Unseal")
                        {
                            if (validateSealRecord[0].TransportExecution.IDTransportExecution != input.IDTransportExecution)
                            {
                                dbResult.ResponseType = Enums.ResponseType.Invalid.ToString();
                                dbResult.Message = "Seal number <strong>" + input.SealNumber + "</strong> can only be Unsealed on Transportation Number " + validateSealRecord[0].TransportExecution.TransportNo;
                                return Json(dbResult, JsonRequestBehavior.AllowGet);
                            }
                            _transportSealBLL.SaveSealDto(items);
                        }
                        else
                        {
                            dbResult.ResponseType = Enums.ResponseType.Invalid.ToString();
                            dbResult.Message = "Seal number <strong>" + input.SealNumber + "</strong> is already sealed on Transportation Number " + validateSealRecord[0].TransportExecution.TransportNo;
                            return Json(dbResult, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        // jika sudah ada 2 seal berpasangan, maka suggest untuk input seal number baru
                        dbResult.ResponseType = Enums.ResponseType.Invalid.ToString();
                        dbResult.Message = "Seal number " + input.SealNumber + " is already unsealed  on Transportation Number " + validateSealRecord[0].TransportExecution.TransportNo + ". Please use another seal number!";
                        return Json(dbResult, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (input.SealActivity == "Seal")
                    {
                        _transportSealBLL.SaveSealDto(items);
                    }
                    else
                    {
                        dbResult.ResponseType = Enums.ResponseType.Invalid.ToString();
                        dbResult.Message = "Seal number "+input.SealNumber+" not yet sealed.";
                        return Json(dbResult, JsonRequestBehavior.AllowGet);
                    }
                }

                        //var item = _transportSeal.SaveSealDto(input);
                dbResult.ResponseType = Enums.ResponseType.Success.ToString();
                dbResult.Message = "Your data has been successfully saved to database.";
                return Json(dbResult, JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                dbResult.ResponseType = Enums.ResponseType.Error.ToString();
                dbResult.Message = ex.Message;

                return Json(dbResult, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult InsertList(InsertUpdateData<TransportSealViewModel> bulkData)
        {
            if (bulkData.Edit != null)
            {
                foreach (var t in bulkData.Edit)
                {
                    if (t == null) continue;
                    var transSeal = Mapper.Map<TransportSealDTO>(t);
                    transSeal.UpdatedBy = GetUserId();
                    try
                    {
                        transSeal.SealTime = t.SealTime;
                        var item = _transportSealBLL.EditData(transSeal);
                        //bulkData.Edit = Mapper.Map<TransportSealViewModel>(item);
                        t.ResponseType = Enums.ResponseType.Success.ToString();
                        t.Message = "Your data has been successfully saved to database.";
                    }
                    catch (ExceptionBase ex)
                    {
                        t.ResponseType = Enums.ResponseType.Error.ToString();
                        t.Message = ex.Message;
                    }
                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public JsonResult ExportToExcel(TransportSealInput input, string exportSelected)
        {
            DataTable boundTable = new DataTable();
            string path;
            if (exportSelected == "RawData")
            {
                List<TransportSealDTO> records = _transportSealBLL.GetDatas(input);
                using (var pkg = new ExcelExportHelper())
                {
                    pkg.Map("Transportation Number", "TN");
                    pkg.Map("Seal Number", "SealNumber");
                    pkg.Map("Activity", "SealActivity");
                    pkg.Map("Location", "LocationName");
                    pkg.Map("Inputted By", "CreatedByFullName");

                    var ws = pkg.CreateWorksheet("Export");
                    pkg.WriteHeader(ws);
                    pkg.WriteRows(ws, records, (dto, cell, pi) =>
                    {
                        var val = pi.GetValue(dto);
                        return val;
                    });
                    pkg.AutoSizeColumns(ws);

                    path = "TransportSeal-RawData" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx";
                    pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

                    return Json(path, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var listSealStock = _masterTruckSealBll.GetMasterTruckSeal();
                var listTransportSeal = _transportSealBLL.GetDatas(input);
                var records = new List<MasterTruckSealDTO>();

                foreach (var dataSealStock in listSealStock)
                {
                    for (int i=Int32.Parse(dataSealStock.SealNumberFrom); i<=Int32.Parse(dataSealStock.SealNumberTo); i++)
                    {
                        var tempRecord = new MasterTruckSealDTO();
                        //tempRecord.SealNumber = i;
                        var length = dataSealStock.SealNumberFrom.Length;
                        tempRecord.SealNo = i.ToString().PadLeft(length, '0');
                        //var tempStatus = listTransportSeal.FindAll(x => x.SealNumber.EndsWith(i.ToString()) && x.SealActivity == "Seal");
                        var tempStatus = listTransportSeal.FindAll(x => x.SealNumber.Contains(tempRecord.SealNo) && x.SealActivity == "Seal");
                        if (tempStatus.Count > 0)
                        {
                            tempRecord.Status = "N/A";
                            tempRecord.UsedOn = tempStatus[tempStatus.Count - 1].SealTime;
                            tempRecord.TN = tempStatus[tempStatus.Count - 1].TN;
                        }
                        else
                        {
                            tempRecord.Status = "A";
                            tempRecord.UsedOn = null;
                        }
                        records.Add(tempRecord);
                    }
                }
                //records = input.SealNumber != null ? records.FindAll(x => x.SealNumber.ToString() == input.SealNumber) : records;

                using (var pkg = new ExcelExportHelper())
                {
                    pkg.Map("Seal Number", "SealNo");
                    pkg.Map("Status", "Status");
                    pkg.Map("Used On", "UsedOn");
                    pkg.Map("TN", "TN");

                    var ws = pkg.CreateWorksheet("Seal Stock");
                    pkg.WriteHeader(ws);
                    pkg.WriteRows(ws, records, (dto, cell, pi) =>
                    {
                        var val = pi.GetValue(dto);
                        if (pi.Name == "UsedOn") cell.Style.Numberformat.Format = "dd MMM yyyy HH:mm:ss";
                        return val;
                    });
                    pkg.AutoSizeColumns(ws);

                    path = "TransportSeal-SealStock" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                    pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

                    return Json(path, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        [HttpPost]
        public JsonResult exportCustomReport(string colCaption, TransportSealInput input)
        {
            DataTable boundTable = new DataTable();
            string path;

            List<TransportSealDTO> records = _transportSealBLL.GetDatas(input);
            using (var pkg = new ExcelExportHelper())
            {
                foreach (var headerName in colCaption.Split(','))
                {
                    if (headerName.Contains("Transportation Number")) pkg.Map("Transportation Number", "TN");
                    else if (headerName.Contains("Seal Number")) pkg.Map("Seal Number", "SealNumber");
                    else if (headerName.Contains("Activity")) pkg.Map("Activity", "SealActivity");
                    else if (headerName.Contains("Location")) pkg.Map("Location", "LocationName");
                    else if (headerName.Contains("Seal Time")) pkg.Map("Seal Time", "SealTime");
                    else if (headerName.Contains("Remarks")) pkg.Map("Remarks", "Remarks");
                    else if (headerName.Contains("Inputted By")) pkg.Map("Inputted By", "CreatedByFullName");
                }

                var ws = pkg.CreateWorksheet("Export");
                pkg.WriteHeader(ws);
                pkg.WriteRows(ws, records, (dto, cell, pi) =>
                {
                    var val = pi.GetValue(dto);
                    if (pi.Name == "SealTime") cell.Style.Numberformat.Format = "dd MMM yyyy HH:mm:ss";
                    return val;
                });
                pkg.AutoSizeColumns(ws);

                path = "TransportSeal-Custom" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx";
                pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

                return Json(path, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Upload() {
            List<string> listError = new List<string>();

            try
            {
                HttpPostedFileBase fileContent = Request.Files.Get(0);
                listError = _transportSealBLL.Upload(fileContent, GetUserId());
            }
            catch (Exception ex)
            {
                listError.Add(ex.Message.ToString());
            }

            return Json(listError, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteSealNumber(string transportationNumber, string sealNumber)
        {
            bool result = false;
            result = _transportSealBLL.DeleteSealNumber(transportationNumber, sealNumber);
            return Json(result);
        }
    }
}