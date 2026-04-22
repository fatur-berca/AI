using System.Linq;
using System.Web.Mvc;
using DFIS.Universal.Domain.Outputs;
using TOM.Transport.BusinessLogics;
using System.Collections.Generic;
using DFIS.Utils;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal;
using System;
using AutoMapper;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.DTOs;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using hms_tom_dev.Models.Transport;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.BusinessLogics;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Helper;
using System.Data;
using DFIS.Contracts;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal.Domain.Inputs;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Web.Configuration;

namespace hms_tom_dev.Controllers
{
    public class TransportationOrderController : BaseController
    {
        private readonly ITransportOrderBLL _transportOrderBLL;
        private readonly IMasterLocationBLL _bllLocation;
        private readonly IMasterListBLL _bllList;
        private readonly IMasterUomBLL _bllUoM;
        private readonly IMasterCostCenterAccountBLL _bllCostCenter;
        private readonly ICustomReportStateBLL _bllCustomReport;
        private readonly IMasterUserLocationMappingBLL _bllUserLoc;
        private readonly IMasterMappingBLL _bllMstMapping;
        private readonly IMasterUserBLL _bllMstUser;
        public TransportationOrderController(IMasterUserBLL bllMstUser,IMasterMappingBLL bllMstMapping, IMasterUserLocationMappingBLL bllUserLoc, ICustomReportStateBLL bllCustomReport, IMasterCostCenterAccountBLL bllCostCenter, IMasterUomBLL bllUoM, IMasterListBLL bllList, ITransportOrderBLL transportOrderBLL, IMasterLocationBLL bllLocation)
        {
            _bllCustomReport = bllCustomReport;
            _transportOrderBLL = transportOrderBLL;
            _bllLocation = bllLocation;
            _bllList = bllList;
            _bllUoM = bllUoM;
            _bllCostCenter = bllCostCenter;
            _bllUserLoc = bllUserLoc;
            _bllMstMapping = bllMstMapping;
            _bllMstUser = bllMstUser;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportationOrder));
        }
        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.ListUserRole = GetListUserRole();
            TransportOrderRequestViewModel viewModel = new TransportOrderRequestViewModel();
            List<SelectListItem> tempZoneList = _bllList.GetMasterListByFieldName("Zone").Select(a => new SelectListItem { Text = a, Value = a }).ToList();
            viewModel.zone = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.zone.AddRange(tempZoneList);
            List<MasterListDTO> tempMasterList = _transportOrderBLL.getMasterList();
            List<SelectListItem> tempOrderTypeList = tempMasterList.Where(x => x.FieldName == "OrderType").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.orderType = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.orderType.AddRange(tempOrderTypeList);
            List<SelectListItem> tempVehicleTypeList = tempMasterList.Where(x => x.FieldName == "VehicleType").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.vehicleType = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.vehicleType.AddRange(tempVehicleTypeList);
            List<SelectListItem> tempMaterialTypeList = tempMasterList.Where(x => x.FieldName == "MaterialType").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.materialType = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.materialType.AddRange(tempMaterialTypeList);
            List<SelectListItem> tempTransportOrderStatusList = tempMasterList.Where(x => x.FieldName == "TransportationStatus").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            //List<SelectListItem> tempOrderStatusList = tempMasterList.Where(x => x.FieldName == "OrderStatus").Select(a => new SelectListItem { Text = a.FieldValue, Value = a.FieldValue }).ToList();
            viewModel.orderStatus = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            //viewModel.orderStatus.AddRange(tempOrderStatusList);
            viewModel.orderStatus.AddRange(tempTransportOrderStatusList);
            bool IsRoleTransport = GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA));
            List<MasterLocationDTO> tempMasterLocation = _transportOrderBLL.getMasterLocation(IsRoleTransport, GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList());
            List<SelectListItem> tempLocationList = _bllLocation.GetByConfig("TOMLocationFilter").Select(c => new KeyValuePair<string, string>(c.IDLocation, c.LocationName)).Select(a => new SelectListItem { Text = a.Value, Value = a.Key }).ToList();
            viewModel.location = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
            viewModel.location.AddRange(tempLocationList);
            return View(viewModel);
        }

        public ViewResult AddNew()
        {
            ViewBag.ListUserRole = GetListUserRole();
            return View("AddNew");
        }
        public ViewResult Edit(string reqno = null)
        {
            var validRoles = new string[] {
                EnumHelper.GetDescription(DFIS.Utils.Enums.RoleUserList.AWR),
                EnumHelper.GetDescription(DFIS.Utils.Enums.RoleUserList.ATR),
                EnumHelper.GetDescription(DFIS.Utils.Enums.RoleUserList.CUSTOMER),
                EnumHelper.GetDescription(DFIS.Utils.Enums.RoleUserList.SA)
            };

            // Stop Caching in IE
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            // Stop Caching in Firefox
            Response.Cache.SetNoStore();

            ViewBag.RequestID = reqno;
            ViewBag.IsTransport = GetListUserRole().Where(c => validRoles.Where(r => c.RoleName.Contains(r)).Count() > 0).Count() > 0;
            ViewBag.CanSetParentSTO = GetListUserRole().Where(c => c.RoleName != EnumHelper.GetDescription(Enums.RoleUserList.CUSTOMER)).Count() > 0;
            return View("Edit");
        }

        #region Custom Report Save/Load
        string CustomReportPageName = "TransportationOrder";
        public ActionResult LoadCustomReport()
        {
            // Stop Caching in IE
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            // Stop Caching in Firefox
            Response.Cache.SetNoStore();
            var crep = _bllCustomReport.GetSavedLayouts(GetUserId(), CustomReportPageName);

            var col = crep.Select(c => new { FieldName = c.FieldName.Split(','), IsActive = c.IsActive, LayoutName = c.LayoutName }).ToArray();
            return Json(col, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SelectCustomReport(string name)
        {
            var val = _bllCustomReport.SelectLayout(GetUserId(), CustomReportPageName, name);
            return Json(val, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveCustomReport(string name, string[] layout)
        {
            var dto = new CustomReportStateDTO()
            {
                FieldName = string.Join(",", layout),
                LayoutName = name
            };
            return Json(new NonQueryResult(
                _bllCustomReport.SaveLayouts(GetUserId(), CustomReportPageName, dto)
                ));
        }
        #endregion

        #region Upload TO
        [HttpPost]
        public ActionResult UploadTO()
        {
            Exception inex = null;
            if (Request.Files.Count <= 0) return Json(new NonQueryResult(false, "No file specified."));
            var eih = new ExcelImportHelper();
            eih.Map("Vehicle", "Vehicle");
            eih.Map("Zone", "Zone");
            eih.Map("Vehicle Type", "VehicleType");
            eih.Map("Shipment Date", "ShippingDate");
            eih.Map("Sender", "Sender");
            eih.Map("Receiver", "Receiver");
            eih.Map("Order Remark", "OrderRemark");
            eih.Map("Order Status", "OrderStatus");
            eih.Map("Material Type", "MaterialType"); 
            eih.Map("Material Code", "MaterialCode");
            eih.Map("Material Description", "MaterialDescription");
            eih.Map("Quantity", "Quantity");
            eih.Map("UoM", "UoM");
            

            // prepare validation data

            var mats = _bllList.GetMasterListByFieldName("MaterialType");
            var uoms = new Dictionary<string, List<string>>();
            foreach (var mat in mats)
            {
                uoms.Add(mat, _bllUoM.GetMasterUom(new MasterUomInput() { MaterialType = mat }).Select(c => c.UoM).ToList());
            }

            var Location = _bllLocation.GetByConfig("TOMLocationFilter").Select(c => new KeyValuePair<string, string>(c.LocationName, c.IDLocation)).ToDictionary(c => c.Key, c => c.Value);
            var VehicleType = _bllList.GetMasterListByFieldName("VehicleType");
            var Zone = _bllList.GetMasterListByFieldName("Zone");
            var OrderType = _bllList.GetMasterListByFieldName("OrderType");
            var OrderStatus = new List<string>() { "Draft", "Submit" };
            
            string errmes = "";
            List<TransportOrderUploadInput> res = new List<TransportOrderUploadInput>();
            var file = Request.Files.Get(0);
            try
            {
                var ds = ExcelImportHelper.ImportExcelAsDataSet(file.InputStream, file.FileName.ToLower().EndsWith(".xlsx"));

                var sheets = new List<TransportOrderRequestData>();
                List<string> errmsg = new List<string>();
                foreach (DataTable table in ds.Tables)
                {
                    if (table.TableName == "Information") continue;
                    var excelRes = eih.Deserialize<TransportOrderUploadInput>(table, c =>
                    {
                        // Set create and update
                        if (c.Vehicle <= 0) throw new RowValidationByFieldException("Vehicle");

                        if (string.IsNullOrWhiteSpace(c.Zone)) throw new RowValidationByFieldException("Zone");
                        if (!Zone.Contains(c.Zone)) throw new RowValidationByFieldException("Zone", c.Zone);

                        if (string.IsNullOrWhiteSpace(c.VehicleType)) throw new RowValidationByFieldException("Vehicle Type");
                        if (!VehicleType.Contains(c.VehicleType)) throw new RowValidationByFieldException("Vehicle Type", c.VehicleType);

                        if (string.IsNullOrWhiteSpace(c.Sender)) throw new RowValidationByFieldException("Sender");
                        if (!Location.ContainsKey(c.Sender)) throw new RowValidationByFieldException("Sender", c.Sender);

                        if (string.IsNullOrWhiteSpace(c.Receiver)) throw new RowValidationByFieldException("Receiver");
                        if (!Location.ContainsKey(c.Receiver)) throw new RowValidationByFieldException("Receiver", c.Receiver);

                        if (string.IsNullOrWhiteSpace(c.OrderStatus)) throw new RowValidationByFieldException("Order Status");
                        if (!OrderStatus.Contains(c.OrderStatus)) throw new RowValidationByFieldException("Order Status", c.OrderStatus);

                        if (string.IsNullOrWhiteSpace(c.MaterialType)) throw new RowValidationByFieldException("Material Type");
                        if (!mats.Contains(c.MaterialType)) throw new RowValidationByFieldException("Material Type", c.MaterialType);

                        if (string.IsNullOrWhiteSpace(c.MaterialCode)) throw new RowValidationByFieldException("Material Code");

                        if (string.IsNullOrWhiteSpace(c.MaterialDescription)) throw new RowValidationByFieldException("Material Description");

                        if (c.Quantity <= 0) throw new RowValidationByFieldException("Quantity");

                        if (string.IsNullOrWhiteSpace(c.UoM)) throw new RowValidationByFieldException("UoM");
                        if (!uoms[c.MaterialType].Contains(c.UoM)) throw new RowValidationByFieldException("UoM", c.UoM);

                        return c;
                    });
                    
                    bool allValid = true;
                    errmsg.AddRange(excelRes.Where(c => !c.Valid).Select(c => table.TableName + ": Row #" + c.RowNumber + " - " + c.ErrorMessage).ToList());
                    if (errmsg.Count <= 0)
                    {
                        // translate to TransportOrderRequestData
                        TransportOrderRequestData data = new TransportOrderRequestData();
                        data.Units = new List<TransportOrderUnitInput>();
                        TransportOrderUnitInput unit = null;
                        TransportOrderRouteInput route = null;
                        TransportOrderMaterialInput mate = null;
                        int vehiID = 0;                        
                        var MaterialTypeBefore = "";

                        var DistinctMaterialType = excelRes.GroupBy(x => x.Result.MaterialType).ToList();
                        if (DistinctMaterialType.Count > 1 && (DistinctMaterialType.Where(y => y.Key.Contains("Cigarette")).ToList().Count > 0 || DistinctMaterialType.Where(z => z.Key.Contains("Market Return")).ToList().Count > 0))
                        {
                            int xyz = 0;
                            String x = "Cannot upload Mixed Material Type : ";
                            foreach (var i in DistinctMaterialType)
                            {
                                if (xyz < DistinctMaterialType.Count()-1)
                                {
                                    x = x + i.Key;
                                    x = x + " + ";
                                }
                                else
                                {
                                    x = x + i.Key;
                                }
                                xyz++;
                            }
                            errmsg.Add(x);
                        }

                        foreach (var c in excelRes)
                        {
                            var row = c.Result;

                            bool newVehi = false;
                            if (unit == null || row.Vehicle != vehiID)
                            {
                                unit = new TransportOrderUnitInput();
                                unit.Routes = new List<TransportOrderRouteInput>();
                                unit.ShipmentDate = row.ShippingDate.Date;
                                unit.VehicleType = row.VehicleType;
                                unit.Zone = row.Zone;
                                vehiID = row.Vehicle;
                                ///MaterialTypeBefore = row.MaterialType;                                
                                
                                data.Units.Add(unit);
                                newVehi = true;
                            }
                            else
                            {
                                // check ship date, vehi type, and zone must be same as current vehi
                                if (row.Zone != unit.Zone)
                                {
                                    errmsg.Add(table.TableName + ": Row #" + c.RowNumber + " - Inconsistent Zone (previously [" + unit.Zone + "], currently [" + row.Zone + "])");
                                }
                                if (row.ShippingDate.Date != unit.ShipmentDate)
                                {
                                    errmsg.Add(table.TableName + ": Row #" + c.RowNumber + " - Inconsistent Shipping Date (previously [" + unit.ShipmentDate.ToString("yyyy-MM-dd") + "], currently [" + row.ShippingDate.ToString("yyyy-MM-dd") + "])");
                                }
                                if (row.VehicleType != unit.VehicleType)
                                {
                                    errmsg.Add(table.TableName + ": Row #" + c.RowNumber + " - Inconsistent Vehicle Type (previously [" + unit.VehicleType + "], currently [" + row.VehicleType + "])");
                                }
                            }
                            if (route == null || newVehi || route.Sender != Location[row.Sender] || route.Receiver != Location[row.Receiver] || route.OrderStatus != row.OrderStatus)
                            {
                                route = new TransportOrderRouteInput();
                                route.Materials = new List<TransportOrderMaterialInput>();
                                route.OrderRemark = row.OrderRemark;
                                route.Sender = Location[row.Sender];
                                route.Receiver = Location[row.Receiver];
                                route.Checked = true;
                                route.OrderStatus = row.OrderStatus;
                                var ot = _transportOrderBLL.GetOrderType(row.MaterialType);
                                route.OrderType = ot != null ? ot.MapTo : "";

                                unit.Routes.Add(route);
                            }

                            //if (route.OrderType == "Finished Good" || route.OrderType == "Bad Stock")
                            //    errmsg.Add(table.TableName + ": Row #" + c.RowNumber + " -  Cannot upload order with Order Type: " + route.OrderType + " or Material Type: " + row.MaterialType);

                            mate = new TransportOrderMaterialInput();
                            mate.MaterialType = row.MaterialType;
                            mate.Code = row.MaterialCode;
                            mate.Description = row.MaterialDescription;
                            mate.Quantity = (decimal)row.Quantity;
                            mate.UoM = row.UoM;

                           /* if ((row.MaterialType == "Cigarette" && MaterialTypeBefore != "Cigarette") || (row.MaterialType == "Material Return" && MaterialTypeBefore != "Material Return"))
                                errmsg.Add("Cannot upload Material Type:" + MaterialTypeBefore + " and Material Type: " + row.MaterialType);
                                */
                            route.Materials.Add(mate);
                        }

                        if (data.Units.Count > 10)
                            errmsg.Add("Maximum number of vehicle exceeded (" + data.Units.Count + " of 10)");

                        sheets.Add(data);
                    }
                    
                }
                if (errmsg.Count <= 0)
                {
                    var reqNo = _transportOrderBLL.SaveData(sheets, GetUserId());
                    if (reqNo == null)
                        errmsg.Add("Failed to upload data!");
                }
                if (errmsg.Count > 0)
                    errmes = "&bull; " + string.Join("<br>&bull; ", errmsg);

            }
            catch (Exception ex)
            {
                inex = ex;
            }
            if (errmes == "" && inex == null)
                return Json(new NonQueryResult(true) { Data = res });
            if (inex != null)
                return Json(new NonQueryResult(false, inex, "Failed to upload data!"));
            return Json(new NonQueryResult(false, errmes));
        }

        #endregion

        [HttpPost]
        public ActionResult Save(TransportOrderRequestData data)
        {
            Sinidong:
            try
            {
                // check for inconsistency of order status
                List<string> _invToOvr = new List<string>();
                List<string> _dupeSTO = new List<string>();
                foreach(var unit in data.Units)
                {
                    foreach(var route in unit.Routes)
                    {
                        var safetoOW = _transportOrderBLL.IsSafeToOverwrite(route.OrderNumber, route.OrderStatus);
                        route.WantRefresh = !safetoOW && !route.Checked;
                        if (route.Checked)
                        {
                            if (!safetoOW)
                                _invToOvr.Add(route.OrderNumber);

                            var unique = _transportOrderBLL.IsSTONoExists(route.OrderNumber, route.IDTransportOrder);
                            if (unique)
                                _dupeSTO.Add(route.OrderNumber);
                        }                        
                    }
                }
                if (_dupeSTO.Count > 0)
                {
                    return Json(new { ValidationFailed = true, Error = "STONotUnique", Data = _dupeSTO });
                }
                if (_invToOvr.Count > 0)
                {
                    return Json(new { ValidationFailed = true, Error = "StatusInconsistent", Data = _invToOvr });
                }

                _transportOrderBLL.BaseUri = WebConfigurationManager.AppSettings["WebRootUrl"];
                if (_transportOrderBLL.BaseUri != null && !_transportOrderBLL.BaseUri.EndsWith("/"))
                    _transportOrderBLL.BaseUri += "/";
                
                var reqNo = _transportOrderBLL.SaveData(data, GetUserId());

                foreach(var unit in data.Units)
                {
                    foreach(var route in unit.Routes)
                    {
                        if (route.WantRefresh)
                        {
                            // refresh route data
                            var to = _transportOrderBLL.Get(new TransportOrderInput() { IDTransportOrder = route.IDTransportOrder }).FirstOrDefault();
                            if (to != null)
                            {
                                route.OrderStatus = to.OrderStatus;
                                route.OrderRemark = to.Remarks;
                                route.OrderNumber = to.STONo;
                                route.OrderType = to.OrderType;
                            }
                        }
                    }
                }

                return Json(new { ReqNo = reqNo, Data = data.Units });
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("TransportationOrder", ex);
                if (ex.InnerException.InnerException.Message.Contains("duplicate"))
                {
                    goto Sinidong;   
                }
                return Json(new NonQueryResult(false, ex));
            }
        }

        public ActionResult CheckSTO(List<TransportOrderSTOCheckInput> stodic)
        {
            List<string> _errs = new List<string>();
            foreach (var itm in stodic)
            {
                if (string.IsNullOrWhiteSpace(itm.STONo)) continue;
                var cnt = _transportOrderBLL.IsSTONoExists(itm.STONo, itm.IDTransportOrder);
                if (!cnt)
                    _errs.Add( itm.Tag + ": " + itm.STONo);
            }
            

            return Json(new { IsSuccess = _errs.Count <= 0, Errors = _errs.ToArray() });
        }
        
        public ActionResult GetCostCenter(string sender = null, string receiver = null, string materialType = null, DateTime? date = null)
        {
            MasterCostCenterAccountInput inp = new MasterCostCenterAccountInput()
            {
                SenderIDLocation = sender,
                ReceiverIDLocation = receiver,
                MaterialType = materialType,
                filterShipmentDate = date
            };
            var cc = _bllCostCenter.GetMasterCostCenterAccountBySenderReceiverMaterial(inp);
            if (cc != null)
                return Json(cc.CostCenter, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadMaterial()
        {
            if (Request.Files.Count <= 0) return Json(new NonQueryResult(false, "No file specified."));
            var eih = new ExcelImportHelper();
            eih.Map("Material Type", "MaterialType");
            eih.Map("Material Code", "Code");
            eih.Map("Material Description", "Description"); 
            eih.Map("Quantity", "Quantity");
            eih.Map("UoM", "UoM");
            // TransportOrderRouteInput
            string errmes = "";
            List<TransportOrderMaterialInput> res = new List<TransportOrderMaterialInput>();
            var mats = _bllList.GetMasterListByFieldName("MaterialType");
            var uoms = new Dictionary<string, List<string>>();
            foreach (var mat in mats)
            {
                uoms.Add(mat, _bllUoM.GetMasterUom(new MasterUomInput() { MaterialType = mat }).Select(c => c.UoM).ToList());
            }
            var file = Request.Files.Get(0);
            try
            {
                var ds = ExcelImportHelper.ImportExcelAsDataSet(file.InputStream, file.FileName.ToLower().EndsWith(".xlsx"));

                var col = new List<ExcelImportResult<TransportOrderMaterialInput>>();
                foreach (DataTable table in ds.Tables)
                {
                    if (table.TableName == "Information") continue;
                    var excelRes = eih.Deserialize<TransportOrderMaterialInput>(table, c =>
                    {
                        // Set create and update
                        if (string.IsNullOrWhiteSpace(c.Code)) throw new RowValidationByFieldException("Material Code");
                        if (string.IsNullOrWhiteSpace(c.MaterialType)) throw new RowValidationByFieldException("Material Type");
                        if (!mats.Contains(c.MaterialType)) throw new RowValidationByFieldException("Material Type", c.MaterialType);
                        if (c.Qty <= 0) throw new RowValidationByFieldException("Quantity");
                        if (string.IsNullOrWhiteSpace(c.UoM)) throw new RowValidationByFieldException("UoM");
                        if (!uoms[c.MaterialType].Contains(c.UoM)) throw new RowValidationByFieldException("UoM", c.UoM);

                        return c;
                    });

                    col.AddRange(excelRes);
                }

                bool allValid = true;
                var errmsg = col.Where(c => !c.Valid).Select(c => "Row #" + c.RowNumber + " - " + c.ErrorMessage).ToArray();
                if (errmsg.Length <= 0)
                {
                    res.AddRange(col.Select(c => c.Result));
                }
                else
                    errmes = "&bull; " + string.Join("<br>&bull; ", errmsg);
            }
            catch { }
            return Json(new NonQueryResult(errmes == "", errmes) { Data = res });
        }

        private bool CanAccessData(string creator, int? isActive)
        {
            return CanAccessData(new SP_GetTransportOrderRequestDTO() { CreatedBy = creator, IsActive = isActive });
        }
        private bool CanAccessData(SP_GetTransportOrderRequestDTO data)
        {
            if (data == null) return true;
            var roles = GetListUserRole();
            if (roles.Where(c => c.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA)).Count() > 0)
                return true;
            if (roles.Where(c => c.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR)).Count() > 0)
                return data.IsActive == 1;

            if (roles.Where(c => c.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.AWR)).Count() > 0)
            {
                // get region of user
                var userRegion = GetListLocation().Select(c => c.IDLocation).ToList();
                var recordCreator = data.CreatedBy;
                try
                {
                    var ulocdto = _bllUserLoc.GetById(recordCreator);
                    var recordRegion = ulocdto == null ? new List<string>() : ulocdto.ListIDLocation;
                    return (recordRegion.Where(u => userRegion.Contains(u)).Count() > 0) && data.IsActive == 1;
                }
                catch (Exception ex) {
                    return data.IsActive == 1;
                }
            }
            else
            {
                // a customer
                return data.CreatedBy == GetUserId() && data.IsActive == 1;
            }
        }

        public ActionResult Load(string reqno)
        {
            List<TransportOrderUnitInput> units = new List<TransportOrderUnitInput>();
            var reqs = _transportOrderBLL.GetRequests(reqno, null, true);
            var ireq = _transportOrderBLL.GetRequests(reqno, null, false);

            if (reqs.Where(r => CanAccessData(r.CreatedBy, r.IsActive ? 1 : 0)).Count() <= 0 && ireq.Where(r => CanAccessData(r.CreatedBy, r.IsActive ? 1 : 0)).Count() <= 0)
                throw new AccessViolationException("User does not have access to this data!");

            foreach (var u in reqs)
            {
                var unit = MappingHelper.Map<TransportOrderUnitInput>(u);
                unit.Routes = new List<TransportOrderRouteInput>();
                foreach (var r in u.TransportOrders)
                {
                    if (!r.IsActive) continue;
                    var route = MappingHelper.Map<TransportOrderRouteInput>(r);
                    route.Materials = new List<TransportOrderMaterialInput>();
                    foreach (var m in r.TransportOrderDetails)
                    {
                        if (m.IsActive)
                        {
                            var mate = MappingHelper.Map<TransportOrderMaterialInput>(m);
                            mate.Quantity = m.Qty == null ? 0 : (decimal)m.Qty;
                            route.Materials.Add(mate);
                        }
                    }
                    route.Sender = r.SenderIDLocation;
                    route.Receiver = r.ReceiverIDLocation;
                    route.OrderNumber = r.STONo;
                    route.OrderRemark = r.Remarks;
                    route.SeqNo = r.SeqNo;
                    route.IDCostCenter = r.IDCostCenter;
                    route.Saved = true;
                    unit.Routes.Add(route);
                }
                units.Add(unit);
            }
            
            // Stop Caching in IE
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            // Stop Caching in Firefox
            Response.Cache.SetNoStore();

            return Json(new { Units = units, AllowEdit = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStoNoFilter(string term)
        {
            bool IsRoleTransport = GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.CUSTOMER));
            var data = _transportOrderBLL.GetSTONoFilterByRegion(term, IsRoleTransport, GetUserRegionSelectList().Select(x => new string(x.Value.ToCharArray())).ToList());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // ReqNo, Zone, Vehicle Type, Shipment Date, OrderNo, Sender, Receiver, Order Type, Order Status. 
        //Order Remark, Material Type, Material Code, Material Description, Quantity, UoM, Order Creator, Created Date, Created By

        public ActionResult CustomExportTO(string[] cols, TransportOrderRequestInput filter)
        {
            var defMap = new Dictionary<string, string>();
            defMap.Add("RequestNo", "Request Number");
            defMap.Add("Zone", "Zone");
            defMap.Add("VehicleType", "Vehicle Type");
            defMap.Add("ShipmentDate", "Shipment Date");
            defMap.Add("OrderNo", "Order No");
            defMap.Add("Sender", "Sender");
            defMap.Add("Receiver", "Receiver");
            defMap.Add("OrderType", "Order Type");
            defMap.Add("OrderStatus", "Order Status");
            defMap.Add("OrderRemark", "Order Remark");
            defMap.Add("MaterialType", "Material Type");
            defMap.Add("MaterialDescription", "Material Description");
            defMap.Add("Code", "Material Code");
            defMap.Add("Qty", "Quantity");
            defMap.Add("UoM", "UoM");
            defMap.Add("CostCenter", "Cost Center");
            defMap.Add("CreatedBy", "Order Creator");
            defMap.Add("CreatedDate", "Request Date");
            defMap.Add("TotalVehicle", "Total Vehicle");
            defMap.Add("SequenceNumber", "Sequence Number");

            bool isNormalExport = false;
            if (cols == null || cols.Length == 0)
            {
                cols = new string[] { "Request Number", "Request Date", "Total Vehicle", "Order Creator" };
                isNormalExport = true;
            }
            else
            {
                var rlCol = new List<string>();
                foreach(var col in cols)
                {
                    if (defMap.ContainsKey(col))
                        rlCol.Add(defMap[col]);
                }
                cols = rlCol.ToArray();
            }

            if (GetListUserRole().Any(x =>
            x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) ||
            x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA)))
                filter.IsRoleTransport = true;
            filter.userRegionList = GetUserRegionSelectList().Select(x => x.Value).ToList();
            filter.CreatedBy = GetUserId();
            var rawData = _transportOrderBLL.GetListViewTransportationOrder(filter).Where(c => c.IsActive == 1 && CanAccessData(c)).OrderBy(od => od.RequestNo).Reverse();
            
            var reqs = rawData.Select(c => c.RequestNo).Distinct();

            // make dtos
            var data = new List<TransportOrderExcelExportDTO>();
            foreach(var rn in reqs)
            {
                List<TransportOrderRequestDTO> torList = _transportOrderBLL.GetTransportOrderRequestById(null, rn);
                foreach (TransportOrderRequestDTO tor in torList)
                {
                    if (!isNormalExport)
                    {
                        foreach (var to in tor.TransportOrders)
                        {
                            if (to.IsActive)
                            {
                                foreach (var tod in to.TransportOrderDetails)
                                {
                                    if (tod.IsActive)
                                    {
                                        var sender = _bllLocation.GetById(to.SenderIDLocation).LocationName;
                                        var recv = _bllLocation.GetById(to.ReceiverIDLocation).LocationName;
                                        var nd = new TransportOrderExcelExportDTO()
                                        {
                                            Code = tod.Code,
                                            CreatedDate = tor.CreatedDate ?? DateTime.MinValue,
                                            MaterialType = tod.MaterialType,
                                            MaterialDescription = tod.Description,
                                            OrderNo = to.STONo,
                                            OrderRemark = to.Remarks,
                                            OrderStatus = to.OrderStatus,
                                            OrderType = to.OrderType,
                                            Qty = tod.Qty ?? 0,
                                            Receiver = recv,
                                            RequestNo = tor.RequestNo,
                                            Sender = sender,
                                            ShipmentDate = to.ShipmentDate,
                                            UoM = tod.UoM,
                                            VehicleType = to.VehicleType,
                                            Zone = tor.Zone,
                                            TotalVehicle = tor.TotalUnit,
                                            SequenceNumber = to.SeqNo,
                                            CostCenter = to.CostCenter
                                        };
                                        if (!string.IsNullOrWhiteSpace(tor.CreatedBy))
                                        {
                                            var usr = _bllMstUser.GetMasterUsers(new MasterUserInput() { IDUser = tor.CreatedBy }).FirstOrDefault();
                                            if (usr != null)
                                                nd.CreatedBy = usr.FullName;
                                        }
                                        data.Add(nd);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (data.Where(c => c.RequestNo == tor.RequestNo).Count() > 0)
                            continue;
                        var nd = new TransportOrderExcelExportDTO()
                        {
                            CreatedDate = tor.CreatedDate ?? DateTime.MinValue,
                            RequestNo = tor.RequestNo,
                            Zone = tor.Zone,
                            TotalVehicle = tor.TotalUnit
                        };
                        if (!string.IsNullOrWhiteSpace(tor.CreatedBy))
                        {
                            var usr = _bllMstUser.GetMasterUsers(new MasterUserInput() { IDUser = tor.CreatedBy }).FirstOrDefault();
                            if (usr != null)
                                nd.CreatedBy = usr.FullName;
                        }
                        data.Add(nd);
                    }
                }
            }
            
            try
            {
                using (ExcelExportHelper eeh = new ExcelExportHelper())
                {
                    TransportOrderRequestSummaryDTO a;
                    foreach (var k in defMap.Keys)
                        eeh.Map(defMap[k], k);
                    eeh.SetHeaderDisplay(cols);

                    eeh.HeaderBackgroundColor = Color.LightGray;
                    eeh.HeaderForegroundColor = Color.Black;

                    var ws = eeh.CreateWorksheet("Export");

                    eeh.WriteHeader(ws);

                    // write values
                    eeh.WriteRows(ws, data, (dto, cell, pi) =>
                    {
                        var val = pi.GetValue(dto);
                        if (pi.Name == "CreatedDate") cell.Style.Numberformat.Format = "dd MMM yyyy";
                        if (pi.Name == "ShipmentDate") cell.Style.Numberformat.Format = "dd MMM yyyy";
                        return val;
                    });
                    eeh.AutoSizeColumns(ws);
                    var path = "TransportationOrder_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    eeh.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
                    return Json(path);
                }
            }
            catch(Exception ex)
            { }

            return Json(new NonQueryResult(false, "Failed to export data"));
        }

        public ActionResult ExportTO(TransportOrderRequestInput filter)
        {
            var defMap = new Dictionary<string, string>();
            defMap.Add("RequestNo", "Request Number");
            defMap.Add("TotalVehicle", "Total Vehicle");
            defMap.Add("CreatedBy", "Order Creator");
            defMap.Add("CreatedDate", "Request Date");
            
            var cols = defMap.Keys.ToArray();

            if (GetListUserRole().Any(x =>
            x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) ||
            x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA)))
                filter.IsRoleTransport = true;
            filter.userRegionList = GetUserRegionSelectList().Select(x => x.Value).ToList();
            filter.CreatedBy = GetUserId();
            var rawData = _transportOrderBLL.GetListViewTransportationOrder(filter).Where(c => c.IsActive == 1 && CanAccessData(c)).OrderBy(od => od.RequestNo).Reverse();

            foreach(var rd in rawData)
            {
                if (!string.IsNullOrWhiteSpace(rd.CreatedBy))
                {
                    var usr = _bllMstUser.GetMasterUsers(new MasterUserInput() { IDUser = rd.CreatedBy }).FirstOrDefault();
                    if (usr != null)
                        rd.CreatedBy = usr.FullName;
                    rd.TotalVehicle = _transportOrderBLL.CountVehicle(rd.RequestNo);
                }
            }

            try
            {
                using (ExcelExportHelper eeh = new ExcelExportHelper())
                {
                    TransportOrderRequestSummaryDTO a;
                    foreach (var k in cols)
                        eeh.Map(defMap[k], k);

                    eeh.HeaderBackgroundColor = Color.LightGray;
                    eeh.HeaderForegroundColor = Color.Black;

                    var ws = eeh.CreateWorksheet("Export");

                    eeh.WriteHeader(ws);

                    // write values
                    eeh.WriteRows(ws, rawData, (dto, cell, pi) =>
                    {
                        var val = pi.GetValue(dto);
                        if (pi.Name == "CreatedDate") cell.Style.Numberformat.Format = "dd MMM yyyy";
                        return val;
                    });
                    eeh.AutoSizeColumns(ws);
                    var path = "TransportationOrder_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    eeh.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
                    return Json(path);
                }
            }
            catch { }

            return Json(new NonQueryResult(false, "Failed to export data"));
        }

        public ActionResult DownloadTemplateMaterial()
        {
            try
            {
                // Generate import template headers
                string[] TemplateHeaders = new string[]
                {
                "Material Type", "Material Code", "Material Description", "Quantity", "UoM"
                };

                // Generate information column
                var UOMs =
                _bllUoM.GetMasterUom(new MasterUomInput() { }).GroupBy(k => k.MaterialType).Select(
                    gr =>
                    new KeyValuePair<string, string[]>(
                        gr.FirstOrDefault() == null ? "?" : gr.FirstOrDefault().MaterialType,
                        gr.Select(c => c.UoM).ToArray()
                    )
                ).ToDictionary(ks => ks.Key, el => el.Value);

                // Generate the excel file
                var file = "TransportationOrderMaterialList_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                var path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + file;
                var exc = new ExcelPackage(path == null ? null : new System.IO.FileInfo(path));

                var ws1 = exc.Workbook.Worksheets.Add("Materials");
                for (var i = 1; i <= TemplateHeaders.Length; i++)
                {
                    ws1.SetValue(1, i, TemplateHeaders[i-1]);
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

                var ws2 = exc.Workbook.Worksheets.Add("Information");
                ws2.SetValue(1, 1, "Material Type");
                ws2.SetValue(1, 2, "UoM");

                var row = 2;
                foreach (var mt in UOMs)
                {
                    foreach (var uom in mt.Value)
                    {
                        ws2.SetValue(row, 1, mt.Key);
                        ws2.SetValue(row, 2, uom);
                        row++;
                    }
                }
                using (var range = ws2.Cells[1, 1, 1, 2])
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
            catch(Exception ex)
            {
                return Json(false);
            }
        }

        public ActionResult DownloadTemplateTO()
        {
            try
            {
                // Generate import template headers
                string[] TemplateHeaders = new string[]
                {
                "Vehicle", "Zone", "Vehicle Type", "Shipment Date", "Sender", "Receiver", "Order Status", "Order Remark", "Material Type", "Material Code", "Material Description", "Quantity", "UoM"
                };

                // Generate the excel file
                var file = "TransportationOrderUpload_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                var path = AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + file;
                var exc = new ExcelPackage(path == null ? null : new System.IO.FileInfo(path));

                var ws1 = exc.Workbook.Worksheets.Add("Sheet1");
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
                ws1.Cells[2, 4].Style.Numberformat.Format = "dd-MMM-yyyy";

                // Dummy Data
                object[] dummy = new object[]
                {
                    1,
                    "East",
                    "CBU",
                    DateTime.Today,
                    "TPO Surabaya",
                    "ADW Bandung",
                    "Draft",
                    "",
                    "DIM",
                    "APTX-4869",
                    "",
                    12,
                    "Box"
                };
                for (var i=1; i <=dummy.Length; i++)
                    ws1.SetValue(2, i, dummy[i-1]);

                var ws2 = exc.Workbook.Worksheets.Add("Information");
                ws2.SetValue(1, 1, "Location Name");

                // Generate information column
                var locs =
                _bllLocation.Get(c => c.IsActive && (c.Type == "Factory" || c.Type == "TPO" || c.Type == "Other")).Select(d => d.LocationName).ToList();
                var row = 2;
                foreach (var mt in locs)
                {
                    ws2.SetValue(row, 1, mt);
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
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult GetTransportOrderList(TransportOrderRequestInput filter)
        {
            if (GetListUserRole().Any(x => 
            x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) ||
            x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA)))
                filter.IsRoleTransport = true;
            filter.userRegionList = GetUserRegionSelectList().Select(x => x.Value).ToList();
            filter.CreatedBy = GetUserId();
            var rawData = _transportOrderBLL.GetListViewTransportationOrder(filter).Where(c => CanAccessData(c)).OrderBy(od => od.RequestNo).Reverse();

            foreach (var rd in rawData)
            {
                if (!string.IsNullOrWhiteSpace(rd.CreatedBy))
                {
                    var usr = _bllMstUser.GetMasterUsers(new MasterUserInput() { IDUser = rd.CreatedBy }).FirstOrDefault();
                    if (usr != null)
                        rd.CreatedName = usr.FullName;
                }
            }
            
            foreach (var d in rawData)
                d.TotalVehicle = _transportOrderBLL.CountVehicle(d.RequestNo);
            
            int totalOrderedUnit = rawData.Sum(x => x.TotalVehicle ?? 0);
            //int totalOrderedUnit = rawData.Select(c => c.IDTransportOrder).Distinct().Count();
            int totalInProcess = rawData.Sum(x => x.TotalInProcess ?? 0);
            // int totalInProcess = 0;
            int totalOnDelivery = rawData.Sum(x => x.TotalOnDelivery ?? 0); 
            // int totalOnDelivery = 0;
            int totalComplete = rawData.Sum(x => x.TotalCompleted ?? 0);
            // int totalComplete = 0;
            int totalSubmit = rawData.Sum(x => x.TotalSubmit ?? 0);


            var temp = new { viewModel = rawData, totOrderUnit = totalOrderedUnit, totInProcess = totalInProcess, totOnDeliver = totalOnDelivery, totComplete = totalComplete, totSubmit = totalSubmit };
            return Json(temp);
        }

        public ActionResult GetTransportOrderTableList(TransportOrderRequestInput filter, DataTableModel model)
        {
            #region FILTER
            if (GetListUserRole().Any(x => x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) || x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA)))
                filter.IsRoleTransport = true;
            filter.userRegionList = GetUserRegionSelectList().Select(x => x.Value).ToList();
            filter.CreatedBy = GetUserId();
            if (filter.zoneListFilter.Count > 0)
            {
                filter.zoneListFilter.RemoveAll(_ => String.IsNullOrEmpty(_));
            }
            if (filter.stoNoListFilter.Count > 0)
            {
                filter.stoNoListFilter.RemoveAll(_ => String.IsNullOrEmpty(_));
            }
            if (filter.senderIdLocListFilter.Count > 0)
            {
                filter.senderIdLocListFilter.RemoveAll(_ => String.IsNullOrEmpty(_));
            }
            if (filter.receiverIdLocListFilter.Count > 0)
            {
                filter.receiverIdLocListFilter.RemoveAll(_ => String.IsNullOrEmpty(_));
            }
            if (filter.orderVehicleTypeListFiter.Count > 0)
            {
                filter.orderVehicleTypeListFiter.RemoveAll(_ => String.IsNullOrEmpty(_));
            }
            if (filter.materialTypeListFilter.Count > 0)
            {
                filter.materialTypeListFilter.RemoveAll(_ => String.IsNullOrEmpty(_));
            }
            if (filter.orderStatusListFilter.Count > 0)
            {
                filter.orderStatusListFilter.RemoveAll(_ => String.IsNullOrEmpty(_));
            }
            #endregion

            var rawData = _transportOrderBLL.GetTransportationOrderDataTable(filter, model);
            var result = new
            {
                recordsTotal = rawData.total,
                recordsFiltered = rawData.total,
                draw = model.draw,
                data = rawData.data,
                dbResultCount = rawData.total,
            };
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult SetActiveFalse(List<int> filter)
        {
            _transportOrderBLL.SetActiveTransportOrderRequest(filter, GetUserId());
            return Json("");
        }

        public ActionResult GetList()
        {
            return Json(_transportOrderBLL.GetList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocation()
        {
            return Json(_transportOrderBLL.GetALLMasterLocation(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetData()
        {
            NoCache();
            var mats = _bllList.GetMasterListByFieldName("MaterialType");
            var uoms = new Dictionary<string, List<string>>();
            var matcat = new Dictionary<string, string>();
            foreach (var mat in mats)
            {
                uoms.Add(mat, _bllUoM.GetMasterUom(new MasterUomInput() { MaterialType = mat }).Select(c => c.UoM).ToList());
                var cat = _bllMstMapping.GetMasterMappings(new MasterMappingInput() { MapFrom = mat }).FirstOrDefault();

                matcat.Add(mat, cat == null ? "" : cat.MapTo);
            }
            return Json(new
            {
                Location = _bllLocation.GetByConfig("TOMLocationFilter").Select(c => new KeyValuePair<string, string>(c.IDLocation, c.LocationName)),
                VehicleType = _bllList.GetMasterListByFieldName("VehicleType"),
                Zone = _bllList.GetMasterListByFieldName("Zone"),
                OrderType = _bllList.GetMasterListByFieldName("OrderType"),
                MaterialType = mats,
                MaterialCats = matcat,
                UoMs = uoms,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderTypeCostCenterUom(MasterCostCenterAccountInput criteria)
        {
            //return Json(_transportOrderBLL.GetOrderType(criteria), JsonRequestBehavior.AllowGet);            
            string OrderType = "";
            string CostCenter = "";
            MasterMapping mastermapping = _transportOrderBLL.GetOrderType(criteria.MaterialType);
            if (mastermapping != null)
            {
                OrderType = mastermapping.MapTo;
            }
            MasterCostCenter mstCostCenter = _transportOrderBLL.GetCostCenter(criteria);
            if (mstCostCenter != null)
            {
                CostCenter = mstCostCenter.CostCenter;
            }
            List<MasterUomDTO> listUom = new List<MasterUomDTO>();
            listUom = _transportOrderBLL.GetUomByMaterial(criteria.MaterialType);
            return Json(new { OrderType = OrderType, CostCenter = CostCenter, Uom = listUom }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSpeakingCode(string Code)
        {
            string Description = _transportOrderBLL.GetFABrandByCode(Code);
            return Json(Description, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitDraftRequestUnit(List<TransportOrderRequestInput> saveData)
        {
            List<TransportOrderRequestDTO> listID = new List<TransportOrderRequestDTO>();
            listID = _transportOrderBLL.SaveRequestUnit(saveData, GetUserId());
            return Json(listID);
        }

        public ActionResult DeleteRequest(string[] requestNo)
        {
            bool res = false;
            try
            {
                res = _transportOrderBLL.DeleteRequest(requestNo);
            }
            catch (Exception ex)
            {
                return Json(new NonQueryResult(false, ex.Message));
            }
            return Json(new NonQueryResult(res));
        }

        public ActionResult DeleteUnit(int requestID)
        {
            try
            {
                _transportOrderBLL.DeleteUnit(requestID);
            }
            catch (Exception ex)
            {
                return Json(new NonQueryResult(false, ex.Message));
            }
            return Json(new NonQueryResult(true));
        }
        public ActionResult DeleteUnits(string[] requestIDs)
        {
            try
            {
                _transportOrderBLL.DeleteUnit(requestIDs);
            }
            catch (Exception ex)
            {
                return Json(new NonQueryResult(false, ex.Message));
            }
            return Json(new NonQueryResult(true));
        }

        public ActionResult DeleteRouteUnit(List<int> deleteData)
        {
            try
            {
                if (deleteData != null)
                    _transportOrderBLL.DeleteRoute(deleteData);
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        public ActionResult UpdateOrderStatus(TransportOrderRequestInput input)
        {
            try
            {
                _transportOrderBLL.UpdateOrderStatus(input, GetUserId());
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
    }
}
