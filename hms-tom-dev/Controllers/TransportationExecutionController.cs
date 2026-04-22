using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using AutoMapper;
using TOM.Transport.BusinessLogics.TransportExecutionBLL;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Outputs;
using DFIS.Utils;
using hms_tom_dev.Models.Transport;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TOM.EntitiesDAL.EDMX;
using System.Web;
using SelectPdf;
using TOM.Transport.BusinessLogics;
using TOM.Transport.Repositories;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Net.Mail;
using System.Net;
using TOM.Master.Domain.DTOs;
using OfficeOpenXml.DataValidation;
using TOM.Master.BusinessLogics;
using hms_tom_dev.Helper;
using DFIS.Contracts;
using hms_tom_dev.Models.Common;
using System.Configuration;
using DFIS.Universal;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfDocument = SelectPdf.PdfDocument;
using iTextSharp.text.html.simpleparser;
using Rotativa.MVC;
using hms_tom_dev.Models.Universal;

namespace hms_tom_dev.Controllers
{
    public class TransportationExecutionController : BaseController
    {
        private readonly ITransportExecutionBLL _bllTransportExecution;
        private readonly ITransportVehicleDataRepo _transportVehicleDataRepo;
        private readonly ITransportDriverManagementRepo _transportDriverManagementRepo;
        private readonly IMasterListBLL _bllList;
        private readonly IMasterVendorTOMBLL _bllVendor;
        private readonly ITransportDriverManagementBLL _bllDriver;
        private readonly ITransportVehicleDataBLL _bllVehicle;
        private readonly ITransportOrderBLL _bllTO;
        private readonly IMasterLocationBLL _bllLoc;
        private readonly IMasterCostCenterAccountBLL _bllCC;
        //private static List<TransportExecutionAddNewDTO> _transportExecutionAddNew = new List<TransportExecutionAddNewDTO>();

