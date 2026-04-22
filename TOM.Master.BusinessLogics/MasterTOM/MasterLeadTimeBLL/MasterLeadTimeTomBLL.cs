using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using System.Web;
using System.Diagnostics;
using DFIS.Universal;

namespace TOM.Master.BusinessLogics
{
    public class MasterLeadTimeTomBLL : IMasterLeadTimeTomBLL
    {
        private readonly IMasterLeadTimeTomRepo _masterLeadTimeTomRepo;
        private readonly IGenericRepository<MasterLeadTime> _generalRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IGenericRepository<MasterVendor> _generalVendor;

        public MasterLeadTimeTomBLL(IMasterLeadTimeTomRepo masterLeadTimeTomRepo, IMasterListRepo MasterListRepo, IGenericRepository<MasterLeadTime> generalRepo, IGenericRepository<MasterVendor> generalVendor)
        {
            _masterLeadTimeTomRepo = masterLeadTimeTomRepo;
            _masterListRepo = MasterListRepo;
            _generalRepo = generalRepo;
            _generalVendor = generalVendor;
        }
        public List<MasterLeadTimeTomDTO> GetAllMasterLeadTimeTOM(MasterLeadTimeTomInput input)
        {
            var objs = _masterLeadTimeTomRepo.GetAllMasterLeadTimeTOM(input);
            return Mapper.Map<List<MasterLeadTime>, List<MasterLeadTimeTomDTO>>(objs);
        }

        public List<MasterList> GetTmList()
        {
            return _masterListRepo.GetMasterListByFieldName("TransportationMode");
        }
        public List<MasterList> GetViaList()
        {
            return _masterListRepo.GetMasterListByFieldName("RouteVia");
        }
        public List<MasterList> GetTypeList()
        {
            List<MasterList> tempList = new List<MasterList>();
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterList>>(_masterListRepo.GetMasterListByFieldName("DataTypeLeadTime"))); //"")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterList>>(_masterListRepo.GetMasterListByFieldName("VendorCategory")));
            return tempList;
        }

