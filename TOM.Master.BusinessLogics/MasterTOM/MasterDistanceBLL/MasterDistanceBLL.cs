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
using OfficeOpenXml;
using System.Diagnostics;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal;

namespace TOM.Master.BusinessLogics
{
    public class MasterDistanceBLL : IMasterDistanceBLL
    {
        private readonly IMasterDistanceRepo _masterDistanceRepo;
        private readonly IGenericRepository<MasterDistance> _generalRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IMasterLocationBLL _generalLocation;

        public MasterDistanceBLL(IMasterDistanceRepo masterDistanceRepo, IMasterListRepo MasterListRepo, IGenericRepository<MasterDistance> generalRepo, IMasterLocationBLL generalLocation)
        {
            _masterDistanceRepo = masterDistanceRepo;
            _masterListRepo = MasterListRepo;
            _generalRepo = generalRepo;
            _generalLocation = generalLocation;
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
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterList>>(_masterListRepo.GetMasterListByFieldName("DataTypeMstDistance")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterList>>(_masterListRepo.GetMasterListByFieldName("VendorCategory")));
            return tempList;
        }
        public List<MasterDistanceDTO> GetAllMasterDistance(MasterDistanceInput input)
        {
            var dist = Mapper.Map<List<MasterDistance>, List<MasterDistanceDTO>>(_masterDistanceRepo.GetAllMasterDistance(input));
            foreach(var d in dist)
            {
                d.MasterLocation = Mapper.Map<MasterLocationDTO>(_generalLocation.GetById(d.IDSender));
                d.MasterLocation1 = Mapper.Map<MasterLocationDTO>(_generalLocation.GetById(d.IDReceiver));
                d.MasterLocation2 = Mapper.Map<MasterLocationDTO>(_generalLocation.GetById(d.Through));
            }
            return dist;
        }
        public List<MasterDistanceDTO> GetData()
        {
            List<MasterDistanceDTO> masterDistanceDTO = _generalRepo.Get().Select(x => new MasterDistanceDTO { CreatedBy = x.CreatedBy, CreatedDate = x.CreatedDate, IsActive = x.IsActive, Remarks = x.Remarks, UpdatedBy = x.UpdatedBy, UpdatedDate = x.UpdatedDate }).ToList();
            return Mapper.Map<List<MasterDistanceDTO>>(masterDistanceDTO); ;
        }


