using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using AutoMapper;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using hms_tom_dev.Models.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using hms_tom_dev.Models.Transport;
using TOM.Transport.BusinessLogics.TransportTicketNCRBLL;
using DFIS.Universal.Domain.Outputs;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.DTOs;
using hms_tom_dev.Util;

namespace hms_tom_dev.Controllers
{
    public class TicketNcrController : BaseController
    {
        private readonly ITransportTicketNCRBLL _transportTicketNcrbll;

        public TicketNcrController(ITransportTicketNCRBLL transportTicketNcrbll)
        {
            _transportTicketNcrbll = transportTicketNcrbll;
            SetPage(EnumHelper.GetDescription(Enums.PageName.TransportTicketNcr));
        }

        public ActionResult Index()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];

            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();

            var getvendor = _transportTicketNcrbll.GetMasterVendor();
            var getticketcategory = _transportTicketNcrbll.GetTicketCategory();
            var getaccidentcategory = _transportTicketNcrbll.GetAccidentCategorys();

            ViewBag.ListVendor = new SelectList(getvendor, "IDVendor", "VendorName");
            ViewBag.ListTicketCategory = new SelectList(getticketcategory, "FieldValue", "FieldValue");
            ViewBag.ListAccidentCategory = new SelectList(getaccidentcategory, "FieldValue", "FieldValue");

