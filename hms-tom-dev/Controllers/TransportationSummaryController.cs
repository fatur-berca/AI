using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DFIS.Universal.Domain.Outputs;
using Newtonsoft.Json;
using TOM.Master.BusinessLogics;
using hms_tom_dev.Models.Masters;
using TOM.Transport.BusinessLogics.TransportExecutionBLL;
using hms_tom_dev.Models.Transport;
using TOM.Transport.BusinessLogics.TransportSummaryBLL;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using DFIS.Utils;
using TOM.Transport.Domain.DTOs;
using System.Web;
using DFIS.Universal.Domain.DTOs;

namespace hms_tom_dev.Controllers
{
    public class TransportationSummaryController : BaseController
    {
        private readonly IMasterListBLL _masterList;
        private readonly IMasterLocationBLL _masterLocationBLL;
        private readonly IMasterVendorTOMBLL _masterVendorTOMBll;
        private readonly ITransportExecutionBLL _transportExecutionBLL;
        private readonly ITransportSummaryBLL _transportSummaryBLL;

        public TransportationSummaryController(IMasterListBLL masterList, IMasterLocationBLL masterLocationBLL, IMasterVendorTOMBLL masterVendorTOMBll, ITransportExecutionBLL transportExecutionBLL, ITransportSummaryBLL transportSummaryBLL)
        {
            _masterList = masterList;
            _masterLocationBLL = masterLocationBLL;
            _masterVendorTOMBll = masterVendorTOMBll;
            _transportExecutionBLL = transportExecutionBLL;
            _transportSummaryBLL = transportSummaryBLL;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TrSummary));
        }

        public ActionResult Index()
        {
            var viewModel = new TransportSummaryViewModel();

            //List<SelectListItem> tempTransportNo = _transportSummaryBLL.GetTransportList()
            //    .Where(to => !string.IsNullOrWhiteSpace(to.TransportNo) && !to.TransportNo.ToUpper().StartsWith("E-"))
            //    .Select(f => new SelectListItem { Text = f.TransportNo, Value = f.TransportNo })
            //    .ToList();
            //List<SelectListItem> tempSTONo = _transportSummaryBLL.GetSTONo().Select(f => new SelectListItem { Text = f.STONo, Value = f.STONo }).ToList();
            List<SelectListItem> tempTransportCategory = _transportSummaryBLL.GetMstList().Where(w => w.FieldName == "TransportationCategory").Select(f => new SelectListItem { Text = f.FieldValue, Value = f.FieldValue }).ToList();
            List<SelectListItem> tempTransportMode = _transportSummaryBLL.GetMstList().Where(w => w.FieldName == "TransportationMode").Select(f => new SelectListItem { Text = f.FieldValue, Value = f.FieldValue }).ToList();
            List<SelectListItem> tempVendorName = _transportSummaryBLL.GetVendorName().Select(f => new SelectListItem { Text = f.VendorName, Value = f.IDVendor }).ToList();
            List<SelectListItem> tempVehicleType = _transportSummaryBLL.GetMstList().Where(w => w.FieldName == "VehicleType").Select(f => new SelectListItem { Text = f.FieldValue, Value = f.FieldValue }).ToList();
            List<SelectListItem> tempOrderCategory = _transportSummaryBLL.GetMstList().Where(w => w.FieldName == "OrderCategory").Select(f => new SelectListItem { Text = f.FieldValue, Value = f.FieldValue }).ToList();
            List<SelectListItem> tempZoneName = _transportSummaryBLL.GetMstList().Where(w => w.FieldName == "Zone").Select(f => new SelectListItem { Text = f.FieldValue, Value = f.FieldValue }).ToList();

            bool IsRoleTransport = GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA));
            List<MasterLocationDTO> tempMasterLocation = _transportExecutionBLL.getMasterLocation(IsRoleTransport, GetUserRegionSelectList().Select(x => x.Value).ToList());
            string[] locType = new string[] { "Warehouse", "Factory", "Agent", "Other", "Transport", "TPO" };
            List<SelectListItem> tempLocation = tempMasterLocation
                .Where(l => locType.Contains(l.Type))
                .Select(a => new SelectListItem { Text = a.LocationName, Value = a.IDLocation }).ToList();
            

            //viewModel.transNumbers = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            //viewModel.transNumbers.AddRange(tempTransportNo);
            //viewModel.orderNumbers = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            //viewModel.orderNumbers.AddRange(tempSTONo);
            viewModel.transCategorys = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.transCategorys.AddRange(tempTransportCategory);
            viewModel.transModes = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.transModes.AddRange(tempTransportMode);
            viewModel.vendorNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.vendorNames.AddRange(tempVendorName);
            viewModel.vehicleTypes = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.vehicleTypes.AddRange(tempVehicleType);
            viewModel.orderCategorys = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.orderCategorys.AddRange(tempOrderCategory);
            viewModel.zoneNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.zoneNames.AddRange(tempZoneName);
            viewModel.startLocations = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.startLocations.AddRange(tempLocation);
            viewModel.senderNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.senderNames.AddRange(tempLocation);
            viewModel.receiverNames = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.receiverNames.AddRange(tempLocation);

            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.ListUserRole = GetListUserRole();
            return View(viewModel);
        }

        public ActionResult GetListTab()
        {
            var dbResult = _transportSummaryBLL.GetListTabName();
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListRole()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];

            var dbResult = new List<MasterRoleModel>();

            var query1 = session.Role.Select(f => new {f.IDRole, f.RoleName }).SingleOrDefault();
            if (query1 != null && (query1.RoleName.ToUpper().Contains(EnumHelper.GetDescription(Enums.RoleUserList.AWR)) || query1.RoleName.ToUpper().Contains(EnumHelper.GetDescription(Enums.RoleUserList.ATR)) || query1.RoleName.ToUpper().Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA))))
            {
                dbResult.Add(new MasterRoleModel { IDRole = 0, RoleName = "ALL" });
            }
            
            var query2 = session.Role.Select(f => new {f.IDRole, f.RoleName }).ToList();
            dbResult.AddRange(from data in query2 where data.RoleName.ToUpper().Contains(EnumHelper.GetDescription(Enums.RoleUserList.AWR)) || data.RoleName.ToUpper().Contains(EnumHelper.GetDescription(Enums.RoleUserList.ATR)) select new MasterRoleModel { IDRole = data.IDRole, RoleName = data.RoleName });

            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportationNumberFilter(string term)
        {
            return Json(_transportSummaryBLL.GetTransportationNumberFilter(term), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoNoFilter(string term)
        {
            return Json(_transportSummaryBLL.GetSTONoFilter(term), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListView(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro)
        {
            var dbResult = _transportSummaryBLL.GetListView(fltrdf, fltrdt, fltrtn, fltron, fltrtc, fltrtm, fltrvn, fltrvt, fltroc, fltrzo, fltrsl, fltrse, fltrre, fltrth, fltrro, GetListUserRole());
            //return Json(dbResult, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(dbResult, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetListViewOpenClose(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro, int fltbln)
        {
            List<TransportExecutionDTOMin> dbResult = _transportSummaryBLL.GetListView(fltrdf, fltrdt, fltrtn, fltron, fltrtc, fltrtm, fltrvn, fltrvt, fltroc, fltrzo, fltrsl, fltrse, fltrre, fltrth, fltrro, GetListUserRole());
            List<TransportExecutionDTOMin> displayData = dbResult.Where(x => x.TransportDate.Month == fltbln).OrderByDescending(x => x.TransportDate).OrderByDescending(x => x.TransportNo).ToList();
            var jsonResult = Json(displayData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return Json(jsonResult);
        }

        public ActionResult CalculateCost(List<string> idTransportExecution)
        {
            _transportSummaryBLL.CalculateCost(idTransportExecution,GetUserId());
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportXls(string check, string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro)
        {
            var xls_file = new ExcelPackage();
            ExcelWorksheet xls_sheet;
            ExcelRange xls_range;

            string[] colMonths = { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string path = "";

            if (check == "DD" || check == "CF")
            {
                string nme_sheet = "";
                string[] colTitles = new string[] { };
                string[] dtaRows = new string[] { };

                int tC = 0;

                if (check == "DD")
                {
                    nme_sheet = "Transaction Summary-Detail";
                    colTitles = new string[] { "", "Trip", "Transportation Number", "Order Number", "Transportation Date", /*5*/"Week", "Month", "Zone", "Sender ID", "Sender",/*10*/ "Receiver ID", "Receiver", "Order Type", "Material Type", "Material Description", /*15*/"Material Quantity", "UoM", "Material Quantity2", "UoM2", "Total Stick", /*20*/"Transportation Category", "Vendor", "Transportation Mode", "Transportation Status", "Vehicle Type", /*25*/"Police Registration Number", "Start Location", "Finish Location", "KM/Order", "Based KM", /*30*/"KM Rail", "KM Sea", "Total KM", "Based Cost", "Discount Cost", /*35*/"ASDP", "SPSI", "Additional Cost", "Total Cost", "Service PO Number", /*40*/"Service GR Number", "Mapping CC", "Cost Center", "Account", "Load Factor", "Liter/TKM", /*46*/"KGCO2" };
                    tC = 46;
                    path = "TransactionSummary-Detail" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                }
                else if (check == "CF")
                {
                    nme_sheet = "Transaction Summary-ReClass";
                    colTitles = new string[] { "", "Transportation Number", "Order Number", "Transportation Date", "Week", /*5*/"Month", "Sender", "Receiver", "Vendor", "Transportation Mode",/*10*/ "Vehicle Type", "Order Type", "Material Type", "Material Quantity", "UoM", /*15*/"Total Cost", "Mapping CC", "Cost Center", "Account" };
                    tC = 18;
                    path = "TransactionSummary-ReClass" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                }

                xls_sheet = xls_file.Workbook.Worksheets.Add(nme_sheet);

                for (int c = 1; c <= tC; c++)
                {
                    using (xls_range = xls_sheet.Cells[1, c])
                    {
                        xls_range.Value = colTitles[c];
                        xls_range.Style.Font.Size = 11;
                        xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                    }

                    if (colTitles[c].Length < 10) { xls_sheet.Column(c).Width = 10; }
                    else { xls_sheet.Column(c).Width = colTitles[c].Length + 2; }
                }

                int stc = 0, enc = 0, r = 2, rts = 2; string te = "", to = ""; decimal v = 0, ts = 0;

                var dbResult = _transportSummaryBLL.GetExportXlsDetailClass(fltrdf, fltrdt, fltrtn, fltron, fltrtc, fltrtm, fltrvn, fltrvt, fltroc, fltrzo, fltrsl, fltrse, fltrre, fltrth, fltrro, GetListUserRole());

                // Check Data with tC=46
                foreach (var data in dbResult)
                {
                    if (tC == 18)
                    {
                        dtaRows = new string[] { "", data.TransportNo, data.STONo, data.TransportDate.ToString("dd-MMM-yyyy"), data.SIWeek.ToString(), /*5*/data.Month.ToString(), data.Sender, data.Receiver, data.VendorName, data.TransportMode, /*10*/data.ActualVehicleType, data.OrderType, data.MaterialType, data.Qty.ToString(), data.UoM, /*15*/data.TotalCost.ToString(), data.MappingCC, data.ActualCostCenter, data.Account, data.IDTransportExecution.ToString(), /*20*/data.IDTransportOrder.ToString(), data.BasedPrice.ToString(), data.DiscountPrice.ToString() };
                    }
                    if (tC == 46)
                    {
                        string literTKM = "";
                        if (data.TransportMode == "Truck")
                            literTKM = data.CFPLiter.ToString();
                        else if(data.TransportMode == "Ship" || data.TransportMode == "Train")
                            literTKM = data.CFPTKM.ToString();
                        dtaRows = new string[] { "", data.Route.ToString(), data.TransportNo, data.STONo, data.TransportDate.ToString("dd-MMM-yyyy"), /*5*/data.SIWeek.ToString(), data.Month.ToString(), data.ZoneBased, data.IDSender, data.Sender, /*10*/data.IDReceiver, data.Receiver, data.OrderType, data.MaterialType, data.Description, /*15*/data.Qty.ToString(), data.UoM, "", data.UoM, "", /*20*/data.TransportCategory, data.VendorName, data.TransportMode, data.TransportStatus, data.ActualVehicleType, /*25*/data.PoliceRegNo, data.StartLocation, data.FinishLocation, data.KMOrder.ToString(), Math.Round(data.KMBased.Value).ToString(), /*30*/Math.Round(data.KMRail.Value).ToString(), Math.Round(data.KMSea.Value).ToString(), Math.Round(data.TotalKM.Value).ToString(), data.BasedPrice.ToString(), data.DiscountPrice.ToString(), /*35*/data.ASDPCost.ToString(), data.SPSICost.ToString(), data.AdditionalCost.ToString(), data.TotalCost.ToString(), data.ServicePONo, /*40*/data.SerivceGRNo, data.MappingCC, data.ActualCostCenter, data.Account, data.AVGLoadFactor.ToString(), /*45*/literTKM, data.KGCO2.ToString(), data.IDTransportExecution.ToString(), data.IDTransportOrder.ToString(), data.StickPerBox.ToString(), /*50*/data.PackPerBox.ToString(), data.StickPerPack.ToString() };
                    }

                    if (dtaRows[tC + 1] != te) { stc = 1; enc = tC; ts = 0; rts = r; }
                    else {
                        if (tC == 18) { stc = 1; enc = 18; }
                        if (tC == 46) { stc = 2; enc = 46; }
                    }

                    for (int c = stc; c <= enc; c++)
                    {
                        using (xls_range = xls_sheet.Cells[r, c])
                        {
                            if (c == 17)
                            {
                                if (tC == 46 && dtaRows[13] != null && dtaRows[13].ToLower() == "cigarette")
                                {
                                    if (dtaRows[16].ToLower() == "box")
                                    {
                                        string box = dtaRows[49];
                                        if (string.IsNullOrEmpty(box))
                                            box = "0.0";
                                        v = Convert.ToDecimal(dtaRows[15]) * Convert.ToDecimal(box);
                                    }
                                    if (dtaRows[16].ToLower() == "pack")
                                    {
                                        string pack = dtaRows[51];
                                        if (string.IsNullOrEmpty(pack))
                                            pack = "0.0";
                                        v = Convert.ToDecimal(dtaRows[15]) * (Convert.ToDecimal(pack));
                                    }

                                    ts = ts + v;
                                    xls_range.Value = double.Parse(v.ToString());
                                    xls_range.Style.Numberformat.Format = "#,##0.00";
                                }
                                else if (tC == 46)
                                {
                                    xls_range.Value = double.Parse(dtaRows[15]);
                                    xls_range.Style.Numberformat.Format = "#,##0.00";
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(dtaRows[c]))
                                    {
                                        var spilitString = dtaRows[c].Split('-');
                                        xls_range.Value = spilitString[1];
                                    }
                                    else
                                        xls_range.Value = "";
                                }
                            }
                            else if (c == 18)
                            {
                                if (dtaRows[13] != null && dtaRows[13].ToLower() == "cigarette") {
                                    xls_range.Value = "Stick";
                                }
                                else
                                {
                                    xls_range.Value = dtaRows[c];
                                }
                            }
                            else if ((tC == 18 && c == 15) || (tC == 46 && c == 38))
                            {
                                if(tC == 46 && dtaRows[tC + 1] != te) { 
                                    xls_sheet.Cells[r, c].Value = Convert.ToDecimal(dtaRows[c]);
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "0,#";
                                }
                                else if(tC == 18)
                                {
                                    if (dtaRows[c] != null && dtaRows[c] != "") { 
                                        xls_sheet.Cells[r, c].Value = Convert.ToDecimal(dtaRows[c]);
                                        xls_sheet.Cells[r, c].Style.Numberformat.Format = "0,#";
                                    }
                                    else
                                        xls_sheet.Cells[r, c].Value = "";
                                }
                            }
                            else if ((tC == 46 && c == 15) )
                            {
                                xls_range.Value = double.Parse(dtaRows[c]);
                                xls_range.Style.Numberformat.Format = "#,##0.00";
                            }
                            else
                            {
                                if (tC == 18 && stc == 2)
                                {
                                    if (c == 2 || c == 6 || c == 7 || c >= 11)
                                    {
                                        xls_range.Value = dtaRows[c];
                                    }
                                }
                                else
                                {
                                    if (tC == 46) {
                                        if (c == 2 || c == 20 || c == 21 || c == 22 || c == 23 || c == 24 || c == 25 ||  c == 26 || c == 27 || c == 39 || c == 40 || c == 42 || c == 43 ) {
                                            if ( c == 42) //kolom cost center
                                            {
                                                if (!string.IsNullOrEmpty(dtaRows[c]))
                                                {
                                                    var spilitString = dtaRows[c].Split('-');
                                                    if (spilitString.Length > 1)
                                                        xls_range.Value = spilitString[1];
                                                    else
                                                        xls_range.Value = dtaRows[c];
                                                }
                                                else{
                                                    xls_range.Value = dtaRows[c];
                                                }
                                            }
                                            else { 
                                                xls_range.Value = dtaRows[c];
                                            }
                                        }
                                        else if (c == 29 || c == 30 || c == 31 || c == 32 || c == 33 || c == 34 || c == 35 || c == 36 || c == 37 || c == 38 || c == 44 || c == 45 || c == 46) {
                                            if(dtaRows[tC + 1] != te) {
                                                xls_range.Value = dtaRows[c];
                                            }
                                            else
                                                xls_range.Value = "";
                                        }
                                        else if (c == 1)
                                        {
                                            if(dtaRows[tC + 1] != te)
                                            {
                                                xls_range.Value = "1";
                                            }
                                        }
                                        else {
                                            xls_range.Value = dtaRows[c];
                                        }
                                    }
                                    else { 
                                        xls_range.Value = dtaRows[c];
                                    }
                                }
                            }

                            if ((tC == 18 && c == 5 && xls_sheet.Cells[r, 5].Value != null) || (tC == 46 && c == 6 && xls_sheet.Cells[r, 6].Value != null))
                            {
                                xls_sheet.Cells[r, c].Value = colMonths[int.Parse(dtaRows[c])];
                            }

                            if (
                                (tC == 18 && (c == 4 || c == 13))
                                ||
                                (tC == 46 && (c == 5 || c == 29 || c == 30 || c == 31 || c == 33 || c == 34 || c == 35 || c == 36 || c == 37 || c == 44 || c == 45 || c == 46))
                                )
                            {
                                if (xls_sheet.Cells[r, c].Value != null && xls_sheet.Cells[r, c].Value != "") { 
                                    if(tC == 18 && c == 4 && dtaRows[c] == "0")
                                        xls_sheet.Cells[r, c].Value = "";
                                    else
                                        xls_sheet.Cells[r, c].Value = Convert.ToDecimal(xls_sheet.Cells[r, c].Value);
                                }
                                else
                                    xls_sheet.Cells[r, c].Value = "";

                                if (c == 33 || c == 34 || c == 35 || c == 36 || c == 37)
                                {
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "0,#";
                                }
                                else if(c == 44)
                                {
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "0%";
                                }
                                else if (c == 45 || c == 46)
                                {
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "#,##0.00";
                                }
                                else
                                {
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "0";
                                }
                            } 
                            
                            if(tC == 46 && c == 32)
                            {
                                if (xls_sheet.Cells[r, c].Value != null && xls_sheet.Cells[r, c].Value != "")
                                    xls_sheet.Cells[r, c].Value = (int) Convert.ToDecimal(xls_sheet.Cells[r, c].Value);
                                else
                                    xls_sheet.Cells[r, c].Value = "";
                            }

                            if ((tC == 18 && c == 3) || (tC == 46 && c == 4))
                            {
                                if(xls_sheet.Cells[r, c].Value != null) { 
                                    xls_sheet.Cells[r, c].Value = Convert.ToDateTime(xls_sheet.Cells[r, c].Value).Date;
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "dd-MMM-yyyy";
                                }
                                else
                                {
                                    xls_sheet.Cells[r, c].Value = "";
                                }
                            }
                        }
                    }

                    if (tC == 46 && ts > 0) {
                        xls_sheet.Cells[rts, 19].Value = ts;
                        xls_sheet.Cells[rts, 19].Style.Numberformat.Format = "0,#";
                    }

                    if (tC == 46)
                    {
                        xls_sheet.Cells[r, 28].Value = Convert.ToDecimal(dtaRows[28]);
                        xls_sheet.Cells[r, 28].Style.Numberformat.Format = "0";
                    }

                    for (int c = 1; c <= tC; c++)
                    {
                        using (xls_range = xls_sheet.Cells[r, c])
                        {
                            xls_range.Style.Font.Size = 11;
                            xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }
                    }

                    te = dtaRows[tC + 1];
                    to = dtaRows[tC + 2];
                    r++;
                }
            }
            else if (check == "CD" || check == "VF")
            {
                string nme_sheet = "";
                string[] colTitles = new string[] { };
                string[] dtaRows = new string[] { };

                int tC = 0;

                if (check == "CD") {
                    nme_sheet = "Transaction Summary-Compact";
                    colTitles = new string[] { "", "Transportation Number", "Order Number", "Total Order", "Zone",/*5*/ "Sender", "Receiver", "Transportation Date", "Week", "Month",/*10*/ "Transportation Category", "Vendor", "Transportaion Mode", "Vehicle Type", "Police Registration Number",/*15*/ "Route", "Transportation Status", "GR Time", "KM/Order", "Based KM",/*20*/ "KM Rail", "KM Sea", "Total KM", "Based Cost", "Discount Cost",/*25*/ "ASDP", "SPSI", "Additional Cost", "Total Cost", "Service PO Number",/*30*/ "Service GR Number", "Load Factor", "Liter/TKM", "KGCO2" };
                    tC = 33;
                    path = "TransactionSummary-Compact" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                }
                else if (check == "VF") {
                    nme_sheet = "Transaction Summary-Validation";
                    colTitles = new string[] { "", "Transportation Number", "Order Number", "Transportation Date", "Week", "Month", "Sender", "Receiver", "Route", "Vendor", "Vehicle Type", "Police Registration Number", "Based KM", "Total KM", "Based Cost", "Discount Cost", "ASDP", "SPSI", "Additional Cost", "Total Cost", "Service PO Number", "Remarks" };
                    tC = 21;
                    path = "TransactionSummary-Validation" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                }

                xls_sheet = xls_file.Workbook.Worksheets.Add(nme_sheet);

                for (int c = 1; c <= tC; c++)
                {
                    using (xls_range = xls_sheet.Cells[1, c])
                    {
                        xls_range.Value = colTitles[c];
                        xls_range.Style.Font.Size = 11;
                        xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                    }

                    if (colTitles[c].Length < 10) { xls_sheet.Column(c).Width = 10; }
                    else { xls_sheet.Column(c).Width = colTitles[c].Length + 2; }
                }

                int r = 1, tto = 0, rv = 0; string te = "", to = "", tr = "";

                var dbResult = _transportSummaryBLL.GetExportXlsCompactValidation(fltrdf, fltrdt, fltrtn, fltron, fltrtc, fltrtm, fltrvn, fltrvt, fltroc, fltrzo, fltrsl, fltrse, fltrre, fltrth, fltrro, GetListUserRole());
                dbResult = dbResult.OrderBy(p => p.TransportNo).ThenBy(p => p.TransportDate).ThenBy(p => p.IDTransportOrder).ThenBy(p => p.IDRoute).ToList();

                foreach (var data in dbResult)
                {
                    if (tC == 21)
                    {
                        dtaRows = new string[] { "", data.TransportNo, data.STONo, data.TransportDate.ToString("dd-MMM-yyyy"), data.SIWeek.ToString(), data.Month.ToString(), data.Sender, data.Receiver, data.Route, data.VendorName, data.ActualVehicleType, data.PoliceRegNo, Math.Round(data.KMBased.Value,0).ToString(), Math.Round(data.TotalKM.Value, 0).ToString(), data.BasedPrice.ToString(), data.DiscountPrice.ToString(), data.ASDPCost.ToString(), data.SPSICost.ToString(), data.AdditionalCost.ToString(), data.TotalCost.ToString(), data.ServicePONo, data.Remarks, data.IDTransportExecution.ToString(), data.IDTransportOrder.ToString(), data.IDRoute.ToString() };
                    }
                    if (tC == 33)
                    {
                        string literTKM = "";
                        if (data.TransportMode == "Truck")
                            literTKM = data.CFPLiter.ToString();
                        else if (data.TransportMode == "Ship" || data.TransportMode == "Train")
                            literTKM = data.CFPTKM.ToString();
                        dtaRows = new string[] { "", data.TransportNo, data.STONo, "", data.ZoneBased, /*5*/data.IDSender + ":" + data.Sender, data.IDReceiver + ":" + data.Receiver, data.TransportDate.ToString("dd-MMM-yyyy"), data.SIWeek.ToString(), data.Month.ToString(), /*10*/data.TransportCategory, data.VendorName, data.TransportMode, data.ActualVehicleType, data.PoliceRegNo, /*15*/data.Route, data.TransportStatus, data.GRDate.ToString(), data.KMOrder.ToString(),Math.Round(data.KMBased.Value).ToString(), /*20*/Math.Round(data.KMRail.Value).ToString(), Math.Round(data.KMSea.Value).ToString(), Math.Round(data.TotalKM.Value).ToString(), data.BasedPrice.ToString(), data.DiscountPrice.ToString(), /*25*/data.ASDPCost.ToString(), data.SPSICost.ToString(), data.AdditionalCost.ToString(), data.TotalCost.ToString(), data.ServicePONo, /*30*/data.SerivceGRNo, data.AVGLoadFactor.ToString(), literTKM, data.KGCO2.ToString(), data.IDTransportExecution.ToString(), /*35*/data.IDTransportOrder.ToString(), data.IDRoute.ToString() };
                    }

                    if (dtaRows[tC + 1] != te) { tto = 0; rv = 0; r++; }

                    for (int c = 1; c <= tC; c++)
                    {
                        using (xls_range = xls_sheet.Cells[r, c])
                        {
                            if (c == 2 || c == 3 || c == 5 || c == 6 || c == 7 || c == 18)
                            {
                                if (dtaRows[tC + 2] != to)
                                {
                                    if (tC == 33 && c == 3) { tto++; xls_range.Value = tto; }

                                    if (xls_sheet.Cells[r, c].Value == null) {
                                        if (c == 2 || c == 5 || c == 6 || c == 7 || (tC == 21 && c == 3))
                                        {
                                            xls_sheet.Cells[r, c].Value = dtaRows[c];
                                        }
                                        else if (tC == 33 && c == 18) { xls_sheet.Cells[r, c].Value = Math.Round(Convert.ToDecimal(dtaRows[c]), 0); }
                                    }
                                    else {
                                        if (c == 2 || (tC == 21 && (c == 6 || c == 7)) || (tC == 33 && (c == 5 || c == 6)))
                                        {
                                            if (dtaRows[tC + 1] != te)
                                            {
                                                xls_sheet.Cells[r, c].Value = dtaRows[c];
                                            }
                                            else if (dtaRows[tC + 1] == te && dtaRows[tC + 2] != to)
                                                xls_sheet.Cells[r, c].Value = xls_sheet.Cells[r, c].Value + ", " + dtaRows[c];
                                        }
                                        else if (tC == 33 && c == 18)
                                        {
                                            xls_sheet.Cells[r, c].Value = xls_sheet.Cells[r, c].Value + "," + Math.Round(Convert.ToDecimal(dtaRows[c]), 0);
                                        }
                                    }
                                }
                            }
                            else if ((tC == 21 && c == 8) || (tC == 33 && c == 15))
                            {
                                if(xls_sheet.Cells[r, c].Value == null)
                                {
                                    var listroute = _transportSummaryBLL.GetTransportRoute(dtaRows[1]);

                                    var count = 0;
                                    foreach(var route in listroute)
                                    {
                                        var locationname = _masterLocationBLL.GetNameById(route.IDLocation);

                                        if(count == 0)
                                        {
                                            xls_sheet.Cells[r, c].Value = locationname;
                                        }
                                        else
                                        {
                                            xls_sheet.Cells[r, c].Value = xls_sheet.Cells[r, c].Value + "-" + locationname;
                                        }

                                        count += 1;
                                    }
                                }

                                /*
                                if (dtaRows[tC + 1] != te)
                                {
                                    xls_sheet.Cells[r, c].Value = dtaRows[c];
                                }
                                else if (dtaRows[tC + 1] == te && dtaRows[tC + 3] != tr)
                                {
                                    if (dtaRows[c] != null)
                                    {
                                        // if (xls_sheet.Cells[r, c].Value.ToString().IndexOf(dtaRows[c]) == -1)
                                        xls_sheet.Cells[r, c].Value = xls_sheet.Cells[r, c].Value + "-" + dtaRows[c];
                                    }
                                    else
                                    {
                                        xls_sheet.Cells[r, c].Value = xls_sheet.Cells[r, c].Value + "";
                                    }
                                    //else { rv = 1; }
                                }
                                */
                            }
                            else { xls_range.Value = dtaRows[c]; }

                            xls_range.Style.Font.Size = 11;
                            xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            if ((tC == 21 && c == 5 && xls_sheet.Cells[r, 5].Value != null) || (tC == 33 && c == 9 && xls_sheet.Cells[r, 9].Value != null))
                            {
                                xls_sheet.Cells[r, c].Value = colMonths[int.Parse(dtaRows[c])];
                            }

                            if (tC == 33 && c == 17 && xls_sheet.Cells[r, 17].Value.ToString().Length > 0)
                            {
                                xls_sheet.Cells[r, 17].Value = Convert.ToDateTime(xls_sheet.Cells[r, 17].Value);
                                xls_sheet.Cells[r, 17].Style.Numberformat.Format = "dd/mm/yy hh:mm AM/PM";
                            }

                            if ((tC == 21 && c == 19) || (tC == 33 && c == 28))
                            {
                                xls_sheet.Cells[r, c].Value = dtaRows[c];//Convert.ToDecimal(dtaRows[c - 5]) - Convert.ToDecimal(dtaRows[c - 4]);
                            }

                            if ((tC == 21 && (c == 4 || c >= 12 && c <= 19)) || (tC == 33 && (c == 8 || (c >= 19 && c <= 28) || c==31 || c ==32 || c == 33)))
                            {
                                if (xls_sheet.Cells[r, c].Value != null && xls_sheet.Cells[r, c].Value != "")
                                    xls_sheet.Cells[r, c].Value = Convert.ToDecimal(xls_sheet.Cells[r, c].Value);
                                else
                                    xls_sheet.Cells[r, c].Value = 0;

                                if ((tC == 21 && (c == 14 || c == 15 || c == 18)) || (tC == 33 && (c == 23 || c == 24 || c == 28)))
                                {
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "0,#";
                                }
                                else if (c == 31)
                                {
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "0%";
                                }
                                else if (c == 32 || c == 33)
                                {
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "#,##0.00";
                                }
                                else
                                {
                                    xls_sheet.Cells[r, c].Style.Numberformat.Format = "0";
                                }
                            }

                            if ((tC == 21 && c == 3) || (tC == 33 && c == 7))
                            {
                                xls_sheet.Cells[r, c].Value = Convert.ToDateTime(xls_sheet.Cells[r, c].Value);
                                xls_sheet.Cells[r, c].Style.Numberformat.Format = "dd-MMM-yyyy";
                            }
                        }
                    }

                    te = dtaRows[tC + 1]; to = dtaRows[tC + 2]; tr = dtaRows[tC + 3];

                    // ONNumber.Clear();
                }
            }

            xls_file.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
            return Json(path, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CustomXls(string colCaption, string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro)
        {
            ExcelPackage xls_file = new ExcelPackage();
            ExcelWorksheet xls_sheet;
            ExcelRange xls_range;

            string path = "";

            xls_sheet = xls_file.Workbook.Worksheets.Add("Transaction Summary Custom");

            int cols = colCaption.Split(',').Count();
            List<string> listcols = colCaption.Split(',').ToList();

            for (int n = 0; n < cols; n++)
            {
                using (xls_range = xls_sheet.Cells[1, n + 1])
                {
                    xls_range.Value = listcols[n];

                    xls_range.Style.Font.Size = 11;
                    xls_range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //xls_range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //xls_range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                }

                //if (colCaption.Split(',')[n].Length < 10) { xls_sheet.Column(n + 1).Width = 10; }
                //else { xls_sheet.Column(n + 1).Width = colCaption.Split(',')[n].Length + 2; }
            }

            int r = 2;

            var dbResult = _transportSummaryBLL.GetCustomXls(fltrdf, fltrdt, fltrtn, fltron, fltrtc, fltrtm, fltrvn, fltrvt, fltroc, fltrzo, fltrsl, fltrse, fltrre, fltrth, fltrro, GetListUserRole());

            foreach (var data in dbResult)
            {
                var dict = new Dictionary<string, string>();
                dict["Transportation Number"] = data.TransportNo;
                dict["Transportation Date"] = data.TransportDate.ToString("dd/MMM/yyyy");
                dict["Transportation Status"] = data.TransportStatus;
                dict["Vendor"] = data.VendorName;
                dict["Actual Vehicle Type"] = data.ActualVehicleType;
                dict["Based KM"] = data.KMBased.ToString();
                dict["Based Cost"] = data.BasedPrice.ToString();

                //string[] dtaRows = { "", data.TransportNo, data.TransportStatus, data.STONo, data.TransportDate.ToString("dd-MMM-yyyy"), data.OrderCategory, data.ZoneBased, data.Sender, data.Receiver, data.CargoType, data.VendorName, data.ActualVehicleType, data.KMBased.ToString(), data.BasedPrice.ToString() };

                for (int n = 0; n < cols; n++)
                {
                    using (xls_range = xls_sheet.Cells[r, n + 1])
                    {
                        xls_range.Value = dict[listcols[n]];

                        xls_range.Style.Font.Size = 11;
                        xls_range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    //if (int.Parse(colIndex.Split(',')[n]) == 3)
                    //{
                    //    xls_sheet.Cells[r, n + 1].Value = Convert.ToInt64(xls_sheet.Cells[r, n + 1].Value);
                    //    xls_sheet.Cells[r, n + 1].Style.Numberformat.Format = "#";
                    //}

                    //if (int.Parse(colIndex.Split(',')[n]) == 12 || int.Parse(colIndex.Split(',')[n]) == 13)
                    //{
                    //    xls_sheet.Cells[r, n + 1].Value = Convert.ToDecimal(xls_sheet.Cells[r, n + 1].Value);
                    //    xls_sheet.Cells[r, n + 1].Style.Numberformat.Format = "#,#";
                    //}
                }

                r++;
            }

            path = "TransactionSummaryCustom" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            xls_file.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

            return Json(path, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ImportValidateXls()
        {
            ExcelPackage xls_file_new = new ExcelPackage();
            ExcelWorksheet xls_sheet_new;
            ExcelRange xls_range_new;

            string[] colMonths = { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string path = "";

            string[] colTitles = { "", "Transportation Number", "Order Number", "Transportation Date", "Week", "Month", "Sender", "Receiver", "Route", "Vendor", "Vehicle Type", "Police Registration Number", "Base KM", "Base Cost", "Discount Cost", "ASDP", "SPSI", "Additional Cost", "Total Cost", "Service PO Number", "Remarks" };
            xls_sheet_new = xls_file_new.Workbook.Worksheets.Add("Transaction Summary-Validation");

            HttpPostedFileBase file = Request.Files[0];

            if (System.IO.Path.GetExtension(file.FileName).ToLower() == ".xlsx")
            {
                ExcelPackage xls_file = new ExcelPackage(file.InputStream);
                ExcelWorksheet xls_sheet;

                if (xls_file.Workbook.Worksheets.Count > 0 && xls_file.Workbook.Worksheets[1].Name == "Transaction Summary-Validation")
                {
                    xls_sheet = xls_file.Workbook.Worksheets[1];
                    if (xls_sheet.Dimension.End.Column == 20 && xls_sheet.Dimension.End.Row > 1)
                    {
                        if (xls_sheet.Cells[1, 1].Value.ToString().ToLower().Trim() == "transportation number" &&
                            xls_sheet.Cells[1, 2].Value.ToString().ToLower().Trim() == "order number" &&
                            xls_sheet.Cells[1, 3].Value.ToString().ToLower().Trim() == "transportation date" &&
                            xls_sheet.Cells[1, 4].Value.ToString().ToLower().Trim() == "week" &&
                            xls_sheet.Cells[1, 5].Value.ToString().ToLower().Trim() == "month" &&
                            xls_sheet.Cells[1, 6].Value.ToString().ToLower().Trim() == "sender" &&
                            xls_sheet.Cells[1, 7].Value.ToString().ToLower().Trim() == "receiver" &&
                            xls_sheet.Cells[1, 8].Value.ToString().ToLower().Trim() == "route" &&
                            xls_sheet.Cells[1, 9].Value.ToString().ToLower().Trim() == "vendor" &&
                            xls_sheet.Cells[1, 10].Value.ToString().ToLower().Trim() == "vehicle type" &&
                            xls_sheet.Cells[1, 11].Value.ToString().ToLower().Trim() == "police registration number" &&
                            xls_sheet.Cells[1, 12].Value.ToString().ToLower().Trim() == "base km" &&
                            xls_sheet.Cells[1, 13].Value.ToString().ToLower().Trim() == "base cost" &&
                            xls_sheet.Cells[1, 14].Value.ToString().ToLower().Trim() == "discount cost" &&
                            xls_sheet.Cells[1, 15].Value.ToString().ToLower().Trim() == "asdp" &&
                            xls_sheet.Cells[1, 16].Value.ToString().ToLower().Trim() == "spsi" &&
                            xls_sheet.Cells[1, 17].Value.ToString().ToLower().Trim() == "additional cost" &&
                            xls_sheet.Cells[1, 18].Value.ToString().ToLower().Trim() == "total cost" &&
                            xls_sheet.Cells[1, 19].Value.ToString().ToLower().Trim() == "service po number" &&
                            xls_sheet.Cells[1, 20].Value.ToString().ToLower().Trim() == "remarks"
                            )
                        {

                            for (int c = 1; c <= 20; c++)
                            {
                                using (xls_range_new = xls_sheet_new.Cells[1, c])
                                {
                                    xls_range_new.Value = colTitles[c];
                                    xls_range_new.Style.Font.Size = 11;
                                    xls_range_new.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    xls_range_new.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    xls_range_new.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    xls_range_new.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
                                }

                                if (colTitles[c].Length < 10) { xls_sheet_new.Column(c).Width = 10; }
                                else { xls_sheet_new.Column(c).Width = colTitles[c].Length + 2; }
                            }

                            int rowCount = xls_sheet.Dimension.End.Row;
                            for (int row = 2; row <= rowCount; row++)
                            {
                                string tn = xls_sheet.Cells[row, 1].Value.ToString().Trim();
                                string on = xls_sheet.Cells[row, 2].Value.ToString().Trim();

                                if (tn != "" && on != "")
                                {
                                    int cols = on.Split(',').Count();

                                    for (int n = 0; n < cols; n++)
                                    {
                                        string tr = "";
                                        var dbResult = _transportSummaryBLL.GetImportValidateXls(tn, on.Split(',')[n]);

                                        foreach (var data in dbResult)
                                        {
                                            string[] r = { "", data.TransportNo, data.STONo, data.TransportDate.ToString("dd-MMM-yyyy"), data.SIWeek.ToString(), data.Month.ToString(), data.Sender, data.Receiver, data.Route, data.VendorName, data.VehicleType, data.PoliceRegNo, data.KMBased.ToString(), data.BasedPrice.ToString(), data.DiscountPrice.ToString(), data.ASDPCost.ToString(), data.SPSICost.ToString(), data.AdditionalCost.ToString(), "", data.ServicePONo, data.Remarks, data.IDRoute.ToString() };

                                            for (int rc = 1; rc <= 20; rc++)
                                            {
                                                if (rc == 2 || rc == 6 || rc == 7)
                                                {
                                                    xls_sheet_new.Cells[row, rc].Value = xls_sheet.Cells[row, rc].Value.ToString();
                                                }
                                                else if (rc == 8)
                                                {
                                                    if (tr == "")
                                                    {
                                                        xls_sheet_new.Cells[row, rc].Value = r[rc];
                                                    }
                                                    else
                                                    {
                                                        xls_sheet_new.Cells[row, rc].Value = xls_sheet_new.Cells[row, rc].Value +  ", " + r[rc];
                                                    }
                                                }
                                                else
                                                {
                                                    xls_sheet_new.Cells[row, rc].Value = r[rc];
                                                }

                                                xls_sheet_new.Cells[row, rc].Style.Font.Size = 11;
                                                xls_sheet_new.Cells[row, rc].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                if (rc == 5 && xls_sheet_new.Cells[row, rc].Value != null)
                                                {
                                                    xls_sheet_new.Cells[row, rc].Value = colMonths[int.Parse(r[rc])];
                                                }
                                                if (rc == 18)
                                                {
                                                    xls_sheet_new.Cells[row, rc].Value = Convert.ToDecimal(r[13]) - Convert.ToDecimal(r[14]);
                                                }
                                                if (rc == 4 || rc >= 12 && rc <= 19)
                                                {
                                                    xls_sheet_new.Cells[row, rc].Value = Convert.ToDecimal(xls_sheet_new.Cells[row, rc].Value);

                                                    if (rc == 13 || rc == 14 || rc == 18) { xls_sheet_new.Cells[row, rc].Style.Numberformat.Format = "#,#"; }
                                                    else { xls_sheet_new.Cells[row, rc].Style.Numberformat.Format = "#"; }
                                                }
                                            }

                                            for (int c = 3; c <= 20; c++)
                                            {
                                                string vnew = xls_sheet_new.Cells[row, c].Value.ToString();
                                                string vcol = "";
                                                if (xls_sheet.Cells[row, c].Value != null)
                                                {
                                                    vcol = xls_sheet.Cells[row, c].Value.ToString();
                                                }

                                                if (c == 2 || c == 6 || c == 7 || c == 8)
                                                {
                                                    if (!vcol.Contains(r[c]))
                                                    {
                                                        xls_sheet_new.Cells[row, c].Value = vcol;
                                                        xls_sheet_new.Cells[row, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                        xls_sheet_new.Cells[row, c].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                                    }
                                                }
                                                else if (c == 4 || c >= 12 && c <= 19)
                                                {
                                                    if (Convert.ToDecimal(vcol) != Convert.ToDecimal(vnew))
                                                    {
                                                        xls_sheet_new.Cells[row, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                        xls_sheet_new.Cells[row, c].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                                                        xls_sheet_new.Cells[row, c].Value = Convert.ToDecimal(vcol);
                                                        if (c == 13 || c == 14 || c == 18) { xls_sheet_new.Cells[row, c].Style.Numberformat.Format = "#,#"; }
                                                        else { xls_sheet_new.Cells[row, c].Style.Numberformat.Format = "#"; }
                                                    }
                                                }
                                                else
                                                {
                                                    if (vcol != vnew)
                                                    {
                                                        xls_sheet_new.Cells[row, c].Value = vcol;
                                                        xls_sheet_new.Cells[row, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                        xls_sheet_new.Cells[row, c].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                                    }
                                                }
                                            }

                                            tr = r[21];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                path = "TransactionSummary-Validation" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                xls_file_new.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
            }

            return Json(path, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult PartialViewIndexTrip(string filteryear, string filterrole)
        //{
        //    ViewBag.FilterYear = filteryear;
        //    ViewBag.FilterRole = filterrole;
        //    return View("IndexTrip");
        //}

        //public ActionResult PartialViewIndexCost(string filteryear, string filterrole)
        //{
        //    ViewBag.FilterYear = filteryear;
        //    ViewBag.FilterRole = filterrole;
        //    return View("IndexCost");
        //}
        //public ActionResult PartialViewIndexKm(string filteryear, string filterrole)
        //{
        //    ViewBag.FilterYear = filteryear;
        //    ViewBag.FilterRole = filterrole;
        //    return View("IndexKm");
        //}

        //public ActionResult PartialViewIndexAs(string filteryear, string filterrole)
        //{
        //    ViewBag.FilterYear = filteryear;
        //    ViewBag.FilterRole = filterrole;
        //    return View("IndexAs");
        //}

        //public ActionResult PartialViewIndexLoad(string filteryear)
        //{
        //    ViewBag.FilterYear = filteryear;
        //    return View("IndexLoadFactor");
        //}

        //public ActionResult PartialViewIndexCFP(string filteryear, string filterrole)
        //{
        //    ViewBag.FilterYear = filteryear;
        //    ViewBag.FilterRole = filterrole;
        //    return View("IndexCFP");
        //}

        //public ActionResult PartialViewIndexCev(string filteryear, string filterrole)
        //{
        //    ViewBag.FilterYear = filteryear;
        //    ViewBag.FilterRole = filterrole;
        //    return View("IndexCrashEachVendor");
        //}
        public ActionResult GetDatasResult(string filtertab, string filterrole, string filtersort, string filteryear)
        {
            var viewModel = Mapper.Map<List<FnTransportationSummaryViewModel>>(_transportSummaryBLL.GetSummaryVendor(filtertab,filterrole, filtersort, filteryear));
            var serializeModel = JsonConvert.SerializeObject(viewModel);
            var jsonResult = Json(serializeModel, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }

        public ActionResult GetReclassResult(int filterYear)
        {
            var dbResult = _transportSummaryBLL.GetReclass(filterYear);
            var viewModel = Mapper.Map<List<TransportSummaryReclassViewViewModel>>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCRateResult(string filterYear)
        {
            var dbResult = _transportSummaryBLL.GetCrashRate(filterYear).OrderBy(x => x.RoW);
            var viewModel = Mapper.Map<List<FnTransportationSummaryCRateViewModel>>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDataAvg(string filterYear)
        {
            var dbResult = _transportSummaryBLL.GetRecordsAvg(Convert.ToInt32(filterYear));
            var viewModel = Mapper.Map<List<KPILoadFactorAvgViewModel>>(dbResult);
            var output = Json(viewModel, JsonRequestBehavior.AllowGet);

            output.MaxJsonLength = int.MaxValue;

            return output;
        }

        public ActionResult GenerateChartsResult(string filtertab, string filterrole, string filtersort, string filteryear)
        {
            var dbResult = filtertab == "TRIP" ? _transportSummaryBLL.GenerateChart(filtertab, filterrole, filtersort, filteryear) : _transportSummaryBLL.GenerateChartDecimal(filtertab, filterrole, filtersort, filteryear);
            var viewModel = Mapper.Map<FnTransportationSummaryViewModel>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet); ;
        }

    }
}