        public int SaveData(MasterDistanceDTO input)
        {
            var dbMstDistance = Mapper.Map<MasterDistance>(input);
            var context = new TOMContextDB();
            // cek exist data utk di reject
            var rejectData = context.MasterDistances
                     .Where(m => m.DistanceType == input.DistanceType)
                     .Where(m => m.IDSender == input.IDSender)
                     .Where(m => m.IDReceiver == input.IDReceiver)
                     .Where(m => m.EffectiveStartDate == input.EffectiveStartDate).FirstOrDefault();
            if (rejectData == null)
            {
                // cek jika start date yg baru lebih kecil dari list data
                var dbRes2 = context.MasterDistances
                         .Where(m => m.DistanceType == input.DistanceType)
                         .Where(m => m.IDSender == input.IDSender)
                         .Where(m => m.IDReceiver == input.IDReceiver)
                         .Where(m => m.EffectiveStartDate > input.EffectiveStartDate).OrderBy(m => m.EffectiveStartDate).FirstOrDefault();
                if (dbRes2 != null)
                {
                    // insert new     
                    Debug.WriteLine("ID exist:" + dbRes2.IDDistance);
                    //var totalx = input.Distance * (input.Buffer / 100);
                    //Debug.WriteLine("totalx:" + totalx);
                    //dbMstDistance.Total = input.Distance * (input.Buffer / 100);
                    dbMstDistance.EffectiveEndDate = dbRes2.EffectiveStartDate.AddDays(-1);
                    dbMstDistance.CreatedDate = DateTime.Now;
                    dbMstDistance.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(dbMstDistance);
                    _generalRepo.Save();
                    return 0;
                }
                else
                {
                    // cek exist data update effective date
                    var dbRes = context.MasterDistances
                             .Where(m => m.DistanceType == input.DistanceType)
                             .Where(m => m.IDSender == input.IDSender)
                             .Where(m => m.IDReceiver == input.IDReceiver)
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
                    //decimal totalx = input.Distance * (input.Buffer / 100);
                    //Debug.WriteLine("totalx:" + totalx);
                    //dbMstDistance.Total = Convert.ToDecimal(input.Distance) * Convert.ToDecimal(input.Buffer / 100);
                    dbMstDistance.CreatedDate = DateTime.Now;
                    dbMstDistance.UpdatedDate = DateTime.Now;
                    _generalRepo.Insert(dbMstDistance);
                    _generalRepo.Save();
                    //Debug.WriteLine("Total: " + totalx);
                    return 0;
                }

            }
            else
            {
                return 1;
            }
        }
        public int EditData(MasterDistanceDTO input)
        {
            var context = new TOMContextDB();
            // cek exist data apakah start datenya between data sebelumnya
            var prevData = context.MasterDistances
                     .Where(m => m.DistanceType == input.DistanceType)
                     .Where(m => m.IDSender == input.IDSender)
                     .Where(m => m.IDReceiver == input.IDReceiver)
                     .Where(m => m.EffectiveStartDate < input.EffectiveStartDate)
                     .Where(m => m.EffectiveEndDate >= input.EffectiveStartDate)
                     .Where(m => m.IDDistance != input.IDDistance).ToList();
            if (prevData.Count > 0)
            {
                return 1;
            }
            else
            {
                MasterDistance c = (from x in context.MasterDistances
                                    where x.IDDistance == input.IDDistance
                                    select x).First();
                c.EffectiveStartDate = input.EffectiveStartDate;
                c.EffectiveEndDate = input.EffectiveEndDate;
                c.UpdatedDate = DateTime.Now;
                c.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();
                return 0;
            }
        }
        public MasterDistanceDTO GetById(int id)
        {
            var result = _generalRepo.GetByID(id);
            return Mapper.Map<MasterDistanceDTO>(result);
        }
        public int UploadDataDistance(MasterDistanceDTO input, string userid, string sheetName)
        {
            input.CreatedBy = userid;
            input.UpdatedBy = userid;
            input.IsActive = true;
            var distance = SaveData(input);
            return distance;

            /*var dbMstDistance = Mapper.Map<MasterDistance>(input);
            var context = new DFISContextDB();
            // cek exist data for reject
            var rejectData = context.MasterDistances
                     .Where(m => m.DistanceType == input.DistanceType)
                     .Where(m => m.IDSender == input.IDSender)
                     .Where(m => m.IDReceiver == input.IDReceiver)
                     .Where(m => m.EffectiveStartDate == input.EffectiveStartDate).FirstOrDefault();
            if (rejectData == null)
            {
                // cek jika start date yg baru lebih kecil dari list data
                var dbRes2 = context.MasterDistances
                         .Where(m => m.DistanceType == input.DistanceType)
                         .Where(m => m.IDSender == input.IDSender)
                         .Where(m => m.IDReceiver == input.IDReceiver)
                         .Where(m => m.EffectiveStartDate > input.EffectiveStartDate).OrderBy(m => m.EffectiveStartDate).FirstOrDefault();
                if (dbRes2 != null)
                {
                    // insert new     
                    Debug.WriteLine("ID exist:" + dbRes2.IDDistance);
                    var totalx = input.Distance * input.Buffer / 100;
                    dbMstDistance.EffectiveEndDate = dbRes2.EffectiveStartDate.AddDays(-1);
                    dbMstDistance.Total = totalx;
                    dbMstDistance.CreatedDate = DateTime.Now;
                    dbMstDistance.UpdatedDate = DateTime.Now;
                    dbMstDistance.UpdatedBy = userid;
                    dbMstDistance.CreatedBy = userid;
                    _generalRepo.Insert(dbMstDistance);
                    _generalRepo.Save();
                    Debug.WriteLine("return 0");
                    return 0;
                }
                else
                {
                    // cek exist data update effective date
                    var dbRes = context.MasterDistances
                             .Where(m => m.DistanceType == input.DistanceType)                             
                             .Where(m => m.IDSender == input.IDSender)
                             .Where(m => m.IDReceiver == input.IDReceiver)
                             .Where(m => m.EffectiveStartDate < input.EffectiveStartDate)
                             .Where(m => m.EffectiveEndDate >= input.EffectiveStartDate).FirstOrDefault();
                    if (dbRes != null)
                    {
                        // update existing data
                        Debug.WriteLine("ID exist:" + dbRes.IDDistance);
                        dbRes.EffectiveEndDate = input.EffectiveStartDate.AddDays(-1);
                        dbRes.UpdatedDate = DateTime.Now;
                        dbRes.UpdatedBy = userid;
                        context.SaveChanges();
                        //_generalRepo.Update(dbRes);
                        //_generalRepo.Save();
                    }
                    // insert new        
                    var totalx = input.Distance * input.Buffer / 100;
                    dbMstDistance.Total = totalx;
                    dbMstDistance.CreatedDate = DateTime.Now;
                    dbMstDistance.UpdatedDate = DateTime.Now;
                    dbMstDistance.UpdatedBy = userid;
                    dbMstDistance.CreatedBy = userid;
                    _generalRepo.Insert(dbMstDistance);
                    _generalRepo.Save();
                    Debug.WriteLine("return 0");
                    return 0;                    
                }                
            }
            else
            {
                Debug.WriteLine("ID exist:" + rejectData.IDDistance);
                Debug.WriteLine("return 1");
                return 1;                
            }
            //return 1;  */
        }
        public List<MasterLocationDTO> GetSenderReceiverTOM(List<string> ListLocation)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            //queryFilter = queryFilter.And(m => m.Type == "Warehouse" && m.IsActive == true && ListLocation.Contains(m.IDLocation));
            queryFilter = queryFilter.And(m => m.IsActive == true && m.IsAssigned == true &&
            (
            m.Type == "Warehouse" || m.Type == "Factory" || m.Type == "Agent" || m.Type == "Other"
            ));
            var dbResult = _generalLocation.Get(queryFilter).OrderByDescending(m => m.LocationName).ToList();
            return Mapper.Map<List<MasterLocationDTO>>(dbResult);
        }

