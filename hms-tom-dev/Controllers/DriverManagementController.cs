using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using TOM.Master.BusinessLogics;
using DFIS.Universal.Domain.Inputs;
using DFIS.Universal.Domain.Outputs;
using hms_tom_dev.Models.Masters;
using hms_tom_dev.Models.Transport;
using hms_tom_dev.Util;
using TOM.Master.Domain.Inputs;
using TOM.Transport.BusinessLogics;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.DTOs;
using hms_tom_dev.Helper;

namespace hms_tom_dev.Controllers
{
    public class DriverManagementController : BaseController
    {
        private readonly ITransportDriverManagementBLL _transportDriverManagementBLL;
        private readonly IMasterVendorTOMBLL _masterVendorBLL;
        private readonly IMasterListBLL _masterListBLL;
        private readonly IMasterUserBLL _masterUserBLL;

        public DriverManagementController(ITransportDriverManagementBLL transportDriverManagementBLL, IMasterUserBLL masterUserBLL, IMasterVendorTOMBLL masterVendorBLL, IMasterListBLL masterListBLL)
        {
            _transportDriverManagementBLL = transportDriverManagementBLL;
            _masterVendorBLL = masterVendorBLL;
            _masterListBLL = masterListBLL;
            _masterUserBLL = masterUserBLL;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportDriverManagement));
        }

        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var viewModel = new TransportDriverManagementViewModel();
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.ListUserRole = GetListUserRole();
            return View(viewModel);
        }

        public ActionResult AddNew(string id=null, string idvendor = null)
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var viewModel = new TransportDriverManagementViewModel();
            viewModel.ID = id;
            viewModel.IDVendor = idvendor;
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            ViewBag.CurrentPage = GetPageID();
            //ViewBag.ClosingPageDate = GetLockDateRangeList();
            ViewBag.ListLocation = session.Location.Select(x => x.IDLocation).ToList();
            ViewBag.ListUserRole = session.Role.Select(x => x.RoleName).ToList();

            return View(viewModel);
        }

        public ActionResult PartialViewCustomReport()
        {
            ViewBag.ListUserRole = GetListUserRole();

            return View("_CustomReport");
        }

        [HttpPost]
        public ActionResult SetStatusResult(TransportDriverManagementInput filter)
        {
            try
            {
                _transportDriverManagementBLL.SetActive(filter.ListIsActiveIdString, filter.IsActive);

                return Json(Enums.ResponseType.Success.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDataByCriteria(TransportDriverManagementInput criteria)
        {
            //var transportDriverManagement = _transportDriverManagementBLL.GetById(criteria.ID);
            var transportDriverManagement = _transportDriverManagementBLL.GetDataByCriteria(criteria).Where(x => x.RoleDriver == "Driver");
            var coDriverManagement = _transportDriverManagementBLL.GetDataByCriteria(criteria).Where(x => x.RoleDriver == "CO-Driver");
            //var viewModel = Mapper.Map<TransportDriverManagementViewModel>(transportDriverManagement);
            //viewModel.Name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(viewModel.Name.ToLower());

            return Json(new { driver = transportDriverManagement, codriver = coDriverManagement }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataById(TransportDriverManagementInput criteria)
        {
            //var transportDriverManagement = _transportDriverManagementBLL.GetById(criteria.ID);
            var transportDriverManagement = _transportDriverManagementBLL
                .GetByIdCardNumberAndIDVendor(criteria.ID,Convert.ToInt32(criteria.IDVendor));
            var viewModel = Mapper.Map<TransportDriverManagementViewModel>(transportDriverManagement);
            viewModel.Name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(viewModel.Name.ToLower());

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckDriver(string idcard, int idvendor)
        {
            var result = _transportDriverManagementBLL.CheckDriver(idcard, idvendor);
            if(result== null)
                return Json("null", JsonRequestBehavior.AllowGet);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportDriverManagements(TransportDriverManagementInput criteria)
        {
            //int idvendor = 0;
            //if(!string.IsNullOrEmpty(Request.Params["IDVendor"]))
            string idvendor = Request.Params["IDVendor"];
            string name = Request.Params["Name"];
            string incompleteRequirement = Request.Params["IncompleteRequirement"];
            string baseTown = Request.Params["BaseTown"];
            string performanceLevel = Request.Params["PerformanceLevel"];
            string id = Request.Params["ID"];
            var includeInActive = Request.Params["IncludeInActive"];
            var state = Request.Params["State"];

            var input = new TransportDriverManagementInput()
            {
                VendorID = idvendor,
                Name = name,
                IncompleteRequirement = incompleteRequirement,
                BaseTown = baseTown,
                PerformanceLevel = performanceLevel,
                ID = id,
                DrivingLicenseNumber = id,
                IncludeInActive = includeInActive,
                IsExportOrSearch = criteria.IsExportOrSearch,
                UserRole = GetListUserRole().FirstOrDefault().RoleName
            };

            var transportDriverManagement = _transportDriverManagementBLL.GetTransportDriverManagements(input);
            var viewModel = Mapper.Map<List<TransportDriverManagementViewModel>>(transportDriverManagement);

            foreach(var item in viewModel)
            {
                item.Name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(item.Name.ToLower());
            }

            if (!GetListUserRole().Select(x => x.IDRole.ToString()).Contains("1"))
            {
                viewModel = viewModel.Where(x => x.IsActive).ToList();
            }
            //var closingpage = state == "editpage" && ClosingPageState(viewModel.Select(x => x.CreatedDate).FirstOrDefault());

            //ViewBag.ClosingPage = closingpage;

            var Summary = new {
                totalDrivers = viewModel.Count,
                driversWithExpiredDriverLicense = viewModel.Where(k => k.DrivingLicensePeriod < DateTime.Today).Count(),
                driversWithMore5YearsExperience = viewModel.Where(k => k.JoinDate < System.DateTime.Today.AddYears(-5)).Count(),
                driversWithoutDrugTest = viewModel.Where(k => k.DrugFreeTest == false).Count(),
                driversWithoutFatigueTest = viewModel.Where(k => k.FatiqueTest == false).Count(),
                driversWithoutInductionTest = viewModel.Where(k => k.InductionTest == false).Count(),
                driversWithoutDefensiveDriversTest = viewModel.Where(k => k.DefensiveDrivingTest == false).Count(),
                driversWithoutBpjsKetenagakerjaan = viewModel.Where(k => k.BPJSKetenagaKerjaan == false).Count(),
                driversWithoutBpjsKesehatan = viewModel.Where(k => k.BPJSKesehatan == false).Count(),
            };

            var jsonResult = Json(new
            {
                viewmodel = viewModel,
                summary = Summary,
            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            /*
            return Json(new
            {
                viewmodel = viewModel,
                summary = Summary,
            }, JsonRequestBehavior.AllowGet);
            */
        }

        public ActionResult GetTransportDriverManagementsTable(TransportDriverManagementInput criteria, DataTableModel model)
        {
            string idvendor = Request.Params["filter[IDVendor]"];
            string name = Request.Params["filter[Name]"];
            string roleDriver = Request.Params["filter[RoleDriver]"];
            string id = Request.Params["filter[ID]"];
            var includeInActive = Request.Params["filter[IncludeInActive]"];
            bool isExportOrSearch = Convert.ToBoolean(Request.Params["filter[IsExportOrSearch]"]);
            var state = Request.Params["State"];
            var IsActive = false;
            if (!GetListUserRole().Select(x => x.IDRole.ToString()).Contains("1"))
            {
                IsActive = true;
            }

            var input = new TransportDriverManagementInput()
            {
                VendorID = idvendor,
                Name = name,
                RoleDriver = roleDriver,
                ID = id,
                IncludeInActive = includeInActive,
                IsExportOrSearch = isExportOrSearch,
                UserRole = GetListUserRole().FirstOrDefault().RoleName,
                IsActive = IsActive
            };

            var transportDriverManagement = _transportDriverManagementBLL.GetTransportationDriverManagementsDataTable(input, model);
            var viewModel = Mapper.Map<List<TransportDriverManagementViewModel>>(transportDriverManagement.data);

            foreach (var item in viewModel)
            {
                item.Name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(item.Name.ToLower());
            }

            var result = new
            {
                recordsTotal = transportDriverManagement.total,
                recordsFiltered = transportDriverManagement.total,
                draw = int.Parse(Request["draw"]),
                data = viewModel,
                dbResultCount = transportDriverManagement.total,
            };
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetSearchResult(TransportDriverManagerSearchnput criteria)
        {
            var dbResult = _transportDriverManagementBLL.getSearchResult(criteria);
            var viewModel = Mapper.Map<List<TransportDriverManagementViewModel>>(dbResult);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }



        public ActionResult CountTransportDriverManagements()
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();

            int totalDrivers = _transportDriverManagementBLL.CountTransportDriverManagements(new TransportDriverManagementInput{}, "totalDrivers");
            int driversWithExpiredDriverLicense = _transportDriverManagementBLL.CountTransportDriverManagements(new TransportDriverManagementInput{}, "driversWithExpiredDriverLicense");
            int driversWithMore5YearsExperience = _transportDriverManagementBLL.CountTransportDriverManagements(new TransportDriverManagementInput{}, "driversWithMore5YearsExperience");
            int driversWithoutDrugTest = _transportDriverManagementBLL.CountTransportDriverManagements(new TransportDriverManagementInput{}, "driversWithoutDrugTest");
            int driversWithoutFatigueTest = _transportDriverManagementBLL.CountTransportDriverManagements(new TransportDriverManagementInput{}, "driversWithoutFatigueTest");
            int driversWithoutInductionTest = _transportDriverManagementBLL.CountTransportDriverManagements(new TransportDriverManagementInput{}, "driversWithoutInductionTest");
            int driversWithoutDefensiveDriversTest = _transportDriverManagementBLL.CountTransportDriverManagements(new TransportDriverManagementInput{}, "driversWithoutDefensiveDriversTest");
            int driversWithoutBpjsKetenagakerjaan = _transportDriverManagementBLL.CountTransportDriverManagements(new TransportDriverManagementInput {}, "driversWithoutBpjsKetenagakerjaan");
            int driversWithoutBpjsKesehatan = _transportDriverManagementBLL.CountTransportDriverManagements(new TransportDriverManagementInput {}, "driversWithoutBpjsKesehatan");

            int[] DriverCount = new int[9];

            DriverCount[0] = totalDrivers;
            DriverCount[1] = driversWithExpiredDriverLicense;
            DriverCount[2] = driversWithMore5YearsExperience;
            DriverCount[3] = driversWithoutDrugTest;
            DriverCount[4] = driversWithoutFatigueTest;
            DriverCount[5] = driversWithoutInductionTest;
            DriverCount[6] = driversWithoutDefensiveDriversTest;
            DriverCount[7] = driversWithoutBpjsKetenagakerjaan;
            DriverCount[8] = driversWithoutBpjsKesehatan;

            return Json(DriverCount, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetInactive()
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            string userid = GetUserId();
            string response = null;
            
            if (!string.IsNullOrEmpty(Request.Params["ID"]))
            {
                string[] ids = Request.Params["ID"].Split(',');

                try
                {
                    foreach (var id in ids)
                    {
                        var inputData = new TransportDriverManagementDTO()
                        {
                            ID = id,
                            IsActive = false,
                            CreatedBy = userid,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            UpdatedBy = userid
                        };

                        _transportDriverManagementBLL.SetInactive(inputData, controller);
                    }

                    response = Enums.ResponseType.Success.ToString();
                }
                catch (ExceptionBase ex)
                {
                    response = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();
                }
            }
         
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateRecord(TransportDriverManagementInput input)
        {
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            
            var inputData = Mapper.Map<TransportDriverManagementDTO>(input);

            try
            {
                _transportDriverManagementBLL.Save(inputData, controller, GetUserId());

                return Json(Enums.ResponseType.Success.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Upload()
        {
            var fileNameKTP = string.Empty;
            var fileNameSIM = string.Empty;
            var fileNameFoto = string.Empty;

            try
            {
                fileNameKTP = Request.Headers["X-File-Name-KTP"];
                fileNameSIM = Request.Headers["X-File-Name-SIM"];
                fileNameFoto = Request.Headers["X-File-Name-Foto"];

                var path = Server.MapPath("~/Assets/Uploads/DriverManagement/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                    //Use the following properties to get file's name, size and MIMEType
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;

                    string mimeType = file.ContentType;
                    System.IO.Stream fileContent = file.InputStream;
                    if (fileName.Contains("\\"))
                    {
                        fileName = fileName.Split('\\')[fileName.Split('\\').Length - 1];
                    }
                    //To save file, use SaveAs method
                    //path = Path.Combine(path, fileName);
                    //file.SaveAs(path); //File will be saved in application root
                    file.SaveAs(path + fileName); //File will be saved in application root
                }

                return Json("Uploaded " + Request.Files.Count + " files");
            }
            catch (ExceptionBase ex)
            {
                throw;
            }
        }

        public JsonResult ExportToExcel(TransportDriverManagementInput input)
        {
            //input.IsExportOrSearch = true;
            input.UserRole = GetListUserRole().FirstOrDefault().RoleName;
            //List<TransportDriverManagementDTO> records = _transportDriverManagementBLL.GetTransportDriverManagements(input);
            List<TransportDriverManagementDTO> records = _transportDriverManagementBLL.GetTransportDriverManagements2(input);

            if (!GetListUserRole().Select(x => x.IDRole.ToString()).Contains("1"))
            {
                records = records.Where(x => x.IsActive).ToList();
            }

            DataTable boundTable = new DataTable();

            string columns = Request.Params["Columns"];
            if (string.IsNullOrEmpty(columns))
            {
                columns = "RECIDENCE ID NUMBER (KTP),Driver Name,Birth Date,Gender,Address,Mobile Phone,Vendor Name,Join Date,DRIVING LICENSE NUMBER (SIM),DRIVING LICENSE VALIDITY PERIOD,BaseTown,Performance Level,Attachment KTP,Attachment SIM,Attachment Foto,BPJS KetenagaKerjaan,BPJS Kesehatan,Drug Free Test,Fatique Test,Induction Test,Defensive Driving Test,Role Driver,Driver Contract Validity Period,Is Active,Created By,Created Date,Updated By,Updated Date,Remarks";
            }

            var Fields = columns.Split(',');

            //foreach (var rec in records)
            //{
            //    rec.CreatedByFullName = _masterUserBLL.GetMasterUsers(new MasterUserInput { IDUser = rec.CreatedBy }).Count > 0 ? _masterUserBLL.GetMasterUsers(new MasterUserInput { IDUser = rec.CreatedBy }).Single().FullName : "";
            //    rec.UpdatedByFullName = _masterUserBLL.GetMasterUsers(new MasterUserInput { IDUser = rec.UpdatedBy }).Count > 0 ? _masterUserBLL.GetMasterUsers(new MasterUserInput { IDUser = rec.UpdatedBy }).Single().FullName : "";
            //}

            #region OLD CODE
            //foreach (string Field in Fields)
            //{
            //    boundTable.Columns.Add(Field, typeof(string));
            //}
            //foreach (TransportDriverManagementDTO record in records)
            //{
            //    var dict = new Dictionary<string, string>();

            //    dict["RECIDENCE ID NUMBER (KTP)"] = record.ID;
            //    dict["Driver Name"] = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(record.Name.ToLower());
            //    dict["Birth Date"] = (record.DateOfBirth != null) ? record.DateOfBirth.Value.ToString("MM/dd/yyyy HH:mm:ss.fff") : "";
            //    dict["Gender"] = record.Gender;
            //    dict["Address"] = record.Address;
            //    dict["Mobile Phone"] = record.MobilePhone;
            //    dict["Vendor Name"] = record.IDVendor.ToString();
            //    dict["Join Date"] = (record.JoinDate != null) ? record.JoinDate.Value.ToString("MM/dd/yyyy HH:mm:ss.fff") : "";
            //    dict["DRIVING LICENSE NUMBER (SIM)"] = record.DrivingLicenseNumber;
            //    dict["DRIVING LICENSE VALIDITY PERIOD"] = record.DrivingLicensePeriod.ToString();
            //    dict["BaseTown"] = record.BaseTown;
            //    dict["Performance Level"] = record.PerformanceLevel;
            //    dict["Attachment KTP"] = record.AttachmentKTP;
            //    dict["Attachment SIM"] = record.AttachmentSIM;
            //    dict["Attachment Foto"] = record.AttachmentFoto;
            //    dict["BPJS KetenagaKerjaan"] = record.BPJSKetenagaKerjaan.ToString();
            //    dict["BPJS Kesehatan"] = record.BPJSKesehatan.ToString();
            //    dict["Drug Free Test"] = record.DrugFreeTest.ToString();
            //    dict["Fatique Test"] = record.FatiqueTest.ToString();
            //    dict["Induction Test"] = record.InductionTest.ToString();
            //    dict["Defensive Driving Test"] = record.DefensiveDrivingTest.ToString();
            //    dict["Role Driver"] = record.RoleDriver;
            //    dict["Driver Contract Validity Period"] = record.DriverContractValidityPeriod.ToString();
            //    dict["Is Active"] = record.IsActive.ToString();
            //    dict["Created By"] = record.CreatedBy;
            //    dict["Created Date"] = record.CreatedDate.ToString("MM/dd/yyyy HH:mm:ss.fff");
            //    dict["Updated By"] = record.UpdatedBy;
            //    dict["Updated Date"] = record.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss.fff");
            //    dict["Remarks"] = record.Remarks;

            //    dict["Status"] = record.PerformanceLevel;

            //    dynamic dr = boundTable.NewRow();
            //    foreach (string Field in Fields)
            //    {
            //        dr[Field] = dict[Field];
            //    }
            //    boundTable.Rows.Add(dr);
            //}
            #endregion

            using (var pkg = new ExcelExportHelper())
            {
                if (Fields.Contains("RECIDENCE ID NUMBER (KTP)")) pkg.Map("RECIDENCE ID NUMBER (KTP)", "ID");
                if (Fields.Contains("Driver Name")) pkg.Map("Driver Name", "Name");
                if (Fields.Contains("Birth Date")) pkg.Map("Birth Date", "DateOfBirth");
                if (Fields.Contains("Gender")) pkg.Map("Gender", "Gender");
                if (Fields.Contains("Address")) pkg.Map("Address", "Address");
                if (Fields.Contains("Mobile Phone")) pkg.Map("Mobile Phone", "MobilePhone");
                if (Fields.Contains("Vendor Name")) pkg.Map("Vendor Name", "VendorName");
                if (Fields.Contains("Join Date")) pkg.Map("Join Date","JoinDate");
                if (Fields.Contains("DRIVING LICENSE NUMBER (SIM)")) pkg.Map("DRIVING LICENSE NUMBER (SIM)", "DrivingLicenseNumber");
                if (Fields.Contains("DRIVING LICENSE VALIDITY PERIOD")) pkg.Map("DRIVING LICENSE VALIDITY PERIOD", "DrivingLicensePeriod");
                if (Fields.Contains("BaseTown")) pkg.Map("BaseTown", "BaseTown");
                if (Fields.Contains("Performance Level")) pkg.Map("Performance Level", "PerformanceLevel");
                if (Fields.Contains("Attachment KTP")) pkg.Map("Attachment KTP", "AttachmentKTP");
                if (Fields.Contains("Attachment SIM")) pkg.Map("Attachment SIM", "AttachmentSIM");
                if (Fields.Contains("Attachment Foto")) pkg.Map("Attachment Foto", "AttachmentFoto");
                if (Fields.Contains("BPJS KetenagaKerjaan")) pkg.Map("BPJS KetenagaKerjaan", "BPJSKetenagaKerjaan");
                if (Fields.Contains("BPJS Kesehatan")) pkg.Map("BPJS Kesehatan", "BPJSKesehatan");
                if (Fields.Contains("Drug Free Test")) pkg.Map("Drug Free Test", "DrugFreeTest");
                if (Fields.Contains("Fatique Test")) pkg.Map("Fatique Test", "FatiqueTest");
                if (Fields.Contains("Induction Test")) pkg.Map("Induction Test", "InductionTest");
                if (Fields.Contains("Defensive Driving Test")) pkg.Map("Defensive Driving Test", "DefensiveDrivingTest");
                if (Fields.Contains("Role Driver")) pkg.Map("Role Driver", "RoleDriver");
                if (Fields.Contains("Driver Contract Validity Period")) pkg.Map("Driver Contract Validity Period", "DriverContractValidityPeriod");
                if (Fields.Contains("Is Active")) pkg.Map("Is Active", "IsActive");
                if (Fields.Contains("Created By")) pkg.Map("Created By", "CreatedByFullName");
                if (Fields.Contains("Created Date")) pkg.Map("Created Date", "CreatedDate");
                if (Fields.Contains("Updated By")) pkg.Map("Updated By", "UpdatedByFullName");
                if (Fields.Contains("Updated Date")) pkg.Map("Updated Date", "UpdatedDate");
                if (Fields.Contains("Remarks")) pkg.Map("Remarks", "Remarks");
                if (Fields.Contains("Performance Level")) pkg.Map("Performance Level", "PerformanceLevel");

                var ws = pkg.CreateWorksheet("ExportCustom");
                pkg.WriteHeader(ws);
                pkg.WriteRowsNested(ws, records, (data) =>
                {
                    if (data.ExcelHeaderName == "Birth Date")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.DateOfBirth == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "Join Date")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.JoinDate == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "DRIVING LICENSE VALIDITY PERIOD")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.DrivingLicensePeriod == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "Driver Contract Validity Period")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.DriverContractValidityPeriod == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "Created Date")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.CreatedDate == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "Updated Date")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.UpdatedDate == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "Is Active")
                    {
                        return (bool)data.Value ? "Active" : "Not Active";
                    }

                    return data.Value;
                });
                pkg.AutoSizeColumns(ws);

                string path = "TransportDriverManagement" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));

                return Json(path, JsonRequestBehavior.AllowGet);
            }

            //DataTableToExcel dataTableToExcel = new DataTableToExcel();
            //string path = "TransportDriverManagement" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            //dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path);

            //return Json(path, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportToExcelDefault(string IDVendor, string Name, string RoleDriver, string ID, string IncludeInActive)
        {
            var IsActive = false;
            if (!GetListUserRole().Select(x => x.IDRole.ToString()).Contains("1"))
            {
                IsActive = true;
            }

            var input = new TransportDriverManagementInput()
            {
                VendorID = IDVendor,
                Name = Name,
                RoleDriver = RoleDriver,
                ID = ID,
                IncludeInActive = IncludeInActive,
                IsExportOrSearch = true,
                UserRole = GetListUserRole().FirstOrDefault().RoleName,
                IsActive = IsActive
            };

            //List<TransportDriverManagementDTO> records = _transportDriverManagementBLL.GetTransportDriverManagements(input);
            var transportDriverManagement = _transportDriverManagementBLL.GetTransportationDriverManagementsDataTable(input, null);
            List<TransportDriverManagementDTO> records = transportDriverManagement.data;

            using (var pkg = new ExcelExportHelper())
            {
                pkg.Map("Driver Name", "Name");
                pkg.Map("Vendor Name", "MasterVendor.VendorName");
                pkg.Map("Driver License Expired Date", "DrivingLicensePeriod");
                pkg.Map("Status", "IsActive");
                pkg.Map("Role Driver", "RoleDriver");

                var ws = pkg.CreateWorksheet("Export");
                pkg.WriteHeader(ws);
                pkg.WriteRowsNested(ws, records, (data) =>
                {
                    if (data.ExcelHeaderName == "Driver License Expired Date")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.DrivingLicensePeriod == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "Status")
                    {
                        return (bool)data.Value ? "Active" : "Not Active";
                    }

                    return data.Value;
                });
                pkg.AutoSizeColumns(ws);

                var fname = "TransportDriverManagement" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + fname));

                return Json(fname, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMasterVendor(MasterVendorTOMInput criteria)
        {
            return Json(_transportDriverManagementBLL.GetMasterVendor(GetListUserRole()), JsonRequestBehavior.AllowGet);            
        }

        public ActionResult GetMasterList(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetMasterLists(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

    }
}
