using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.BusinessLogics;
using DFIS.Utils.Exceptions;
using DFIS.Universal.Domain.Inputs;
using System.Diagnostics;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.Inputs;
using TOM.Master.Domain.DTOs;
using System.Linq;
using hms_tom_dev.Helper;
using DFIS.Contracts;
using DFIS.Utils;
using DFIS.Universal.Domain.Outputs;

namespace hms_tom_dev.Controllers
{
    public class MstCostController : BaseController
    {
        private readonly IMasterCostBLL _masterCostBLL;
        private readonly IMasterUserRoleBLL _bllUserRoleMapping;
        private readonly IMasterVendorTOMBLL _bllVendor;
        private readonly IMasterListBLL _bllList;
        private readonly IMasterLocationBLL _bllLocation;

        public MstCostController(IMasterLocationBLL bllLocation, IMasterCostBLL masterCostBLL, IMasterUserRoleBLL bllUserRoleMapping, IMasterVendorTOMBLL bllVendor, IMasterListBLL bllList)
        {
            _masterCostBLL = masterCostBLL;
            _bllUserRoleMapping = bllUserRoleMapping;
            _bllVendor = bllVendor;
            _bllList = bllList;
            _bllLocation = bllLocation;            
        }

        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.ListUserRole = GetListUserRole();
            return View();
        }

        public ActionResult MstCostTrip()
        {
            return View("MstCostTrip");
        }