        public bool DelData(int keyID)
        {
            var ent = _generalRepo.Get(c => c.IDDistance == keyID);
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

        public int RecalculateTotal()
        {
            try
            {
                var context = new TOMContextDB();
                return context.Database.ExecuteSqlCommand("UPDATE MasterDistance SET Total = ROUND(Distance + (Buffer * Distance / 100),0)");
            }
            catch { }
            return -1;
        }

        public bool _spc_updateKMBased()
        {
            try
            {
                var context = new TOMContextDB();
                context.Database.ExecuteSqlCommand("UPDATE MasterDistance SET Buffer = 5 WHERE Buffer=0 AND DistanceType='KM Based'");
                return true;
            }
            catch { }
            return false;
        }

        public void SaveData(MasterDistance input, bool status)
        {
            _masterDistanceRepo.SaveData(input, status);
        }
        public void InsertOrUpdate(MasterDistanceDTO value, bool canUpdate = true)
        {
            // filters out the same record
            var existingRecords = _generalRepo.Get(v =>
            (value.IDDistance == v.IDDistance)
            ||
            (value.DistanceType == v.DistanceType
            && value.IDReceiver == v.IDReceiver
            && value.IDSender == v.IDSender)
               );

            // create date range helper
            DateRangeHelper hlp = new DateRangeHelper();
            foreach (var rec in existingRecords)
                hlp.Add(new DateRange(rec.EffectiveStartDate, rec.EffectiveEndDate, rec));

            value.UpdatedDate = DateTime.Now;
            if (existingRecords.Count() == 0)
            {
                // new data mode (type, sender or receiver is new)
                _generalRepo.Insert(Mapper.Map<MasterDistance>(value));
            }
            else
            {
                // update mode (type, sender and receiver are same)
                var record = existingRecords.Where(mtl => mtl.IDDistance == value.IDDistance).FirstOrDefault();
                if (record == null)
                {
                    // get all intersections
                    var itx = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate));
                    if (itx.Count == 0)
                    {
                        // no intersection, happily insert the data :D
                        _generalRepo.Insert(Mapper.Map<MasterDistance>(value));
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
                        if (itr.SourceRange.Tag is MasterDistance)
                        {
                            var oldValue = itr.SourceRange.Tag as MasterDistance;
                            oldValue.EffectiveEndDate = value.EffectiveStartDate - new TimeSpan(1, 0, 0, 0);
                            _generalRepo.Update(oldValue);
                        }
                        // insert the new data
                        _generalRepo.Insert(Mapper.Map<MasterDistance>(value));
                    }
                    else
                        throw new EffectiveDateConflictException();

                }
                else
                {
                    // cannot intersect with anything but itself
                    var recs = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate));
                    if ((recs.Count == 1 && recs.ElementAt(0).SourceRange.Tag is MasterDistance &&
                        (recs.ElementAt(0).SourceRange.Tag as MasterDistance).IDDistance != value.IDDistance)
                        ||
                        recs.Count > 1)
                        throw new EffectiveDateConflictException();
                    // pure update
                    value.IDDistance = record.IDDistance;
                    MappingHelper.Map(value, record);
                    _generalRepo.Update(record);
                }
            }


            _generalRepo.Save();
        }

        #region IImporterBLL implementation Template
        bool _importBegin = false;
        public void Import(IEnumerable<MasterDistanceDTO> newData)
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
        public void ImportRow(MasterDistanceDTO data)
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

        public List<MasterDistanceDTO> GetAllMasterDistanceLocation(MasterDistanceInput input)
        {
            return _masterDistanceRepo.GetAllMasterDistanceDTO(input);
        }

        public List<MasterDistanceDTO> GetExportMasterDistance(MasterDistanceInput input)
        {
            using (var context = new TOMContextDB())
            {
                var mde = (from md in context.MasterDistances
                           join sender in context.MasterLocations on md.IDSender equals sender.IDLocation
                           join receiver in context.MasterLocations on md.IDReceiver equals receiver.IDLocation
                           select new MasterDistanceDTO
                           {
                               IDDistance = md.IDDistance,
                               DistanceType = md.DistanceType,
                               IDSender = md.IDSender,
                               SenderLocationName = sender.LocationName,
                               IDReceiver = md.IDReceiver,
                               ReceiverLocationName = receiver.LocationName,
                               TransportationMode = md.TransportationMode,
                               Distance = md.Distance,
                               Buffer = md.Buffer,
                               Total = md.Total,
                               Through = md.Through,
                               Via = md.Via,
                               EffectiveStartDate = md.EffectiveStartDate,
                               EffectiveEndDate = md.EffectiveEndDate,
                               IsActive = md.IsActive,
                               CreatedBy = md.CreatedBy,
                               CreatedDate = md.CreatedDate,
                               UpdatedBy = md.UpdatedBy,
                               UpdatedDate = md.UpdatedDate,
                               Remarks = md.Remarks
                           }).Where(x => x.IsActive == true).ToList();


                #region comment
                //var mde = (from x in context.MasterDistances
                //            select new MasterDistanceDTO
                //            {
                //                IDDistance = x.IDDistance,
                //                DistanceType = x.DistanceType,
                //                IDSender = x.IDSender,
                //                IDReceiver = x.IDReceiver,
                //                TransportationMode = x.TransportationMode,
                //                Distance = x.Distance,
                //                Buffer = x.Buffer,
                //                Total = x.Total,
                //                Through = x.Through,
                //                Via = x.Via,
                //                EffectiveStartDate = x.EffectiveStartDate,
                //                EffectiveEndDate = x.EffectiveEndDate,
                //                IsActive = x.IsActive,
                //                CreatedBy = x.CreatedBy,
                //                CreatedDate = x.CreatedDate,
                //                UpdatedBy = x.UpdatedBy,
                //                UpdatedDate = x.UpdatedDate,
                //                Remarks = x.Remarks,
                //                //SenderName = x.SenderName,
                //                //ReceiverName = x.ReceiverName,
                //                //ThroughName = x.ThroughName
                //            }).ToList();
                #endregion

                return mde;
            }
        }
    }
}
