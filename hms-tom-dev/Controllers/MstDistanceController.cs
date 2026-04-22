using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using TOM.Master.BusinessLogics;
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
using TOM.Master.Domain.Inputs;
using DFIS.Universal;
using hms_tom_dev.Helper;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using DFIS.Contracts;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using System.Drawing;
using Microsoft.Office.Interop.Excel;

namespace hms_tom_dev.Controllers
{
    public class MstDistanceController : BaseController
    {
        private readonly IMasterDistanceBLL _masterDistanceBLL;
        private readonly IMasterLocationBLL _masterLocationBLL;
        private readonly IMasterListBLL _masterListBLL;
        //
        // GET: /MstDistance/
        public MstDistanceController(IMasterDistanceBLL masterDistanceBLL, IMasterLocationBLL masterLocationBLL, IMasterListBLL masterListBLL)
        {
            _masterDistanceBLL = masterDistanceBLL;
            _masterLocationBLL = masterLocationBLL;
            _masterListBLL = masterListBLL;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMasterDistance(MasterDistanceInput criteria)
        {
            return Json(_masterDistanceBLL.GetAllMasterDistanceLocation(criteria), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataById(MasterDistanceInput criteria)
        {
            return Json(_masterDistanceBLL.GetById(criteria.IDDistance), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserWarehouse()
        {
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var ListLocation = session.Location.Select(x => x.IDLocation).ToList();
            //var getdata = _masterLocationBLL.GetByConfig("TOMLocationFilter", loc => ListLocation.Contains(loc.IDLocation));
            var getdata = _masterLocationBLL.GetByConfig("TOMLocationFilter");
            return Json(getdata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropDownTm()
        {
            return Json(_masterDistanceBLL.GetTmList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropDownVia()
        {
            return Json(_masterDistanceBLL.GetViaList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDropDownType()
        {
            return Json(_masterDistanceBLL.GetTypeList(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult InsertDistance(InsertUpdateData<MasterDistanceViewModel> bulkData)
        {
            try
            {
                if (bulkData.New != null)
                {
                    var mstDistance = MappingHelper.Map<MasterDistanceDTO>(bulkData.New[0]);
                    mstDistance.Distance /= 100;
                    mstDistance.Total = Convert.ToDecimal(mstDistance.Distance + mstDistance.Distance * bulkData.New[0].Buffer / 100);
                    mstDistance.CreatedBy = GetUserId();
                    mstDistance.CreatedDate = DateTime.Today;
                    mstDistance.UpdatedBy = GetUserId();
                    _masterDistanceBLL.InsertOrUpdate(mstDistance);
                }
                else
                {
                    var mstDistance = MappingHelper.Map<MasterDistanceDTO>(bulkData.Edit[0]);
                    mstDistance.Distance /= 100;
                    mstDistance.Total = Convert.ToDecimal(mstDistance.Distance + mstDistance.Distance * bulkData.Edit[0].Buffer / 100);
                    mstDistance.UpdatedBy = GetUserId();
                    _masterDistanceBLL.InsertOrUpdate(mstDistance);
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

        public ActionResult DebugGenData(int count)
        {
            var locs = _masterLocationBLL.Get(n => n.IsActive == true);
            var listDistanceType = _masterListBLL.GetMasterLists(new MasterListInput() { FieldName = "TypeDistance" }).Select(c => c.FieldValue).ToList();
            var rand = new Random();

            for (int i = 0; i < count; i++)
            {
                var data = new MasterDistanceDTO()
                {
                    CreatedBy = GetUserId(),
                    CreatedDate = DateTime.Now,
                    UpdatedBy = GetUserId(),
                    UpdatedDate = DateTime.Now,
                    Distance = rand.Next(0, 300),
                    Buffer = (int)Math.Round(rand.NextDouble() * 100),
                    EffectiveStartDate = DateTime.Today + new TimeSpan(rand.Next(100), 0, 0, 0, 0),
                    EffectiveEndDate = DateTime.MaxValue,
                    IDReceiver = locs[rand.Next(locs.Count)].IDLocation,
                    IDSender = locs[rand.Next(locs.Count)].IDLocation,
                    IsActive = true,
                    DistanceType = listDistanceType[rand.Next(listDistanceType.Count())],
                    Through = locs[rand.Next(locs.Count)].IDLocation,
                    TransportationMode = "-",
                    Total = 999
                };
                try
                {
                    _masterDistanceBLL.InsertOrUpdate(data);
                }
                catch { }
            }
            return Json(true);
        }

        public ActionResult UploadDistance()
        {
            // Warehouse only!!
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var ListLocation = session.Location.Select(x => x.IDLocation).ToList();
            var locdata = _masterLocationBLL.GetByConfig("TOMLocationFilter"); //, loc => ListLocation.Contains(loc.IDLocation));

            // TypeDistance Only!!
            var listDistanceType = _masterListBLL.GetMasterLists(new MasterListInput() { FieldName = "TypeDistance" }).Select(c => c.FieldValue);
            var listVia = _masterListBLL.GetMasterLists(new MasterListInput() { FieldName = "ViaRoute" }).Select(c => c.FieldValue);

            // Input checking
            var uid = GetUserId();
            if (Request.Files.Count <= 0) return Json(new NonQueryResult(false, "No file specified."));
            var context = new TOMContextDB();
            var role = context.MasterUserRoleMappings
                .Where(n => n.IDUser == uid)
                .Select(n => n.IDRole).ToList();
            if (!role.Contains(1) && !role.Contains(5) && !role.Contains(7))
                return Json(new NonQueryResult(false, "You are not authorized to do this action."));

            // Mapping Setup
            var eih = new ExcelImportHelper();
            eih.Map("Distance Type", "DistanceType");
            eih.Map("Sender", "IDSender");
            eih.Map("Receiver", "IDReceiver");
            eih.Map("Distance", "Distance");
            eih.Map("Buffer", "Buffer");
            eih.Map("Via", "Via");
            eih.Map("Through", "Through");
            eih.Map("Effective Start Date", "EffectiveStartDate");

            // get all valid data
            var dataCol = new Dictionary<string, List<ExcelImportResult<MasterDistanceDTO>>>();
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
                        foreach (System.Data.DataTable table in ds.Tables)
                        {
                            var col = new List<ExcelImportResult<MasterDistanceDTO>>();
                            dataCol.Add(file.FileName + "#" + table.TableName, col);

                            var excelRes = eih.Deserialize<MasterDistanceDTO>(table, c =>
                            {
                                // Set create and update
                                c.CreatedBy = GetUserId();
                                c.CreatedDate = DateTime.Now;
                                c.UpdatedBy = GetUserId();
                                c.UpdatedDate = DateTime.Now;

                                // calculate total distance
                                c.IsActive = true;
                                c.EffectiveEndDate = new DateTime(2999, 12, 31);

                                // check required fields
                                if (c.IDSender == null) throw new RowValidationByFieldException("Sender");
                                if (c.IDReceiver == null) throw new RowValidationByFieldException("Receiver");

                                // check whether location exists
                                var locSender = locdata.Where(loc => loc.LocationName == c.IDSender).FirstOrDefault();
                                var locRcv = locdata.Where(loc => loc.LocationName == c.IDReceiver).FirstOrDefault();

                                if (locSender == null) throw new RowValidationByFieldException("Sender", c.IDSender, "not found in");
                                if (locRcv == null) throw new RowValidationByFieldException("Receiver", c.IDReceiver, "not found in");

                                c.IDSender = locSender != null ? locSender.IDLocation : null;
                                c.IDReceiver = locRcv != null ? locRcv.IDLocation : null;

                                if (!listDistanceType.Contains(c.DistanceType))
                                    throw new RowValidationByFieldException("Distance Type", c.DistanceType, "not found in");
                                if (c.Distance <= 0)
                                    throw new RowValidationByFieldException("Distance", c.Distance.ToString());
                                if (c.Buffer < 0)
                                    throw new RowValidationByFieldException("Buffer", c.Buffer.ToString());
                                if (c.Buffer < 1)
                                    c.Buffer *= 100;

                                if (c.DistanceType == "Trip Based")
                                {
                                    if (c.Through == null)
                                    {
                                        // throw new RowValidationByFieldException("Through");
                                    }
                                    else
                                    {
                                        var locTrg = locdata.Where(loc => loc.LocationName == c.Through).FirstOrDefault();

                                        if (locTrg == null) throw new RowValidationByFieldException("Through", c.Through, "not found in");
                                        c.Through = locTrg != null ? locTrg.IDLocation : null;
                                    }

                                    if (c.Via == null)
                                    {
                                        // throw new RowValidationByFieldException("Via");
                                    }
                                    else if (!listVia.Contains(c.Via))
                                        throw new RowValidationByFieldException("Via", c.Via, "not found in");
                                }
                                else
                                {
                                    c.Via = null;
                                    c.Through = null;
                                }

                                c.Total = c.Distance + (c.Distance * c.Buffer / 100);
                                c.TransportationMode = "-";
                                return c;
                            });

                            col.AddRange(excelRes);
                        }
                    }
                    catch { }
                }
            }

            // push the data to database
            var res = eih.Import(dataCol, _masterDistanceBLL);

            // import completed successfully
            return Json(new NonQueryResult(res == null, res));
        }

        public ActionResult RecalculateTotal()
        {
            var val = _masterDistanceBLL.RecalculateTotal();
            return Json(val, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int keyID)
        {
            try
            {
                _masterDistanceBLL.DelData(keyID);
                return Json(new NonQueryResult(true));
            }
            catch (Exception ex)
            {
                return Json(new NonQueryResult(false, ex.Message));
            }
        }

        #region Nicco
        public ActionResult ExportTO(TransportOrderRequestInput filter)
        {
            var defMap = new Dictionary<string, string>();
            defMap.Add("IDDistance", "ID Distance");
            defMap.Add("DistanceType", "Data Type");
            defMap.Add("IDSender", "Sender");
            defMap.Add("SenderLocationName", "Sender Location");
            defMap.Add("IDReceiver", "Receiver");
            defMap.Add("ReceiverLocationName", "Receiver Location");
            defMap.Add("TransportationMode", "Transportation Mode");
            defMap.Add("Distance", "Distance");
            defMap.Add("Buffer", "Buffer");
            defMap.Add("Total", "Total");
            defMap.Add("Through", "Through");
            defMap.Add("Via", "Via");
            defMap.Add("EffectiveStartDate", "Start Date");
            defMap.Add("EffectiveEndDate", "End Date");
            defMap.Add("IsActive", "Is Active");
            defMap.Add("CreatedBy", "Created By");
            defMap.Add("CreatedDate", "Created Date");
            defMap.Add("UpdatedBy", "Updated By");
            defMap.Add("UpdatedDate", "Last Update");
            defMap.Add("Remarks", "Remarks");


            var cols = defMap.Keys.ToArray();

            if (GetListUserRole().Any(x =>
            x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.ATR) ||
            x.RoleName == EnumHelper.GetDescription(Enums.RoleUserList.SA)))
                filter.IsRoleTransport = true;
            filter.userRegionList = GetUserRegionSelectList().Select(x => x.Value).ToList();
            filter.CreatedBy = GetUserId();
            MasterDistanceInput criteria = new MasterDistanceInput();
            //var rawData2 = _masterDistanceBLL.GetListViewTransportationOrder(filter).Where(c => c.IsActive == 1 && CanAccessData(c)).OrderBy(od => od.RequestNo).Reverse();
            var rawData = _masterDistanceBLL.GetExportMasterDistance(criteria);
            //foreach (var rd in rawData)
            //{
            //    //if (!string.IsNullOrWhiteSpace(rd.CreatedBy))
            //    //{
            //    //    var usr = _bllMstUser.GetMasterUsers(new MasterUserInput() { IDUser = rd.CreatedBy }).FirstOrDefault();
            //    //    if (usr != null)
            //    //        rd.CreatedBy = usr.FullName;
            //    //    rd.TotalVehicle = _transportOrderBLL.CountVehicle(rd.RequestNo);
            //    //}
            //}

            try
            {
                using (ExcelExportHelper eeh = new ExcelExportHelper())
                {
                    MasterLocationDTO a;
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
                    var path = "MasterDistance_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    eeh.Package.SaveAs(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/Assets/Download/" + path));
                    return Json(path);

                    //eeh.Package.SaveAs(new FileInfo(path));
                    //var downloadUrl = Url.Content("~/Assets/Download/" + path);
                    //return Json(new { url = downloadUrl });

                }
            }
            catch { }

            return Json(new NonQueryResult(false, "Failed to export data"));
            }
            #endregion
        }
}