using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using System.Collections;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using TOM.Master.BusinessLogics;

namespace hms_tom_dev.Controllers
{
    public class MstGuidelineController : BaseController
    {
        private readonly IMasterGuidelineBLL _masterGuidelineBLL;
       
        //
        // GET: /MstGuideline/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexSearch()
        {
            return View();
        }

        public ActionResult ContinousImprovementGuide()
        {
            return View();
        }

        public MstGuidelineController(IMasterGuidelineBLL masterGuidelineBLL)
        {
            _masterGuidelineBLL = masterGuidelineBLL;
            SetPage(EnumHelper.GetDescription(Enums.PageName.MstGuideline));
        }

        public ActionResult GetDataById(MasterGuidelineInput criteria)
        {
            var masterGuidelines = _masterGuidelineBLL.GetById(criteria.IDGuideline);
            var viewModel = Mapper.Map<MasterGuidelineViewModel>(masterGuidelines);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMasterGuideline(MasterGuidelineInput criteria)
        {
            var masterGuidelineBLL = _masterGuidelineBLL.GetMasterGuidelines(criteria);
            var viewModel = Mapper.Map<List<MasterGuidelineViewModel>>(masterGuidelineBLL);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListForGuideline(MasterFunctionInput criteria)
        {
            var masterFunctions = _masterGuidelineBLL.GetListForGuidelines(criteria);
            var viewModel = Mapper.Map<List<MasterFunctionViewModel>>(masterFunctions);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //public ActionResult InsertList(MasterListInput criteria)
        public ActionResult InsertGuideline(InsertUpdateData<MasterGuidelineViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var mstGuideline = Mapper.Map<MasterGuidelineDTO>(bulkData.New[i]);

                    //set createdby and updatedby

                    mstGuideline.CreatedBy = GetUserId();
                    mstGuideline.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _masterGuidelineBLL.SaveData(mstGuideline);
                        bulkData.New[i] = Mapper.Map<MasterGuidelineViewModel>(item);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                }
            }
            else
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    if (bulkData.Edit[i] == null) continue;
                    var mstGuideline = Mapper.Map<MasterGuidelineDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    mstGuideline.CreatedBy = "system";
                    mstGuideline.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _masterGuidelineBLL.EditData(mstGuideline);
                        bulkData.Edit[i] = Mapper.Map<MasterGuidelineViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                }
            }

            return Json(bulkData);

        }

        public ActionResult GetListForGuidelinesByDescAndKeyword(String keyword)
        {
            MasterGuidelineInput input = new MasterGuidelineInput();
            input.Keywords = keyword;
            return Json(_masterGuidelineBLL.GetListForGuidelinesByDescAndKeyword(input), JsonRequestBehavior.AllowGet);
        }
    }
}
