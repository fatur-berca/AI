using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.BusinessLogics;
using DFIS.Utils.Exceptions;
using TOM.EntitiesDAL;
using System;
using DFIS.Universal.Domain.Inputs;
using TOM.Master.BusinessLogics;
using DFIS.Universal;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.Domain.DTOs;

namespace hms_tom_dev.Controllers
{
    public class MstVendorTOMController : BaseController
    {
        private readonly IMasterVendorTOMBLL _masterVendorTOMBll;
        private readonly IMasterListBLL _masterListBLL;
        private readonly IMasterMappingBLL _masterMapBLL;

        public MstVendorTOMController(IMasterVendorTOMBLL masterVendorTOMBll, IMasterListBLL masterListBLL, IMasterMappingBLL masterMapBLL)
        {
            _masterVendorTOMBll = masterVendorTOMBll;
            _masterListBLL = masterListBLL;
            _masterMapBLL = masterMapBLL;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDropDown()
        {
            return Json(_masterVendorTOMBll.GetList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDropDownVendorCategory()
        {
            return Json(_masterVendorTOMBll.GetList().FindAll(v => v.FieldName == "VendorCategory"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDropDownTransportationMode()
        {
            return Json(_masterVendorTOMBll.GetList().FindAll(v => v.FieldName == "TransportationMode"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterVendorTOM()
        {
            var viewModel = Mapper.Map<List<MasterVendorTOMViewModel>>(_masterVendorTOMBll.GetALLMasterVendors());
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetParentVendorTOM()
        {
            return Json(_masterVendorTOMBll.GetALLMasterVendors().FindAll(v => v.ParentVendor == null && v.IsActive), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCities(string prov = null)
        {
            if (prov == null)
                return Json(new string[0], JsonRequestBehavior.AllowGet);
            return Json(_masterMapBLL.GetMasterMappings(new MasterMappingInput() { MapFrom = prov }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProvinces()
        {
            return Json(_masterListBLL.GetMasterListByFieldName(new MasterListInput() { FieldName = "Province" }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertList(InsertUpdateData<MasterVendorTOMViewModel> bulkData)
        {
            try
            {
                var datas = bulkData.New != null ? bulkData.New : bulkData.Edit;
                var editMode = bulkData.New == null;
                var vendors = _masterVendorTOMBll.GetALLMasterVendorsEntity();
                foreach (var data in datas)
                {
                    var entity = MappingHelper.Map<MasterVendorTOMDTO>(data);
                    entity.UpdatedBy = GetUserId();
                    entity.UpdatedDate = DateTime.Now;
                    if (!editMode)
                    {
                        entity.CreatedBy = GetUserId();
                        entity.CreatedDate = DateTime.Now;
                    }

                    if (!editMode)
                    {
                        // kalo parentnya mati, g boleh update (cuma buat new)
                        var prt = vendors.FindAll(c => c.IDVendor == entity.ParentVendor);
                        if (prt.Count > 0 && !prt[0].IsActive)
                            return Json(new NonQueryResult(false, "Cannot overwrite existing child when its parent is inactive."));
                        // gak boleh pindah parent
                        var ego = vendors.FindAll(c => c.VendorName == entity.VendorName);
                        if (ego.Count > 0 && ego[0].ParentVendor != entity.ParentVendor)
                            return Json(new NonQueryResult(false, "Either the parent of an existing item is inactive or you tried to change the parent of an existing item."));
                    }

                    // cari parent lalu idupin kalo childnya idup
                    if (editMode && data.ParentVendor != null && data.IsActive)
                    {
                        var pdr = vendors.Find(v =>
                           v.IDVendor == data.ParentVendor
                           );
                        if (pdr != null)
                        {
                            pdr.IsActive = true;
                            _masterVendorTOMBll.InsertOrUpdate(pdr, !editMode);
                        }
                    }
                    // cari child lalu kill kalo parentnya mati
                    if (editMode && data.ParentVendor == null && !data.IsActive)
                    {
                        var pdr = vendors.FindAll(v =>
                           v.ParentVendor == data.IDVendor
                           );
                        foreach (var p in pdr)
                        {
                            p.IsActive = false;
                            _masterVendorTOMBll.InsertOrUpdate(p, !editMode);
                        }
                    }

                    // baru update itemnya????
                    _masterVendorTOMBll.InsertOrUpdate(entity);
                }
                return Json(
                    new NonQueryResult(true)
                    );
            }
            catch (Exception ex)
            {
                try
                {
                    return Json(
                        new NonQueryResult(false, ex)
                    );
                }
                catch { }

                return Json(new NonQueryResult(false, ex.Message));
            }
        }
    }
}