            return View();
        }

        public ActionResult PartialViewCustomReport()
        {
            ViewBag.ListUserRole = GetListUserRole();

            return View("_CustomReport");
        }

        [HttpPost]
        public ActionResult SetStatusResult(TransportTicketNCRInput filter)
        {
            try
            {
                _transportTicketNcrbll.SetActive(filter.ListIsActiveIdString, filter.IsActive);

                return Json(Enums.ResponseType.Success.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (ExceptionBase ex)
            {
                string msg = (ex.InnerException == null) ? ex.Message : Enums.ResponseType.Error.ToString();

                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TransportTicketNcrInput()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            //ViewBag.ClosingPageDate = GetLockDateRangeList();
            //ViewBag.ListLocation = session.Location.Select(x => x.IDLocation).ToList();
            //ViewBag.ListUserRole = session.Role.Select(x => x.RoleName).ToList();
            return View("Input");
        }

        public ActionResult TransportTicketNcrEdit()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            ViewBag.PageAccess = session.Page.Select(x => x.FunctionName).ToList();
            //ViewBag.ClosingPageDate = GetLockDateRangeList();
            //ViewBag.ListLocation = session.Location.Select(x => x.IDLocation).ToList();
            ViewBag.ListUserRole = session.Role.Select(x => x.RoleName).ToList();
            List<ListRoadCondition> getlisroadcon = _transportTicketNcrbll.GetRoadConditions().Select(x => new ListRoadCondition { IDList = x.IDList, FieldName = x.FieldName, FieldValue = x.FieldValue }).ToList();

            var getvendor = _transportTicketNcrbll.GetMasterVendor();
            var getticketcategory = _transportTicketNcrbll.GetTicketCategory();
            var getoffenderrole = _transportTicketNcrbll.GetOffenderRoles();
            var getvehicletype = _transportTicketNcrbll.Getvehicletypes();
            var getoffenderyear = _transportTicketNcrbll.GetOffenderYOS();
            var getlocationcategory = _transportTicketNcrbll.GetLocationCategorys();
            var gettypeoflocation = _transportTicketNcrbll.GetLocationTypes();
            var getroadcondition = _transportTicketNcrbll.GetRoadConditions();
            var getweathercondition = _transportTicketNcrbll.GetWeatherConditions();
            var getaccidentcategory = _transportTicketNcrbll.GetAccidentCategorys();
            var getroot = _transportTicketNcrbll.GetAnalystProblems();
            var getmainproblem = _transportTicketNcrbll.GetMainProblems();

            ViewBag.ListVendor = new SelectList(getvendor, "IDVendor", "VendorName");
            ViewBag.ListTicketCategory = new SelectList(getticketcategory, "FieldValue", "FieldValue");
            ViewBag.ListOffenderRole = new SelectList(getoffenderrole, "FieldValue", "FieldValue");
            ViewBag.ListVehicleType = new SelectList(getvehicletype, "FieldValue", "FieldValue");
            ViewBag.ListOffenderYOS = new SelectList(getoffenderyear, "FieldValue", "FieldValue");
            ViewBag.ListLocationCategory = new SelectList(getlocationcategory, "FieldValue", "FieldValue");
            ViewBag.ListLocationType = new SelectList(gettypeoflocation, "FieldValue", "FieldValue");
            ViewBag.ListRoadCondition = new SelectList(getroadcondition, "FieldValue", "FieldValue");
            ViewBag.ListWeatherCondition = new SelectList(getweathercondition, "FieldValue", "FieldValue");
            ViewBag.ListAccidentCategory = new SelectList(getaccidentcategory, "FieldValue", "FieldValue");
            ViewBag.ListRoot = new SelectList(getroot, "FieldValue", "FieldValue");
            ViewBag.ListMainProblem = new SelectList(getmainproblem, "FieldValue", "FieldValue");

            //TransportTicketNCRViewModel viewmodel = new TransportTicketNCRViewModel()
            //{
            //    Location = "Tesss",
            //    ListRoad= getlisroadcon
            //};
            //return View("Edit", viewmodel);

            return View("Edit");
        }

        public ActionResult GetMasterListByFieldName(string fieldName)
        {
            return Json(_transportTicketNcrbll.GetMasterListByFieldName(fieldName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterLocation()
        {
            return Json(_transportTicketNcrbll.GetMasterLocation(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterVendor()
        {
            return Json(_transportTicketNcrbll.GetMasterVendor(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetALLTransportTicketNcr(TransportTicketNCRInput filter)
        {
            //return Json(_transportTicketNcrbll.GetALLTransportTicketNcr(filter), JsonRequestBehavior.AllowGet);
            return Json("");
        }

        public ActionResult GetTransportTicketNcrListColumnName()
        {
            //return Json(_transportTicketNcrbll.GetKPIProductivityListColumnName(), JsonRequestBehavior.AllowGet);
            return Json("");
        }

        public ActionResult GetDropTicketCatergory()
        {
            var dropForTicketCategory = _transportTicketNcrbll.GetTicketCategory();

            return Json(dropForTicketCategory, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOffenderYOS()
        {
            var dropForTicketCategory = _transportTicketNcrbll.GetOffenderYOS();

            return Json(dropForTicketCategory, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getTicketnumber()
        {
            var generateTicket = _transportTicketNcrbll.getTicketnumbers();

            return Json(generateTicket, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getNonTicketnumber()
        {
            var generateTicket = _transportTicketNcrbll.getNonTicketnumbers();

            return Json(generateTicket, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getvehicletype()
        {
            var generatevehicletype = _transportTicketNcrbll.Getvehicletypes();

            return Json(generatevehicletype, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocationType()
        {
            var generatevehicletype = _transportTicketNcrbll.GetLocationTypes();

            return Json(generatevehicletype, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocationCategory()
        {
            var generatevehicletype = _transportTicketNcrbll.GetLocationCategorys();

            return Json(generatevehicletype, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRoadCondition()
        {
            var generatevehicletype = _transportTicketNcrbll.GetRoadConditions();

            return Json(generatevehicletype, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWeatherCondition()
        {
            var generatevehicletype = _transportTicketNcrbll.GetWeatherConditions();

            return Json(generatevehicletype, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAccidentCategory()
        {
            var generatevehicletype = _transportTicketNcrbll.GetAccidentCategorys();

            return Json(generatevehicletype, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTicketNCR(TransportTicketNCRInput category)
        {
            var generateTicketNCR = _transportTicketNcrbll.GetTicketNCRs(category);
            return Json(generateTicketNCR, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTicketNCRTableList(TransportTicketNCRInput filter, DataTableModel model)
        {
            var rawData = _transportTicketNcrbll.GetTicketNCRsTable(filter, model);
            var result = new
            {
                recordsTotal = rawData.total,
                recordsFiltered = rawData.total,
                draw = int.Parse(Request["draw"]),
                data = rawData.data,
                dbResultCount = rawData.total,
            };
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetSumTicket(TransportTicketNCRInput category)
        {
            var generateTicketNCR = _transportTicketNcrbll.GetSumTickets(category);
            var generateNCR = _transportTicketNcrbll.GetCountNRCNumbers(category);
            var generateAccident = _transportTicketNcrbll.GetTotalAncidents(category);
            var generateIncident = _transportTicketNcrbll.GetSumAccidents(category);

            var getdata = new GetCalculateSummary()
            {
                TotalCrash = generateIncident,
                TotalIncident = generateAccident,
                TotalNCR = generateNCR,
                TotalTicket = generateTicketNCR
            };

            return Json(getdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCountNRCNumber(TransportTicketNCRInput category)
        {
            var generateTicketNCR = _transportTicketNcrbll.GetCountNRCNumbers(category);
            return Json(generateTicketNCR, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalAccident(TransportTicketNCRInput category)
        {
            var generateTicketNCR = _transportTicketNcrbll.GetTotalAncidents(category);
            return Json(generateTicketNCR, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSumAccident(TransportTicketNCRInput category)
        {
            var generateTicketNCR = _transportTicketNcrbll.GetSumAccidents(category);
            return Json(generateTicketNCR, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOffenderRole()
        {
            var generateTicketNCR = _transportTicketNcrbll.GetOffenderRoles();
            return Json(generateTicketNCR, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAnalisystProblem()
        {
            var generateTicketNCR = _transportTicketNcrbll.GetAnalystProblems();
            return Json(generateTicketNCR, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMainProblem()
        {
            var generateTicketNCR = _transportTicketNcrbll.GetMainProblems();
            return Json(generateTicketNCR, JsonRequestBehavior.AllowGet);
        }

        public void ExportReport(int monthF, int monthT, int year, string active)
        {
            HttpResponse response = System.Web.HttpContext.Current.Response;
            HttpResponse resp = System.Web.HttpContext.Current.Response;
            var fileName = "TransportTicketNcr" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            resp.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            resp.AppendHeader("content-disposition", "attachment; filename=" + fileName);

            response.Flush();
            response.End();
        }

        //[HttpPost]
        //public ActionResult SetActive(KPIProductivityInput filter)
        //{
        //    return Json("");
        //}

        [HttpPost]
        public ActionResult InsertTransTicketNcr(InsertUpdateData<TransportTicketNCRViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var trsTicketNcr = Mapper.Map<TransportTicketNCRDTO>(bulkData.New[i]);

                    //set createdby and updatedby

                    trsTicketNcr.CreatedBy = GetUserId();
                    trsTicketNcr.UpdatedBy = GetUserId();

                    try
                    {
                        var item = _transportTicketNcrbll.SaveData(trsTicketNcr);
                        bulkData.New[i] = Mapper.Map<TransportTicketNCRViewModel>(item);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                        bulkData.New[i].Message = Enums.ResponseType.Success.ToString();

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
                    var trsTicketNcr = Mapper.Map<TransportTicketNCRDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                    //trsTicketNcr.CreatedBy = GetUserId();
                    trsTicketNcr.UpdatedBy = GetUserId();
                    try
                    {
                        var item = _transportTicketNcrbll.EditData(trsTicketNcr);
                        bulkData.Edit[i] = Mapper.Map<TransportTicketNCRViewModel>(item);
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

        public ActionResult GetManufacturingDate()
        {
            int[] getyear = new int[7];

            var year = DateTime.Now.Year - 4;
            for (var i = 0; i < getyear.Length; i++)
            {
                getyear[i] = year + i;
            }

            return Json(getyear, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InsertCirf(InsertUpdateData<TransportTicketNCRViewModel> bulkData)
        {
            for (var i = 0; i < bulkData.Edit.Count; i++)
            {
                if (bulkData.Edit[i] == null) continue;
                var trsTicketNcr = Mapper.Map<TransportTicketNCRDTO>(bulkData.Edit[i]);

                //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                trsTicketNcr.CreatedBy = GetUserId();
                trsTicketNcr.UpdatedBy = GetUserId();
                try
                {
                    var item = _transportTicketNcrbll.EditDataCirf(trsTicketNcr);
                    bulkData.Edit[i] = Mapper.Map<TransportTicketNCRViewModel>(item);
                    bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                }
                catch (ExceptionBase ex)
                {
                    bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                    bulkData.Edit[i].Message = ex.Message;
                }
            }

            return Json(bulkData);
        }

        public ActionResult setInactive(InsertUpdateData<TransportTicketNCRViewModel> bulkData)
        {
            for (var i = 0; i < bulkData.Edit.Count; i++)
            {
                if (bulkData.Edit[i] == null) continue;
                var trsTicketNcr = Mapper.Map<TransportTicketNCRDTO>(bulkData.Edit[i]);

                //set createdby and updatedby, nanti ditambah SSO pas udh jadi
                trsTicketNcr.CreatedBy = GetUserId();
                trsTicketNcr.UpdatedBy = GetUserId();
                try
                {
                    var item = _transportTicketNcrbll.SetInactives(trsTicketNcr);
                    bulkData.Edit[i] = Mapper.Map<TransportTicketNCRViewModel>(item);
                    bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                }
                catch (ExceptionBase ex)
                {
                    bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                    bulkData.Edit[i].Message = ex.Message;
                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Assets/Uploads/TransportTicketNCR/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }

        }

        public JsonResult ExportTicketNCR(TransportTicketNCRInput criteria)
        {
            //var filterMonth = Request.Params["filterData"];
            //var filterYear = Int32.Parse(Request.Params["filterYear"]);
            //var generateType = "advSearch"; //Request.Params["generateType"];
            var dbResult = _transportTicketNcrbll.GetTicketNCRs(criteria);

            DataTable boundTable = new DataTable();
            boundTable.Columns.Add("TicketStatus", typeof(bool));
            boundTable.Columns.Add("TicketTitle", typeof(string));
            boundTable.Columns.Add("IncidentDateTime", typeof(string));
            boundTable.Columns.Add("VendorName", typeof(string));
            boundTable.Columns.Add("PoliceRegNumber", typeof(string));
            boundTable.Columns.Add("Category", typeof(string));
            boundTable.Columns.Add("AccidentCategory", typeof(string));
            boundTable.Columns.Add("OffenderName", typeof(string));
            boundTable.Columns.Add("OffenderRole", typeof(string));
            boundTable.Columns.Add("OffenderAge", typeof(int));
            boundTable.Columns.Add("OffenderYearOfService", typeof(string));
            boundTable.Columns.Add("Location", typeof(string));
            boundTable.Columns.Add("SIRSNumber", typeof(int));

            foreach (TransportTicketNCRDTO item in dbResult)
            {
                dynamic dr = boundTable.NewRow();
                dr["TicketStatus"] = item.IsActive;
                dr["TicketTitle"] = item.TicketNumber;
                dr["IncidentDateTime"] = item.CorrectiveActionDate;
                dr["VendorName"] = item.VendorName;
                dr["PoliceRegNumber"] = item.PoliceRegNumber;
                dr["Category"] = item.TicketCategory;
                dr["AccidentCategory"] = item.AccidentCategory;
                dr["OffenderName"] = item.OffenderName;
                dr["OffenderRole"] = item.OffenderRole;
                dr["OffenderAge"] = (item.OffenderAge == null) ? 0 : item.OffenderAge;
                dr["OffenderYearOfService"] = item.OffenderYearOfService;
                dr["Location"] = item.Location;

                boundTable.Rows.Add(dr);
            }

            DataTableToExcel dataTableToExcel = new DataTableToExcel();
            string path = "TransportTicketNCR" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path);

            return Json(path, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportToExcel(TransportTicketNCRInput criteria)
        {
            var records = _transportTicketNcrbll.GetTicketNCRs(criteria);

            DataTable boundTable = new DataTable();

            string columns = Request.Params["Columns"];
            if (string.IsNullOrEmpty(columns))
            {
                columns = "Ticket Title,Category,Ticket Quantity,Location,Vendor Name,Police Registration Number,Offender Name,Offender Role,Offender Age,Remarks Ticket,Attachment,Vehicle Type,Manufacturing Year,Offender Year Of Service,Location Category,Type Of Location,Road Condition,Preventablility,Weather Condition,Accident Category,Value Of Loss,Corrective Action,Incident Date & Time,Ticket Status,SIRS Number,NCR Number,NRC Attachment,Problem Issue 1,Problem Issue 2,Problem Issue 3,Main Factor,Is Active,Created By,Created Date,Updated By,Updated Date,Remarks";
            }

            string[] integerFields = { "Ticket Quantity", "Offender Age", "Manufacturing Year" };
            string[] decimalFields = { "Value Of Loss" };

            string[] Fields = columns.Split(',');
            foreach (string Field in Fields)
            {
                if (integerFields.Contains(Field))
                {
                    boundTable.Columns.Add(Field, typeof(int));
                }
                else if (decimalFields.Contains(Field))
                {
                    boundTable.Columns.Add(Field, typeof(Decimal));
                }
                else
                {
                    boundTable.Columns.Add(Field, typeof(string));
                }
            }

            foreach (TransportTicketNCRDTO record in records)
            {
                var dict = new Dictionary<string, string>();

                dict["Ticket Title"] = record.TicketNumber;
                dict["Category"] = record.TicketCategory;
                dict["Ticket Quantity"] = String.IsNullOrEmpty(record.TicketQuantity.ToString()) ? "0" : record.TicketQuantity.ToString();
                dict["Location"] = record.Location;
                dict["Vendor Name"] = record.VendorName;
                dict["Police Registration Number"] = record.PoliceRegNumber;
                dict["Offender Name"] = record.OffenderName;
                dict["Offender Role"] = record.OffenderRole;
                dict["Offender Age"] = String.IsNullOrEmpty(record.OffenderAge.ToString()) ? "0" : record.OffenderAge.ToString();
                dict["Remarks Ticket"] = record.RemarksTicket;
                dict["Attachment"] = record.Attachment;
                dict["Vehicle Type"] = record.VehicleType;
                dict["Manufacturing Year"] = String.IsNullOrEmpty(record.ManufacturingYear.ToString()) ? "0" : record.ManufacturingYear.ToString();
                dict["Offender Year Of Service"] = record.OffenderYearOfService;
                dict["Location Category"] = record.LocationCategory;
                dict["Type Of Location"] = record.TypeOfLocation;
                dict["Road Condition"] = record.RoadCondition;
                dict["Preventablility"] = (record.Preventablility == true) ? "True" : "False";
                dict["Weather Condition"] = record.WeatherCondition;
                dict["Accident Category"] = record.AccidentCategory;
                dict["Value Of Loss"] = String.IsNullOrEmpty(record.ValueOfLoss.ToString()) ? "0" : record.ValueOfLoss.ToString();
                dict["Corrective Action"] = record.CorrectiveAction;
                dict["Incident Date & Time"] = record.CorrectiveActionDate.ToString();
                dict["Ticket Status"] = record.TicketStatus;
                dict["SIRS Number"] = record.SIRSNumber;
                dict["NCR Number"] = record.NCRNumber;
                dict["NRC Attachment"] = record.AttachmentNRC;
                dict["Problem Issue 1"] = record.ProblemIssue1;
                dict["Problem Issue 2"] = record.ProblemIssue2;
                dict["Problem Issue 3"] = record.ProblemIssue3;
                dict["Main Factor"] = record.MainFactor;
                dict["Is Active"] = record.IsActive.ToString();
                dict["Created By"] = record.CreatedBy;
                dict["Created Date"] = record.CreatedDate.ToString("MM/dd/yyyy HH:mm:ss.fff");
                dict["Updated By"] = record.UpdatedBy;
                dict["Updated Date"] = record.UpdatedDate.ToString("MM/dd/yyyy HH:mm:ss.fff");
                dict["Remarks"] = record.Remarks;

                dynamic dr = boundTable.NewRow();
                foreach (string Field in Fields)
                {
                    if (integerFields.Contains(Field))
                    {
                        dr[Field] = Convert.ToInt16(dict[Field]);
                    }
                    else if (decimalFields.Contains(Field))
                    {
                        dr[Field] = Convert.ToDecimal(dict[Field]);
                    }
                    else
                    {
                        dr[Field] = dict[Field];
                    }
                }
                boundTable.Rows.Add(dr);
            }

            DataTableToExcel dataTableToExcel = new DataTableToExcel();
            string path = "TransportTicketNCR" + (DateTime.Now.ToString("yyyyMMddhhmmss")) + ".xlsx";
            dataTableToExcel.generateExcels(boundTable, AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path);

            return Json(path, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataByID(string id)
        {
            var generateTicketNCR = _transportTicketNcrbll.GetDataByIDs(id);
            return Json(generateTicketNCR, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeneratePDF(string id)
        {
            var dataresult = _transportTicketNcrbll.GetTicketNCRPDFs(id);
            var dr = dataresult;
            var fonttype = BaseFont.HELVETICA_BOLD;
            //Font fontH1 = new Font(, 16, Font.NORMAL);
            string imagepath = Server.MapPath("../assets/images");
            Document doc = new Document(PageSize.A4, 88f, 88f, 10f, 10f);
            Font NormalFont = FontFactory.GetFont(fonttype, 12, Font.NORMAL, BaseColor.BLACK);

            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {

                //PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                PdfWriter.GetInstance(doc, new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + id + ".pdf", FileMode.Create));
                //Phrase phrase = null;
                PdfPCell cell = null;
                PdfPTable table = null;
                //BaseColor color = null;

                //doc.Open();

                //PdfWriter.GetInstance(doc, new FileStream(pdfpath + "/Images.pdf", FileMode.Create));
                doc.Open();
                //var fonttype = "Segoe UI";

                BarcodePDF417 pdf417 = new BarcodePDF417();
                String text = dataresult.Select(m => m.TicketNumber).FirstOrDefault().ToString();
                pdf417.SetText(text);
                Image img = pdf417.GetImage();
                img.Alignment = Element.ALIGN_RIGHT;
                img.ScalePercent(80f);
                doc.Add(img);

                Image gif = Image.GetInstance(imagepath + "/sampoerna-logo.png");
                gif.Alignment = Image.ALIGN_CENTER;
                gif.ScalePercent(30f);
                doc.Add(gif);
                Paragraph tiket = new Paragraph("TICKET", FontFactory.GetFont(fonttype, 16, Font.BOLD, BaseColor.BLACK));
                tiket.Alignment = Element.ALIGN_CENTER;
                doc.Add(tiket);

                Paragraph ticketnumber = new Paragraph("Number : " + dataresult.Select(m => m.TicketNumber).FirstOrDefault().ToString(), FontFactory.GetFont(fonttype, 14, BaseColor.BLACK));
                ticketnumber.Alignment = Element.ALIGN_CENTER;
                doc.Add(ticketnumber);

                Paragraph tikcetquantity = new Paragraph(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK));
                // tikcetquantity.Alignment = Element.ALIGN_CENTER;
                doc.Add(tikcetquantity);

                Paragraph tikcetcategory = new Paragraph(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK));
                // tikcetquantity.Alignment = Element.ALIGN_CENTER;
                doc.Add(tikcetcategory);

                table = new PdfPTable(2);
                //table.WidthPercentage = 98;
                //table.TotalWidth = 500f;
                table.WidthPercentage = 100;
                float[] widths = new float[] { 170f, 330f };
                table.SetWidths(widths);

                cell = new PdfPCell(new Phrase(" "));
                cell.BorderColor = BaseColor.WHITE;
                cell.Colspan = 3;
                cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Ticket Quantity", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(":  " + dataresult.Select(m => m.TicketQuantity).FirstOrDefault().ToString(), FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Ticket Category", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(":  " + dataresult.Select(m => m.TicketCategory).FirstOrDefault().ToString(), FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Incident Date", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                DateTime test = Convert.ToDateTime(dataresult.Select(m => m.CorrectiveActionDate).FirstOrDefault());
                var tanggalkejadian = test.ToString("dd MMMM yyyy");
                cell = new PdfPCell(new Phrase(":  " + tanggalkejadian, FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Incident Time", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                var jamkejadian = test.Hour + ":" + test.Minute;
                cell = new PdfPCell(new Phrase(":  " + jamkejadian, FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Location", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(":  " + dataresult.Select(m => m.Location).FirstOrDefault().ToString(), FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Vendor Name", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(":  " + dataresult.Select(m => m.VendorName).FirstOrDefault().ToString(), FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Police Reg.Number", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(":  " + dataresult.Select(m => m.PoliceRegNumber).FirstOrDefault().ToString(), FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Offender Name", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(":  " + dataresult.Select(m => m.OffenderName).FirstOrDefault().ToString(), FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Offender Age", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(":  " + dataresult.Select(m => m.OffenderAge).FirstOrDefault().ToString() + " Year", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                //table.SetWidths(new int[] { 200, 50 });
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Offender Role", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(":  " + dataresult.Select(m => m.OffenderRole).FirstOrDefault().ToString(), FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Remark", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(":  ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell.BorderColor = BaseColor.WHITE;
                cell.FixedHeight = 30f;
                table.AddCell(cell);

                doc.Add(table);

                //Remark

                PdfPCell cell1 = null;
                PdfPTable table1 = null;

                table1 = new PdfPTable(1);
                //table.WidthPercentage = 98;
                table1.TotalWidth = 500f;
                //float[] widths = new float[] { 170f, 330f };
                //table.SetWidths(widths);
                table1.WidthPercentage = 100;

                cell1 = new PdfPCell(new Phrase(dataresult.Select(m => m.RemarksTicket).FirstOrDefault(), FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));

                cell1.Colspan = 3;
                cell1.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                cell1.FixedHeight = 200f;
                table1.AddCell(cell1);

                doc.Add(table1);

                //PdfPCell cell2 = null;
                //PdfPTable table2 = null;
                //table2 = new PdfPTable(1);
                ////table.WidthPercentage = 98;
                //table2.TotalWidth = 500f;

                //cell2 = new PdfPCell(new Phrase("Sign By,", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                //cell2.BorderColor = BaseColor.WHITE;

                //cell2.Colspan = 3;
                //cell2.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right            
                //table2.AddCell(cell2);

                //doc.Add(table2);

                Paragraph ticketnumber1 = new Paragraph("Sign by, ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK));
                ticketnumber1.Alignment = Element.ALIGN_CENTER;
                doc.Add(ticketnumber1);
                Paragraph ticketnumber2 = new Paragraph(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK));
                ticketnumber2.Alignment = Element.ALIGN_CENTER;
                doc.Add(ticketnumber2);

                PdfPCell cell3 = null;
                PdfPTable table3 = null;
                table3 = new PdfPTable(2);
                table3.WidthPercentage = 100;
                //table.WidthPercentage = 98;
                table3.TotalWidth = 500f;
                float[] widths1 = new float[] { 250f, 250f };
                table3.SetWidths(widths1);
                cell3 = new PdfPCell(new Phrase("PIC Sampoerna", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell3.BorderColor = BaseColor.WHITE;
                cell3.HorizontalAlignment = 1;
                cell3.FixedHeight = 70f;
                table3.AddCell(cell3);

                cell3 = new PdfPCell(new Phrase("Vendor", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell3.BorderColor = BaseColor.WHITE;
                cell3.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                cell3.FixedHeight = 70f;
                table3.AddCell(cell3);

                //cell3 = new PdfPCell(new Phrase(" ", FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                //cell3.BorderColor = BaseColor.WHITE;
                //cell3.Colspan = 3;
                //cell3.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                //cell3.FixedHeight = 70f;
                //table.AddCell(cell3);

                cell3 = new PdfPCell(new Phrase(dataresult.Select(m => m.FullName).FirstOrDefault().ToString(), FontFactory.GetFont(fonttype, 12, BaseColor.BLACK)));
                cell3.BorderColor = BaseColor.WHITE;
                cell3.HorizontalAlignment = 1;
                table3.AddCell(cell3);

                cell3 = new PdfPCell(new Phrase("_____________"));
                cell3.BorderColor = BaseColor.WHITE;
                cell3.HorizontalAlignment = 1;
                table3.AddCell(cell3);

                doc.Add(table3);

                doc.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                //Response.Clear();
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("Content-Disposition", "attachment; filename="+ id +".pdf");
                //Response.ContentType = "application/pdf";
                //Response.Buffer = true;
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.BinaryWrite(bytes);
                //Response.End();
                //Response.Close();

                return Json(id + ".pdf", JsonRequestBehavior.AllowGet);
            }


        }
        private static void DrawLine(PdfWriter writer, float x1, float y1, float x2, float y2, iTextSharp.text.BaseColor color)
        {
            PdfContentByte contentByte = writer.DirectContent;
            contentByte.SetColorStroke(color);
            contentByte.MoveTo(x1, y1);
            contentByte.LineTo(x2, y2);
            contentByte.Stroke();
        }
        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }

        private static PdfPCell ImageCell(string path1, float scale, int align)
        {
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + '/' + path1);
            image.ScalePercent(scale);
            PdfPCell cell = new PdfPCell(image);

            cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
            cell.VerticalAlignment = PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 0f;
            cell.PaddingTop = 0f;
            return cell;
        }
    }

    public class GetCalculateSummary
    {
        public string TotalTicket { get; set; }
        public string TotalNCR { get; set; }
        public string TotalCrash { get; set; }
        public string TotalIncident { get; set; }

    }
}