        public ActionResult GetLocation()
        {
            return Json(_bllLocation.GetByConfig("TOMLocationFilter"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetList()
        {
            return Json(_masterCostBLL.GetList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendor(MasterCostInput req)
        {
            var vendors = _masterCostBLL.GetALLVendor();
            var vcat = Request["VendorCategory"];
            if (vcat == "ASDP" || vcat == "SPSI")
                vcat = "KM Based";
            return Json(vendors.FindAll(C => C.VendorCategory == vcat && C.ParentVendor == null),
            JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDropDownType()
        {
            return Json(_masterCostBLL.GetTypeList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCostKM()
        {
            List<string> temp = new List<string>(new[] { "KM", "Box", "SPSI" });
            return Json(_masterCostBLL.GetALLMasterCostByCostType(temp), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllMasterCost(MasterCostInput criteria)
        {
            var viewModel = Mapper.Map<List<MasterCostViewModel>>(_masterCostBLL.GetAllMasterCost(criteria));
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCostTrip()
        {
            List<string> temp = new List<string>(new[] { "Trip", "ASDP" });
            return Json(_masterCostBLL.GetALLMasterCostByCostType(temp), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertList(InsertUpdateData<MasterCostViewModel> bulkData)
        {
            int result = 0;
            try
            {
                if (bulkData.New != null)
                {
                    var mstCost = Mapper.Map<MasterCostDTO>(bulkData.New[0]);
                    mstCost.CreatedBy = GetUserId();
                    mstCost.CreatedDate = DateTime.Now;
                    mstCost.UpdatedBy = GetUserId();
                    _masterCostBLL.InsertOrUpdate(mstCost, false);
                }
                else
                {
                    var mstCost = Mapper.Map<MasterCostDTO>(bulkData.Edit[0]);
                    mstCost.UpdatedBy = GetUserId();
                    _masterCostBLL.InsertOrUpdate(mstCost, true);
                }
            }
            catch (Exception ex)
            {
                return Json(
                    new NonQueryResult(false, ex.Message)
                );
            }
            return Json(
                new NonQueryResult(true)
                );
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            // Auth checking
            if (!_bllUserRoleMapping.UserHasRole(GetUserId(), 1, 5, 7))
                return Json(new NonQueryResult(false, "You are not authorized to do this action."));

            // Setting map header excel to field name (DTO)
            var eih = new ExcelImportHelper();
            eih.Map("Cost Type", "CostType");
            eih.Map("Vendor Name", "VendorName");
            eih.Map("Vehicle Type", "VehicleType");
            eih.Map("Order Type", "OrderType");
            eih.Map("Sender", "SenderIDLocation");
            eih.Map("Through", "ThroughIDLocation");
            eih.Map("Receiver", "ReceiverIDLocation");
            eih.Map("Via", "Via");

            eih.Map("Minimum KM", "MinimumKM");
            eih.Map("Minimum Box", "MinimumBox");
            eih.Map("Based Price", "BasedPrice");
            eih.Map("Discount Price", "DiscountPrice");
            eih.Map("Additional Unit Price", "AdditionalUnitPrice");

            eih.Map("Effective Start Date", "EffectiveStartDate");

            // prepare data for validation
            var _listCostType = _bllList.GetMasterListByFieldName("DataTypeMstCost");
            var _listVia = _bllList.GetMasterListByFieldName("ViaRoute");
            var _listLocation = _bllLocation.GetByConfig("TOMLocationFilter");

            // Import logic
            var err = eih.AutoImport(Request.Files, _masterCostBLL, data =>
            {
                // Set primary values
                data.CreatedBy = GetUserId();
                data.CreatedDate = DateTime.Now;
                data.UpdatedBy = GetUserId();
                data.UpdatedDate = DateTime.Now;
                data.IsActive = true;
                data.EffectiveEndDate = new DateTime(2999, 12, 31);

                // data validation logic, if not valid, just throw exception
                /// === Cost Type
                if (data.CostType == null) throw new RowValidationByFieldException("Cost Type");
                if (_listCostType.Contains(data.CostType) == null) throw new RowValidationByFieldException("Vendor Name", data.VendorName, "not found in");

                /// === Vendor
                if (data.VendorName == null) throw new RowValidationByFieldException("Vendor Name");
                var vend = _bllVendor.GetVendorByName(data.VendorName);
                if (vend == null || !vend.IsActive) throw new RowValidationByFieldException("Vendor Name", data.VendorName, "not found in");
                if (vend.ParentVendor != null) throw new RowValidationByFieldException("Vendor Name", data.VendorName, "not a parent vendor in");
                data.IDVendor = vend.IDVendor;

                /// === Vehicle Type
                if (data.CostType == "Trip Based" || data.CostType == "ASDP")
                {
                    if (data.VehicleType == null) throw new RowValidationByFieldException("Vehicle Type");
                }
                else data.VehicleType = null;

                /// === Order Type
                if (data.CostType == "Trip Based")
                {
                    if (data.OrderType == null) throw new RowValidationByFieldException("Order Type");
                }
                else data.OrderType = null;

                /// === Sender
                if (data.CostType != "KM Based" && data.CostType != "SPSI")
                {
                    if (data.SenderIDLocation == null) throw new RowValidationByFieldException("Sender");
                    var loc = _listLocation.Where(c => c.LocationName == data.SenderIDLocation).FirstOrDefault();
                    if (loc == null) throw new RowValidationByFieldException("Sender", data.SenderIDLocation, "not found in");
                    data.SenderIDLocation = loc.IDLocation;
                }
                else data.SenderIDLocation = null;

                /// === Through
                if (data.ThroughIDLocation != null && (data.CostType == "Box Based" || data.CostType == "Trip Based"))
                {
                    var loc = _listLocation.Where(c => c.LocationName == data.ThroughIDLocation).FirstOrDefault();
                    if (loc == null) throw new RowValidationByFieldException("Through", data.ThroughIDLocation, "not found in");
                    data.ThroughIDLocation = loc.IDLocation;
                }
                else data.ThroughIDLocation = null;

                /// === Receiver
                if (data.CostType != "KM Based")
                {
                    if (data.ReceiverIDLocation == null) throw new RowValidationByFieldException("Receiver");
                    var loc = _listLocation.Where(c => c.LocationName == data.ReceiverIDLocation).FirstOrDefault();
                    if (loc == null) throw new RowValidationByFieldException("Receiver", data.ReceiverIDLocation, "not found in");
                    data.ReceiverIDLocation = loc.IDLocation;
                }
                else data.ReceiverIDLocation = null;

                /// === Via
                if (data.CostType == "Trip Based")
                {
                    if (data.Via != null && !_listVia.Contains(data.Via)) throw new RowValidationByFieldException("Via", data.Via);
                }

                /// === Minimum KM
                if (data.CostType == "KM Based")
                {
                    if (data.MinimumKM == null) throw new RowValidationByFieldException("Minimum KM");
                    if (data.MinimumKM < 0) throw new RowValidationByFieldException("Minimum KM", data.MinimumKM.ToString());
                }

                /// === Minimum Box
                if (data.CostType == "Box Based")
                {
                    if (data.MinimumBox == null) throw new RowValidationByFieldException("Minimum Box");
                    if (data.MinimumBox < 0) throw new RowValidationByFieldException("Minimum Box", data.MinimumBox.ToString());
                }

                /// === Based Price
                if (data.BasedPrice < 0) throw new RowValidationByFieldException("Based Price", data.BasedPrice.ToString());

                /// === Discount Price
                if (data.DiscountPrice <= 0) throw new RowValidationByFieldException("Discount Price", data.DiscountPrice.ToString());

                /// === Additional Unit
                if (data.AdditionalUnitPrice != null && data.AdditionalUnitPrice <= 0) throw new RowValidationByFieldException("Additional Unit", data.AdditionalUnitPrice.ToString());

                // return the validated data, return null to set current record as invalid data
                return data;
            });

            return Json(new NonQueryResult(err));

            #region Old Upload Logic
            /*
            List<string> listError = new List<string>();
            try
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        listError = _masterCostBLL.Upload(fileContent, GetUserId());
                    }
                }
            }
            catch (Exception e)
            {
                return Json(e);
            }
            //Debug.WriteLine("listError:"+listError);
            return Json(listError);
            */
            #endregion
        }

        public ActionResult Delete(int keyID)
        {
            bool result = false;

            result = _masterCostBLL.DelData(keyID);
            return Json(result);
        }
    }
}