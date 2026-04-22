using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DFIS.Universal.Domain.Outputs;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.Inputs;
using hms_tom_dev.Models.Masters;
using AutoMapper;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.BusinessLogics.TransportExecutionBLL;
using hms_tom_dev.Models.Transport;
using DFIS.Universal.Domain.Inputs;
using TOM.Transport.BusinessLogics.TransportMonitoringBLL;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.Transport.Domain.DTOs;
using hms_tom_dev.Models.Common;
using DFIS.Universal;
using TOM.Transport.BusinessLogics;
using hms_tom_dev.Helper;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.BusinessLogics;
using System.Data;
using DFIS.Contracts;
using System.Configuration;

namespace hms_tom_dev.Controllers
{
    public class TransportationMonitoringController : BaseController
    {
        private readonly ITransportMonitoringBLL _transportMonitoringBLL;
        private readonly IMasterVendorTOMBLL _masterVendorTOMBll;
        private readonly ITransportExecutionBLL _transportExecutionBLL;
        private readonly IMasterLocationBLL _masterLocationBLL;
        private readonly IMasterListBLL _masterList;
        private readonly IMasterUserBLL _bllUser;
        private readonly ITransportOrderBLL _bllTO;
        private readonly ICustomReportStateBLL _bllCustomReport;
        private readonly ITransportDriverManagementBLL _transDriverManagementBLL;
        private readonly IMasterConfigurationBLL _masterConfigurationBll;