        public TransportationExecutionController(IMasterCostCenterAccountBLL bllCC, IMasterLocationBLL bllLoc, ITransportOrderBLL bllTO, ITransportVehicleDataBLL bllVehicle, ITransportDriverManagementBLL bllDriver, IMasterListBLL bllList, IMasterVendorTOMBLL bllVendor, ITransportExecutionBLL transportExecutionBll, ITransportVehicleDataRepo transportVehicleDataRepo, ITransportDriverManagementRepo transportDriverManagementRepo)
        {
            _bllTransportExecution = transportExecutionBll;
            _transportVehicleDataRepo = transportVehicleDataRepo;
            _transportDriverManagementRepo = transportDriverManagementRepo;
            _bllVendor = bllVendor;
            _bllList = bllList;
            _bllDriver = bllDriver;
            _bllVehicle = bllVehicle;
            _bllTO = bllTO;
            _bllLoc = bllLoc;
            _bllCC = bllCC;

            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportationExecution));
        }


        public ActionResult DownloadTemplateExcel(string mode = null)
        {
            try
            {
                if (mode == "si")
                {
                    // Generate import template headers
                    string[] TemplateHeaders = new string[]
                    {
                        "Transportation Number", "SI Type", "Target of Arrival", "SI Status", "New Vendor", "New SI Type", "New Target of Arrival", "New Transportation Date"
                    };

                    // Generate information column
                    var Vends =
                    _bllVendor.GetALLMasterVendors().Where(c => c.ParentVendor == null).Select(c => c.VendorName).ToList();

                    // Generate the excel file
                    var file = "TransportationExecutionSITemplate_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    var path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + file;
                    var exc = new ExcelPackage(path == null ? null : new System.IO.FileInfo(path));

                    var ws1 = exc.Workbook.Worksheets.Add("SI");
                    for (var i = 1; i <= TemplateHeaders.Length; i++)
                    {
                        ws1.SetValue(1, i, TemplateHeaders[i - 1]);
                    }
                    using (var range = ws1.Cells[1, 1, 1, TemplateHeaders.Length])
                    {
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.Font.Color.SetColor(Color.Black);
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    ws1.Column(3).Style.Numberformat.Format = "dd/MMM/yyyy HH:mm:ss";
                    ws1.Column(7).Style.Numberformat.Format = "dd/MMM/yyyy HH:mm:ss";
                    ws1.Column(8).Style.Numberformat.Format = "dd/MMM/yyyy HH:mm:ss";

                    ws1.SetValue(2, 1, "TN/1801/00001");
                    ws1.SetValue(2, 2, "Weekly");
                    ws1.SetValue(2, 3, DateTime.Now);
                    ws1.SetValue(2, 4, "Fullfil");
                    ws1.SetValue(2, 5, "PT. Serasi Logistics Indonesia (Dedicated)");
                    ws1.SetValue(2, 6, "Weekly");
                    ws1.SetValue(2, 7, DateTime.Now);
                    ws1.SetValue(2, 8, DateTime.Now);

                    var ws2 = exc.Workbook.Worksheets.Add("Information");
                    ws2.SetValue(1, 1, "Vendor Name");

                    var row = 2;
                    foreach (var v in Vends)
                    {
                        ws2.SetValue(row, 1, v);
                        row++;
                    }

                    using (var range = ws2.Cells[1, 1, 1, 1])
                    {
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.Font.Color.SetColor(Color.Black);
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    for (var i = 1; i < ws1.Dimension.Columns; i++)
                        ws1.Column(i).AutoFit();
                    for (var i = 1; i < ws2.Dimension.Columns; i++)
                        ws2.Column(i).AutoFit();

                    exc.Save();

                    return Json(file);
                }
                else if (mode == "tn")
                {
                    // Generate import template headers
                    string[] TemplateHeaders = new string[]
                    {
                        "Transportation Number", "Vessel Name", "Container Number", "Seal Number", "Actual Arrive", "Service PO Number", "Service GR Number", "Police Reg Number",
                        "Driver 1", "Driver 2", "Co-Driver"
                    };

                    // Generate information column
                    var Vessels = _bllList.GetMasterListByFieldName("VesselName").ToList();

                    // Generate the excel file
                    var file = "TransportationExecutionTNTemplate_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    var path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + file;
                    var exc = new ExcelPackage(path == null ? null : new System.IO.FileInfo(path));

                    var ws1 = exc.Workbook.Worksheets.Add("TN");
                    for (var i = 1; i <= TemplateHeaders.Length; i++)
                    {
                        ws1.SetValue(1, i, TemplateHeaders[i - 1]);
                    }
                    using (var range = ws1.Cells[1, 1, 1, TemplateHeaders.Length])
                    {
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.Font.Color.SetColor(Color.Black);
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    ws1.Column(5).Style.Numberformat.Format = "dd/MMM/yyyy HH:mm:ss";

                    ws1.SetValue(2, 1, "TN/1801/00001");
                    ws1.SetValue(2, 2, "Vessel1");
                    ws1.SetValue(2, 3, "Cont12345");
                    ws1.SetValue(2, 4, "Seal12345");
                    ws1.SetValue(2, 5, DateTime.Now);
                    ws1.SetValue(2, 6, "PO12345");
                    ws1.SetValue(2, 7, "GRNUM12345");
                    ws1.SetValue(2, 8, "RN-1234-56");
                    ws1.SetValue(2, 9, "");
                    ws1.SetValue(2, 10, "");
                    ws1.SetValue(2, 11, "");

                    var ws2 = exc.Workbook.Worksheets.Add("Information");
                    ws2.SetValue(1, 1, "Vessel Name");

                    var row = 2;
                    foreach (var v in Vessels)
                    {
                        ws2.SetValue(row, 1, v);
                        row++;
                    }

                    using (var range = ws2.Cells[1, 1, 1, 1])
                    {
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.Font.Color.SetColor(Color.Black);
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    for (var i = 1; i < ws1.Dimension.Columns; i++)
                        ws1.Column(i).AutoFit();
                    for (var i = 1; i < ws2.Dimension.Columns; i++)
                        ws2.Column(i).AutoFit();

                    exc.Save();

                    return Json(file);
                }
                else if (mode == "via")
                {
                    // Generate import template headers
                    string[] TemplateHeaders = new string[]
                    {
                        "Transportation Number", "Via"
                    };

                    // Generate the excel file
                    var file = "TransportationExecutionViaTemplate_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    var path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + file;
                    var exc = new ExcelPackage(path == null ? null : new System.IO.FileInfo(path));

                    var ws1 = exc.Workbook.Worksheets.Add("Via");
                    for (var i = 1; i <= TemplateHeaders.Length; i++)
                    {
                        ws1.SetValue(1, i, TemplateHeaders[i - 1]);
                    }
                    using (var range = ws1.Cells[1, 1, 1, TemplateHeaders.Length])
                    {
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        range.Style.Font.Color.SetColor(Color.Black);
                        range.Style.Font.Bold = true;
                        range.Style.ShrinkToFit = false;
                    }

                    ws1.SetValue(2, 1, "TN/1801/00001");
                    ws1.SetValue(2, 2, "Klari");

                    for (var i = 1; i < ws1.Dimension.Columns; i++)
                    {
                        ws1.Column(i).AutoFit();
                    }

                    exc.Save();

                    return Json(file);
                }
            }
            catch (Exception ex)
            {
            }
            return Json(false);
        }
        /**/

        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.ListUserRole = GetListUserRole();
            TransportExecutionViewModel viewModel = new TransportExecutionViewModel();
            List<MasterListDTO> tempMasterList = _bllTransportExecution.getMasterList();
            List<SelectListItem> tempOrderCategoryList = tempMasterList.Where(x => x.FieldName == "OrderCategory").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.orderCategoryList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.orderCategoryList.AddRange(tempOrderCategoryList);
            List<SelectListItem> tempSIStatusList = tempMasterList.Where(x => x.FieldName == "SIStatus").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.siStatusList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.siStatusList.AddRange(tempSIStatusList);
            List<SelectListItem> tempOrderTypeList = tempMasterList.Where(x => x.FieldName == "OrderType").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.orderTypeList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.orderTypeList.AddRange(tempOrderTypeList);
            List<SelectListItem> tempZoneList = _bllTransportExecution.GetZoneList(GetListUserRole()).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.zoneList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.zoneList.AddRange(tempZoneList);
            List<SelectListItem> tempMaterialTypeList = tempMasterList.Where(x => x.FieldName == "MaterialType").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.materialTypeList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.materialTypeList.AddRange(tempMaterialTypeList);

            bool IsRoleTransport = GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA));
            List<MasterLocationDTO> tempMasterLocation = _bllTransportExecution.getMasterLocation(IsRoleTransport, GetUserRegionSelectList().Select(x => x.Value).ToList());

            string[] locType = new string[] { "Warehouse", "Factory", "Agent", "Other", "Transport", "TPO" };
            List<SelectListItem> tempLocationList = tempMasterLocation
                .Where(l => locType.Contains(l.Type))
                .Select(a => new SelectListItem { Text = a.LocationName, Value = a.IDLocation }).ToList();
            viewModel.locationList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.locationList.AddRange(tempLocationList);
            List<SelectListItem> tempStatusList = tempMasterList
                .Where(x => x.FieldName == "TransportationStatus" && x.FieldValue != "Draft" && x.FieldValue != "Submit")
                .Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.transportStatusList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.transportStatusList.AddRange(tempStatusList);
            List<SelectListItem> tempModeList = tempMasterList.Where(x => x.FieldName == "TransportationMode").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.transportModeList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.transportModeList.AddRange(tempModeList);
            return View(viewModel);
        }

        public ActionResult AddNew()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.ListUserRole = GetListUserRole();
            ViewBag.IsAdminWarehouse = GetListUserRole().Where(f => f.RoleName == EnumHelper.GetDescription(DFIS.Utils.Enums.RoleUserList.AWR)).FirstOrDefault() != null;
            TransportExecutionViewModel viewModel = new TransportExecutionViewModel();
            List<MasterListDTO> tempMasterList = _bllTransportExecution.getMasterList();
            List<SelectListItem> tempExecutionType = tempMasterList.Where(x => x.FieldName == "ExecutionType").OrderBy(x => x.IDList).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.executionTypeList = new List<SelectListItem>();
            viewModel.executionTypeList.AddRange(tempExecutionType);
            List<SelectListItem> tempZoneList = _bllTransportExecution.GetZoneList(GetListUserRole()).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.zoneList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.zoneList.AddRange(tempZoneList);
            List<SelectListItem> tempOrderTypeList = tempMasterList.Where(x => x.FieldName == "OrderType").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.orderTypeList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.orderTypeList.AddRange(tempOrderTypeList);
            //List<MasterLocationDTO> tempMasterLocation = _transportExecutionBll.getMasterLocation(true, GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList());

            bool IsRoleTransport = GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA));
            List<MasterLocationDTO> tempMasterLocation = _bllTransportExecution.getMasterLocation(IsRoleTransport, GetUserRegionSelectList().Select(x => x.Value).ToList());
            string[] locType = new string[] { "Warehouse", "Factory", "Agent", "Other", "TPO", "Transport" };
            List<SelectListItem> tempLocationList = tempMasterLocation
                .Where(l => locType.Contains(l.Type))
                .Select(a => new SelectListItem { Text = a.LocationName, Value = a.IDLocation }).ToList();
            viewModel.locationList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.locationList.AddRange(tempLocationList);

            List<SelectListItem> tempVehicleTypeList = tempMasterList.Where(x => x.FieldName == "VehicleType").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.vehicleTypeList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.vehicleTypeList.AddRange(tempVehicleTypeList);
            List<SelectListItem> tempTransportModeList = tempMasterList.Where(x => x.FieldName == "TransportationMode").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.transportationModeList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.transportationModeList.AddRange(tempTransportModeList);
            List<SelectListItem> tempMasterVendorList = _bllTransportExecution.GetVendorList(GetListUserRole()).Select(a => new SelectListItem { Text = a.VendorName, Value = a.IDVendor.ToString() }).ToList();
            viewModel.vendorList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.vendorList.AddRange(tempMasterVendorList);
            viewModel.weekNow = _bllTransportExecution.GetWeekNow().Week;
            viewModel.yearNow = DateTime.Now.Year;
            return View("AddNew", viewModel);
        }

        public ActionResult Edit()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.ListUserRole = GetListUserRole();
            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            ViewBag.UserRole = "";
            if (UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                ViewBag.UserRole = GetListUserRole().FirstOrDefault().RoleName;
            }

            TransportExecutionViewModel viewModel = new TransportExecutionViewModel();
            int? IDVendor = null;
            if (!String.IsNullOrEmpty(Request.QueryString["pk"]))
            {
                //IDVendor = _bllTransportExecution.GetTransportExecutionByID(Int32.Parse(Request.QueryString["pk"])).IDVendor;
                List<MasterListDTO> tempMasterList = _bllTransportExecution.getMasterList(new List<string>(new[] { "TransportationCategory", "VehicleType", "TransportationMode", "SIType", "SIStatus", "OrderCategory", "ViaRoute", "VesselName" }));
                List<SelectListItem> tempTransportCategory = tempMasterList.Where(x => x.FieldName == "TransportationCategory").OrderBy(x => x.IDList).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
                viewModel.transportCategoryList = new List<SelectListItem>();
                viewModel.transportCategoryList.AddRange(tempTransportCategory);
                List<SelectListItem> tempVehicleType = tempMasterList.Where(x => x.FieldName == "VehicleType").OrderBy(x => x.IDList).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
                viewModel.vehicleTypeList = new List<SelectListItem>();
                viewModel.vehicleTypeList.AddRange(tempVehicleType);
                List<SelectListItem> tempTransportMode = tempMasterList.Where(x => x.FieldName == "TransportationMode").OrderBy(x => x.IDList).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
                viewModel.transportationModeList = new List<SelectListItem>();
                viewModel.transportationModeList.AddRange(tempTransportMode);
                List<SelectListItem> tempSIType = tempMasterList.Where(x => x.FieldName == "SIType").OrderBy(x => x.IDList).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
                viewModel.siTypeList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                viewModel.siTypeList.AddRange(tempSIType);
                List<SelectListItem> tempSIStatus = tempMasterList.Where(x => x.FieldName == "SIStatus").OrderBy(x => x.IDList).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
                viewModel.siStatusList = new List<SelectListItem>();
                viewModel.siStatusList.AddRange(tempSIStatus);
                List<SelectListItem> tempOrderCategory = tempMasterList.Where(x => x.FieldName == "OrderCategory").OrderBy(x => x.IDList).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
                viewModel.orderCategoryList = new List<SelectListItem>();
                viewModel.orderCategoryList.AddRange(tempOrderCategory);
                List<SelectListItem> tempVia = tempMasterList.Where(x => x.FieldName == "ViaRoute").OrderBy(x => x.IDList).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
                viewModel.viaList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                viewModel.viaList.AddRange(tempVia);
                List<SelectListItem> tempVesselName = tempMasterList.Where(x => x.FieldName == "VesselName").OrderBy(x => x.IDList).Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
                viewModel.vesselNameList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                viewModel.vesselNameList.AddRange(tempVesselName);
                List<SelectListItem> tempMasterVendorList = _bllTransportExecution.GetVendorList(GetListUserRole()).Select(a => new SelectListItem { Text = a.VendorName, Value = a.IDVendor.ToString() + "-" + a.VendorCategory + "-" + a.TransportationMode }).ToList();
                viewModel.vendorList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                viewModel.vendorList.AddRange(tempMasterVendorList);

                bool IsRoleTransport = GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA));
                List<MasterLocationDTO> tempMasterLocation = _bllTransportExecution.getMasterLocation(IsRoleTransport, GetUserRegionSelectList().Select(x => x.Value).ToList());
                string[] locType = new string[] { "Warehouse", "Factory", "Agent", "Other", "TPO", "Transport" };
                List<SelectListItem> tempLocationList = tempMasterLocation
                    .Where(l => locType.Contains(l.Type))
                    .Select(a => new SelectListItem { Text = a.LocationName, Value = a.IDLocation }).ToList();
                viewModel.locationList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                viewModel.locationList.AddRange(tempLocationList);

                List<SelectListItem> tempCostCenterList = _bllTransportExecution.GetCostCenterList().Select(a => new SelectListItem { Text = a, Value = a }).ToList();
                viewModel.costCenterList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                viewModel.costCenterList.AddRange(tempCostCenterList);
                //List<SelectListItem> tempVehicleList = _bllTransportExecution.GetVehicleDataList(GetListUserRole()).Where(f => f.IDVendor == IDVendor).Select(a => new SelectListItem { Text = a.IDPoliceRegNumber, Value = a.IDPoliceRegNumber }).ToList();
                //viewModel.policeRegNumberList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "-" } };
                //viewModel.policeRegNumberList.AddRange(tempVehicleList);
                //List<SelectListItem> tempDriverList = _bllTransportExecution.GetDriverList(GetListUserRole()).Where(f => f.IDVendor == IDVendor && f.RoleDriver == "Driver").Select(a => new SelectListItem { Text = a.Name, Value = a.ID }).ToList();
                //viewModel.driverList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "-" } };
                //viewModel.driverList.AddRange(tempDriverList);
                //List<SelectListItem> tempCoDriverList = _bllTransportExecution.GetDriverList(GetListUserRole()).Where(f => f.IDVendor == IDVendor && f.RoleDriver == "CO-Driver").Select(a => new SelectListItem { Text = a.Name, Value = a.ID }).ToList();
                //viewModel.codriverList = new List<SelectListItem> { new SelectListItem { Text = "", Value = "-" } };
                //viewModel.codriverList.AddRange(tempCoDriverList);
            }

            return View("Edit", viewModel);
        }

        public ActionResult GetTransportationNumberFilter(string term)
        {
            bool IsRoleTransport = GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA));
            return Json(_bllTransportExecution.GetTransportationNumberFilter(term, IsRoleTransport, GetUserId()), JsonRequestBehavior.AllowGet);
            //return Json(_bllTransportExecution.GetTransportationNumberFilterByRegion(term, IsRoleTransport, GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTNCreatorShipping(string term)
        {
            var result = _bllTransportExecution.GetTNCreatorShipping(term);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoNoFilter(string term)
        {
            bool IsRoleTransport = GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA));
            return Json(_bllTransportExecution.GetSTONoFilterByRegion(term, IsRoleTransport, GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportOrderByStoNo(string stoNo)
        {
            if (_bllTransportExecution.GetTransportOrderActiveByStoNO(stoNo) == null)
            {
                List<TransportOrderTransportExecutionEditDTO> tempList = new List<TransportOrderTransportExecutionEditDTO>();
                return Json(tempList, JsonRequestBehavior.AllowGet);
            }
            return Json(_bllTransportExecution.GetTransportOrderActiveByStoNO(stoNo), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDriverByCriteriaActive(TransportDriverManagementInput criteria)
        {
            var transportDriverManagement = _bllTransportExecution.GetDriverByCriteriaActive(criteria).Where(x => x.RoleDriver == "Driver");
            var coDriverManagement = _bllTransportExecution.GetDriverByCriteriaActive(criteria).Where(x => x.RoleDriver != null && x.RoleDriver.ToLower() == "co-driver");
            return Json(new { driver = transportDriverManagement, codriver = coDriverManagement }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVehicleByCriteriaActive(TransportVehicleDataInput criteria)
        {
            var transportVehicleData = _bllTransportExecution.GetVehicleByCriteriaActive(criteria);
            return Json(transportVehicleData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTransportExecutionList(TransportOrderInput filter, DataTableModel model)
        {
            if (GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.TRANSPORT)))
                filter.IsRoleTransport = true;
            filter.userRegionList = GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList();
            filter.userLocationList = GetListLocation().Select(x => x.IDLocation).ToList();//GetListLocation().Where(x => x.Type == "Warehouse").Select(x => x.IDLocation).ToList();
            //var tempList = _bllTransportExecution.GetALLTransportationExecution(filter, GetUserId());
            filter.UserRole = GetListUserRole().FirstOrDefault().RoleName;

            var tempList = _bllTransportExecution.GetExecutionDataTable(filter, GetUserId(), model);
            var data = tempList.data;
            var totalTrip = tempList.total;

            var temp = new
            {
                data = data,
                draw = model.draw,
                recordsTotal = totalTrip,
                recordsFiltered = totalTrip,
                totTrip = totalTrip
            };

            var jsonResult = Json(temp, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult GetServicePO(int idvendor, DateTime date)
        {
            var temp = _bllTransportExecution.GetServicePO(idvendor, date);
            if (temp == null)
                return Json("null");
            return Json(temp);
        }

        [HttpPost]
        public ActionResult GetTransportExecutionAddNewList(TransportOrderInput input)
        {
            List<TransportExecutionAddNewDTO> _transportExecutionAddNew2 = new List<TransportExecutionAddNewDTO>();
            _transportExecutionAddNew2 = _bllTransportExecution.GetTransportationExecutionAddNew(input);


            List<TransportExecutionAddNewDTO> listNewTN = _bllTransportExecution.SaveDataTemp(_transportExecutionAddNew2, GetUserId());


            var checkIDs = listNewTN.Select(c => c.IDCheck.HasValue ? c.IDCheck.Value : -1).Distinct().ToList();
            checkIDs.Sort();

            List<List<TransportExecutionAddNewDTO>> _groups = new List<List<TransportExecutionAddNewDTO>>();
            foreach (var ci in checkIDs)
            {
                List<TransportExecutionAddNewDTO> _dtos = new List<TransportExecutionAddNewDTO>();
                var _rm = listNewTN.Where(c => c.IDCheck.HasValue && c.IDCheck == ci && c.OrderType == "Raw Material").ToList();
                var _nrm = listNewTN.Where(c => c.IDCheck.HasValue && c.IDCheck == ci && c.OrderType != "Raw Material").ToList();
                _dtos.AddRange(_rm);
                _dtos.AddRange(_nrm);
                _groups.Add(_dtos);
            }
            listNewTN.Clear();
            if (input.executionTypeFilter == "With TN")
            {
                var _wVendor = _groups.Where(c => c.Count > 0 && c.First().IDVendor.HasValue).ToList();
                var _woVendor = _groups.Where(c => c.Count > 0 && !c.First().IDVendor.HasValue).ToList();

                _groups.Clear();
                _groups.AddRange(_woVendor);
                _groups.AddRange(_wVendor);
            }
            foreach (var dtos in _groups)
            {
                foreach (var dto in dtos)
                {
                    listNewTN.Add(dto);
                }
            }

            var lstChkID = -1;
            var lstTOID = -1;
            foreach (var li in listNewTN)
            {
                if (li.IDCheck.HasValue && li.IDCheck.Value != lstChkID)
                {
                    li.IsHeader = true;
                    lstChkID = li.IDCheck.Value;
                }
                else
                {
                    li.IsHeader = false;
                }

                if (li.IDTransportOrder.HasValue && li.IDTransportOrder.Value != lstTOID && li.IDVendor != null)
                {
                    lstTOID = li.IDTransportOrder.Value;
                    li.IsTNHeader = true;
                }
                else
                {
                    li.IsTNHeader = false;
                }
            }
            //return Json(_transportExecutionAddNew);
            //var jsonResult = Json(_transportExecutionAddNew2, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(listNewTN, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult GetTotalVendorAddNew(TransportOrderInput input)
        {
            List<TransportExecutionVendorDTO> _transportExecutionVendor = new List<TransportExecutionVendorDTO>();
            _transportExecutionVendor = _bllTransportExecution.GetTotalVendorByTN(input);
            return Json(_transportExecutionVendor);
        }

        [HttpPost]
        public ActionResult GenerateTransportationNumber(string idStartLoc, string transMode, List<int> idcheckList, string tabExec)
        {
            //List<TransportExecutionAddNewDTO> listSave = _transportExecutionAddNew.Where(x => idcheckList.Contains(x.IDCheck.Value)).ToList();
            //List<TransportExecutionAddNewDTO> _transportExecutionAddNew2 = new List<TransportExecutionAddNewDTO>();
            //_transportExecutionAddNew2 = (List<TransportExecutionAddNewDTO>)System.Web.HttpContext.Current.Session[tabExec + GetUserId()];            
            //List<TransportExecutionAddNewDTO> listSave = _transportExecutionAddNew2.Where(x => idcheckList.Contains(x.IDCheck.Value)).ToList();
            //List<TransportExecutionAddNewDTO> listSave = (System.Web.HttpContext.Current.Session[tabExec + GetUserId()]).Where(x => idcheckList.Contains(x.IDCheck.Value)).ToList();
            List<TransportExecutionAddNewDTO> listNewTN = _bllTransportExecution.GetDataTemp(GetUserId());
            List<TransportExecutionAddNewDTO> listSave = listNewTN.Where(x => idcheckList.Contains(x.IDCheck.Value)).ToList();
            return Json(_bllTransportExecution.GenerateTransportationNumber(idStartLoc, transMode, GetUserId(), listSave));
        }

        public ActionResult GetTransportExecutionByID(int id)
        {
            List<TransportExecutionEditUpdatedDate> getListSession = (List<TransportExecutionEditUpdatedDate>)Session["dateEdit"];
            var temp = _bllTransportExecution.GetTransportExecutionByID(id);
            TransportExecutionEditUpdatedDate newSession = new TransportExecutionEditUpdatedDate();
            newSession.TransportNo = temp.TransportNo;
            newSession.UpdatedDate = temp.UpdatedDate;
            if (getListSession != null)
            {
                TransportExecutionEditUpdatedDate getSession = getListSession.Where(x => x.TransportNo == temp.TransportNo).FirstOrDefault();
                if (getSession == null)
                {
                    getListSession.Add(newSession);
                }
                else
                {
                    getListSession.Where(x => x.TransportNo == temp.TransportNo).Select(x =>
                    {
                        x.UpdatedDate = temp.UpdatedDate;
                        return x;
                    }).ToList();
                }
            }
            else
            {
                getListSession = new List<TransportExecutionEditUpdatedDate>();
                getListSession.Add(newSession);
            }
            Session["dateEdit"] = getListSession;

            var jsonRes = Json(temp, JsonRequestBehavior.AllowGet);
            jsonRes.MaxJsonLength = int.MaxValue;
            return jsonRes;
            //return Json(temp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetActiveFalse(List<int> filter)
        {
            _bllTransportExecution.SetActiveTransportExecution(filter, GetUserId());
            return Json("");
        }

        [HttpPost]
        public ActionResult TransportExecutionCalculateDistance(string vendorCategory, DateTime transDate, string transportMode, List<TransportRouteDTO> transRoute, string via)
        {
            //NOT USED
            //return Json(_bllTransportExecution.CalculateTransportExecutionDistance(vendorCategory, transDate, transportMode, transRoute, via));
            return Json(null);
        }

        [HttpPost]
        public ActionResult CalculateDistance(string vendorCategory, DateTime transDate, string transportMode, List<TransportRouteDTO> transRoute)
        {
            decimal result = 0;
            var sender = new TransportRouteDTO();
            var receiver = new TransportRouteDTO();
            List<TransportRouteDTO> _routes = null;
            if (transRoute != null)
            {
                result = _bllTransportExecution.CalculateDistance(vendorCategory, transDate, transportMode, transRoute);
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult SaveData(TransportExecutionDTO saveTransExe, List<TransportRouteDTO> saveTransRoute, List<TransportOrderDTO> saveTransOrder, TransportVesselMonitoringDTO saveTransVessel, TransportVendorChangeLogDTO saveVendorLog)
        {
            List<TransportExecutionEditUpdatedDate> getListSession = (List<TransportExecutionEditUpdatedDate>)Session["dateEdit"];
            TransportExecutionEditUpdatedDate getSession = getListSession.Where(x => x.TransportNo == saveTransExe.TransportNo).First();
            return Json(_bllTransportExecution.CheckSaveData(saveTransExe, saveTransRoute, saveTransOrder, saveTransVessel, saveVendorLog, GetUserId(), getSession.UpdatedDate));
        }

        [HttpPost]
        public ActionResult ConfirmSaveData(TransportExecutionDTO saveTransExe, List<TransportRouteDTO> saveTransRoute, List<TransportOrderDTO> saveTransOrder, TransportVesselMonitoringDTO saveTransVessel, TransportVendorChangeLogDTO saveVendorLog)
        {
            List<TransportExecutionEditUpdatedDate> getListSession = (List<TransportExecutionEditUpdatedDate>)Session["dateEdit"];
            TransportExecutionEditUpdatedDate getSession = getListSession.Where(x => x.TransportNo == saveTransExe.TransportNo).First();
            return Json(_bllTransportExecution.SaveData(saveTransExe, saveTransRoute, saveTransOrder, saveTransVessel, saveVendorLog, GetUserId(), getSession.UpdatedDate));
        }

        #region SUB EXPORT
        [HttpPost]
        public JsonResult ExportExecutionXls(TransportOrderInput filter)
        {
            ExcelPackage xls_file = new ExcelPackage();
            ExcelWorksheet xls_sheet;
            ExcelRange xls_range;

            string path = "";

            //if (tipe == "RD")
            //{
            //string[] colTitles = { "", "Transportation Number", "Order Number", "Zone", "Order Type", "Sender", "Receiver", "Shipment Date", "Ordered Vehicle Type", "Material Type", "Description", "Quantity", "UoM", "Order Category", "TN Creator", "Vendor", "SI Week", "SI Type", "SI Status", "Start Location", "Last Update" };
            //string[] colTitles = { "IsActive", "Transportation Number","TN Creator", "Vendor", "SI Week", "SI Type", "SI Status", "Start Location", "Last Update" };
            string[] colTitles = { "Transportation Number", "Transportation Status", "TN Creator", "Vendor", "SI Week", "SI Type", "SI Status", "Start Location", "Transportation Date", "Last Update" };

            xls_sheet = xls_file.Workbook.Worksheets.Add("Export");

            xls_sheet.Column(1).Width = 20;
            xls_sheet.Column(2).Width = 20;
            xls_sheet.Column(3).Width = 15;
            xls_sheet.Column(4).Width = 37;
            xls_sheet.Column(5).Width = 10;
            xls_sheet.Column(6).Width = 12;
            xls_sheet.Column(7).Width = 12;
            xls_sheet.Column(8).Width = 27;
            xls_sheet.Column(9).Width = 50;

            /*using (xls_range = xls_sheet.Cells[1, 1, 1, 9])
            {
                xls_range.Value = "Execution Raw Data";
                xls_range.Merge = true;
                xls_range.Style.Font.Size = 10;
                //xls_range.Style.Font.Bold = true;
                xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }*/

            for (int c = 1; c <= 10; c++)
            {
                //using (xls_range = xls_sheet.Cells[2, c])
                //{
                //    xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.White);
                //    if (c == 8) { xls_range.Style.Border.Right.Style = ExcelBorderStyle.None; }
                //}
                using (xls_range = xls_sheet.Cells[1, c])
                {
                    xls_range.Value = colTitles[c - 1];
                    xls_range.Style.Font.Size = 10;
                    //xls_range.Style.Font.Bold = true;
                    xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                    //xls_range.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.Black);
                }
            }

            //int r = 4;
            int r = 2;
            string IDTOD = "";

            if (GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA)))
                filter.IsRoleTransport = true;
            filter.UserRole = GetListUserRole().FirstOrDefault().RoleName;
            filter.userRegionList = GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList();

            var tempList = _bllTransportExecution.GetExecutionDataTable(filter, GetUserId());

            foreach (var datas in tempList.data)
            {
                var data = (TransportExecutionDataTableDTO) datas;
                string[] dtaRows = { data.TransportNo, data.TransportStatus, data.CreatedBy, data.VendorName, data.SIWeek.ToString(), data.SIType, data.SIStatus, data.StartLocation,
                    data.TransportDate.HasValue ? data.TransportDate.Value.ToString("dd-MMM-yyyy") : null, "by " + data.UpdatedBy + " on " +
                    (data.UpdatedDate.HasValue ? data.UpdatedDate.Value.ToString("dd-MMM-yyyy HH:mm") : "") };

                for (int c = 1; c <= 10; c++)
                {
                    using (xls_range = xls_sheet.Cells[r, c])
                    {
                        xls_range.Value = dtaRows[c - 1];
                        xls_range.Style.Font.Size = 10;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                }
                r++;
            }

            path = "TransportExecutionRawData" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            xls_file.SaveAs(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
            return Json(path);
        }
        #endregion SUB EXPORT

        #region SUB CUSTOM
        public JsonResult CustomExecutionXls(string[] colCaption, TransportOrderInput filter)
        {
            var fields = new string[] {
                "Transportation Number|TransportNo",
                "Transportation Status|TransportStatus",
                "Transportation Date|TransportDate",
                "Transportation Category|TransportCategory",
                "Actual Vehicle Type|ActualVehicleType",
                "Transportation Mode|TransportMode",
                "Vessel Name|TransportVesselCustomReport.VesselNameCustomReport",
                "ETD1|TransportVesselCustomReport.ETD1",
                "ETA1|TransportVesselCustomReport.ETA1",
                "Container Number|TransportVesselCustomReport.ContainerNoCustomReport",
                "Container Seal|TransportVesselCustomReport.ContainerSealCustomReport",
                "Vendor|VendorName",
                "SI Type|SIType",
                "SI Status|SIStatus",
                "SI Week|SIWeek",
                "Target of Arrival|TargetOfArrival",
                "Actual Arrive|ActualArrive",
                "Start Location|StartLocation",
                "Finish Location|FinishLocation",
                "Service PO Number|ServicePONo",
                "Service GR Number|SerivceGRNo",
                "Actual Cost Center|ActualCostCenterDisplay",
                "Order Number|STONo",
                "Material Type|MaterialType",
                "Material Description|Description",
                "Quantity|Qty",
                "UoM",
                "Sender",
                "Receiver",
                "Route|RouteNameCustomReport",
                "KM|KMOrder",
                "Order Category|OrderCategory",
                "Ordered Vehicle Type|VehicleType",
                "Ordered Cost Center|MasterCostCenter.CostCenter",
                "Police Reg Number|PoliceRegistrationNumber",
                "Driver 1|Driver1",
                "Driver 2|Driver2",
                "Co-Driver|CoDriver",
                "Total KM|TotalKM",
                "Total Box|TotalBox",
                "Additional Cost|AdditionalCost",
                "Via",
                "Month",
                "Year",
                "Created By|CreatedBy",
                "Created Date|CreatedDate",
                "Updated By|UpdatedBy",
                "Updated Date|UpdatedDate",
                "Is Active|IsActive",
                "Delivery Qty|DeliveryQty",
                "Total KM Rail|TotalKMRail",
                "Total KM Based|TotalKMBased",
                "Total KM Sea|TotalKMSea",
                "CFP Liter|CFPLiter",
                "CFPTKM  KGCO2|CFPTKM",
                "Average Load Factor|AVGLoadFactor",
                "Remarks",
                "Cost|BasedPrice"  ,
                "Discount Cost",
                "Total Cost",
                "Cost Center" };

            if (GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA)))
                filter.IsRoleTransport = true;
            filter.UserRole = GetListUserRole().FirstOrDefault().RoleName;
            filter.userRegionList = GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList();
            var dbResult = _bllTransportExecution.GetExecutionDataTableExportCustomReport(filter, GetUserId());
            //var dbResult = _bllTransportExecution.GetExecutionDataTable(filter, GetUserId()).Select(x => new TransportExecutionCustomExportDTO(x));
            //var data = new List<TransportExecutionDTO>();

            //// fill missing data
            //foreach (var dt in dbResult)
            //{
            //    var te = _bllTransportExecution.GetTransportExecutionByID(dt.IDTransportExecution);
            //    var vessel = _bllTransportExecution.GetTransportVessel(dt.IDTransportExecution);
            //    if (vessel != null)
            //    {
            //        dt.TransportVesselCustomReport = new TransportVesselCustomReport
            //        {
            //            ContainerNoCustomReport = vessel.ContainerNo,
            //            ContainerSealCustomReport = vessel.ContainerSeal,
            //            VesselNameCustomReport = vessel.VesselName,
            //            ETD1 = vessel.ETD1,
            //            ETA1 = vessel.ETA1
            //        };
            //    }

            //    if (te != null)
            //    {
            //        dt.TransportCategory = te.TransportCategory;
            //        dt.ActualVehicleType = te.ActualVehicleType;
            //        dt.TransportMode = te.TransportMode;
            //        dt.VendorName = te.MasterVendor == null ? "" : te.MasterVendor.VendorName;
            //        dt.ServicePONo = te.ServicePONo;
            //        dt.ServiceGRNo = te.ServiceGRNo;
            //        dt.ActualCostCenter = te.ActualCostCenter;
            //        dt.TotalKM = te.TotalKM;
            //        dt.TotalBox = te.TotalBox;
            //        dt.UpdatedDate = te.UpdatedDate;
            //        dt.Via = te.Via;
            //        dt.Month = te.Month;
            //        dt.Year = te.Year;
            //        dt.AdditionalCost = te.AdditionalCost;
            //        dt.ActualArrive = te.ActualArrive;
            //        dt.TargetOfArrival = te.TargetOfArrival;
            //        dt.PoliceRegistrationNumber = te.PoliceRegNo;
            //        dt.Driver1 = _bllDriver.GetNameById(te.IDDriver1);
            //        dt.Driver2 = _bllDriver.GetNameById(te.IDDriver2);
            //        dt.CoDriver = _bllDriver.GetNameById(te.IDCoDriver);
            //        dt.DeliveryQty = te.DeliveryQty;
            //        dt.TotalKMRail = te.TotalKMRail;
            //        dt.TotalKMSea = te.TotalKMSea;
            //        dt.TotalKMBased = te.TotalKMBased;
            //        dt.Remarks = te.Remarks;
            //        dt.CreatedDate = te.CreatedDate;

            //        var rte = new List<string>();
            //        foreach (var route in te.TransportRoutes)
            //        {
            //            var loc = _bllLoc.GetById(route.IDLocation);
            //            if (loc != null)
            //                rte.Add(loc.LocationName);
            //        }
            //        dt.RouteNameCustomReport = string.Join("-", rte);
            //    }

            //    /*
            //    if (dt.RouteCustomReport != null)
            //    {
            //        dt.Route = string.Join("-", dt.RouteCustomReport);
            //    }
            //    if (dt.RouteNameCustomReport != null)
            //    {
            //        dt.RouteNameCustomReport = dt.RouteNameCustomReport.Replace(",", "-");
            //    }
            //    */

            //    // select TO ->
            //    var tos = _bllTO.GetTransportOrderByIDTransportExecution(dt.IDTransportExecution);
            //    foreach (var to in tos)
            //    {
            //        var tods = _bllTO.GetTransportOrderDetails(to.IDTransportOrder);

            //        var sender = _bllLoc.Get(w => w.IDLocation == to.SenderIDLocation).FirstOrDefault();
            //        var receiv = _bllLoc.Get(w => w.IDLocation == to.ReceiverIDLocation).FirstOrDefault();
            //        var cc =
            //            to.IDCostCenter.HasValue ?
            //            _bllCC.GetALLMasterCostCenterAccount(new TOM.Master.Domain.Inputs.MasterCostCenterAccountInput { IDCostCenter = to.IDCostCenter.Value }).FirstOrDefault() : null;

            //        foreach (var tod in tods)
            //        {
            //            var ndto = new TransportExecutionDTO();
            //            MappingHelper.Map(dt, ndto);

            //            ndto.IDTransportOrder = to.IDTransportOrder;
            //            ndto.KMOrder = to.KM;
            //            ndto.STONo = to.STONo;
            //            ndto.OrderCategory = to.OrderCategory;
            //            ndto.VehicleType = to.VehicleType;
            //            ndto.Sender = sender != null ? sender.LocationName : null;
            //            ndto.Receiver = receiv != null ? receiv.LocationName : null;
            //            ndto.MasterCostCenter = cc;

            //            ndto.MaterialType = tod.MaterialType;
            //            ndto.Description = tod.Description;
            //            ndto.UoM = tod.UoM;
            //            ndto.Qty = tod.Qty;

            //            data.Add(ndto);
            //        }
            //    }
            //}

            try
            {
                using (var pkg = new ExcelExportHelper())
                {
                    foreach (var f in fields)
                    {
                        var spl = f.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        if (spl.Length == 1) pkg.Map(spl[0].Trim(), spl[0].Trim());
                        if (spl.Length == 2) pkg.Map(spl[0].Trim(), spl[1].Trim());
                    }

                    pkg.HeaderBackgroundColor = Color.LightGray;
                    pkg.HeaderForegroundColor = Color.Black;

                    pkg.SetHeaderDisplay(colCaption);

                    var ws = pkg.CreateWorksheet("Export");

                    pkg.WriteHeader(ws);

                    // write values
                    pkg.WriteRowsNested(ws, dbResult, null, (cell, hdr, obj) =>
                    {
                        if (obj is DateTime || obj is DateTime?)
                            cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                    });
                    pkg.AutoSizeColumns(ws);
                    var path = "TransportationExecution_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
                    return Json(path);
                }
            }
            catch { }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion SUB CUSTOM

        #region SUB SI
        public ActionResult GetVendorShip()
        {
            var getVendor = _bllTransportExecution.GetVendorSI(GetListUserRole());
            return Json(getVendor, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWeekShip(int year)
        {
            var getWeek = _bllTransportExecution.GetWeekMstGen(year);

            TOMContextDB ctx = new TOMContextDB();
            var mgw =
                year == DateTime.Today.Year ?
                ctx.MasterGenWeeks.Where(gw => gw.StartDate <= DateTime.Today && gw.EndDate >= DateTime.Today).FirstOrDefault() : null;

            return Json(
                new
                {
                    week = getWeek,
                    def = mgw == null ? -1 : mgw.Week
                }
                , JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportSI(string option, int fltrvn, int fltrwk, int fltryr, List<string> tncreator)
        {
            var IsRoleTransport = false;
            if (GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA)))
                IsRoleTransport = true;
            var UserRole = GetListUserRole().FirstOrDefault().RoleName;
            if (UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
                IsRoleTransport = true;
            List<string> userRegionList = new List<string>();
            userRegionList = GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList();

            ExcelPackage xls_file = new ExcelPackage();

            Dictionary<string, string> columns = new Dictionary<string, string>();
            columns.Add("Transportation Number", "TransportNo");
            columns.Add("Transportation Date", "TransportDate");
            var toBegin = columns.Count;
            //columns.Add("Order Number", "STONo");
            columns.Add("Sender Warehouse", "SenderLocationName");
            columns.Add("Receiver Warehouse", "ReceiverLocationName");
            columns.Add("Material Type", "MaterialType");
            columns.Add("Order Type", "OrderType");
            var toEnd = columns.Count;
            columns.Add("Via", "Via");
            columns.Add("Container No.", "ContainerNo");
            columns.Add("Vehicle Type", "VehicleType");
            columns.Add("SI Type", "SIType");
            columns.Add("SI Status", "SIStatus");
            columns.Add("Target of Arrival", "TargetOfArrival");
            columns.Add("Police Reg Number", "PoliceRegNo");
            columns.Add("Driver 1", "Driver1");
            columns.Add("Driver 2", "Driver2");
            columns.Add("Co Driver", "CoDriver");
            columns.Add("Unit Position When Sending SI", "_EstArrivalDate");
            columns.Add("Unit Status", "_OrderStatus");
            columns.Add("Estimate Arrival", "_EstArrivalDate");
            columns.Add("Remarks", "_Remarks");

            var vendor = _bllTransportExecution.GetVendor(fltrvn);
            //var vendorZone = _bllTransportExecution.GetVendorByZone(fltrvn);
            //if (option == "mail" && vendorZone == null)
            //{
            //    return Json(false, JsonRequestBehavior.AllowGet);
            //}
            var dbresGenWeek = _bllTransportExecution.GetMasterGenWeek(fltrwk, fltryr);
            if (dbresGenWeek != null)
            {
                var tabtglawal = dbresGenWeek.StartDate;
                var tabtglakhir = dbresGenWeek.EndDate;

                for (DateTime curDate = tabtglawal.Value.Date; curDate <= tabtglakhir.Value.Date; curDate += new TimeSpan(1, 0, 0, 0))
                {
                    var sheet = xls_file.Workbook.Worksheets.Add(curDate.ToString("dd-MMM-yyyy"));

                    #region Generate Header
                    sheet.Cells[1, 1].Value = "PT.HM Sampoerna Tbk";
                    sheet.Cells[1, 1].Style.Font.Bold = true;
                    sheet.Cells[1, 1].Style.Font.Size = 13;

                    sheet.Cells[2, 1].Value = "Operation Transport";

                    sheet.Cells[3, 1].Value = "Shipping Instruction";

                    sheet.Cells[4, 1].Value = "Week " + dbresGenWeek.Week + " ("
                        + dbresGenWeek.StartDate.Value.ToString("dd-MMM-yyyy") + " until "
                        + dbresGenWeek.EndDate.Value.ToString("dd-MMM-yyyy")
                        + ")";

                    sheet.Cells[6, 1].Value = "Vendor";
                    sheet.Cells[6, 1].Style.Font.Bold = true;
                    sheet.Cells[6, 2].Value = vendor.VendorName;

                    sheet.Cells[7, 1].Value = "Alamat";
                    sheet.Cells[7, 1].Style.Font.Bold = true;
                    sheet.Cells[7, 2].Value = vendor != null ? vendor.VendorAddress + ", " + vendor.VendorCity : null;

                    sheet.Cells[1, 1, 7, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[2, 1, 7, 2].Style.Font.Size = 11;

                    var colTitles = columns.Keys.ToArray();

                    for (var i = 0; i < colTitles.Length; i++)
                    {
                        sheet.Cells[10, i + 1].Value = colTitles[i];
                        sheet.Cells[10, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        sheet.Cells[10, i + 1].Style.Font.Bold = true;
                    }
                    #endregion

                    #region Generate Data
                    var data = _bllTransportExecution.GetExportSI(curDate, vendor.IDVendor, IsRoleTransport, userRegionList, tncreator);

                    var row = 11;
                    var typ = typeof(TransportExecutionExportSIDTO);
                    foreach (var d in data)
                    {
                        // write Execution data
                        for (var ci = 0; ci < columns.Count; ci++)
                        {
                            if (ci >= toBegin && ci < toEnd) continue;
                            var cl = columns.ElementAt(ci);
                            var pi = typ.GetProperty(cl.Value);

                            if (pi != null)
                            {
                                if (pi.CanRead)
                                {
                                    sheet.Cells[row, ci + 1].Value = pi.GetValue(d);
                                }
                            }
                        }


                        var tobySendRecv = d.TransportOrders.SelectMany(dto => dto.TransportOrderDetails.Select(
                            tod => new
                            {
                                SenderLocationName = dto.SenderLocationName,
                                SenderIDLocation = dto.SenderIDLocation,
                                ReceiverLocationName = dto.ReceiverLocationName,
                                ReceiverIDLocation = dto.ReceiverIDLocation,
                                OrderType = dto.OrderType,
                                MaterialType = tod.MaterialType,
                                Grouper = dto.SenderIDLocation + "!$!" + dto.ReceiverIDLocation + "!$!" + tod.MaterialType
                            }
                            )).GroupBy(k => k.Grouper).Select(gu => gu.FirstOrDefault());

                        foreach (var to in tobySendRecv)
                        {
                            var tto = to.GetType();
                            for (var ci = toBegin; ci < toEnd; ci++)
                            {
                                var cl = columns.ElementAt(ci);
                                var pi = tto.GetProperty(cl.Value);
                                if (pi != null)
                                {
                                    if (pi.CanRead)
                                    {
                                        sheet.Cells[row, ci + 1].Value = pi.GetValue(to);
                                    }
                                }
                            }
                            row++;
                        }

                        if (d.TransportOrders.Count == 0)
                            row++;
                    }
                    #endregion

                    // format cells
                    for (var i = 1; i <= columns.Count; i++)
                    {
                        sheet.Column(i).AutoFit();
                        var hdr = columns.Values.ElementAt(i - 1);
                        if (hdr == "TransportDate")
                            sheet.Column(i).Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (hdr == "TargetOfArrival")
                            sheet.Column(i).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                        if (hdr == "EstArrivalDate")
                            sheet.Column(i).Style.Numberformat.Format = "dd-MMM-yyyy";
                    }
                }


                /*
                for (int t = 0; t < 7; t++)
                {
                    xls_sheet = xls_file.Workbook.Worksheets.Add(currDate.ToString("dd-MMM-yyyy"));
                    
                    //List<TransportExecutionDTO> dbResult1 = _transportExecutionBll.GetDataTEByVendorTransportDate(currDate, vendor.IDVendor, IsRoleTransport, userRegionList);
                    var dbResult1 = _bllTransportExecution.GetDataTEByVendorTransportDate(currDate, vendor.IDVendor, IsRoleTransport, userRegionList, startloc);
                    //var totRow = dbResult1.Count();
                    if (dbResult1 != null)
                    {
                        int row = 10;
                        var prevTN = "";
                        var prevSender = "";
                        var prevReceiver = "";
                        var prevVehicle = "";
                        var prevMaterial = "";
                        var prevSiType = "";
                        var prevSiStatus = "";
                        foreach (var data in dbResult1)
                        {
                            //string[] dtaRows = { "", data.TransportDate.ToString("dd-MMM-yyyy"), data.SIWeek.ToString(), data.VendorName, data.VendorAddress + ", " + data.VendorCity, data.VendorEmail, data.VendorRegion };
                            string[] dtaRowsd = { "", data.TransportNo, data.TransportDate.ToString("dd-MMM-yyyy"), data.Sender, data.Receiver, data.VehicleType, data.MaterialType, data.SIType, data.SIStatus, data.TargetOfArrival.ToString(), data.PoliceRegNo, data.IDDriver1, data.IDDriver2, data.IDCoDriver, data.Remarks };

                            if (prevTN != dtaRowsd[1])
                            {
                                xls_sheet.Cells[row, 1].Value = dtaRowsd[1];
                            }
                            else
                                xls_sheet.Cells[row, 1].Value = "";

                            xls_sheet.Cells[row, 2].Value = dtaRowsd[2];
                            xls_sheet.Cells[row, 3].Value = dtaRowsd[3];
                            xls_sheet.Cells[row, 4].Value = dtaRowsd[4];
                            xls_sheet.Cells[row, 5].Value = prevTN != dtaRowsd[1] ? dtaRowsd[5] : "";
                            xls_sheet.Cells[row, 6].Value = dtaRowsd[6];

                            if (prevTN != dtaRowsd[1])
                            {
                                xls_sheet.Cells[row, 7].Value = dtaRowsd[7];
                            }
                            else
                                xls_sheet.Cells[row, 7].Value = "";

                            if (prevTN != dtaRowsd[1])
                            {
                                prevTN = dtaRowsd[1];
                                xls_sheet.Cells[row, 8].Value = dtaRowsd[8];
                            }
                            else
                                xls_sheet.Cells[row, 8].Value = "";

                            xls_sheet.Cells[row, 9].Value = dtaRowsd[9];
                            var list = xls_sheet.Cells[row, 13].DataValidation.AddListDataValidation();
                            list.Formula.ExcelFormula = "='Data Source'!$A$2:$A$10000";
                            list.AllowBlank = false;
                            xls_sheet.Cells[row, 13].Value = dtaRowsd[10];
                            for (int i = 14; i <= 15; i++)
                            {
                                var list2 = xls_sheet.Cells[row, i].DataValidation.AddListDataValidation();
                                list2.Formula.ExcelFormula = "='Data Source'!$B$2:$B$10000";
                                list2.AllowBlank = false;
                                xls_sheet.Cells[row, i].Value = dtaRowsd[i - 3];
                            }
                            var list3 = xls_sheet.Cells[row, 16].DataValidation.AddListDataValidation();
                            list3.Formula.ExcelFormula = "='Data Source'!$C$2:$C$10000";
                            list3.AllowBlank = false;
                            xls_sheet.Cells[row, 16].Value = dtaRowsd[13];
                            xls_sheet.Cells[row, 17].Value = dtaRowsd[14];
                            row++;

                        }
                    }

                    for (int c = 1; c <= 17; c++)
                    {
                        using (xls_range = xls_sheet.Cells[9, c])
                        {
                            xls_range.Value = colTitles[c];
                            xls_range.Style.Font.Size = 11;
                            xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            xls_range.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                        if (colTitles[c].Length < 10) { xls_sheet.Column(c).Width = 10; }
                        else { xls_sheet.Column(c).Width = colTitles[c].Length + 2; }
                    }
                    // get data TE

                    currDate = currDate.AddDays(1);
                }
            */
            }

            #region Sheet Data Source
            string[] colSources = { "", "Police Reg Number", "Driver", "Co-Driver" };
            var xls_sheet = xls_file.Workbook.Worksheets.Add("Data Source");
            for (int c = 1; c <= 3; c++)
            {
                using (var xls_range = xls_sheet.Cells[1, c])
                {
                    xls_range.Value = colSources[c];
                    xls_range.Style.Font.Size = 11;
                    xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                if (colSources[c].Length < 10) { xls_sheet.Column(c).Width = 20; }
                else { xls_sheet.Column(c).Width = colSources[c].Length + 2; }
            }
            int tr;
            tr = 2;
            //var dbResultVD = _transportVehicleDataRepo.GetALLTransportVehicleDataActiveList().Where(o => o.IDVendor == fltrvn).OrderBy(o => o.IDPoliceRegNumber);
            var dbResultVD = _transportVehicleDataRepo.GetALLTransportVehicleDataActiveList().GroupBy(x => x.IDPoliceRegNumber).Select(x => x.First()).Where(x => x.IDVendor == fltrvn).OrderBy(o => o.IDPoliceRegNumber).ToList();
            if (dbResultVD != null)
            {
                foreach (var data in dbResultVD)
                {
                    string[] dtaRows = { "", data.IDPoliceRegNumber };
                    using (var xls_range = xls_sheet.Cells[tr, 1])
                    {
                        xls_range.Value = dtaRows[1];
                        xls_range.Style.Font.Size = 11;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    tr++;
                }
            }
            tr = 2;
            //var dbResultTD = _transportDriverManagementRepo.GetALLTransportDriverManagementActiveList().OrderBy(o => o.Name).OrderBy(o => o.ID);
            var dbResultTD = _transportDriverManagementRepo.GetALLTransportDriverManagementActiveList().Where(o => o.IDVendor == fltrvn && o.RoleDriver == "Driver").OrderBy(o => o.Name);
            //var dbResultTD = _transportDriverManagementRepo.GetALLTransportDriverManagementActiveList().GroupBy(x => x.ID).Select(x => x.First()).Where(x => x.IDVendor == fltrvn && x.RoleDriver == "Driver").OrderBy(x => x.Name).ToList();
            var dbResultCoTD = _transportDriverManagementRepo.GetALLTransportDriverManagementActiveList().Where(o => o.IDVendor == fltrvn && o.RoleDriver == "Co-Driver").OrderBy(o => o.Name);
            //var dbResultCoTD = _transportDriverManagementRepo.GetALLTransportDriverManagementActiveList().GroupBy(x => x.ID).Select(x => x.First()).Where(x => x.IDVendor == fltrvn && x.RoleDriver == "CO-Driver").OrderBy(x => x.Name).ToList();
            if (dbResultTD != null)
            {
                foreach (var data in dbResultTD)
                {
                    string[] dtaRows = { "", data.Name + "-" + data.ID };
                    using (var xls_range = xls_sheet.Cells[tr, 2])
                    {
                        xls_range.Value = dtaRows[1];
                        xls_range.Style.Font.Size = 11;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    tr++;
                }
            }
            tr = 2;
            if (dbResultCoTD != null)
            {
                foreach (var data in dbResultCoTD)
                {
                    string[] dtaRows = { "", data.Name + "-" + data.ID };
                    using (var xls_range = xls_sheet.Cells[tr, 3])
                    {
                        xls_range.Value = dtaRows[1];
                        xls_range.Style.Font.Size = 11;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    tr++;
                }
            }
            #endregion

            #region Save Excel
            string path = "TransportationExecution-SI" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            var PathFile = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path;
            xls_file.SaveAs(new System.IO.FileInfo(PathFile));
            #endregion

            if (option == "mail")
            {
                if (vendor != null)
                {
                    string baseUrl = ConfigurationManager.AppSettings["BaseDirectory"] + "\\Assets\\Download\\";

                    var mailto = vendor.VendorEmail;
                    var name = vendor.VendorName;
                    var reg = vendor.VendorCity;
                    var tglAwal = dbresGenWeek.StartDate.HasValue ? dbresGenWeek.StartDate.Value.ToString("dd MMMM yyyy") : null;
                    var tglAkhir = dbresGenWeek.EndDate.HasValue ? dbresGenWeek.EndDate.Value.ToString("dd MMMM yyyy") : null;
                    //var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
                    var sender = GetUserName();
                    //bool sendmail = false;
                    //sendmail = _bllTransportExecution.SendEmailSI(mailto, name, reg, fltrwk.ToString(), tabtglawal.Value.ToString("dd MMMM yyyy"), tabtglakhir.Value.ToString("dd MMMM yyyy"), session.Username, AppDomain.CurrentDomain.BaseDirectory + "Assets\\Download\\" + path);
                    //sendmail = _bllTransportExecution.SendEmailSI(mailto, name, reg, fltrwk.ToString(), tabtglawal.Value.ToString("dd MMMM yyyy"), tabtglakhir.Value.ToString("dd MMMM yyyy"), session.Username, baseUrl + path);

                    string MailFrom = ConfigurationManager.AppSettings["MailFrom"];
                    MailMessage msg = new MailMessage();
                    msg.IsBodyHtml = true;
                    msg.From = new MailAddress(MailFrom);
                    //msg.To.Add("andre.yusdianto@contracted.sampoerna.com");
                    if (!string.IsNullOrWhiteSpace(mailto))
                        msg.To.Add(mailto.Replace(";", ","));
                    if (vendor != null && !string.IsNullOrWhiteSpace(vendor.CC))
                        msg.CC.Add(vendor.CC.Replace(";", ","));
                    msg.Subject = "TOM Email Shipping Instruction";
                    var msgBody = "Shipping Instruction " + name + "-" + reg + " Week " + fltrwk.ToString() + "<br><br>Hi,<br><br>We would like to inform you that,<br>Shipping Instruction Week " + fltrwk.ToString() + " (" + tglAwal + " - " + tglAkhir + ") have been created and send by<br><b><font color=#000><u> " + sender + " </u></font></b><br>Please give feedback using the same file as attached<br><br>Thank You";
                    msg.Body = msgBody;
                    //if (System.IO.File.Exists(baseUrl + path))
                    if (System.IO.File.Exists(PathFile))
                    {
                        Attachment attachment = new Attachment(baseUrl + path);
                        msg.Attachments.Add(attachment);
                    }

                    if (msg.To.Count > 0)
                    {
                        SmtpClient SmtpMail = new SmtpClient();
                        SmtpMail.Host = ConfigurationManager.AppSettings["SMTPMailHost"];
                        SmtpMail.Port = int.Parse(ConfigurationManager.AppSettings["SMTPMailPort"]);
                        SmtpMail.Send(msg);
                    }
                }
            }
            //onlyxls:
            return Json(path, JsonRequestBehavior.AllowGet);
        }
        #endregion SUB SI

        public ActionResult UploadExecution()
        {
            // Mapping Setup
            var eih = new ExcelImportHelper();
            eih.Map("Transportation Number", "TransportNo");
            eih.Map("SI Type", "SIType");
            eih.Map("Target of Arrival", "TargetOfArrival");
            eih.Map("SI Status", "SIStatus");
            eih.Map("New Vendor", "NewVendorName");
            eih.Map("New SI Type", "NewSIType");
            eih.Map("New Target of Arrival", "NewTargetOfArrival");
            eih.Map("New Transportation Date", "NewTransportDate");
            eih.Map("Vessel Name", "VesselName");
            eih.Map("Container Number", "ContainerNumber");
            eih.Map("Seal Number", "SealNumber");
            eih.Map("Actual Arrive", "ActualArrive");
            eih.Map("Service PO Number", "ServicePONumber");
            eih.Map("Service GR Number", "ServiceGRNumber");
            eih.Map("Police Reg Number", "PoliceRegNumber");
            eih.Map("Driver 1", "Driver1");
            eih.Map("Driver 2", "Driver2");
            eih.Map("Co-Driver", "CoDriver");
            eih.Map("Via", "Via");

            var si_type = new List<string>();
            var si_status = new List<string>();
            var vessel = new List<string>();
            var vendor = new List<string>();

            var ds = ExcelImportHelper.ImportExcelAsDataSet(Request.Files.Get(0).InputStream, Request.Files.Get(0).FileName.ToLower().EndsWith(".xlsx"));
            var dataCol = new Dictionary<string, List<ExcelImportResult<TransportExecutionImportInput>>>();

            List<DataTable> _dt = new List<DataTable>(ds.Tables.Cast<DataTable>());
            if (!_dt.Where(_ => _.TableName == "Via").Any())
            {
                si_type = _bllList.GetMasterListByFieldName("SIType");
                si_status = _bllList.GetMasterListByFieldName("SIStatus");
                vessel = _bllList.GetMasterListByFieldName("VesselName");
                vendor = _bllVendor.GetALLMasterVendors().Where(c => c.ParentVendor == null).Select(c => c.VendorName).ToList();
            }

            foreach (DataTable table in ds.Tables)
            {
                if (table.TableName == "SI")
                {
                    var col = new List<ExcelImportResult<TransportExecutionImportInput>>();
                    dataCol.Add(table.TableName, col);

                    var excelres = eih.Deserialize<TransportExecutionImportInput>(table, c =>
                    {
                        // fill table name
                        c.DataType = table.TableName;

                        // required, no matter what
                        ExcelImportHelper.CheckHasValue(c.TransportNo, "Transportation Number");
                        // Does TN exists?
                        var tex = _bllTransportExecution.GetTransportNo(c.TransportNo);
                        if (tex == null)
                            throw new RowValidationByFieldException("Transportation Number", c.TransportNo);

                        // SI Status: Must exist in master list, vendor must be filled, if unfulfilled and new vendor is empty then error
                        if (!string.IsNullOrEmpty(c.SIStatus))
                            ExcelImportHelper.CheckExistsIn(c.SIStatus, si_status, "SI Status");

                        if (c.SIStatus == EnumHelper.GetDescription(Enums.SIStatus.Unfullfill))
                        {
                            ExcelImportHelper.CheckExistsIn(c.NewVendorName, vendor, "New Vendor");

                            if (!string.IsNullOrEmpty(c.NewSIType))
                                ExcelImportHelper.CheckExistsIn(c.NewSIType, si_type, "New SI Type");

                            if (c.NewTargetOfArrival != null)
                                ExcelImportHelper.CheckHasValue(c.NewTargetOfArrival, "New Target of Arrival");

                            if (c.NewTransportDate != null)
                                ExcelImportHelper.CheckHasValue(c.NewTransportDate, "New Transportation Date");
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(c.NewSIType))
                                throw new Exception("New SI Type should not be filled when status is not Unfulfill");
                            if (!string.IsNullOrWhiteSpace(c.NewVendorName))
                                throw new Exception("New Vendor should not be filled when status is not Unfulfill");
                            if (c.NewTargetOfArrival.HasValue && c.NewTargetOfArrival.Value != DateTime.MinValue)
                                throw new Exception("New Target of Arrival should not be filled when status is not Unfulfill");
                            if (c.NewTransportDate.HasValue && c.NewTransportDate.Value != DateTime.MinValue)
                                throw new Exception("New Transport Date should not be filled when status is not Unfulfill");
                        }

                        return c;
                    });
                    col.AddRange(excelres);
                }
                else if (table.TableName == "TN")
                {
                    var col = new List<ExcelImportResult<TransportExecutionImportInput>>();
                    dataCol.Add(table.TableName, col);

                    var excelres = eih.Deserialize<TransportExecutionImportInput>(table, c =>
                    {
                        // fill table name
                        c.DataType = table.TableName;

                        // required, no matter what
                        ExcelImportHelper.CheckHasValue(c.TransportNo, "Transportation Number");
                        // Does TN exists?
                        var tex = _bllTransportExecution.GetTransportNo(c.TransportNo);
                        if (tex == null)
                            throw new RowValidationByFieldException("Transportation Number", c.TransportNo);

                        if (!string.IsNullOrWhiteSpace(c.VesselName))
                            ExcelImportHelper.CheckExistsIn(c.VesselName, vessel, "Vessel Name");

                        if (c.ActualArrive.HasValue)
                            ExcelImportHelper.CheckHasValue(c.ActualArrive.Value, "Actual Arrive");

                        int IDVendor = tex.IDVendor.HasValue ? tex.IDVendor.Value : 0;
                        if (!string.IsNullOrWhiteSpace(c.PoliceRegNumber))
                        {
                            var dto = _bllVehicle.GetTransportVehicleDatas(new TransportVehicleDataInput() { IDPoliceRegNumber = c.PoliceRegNumber, IsActive = true }).OrderBy(w => w.STNKValidityPeriod).Reverse().FirstOrDefault();
                            if (dto == null)
                                throw new RowValidationByFieldException("Police Reg Number", c.PoliceRegNumber);
                            if (dto.IDVendor != IDVendor)
                                throw new RowValidationByFieldException(null, c.PoliceRegNumber, " - This vehicle belongs to another vendor.");
                            if (dto.STNKValidityPeriod <= tex.TransportDate)
                                throw new RowValidationByFieldException(null, c.PoliceRegNumber, " - The vehicle reg number will expire when the transport started.");
                        }

                        if (!string.IsNullOrWhiteSpace(c.Driver1))
                        {
                            var spl = c.Driver1.Split('-');
                            if (spl.Length == 2)
                            {
                                var dto = _bllDriver.GetTransportDriverManagements(new TransportDriverManagementInput() { Name = spl[0], ID = spl[1], IsActive = true, RoleDriver = "Driver" }).FirstOrDefault();
                                if (dto == null)
                                    throw new RowValidationByFieldException("Driver 1", c.Driver1);
                                if (dto.IDVendor != IDVendor)
                                    throw new RowValidationByFieldException(null, c.Driver1, " - This driver belongs to another vendor.");
                                c.Driver1 = spl[1];
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(c.Driver2))
                        {
                            var spl = c.Driver2.Split('-');
                            if (spl.Length == 2)
                            {
                                var dto = _bllDriver.GetTransportDriverManagements(new TransportDriverManagementInput() { Name = spl[0], ID = spl[1], IsActive = true, RoleDriver = "Driver" }).FirstOrDefault();
                                if (dto == null)
                                    throw new RowValidationByFieldException("Driver 2", c.Driver2);
                                if (dto.IDVendor != IDVendor)
                                    throw new RowValidationByFieldException(null, c.Driver2, " - This driver belongs to another vendor.");
                                c.Driver2 = spl[1];
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(c.CoDriver))
                        {
                            var spl = c.CoDriver.Split('-');
                            if (spl.Length == 2)
                            {
                                var dto = _bllDriver.GetTransportDriverManagements(new TransportDriverManagementInput() { Name = spl[0], ID = spl[1], IsActive = true, RoleDriver = "Co-Driver" }).FirstOrDefault();
                                if (dto == null)
                                    throw new RowValidationByFieldException("CoDriver", c.CoDriver);
                                if (dto.IDVendor != IDVendor)
                                    throw new RowValidationByFieldException(null, c.CoDriver, " - This co-driver belongs to another vendor.");
                                c.CoDriver = spl[1];
                            }
                        }

                        return c;
                    });
                    col.AddRange(excelres);
                }
                else if (table.TableName == "Via")
                {
                    var col = new List<ExcelImportResult<TransportExecutionImportInput>>();
                    dataCol.Add(table.TableName, col);

                    var excelres = eih.Deserialize<TransportExecutionImportInput>(table, c =>
                    {
                        // fill table name
                        c.DataType = table.TableName;

                        // required, no matter what
                        ExcelImportHelper.CheckHasValue(c.TransportNo, "Transportation Number");
                        // Does TN exists?
                        var tex = _bllTransportExecution.GetTransportNo(c.TransportNo);
                        if (tex == null)
                            throw new RowValidationByFieldException("Transportation Number", c.TransportNo);

                        if (tex.TransportMode.ToUpper() == "TRUCK")
                        {
                            throw new RowValidationByFieldException("Transportation Number", c.TransportNo, "for Truck");
                        }

                        if (String.IsNullOrWhiteSpace(c.Via))
                            throw new RowValidationByFieldException("Via", c.Via);

                        return c;
                    });
                    col.AddRange(excelres);
                }
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

            if (dataCol.Count <= 0)
                errmes = "&bull; No data found<br />";

            if (errmes != "")
                return Json(new NonQueryResult(false, errmes));

            var imported = new List<IEnumerable<TransportExecutionImportInput>>();
            foreach (var kv in dataCol)
            {
                var data = kv.Value.Where(c => c.Valid).Select(c => c.Result);
                imported.Add(data);
            }
            _bllTransportExecution.UploadExecution(imported, GetUserId());
            return Json(new NonQueryResult(true));

            #region Old Logic
            List<string> listError = new List<string>();
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase fileContent = Request.Files.Get(0);
                    listError = _bllTransportExecution.UploadExecution(fileContent, GetUserId());
                }
            }
            catch (Exception e)
            {
                return Json(e);
            }
            return Json(listError);
            #endregion
        }

        public JsonResult PrintExecution(string strUrl, List<int> idTE, string typePrint)
        {
            #region Rotativa
            /*
            List<TransportExecutionPrintFileViewModel> listResult = new List<TransportExecutionPrintFileViewModel>();
            for (int i = 0; i < idTE.Count; i++)
            {
                TransportExecutionPrintFileViewModel result = new TransportExecutionPrintFileViewModel();
                string Filename = "";
                string path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/";
                if (typePrint == "delivery")
                    Filename = "Transportation Delivery - " + (DateTime.Now.ToString("yyyyMMddhhmmss")) + "-" + idTE[i] + ".pdf";
                else
                    Filename = "IPB-" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".pdf";
                var idTN = idTE[i];

                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                string url = "/PrintDocument/PrintTransportExecution?id=" + idTN + "&uName=" + HttpUtility.UrlEncode(GetUserName()) + "&typePrint=" + typePrint;
                var actionResult = new UrlAsPdf(url);
                actionResult.RotativaOptions.PageWidth = 1024;
                var byteArray = actionResult.BuildPdf(ControllerContext);
                var fileStream = new FileStream(path + Filename, FileMode.Create, FileAccess.Write);
                fileStream.Write(byteArray, 0, byteArray.Length);
                fileStream.Close();

                Guid guid = Guid.NewGuid();
                result.FileName = Filename;
                result.Uid = guid.ToString();
                listResult.Add(result);
            }
            */
            #endregion

            #region iTextSharp
           /* string idx = "";
            for (int i=0; i < idTE.Count; i++)
            {
                idx += idTE[i];
                if (i != (idTE.Count - 1))
                    idx += ",";
            }
            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc;
            converter.Options.PdfPageSize = PdfPageSize.A4;
            Guid guid = Guid.NewGuid();
            var result = new
            {
                FileName = "",
                Uid = ""
            };
            string FileName = "";
            string username = GetUserName();
            string path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/";
            if (typePrint == "delivery")
                FileName = "DeliveryNote-" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".pdf"; 
            else            
                FileName = "IPB-" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".pdf"; 
                        
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";            
            string url = baseUrl + "/PrintDocument/PrintTransportExecution?id=" + idx + "&uName=" + username + "&typePrint=" + typePrint;
            doc = converter.ConvertUrl(url);
            doc.Save(path + FileName);

            //close pdf document 
            doc.Close();
            result = new
            {
                FileName = FileName,
                Uid = guid.ToString()
            };
            return Json(result, JsonRequestBehavior.AllowGet);*/
            #endregion

            #region WkPDF
            /*List<TransportExecutionPrintFileViewModel> listResult = new List<TransportExecutionPrintFileViewModel>();
            for (int i = 0; i < idTE.Count; i++)
            {
                TransportExecutionPrintFileViewModel result = new TransportExecutionPrintFileViewModel();
                var idTN = idTE[i];
                var username = GetUserName();

                string Filename = "";
                string path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/";
                if (typePrint == "delivery")
                    Filename = "Transportation Delivery - " + (DateTime.Now.ToString("yyyyMMddhhmmss")) + "-" + idTE[i] + ".pdf";
                else
                    Filename = "IPB-" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".pdf";

                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                string url = baseUrl + "/PrintDocument/PrintTransportExecution?id=" + idTN + "&uName=" + username + "&typePrint=" + typePrint;
                Guid guid = Guid.NewGuid();

                var converter = new NReco.PdfGenerator.HtmlToPdfConverter();
                converter.CustomWkHtmlArgs = String.Format("--disable-smart-shrinking --viewport-size 1280x1024 --username s-idtomdev --password D3vT0M1@#");
                converter.Size = NReco.PdfGenerator.PageSize.A4;
                converter.Zoom = 0.76f;
                converter.Margins.Bottom = 10;
                converter.Margins.Top = 10;
                converter.Margins.Left = 0;
                converter.Margins.Right = 0;

                byte[] filebyte = converter.GeneratePdfFromFile(url, "");

                var fileStream = new FileStream(path + Filename, FileMode.Create, FileAccess.Write);
                fileStream.Write(filebyte, 0, filebyte.Length);
                fileStream.Close();

                result.FileName = Filename;
                result.Uid = guid.ToString();
                listResult.Add(result);
            }
            */
            #endregion
            #region SelectPdf
            
            List<TransportExecutionPrintFileViewModel> listResult = new List<TransportExecutionPrintFileViewModel>();
            for (int i = 0; i < idTE.Count; i++)
            {
                TransportExecutionPrintFileViewModel result = new TransportExecutionPrintFileViewModel();
                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc;
                converter.Options.PdfPageSize = PdfPageSize.A4;
                var idTN = idTE[i];
                string username = GetUserName();
                var dto = _bllTransportExecution.PrintDocument(idTN, username);
                var gtps = dto.TotalGatePass > 24 ? (int)Math.Ceiling((dto.TotalGatePass - 24) / 30.0) + 1 : 1;
                converter.Options.WebPageHeight = (dto.TotalPage + gtps ) * 1080;                
                Guid guid = Guid.NewGuid();
                string FileName = "";
                string path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/";
                if (typePrint == "delivery")
                    FileName = "Transportation Delivery - " + (DateTime.Now.ToString("yyyyMMddhhmmss")) + "-" + idTE[i] + ".pdf";
                else
                    FileName = "IPB-" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".pdf";

                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                string url = baseUrl + "/PrintDocument/PrintTransportExecution?id=" + idTN + "&uName=" + username + "&typePrint=" + typePrint;
                
                doc = converter.ConvertUrl(url);
                doc.Save(path + FileName);
                
                doc.Close();
                result.FileName = FileName;
                result.Uid = guid.ToString();
                listResult.Add(result);
            }
            
            #endregion
            return Json(listResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendor(int IDTE)
        {
            List<MasterVendor> listVendor = new List<MasterVendor>();
            List<MasterVendorTOMDTO> listVendorConfig = new List<MasterVendorTOMDTO>();

            //List<SelectListItem> tempMasterVendorList = _transportExecutionBll.GetVendorList(GetListUserRole()).Select(a => new SelectListItem { Text = a.VendorName, Value = a.IDVendor.ToString() }).ToList();
            // get list vendor from Master Configuration
            listVendorConfig = _bllTransportExecution.GetVendorSuggestionList(GetListUserRole()).Select(a => new MasterVendorTOMDTO { VendorName = a.VendorName, IDVendor = a.IDVendor }).ToList();
            if (listVendorConfig.Count > 0)
            {
                return Json(listVendorConfig, JsonRequestBehavior.AllowGet);
            }
            else
            {
                listVendor = _bllTransportExecution.GetVendorBySuggestionList(IDTE);
                return Json(listVendor, JsonRequestBehavior.AllowGet);
            }
            //listVendor = _transportExecutionBll.GetVendorBySuggestionList(IDTE, GetListUserRole());            
        }

        public ActionResult GetDocument(string strUrl, string id, string typePrint)
        {
            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc;
            converter.Options.PdfPageSize = PdfPageSize.A4;
            Guid guid = Guid.NewGuid();
            var result = new
            {
                FileName = "",
                Uid = ""
            };
            string FileName = "";
            string username = GetUserName();
            string path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/";
            if (typePrint == "delivery")
                FileName = "DeliveryNote" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".pdf";
            else
                FileName = "IPB" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".pdf"; ;

            string url = strUrl + "/PrintDocument/PrintTransportExecution?id=" + id + "&uName=" + username + "&typePrint=" + typePrint;
            doc = converter.ConvertUrl(url);
            doc.Save(path + FileName);

            //close pdf document 
            doc.Close();
            result = new
            {
                FileName = FileName,
                Uid = guid.ToString()
            };
            //byte[] pdf = doc.Save();
            return Json(result);
            // return resulted pdf document 
            /*FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = FileName + DateTime.Now.ToString("yyyyMMddHHmm") + ".pdf";
            return fileResult;*/
        }

        /*public ActionResult GetDocument(string strUrl, string id, string typePrint)
        {            
            HtmlToPdf converter = new HtmlToPdf();
            var username = GetUserName();
            string url = "";
            string fileName = "";
            url = strUrl + "/PrintDocument/PrintTransportExecution?id=" + id + "&uName=" + username +"&typePrint=" + typePrint;
            if (typePrint == "delivery")
            {
                fileName = "DeliveryNote";
            }else
            {
                fileName = "IPB";
            }
            PdfDocument doc = converter.ConvertUrl(url);

            // save pdf document 
            byte[] pdf = doc.Save();

            // close pdf document 
            doc.Close();

            // return resulted pdf document 
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = fileName + DateTime.Now.ToString("yyyyMMddHHmm") + ".pdf";
            return fileResult;
        } */

        [HttpPost]
        public ActionResult UpdateVendor(List<TransportationExecutionInput> saveData)
        {
            try
            {
                _bllTransportExecution.SaveVendorTE(saveData, GetUserId());
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public ActionResult GetListVia()
        {
            var getVia = _bllTransportExecution.getMasterList(new List<string>(new[] { "ViaRoute" }));
            var dataVia = getVia.Where(_ => _.IsActive).Select(_ => _.FieldValue);
            return Json(dataVia);
        }
    }
}
