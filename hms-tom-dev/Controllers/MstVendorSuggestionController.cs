using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.BusinessLogics;
using DFIS.Utils.Exceptions;
using DFIS.Universal.Domain.DTOs;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.DTOs;
using DFIS.Universal.Domain.Outputs;
using System.Linq;
using DFIS.Universal.Domain.Inputs;
using hms_tom_dev.Helper;
using DFIS.Universal;
using System.Collections;
using TOM.Master.Domain.Inputs;

namespace hms_tom_dev.Controllers
{
    public class MstVendorSuggestionController : BaseController
    {
        private readonly IMasterVendorSuggestionBLL _masterVendorSuggestionBll;
        private readonly IMasterLocationBLL _mstLocationBll;
        private readonly IMasterVendorTOMBLL _mstVendorBll;

        public MstVendorSuggestionController(IMasterVendorTOMBLL mstVendorBll, IMasterVendorSuggestionBLL masterVendorSuggestionBll, IMasterLocationBLL mstLocationBll)
        {
            _masterVendorSuggestionBll = masterVendorSuggestionBll;
            _mstLocationBll = mstLocationBll;
            _mstVendorBll = mstVendorBll;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDropDown()
        {
            return Json(_masterVendorSuggestionBll.GetList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocation()
        {
            var getdata = _mstLocationBll.GetByConfig("TOMLocationFilter");
            return Json(getdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendor(MasterVendorSuggestionDTO Input)
        {
            return Json(_masterVendorSuggestionBll.GetVendorByCriteria(Input), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterVendorSuggestion(MasterVendorSuggestionInput criteria)
        {
            var viewModel = Mapper.Map<List<MasterVendorSuggestionViewModel>>(_masterVendorSuggestionBll.GetALLMasterVendorSuggestion(criteria));
            foreach (var vm in viewModel)
            {
                vm.MasterLocation = Mapper.Map<MasterLocation>(_mstLocationBll.Get(c => c.IDLocation == vm.StartLocation).FirstOrDefault());
                vm.MasterLocation1 = Mapper.Map<MasterLocation>(_mstLocationBll.Get(c => c.IDLocation == vm.ReceiverIDLocation).FirstOrDefault());
                vm.MasterTransportVendor = Mapper.Map<MasterVendor>(_mstVendorBll.Get(new TOM.Master.Domain.Inputs.MasterVendorTOMInput() { IDVendor = vm.SuggestedVendor }).FirstOrDefault());
            }
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertList(InsertUpdateData<MasterVendorSuggestionViewModel> bulkData)
        {
            try
            {
                if (bulkData.New != null)
                {
                    var mstVendorSuggestion = Mapper.Map<MasterVendorSuggestion>(bulkData.New[0]);

                    //pengecekan data double by attribute dari DDL
                    var dbRes = _masterVendorSuggestionBll.ValidateData(mstVendorSuggestion);

                    mstVendorSuggestion.UpdatedBy = GetUserId();
                    if (dbRes == null)
                    {
                        mstVendorSuggestion.CreatedBy = GetUserId();
                        _masterVendorSuggestionBll.SaveData(mstVendorSuggestion, true);
                    } //jika record found dan status masih aktif maka beri pesan error data sudah pernah ada.
                    else if (dbRes != null && dbRes.IsActive)
                    {
                        return Json(
                           new NonQueryResult(false, "Data already exists.")
                       );
                    } //jika record found dan data tidak aktif maka aktifkan kembali dan panggil function untuk update.
                    else if (dbRes != null && !dbRes.IsActive)
                    {
                        mstVendorSuggestion.IDVendorSuggestion = dbRes.IDVendorSuggestion;
                        mstVendorSuggestion.CreatedBy = dbRes.CreatedBy;
                        mstVendorSuggestion.CreatedDate = dbRes.CreatedDate;
                        _masterVendorSuggestionBll.SaveData(mstVendorSuggestion, false);
                    }
                }
                else
                {
                    var mstVendorSuggestion = _masterVendorSuggestionBll.GetByID(bulkData.Edit[0].IDVendorSuggestion);
                    MappingHelper.Map(bulkData.Edit[0], mstVendorSuggestion);
                    mstVendorSuggestion.UpdatedBy = GetUserId();
                    mstVendorSuggestion.UpdatedDate = DateTime.Now;
                    _masterVendorSuggestionBll.InsertOrUpdate(mstVendorSuggestion, false);
                }
            }
            catch (ExceptionBase ex)
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
            var context = new TOMContextDB();
            var userId = GetUserId();
            var role = context.MasterUserRoleMappings
                .Where(n => n.IDUser == userId)
                .Select(n => n.IDRole).ToList();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase uploadFile = Request.Files.Get(0);
                var listOrderType = _masterVendorSuggestionBll.GetList().Where(c => c.FieldName == "OrderType").Select(c => c.FieldValue);
                var listTraCat = _masterVendorSuggestionBll.GetList().Where(c => c.FieldName == "TransportationCategory").Select(c => c.FieldValue);
                var listTraMode = _masterVendorSuggestionBll.GetList().Where(c => c.FieldName == "TransportationMode").Select(c => c.FieldValue);
                var listVehiType = _masterVendorSuggestionBll.GetList().Where(c => c.FieldName == "VehicleType").Select(c => c.FieldValue);
                
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    var filename = uploadFile.FileName.ToLower();
                    try
                    {
                        // Excel Helper configuration, this maps the excel header to field
                        // exc.HeaderMap[<header text>] = <database field>;
                        var exc = new ExcelHelper(uploadFile.InputStream, filename.EndsWith(".xlsx"));
                        exc.HeaderMap["Start Location"] = "StartLocation";
                        exc.HeaderMap["Receiver"] = "ReceiverIDLocation";
                        exc.HeaderMap["Order Type"] = "OrderType";
                        exc.HeaderMap["Transportation Category"] = "TransportationCategory";
                        exc.HeaderMap["Transportation Mode"] = "TransportationMode";
                        exc.HeaderMap["Vehicle Type"] = "VehicleType";
                        exc.HeaderMap["Suggested Vendor"] = "SuggestedVendorName";

                        if (role.Contains(1) || role.Contains(5) || role.Contains(7))
                        {
                            // Map table to object
                            // Callback: further check data or modify data per record. return the object afterwards, return null if invalid.
                            var objList = exc.MapTable<MasterVendorSuggestionDTO>(exc.ExcelData.Tables[0], (c) =>
                            {
                                c.CreatedBy = GetUserId();
                                c.CreatedDate = DateTime.Now;
                                c.UpdatedBy = GetUserId();
                                c.UpdatedDate = DateTime.Now;
                                c.IsActive = true;

                                if (c.StartLocation == null) throw new Exception("Sender is required.");
                                if (c.ReceiverIDLocation == null) throw new Exception("Receiver is required.");

                                var locSender = _mstLocationBll.GetMasterLocations(new MasterLocationInput() { LocationName = c.StartLocation, IsAssigned = true }).FirstOrDefault();
                                var locRcv = _mstLocationBll.GetMasterLocations(new MasterLocationInput() { LocationName = c.ReceiverIDLocation, IsAssigned = true }).FirstOrDefault();

                                if (locSender == null) throw new Exception("Sender [" + c.StartLocation + "] is not found.");
                                if (locRcv == null) throw new Exception("Receiver [" + c.ReceiverIDLocation + "] is not found.");

                                if (c.SuggestedVendorName == null) throw new Exception("Vendor is required.");
                                var vendor = _mstVendorBll.Get(new TOM.Master.Domain.Inputs.MasterVendorTOMInput() { VendorName = c.SuggestedVendorName });
                                if (vendor.Count == 0) throw new Exception("Vendor [" + c.SuggestedVendorName + "] is not found.");
                                if (vendor.ElementAt(0).ParentVendor != null)
                                    throw new Exception("Vendor must be a parent vendor");
                                c.SuggestedVendor = vendor.ElementAt(0).IDVendor;

                                c.StartLocation = locSender != null ? locSender.IDLocation : null;
                                c.ReceiverIDLocation = locRcv != null ? locRcv.IDLocation : null;

                                if (!listOrderType.Contains(c.OrderType))
                                    throw new Exception("Order type [" + c.OrderType + "] is not valid.");
                                if (!listTraCat.Contains(c.TransportationCategory))
                                    throw new Exception("Transportation Category [" + c.TransportationCategory + "] is not valid.");
                                if (!listTraMode.Contains(c.TransportationMode))
                                    throw new Exception("Transportation Mode [" + c.TransportationMode + "] is not valid.");
                                if (!listVehiType.Contains(c.VehicleType))
                                    throw new Exception("Vehicle Type [" + c.VehicleType + "] is not valid.");
                                return c;
                            });
                            if (exc.ErrorLog.Count > 0)
                                return Json(new NonQueryResult(false, exc.CompileLog("<br />", "&bull; ")));

                            Hashtable hash = new Hashtable();

                            foreach (var obj in objList)
                            {
                                var key = obj.StartLocation + obj.ReceiverIDLocation + obj.OrderType + obj.TransportationCategory + obj.TransportationMode + obj.VehicleType;
                                if (hash.ContainsKey(key))
                                {
                                    return Json(new NonQueryResult(false, "Duplicate data at row #" + (hash.Count + 2)));
                                }
                                hash.Add(key, true);                               
                            }
                            var res = exc.AutoImport(objList, (IImporterBLL<MasterVendorSuggestionDTO>)_masterVendorSuggestionBll);                             
                            if (res != null)
                                return Json(new NonQueryResult(false, "&bull;" + string.Join("<br />&bull;", res)));
                        }
                        else
                            return Json(new NonQueryResult(false, "Not allowed"));
                    }
                    catch (Exception ex)
                    {
                        return Json(new NonQueryResult(false, ex.Message));
                    }
                    return Json(new NonQueryResult(true));
                }
            }

            return Json(new NonQueryResult(false, "Invalid file."));

            List<string> listError = new List<string>();
            try
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        listError = _masterVendorSuggestionBll.Upload(fileContent, GetUserId());
                    }
                }
                if (listError.Count > 0)
                {
                    for (int i = 0; i < listError.Count; i++)
                    {
                        try
                        {
                            var spl = listError[i].Split(new char[] { '-' }, StringSplitOptions.None);
                            listError[i] = "Row #" + spl[2] + " - The " + spl[0] + " value is invalid. (" + spl[1] + ")";
                        }
                        catch { }
                    }
                    return Json(new NonQueryResult(false, String.Join("<br />&bull; ", listError)));
                }
                else
                    return Json(new NonQueryResult(true));
            }
            catch (Exception e)
            {
                return Json(new NonQueryResult(false, e.Message));
            }
        }
    }
}