        //
        // GET: /TransportationVesselMonitoring/
        public TransportationMonitoringController(IMasterUserBLL bllUser, IMasterConfigurationBLL masterConfigurationBll, ICustomReportStateBLL bllCustomReport, ITransportOrderBLL bllTO, ITransportDriverManagementBLL transDriverManagementBLL, ITransportMonitoringBLL transportMonitoringBLL, IMasterVendorTOMBLL masterVendorTOMBll, ITransportExecutionBLL transportExecutionBLL, IMasterLocationBLL masterLocationBLL, IMasterListBLL masterList)
        {
            _transportMonitoringBLL = transportMonitoringBLL;
            _masterVendorTOMBll = masterVendorTOMBll;
            _transportExecutionBLL = transportExecutionBLL;
            _masterLocationBLL = masterLocationBLL;
            _masterList = masterList;
            _bllTO = bllTO;
            _bllCustomReport = bllCustomReport;
            _transDriverManagementBLL = transDriverManagementBLL;
            _bllUser = bllUser;
            _masterConfigurationBll = masterConfigurationBll;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportationMonitoring));
        }

        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            //var viewModel = new TransportVesselMonitoringViewModel();
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            var _userRole = GetListUserRole();
            ViewBag.ListUserRole = _userRole;
            //Role View
            var UserRole = _userRole.FirstOrDefault().RoleName;
            ViewBag.UserRole = "";
            if (UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                ViewBag.UserRole = UserRole;
            }

            return View();
        }

        public ActionResult PartialViewIndexShip()
        {
            List<SelectListItem> tempVendorName = _masterVendorTOMBll.GetDatas(new MasterVendorTOMInput { TransportationMode = "Ship", IsActive = true }).OrderBy(o => o.VendorName).Select(f => new SelectListItem { Text = f.VendorName, Value = f.IDVendor.ToString() }).ToList();

            TransportMonitoringViewModel viewModel = new TransportMonitoringViewModel();

            var masterListData = _transportMonitoringBLL.getMasterListForFilter(new List<string>(new[] { "TransportationStatus" }));
            var _transStatus = masterListData.Where(_ => _.FieldValue != "Draft" && _.FieldValue != "Submit").Select(_ => new SelectListItem { Text = _.FieldValue, Value = _.FieldValue }).ToList();
            viewModel.transStatus = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.transStatus.AddRange(_transStatus);

            string[] locType = new string[] { "Warehouse", "Factory", "Agent", "Other", "Transport", "TPO" };
            var masterLocationData = _masterLocationBLL.Get(_ => locType.Contains(_.Type)).Select(_ => new SelectListItem { Text = _.LocationName, Value = _.IDLocation }).ToList();
            viewModel.locationNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.locationNames.AddRange(masterLocationData);

            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            if (UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                viewModel.UserRole = UserRole;
            }

            viewModel.vendorNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.vendorNames.AddRange(tempVendorName);

            return View("IndexShip", viewModel);
        }

        public ActionResult PartialViewIndexTruck()
        {
            List<SelectListItem> tempVendorName = _masterVendorTOMBll.GetDatas(new MasterVendorTOMInput { TransportationMode = "Truck", IsActive = true }).OrderBy(o => o.VendorName).Select(f => new SelectListItem { Text = f.VendorName, Value = f.IDVendor.ToString() }).ToList();
            
            TransportMonitoringViewModel viewModel = new TransportMonitoringViewModel();

            var masterListData = _transportMonitoringBLL.getMasterListForFilter(new List<string>(new[] { "TransportationStatus" }));
            var _transStatus = masterListData.Where(_ => _.FieldValue != "Draft" && _.FieldValue != "Submit").Select(_ => new SelectListItem { Text = _.FieldValue, Value = _.FieldValue }).ToList();
            viewModel.transStatus = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.transStatus.AddRange(_transStatus);

            string[] locType = new string[] { "Warehouse", "Factory", "Agent", "Other", "Transport", "TPO" };
            var masterLocationData = _masterLocationBLL.Get(_ => locType.Contains(_.Type)).Select(_ => new SelectListItem { Text = _.LocationName, Value = _.IDLocation }).ToList();
            viewModel.locationNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.locationNames.AddRange(masterLocationData);

            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            if (UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                viewModel.UserRole = UserRole;
            }

            viewModel.vendorNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.vendorNames.AddRange(tempVendorName);

            return View("IndexTruck", viewModel);
        }

        public ActionResult PartialViewIndexTrain()
        {
            TransportMonitoringViewModel viewModel = new TransportMonitoringViewModel();

            List<SelectListItem> tempVendorName = _masterVendorTOMBll.GetDatas(new MasterVendorTOMInput { TransportationMode = "Train", IsActive = true }).OrderBy(o => o.VendorName).Select(f => new SelectListItem { Text = f.VendorName, Value = f.IDVendor.ToString() }).ToList();

            var masterListData = _transportMonitoringBLL.getMasterListForFilter(new List<string>(new[] { "TransportationStatus" }));
            var _transStatus = masterListData.Where(_ => _.FieldValue != "Draft" && _.FieldValue != "Submit").Select(_ => new SelectListItem { Text = _.FieldValue, Value = _.FieldValue }).ToList();
            viewModel.transStatus = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.transStatus.AddRange(_transStatus);

            string[] locType = new string[] { "Warehouse", "Factory", "Agent", "Other", "Transport", "TPO" };
            var masterLocationData = _masterLocationBLL.Get(_ => locType.Contains(_.Type)).Select(_ => new SelectListItem { Text = _.LocationName, Value = _.IDLocation }).ToList();
            viewModel.locationNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.locationNames.AddRange(masterLocationData);

            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            if (UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                viewModel.UserRole = UserRole;
            }

            viewModel.vendorNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.vendorNames.AddRange(tempVendorName);

            return View("IndexTrain", viewModel);
        }

        public ActionResult EditShip()
        {
            //var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            //var viewModel = new TransportVesselMonitoringViewModel();

            //ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ////ViewBag.ClosingPageDate = GetLockDateRangeList();
            //ViewBag.ListLocation = session.Location.Select(x => x.IDLocation).ToList();
            //ViewBag.ListUserRole = session.Role.Select(x => x.RoleName).ToList();

            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            if (UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                ViewBag.UserRole = UserRole;
            }

            return View("EditShip");
        }

        public ActionResult GetVendor(MasterVendorTOMInput input)
        {
            var dbResult = _masterVendorTOMBll.GetDatas(input);
            var viewMapper = Mapper.Map<List<MasterVendorTOMViewModel>>(dbResult);
            return Json(viewMapper, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportNoDtos(TransportationExecutionInput input)
        {
            var dbResult = _transportExecutionBLL.GetTransportNo(input);
            var viewMapper = Mapper.Map<List<TransportExecutionViewModel>>(dbResult).Where(evm => !string.IsNullOrWhiteSpace(evm.TransportNo) && !evm.TransportNo.ToUpper().StartsWith("E-"));
            return Json(viewMapper, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMstConfigurationRecords(MasterConfigurationInput criteria)
        {
            var masterConfiguration = _masterConfigurationBll.GetRecords(criteria);
            var viewModel = Mapper.Map<List<MasterConfigurationViewModel>>(masterConfiguration);
            var output = Json(viewModel, JsonRequestBehavior.AllowGet);

            output.MaxJsonLength = int.MaxValue;

            return output;
        }

        public ActionResult GetOrderNoDtos(TransportOrderInput input)
        {
            var dbResult = _transportExecutionBLL.GetOrderNo(input);
            var viewMapper = Mapper.Map<List<TransportOrderViewModel>>(dbResult);
            return Json(viewMapper, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocations(MasterLocationInput input)
        {
            var dbResult = _masterLocationBLL.GetMasterLocations(input).Where( l => l.IsAssigned.HasValue && l.IsAssigned.Value);
            var viewMapper = Mapper.Map<List<MasterLocationViewModel>>(dbResult);
            return Json(viewMapper.Where(x => x.IsActive), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTransportStatus(MasterListInput input)
        {
            var dbResult = _masterList.GetMasterLists(input).Where(x => x.FieldValue.ToUpper() != "SUBMIT" && x.FieldValue.ToUpper() != "DRAFT");
            var viewMapper = Mapper.Map<List<MasterListViewModel>>(dbResult);
            return Json(viewMapper, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataShip(TransportMonitoringViewInput criteria)
        {
            var result = _transportMonitoringBLL.getDataShip(criteria).OrderByDescending(x => x.TransportExecution.TransportDate).ThenBy(x => x.TransportExecution.TransportNo);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportMonitoringDatas(TransportMonitoringViewInput criteria)
        {
            var dbResult = _transportMonitoringBLL.GetListTruckTrain(
                "ship",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                ""
                );
            var viewModel = MappingHelper.Map<TransportMonitoringViewViewModel>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateData(TransportMonitoringViewInput input)
        {
            var userid = GetUserId();
            try
            {
                _transportExecutionBLL.UpdateVessel(input, userid);
                //_transportExecutionBLL.UpdateOrder(input, userid);
                return Json(Enums.ResponseType.Success.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateDataShip(TransportMonitoringViewInput input)
        {
            var userid = GetUserId();
            try
            {
                //_transportExecutionBLL.UpdateVessel(input, userid);
                //_transportExecutionBLL.UpdateOrder(input, userid);
                _transportMonitoringBLL.updateShip(input, userid);
                return Json(Enums.ResponseType.Success.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        #region SUB TRUCK TRAIN
        private List<UserRole> GetListUserRole()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var listrole = currentSession.Role.ToList();
            return listrole;
        }
        public ActionResult GetListViewTruckTrain(string mode, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl)
        {
            var isSuperAdmin = GetListUserRole().Select(c => c.RoleName).Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA));            
            var dbResult = _transportMonitoringBLL.GetListTruckTrain(mode, fltrtn, fltrdf, fltrdt, fltrts, fltron, fltrvn, fltrse, fltrre, fltrsl, fltrfl).Where(
                c => (c.IsActive || isSuperAdmin) && (!string.IsNullOrWhiteSpace(c.TransportNo) && !c.TransportNo.StartsWith("E-TN"))
                ).OrderByDescending(x => x.TransportDate).ThenBy(x => x.TransportNo);

            var allTransportDriver = _transDriverManagementBLL.GetDataByCriteria(new TransportDriverManagementInput());

            foreach (var records in dbResult)
            {
                var driver1 = allTransportDriver.Where(x => x.ID == records.Driver1).FirstOrDefault();
                var driver2 = allTransportDriver.Where(x => x.ID == records.Driver2).FirstOrDefault();
                var coDriver = allTransportDriver.Where(x => x.ID == records.CoDriver).FirstOrDefault();

                if (driver1 != null)
                {
                    records.Driver1 = driver1.Name;
                }
                if (driver2 != null)
                {
                    records.Driver2 = driver2.Name;
                }
                if (coDriver != null)
                {
                    records.CoDriver = coDriver.Name;
                }
                //records.Driver1 = _transDriverManagementBLL.GetById(records.Driver1) == null ? "" : _transDriverManagementBLL.GetById(records.Driver1).Name;
                //records.Driver2 = _transDriverManagementBLL.GetById(records.Driver2) == null ? "" : _transDriverManagementBLL.GetById(records.Driver2).Name;
                //records.CoDriver = _transDriverManagementBLL.GetById(records.CoDriver) == null ? "" : _transDriverManagementBLL.GetById(records.CoDriver).Name;
            }

            var jsonResult = Json(dbResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListViewTruckShipTrainServerSide(TransportMonitoringFilterInput filter, DataTableModel model)
        {
            dynamic data = new System.Dynamic.ExpandoObject();
            int total = 0;
            var isSuperAdmin = GetListUserRole().Select(c => c.RoleName).Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA));

            if (filter.mode == "Truck" || filter.mode == "Train")
            {
                var getData = _transportMonitoringBLL.GetMonitoringDataTableTruckTrain(filter, isSuperAdmin, model);
                total = (int)getData.total;
                var _data = (List<TransportExecutionDTO>) getData.data;
                data = _data.Select(_ => new {
                    _.IDTransportExecution,
                    _.IsActive,
                    _.TransportNo,
                    _.TransportStatus,
                    _.TransportDate,
                    _.ActualVehicleType,
                    _.VendorName,
                    _.StartLocation,
                    _.FinishLocation,
                    _.ContainerNo,
                    _.Via,
                    _.PoliceRegistrationNumber,
                    _.Driver1,
                    _.Driver2,
                    _.CoDriver,
                    _.CurrentLocation,
                    _.CurrentLocationUpdateTime
                });
            }
            else
            {
                var getData = _transportMonitoringBLL.GetMonitoringDataTableShip(filter, isSuperAdmin, model);
                var _data = (List<TransportVesselMonitoringDTO>) getData.data;
                total = (int)getData.total;
                data = _data.Select(_ => new { 
                    _.IsActive,
                    _.IDTransportExecution,
                    _.IDTransportVesselMonitoring,
                    _.TransportExecution.TransportNo,
                    _.TransportExecution.TransportDate,
                    _.StartLocationName,
                    _.FinishLocationName,
                    _.TransportExecution.MasterVendor.VendorName,
                    _.VesselName,
                    _.TransportExecution.TransportStatus,
                    _.ContainerNo,
                    _.TransportExecution.Via,
                    _.ETD1,
                    _.ETD2,
                    _.ATD,
                    _.EstReceived,
                    _.Remarks
                });
            }

            var result = new
            {
                data = data,
                draw = model.draw,
                recordsTotal = total,
                recordsFiltered = total,
            };

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        //public ActionResult GetCountTruckTrain(string modeS, string fltrtn, string fltrdf, string fltrdt, string fltrts, string fltron, string fltrvn, string fltrse, string fltrre, string fltrsl, string fltrfl, string stOrder)
        //{
        //    var mode = modeS;
        //    //var req = Request;
        //    //List<object> req = new List<object>() { };
        //    object reqFilter =  null;


        //    var isSuperAdmin = GetListUserRole().Select(c => c.RoleName).Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA));
        //    //var resultCount = _transportMonitoringBLL.GetCountTruckTrainServerSide(mode, fltrtn, fltrdf, fltrdt, fltrts, fltron, fltrvn, fltrse, fltrre, fltrsl, fltrfl, isSuperAdmin, reqFilter, stOrder);
        //    var db = _transportMonitoringBLL.GetMonitoringDataTable(mode, fltrtn, fltrdf, fltrdt, fltrts, fltron, fltrvn, fltrse, fltrre, fltrsl, fltrfl, isSuperAdmin, reqFilter, stOrder, 0, 0);
        //    var result = new
        //    {
        //        resultCount = db.total,
        //    };
        //    var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;
        //    //return Json(dbResult, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult DetailTruck(string id = null)
        {
            TransportExecutionViewModel viewModel = new TransportExecutionViewModel();
            if (!String.IsNullOrEmpty(id))
            {
                var dbResult = _transportMonitoringBLL.GetDetail(id);
                if (dbResult != null)
                {
                    viewModel = Mapper.Map<TransportExecutionDTO, TransportExecutionViewModel>(dbResult);

                    viewModel.TransportOrders = Mapper.Map<List<TransportOrderViewModel>>(_transportMonitoringBLL.GetRowListView(id));
                    var tvo = viewModel.TransportOrders.FirstOrDefault();
                    viewModel.TransportMode = "Truck";
                    viewModel.VehicleType = dbResult.ActualVehicleType;

                    var userRole = GetListUserRole().FirstOrDefault().RoleName;
                    if (userRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
                    {
                        viewModel.UserRole = userRole;
                    }
                    /*
                    if (tvo.IDRequest != null)
                    {
                        var tor = _bllTO.GetRequests(null, tvo.IDRequest).FirstOrDefault();
                        if (tor != null)
                            viewModel.VehicleType = tor.VehicleType;
                    }
                    */
                    return View("DetailTruck", viewModel);
                }
            }
            return RedirectToAction("Index", new { });
        }

        public ActionResult DetailTrain(string id)
        {
            TransportExecutionViewModel viewModel = new TransportExecutionViewModel();
            if (!String.IsNullOrEmpty(id))
            {
                var dbResult = _transportMonitoringBLL.GetDetail(id);
                if (dbResult != null)
                {
                    viewModel = Mapper.Map<TransportExecutionDTO, TransportExecutionViewModel>(dbResult);
                    viewModel.TransportOrders = Mapper.Map<List<TransportOrderViewModel>>(_transportMonitoringBLL.GetRowListView(id));
                    var tvo = viewModel.TransportOrders.FirstOrDefault();
                    viewModel.TransportMode = "Train";
                    viewModel.VehicleType = dbResult.ActualVehicleType;

                    var userRole = GetListUserRole().FirstOrDefault().RoleName;
                    if (userRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
                    {
                        viewModel.UserRole = userRole;
                    }
                    /*
                    if (tvo != null)
                    {
                        var tor = _bllTO.GetRequests(null, tvo.IDRequest).FirstOrDefault();
                        if (tor != null)
                            viewModel.VehicleType = tor.VehicleType;
                    }
                    */
                    return View("DetailTruck", viewModel);
                }
            }
            return RedirectToAction("Index", new { });
        }

        public JsonResult ExportMonitoringXlsShip(TransportMonitoringFilterInput criteria)
        {
            ExcelPackage xls_file = new ExcelPackage();
            ExcelWorksheet xls_sheet;
            ExcelRange xls_range;

            string[] colMonths = { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string[] colTitles = { "", "Transportation Number", "Stuffing Date", "Start Location", "Finish Location", "Vendor Name", "Vessel Name", "Transportation Status", "Container No.", "Via", "ETD 1", "ETD 2", "ATD", "Estimate Received", "Remarks" };

            xls_sheet = xls_file.Workbook.Worksheets.Add("Export");

            var isSuperAdmin = GetListUserRole().Select(c => c.RoleName).Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA));

            var getData = _transportMonitoringBLL.GetMonitoringDataTableShip(criteria, isSuperAdmin);
            var dbResult = (List<TransportVesselMonitoringDTO>)getData.data;

            for (int c = 1; c <= 11; c++)
            {
                using (xls_range = xls_sheet.Cells[1, c])
                {
                    xls_range.Value = colTitles[c];
                    xls_range.Style.Font.Size = 10;
                    xls_range.Style.Font.Bold = true;
                    xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                    xls_range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                }
                xls_sheet.Column(c).Width = 15;
            }

            int r = 2;

            foreach (var data in dbResult)
            {
                string[] dtaRows = {
                    "",
                    data.TransportExecution.TransportNo,
                    data.TransportExecution.TransportDate.ToString("dd-MMM-yyyy"),
                    data.StartLocationName,
                    data.FinishLocationName,
                    data.TransportExecution.MasterVendor != null ? data.TransportExecution.MasterVendor.VendorName : "",
                    data.VesselName,
                    data.TransportExecution.TransportStatus,
                    data.ContainerNo,
                    data.TransportExecution.Via,
                    data.ETD1 != null ? ((DateTime)data.ETD1).ToString("dd-MMM-yyyy") : "",
                    data.ETD2 != null ? ((DateTime)data.ETD2).ToString("dd-MMM-yyyy") : "",
                    data.ATD != null ? ((DateTime)data.ATD).ToString("dd-MMM-yyyy") : "",
                    data.EstReceived != null ? ((DateTime)data.EstReceived).ToString("dd-MMM-yyyy") : "",
                    data.Remarks,
                };

                for (int c = 1; c <= 11; c++)
                {
                    using (xls_range = xls_sheet.Cells[r, c])
                    {
                        xls_range.Value = dtaRows[c];
                        if (c == 2 || (c >= 10 && c <= 13))
                        {
                            xls_range.Style.Numberformat.Format = "dd-MMM-yyyy";
                        }

                        xls_range.Style.Font.Size = 10;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                }

                r++;
            }

            string path = "TransportMonitoringShip" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            xls_file.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

            return Json(path, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomMonitoringXlsShip(TransportMonitoringFilterInput criteria, string colCaption, string colIndex, string[] colExport)
        {
            ExcelPackage xls_file = new ExcelPackage();

            var isSuperAdmin = GetListUserRole().Select(c => c.RoleName).Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA));

            criteria.isForExport = true;
            var getData = _transportMonitoringBLL.GetMonitoringDataTableShip(criteria, isSuperAdmin);
            var dbResult = (List<TransportVesselMonitoringDTO>)getData.data;


            var expRes = new List<TransportMonitoringExportShipDTO>();
            foreach (var data in dbResult)
            {
                foreach (var recTO in data.TransportExecution.TransportOrderList)
                {
                    var dto = new TransportMonitoringExportShipDTO();
                    dto.TransportationNumber = data.TransportExecution != null ? data.TransportExecution.TransportNo : null;
                    dto.STONumber = !string.IsNullOrEmpty(recTO.STONo) ? recTO.STONo : null;
                    dto.StuffingDate = data.TransportExecution != null ? data.TransportExecution.TransportDate : (DateTime?)null;
                    dto.StartLocation = data.StartLocationName;
                    dto.FinishLocation = data.FinishLocationName;
                    dto.VendorName = data.TransportExecution != null ? (data.TransportExecution.MasterVendor != null ? data.TransportExecution.MasterVendor.VendorName : "") : "";
                    dto.VesselName = data.VesselName;
                    dto.ETD1 = data.ETD1;
                    dto.ETD2 = data.ETD2;
                    dto.ATD = data.ATD;
                    dto.EstimateReceived = data.EstReceived;
                    dto.Remarks = data.Remarks;
                    dto.TransportationStatus = data.TransportExecution != null ? data.TransportExecution.TransportStatus : "";
                    dto.ETA1 = data.ETA1;
                    dto.ETA2 = data.ETA2;
                    dto.ATA = data.ATA;
                    dto.ActualTimeBerthing = data.ActualTimeBerthing;
                    dto.ContainerNo = data.ContainerNo;
                    dto.ContainerSeal = data.ContainerSeal;
                    dto.OrderStatus = recTO.OrderStatus;
                    dto.Sender = recTO.ActualSenderLocationName;
                    dto.Receiver = recTO.ActualReceiverLocationName;
                    dto.GIDate = recTO.GIDate;
                    dto.GRDate = recTO.GRDate;
                    dto.GRBy = recTO.GRBy;
                    dto.CreatedBy = _bllUser.GetFullName(recTO.CreatedBy);
                    dto.CreatedDate = recTO.CreatedDate;
                    dto.UpdatedBy = _bllUser.GetFullName(recTO.UpdatedBy);
                    dto.UpdatedDate = recTO.UpdatedDate;
                    dto.POWeekAgent = recTO.POWeek.HasValue && recTO.POWeek.Value > 0 ? recTO.POWeek : null;
                    expRes.Add(dto);
                }
            }

            using (var pkg = new ExcelExportHelper())
            {
                pkg.Map("Transportation Number");
                pkg.Map("STO Number");
                pkg.Map("Stuffing Date");
                pkg.Map("Start Location");
                pkg.Map("Finish Location");
                pkg.Map("Vendor Name");
                pkg.Map("Vessel Name");
                pkg.Map("ETD 1");
                pkg.Map("ETD 2");
                pkg.Map("ATD");
                pkg.Map("Estimate Received");
                pkg.Map("Remarks");
                pkg.Map("Transportation Status");
                pkg.Map("ETA 1");
                pkg.Map("ETA 2");
                pkg.Map("ATA");
                pkg.Map("Actual Time Berthing");
                pkg.Map("Container No");
                pkg.Map("Via");
                pkg.Map("Container Seal");
                pkg.Map("Order Status");
                pkg.Map("PO Week Agent");
                pkg.Map("Sender");
                pkg.Map("Receiver");
                pkg.Map("GI Date");
                pkg.Map("GR Date");
                pkg.Map("GR By");
                pkg.Map("Created By");
                pkg.Map("Created Date");
                pkg.Map("Updated By");
                pkg.Map("Updated Date");

                pkg.SetHeaderDisplay(colExport);

                var ws = pkg.CreateWorksheet("Export");
                pkg.WriteHeader(ws);
                pkg.WriteRowsNested(ws, expRes, (data) =>
                {
                    var o = data.OriginalObject;
                    //o.TransportExecution.MasterVendor.VendorName

                    if (data.ExcelHeaderName != null && (
                        data.ExcelHeaderName.Contains("Date")
                        || data.ExcelHeaderName.Contains("ETD")
                        || data.ExcelHeaderName.Contains("ETA")
                        || data.ExcelHeaderName.Contains("ATD")
                        || data.ExcelHeaderName.Contains("ATA")
                        || data.ExcelHeaderName.Contains("Estimate Received")
                        || data.ExcelHeaderName.Contains("Actual Time Berthing")
                    ))
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.Value is DateTime && (DateTime)data.Value == DateTime.MinValue)
                            return null;
                    }
                    return data.Value;
                });
                pkg.AutoSizeColumns(ws);

                var fname = "TransportMonitoringCustomShip_" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx";
                pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + fname));

                return Json(fname, JsonRequestBehavior.AllowGet);
            }


            #region Old Logic
            // ====================================================================================================================
            ExcelWorksheet xls_sheet;
            ExcelRange xls_range;

            string[] colMonths = { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            xls_sheet = xls_file.Workbook.Worksheets.Add("Export");

            int cols = colCaption.Split(',').Count();

            //using (xls_range = xls_sheet.Cells[1, 1, 1, cols])
            //{
            //    xls_range.Value = "Monitoring Custom - Ship";
            //    xls_range.Merge = true;
            //    xls_range.Style.Font.Size = 10;
            //    xls_range.Style.Font.Bold = true;
            //    xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //    xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //}

            int nn = 0;
            for (int n = 0; n < cols; n++)
            {
                //using (xls_range = xls_sheet.Cells[2, n + 1])
                //{
                //    xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.White);
                //}
                using (xls_range = xls_sheet.Cells[1, n + 1])
                {
                    xls_range.Value = colCaption.Split(',')[n];

                    xls_range.Style.Font.Size = 10;
                    xls_range.Style.Font.Bold = true;
                    xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                    xls_range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                }

                xls_sheet.Column(n + 1).Width = 15;
                nn = n;
            }

            //xls_sheet.Cells[2, nn + 1].Style.Border.Right.Style = ExcelBorderStyle.None;

            int r = 2;

            //var dbResult = _transportMonitoringBLL.getDataShip(criteria).Where(c => c.TransportExecution != null && !(c.TransportExecution.TransportNo != null && c.TransportExecution.TransportNo.StartsWith("E-TN")));

            foreach (var data in dbResult)
            {
                MasterUserInput inputUser = new MasterUserInput();
                inputUser.IsActive = true;
                var listUser = _bllUser.GetMasterUsers(inputUser).ToList();
                foreach (var recTO in data.TransportExecution.TransportOrderList)
                {
                    var flagTN = true;
                    var createdBy = !string.IsNullOrEmpty(recTO.CreatedBy) ? listUser.Where(x => x.IDUser.ToUpper() == recTO.CreatedBy.ToUpper()).Count() > 0 ? listUser.Where(x => x.IDUser.ToUpper() == recTO.CreatedBy.ToUpper()).First().FullName : "" : "";
                    var updatedBy = !string.IsNullOrEmpty(recTO.UpdatedBy) ? listUser.Where(x => x.IDUser.ToUpper() == recTO.UpdatedBy.ToUpper()).Count() > 0 ? listUser.Where(x => x.IDUser.ToUpper() == recTO.UpdatedBy.ToUpper()).First().FullName : "" : "";
                    //var IDGRby = data.TransportExecution != null ? (data.TransportExecution.TransportOrderList != null ? (data.TransportExecution.TransportOrderList.Count > 0 ? data.TransportExecution.TransportOrderList.First().GRBy : "") : "") : "";
                    var GRBy = !string.IsNullOrEmpty(recTO.GRBy) ? listUser.Where(x => x.IDUser.ToUpper() == recTO.GRBy.ToUpper()).Count() > 0 ? listUser.Where(x => x.IDUser.ToUpper() == recTO.GRBy.ToUpper()).First().FullName : "" : "";
                    string[] dtaRows = {
                        "",
                        flagTN ? data.TransportExecution != null ? data.TransportExecution.TransportNo : "" : "",
                        !string.IsNullOrEmpty(recTO.STONo) ? recTO.STONo : "",
                        data.TransportExecution != null ? data.TransportExecution.TransportDate.ToString("dd-MMM-yyyy") : "",
                        data.StartLocationName,
                        data.FinishLocationName,
                        data.TransportExecution != null ? (data.TransportExecution.MasterVendor != null? data.TransportExecution.MasterVendor.VendorName : "") : "",
                        data.VesselName,
                        data.ETD1 != null ? ((DateTime)data.ETD1).ToString("dd-MMM-yyyy") : "",
                        data.ETD2 != null ? ((DateTime)data.ETD2).ToString("dd-MMM-yyyy") : "",
                        data.ATD != null ? ((DateTime)data.ATD).ToString("dd-MMM-yyyy") : "",
                        data.EstReceived != null ? ((DateTime)data.EstReceived).ToString("dd-MMM-yyyy") : "",
                        data.Remarks,
                        data.TransportExecution != null ? data.TransportExecution.TransportStatus : "",
                        data.ETA1 != null ? ((DateTime)data.ETA1).ToString("dd-MMM-yyyy") : "",
                        data.ETA2 != null ? ((DateTime)data.ETA2).ToString("dd-MMM-yyyy") : "",
                        data.ATA != null ? ((DateTime)data.ATA).ToString("dd-MMM-yyyy") : "",
                        data.ActualTimeBerthing != null ? ((DateTime)data.ActualTimeBerthing).ToString("dd-MMM-yyyy") : "",
                        data.ContainerNo,
                        data.ContainerSeal,
                        recTO.OrderStatus,
                        recTO.ActualSenderLocationName,
                        recTO.ActualReceiverLocationName,
                        recTO.GIDate != null ? ((DateTime)recTO.GIDate).ToString("dd-MMM-yyyy") : "",
                        recTO.GRDate != null ? ((DateTime)recTO.GRDate).ToString("dd-MMM-yyyy") : "",
                        GRBy,
                        createdBy,
                        ((DateTime)recTO.CreatedDate).ToString("dd-MMM-yyyy"),
                        updatedBy,
                        ((DateTime)recTO.UpdatedDate).ToString("dd-MMM-yyyy"),
                    };
                    flagTN = false;

                    for (int n = 0; n < cols; n++)
                    {
                        using (xls_range = xls_sheet.Cells[r, n + 1])
                        {
                            int c = int.Parse(colIndex.Split(',')[n]);
                            xls_range.Value = dtaRows[c];
                            if (c == 3 || (c >= 7 && c <= 10) || (c >= 13 && c <= 16) || (c >= 22 && c <= 23) || c == 26 || c == 28)
                            {
                                xls_range.Style.Numberformat.Format = "dd-MMM-yyyy";
                            }
                            //if (c >= 6 && c <= 9)
                            //{
                            //    xls_range.Value = "";
                            //    if (dtaRows[c] != "01-01-0001 00:00:00" && dtaRows[c] != "")
                            //    {
                            //        //xls_range.Value = dtaRows[c].Split('-')[0] + '/' + colMonths[Convert.ToInt32(dtaRows[c].Split('-')[1])] + '/' + dtaRows[c].Split('-')[2];
                            //        xls_range.Value = dtaRows[c];
                            //    }
                            //}
                            //else
                            //{
                            //    xls_range.Value = dtaRows[c];
                            //}

                            xls_range.Style.Font.Size = 10;
                            xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            //xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            //xls_range.Style.Border.Bottom.Color.SetColor(Color.Black);
                            //xls_range.Style.Border.Top.Color.SetColor(Color.Black);
                        }

                        //if (n == 0)
                        //{
                        //    xls_sheet.Cells[r, 1].Style.Border.Left.Style = ExcelBorderStyle.Medium;
                        //}
                        //else if ((n + 1) == cols)
                        //{
                        //    xls_sheet.Cells[r, n + 1].Style.Border.Right.Style = ExcelBorderStyle.Medium;
                        //}
                    }

                    r++;
                }
            }

            String path = "TransportMonitoringCustomShip" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            xls_file.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

            return Json(path, JsonRequestBehavior.AllowGet);
            #endregion
        }

        public JsonResult ExportMonitoringXlsTruckTrain(TransportMonitoringFilterInput filter)
        {
            var isSuperAdmin = GetListUserRole().Select(c => c.RoleName).Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA));

            var getData = _transportMonitoringBLL.GetMonitoringDataTableTruckTrain(filter, isSuperAdmin);
            var dbResult = (List<TransportExecutionDTO>)getData.data;

            using (var pkg = new ExcelExportHelper())
            {
                pkg.Map("Transportation Number", "TransportNo");
                pkg.Map("Transportation Date", "TransportDate");
                pkg.Map("Vehicle Type", "ActualVehicleType");
                pkg.Map("Vendor Name", "VendorName");
                pkg.Map("Transportation Status", "TransportStatus");
                pkg.Map("Start Location", "StartLocation");
                pkg.Map("Finish Location", "FinishLocation");
                if (filter.mode == "Train")
                {
                    pkg.Map("Container No.", "ContainerNo");
                    pkg.Map("Via", "Via");
                }
                pkg.Map("Police Registration Number", "PoliceRegistrationNumber");
                pkg.Map("Driver 1", "Driver1");
                pkg.Map("Driver 2", "Driver2");
                pkg.Map("Co-Driver", "CoDriver");
                pkg.Map("Location", "CurrentLocation");
                pkg.Map("Date & Time", "CurrentLocationUpdateTime");

                var ws = pkg.CreateWorksheet("Export");
                pkg.WriteHeader(ws);
                pkg.WriteRowsNested(ws, dbResult, (data) =>
                {
                    if (data.ExcelHeaderName == "Date & Time")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                        if (data.OriginalObject.CurrentLocationUpdateTime == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "Transportation Date")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.TransportDate == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "GR By")
                    {
                        if (string.IsNullOrWhiteSpace(data.OriginalObject.UpdatedBy) || data.OriginalObject.UpdatedDate == DateTime.MinValue)
                            return null;
                        return (data.Value != null ? data.Value.ToString() : "") + " on " + data.OriginalObject.UpdatedDate.ToString("dd-MMM-yyyy HH:mm:ss");
                    }
                    return data.Value;
                });
                pkg.AutoSizeColumns(ws);

                var fname = "TransportMonitoring" + filter.mode.Substring(0, 1).ToUpper() + filter.mode.Substring(1) + "_" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx";
                pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + fname));

                return Json(fname, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CustomMonitoringXlsTruckTrain(TransportMonitoringFilterInput filter, string[] cols)
        {
            var isSuperAdmin = GetListUserRole().Select(c => c.RoleName).Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA));

            var getData = _transportMonitoringBLL.GetMonitoringDataTableTruckTrain(filter, isSuperAdmin);
            var dbResult = (List<TransportExecutionDTO>)getData.data;

            List<TransportExecutionDTO> _rawData = new List<TransportExecutionDTO>();

            foreach (var records in dbResult)
            {
                var getTO = _bllTO.GetTransportOrderByIDTransportExecution(records.IDTransportExecution);
                bool hasTO = false;
                foreach (var to in getTO.Where(_ => _.IsActive))
                {
                    TransportExecutionDTO dto = records;
                    dto.LeadTime = to.LeadTime;
                    dto.STONo = to.STONo;
                    dto.EstReceived = to.EstArrivalDate;
                    var snd = _masterLocationBLL.GetById(to.ActualSenderIDLocation);
                    dto.Sender = snd != null ? snd.LocationName : null;
                    var rcv = _masterLocationBLL.GetById(to.ActualReceiverIDLocation);
                    dto.Receiver = rcv != null ? rcv.LocationName : null;
                    var usr = _bllUser.GetMasterUsers(new MasterUserInput() { IDUser = to.CreatedBy }).FirstOrDefault();
                    dto.CreatedBy = usr != null && to.CreatedBy != null ? usr.FullName : to.CreatedBy;
                    usr = _bllUser.GetMasterUsers(new MasterUserInput() { IDUser = to.UpdatedBy }).FirstOrDefault();
                    dto.UpdatedBy = usr != null && to.UpdatedBy != null ? usr.FullName : to.UpdatedBy;
                    usr = _bllUser.GetMasterUsers(new MasterUserInput() { IDUser = to.GRBy }).FirstOrDefault();
                    dto.GRBy = usr != null && to.GRBy != null ? usr.FullName : to.GRBy;
                    usr = _bllUser.GetMasterUsers(new MasterUserInput() { IDUser = to.GIBy }).FirstOrDefault();
                    dto.GIBy = usr != null && to.GIBy != null ? usr.FullName : to.GIBy;
                    dto.GIDate = to.GIDate;
                    dto.GRDate = to.GRDate;
                    dto.OrderStatus = to.OrderStatus;

                    int jumlahPosition = 0;
                    if (dto.TransportPositionDetails != null)
                    {
                        List<TransportPositionDetailDTO> tempList = dto.TransportPositionDetails.OrderByDescending(x => x.CreatedDate).ToList();
                        foreach (TransportPositionDetailDTO temp in tempList)
                        {
                            if (jumlahPosition == 3)
                                break;
                            if (jumlahPosition == 0)
                            {
                                dto.PositionDate1 = temp.PositionDate;
                                dto.PositionName1 = temp.PositionName;
                            }
                            else if (jumlahPosition == 1)
                            {
                                dto.PositionDate2 = temp.PositionDate;
                                dto.PositionName2 = temp.PositionName;
                            }
                            else if (jumlahPosition == 2)
                            {
                                dto.PositionDate3 = temp.PositionDate;
                                dto.PositionName3 = temp.PositionName;
                            }
                            jumlahPosition++;
                        }
                    }

                    hasTO = true;
                    _rawData.Add(dto);
                }
                if (!hasTO) _rawData.Add(records);
            }

            using (var pkg = new ExcelExportHelper())
            {
                pkg.Map("Transportation Number", "TransportNo");
                pkg.Map("Transportation Date", "TransportDate");
                pkg.Map("Vehicle Type", "ActualVehicleType");
                pkg.Map("Vendor Name", "VendorName");
                pkg.Map("Transportation Status", "TransportStatus");
                pkg.Map("Start Location", "StartLocation");
                pkg.Map("Finish Location", "FinishLocation");
                if (filter.mode == "Train")
                {
                    pkg.Map("Container No.", "ContainerNo");
                    pkg.Map("Via", "Via");
                }
                pkg.Map("Police Registration Number", "PoliceRegistrationNumber");
                pkg.Map("Driver 1", "Driver1");
                pkg.Map("Driver 2", "Driver2");
                pkg.Map("Co-Driver", "CoDriver");
                //pkg.Map("Location", "CurrentLocation");
                pkg.Map("Order Number", "STONo");
                pkg.Map("Order Status", "OrderStatus");
                pkg.Map("Sender", "Sender");
                pkg.Map("Receiver", "Receiver");
                pkg.Map("Lead Time", "LeadTime");
                pkg.Map("Estimate Arrival", "EstReceived");
                //pkg.Map("Date & Time", "CurrentLocationUpdateTime");
                pkg.Map("GR By", "GRBy");
                pkg.Map("Created By", "CreatedBy");
                pkg.Map("GI", "GIDate");
                pkg.Map("GR", "GRDate");

                for (var i = 0; i < cols.Length; i++)
                    cols[i] = cols[i] != null ? cols[i].Replace("&amp;", "&") : cols[i];

                List<string> listCol = cols.ToList();

                for (var i = 3; i >= 1; i--)
                {
                    if (Array.IndexOf(cols, "Position Date") > 0)
                    {
                        pkg.Map("Position Date " + i, "PositionDate" + i);
                        listCol.Add("Position Date " + i);
                    }
                    if (Array.IndexOf(cols, "Position Name") > 0)
                    {
                        pkg.Map("Position Name " + i, "PositionName" + i);
                        listCol.Add("Position Name " + i);
                    }
                }

                var ws = pkg.CreateWorksheet("Export");

                pkg.SetHeaderDisplay(listCol.ToArray());
                pkg.WriteHeader(ws);
                pkg.WriteRowsNested(ws, _rawData, (data) =>
                {
                    //if (data.ExcelHeaderName == "Date & Time")
                    //{
                    //    data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    //    if (data.OriginalObject.CurrentLocationUpdateTime == DateTime.MinValue)
                    //        return null;
                    //}
                    if (data.ExcelHeaderName == "Estimate Arrival")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.EstReceived == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "GI")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                        if (!data.OriginalObject.GIDate.HasValue || data.OriginalObject.GIDate.Value == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "GR")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                        if (!data.OriginalObject.GRDate.HasValue || data.OriginalObject.GRDate.Value == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "Transportation Date")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.TransportDate == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName.Contains("Position Date 1"))
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                        if (data.OriginalObject.PositionDate1 == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName.Contains("Position Date 2"))
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                        if (data.OriginalObject.PositionDate2 == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName.Contains("Position Date 3"))
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                        if (data.OriginalObject.PositionDate3 == DateTime.MinValue)
                            return null;
                    }
                    return data.Value;
                });
                pkg.AutoSizeColumns(ws);

                var fname = "TransportMonitoring" + filter.mode.Substring(0, 1).ToUpper() + filter.mode.Substring(1) + "_" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + ".xlsx";
                pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + fname));

                return Json(fname, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ImportMonitoringXlsTruckTrain()
        {
            bool result = false;

            if (Request.Files.Count <= 0) return Json(new NonQueryResult(false, "No file specified."));
            var eih = new ExcelImportHelper();
            eih.Map("Order Number", "OrderNumber");
            eih.Map("Order Status", "OrderStatus");
            eih.Map("GI Date Time", "GIDate");
            eih.Map("GR Date Time", "GRDate");
            eih.Map("Transportation Number", "TransportationNumber");
            eih.Map("Update Position", "UpdatePosition");
            eih.Map("Date Time", "DateTime");

            HttpPostedFileBase file = Request.Files[0];
            try
            {
                var ds = ExcelImportHelper.ImportExcelAsDataSet(file.InputStream, file.FileName.ToLower().EndsWith(".xlsx"));

                var sheets = new List<TransportMonitoringUploadInput>();
                List<string> errmsg = new List<string>();
                List<string> _statuses = new List<string>() {
                    "In Process",
                    "On Delivery",
                    "Arrive At Destination and Waiting For Confirmation",
                    "Complete"
                };
                var dataCol = new Dictionary<string, List<ExcelImportResult<TransportMonitoringUploadInput>>>();
                foreach (DataTable table in ds.Tables)
                {
                    if (table.TableName == "Information") continue;
                    var col = new List<ExcelImportResult<TransportMonitoringUploadInput>>();
                    dataCol.Add(table.TableName, col);

                    var excelRes = eih.Deserialize<TransportMonitoringUploadInput>(table, c =>
                    {
                        if (!string.IsNullOrWhiteSpace(c.OrderNumber))
                        {
                            ExcelImportHelper.CheckHasValue(c.OrderNumber, "Order Number");
                            var to = _bllTO.Get(new TransportOrderInput() { STONo = c.OrderNumber }).FirstOrDefault();
                            if (to == null)
                                throw new RowValidationByFieldException("Order Number", c.OrderNumber);

                            var ni = _statuses.IndexOf(c.OrderStatus);
                            var oi = _statuses.IndexOf(to.OrderStatus);
                            if (!string.IsNullOrWhiteSpace(c.OrderStatus))
                            {
                                if (ni <= 0)
                                    throw new RowValidationByFieldException("Order Status", c.OrderStatus);
                                if (ni < oi)
                                    throw new RowValidationByFieldException("Order Status", c.OrderStatus, " cannot override '" + to.OrderStatus + "' in");
                            }

                            return c;
                        }
                        if (!string.IsNullOrWhiteSpace(c.TransportationNumber))
                        {
                            ExcelImportHelper.CheckHasValue(c.TransportationNumber, "Transportation Number");
                            var tn = _transportExecutionBLL.GetTransportNo(c.TransportationNumber);
                            if (tn == null)
                                throw new RowValidationByFieldException("Transportation Number", c.TransportationNumber);

                            ExcelImportHelper.CheckHasValue(c.UpdatePosition, "Update Position");
                            ExcelImportHelper.CheckHasValue(c.DateTime, "Date Time");

                            return c;
                        }
                        throw new Exception("Invalid table structure!");
                    });
                    col.AddRange(excelRes);
                }

                bool valid = false;
                string errmes = "";
                foreach (var kv in dataCol)
                {
                    var col = kv.Value;
                    var errs = col.Where(c => !c.Valid).Select(c => kv.Key + ": Row #" + c.RowNumber + " - " + c.ErrorMessage).ToList();
                    if (errs.Count > 0)
                        errmes += "&bull; " + string.Join("<br>&bull; ", errs) + "<br />";
                }
                if (errmes != "")
                    return Json(new NonQueryResult(false, errmes));

                // do import
                var imported = new List<TransportMonitoringUploadInput>();
                foreach (var kv in dataCol)
                {
                    var data = kv.Value.Where(c => c.Valid).Select(c => c.Result);
                    imported.AddRange(data);
                }

                _transportMonitoringBLL.ImportXlsGI(imported, GetUserName());
                _transportMonitoringBLL.ImportXlsUP(imported, GetUserName());

                return Json(new NonQueryResult(true));
            }
            catch (Exception ex)
            {
                return Json(new NonQueryResult(false, ex));
            }

            #region Old Logic
            /*
            if (System.IO.Path.GetExtension(file.FileName).ToLower() == ".xlsx")
            {
                ExcelPackage xls_file = new ExcelPackage(file.InputStream);
                ExcelWorksheet xls_sheet;

                if (xls_file.Workbook.Worksheets.Count > 0 && xls_file.Workbook.Worksheets[1].Name == "Import")
                {
                    xls_sheet = xls_file.Workbook.Worksheets[1];
                    if (xls_sheet.Dimension.End.Column == 3 && xls_sheet.Dimension.End.Row > 1)
                    {
                        if (
                            xls_sheet.Cells[1, 1].Value.ToString().ToLower().Trim() == "order number" &&
                            xls_sheet.Cells[1, 2].Value.ToString().ToLower().Trim() == "order status" &&
                            xls_sheet.Cells[1, 3].Value.ToString().ToLower().Trim() == "gi date time"
                            )
                        {
                            int rowCount = xls_sheet.Dimension.End.Row;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                string on = xls_sheet.Cells[row, 1].Value.ToString().Trim();
                                string os = xls_sheet.Cells[row, 2].Value.ToString().Trim();
                                string gi = xls_sheet.Cells[row, 3].Value.ToString().Trim();

                                Int64 c1, onn; DateTime c3, gii;

                                if (DateTime.TryParse(gi, out c3) == true)
                                {
                                    result = _transportMonitoringBLL.GetImportXlsGI(on, os, c3);
                                }
                            }
                        }
                        else if (
                            xls_sheet.Cells[1, 1].Value.ToString().ToLower().Trim() == "transportation number" &&
                            xls_sheet.Cells[1, 2].Value.ToString().ToLower().Trim() == "update position" &&
                            xls_sheet.Cells[1, 3].Value.ToString().ToLower().Trim() == "date time"
                            )
                        {
                            int rowCount = xls_sheet.Dimension.End.Row;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                string tn = xls_sheet.Cells[row, 1].Value.ToString().Trim();
                                string pn = xls_sheet.Cells[row, 2].Value.ToString().Trim();
                                string dt = xls_sheet.Cells[row, 3].Value.ToString().Trim();

                                DateTime c3, dtt;

                                if (DateTime.TryParse(dt, out c3) == true)
                                {
                                    dtt = Convert.ToDateTime(dt);

                                    var data = new TransportPositionDetailViewModel();
                                    var transPosition = Mapper.Map<TransportPositionDetailDTO>(data);

                                    transPosition.PositionName = pn;
                                    transPosition.PositionDate = dtt;
                                    transPosition.CreatedBy = GetUserId();
                                    transPosition.CreatedDate = DateTime.Now;
                                    transPosition.UpdatedBy = GetUserId();
                                    transPosition.UpdatedDate = DateTime.Now;

                                    result = _transportMonitoringBLL.GetImportXlsUP(tn, transPosition);
                                }
                            }
                        }
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
            */
            #endregion
        }

        [HttpPost]
        public ActionResult ImportMonitoringXlsShip(HttpPostedFileBase file)
        {
            int err = 0, rec = 0; string sht = "";

            //HttpPostedFileBase file = Request.Files[0];

            if (System.IO.Path.GetExtension(file.FileName).ToLower() == ".xlsx")
            {
                ExcelPackage xls_file = new ExcelPackage(file.InputStream);
                ExcelWorksheet xls_sheet;

                if (xls_file.Workbook.Worksheets.Count > 1 && xls_file.Workbook.Worksheets[1].Name == "Import-Update" && xls_file.Workbook.Worksheets[2].Name == "Import-POWeek")
                {
                    xls_sheet = xls_file.Workbook.Worksheets[1];
                    if (xls_sheet.Dimension.End.Column == 5 && xls_sheet.Dimension.End.Row > 1)
                    {
                        sht = "Update";
                        if (xls_sheet.Cells[1, 1].Value.ToString().ToLower().Trim() == "transportation number" &&
                            xls_sheet.Cells[1, 2].Value.ToString().ToLower().Trim() == "etd 2" &&
                            xls_sheet.Cells[1, 3].Value.ToString().ToLower().Trim() == "atd" &&
                            xls_sheet.Cells[1, 4].Value.ToString().ToLower().Trim() == "ata" &&
                            xls_sheet.Cells[1, 5].Value.ToString().ToLower().Trim() == "actual time berthing"
                            )
                        {
                            int rowCount = xls_sheet.Dimension.End.Row;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                string tn = xls_sheet.Cells[row, 1].Value != null ? xls_sheet.Cells[row, 1].Value.ToString().Trim() : null;
                                string et = !string.IsNullOrEmpty(xls_sheet.Cells[row, 2].Text) ? xls_sheet.Cells[row, 2].Value.ToString().Trim() : "";
                                string td = !string.IsNullOrEmpty(xls_sheet.Cells[row, 3].Text) ? xls_sheet.Cells[row, 3].Value.ToString().Trim() : "";
                                string ta = !string.IsNullOrEmpty(xls_sheet.Cells[row, 4].Text) ? xls_sheet.Cells[row, 4].Value.ToString().Trim() : "";
                                string tb = !string.IsNullOrEmpty(xls_sheet.Cells[row, 5].Text) ? xls_sheet.Cells[row, 5].Value.ToString().Trim() : "";

                                DateTime? c2, c3, c4, c5, c22, c33, c44, c55;

                                //if (DateTime.TryParse(et, out c2) == true && DateTime.TryParse(td, out c3) == true && DateTime.TryParse(ta, out c4) == true && DateTime.TryParse(tb, out c5) == true)
                                //{

                                //}
                                //else { err = 2; rec = row - 3; goto error; }

                                c22 = !string.IsNullOrEmpty(et) ? Convert.ToDateTime(et) : (DateTime?)null;
                                c33 = !string.IsNullOrEmpty(td) ? Convert.ToDateTime(td) : (DateTime?)null;
                                c44 = !string.IsNullOrEmpty(ta) ? Convert.ToDateTime(ta) : (DateTime?)null;
                                c55 = !string.IsNullOrEmpty(tb) ? Convert.ToDateTime(tb) : (DateTime?)null;

                                if (tn != null)
                                {
                                    var dbresult = _transportMonitoringBLL.GetSheetUpdateXls(tn, c22, c33, c44, c55);
                                    if (!dbresult) { err = 3; rec = row - 3; goto error; }
                                } else { err = 2; rec = row - 3; goto error; }
                            }
                        }
                        else { err = 1; goto error; }
                    }

                    xls_sheet = xls_file.Workbook.Worksheets[2];
                    if (xls_sheet.Dimension.End.Column == 2 && xls_sheet.Dimension.End.Row > 1)
                    {
                        sht = "PO Week";
                        if (xls_sheet.Cells[1, 1].Value.ToString().ToLower().Trim() == "order number" &&
                            xls_sheet.Cells[1, 2].Value.ToString().ToLower().Trim() == "po week (agent)"
                            )
                        {
                            int rowCount = xls_sheet.Dimension.End.Row;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                string nod = !string.IsNullOrEmpty(xls_sheet.Cells[row, 1].Text) ? xls_sheet.Cells[row, 1].Value.ToString().Trim() : "";
                                string wk = !string.IsNullOrEmpty(xls_sheet.Cells[row, 2].Text) ? xls_sheet.Cells[row, 2].Value.ToString().Trim() : "";

                                Int32? c2, c22;

                                //if (Int32.TryParse(wk, out c2) == true)
                                //{
                                //}
                                //else { err = 2; rec = row - 3; goto error; }
                                c22 = !string.IsNullOrEmpty(wk) ? Convert.ToInt32(wk) : (Int32?)null;

                                var dbresult = _transportMonitoringBLL.GetSheetPOWeekXls(nod, c22);
                                if (!dbresult) { err = 3; rec = row - 3; goto error; }
                            }
                        }
                        else { err = 1; goto error; }
                    }
                }
                else { err = 1; goto error; }
            }

            error:
            return Json(new { err, rec, sht }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportMonitoringXlsShipGR()
        {
            bool result = false;

            if (Request.Files.Count <= 0) return Json(new NonQueryResult(false, "No file specified."));
            var eih = new ExcelImportHelper();
            eih.Map("Order Number", "OrderNumber");
            eih.Map("Order Status", "OrderStatus");
            eih.Map("GI Date Time", "GIDate");
            eih.Map("GR Date Time", "GRDate");
            eih.Map("Transportation Number", "TransportationNumber");
            eih.Map("Update Position", "UpdatePosition");
            eih.Map("Date Time", "DateTime");

            HttpPostedFileBase file = Request.Files[0];
            try
            {
                ExcelPackage xls_file = new ExcelPackage(file.InputStream);
                if (xls_file.Workbook.Worksheets[1].Name == "Import-Update")
                {
                    ImportMonitoringXlsShip(file);
                }
                var ds = ExcelImportHelper.ImportExcelAsDataSet(file.InputStream, file.FileName.ToLower().EndsWith(".xlsx"));
                var sheets = new List<TransportMonitoringUploadInput>();
                List<string> errmsg = new List<string>();
                List<string> _statuses = new List<string>() {
                    "In Process",
                    "On Delivery",
                    "Arrive At Destination and Waiting For Confirmation",
                    "Complete"
                };
                var dataCol = new Dictionary<string, List<ExcelImportResult<TransportMonitoringUploadInput>>>();
                foreach (DataTable table in ds.Tables)
                {
                    if (table.TableName == "Information") continue;
                    var col = new List<ExcelImportResult<TransportMonitoringUploadInput>>();
                    dataCol.Add(table.TableName, col);

                    var excelRes = eih.Deserialize<TransportMonitoringUploadInput>(table, c =>
                    {
                        if (!string.IsNullOrWhiteSpace(c.OrderNumber))
                        {
                            ExcelImportHelper.CheckHasValue(c.OrderNumber, "Order Number");
                            var to = _bllTO.Get(new TransportOrderInput() { STONo = c.OrderNumber }).FirstOrDefault();
                            if (to == null)
                                throw new RowValidationByFieldException("Order Number", c.OrderNumber);

                            var ni = _statuses.IndexOf(c.OrderStatus);
                            var oi = _statuses.IndexOf(to.OrderStatus);
                            if (!string.IsNullOrWhiteSpace(c.OrderStatus))
                            {
                                if (ni <= 0)
                                    throw new RowValidationByFieldException("Order Status", c.OrderStatus);
                                if (ni < oi)
                                    throw new RowValidationByFieldException("Order Status", c.OrderStatus, " cannot override '" + to.OrderStatus + "' in");
                            }

                            return c;
                        }
                        if (!string.IsNullOrWhiteSpace(c.TransportationNumber))
                        {
                            ExcelImportHelper.CheckHasValue(c.TransportationNumber, "Transportation Number");
                            var tn = _transportExecutionBLL.GetTransportNo(c.TransportationNumber);
                            if (tn == null)
                                throw new RowValidationByFieldException("Transportation Number", c.TransportationNumber);

                            ExcelImportHelper.CheckHasValue(c.UpdatePosition, "Update Position");
                            ExcelImportHelper.CheckHasValue(c.DateTime, "Date Time");

                            return c;
                        }
                        throw new Exception("Invalid table structure!");
                    });
                    col.AddRange(excelRes);
                }

                bool valid = false;
                string errmes = "";
                foreach (var kv in dataCol)
                {
                    var col = kv.Value;
                    var errs = col.Where(c => !c.Valid).Select(c => kv.Key + ": Row #" + c.RowNumber + " - " + c.ErrorMessage).ToList();
                    if (errs.Count > 0)
                        errmes += "&bull; " + string.Join("<br>&bull; ", errs) + "<br />";
                }
                if (errmes != "")
                    return Json(new NonQueryResult(false, errmes));

                // do import
                var imported = new List<TransportMonitoringUploadInput>();
                foreach (var kv in dataCol)
                {
                    var data = kv.Value.Where(c => c.Valid).Select(c => c.Result);
                    imported.AddRange(data);
                }

                _transportMonitoringBLL.ImportXlsGI(imported, GetUserName());
                _transportMonitoringBLL.ImportXlsUP(imported, GetUserName());

                return Json(new NonQueryResult(true));
            }
            catch (Exception ex)
            {
                return Json(new NonQueryResult(false, ex));
            }
        }
        public ActionResult GetRowOrderTruckTrain(string idte)
        {
            var dbResult = _transportMonitoringBLL.GetRowListView(idte);
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterListOrderSts()
        {
            var dbResult = _transportMonitoringBLL.GetListOrderSts();
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMasterListOrderStatus()
        {
            var filter = new string[] {
                "In Process",
                "On Delivery",
                "Arrive At Destination and Waiting For Confirmation",
                "Complete"
            };
            return Json(filter, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateTransportOrder(List<TransportOrderMonitoringInput> TO, int TEID)
        {
            if (_transportMonitoringBLL.UpdateDataTransOrd(TO, TEID, GetUserId()))
                return Json(new NonQueryResult(true));
            return Json(new NonQueryResult(false));
        }

        public ActionResult GetTransportPosition(int TEID)
        {
            var val = _transportMonitoringBLL.GetPosition(TEID);
            return Json(val, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteTransportPosition(int TPID)
        {
            var val = _transportMonitoringBLL.DeletePosition(TPID);
            return Json(new NonQueryResult(val));
        }

        public ActionResult AddTransportPosition(TransportOrderPositionInput pos)
        {
            var transPosition = new TransportPositionDetailDTO();

            if (pos.ID != null && pos.Date != null)
            {
                try
                {
                    transPosition.IDTransportExecution = (int)pos.ID;
                    transPosition.PositionDate = (DateTime)pos.Date;
                    transPosition.PositionName = pos.Location;
                    transPosition.CreatedBy = GetUserId();
                    transPosition.CreatedDate = DateTime.Now;
                    transPosition.UpdatedBy = GetUserId();
                    transPosition.UpdatedDate = DateTime.Now;

                    var val = _transportMonitoringBLL.InsertDataTransOrdPU(transPosition);
                    return Json(new NonQueryResult(val));
                }
                catch
                {
                }
            }
            return Json(new NonQueryResult(false));
        }

        public ActionResult InsertUpdateDataTransOrder(string valTO, string valPU)
        {
            bool result = false;

            if (valTO.Length > 1)
            {
                int row = valTO.Split('|').Count();

                for (int r = 0; r < row; r++)
                {
                    var data = valTO.Split('|')[r];
                    Int64 key = Int64.Parse(data.Split(',')[0]);

                    result = _transportMonitoringBLL.UpdateDataTransOrd(key, data);
                }
            }

            if (valPU.Length > 1)
            {
                int row = valPU.Split('|').Count();

                DateTime dt, dtt;
                for (int r = 0; r < row; r++)
                {
                    var data = valPU.Split('|')[r];
                    var dta = new TransportPositionDetailViewModel();
                    var transPosition = Mapper.Map<TransportPositionDetailDTO>(dta);

                    transPosition.IDTransportExecution = int.Parse(data.Split(',')[0]);
                    if (DateTime.TryParse(data.Split(',')[1], out dt) == true) { dtt = Convert.ToDateTime(data.Split(',')[1]); transPosition.PositionDate = dtt; }
                    transPosition.PositionName = data.Split(',')[2];
                    transPosition.CreatedBy = GetUserId();
                    transPosition.CreatedDate = DateTime.Now;
                    transPosition.UpdatedBy = GetUserId();
                    transPosition.UpdatedDate = DateTime.Now;

                    result = _transportMonitoringBLL.InsertDataTransOrdPU(transPosition);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion SUB TRUCK TRAIN
        public ActionResult LoadCustomReportTru()
        {

            // Stop Caching in IE
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            // Stop Caching in Firefox
            Response.Cache.SetNoStore();
            var crep = _bllCustomReport.GetSavedLayouts(GetUserId(), "MonitoringTruck");

            var col = crep.Select(c => new { FieldName = c.FieldName.Split(','), IsActive = c.IsActive, LayoutName = c.LayoutName }).ToArray();
            return Json(col, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCustomReportTru(string name, string[] layout)
        {
            var dto = new CustomReportStateDTO()
            {
                FieldName = string.Join(",", layout),
                LayoutName = name
            };
            return Json(new NonQueryResult(
                _bllCustomReport.SaveLayouts(GetUserId(), "MonitoringTruck", dto)
                ));
        }
        public ActionResult LoadCustomReportTra()
        {

            // Stop Caching in IE
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            // Stop Caching in Firefox
            Response.Cache.SetNoStore();
            var crep = _bllCustomReport.GetSavedLayouts(GetUserId(), "MonitoringTrain");

            var col = crep.Select(c => new { FieldName = c.FieldName.Split(','), IsActive = c.IsActive, LayoutName = c.LayoutName }).ToArray();
            return Json(col, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCustomReportTra(string name, string[] layout)
        {
            var dto = new CustomReportStateDTO()
            {
                FieldName = string.Join(",", layout),
                LayoutName = name
            };
            return Json(new NonQueryResult(
                _bllCustomReport.SaveLayouts(GetUserId(), "MonitoringTrain", dto)
                ));
        }

        public ActionResult TestSendMail(string to = "")
        {
            try
            {
                var re = to.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                _transportMonitoringBLL.SendMail("Test Mail", "Test Content", re);
                return Json(new NonQueryResult(true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new NonQueryResult(false, ex), JsonRequestBehavior.AllowGet);
            }
        }
    }
}