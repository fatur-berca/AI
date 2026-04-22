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
using DFIS.Universal.Domain.Outputs;
using TOM.EntitiesDAL.EDMX;
using System.Data;
using OfficeOpenXml;
using System.IO;
using System.Text;
using System.Diagnostics;
using TOM.EntitiesDAL;
using TOM.Master.BusinessLogics;
using TOM.Master.Repositories;
using TOM.Master.Domain.Inputs;
using DFIS.Universal;
using TOM.Master.Domain.DTOs;
using hms_tom_dev.Helper;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using DFIS.Contracts;

namespace hms_tom_dev.Controllers
{
    public class MstLeadTimeTomController : BaseController
    {
        private readonly IMasterLeadTimeTomBLL _masterLeadTimeTomBLL;
        private readonly IMasterDistanceBLL _masterDistanceBLL;
        private readonly IMasterVendorTOMRepo _masterVendorTOMRepo;
        private readonly IMasterLocationBLL _masterLocationBLL;
        //private readonly IMasterVendorTOMBLL _masterVendorTOMBLL;
        //
        // GET: /MstDistance/
        public MstLeadTimeTomController(IMasterLeadTimeTomBLL masterLeadTimeTomBLL, IMasterDistanceBLL masterDistanceBLL, IMasterVendorTOMRepo masterVendorTOMRepo, IMasterLocationBLL masterLocationBLL)
        {
            _masterLeadTimeTomBLL = masterLeadTimeTomBLL;
            _masterDistanceBLL = masterDistanceBLL;
            _masterVendorTOMRepo = masterVendorTOMRepo;
            _masterLocationBLL = masterLocationBLL;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMasterLeadTimeTom(MasterLeadTimeTomInput criteria)
        {
            var viewModel = Mapper.Map<List<MasterLeadTimeTomViewModel>>(_masterLeadTimeTomBLL.GetAllMasterLeadTimeTOM(criteria));
            var locs = _masterLocationBLL.GetMasterLocations(new MasterLocationInput());
            foreach (var vm in viewModel)
            {
                vm.MasterLocation = vm.SenderIDLocation == null ? null : Mapper.Map<MasterLocation>(locs.Find(c => c.IDLocation == vm.SenderIDLocation));
                vm.MasterLocation1 = vm.ReceiverIDLocation == null ? null : Mapper.Map<MasterLocation>(locs.Find(c => c.IDLocation == vm.ReceiverIDLocation));
                vm.MasterLocation2 = vm.ThroughIDLocation == null ? null : Mapper.Map<MasterLocation>(locs.Find(c => c.IDLocation == vm.ThroughIDLocation));
                _masterVendorTOMRepo.AllowLazyLoading = false;
                var vend = vm.IDVendor == null ? null : _masterVendorTOMRepo.Get(c => c.IDVendor == vm.IDVendor).FirstOrDefault();
                vm.MasterTransportVendor = Mapper.Map<MasterVendor>(Mapper.Map<MasterVendorTOMDTO>(vend));
                if (vm.MasterTransportVendor != null)
                {
                    vm.MasterTransportVendor.MasterVendor1 = null;
                    vm.MasterTransportVendor.MasterVendor2 = null;
                    vm.MasterTransportVendor.MasterVendorSuggestions = null;
                }
            }
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataById(MasterLeadTimeTomInput criteria)
        {
            var masterLeadTimes = _masterLeadTimeTomBLL.GetById(criteria.IDLeadTime);
            var viewModel = Mapper.Map<MasterLeadTimeTomViewModel>(masterLeadTimes);
            var vm = viewModel;

            var locs = _masterLocationBLL.GetMasterLocations(new MasterLocationInput());
            vm.MasterLocation = vm.SenderIDLocation == null ? null : Mapper.Map<MasterLocation>(locs.Find(c => c.IDLocation == vm.SenderIDLocation));
            vm.MasterLocation1 = vm.ReceiverIDLocation == null ? null : Mapper.Map<MasterLocation>(locs.Find(c => c.IDLocation == vm.ReceiverIDLocation));
            vm.MasterLocation2 = vm.ThroughIDLocation == null ? null : Mapper.Map<MasterLocation>(locs.Find(c => c.IDLocation == vm.ThroughIDLocation));
            vm.MasterTransportVendor = vm.IDVendor == null ? null : Mapper.Map<MasterVendor>(Mapper.Map<MasterVendorTOMViewModel>(_masterVendorTOMRepo.GetByID(vm.IDVendor)));
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserWarehouse()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var ListLocation = session.Location.Select(x => x.IDLocation).ToList();
            //var getdata = _masterLocationBLL.GetByConfig("TOMLocationFilter", loc => ListLocation.Contains(loc.IDLocation));
            var getdata = _masterLocationBLL.GetByConfig("TOMLocationFilter");
            return Json(getdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropDownVendor()
        {
            var getdata = _masterLeadTimeTomBLL.GetDropDownVendor().FindAll(v => v.ParentVendor == null);
            return Json(getdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterVendorTom(MasterVendorTOMInput criteria)
        {
            var masterVendors = _masterLeadTimeTomBLL.GetMasterVendorTom(criteria).FindAll(m => m.ParentVendor == null);
            return Json(masterVendors, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropDownTm()
        {
            return Json(_masterLeadTimeTomBLL.GetTmList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropDownVia()
        {
            return Json(_masterLeadTimeTomBLL.GetViaList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropDownType()
        {
            return Json(_masterLeadTimeTomBLL.GetTypeList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int keyId)
        {
            return Json(_masterLeadTimeTomBLL.Delete(keyId));
        }

        [HttpPost]
        public ActionResult InsertLeadTimeTom(InsertUpdateData<MasterLeadTimeTomViewModel> bulkData)
        {
            try
            {
                /*
                if (bulkData.New != null)
                {
                    Debug.WriteLine("Insert");
                    var mstLeadTime = Mapper.Map<MasterLeadTimeTomDTO>(bulkData.New[0]);
                    mstLeadTime.CreatedBy = GetUserId();
                    mstLeadTime.UpdatedBy = GetUserId();
                    _masterLeadTimeTomBLL.SaveData(mstLeadTime);
                }
                else
                {
                    Debug.WriteLine("Update");
                    var mstLeadTime = Mapper.Map<MasterLeadTimeTomDTO>(bulkData.Edit[0]);
                    mstLeadTime.CreatedBy = GetUserId();
                    mstLeadTime.UpdatedBy = GetUserId();
                    _masterLeadTimeTomBLL.EditData(mstLeadTime);
                }
                */
                var data = bulkData.New != null ? bulkData.New[0] : bulkData.Edit[0];
                var val = MappingHelper.Map<MasterLeadTimeTomDTO>(data);
                val.UpdatedBy = GetUserId();
                if (bulkData.New != null)
                {
                    val.CreatedBy = GetUserId();
                    val.CreatedDate = DateTime.Now;
                }
                _masterLeadTimeTomBLL.InsertOrUpdate(val);
                return Json(new NonQueryResult(true));
            }
            catch (Exception ex)
            {
                return Json(new NonQueryResult(false, ex.Message));
            }
            //Debug.WriteLine("Return:" + result);            
        }

        public ActionResult UploadLeadTime()
        {
            // Input checking
            var uid = GetUserId();
            if (Request.Files.Count <= 0) return Json(new NonQueryResult(false, "No file specified."));

            var context = new TOMContextDB();
            /*
            var role = context.MasterUserRoleMappings
                .Where(n => n.IDUser == uid)
                .Select(n => n.IDRole).ToList();
            if (!role.Contains(1) && !role.Contains(5) && !role.Contains(7))
                return Json(new NonQueryResult(false, "You are not authorized to do this action."));
            */

            // Mapping Setup
            var eih = new ExcelImportHelper();
            eih.Map("Data Type", "VendorCategory");
            eih.Map("Sender", "SenderIDLocation");
            eih.Map("Receiver", "ReceiverIDLocation");
            eih.Map("Vendor", "VendorName");
            eih.Map("Lead Time", "Time");
            eih.Map("Effective Start Date", "EffectiveStartDate");

            /*
            eih.Map("Vendor Name", "Remarks");
            eih.Map("Sender", "SenderIDLocation");
            eih.Map("Receiver", "ReceiverIDLocation");
            eih.Map("Via", "Via");
            eih.Map("Time", "Time");
            eih.Map("Effective Start Date", "EffectiveStartDate");
            */

            // get all valid data
            var dataCol = new Dictionary<string, List<ExcelImportResult<MasterLeadTimeTomDTO>>>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files.Get(i);
                // get workbook for each file
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        var ds = ExcelImportHelper.ImportExcelAsDataSet(file.InputStream, file.FileName.ToLower().EndsWith(".xlsx"));
                        // get worksheet in every workbook
                        foreach (DataTable table in ds.Tables)
                        {
                            var col = new List<ExcelImportResult<MasterLeadTimeTomDTO>>();
                            dataCol.Add(file.FileName + "#" + table.TableName, col);

                            var excelRes = eih.Deserialize<MasterLeadTimeTomDTO>(table, c =>
                            {
                                // Set Created dan Updated Information
                                c.CreatedBy = GetUserId();
                                c.CreatedDate = DateTime.Now;
                                c.UpdatedBy = GetUserId();
                                c.UpdatedDate = DateTime.Now;

                                // Set Active Default Value
                                c.IsActive = true;
                                c.Remarks = null;

                                // Validate Required/Mandatory Column
                                if (c.VendorCategory == null) throw new RowValidationByFieldException("Vendor Category");
                                if (c.SenderIDLocation == null) throw new RowValidationByFieldException("Sender");
                                if (c.ReceiverIDLocation == null) throw new RowValidationByFieldException("Receiver");
                                if (c.VendorName == null) throw new RowValidationByFieldException("Vendor Name");
                                if (c.Time == null) throw new RowValidationByFieldException("Lead Time");
                                if (c.EffectiveStartDate == null) throw new RowValidationByFieldException("Effective Start Date");

                                // Get and Set Sender and Received Location From Excel File
                                var sender = _masterLocationBLL.GetMasterLocations(new MasterLocationInput() { LocationName = c.SenderIDLocation, IsAssigned = true }).FirstOrDefault();
                                var receiver = _masterLocationBLL.GetMasterLocations(new MasterLocationInput() { LocationName = c.ReceiverIDLocation, IsAssigned = true }).FirstOrDefault();

                                if (sender == null) throw new RowValidationByFieldException("Sender", c.SenderIDLocation, "not found in");
                                if (receiver == null) throw new RowValidationByFieldException("Receiver", c.ReceiverIDLocation, "not found in");

                                c.SenderIDLocation = sender != null ? sender.IDLocation : null;
                                c.ReceiverIDLocation = receiver != null ? receiver.IDLocation : null;

                                // Get and Set Vendor From Excel File
                                var vendor = _masterLeadTimeTomBLL.GetMasterVendorTom(new MasterVendorTOMInput() {
                                    VendorName = c.VendorName,
                                    VendorCategory = c.VendorCategory
                                }).FirstOrDefault();

                                if (vendor == null) throw new RowValidationByFieldException("Vendor Name", c.VendorName, "not found in");
                                if (vendor.ParentVendor != null) throw new Exception("[Vendor] must be a parent vendor");

                                c.IDVendor = vendor.IDVendor;

                                // Get and Set StartDate, EndDate Variable From Excel File
                                c.EffectiveEndDate = new DateTime(2999, 12, 31);

                                DateRangeHelper daterangevalidate = new DateRangeHelper();

                                var criteria = new MasterLeadTimeTomInput
                                {
                                    IDVendor = c.IDVendor,
                                    SenderIDLocation = c.SenderIDLocation,
                                    ReceiverIDLocation = c.ReceiverIDLocation,
                                };
                                var existrecord = _masterLeadTimeTomBLL.GetAllMasterLeadTimeTOM(criteria);

                                foreach (var record in existrecord)
                                    daterangevalidate.Add(new DateRange(record.EffectiveStartDate, record.EffectiveEndDate, record));

                                if(existrecord.Count() >= 1)
                                {
                                    var itx = daterangevalidate.IntersectWith(new DateRange(c.EffectiveStartDate, c.EffectiveEndDate));
                                    if(itx.Count == 1)
                                    {
                                        var itr = itx.ElementAt(0);
                                        if (itr.SourceRange.DateEnd.ToString("yyyy-MM-dd") != "2999-12-31")
                                            throw new EffectiveDateConflictException();
                                        // but only when the new data starts after the start date of the intersected data
                                        if (itr.SourceRange.DateStart >= c.EffectiveStartDate)
                                            throw new EffectiveDateConflictException();
                                        // update the old data
                                        if (itr.SourceRange.Tag is MasterLeadTime)
                                        {
                                            var oldValue = itr.SourceRange.Tag as MasterLeadTime;
                                            oldValue.EffectiveEndDate = c.EffectiveStartDate - new TimeSpan(1, 0, 0, 0);

                                            var oldvaluecriteria = new MasterLeadTimeTomDTO
                                            {
                                                IDLeadTime = oldValue.IDLeadTime,
                                                IDVendor = oldValue.IDVendor,
                                                SenderIDLocation = oldValue.SenderIDLocation,
                                                ReceiverIDLocation = oldValue.ReceiverIDLocation,
                                                EffectiveStartDate = oldValue.EffectiveStartDate,
                                                EffectiveEndDate = oldValue.EffectiveEndDate
                                            };
                                            _masterLeadTimeTomBLL.EditData(oldvaluecriteria);
                                        }
                                    }
                                    if(itx.Count > 1)
                                        throw new EffectiveDateConflictException();
                                }

                                // calculate total distance
                                // c.IsActive = true;
                                // c.EffectiveEndDate = new DateTime(2999, 12, 31);

                                // check required fields
                                // if (c.SenderIDLocation == null) throw new RowValidationByFieldException("Sender");
                                // if (c.ReceiverIDLocation == null) throw new RowValidationByFieldException("Receiver");
                                // if (c.IDVendor == null) throw new RowValidationByFieldException("Vendor");

                                // validate vendor
                                // var vendor = _masterLeadTimeTomBLL.GetMasterVendorTom(new MasterVendorTOMInput() { VendorName = c.Remarks }).FirstOrDefault();
                                // if (vendor == null)
                                // throw new RowValidationByFieldException("Vendor Name", c.Remarks);
                                // if (vendor.ParentVendor != null)
                                // throw new Exception("[Vendor] must be a parent vendor");
                                // c.IDVendor = vendor.IDVendor;
                                // c.Remarks = null;

                                // check whether location exists
                                // var locSender = _masterLocationBLL.GetMasterLocations(new MasterLocationInput() { LocationName = c.SenderIDLocation, IsAssigned = true }).FirstOrDefault();
                                // var locRcv = _masterLocationBLL.GetMasterLocations(new MasterLocationInput() { LocationName = c.ReceiverIDLocation, IsAssigned = true }).FirstOrDefault();

                                // if (locSender == null) throw new RowValidationByFieldException("Sender", c.SenderIDLocation, "not found in");
                                // if (locRcv == null) throw new RowValidationByFieldException("Receiver", c.ReceiverIDLocation, "not found in");

                                // c.SenderIDLocation = locSender != null ? locSender.IDLocation : null;
                                // c.ReceiverIDLocation = locRcv != null ? locRcv.IDLocation : null;

                                return c;
                            });

                            col.AddRange(excelRes);
                        }
                    }
                    catch { }
                }
            }

            // push the data to database
            var res = eih.Import<MasterLeadTimeTomDTO>(dataCol, _masterLeadTimeTomBLL);

            // import completed successfully
            return Json(new NonQueryResult(res == null, res));
        }
    }
}