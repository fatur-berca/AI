using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DFIS.Universal.Domain.Outputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using DFIS.Universal.Domain.Inputs;
using hms_tom_dev.Models.Masters;
using hms_tom_dev.Models.Transport;
using hms_tom_dev.Util;
using TOM.Transport.BusinessLogics;
using TOM.Master.BusinessLogics;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.DTOs;
using hms_tom_dev.Helper;
using TOM.Master.Domain.Inputs;

namespace hms_tom_dev.Controllers
{
    public class VehicleDataController : BaseController
    {
        private readonly ITransportVehicleDataBLL _transportVehicleDataBLL;
        private readonly ITransportUnitFrequentBLL _transportUnitFrequentBLL;
        private readonly IMasterListBLL _masterListBLL;
        private readonly IMasterUserBLL _masterUserBLL;
        private readonly IMasterVendorTOMBLL _masterVendorBLL;

        public VehicleDataController(ITransportVehicleDataBLL transportVehicleDataBLL, IMasterVendorTOMBLL masterVendorBLL, IMasterUserBLL masterUserBLL, ITransportUnitFrequentBLL transportUnitFrequentBLL, IMasterListBLL masterListBLL)
        {
            _transportVehicleDataBLL = transportVehicleDataBLL;
            _transportUnitFrequentBLL = transportUnitFrequentBLL;
            _masterListBLL = masterListBLL;
            _masterUserBLL = masterUserBLL;
            _masterVendorBLL = masterVendorBLL;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportVehicleData));
        }

        //public ActionResult Index()
        //{
        //    var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
        //    var viewModel = new TransportVehicleDataViewModel();
        //    ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
        //    //ViewBag.ListUserRole = GetListUserRole();
        //    return View(viewModel);
        //}

        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var viewModel = new TransportVehicleDataViewModel();
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            //ViewBag.ListUserRole = GetListUserRole();

            ViewBag.ListUserRole = GetListUserRole();

            var getVendor = _transportVehicleDataBLL.GetMasterVendor(GetListUserRole());
            var getVehilceType = _transportUnitFrequentBLL.GetMasterLists();

            ViewBag.ListVendor = new SelectList(getVendor, "VendorName", "VendorName");
            ViewBag.ListVehcileType = new SelectList(getVehilceType, "FieldValue", "FieldValue");

            return View(viewModel);
        }

        public ActionResult PartialVehicleDataSearch()
        {
            ViewBag.ListUserRole = GetListUserRole();

            return View("_VehicleDataSearch");
        }

        public ActionResult PartialViewCustomReport()
        {
            ViewBag.ListUserRole = GetListUserRole();

            return View("_CustomReport");
        }

        public ActionResult PartialViewCustomReportUF()
        {
            ViewBag.ListUserRole = GetListUserRole();

            return View("_CustomReportUF");
        }

        public ActionResult PartialUnitFrequentDataSearch()
        {
            ViewBag.ListUserRole = GetListUserRole();

            var getVendor = _transportVehicleDataBLL.GetMasterVendor(GetListUserRole());
            var getVehilceType = _transportUnitFrequentBLL.GetMasterLists();

            ViewBag.ListVendor = new SelectList(getVendor, "VendorName", "VendorName");
            ViewBag.ListVehcileType = new SelectList(getVehilceType, "FieldValue", "FieldValue");
            
            return View("_UnitFrequentDataSearch");
        }

        public ActionResult AddNewVehicleData()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var viewModel = new TransportVehicleDataViewModel();

            var getVendor = _transportVehicleDataBLL.GetMasterVendor(GetListUserRole());

            ViewBag.ListVendor = new SelectList(getVendor, "IDVendor", "VendorName");

            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            //ViewBag.ClosingPageDate = GetLockDateRangeList();
            ViewBag.ListLocation = session.Location.Select(x => x.IDLocation).ToList();
            ViewBag.ListUserRole = session.Role.Select(x => x.RoleName).ToList();

            return View(viewModel);
        }

        public ActionResult GetDataById(TransportVehicleDataInput criteria)
        {
            var transportVehicleData = _transportVehicleDataBLL.GetTransportVehicleDatas(criteria);
            var viewModel = new TransportVehicleDataViewModel();
            if (transportVehicleData != null && transportVehicleData.Count > 0)
                viewModel = Mapper.Map<TransportVehicleDataViewModel>(transportVehicleData[0]);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataByCriteria(TransportVehicleDataInput criteria)
        {
            //var criteria = Mapper.Map<TransportVehicleDataViewModel>(input);
            var transportVehicleData = _transportVehicleDataBLL.GetDataByCriteria(criteria);
            //var viewModel = Mapper.Map<TransportVehicleDataViewModel>(transportVehicleData);

            return Json(transportVehicleData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendor()
        {
            return Json(_transportVehicleDataBLL.GetMasterVendor(GetListUserRole()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransportVehicleDatas(TransportVehicleDataInput criteria)
        {
            criteria.UserRole = GetListUserRole().FirstOrDefault().RoleName;
            var transportVehicleData = _transportVehicleDataBLL.GetTransportVehicleDatas(criteria);
            var viewModel = Mapper.Map<List<TransportVehicleDataViewModel>>(transportVehicleData);
            var allVendor = _masterVendorBLL.GetALLMasterVendors();
            foreach (var item in viewModel)
            {
                //var vendor = _transportVehicleDataBLL.GetMasterVendor(GetListUserRole()).Where(x => x.IDVendor == item.IDVendor).FirstOrDefault();
                var vendor = allVendor.Where(x => x.IDVendor == item.IDVendor).FirstOrDefault();
                if (vendor != null)
                {
                    item.Vendor = Mapper.Map<MasterVendorTOMViewModel>(vendor);
                }
                else
                {
                    item.Vendor = new MasterVendorTOMViewModel();
                }
            }

            if (!GetListUserRole().Select(x => x.IDRole.ToString()).Contains("1"))
            {
                viewModel = viewModel.Where(x => x.IsActive).ToList();
            }

            if (criteria.UserRole.Contains(ConfigurationManager.AppSettings["AdminVendor"]))
            {
                viewModel = viewModel.Where(x => criteria.UserRole.ToLower().Contains(x.Vendor.TransportationMode == null ? "" : x.Vendor.TransportationMode.ToLower())).ToList();
            }

            var Summary = new {
                totalLightTruckUnits = viewModel.Where(k => k.Type == "Light Truck").Count(),
                totalStandardUnits = viewModel.Where(k => k.Type == "Standard").Count(),
                totalTrontonLongUnits = viewModel.Where(k => k.Type == "Tronton Long").Count(),
                totalUnitWithMoreThanEightYearsOld = viewModel.Where(k => k.ManufacturingYear < (System.DateTime.Now.Year - 8)).Count(),
                totalUnitWithGPS = viewModel.Where(k => k.GPS == "Yes").Count(),
            };

            var output = Json(new {
                ViewModel = viewModel,
                summary = Summary
            }, JsonRequestBehavior.AllowGet);

            output.MaxJsonLength = int.MaxValue;

            return output;
        }

        public ActionResult GetVehicleDataTableList(TransportVehicleDataInput filter, DataTableModel model)
        {
            filter.UserRole = GetListUserRole().FirstOrDefault().RoleName;
            var IsActive = false;
            if (!GetListUserRole().Select(x => x.IDRole.ToString()).Contains("1"))
            {
                IsActive = true;
            }
            filter.IsActive = IsActive;
            var rawData = _transportVehicleDataBLL.GetVehicleDataTable(filter, model);
            var allVendor = _masterVendorBLL.GetALLMasterVendors();
            var viewModel = Mapper.Map<List<TransportVehicleDataViewModel>>(rawData.data);
            foreach (var tm in viewModel)
            {
                var MstVendor = allVendor.Where(_ => _.IDVendor == tm.IDVendor).FirstOrDefault();
                if (MstVendor != null)
                {
                    tm.Vendor = Mapper.Map<MasterVendorTOMViewModel>(MstVendor);
                }
                else
                {
                    tm.Vendor = new MasterVendorTOMViewModel();
                }
            }

            var result = new
            {
                recordsTotal = rawData.total,
                recordsFiltered = rawData.total,
                draw = int.Parse(Request["draw"]),
                data = viewModel,
                dbResultCount = rawData.total,
            };
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult CountTransportVehicleDatas(TransportVehicleDataInput criteria)
        {
            int totalLightTruckUnits = _transportVehicleDataBLL.CountTransportVehicleDatas(criteria, "totalLightTruckUnits");
            int totalStandardUnits = _transportVehicleDataBLL.CountTransportVehicleDatas(criteria, "totalStandardUnits");
            int totalTrontonLongUnits = _transportVehicleDataBLL.CountTransportVehicleDatas(criteria, "totalTrontonLongUnits");
            int totalUnitwithMoreThanEightYearsOld = _transportVehicleDataBLL.CountTransportVehicleDatas(criteria, "totalUnitwithMoreThanEightYearsOld");
            int totalUnitwithGPS = _transportVehicleDataBLL.CountTransportVehicleDatas(criteria, "totalUnitwithGPS");

            int[] DriverCount = new int[5];
            DriverCount[0] = totalLightTruckUnits;
            DriverCount[1] = totalStandardUnits;
            DriverCount[2] = totalTrontonLongUnits;
            DriverCount[3] = totalUnitwithMoreThanEightYearsOld;
            DriverCount[4] = totalUnitwithGPS;

            return Json(DriverCount, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateRecord(TransportVehicleDataInput input)
        {
            string controller = ControllerContext.RouteData.Values["controller"].ToString();

            var inputData = Mapper.Map<TransportVehicleDataDTO>(input);

            try
            {
                int result = _transportVehicleDataBLL.UpdateTransportVehicleData(inputData, controller, GetUserId());
                if(result == 0)
                    return Json(Enums.ResponseType.Success.ToString(), JsonRequestBehavior.AllowGet);
                if(result == 1)
                    return Json(Enums.ResponseType.DoubleData.ToString(), JsonRequestBehavior.AllowGet);
                return Json(Enums.ResponseType.Error.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SetInactiveRecovery()
        {
            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                string[] IDPoliceRegNumbers = Request.Params["IDPoliceRegNumbers"].Split(',');
                foreach (string v in IDPoliceRegNumbers)
                {
                    var inputData = new TransportVehicleDataDTO
                    {
                        IDPoliceRegNumber = v,
                        IsActive = Boolean.Parse(Request.Params["IsActive"]),
                        CreatedBy = GetUserId(),
                        UpdatedBy = GetUserId()
                    };
                    
                    _transportVehicleDataBLL.SetInactiveRecovery(inputData, controller, GetUserId());
                    
                }

                return Json(Enums.ResponseType.Success.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ExportToExcel(TransportVehicleDataInput criteria)
        {
            //List<TransportVehicleDataDTO> recordsDTO = _transportVehicleDataBLL.GetTransportVehicleDatas(criteria);
            List<TransportVehicleDataDTO> recordsDTO = _transportVehicleDataBLL.GetTransportVehicleDatas2(criteria);

            if (!GetListUserRole().Select(x => x.IDRole.ToString()).Contains("1"))
            {
                recordsDTO = recordsDTO.Where(x => x.IsActive).ToList();
            }
            //var records = Mapper.Map<List<TransportVehicleDataViewModel>>(recordsDTO).OrderBy(a => a.IDPoliceRegNumber);
         
            //List<TransportVehicleDataViewModel> apakek = records.ToList();
            //foreach (var rec in records)
            //{
            //    var vendor = _transportVehicleDataBLL.GetMasterVendor(GetListUserRole()).Where(x => x.IDVendor == rec.IDVendor).FirstOrDefault();
            //    rec.Vendor = vendor != null ? Mapper.Map<MasterVendorTOMViewModel>(vendor) : new MasterVendorTOMViewModel();
            //    rec.CreatedByFullName = _masterUserBLL.GetMasterUsers(new MasterUserInput { IDUser = rec.CreatedBy }).Count > 0 ? _masterUserBLL.GetMasterUsers(new MasterUserInput { IDUser = rec.CreatedBy }).Single().FullName : "";
            //    rec.UpdatedByFullName = _masterUserBLL.GetMasterUsers(new MasterUserInput { IDUser = rec.UpdatedBy }).Count > 0 ? _masterUserBLL.GetMasterUsers(new MasterUserInput { IDUser = rec.UpdatedBy }).Single().FullName : "";
            //}

            DataTable boundTable = new DataTable();

            string columns = Request.Params["Columns"].ToString();
            if (string.IsNullOrEmpty(columns))
            {
                columns = "Vendor Name,Police Registration Number,Manufacturing Year,Karoseri,Unit Brand,Unit Type,Cargo,Vehicle Identity Number,BaseTown,STNK Expired Date,GPS,Is Active,Created By,Created Date,Updated By,Updated Date,Remarks";
            }

            string[] Fields = columns.Split(',');
            #region OLDCODE
            //foreach (string Field in Fields)
            //{
            //    boundTable.Columns.Add(Field, typeof(string));
            //}

            //foreach (TransportVehicleDataDTO record in records)
            //{
            //    var dict = new Dictionary<string, string>();

            //    dict["Police Registration Number"] = record.IDPoliceRegNumber;
            //    dict["Manufacturing Year"] = record.ManufacturingYear.ToString();
            //    dict["Karoseri"] = record.Karoseri;
            //    dict["Unit Brand"] = record.Merk;
            //    dict["Unit Type"] = record.Type;
            //    dict["Cargo"] = record.Cargo;
            //    dict["Vehicle Identity Number"] = record.VehicleIdentityNumber;
            //    dict["BaseTown"] = record.BaseTown;
            //    dict["STNK Expired Date"] = record.STNKValidityPeriod.ToString("MM/dd/yyyy HH:mm:ss.fff");
            //    dict["GPS"] = record.GPS;
            //    dict["Status"] = record.Status;
            //    dict["Is Active"] = record.IsActive.ToString();
            //    dict["Created By"] = record.CreatedBy;
            //    dict["Created Date"] = record.CreatedDate.ToString("MM/dd/yyyy HH:mm:ss.fff");
            //    dict["Updated By"] = record.UpdatedBy;
            //    dict["Updated Date"] = record.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss.fff");
            //    dict["Remarks"] = record.Remarks;

            //    dynamic dr = boundTable.NewRow();
            //    foreach (string Field in Fields)
            //    {
            //        dr[Field] = dict[Field];
            //    }
            //    boundTable.Rows.Add(dr);
            //}

            //DataTableToExcel dataTableToExcel = new DataTableToExcel();
            //string path = "TransportVehicleData" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            //dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path);
            #endregion

            using (var pkg = new ExcelExportHelper())
            {
                if (Fields.Contains("Vendor Name")) pkg.Map("Vendor Name", "VendorName");
                if (Fields.Contains("Police Registration Number")) pkg.Map("Police Registration Number", "IDPoliceRegNumber");
                if (Fields.Contains("Manufacturing Year")) pkg.Map("Manufacturing Year", "ManufacturingYear");
                if (Fields.Contains("Karoseri")) pkg.Map("Karoseri", "Karoseri");
                if (Fields.Contains("Unit Brand")) pkg.Map("Unit Brand", "Merk");
                if (Fields.Contains("Unit Type")) pkg.Map("Unit Type", "Type");
                if (Fields.Contains("Cargo")) pkg.Map("Cargo", "Cargo");
                if (Fields.Contains("Vehicle Identity Number")) pkg.Map("Vehicle Identity Number", "VehicleIdentityNumber");
                if (Fields.Contains("BaseTown")) pkg.Map("BaseTown", "BaseTown");
                if (Fields.Contains("STNK Expired Date")) pkg.Map("STNK Expired Date", "STNKValidityPeriod");
                if (Fields.Contains("GPS")) pkg.Map("GPS", "GPS");
                if (Fields.Contains("Status")) pkg.Map("Status", "Status");
                if (Fields.Contains("Is Active")) pkg.Map("Is Active", "IsActive");
                if (Fields.Contains("Created By")) pkg.Map("Created By", "CreatedByFullName");
                if (Fields.Contains("Created Date")) pkg.Map("Created Date", "CreatedDate");
                if (Fields.Contains("Updated By")) pkg.Map("Updated By", "UpdatedByFullName");
                if (Fields.Contains("Updated Date")) pkg.Map("Updated Date", "UpdatedDate");
                if (Fields.Contains("Remarks")) pkg.Map("Remarks", "Remarks");
                if (Fields.Contains("Attachment STNK")) pkg.Map("Attachment STNK", "AttachmentSTNK");
                if (Fields.Contains("Attachment Front View")) pkg.Map("Attachment Front View", "AttachmentPhoto");
                if (Fields.Contains("Attachment Side View")) pkg.Map("Attachment Side View", "AttachmentPhotoTwo");

                var ws = pkg.CreateWorksheet("Export");
                pkg.WriteHeader(ws);
                pkg.WriteRowsNested(ws, recordsDTO, (data) =>
                {
                    if (data.ExcelHeaderName == "STNK Expired Date")
                    {
                        data.Cell.Style.Numberformat.Format = "dd-MMM-yyyy";
                        if (data.OriginalObject.STNKValidityPeriod == DateTime.MinValue)
                            return null;
                    }
                    if (data.ExcelHeaderName == "Is Active")
                    {
                        return (bool)data.Value ? "Active" : "Not Active";
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

                    return data.Value;
                });
                pkg.AutoSizeColumns(ws);

                string path = "TransportVehicleData" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
                pkg.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
                
                return Json(path, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ExportToExcelUF(TransportUnitFrequentInput criteria)
        {
            if (criteria.TransactionDate == null || criteria.TransactionDate == "")
            {
                criteria.TransactionDate = DateTime.Now.ToString();
            }
            else
            {
                criteria.TransactionDate = criteria.TransactionDate + " 00:00";
            }

            DateTime inputdate;
            inputdate = Convert.ToDateTime(criteria.TransactionDate);

            var records = _transportUnitFrequentBLL.GetViewUnitFrequent(criteria);

            DataTable boundTable = new DataTable();

            string columns = Request.Params["Columns"].ToString();
            if (string.IsNullOrEmpty(columns))
            {
                columns = "Police Registration Number,Vehicle Type,Vendor,Day 1,Day 2,Day 3,Day 4,Day 5,Day 6,Day 7";
            }

            string[] integerFields = { "Day 1", "Day 2", "Day 3", "Day 4", "Day 5", "Day 6", "Day 7" };
            string theField = "";

            int number = 0;

            string[] Fields = columns.Split(',');
            foreach (string Field in Fields)
            {
                if (integerFields.Contains(Field))
                {
                    number = Convert.ToInt16(Field.Substring(4));
                    theField = Convert.ToString(inputdate.AddDays((1 - number)).Date.ToString("dd-MMM-yyyy"));

                    boundTable.Columns.Add(theField, typeof(int));
                }
                else
                {
                    boundTable.Columns.Add(Field, typeof(string));
                }
            }

            foreach (TransportUnitFrequentDTO record in records)
            {
                var dict = new Dictionary<string, string>();

                dict["Police Registration Number"] = record.IDPoliceRegNumber;
                dict["Vehicle Type"] = record.VehicleType;
                dict["Vendor"] = record.Vendor;
                dict[Convert.ToString(inputdate.Date.ToString("dd-MMM-yyyy"))] = record.C1.ToString();
                dict[Convert.ToString(inputdate.AddDays(-1).Date.ToString("dd-MMM-yyyy"))] = record.C2.ToString();
                dict[Convert.ToString(inputdate.AddDays(-2).Date.ToString("dd-MMM-yyyy"))] = record.C3.ToString();
                dict[Convert.ToString(inputdate.AddDays(-3).Date.ToString("dd-MMM-yyyy"))] = record.C4.ToString();
                dict[Convert.ToString(inputdate.AddDays(-4).Date.ToString("dd-MMM-yyyy"))] = record.C5.ToString();
                dict[Convert.ToString(inputdate.AddDays(-5).Date.ToString("dd-MMM-yyyy"))] = record.C6.ToString();
                dict[Convert.ToString(inputdate.AddDays(-6).Date.ToString("dd-MMM-yyyy"))] = record.C7.ToString();

                dynamic dr = boundTable.NewRow();
                foreach (string Field in Fields)
                {
                    if (integerFields.Contains(Field))
                    {
                        number = Convert.ToInt16(Field.Substring(4));

                        theField = Convert.ToString(inputdate.AddDays((1 - number)).Date.ToString("dd-MMM-yyyy"));

                        dr[theField] = Convert.ToInt16(dict[theField]);
                    }
                    else
                    {
                        dr[Field] = dict[Field];
                    }
                }
                boundTable.Rows.Add(dr);
            }

            DataTableToExcel dataTableToExcel = new DataTableToExcel();
            string path = "TransportUnitFrequent" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path);

            return Json(path, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadPhoto()
        {
            string attachmentLocation = null;
            string output = null;

            try
            {
                var uploadPath = Server.MapPath("~/Assets/Uploads/VehicleData/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase fileContent = Request.Files[file];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var stream = fileContent.InputStream;

                        string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "photo";
                        string strFileName = fileContent.FileName;

                        if (strFileName.Contains("\\"))
                        {
                            strFileName = strFileName.Split('\\')[strFileName.Split('\\').Length - 1];
                        }

                        string extension = Path.GetExtension(strFileName);

                        var location = ConfigurationManager.AppSettings["LocationVehicleData"];

                        attachmentLocation = location + fileName + extension;

                        var path = Path.Combine(Server.MapPath(location), fileName + extension);
                        
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
                output = attachmentLocation;
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                output = "Upload failed";
            }

            return Json(output);
        }

        [HttpPost]
        public JsonResult UploadPhotoTwo()
        {
            string attachmentLocation = null;
            string output = null;

            try
            {
                var uploadPath = Server.MapPath("~/Assets/Uploads/VehicleData/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase fileContent = Request.Files[file];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var stream = fileContent.InputStream;

                        string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "photoTwo";
                        string strFileName = fileContent.FileName;

                        if (strFileName.Contains("\\"))
                        {
                            strFileName = strFileName.Split('\\')[strFileName.Split('\\').Length - 1];
                        }

                        string extension = Path.GetExtension(strFileName);

                        var location = ConfigurationManager.AppSettings["LocationVehicleData"];

                        attachmentLocation = location + fileName + extension;

                        var path = Path.Combine(Server.MapPath(location), fileName + extension);

                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
                output = attachmentLocation;
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                output = "Upload failed";
            }

            return Json(output);
        }

        [HttpPost]
        public JsonResult UploadPhotoThree()
        {
            string attachmentLocation = null;
            string output = null;

            try
            {
                var uploadPath = Server.MapPath("~/Assets/Uploads/VehicleData/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase fileContent = Request.Files[file];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var stream = fileContent.InputStream;

                        string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "photoThree";
                        string strFileName = fileContent.FileName;

                        if (strFileName.Contains("\\"))
                        {
                            strFileName = strFileName.Split('\\')[strFileName.Split('\\').Length - 1];
                        }

                        string extension = Path.GetExtension(strFileName);

                        var location = ConfigurationManager.AppSettings["LocationVehicleData"];

                        attachmentLocation = location + fileName + extension;

                        var path = Path.Combine(Server.MapPath(location), fileName + extension);

                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
                output = attachmentLocation;
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                output = "Upload failed";
            }

            return Json(output);
        }

        [HttpPost]
        public JsonResult UploadSTNK()
        {
            string attachmentLocation = null;
            string output = null;

            try
            {
                var uploadPath = Server.MapPath("~/Assets/Uploads/VehicleData/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase fileContent = Request.Files[file];

                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var stream = fileContent.InputStream;

                        string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + "stnk";
                        string strFileName = fileContent.FileName;

                        if (strFileName.Contains("\\"))
                        {
                            strFileName = strFileName.Split('\\')[strFileName.Split('\\').Length - 1];
                        }

                        string extension = Path.GetExtension(strFileName);

                        var location = ConfigurationManager.AppSettings["LocationVehicleData"];

                        attachmentLocation = location + fileName + extension;

                        var path = Path.Combine(Server.MapPath(location), fileName + extension);

                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
                output = attachmentLocation;
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                output = "Upload failed";
            }

            return Json(output);
        }
        public ActionResult GenerateColumn(string trdate)
        {
            DateTime inputdate;
            if (trdate == null || trdate == "")
            {
                inputdate = DateTime.Now;
            }
            else
            {
                inputdate = Convert.ToDateTime(trdate + " 00:00");
            }
            DateTime[] arraydatecolumn = new DateTime[7];

            arraydatecolumn[0] = inputdate;
            arraydatecolumn[1] = inputdate.AddDays(-1);
            arraydatecolumn[2] = inputdate.AddDays(-2);
            arraydatecolumn[3] = inputdate.AddDays(-3);
            arraydatecolumn[4] = inputdate.AddDays(-4);
            arraydatecolumn[5] = inputdate.AddDays(-5);
            arraydatecolumn[6] = inputdate.AddDays(-6);
            
            return Json(arraydatecolumn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnitFrequent(TransportUnitFrequentInput criteria)
        {

            if (criteria.TransactionDate == null || criteria.TransactionDate == "")
            {
                criteria.TransactionDate = DateTime.Now.ToString();
            }
            else
            {
                criteria.TransactionDate = criteria.TransactionDate + " 00:00";
            }
            var getData = _transportUnitFrequentBLL.GetViewUnitFrequent(criteria);
          
            return Json(getData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterVendors()
        {
            var getdata = _transportVehicleDataBLL.GetMasterVendor(GetListUserRole());

            return Json(getdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportUnitFreq(TransportUnitFrequentInput criteria)
        {
            if (criteria.TransactionDate == null || criteria.TransactionDate == "")
            {
                criteria.TransactionDate = DateTime.Now.ToString();
            }
            else
            {
                criteria.TransactionDate = criteria.TransactionDate + " 00:00";
            }
            
            DateTime inputdate;
            inputdate = Convert.ToDateTime(criteria.TransactionDate);
           
            var dbResult = _transportUnitFrequentBLL.GetViewUnitFrequent(criteria);

            DataTable boundTable = new DataTable();
            boundTable.Columns.Add("PoliceRegNumber", typeof(string));
            boundTable.Columns.Add("VehicleType", typeof(string));
            boundTable.Columns.Add("Vendor", typeof(string));
            boundTable.Columns.Add(Convert.ToString(inputdate.Date.ToString("dd-MMM-yyyy")), typeof(int));
            boundTable.Columns.Add(Convert.ToString(inputdate.AddDays(-1).Date.ToString("dd-MMM-yyyy")), typeof(int));
            boundTable.Columns.Add(Convert.ToString(inputdate.AddDays(-2).Date.ToString("dd-MMM-yyyy")), typeof(int));
            boundTable.Columns.Add(Convert.ToString(inputdate.AddDays(-3).Date.ToString("dd-MMM-yyyy")), typeof(int));
            boundTable.Columns.Add(Convert.ToString(inputdate.AddDays(-4).Date.ToString("dd-MMM-yyyy")), typeof(int));
            boundTable.Columns.Add(Convert.ToString(inputdate.AddDays(-5).Date.ToString("dd-MMM-yyyy")), typeof(int));
            boundTable.Columns.Add(Convert.ToString(inputdate.AddDays(-6).Date.ToString("dd-MMM-yyyy")), typeof(int));

            foreach (TransportUnitFrequentDTO item in dbResult)
            {
                dynamic dr = boundTable.NewRow();
                dr["PoliceRegNumber"] = item.policenumber;
                dr["VehicleType"] = item.VehicleType;
                dr["Vendor"] = item.Vendor;
                dr[Convert.ToString(inputdate.Date.ToString("dd-MMM-yyyy"))] = item.C1;
                dr[Convert.ToString(inputdate.AddDays(-1).Date.ToString("dd-MMM-yyyy"))] = item.C2;
                dr[Convert.ToString(inputdate.AddDays(-2).Date.ToString("dd-MMM-yyyy"))] = item.C3;
                dr[Convert.ToString(inputdate.AddDays(-3).Date.ToString("dd-MMM-yyyy"))] = item.C4;
                dr[Convert.ToString(inputdate.AddDays(-4).Date.ToString("dd-MMM-yyyy"))] = item.C5;
                dr[Convert.ToString(inputdate.AddDays(-5).Date.ToString("dd-MMM-yyyy"))] = item.C6;
                dr[Convert.ToString(inputdate.AddDays(-6).Date.ToString("dd-MMM-yyyy"))] = item.C7;
             
                boundTable.Rows.Add(dr);
            }

            DataTableToExcel dataTableToExcel = new DataTableToExcel();
            string path = "TransportUnitFrequent" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path);

            return Json(path, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterLists()
        {
            var getdata = _transportUnitFrequentBLL.GetMasterLists();

            return Json(getdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalUnitUsed(TransportUnitFrequentInput criteria)
        {
            if (criteria.TransactionDate == null || criteria.TransactionDate == "")
            {
                criteria.TransactionDate = DateTime.Now.ToString();
            }
            else
            {
                criteria.TransactionDate = criteria.TransactionDate + " 00:00";
            }
            var getdata = _transportUnitFrequentBLL.GetViewUnitFrequent(criteria);
            string totalunitused;
            string minused;
            string maxused;
            try
            {
                int calculatetotal = getdata.Sum(x => x.C1).Value + getdata.Sum(x => x.C2).Value + getdata.Sum(x => x.C3).Value + getdata.Sum(x => x.C4).Value + getdata.Sum(x => x.C5).Value + getdata.Sum(x => x.C6).Value + getdata.Sum(x => x.C7).Value;
                totalunitused = calculatetotal.ToString();

            }
            catch 
            {
                 totalunitused = "0";
            }

            try
            {
                
                 minused = getdata.Where(m => m.C1 > 0 || m.C2 > 0 || m.C3 > 0 || m.C4 > 0 || m.C5 > 0 || m.C6 > 0 || m.C7 > 0).Min(x => x.C1 + x.C2 + x.C3 + x.C4 + x.C5 + x.C6 + x.C7).ToString();
                 if (minused == "")
                 {
                     minused = "0";
                 }

            }
            catch
            {
                minused = "0";
            }

            try
            {
                 maxused = getdata.Max(x => x.C1 + x.C2 + x.C3 + x.C4 + x.C5 + x.C6 + x.C7).ToString();
            }
            catch
            {
                maxused = "0";
            }
            //var totalunitused = getdata.Sum(x => x.C3);

         

            var getSummary = new ListSummaryUnitFreq()
            {
                MaxFreq = maxused,
                MinFreq = minused,
                TotalFreq = totalunitused

            };


            return Json(getSummary, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMinUsed(TransportUnitFrequentInput criteria)
        {
            if (criteria.TransactionDate == null || criteria.TransactionDate == "")
            {
                criteria.TransactionDate = DateTime.Now.ToString();
            }
            else
            {
                criteria.TransactionDate = criteria.TransactionDate + " 00:00";
            }
            var getdata = _transportUnitFrequentBLL.GetViewUnitFrequent(criteria);
    
            int minused = getdata.Where(m => m.C1 > 0 || m.C2 > 0 || m.C3 > 0 || m.C4 > 0 || m.C5 > 0 || m.C6 > 0 || m.C7 > 0).Min(x => x.C1 + x.C2 + x.C3 + x.C4 + x.C5 + x.C6 + x.C7).Value;
            //var totalunitused = getdata.Sum(x => x.C3);
            return Json(minused, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaxUsed(TransportUnitFrequentInput criteria)
        {
            if (criteria.TransactionDate == null || criteria.TransactionDate == "")
            {
                criteria.TransactionDate = DateTime.Now.ToString();
            }
            else
            {
                criteria.TransactionDate = criteria.TransactionDate + " 00:00";
            }
            var getdata = _transportUnitFrequentBLL.GetViewUnitFrequent(criteria);
            int maxused= getdata.Max(x => x.C1 + x.C2 + x.C3 + x.C4 + x.C5 + x.C6 + x.C7).Value;
            //var totalunitused = getdata.Sum(x => x.C3);
            return Json(maxused, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterListByFieldName(MasterListInput criteria)
        {
            var masterLists = _masterListBLL.GetMasterListByFieldName(criteria);
            var viewModel = Mapper.Map<List<MasterListViewModel>>(masterLists);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
    }
    public class ListSummaryUnitFreq
    {
        public string TotalFreq { get; set; }
        public string MaxFreq { get; set; }
        public string MinFreq { get; set; }
    }
}
