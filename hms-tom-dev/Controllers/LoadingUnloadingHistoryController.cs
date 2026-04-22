using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using TOM.Transport.BusinessLogics;
using TOM.Transport.BusinessLogics.TransportSealBLL;
using DFIS.Universal.Domain.Outputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Transport;
using hms_tom_dev.Util;
using TOM.Master.BusinessLogics;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using System.Web;
using hms_tom_dev.Helper;

namespace hms_tom_dev.Controllers
{
    public class LoadingUnloadingHistoryController : BaseController
    {
        private readonly IMasterUserLocationMappingBLL _mstuserlocationmappingbll;
        private readonly ITransportSealBLL _transportSeal;
        private readonly ITransportOrderBLL _transportorderBll;
        private readonly IMasterLocationBLL _masterLocationBll;

        public LoadingUnloadingHistoryController(IMasterUserLocationMappingBLL mstuserlocationmappingbll, ITransportSealBLL transportSeal, ITransportOrderBLL transportorderBll, IMasterLocationBLL masterLocationBll)
        {
            _mstuserlocationmappingbll = mstuserlocationmappingbll;
            _transportSeal = transportSeal;
            _masterLocationBll = masterLocationBll;
            //_masterTruckSealBll = masterTruckSealBll;
            _transportorderBll = transportorderBll;
            SetPage(EnumHelper.GetDescription(Enums.PageName.LoadingUnloadingHistory));
        }
        //
        // GET: /LoadingUnloadingHistory/
        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var viewModel = new TransportOrderViewModel();
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.ListUserRole = GetListUserRole();
            return View(viewModel);
        }

        public ActionResult EditIndex()
        {
            return View();
        }

        public ActionResult PartialViewCustomReport()
        {
            ViewBag.ListUserRole = GetListUserRole();

            return View("_CustomReport");
        }