        public List<MasterVendorTOMDTO> GetDropDownVendor()
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = (m => m.IsActive == true);
            var dbResult = _generalVendor.Get(queryFilter).OrderByDescending(m => m.VendorName).ToList();
            return Mapper.Map<List<MasterVendorTOMDTO>>(dbResult);
        }

        public List<MasterVendorTOMDTO> GetMasterVendorTom(MasterVendorTOMInput input)
        {
            var queryFilter = PredicateHelper.True<MasterVendor>();
            queryFilter = queryFilter.And(m => m.IsActive == true);
            if (!String.IsNullOrEmpty(input.VendorCategory))
            {
                queryFilter = queryFilter.And(m => m.VendorCategory == input.VendorCategory);
            }
            if (!String.IsNullOrEmpty(input.TransportationMode))
            {
                queryFilter = queryFilter.And(m => m.TransportationMode == input.TransportationMode);
            }
            if (!String.IsNullOrEmpty(input.VendorName))
            {
                queryFilter = queryFilter.And(m => m.VendorName == input.VendorName);
            }
            //input.SortExpression = "VendorName";
            //input.SortOrder = "ASC";
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "VendorName" }, "ASC");
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterVendor>();
            var dbResult = _generalVendor.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterVendorTOMDTO>>(dbResult);
        }

        public List<MasterLeadTimeTomDTO> GetMasterLeadTimeToms(MasterLeadTimeTomInput input)
        {
            var queryFilter = PredicateHelper.True<MasterLeadTime>();
            if (!string.IsNullOrEmpty(input.SenderIDLocation))
                queryFilter = queryFilter.And(m => m.SenderIDLocation == input.SenderIDLocation);

            if (!string.IsNullOrEmpty(input.ReceiverIDLocation))
            {
                queryFilter = queryFilter.And(m => m.ReceiverIDLocation == input.ReceiverIDLocation);
            }
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterLeadTime>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            return Mapper.Map<List<MasterLeadTimeTomDTO>>(dbResult);
        }
        public int SaveData(MasterLeadTimeTomDTO input)
        {
            var dbMstLeadTimeTom = Mapper.Map<MasterLeadTime>(input);
            var context = new TOMContextDB();
            // cek exist data utk di reject
            var rejectData = context.MasterLeadTimes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.SenderIDLocation == input.SenderIDLocation)
                     .Where(m => m.ReceiverIDLocation == input.ReceiverIDLocation)
                     .Where(m => m.EffectiveStartDate == input.EffectiveStartDate).FirstOrDefault();
            if (rejectData == null)
            {
                // cek jika start date yg baru lebih kecil dari list data
                var dbRes2 = context.MasterLeadTimes
                         .Where(m => m.IDVendor == input.IDVendor)
                         .Where(m => m.SenderIDLocation == input.SenderIDLocation)
                         .Where(m => m.ReceiverIDLocation == input.ReceiverIDLocation)
                         .Where(m => m.EffectiveStartDate > input.EffectiveStartDate).OrderBy(m => m.EffectiveStartDate).FirstOrDefault();
                if (dbRes2 != null)
                {
                    // insert new     
                    Debug.WriteLine("ID exist:" + dbRes2.IDLeadTime);
                    dbMstLeadTimeTom.EffectiveEndDate = dbRes2.EffectiveStartDate.AddDays(-1);
                    dbMstLeadTimeTom.ThroughIDLocation = "NODATA";
                    dbMstLeadTimeTom.CreatedDate = DateTime.Now;
                    dbMstLeadTimeTom.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(dbMstLeadTimeTom);
                    _generalRepo.Save();
                    return 0;
                }
                else
                {
                    // cek exist data update effective date
                    var dbRes = context.MasterLeadTimes
                             .Where(m => m.IDVendor == input.IDVendor)
                             .Where(m => m.SenderIDLocation == input.SenderIDLocation)
                             .Where(m => m.ReceiverIDLocation == input.ReceiverIDLocation)
                             .Where(m => m.EffectiveStartDate < input.EffectiveStartDate)
                             .Where(m => m.EffectiveEndDate >= input.EffectiveStartDate).FirstOrDefault();
                    if (dbRes != null)
                    {
                        // update existing data
                        dbRes.EffectiveEndDate = input.EffectiveStartDate.AddDays(-1);
                        dbRes.UpdatedDate = DateTime.Now;
                        dbRes.UpdatedBy = input.UpdatedBy;
                        context.SaveChanges();
                    }
                    // insert new   
                    dbMstLeadTimeTom.ThroughIDLocation = "NODATA";
                    dbMstLeadTimeTom.CreatedDate = DateTime.Now;
                    dbMstLeadTimeTom.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(dbMstLeadTimeTom);
                    _generalRepo.Save();
                    return 0;
                }
            }
            else
            {
                return 1;
            }
        }

        public int EditData(MasterLeadTimeTomDTO input)
        {
            var context = new TOMContextDB();
            // cek exist data apakah start datenya between data sebelumnya
            var prevData = context.MasterLeadTimes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.SenderIDLocation == input.SenderIDLocation)
                     .Where(m => m.ReceiverIDLocation == input.ReceiverIDLocation)
                     .Where(m => m.EffectiveStartDate < input.EffectiveStartDate)
                     .Where(m => m.EffectiveEndDate >= input.EffectiveStartDate)
                     .Where(m => m.IDLeadTime != input.IDLeadTime).ToList();
            if (prevData.Count > 0)
            {
                return 1;
            }
            else
            {

                MasterLeadTime c = (from x in context.MasterLeadTimes
                                             where x.IDLeadTime == input.IDLeadTime
                                             select x).First();
                c.EffectiveStartDate = input.EffectiveStartDate;
                c.EffectiveEndDate = input.EffectiveEndDate;
                c.UpdatedDate = DateTime.Now;
                c.IsActive = input.IsActive;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();
                return 0;
            }
        }
        public void InsertOrUpdate(MasterLeadTimeTomDTO value, bool canUpdate = true)
        {
            // filters out the same record
            var existingRecords = _generalRepo.Get(v =>
            (value.IDLeadTime == v.IDLeadTime)
            ||
            (value.IDVendor == v.IDVendor
            && value.SenderIDLocation == v.SenderIDLocation
            && value.ReceiverIDLocation == v.ReceiverIDLocation)
               );

            // create date range helper
            DateRangeHelper hlp = new DateRangeHelper();
            foreach (var rec in existingRecords)
                hlp.Add(new DateRange(rec.EffectiveStartDate, rec.EffectiveEndDate, rec));

            value.UpdatedDate = DateTime.Now;
            if (existingRecords.Count() == 0)
            {
                // new data mode (vendor, sender or receiver is new)
                _generalRepo.Insert(Mapper.Map<MasterLeadTime>(value));
            }
            else
            {
                // update mode (vendor, sender and receiver are same)
                var record = existingRecords.Where(mtl => mtl.IDLeadTime == value.IDLeadTime).FirstOrDefault();
                if (record == null)
                {
                    // get all intersections
                    var itx = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate));
                    if (itx.Count == 0)
                    {
                        // no intersection, happily insert the data :D
                        _generalRepo.Insert(Mapper.Map<MasterLeadTime>(value));
                    }
                    else if (itx.Count == 1)
                    {
                        // the new data can only intersects the inf ended data
                        var itr = itx.ElementAt(0);
                        if (itr.SourceRange.DateEnd.ToString("yyyy-MM-dd") != "2999-12-31")
                            throw new EffectiveDateConflictException();
                        // but only when the new data starts after the start date of the intersected data
                        if (itr.SourceRange.DateStart >= value.EffectiveStartDate)
                            throw new EffectiveDateConflictException();
                        // update the old data
                        if (itr.SourceRange.Tag is MasterLeadTime)
                        {
                            var oldValue = itr.SourceRange.Tag as MasterLeadTime;
                            oldValue.EffectiveEndDate = value.EffectiveStartDate - new TimeSpan(1, 0, 0, 0);
                            _generalRepo.Update(oldValue);
                        }
                        // insert the new data
                        _generalRepo.Insert(Mapper.Map<MasterLeadTime>(value));
                    }
                    else
                        throw new EffectiveDateConflictException();

                }
                else
                {
                    // cannot intersect with anything but itself
                    var recs = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate));
                    if ((recs.Count == 1 && recs.ElementAt(0).SourceRange.Tag is MasterLeadTime &&
                        (recs.ElementAt(0).SourceRange.Tag as MasterLeadTime).IDLeadTime != value.IDLeadTime)
                        ||
                        recs.Count > 1)
                        throw new EffectiveDateConflictException();
                    // pure update
                    value.IDLeadTime = record.IDLeadTime;
                    MappingHelper.Map(value, record);
                    _generalRepo.Update(record);
                }
            }
            _generalRepo.Save();
        }

        public MasterLeadTimeTomDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);
            // return result and Mapping result from entity to DTO
            // because the return type is DTO
            return Mapper.Map<MasterLeadTimeTomDTO>(result);
        }

        public bool Delete(int keyId)
        {
            var ent = _generalRepo.Get(c => c.IDLeadTime == keyId);
            if (ent.Count() > 0)
            {
                _generalRepo.Delete(ent.ElementAt(0));
                _generalRepo.Save();

                return true;
            }
            else
            {
                return false;
            }
        }

        public int UploadDataLeadTimeTom(MasterLeadTimeTomDTO input, string userid, string sheetName)
        {
            input.CreatedBy = userid;
            input.UpdatedBy = userid;
            input.IsActive = true;
            var leadtime = SaveData(input);
            return leadtime;
            /*var dbMstLeadTimeTom = Mapper.Map<MasterLeadTimeTOM>(input);
            var context = new DFISContextDB();
            // cek exist data for reject
            var rejectData = context.MasterLeadTimeTOMs
                     .Where(m => m.DataType == input.DataType)
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.IDSender == input.IDSender)
                     .Where(m => m.IDReceiver == input.IDReceiver)
                     .Where(m => m.EffectiveStartDate == input.EffectiveStartDate).FirstOrDefault();
            if (rejectData == null)
            {
                // cek jika start date yg baru lebih kecil dari list data
                var dbRes2 = context.MasterLeadTimeTOMs
                         .Where(m => m.DataType == input.DataType)
                         .Where(m => m.IDVendor == input.IDVendor)
                         .Where(m => m.IDSender == input.IDSender)
                         .Where(m => m.IDReceiver == input.IDReceiver)
                         .Where(m => m.EffectiveStartDate > input.EffectiveStartDate).OrderBy(m => m.EffectiveStartDate).FirstOrDefault();
                if (dbRes2 != null)
                {
                    // insert new     
                    Debug.WriteLine("ID exist:" + dbRes2.IDLeadTime);
                    dbMstLeadTimeTom.EffectiveEndDate = dbRes2.EffectiveStartDate.AddDays(-1);
                    dbMstLeadTimeTom.CreatedDate = DateTime.Now;
                    dbMstLeadTimeTom.UpdatedDate = DateTime.Now;
                    dbMstLeadTimeTom.UpdatedBy = userid;
                    dbMstLeadTimeTom.CreatedBy = userid;
                    _generalRepo.Insert(dbMstLeadTimeTom);
                    _generalRepo.Save();
                    Debug.WriteLine("return 0");
                    return 0;
                }
                else
                {
                    // cek exist data update effective date
                    var dbRes = context.MasterLeadTimeTOMs
                             .Where(m => m.DataType == input.DataType)
                             .Where(m => m.IDVendor == input.IDVendor)
                             .Where(m => m.IDSender == input.IDSender)
                             .Where(m => m.IDReceiver == input.IDReceiver)
                             .Where(m => m.EffectiveStartDate < input.EffectiveStartDate)
                             .Where(m => m.EffectiveEndDate >= input.EffectiveStartDate).FirstOrDefault();
                    if (dbRes != null)
                    {
                        // update existing data
                        Debug.WriteLine("ID exist:" + dbRes.IDLeadTime);
                        dbRes.EffectiveEndDate = input.EffectiveStartDate.AddDays(-1);
                        dbRes.UpdatedDate = DateTime.Now;
                        dbRes.UpdatedBy = userid;
                        context.SaveChanges();                       
                    }
                    // insert new                                               
                    dbMstLeadTimeTom.CreatedDate = DateTime.Now;
                    dbMstLeadTimeTom.UpdatedDate = DateTime.Now;
                    dbMstLeadTimeTom.UpdatedBy = userid;
                    dbMstLeadTimeTom.CreatedBy = userid;
                    _generalRepo.Insert(dbMstLeadTimeTom);
                    _generalRepo.Save();
                    Debug.WriteLine("return 0");
                    return 0;
                }
            }
            else
            {
                Debug.WriteLine("ID exist:" + rejectData.IDLeadTime);
                Debug.WriteLine("return 1");
                return 1;
            }*/
        }

        #region IImporterBLL implementation Template
        bool _importBegin = false;
        public void Import(IEnumerable<MasterLeadTimeTomDTO> newData)
        {
            BeginImport();
            try
            {
                foreach (var d in newData) ImportRow(d);
                FinalizeImport();
            }
            catch (Exception ex)
            {
                CancelImport();
                throw ex;
            }
        }
        public void ImportRow(MasterLeadTimeTomDTO data)
        {
            InsertOrUpdate(data, false);
        }
        public void BeginImport()
        {
            if (_importBegin) return;
            _importBegin = true;

            _generalRepo.BeginTransaction();
        }
        public void FinalizeImport()
        {
            if (!_importBegin) return;
            _generalRepo.EndTransaction();
        }
        public void CancelImport()
        {
            if (!_importBegin) return;
            _generalRepo.EndTransaction(false);
        }
        #endregion
    }
}
