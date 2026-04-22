using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using hms_tom_dev.Models;
using hms_tom_dev.Util;
using hms_tom_dev.Models.Universal;

namespace hms_tom_dev.Controllers
{
    public class CustomReportStateController : BaseController
    {
        private readonly ICustomReportStateBLL _customReportStateBLL;
        public CustomReportStateController(ICustomReportStateBLL customReportStateBLL)
        {
            _customReportStateBLL = customReportStateBLL;
            SetPage(EnumHelper.GetDescription(Enums.PageName.CustomReportState));
        }

        public ActionResult GetLayouts(CustomReportStateInput criteria)
        {
            //var customReportStateBLL = _customReportStateBLL.GetLayouts(criteria, GetUserId());
            var customReportStateBLL = _customReportStateBLL.GetLayoutsJoin(criteria, GetUserId());
            var viewModel = Mapper.Map<List<CustomReportStateViewModel>>(customReportStateBLL);
            var output = Json(viewModel, JsonRequestBehavior.AllowGet);

            output.MaxJsonLength = int.MaxValue;

            return output;
        }

        public ActionResult GetSelectedLayout(CustomReportStateInput criteria)
        {
            var customReportStateBLL = _customReportStateBLL.GetSelectedLayout(criteria, GetUserId());
            var viewModel = Mapper.Map<List<CustomReportStateViewModel>>(customReportStateBLL);
            var output = Json(viewModel, JsonRequestBehavior.AllowGet);

            output.MaxJsonLength = int.MaxValue;

            return output;
        }

        public ActionResult GetSavedSelectedLayout(CustomReportStateInput criteria)
        {
            var customReportStateBLL = _customReportStateBLL.GetSavedSelectedLayout(criteria, GetUserId());
            var viewModel = Mapper.Map<List<CustomReportStateViewModel>>(customReportStateBLL);
            var output = Json(viewModel, JsonRequestBehavior.AllowGet);

            output.MaxJsonLength = int.MaxValue;

            return output;
        }

        [HttpPost]
        public ActionResult InsertRecords()
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            string userid = GetUserId();
            string response = null;

            string layoutName = Request.Params["layoutName"];
            string fieldName = Request.Params["fieldNames"];
            string pageName = Request.Params["pageName"];
            string global = Request.Params["isGlobal"];

            if (!string.IsNullOrEmpty(layoutName) &&
                !string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(pageName))
            {
                string[] Fields = Request.Params["fieldNames"].Split(',');

                try
                {
                    _customReportStateBLL.DeleteDataBase(Request.Params["pageName"], Request.Params["layoutName"],
                        controller, userid);
                    foreach (string Field in Fields)
                    {
                        var inputData = new CustomReportStateDTO
                        {
                            IDUser = GetUserId(),
                            PageName = pageName,
                            FieldName = Field,
                            LayoutName = layoutName,
                            IsGlobal = bool.Parse(global),
                            IsActive = true,
                            CreatedBy = GetUserId(),
                            UpdatedBy = GetUserId()
                        };

                        _customReportStateBLL.InsertCustomReportState(inputData, controller, userid);
                    }

                    response = Enums.ResponseType.Success.ToString();
                }
                catch (ExceptionBase ex)
                {
                    response = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();
                }
            }
            else
            {
                response = Enums.ResponseType.NoLayoutName.ToString();
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveSelectedLayout()
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            string userid = GetUserId();
            string response = null;
            var layoutName = Request.Params["layoutName"];
            var fieldName = Request.Params["fieldNames"];
            var pageName = Request.Params["pageName"];

            if (!string.IsNullOrEmpty(layoutName) && !string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(pageName))
            {
                try
                {
                    var PageNameUserId = Request.Params["pageName"] + '_' + userid;

                    var inputData = new CustomReportStateDTO
                    {
                        IDUser = GetUserId(),
                        PageName = PageNameUserId,
                        FieldName = Request.Params["fieldNames"],
                        LayoutName = Request.Params["layoutName"],
                        IsGlobal = bool.Parse(Request.Params["isGlobal"]),
                        IsActive = true,
                        CreatedBy = GetUserId(),
                        UpdatedBy = GetUserId()
                    };

                    _customReportStateBLL.SaveSelectedLayout(inputData, controller, userid);

                    response = Enums.ResponseType.Success.ToString();
                }
                catch (ExceptionBase ex)
                {
                    response = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();
                }
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }
        
	}
}