        public ActionResult GetLocationByIdUser()
        {
            string id = GetUserId();//Request.Params["IdUser"];
            string type = Request.Params["Type"];
            var dbResult = _mstuserlocationmappingbll.GetLocationsByIdUserAndWarehouseType(id);
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWarehouseLocation(string transportno)
        {
            var dbResult = _transportorderBll.GetLocatioByTn(transportno);
            var viewMapper = Mapper.Map<List<TransportOrderViewModel>>(dbResult);
            return Json(viewMapper, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportOrderByNo()
        {
            var id = Request.Params["TransportNo"];
            var state = Request.Params["State"];
            var dbResult = _transportorderBll.GetTransportOrderByTransactionNo(id,state);
            var viewMapper = Mapper.Map<List<TransportOrderViewModel>>(dbResult);
            return Json(viewMapper, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportDateResult()
        {
            var listTransportDate = _transportorderBll.GetTransportDate();
            return Json(listTransportDate, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetSenderResult()
        {
            //var dbResult = _transportorderBll.GetSenderLocationByTn();

            var dbResult = _masterLocationBll.GetByConfig("TOMLocationFilter");
            var viewMapper = dbResult.Select(l => new TransportOrderViewModel() { IdLocation = l.IDLocation, LocationName = l.LocationName });
            var output = Json(viewMapper, JsonRequestBehavior.AllowGet);
            output.MaxJsonLength = int.MaxValue;
            return output;

        }

        public ActionResult GetReceiverResult()
        {
            //var dbResult = _transportorderBll.GetReceiverLocationByTn();

            var dbResult = _masterLocationBll.GetByConfig("TOMLocationFilter");
            var viewMapper = dbResult.Select(l => new TransportOrderViewModel() { IdLocation = l.IDLocation, LocationName = l.LocationName });
            var output = Json(viewMapper, JsonRequestBehavior.AllowGet);
            output.MaxJsonLength = int.MaxValue;
            return output;
        }

        public ActionResult GetData(string transportNo, string[] senderLocation, string[] receiverLocation, DateTime? transportEndDate, DateTime? transportStartDate, string state)
        {
            var roleName = GetListUserRole().FirstOrDefault().RoleName;

            var dbResult = state == "advSearch"
                ? _transportorderBll.GetDataDtos(transportNo, senderLocation.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray(), receiverLocation.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray(), transportEndDate, transportStartDate, roleName)
                : _transportorderBll.GetDataDtos(transportNo, roleName);

            var res = dbResult.Select(d => new LoadingUnloadingHistoryDataGridDTO()
            {
                IsActive = d.IsActive,
                LoadBoxperWorkingTime = d.LoadBoxperWorkingTime,
                ReceiverLocationName = d.MasterLocation1 != null ? d.MasterLocation1.LocationName : "",
                SenderLocationName = d.MasterLocation != null ? d.MasterLocation.LocationName : "",
                STONo = d.STONo,
                TransportDate = d.TransportExecution != null ? d.TransportExecution.TransportDate : (DateTime?)null,
                TransportNo = d.TransportExecution != null ? d.TransportExecution.TransportNo : null,
                UnloadBoxperWorkingTime = d.UnloadBoxperWorkingTime
            });
            
            return Json(res , JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportationNumberFilter(string term)
        {
            bool IsRoleTransport = GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA));
            return Json(_transportorderBll.GetTransportationNumberFilterByRegion(term, IsRoleTransport, GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadTemplate()
        {
            var errorMessage = "";
            var status = Enums.ResponseType.Success.ToString();
            DataSet result = null;
            Excel.IExcelDataReader reader = null;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase fileContent = Request.Files[file];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    try
                    {
                        var filename = fileContent.FileName;
                        var stream = fileContent.InputStream;
                        if (filename.EndsWith(".xls"))
                        {
                            reader = Excel.ExcelReaderFactory.CreateBinaryReader(fileContent.InputStream);
                        }
                        else if (filename.EndsWith(".xlsx"))
                        {
                            reader = Excel.ExcelReaderFactory.CreateOpenXmlReader(fileContent.InputStream);
                        }

                        reader.IsFirstRowAsColumnNames = true;
                        result = reader.AsDataSet();
                        reader.Close();
                        List<string> OrderNumbers = new List<string>();
                        //var hashOrder = new Hashtable();                        
                        if (result.Tables.Count > 0)
                        {
                            var model = new List<TransportOrderInput>();
                            foreach (DataRow r in result.Tables[0].Rows)
                            {
                                string OrderNumber = "" + r[0];
                                DateTime? StartDateTime = !String.IsNullOrEmpty(""+r[1]) ? DateTime.Parse(""+r[1]) : (DateTime?)null;
                                DateTime? FinishDateTime = !String.IsNullOrEmpty("" + r[2]) ? DateTime.Parse("" + r[2]) : (DateTime?)null;
                                var newRow = new TransportOrderInput();
                                newRow.STONo = OrderNumber;
                                OrderNumbers.Add(OrderNumber);
                                if (result.Tables[0].TableName == "Loading")
                                {
                                    newRow.LoadStartTime = StartDateTime;
                                    newRow.LoadFinishTime  = FinishDateTime;
                                    //TimeSpan loadtimediff = FinishDateTime.Value.Subtract(StartDateTime.Value);
                                    //var intMinutes = loadtimediff.TotalMinutes;
                                    //newRow.LoadBoxperWorkingTime = Convert.ToDecimal(intMinutes);
                                }
                                else if (result.Tables[0].TableName == "Unloading")                                
                                {
                                    newRow.UnloadStartTime = StartDateTime;
                                    newRow.UnloadFinishTime = FinishDateTime;
                                    //TimeSpan unloadtimediff = FinishDateTime.Value.Subtract(StartDateTime.Value);
                                    //var intMinutesunload = unloadtimediff.TotalMinutes;
                                    //newRow.UnloadBoxperWorkingTime = Convert.ToDecimal(intMinutesunload);
                                }
                                model.Add(newRow);
                                //hashOrder.Add(newRow.STONo, model);                               
                            }

                            _transportorderBll.UploadData(model, OrderNumbers, result.Tables[0].TableName);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        status = Enums.ResponseType.Error.ToString();
                    }

                }
            }

            var jsonResult = new
            {
                status = status,
                message = status == Enums.ResponseType.Error.ToString() ? errorMessage : "Upload Failed"
            };
            return Json(jsonResult);
        }


        [HttpPost]
        public ActionResult EditList(InsertUpdateData<TransportOrderViewModel> bulkData)
        {
            if (bulkData.Edit != null)
            {
                foreach (var t in bulkData.Edit)
                {
                    if (t == null) continue;
                    var datain = Mapper.Map<TransportOrderDTO>(t);
                    try
                    {
                        var item = _transportorderBll.EditData(datain);
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

        public JsonResult ExportToExcel(string param1, string[] param2, string[] param3, DateTime? param4, DateTime? param5, string[] Columns)
        {
            //input.IsExportOrSearch = true;
            var data = _transportorderBll.GetDataExcelDtos(param1, param2, param3, param4, param5);
            
            ExcelExportHelper eeh = new ExcelExportHelper();
            eeh.Map("Transport No", "TransportNo");
            eeh.Map("Order No", "STONo");
            eeh.Map("Transport Date", "TransportDate");
            eeh.Map("Sender", "SenderLocationName");
            eeh.Map("Receiver", "ReceiverLocationName");
            eeh.Map("Code", "Code");
            eeh.Map("Description", "Description");
            eeh.Map("Qty", "Qty");
            eeh.Map("UoM", "UoM");
            eeh.Map("Load Start Time", "LoadStartTime");
            eeh.Map("Load Finish Time", "LoadFinishTime");
            eeh.Map("Unload Start Time", "UnloadStartTime");
            eeh.Map("Unload Finish Time", "UnloadFinishTime");
            eeh.Map("Load Box/Working Time", "LoadBoxperWorkingTime");
            eeh.Map("Unload Box/Working Time", "UnloadBoxperWorkingTime");

            if (Columns == null || Columns.Length == 0)
            {
                //  ini exportan
                Columns = new string[] {
                    "Transport No",
                    "Order No",
                    "Transport Date",
                    "Sender",
                    "Receiver",
                    "Load Working Time",
                    "Unload Working Time"
                };
            }
            eeh.SetHeaderDisplay(Columns);

            var ws = eeh.CreateWorksheet("Export");
            eeh.WriteHeader(ws);

            // write values
            eeh.WriteRows(ws, data, (dto, cell, pi) =>
            {
                var val = pi.GetValue(dto);
                if (pi.Name == "LoadStartTime") cell.Style.Numberformat.Format = "dd MMM yyyy HH:mm";
                if (pi.Name == "LoadFinishTime") cell.Style.Numberformat.Format = "dd MMM yyyy HH:mm";
                if (pi.Name == "UnloadStartTime") cell.Style.Numberformat.Format = "dd MMM yyyy HH:mm";
                if (pi.Name == "UnloadFinishTime") cell.Style.Numberformat.Format = "dd MMM yyyy HH:mm";
                if (pi.Name == "TransportDate") cell.Style.Numberformat.Format = "dd-MMM-yyyy";

                if (val is DateTime?)
                {
                    var vdt = (DateTime?)val;
                    if (vdt.HasValue && vdt == DateTime.MinValue)
                        return null;
                }

                return val;
            });
            eeh.AutoSizeColumns(ws);
            var path = "LoadingUnloadingHistory_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            eeh.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
            return Json(path);
        }

        public JsonResult ExportToExcelDefault(string TransportationNumber)
        {
            var roleName = GetListUserRole().FirstOrDefault().RoleName;

            List<TransportOrderDTO> records = _transportorderBll.GetDataDtos(TransportationNumber, roleName);

            DataTable boundTable = new DataTable();
            boundTable.Columns.Add("Transportation Number", typeof(string));
            boundTable.Columns.Add("Transportation Date", typeof(string));
            boundTable.Columns.Add("Order Number", typeof(string));
            boundTable.Columns.Add("Sender", typeof(string));
            boundTable.Columns.Add("Receiver", typeof(string));
            boundTable.Columns.Add("LoadBoxperWorkingTime", typeof(string));
            boundTable.Columns.Add("UnloadBoxperWorkingTime", typeof(string));

            foreach (TransportOrderDTO item in records)
            {
                dynamic dr = boundTable.NewRow();
                dr["Transportation Number"] = item.TransportExecution.TransportNo;
                dr["Transportation Date"] = item.TransportExecution.TransportDate.ToString("MM/dd/yyyy HH:mm:ss.fff");
                dr["Order Number"] = item.STONo;
                dr["Sender"] = item.MasterLocation1.LocationName;
                dr["Receiver"] = item.MasterLocation.LocationName;
                dr["LoadBoxperWorkingTime"] = "";
                dr["UnloadBoxperWorkingTime"] = "";
                boundTable.Rows.Add(dr);
            }

            DataTableToExcel dataTableToExcel = new DataTableToExcel();
            string path = "LoadingUnloadingHistory" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx";
            dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path);

            return Json(path, JsonRequestBehavior.AllowGet);
        }
    }
}