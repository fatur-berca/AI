using System.Collections.Generic;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Repositories;
using System.Linq;
using System;
using DFIS.Contracts;
using DFIS.Utils;
using TOM.EntitiesDAL;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal;

namespace TOM.Master.BusinessLogics
{
    public class MasterCostCenterAccountBLL : IMasterCostCenterAccountBLL
    {
        private readonly IMasterCostCenterAccountRepo _masterCostCenterAccountRepo;
        private readonly IMasterListRepo _masterListRepo;
        private readonly IGenericRepository<MasterCostCenter> _generalRepo;
        private readonly IMasterLocationRepo _masterLoc;

        public MasterCostCenterAccountBLL(IMasterLocationRepo masterLoc, IMasterCostCenterAccountRepo masterCostCenterAccountRepo, IMasterListRepo masterListRepo, IGenericRepository<MasterCostCenter> genRepo)
        {
            _masterCostCenterAccountRepo = masterCostCenterAccountRepo;
            _masterListRepo = masterListRepo;
            _generalRepo = genRepo;
            _masterLoc = masterLoc;
        }

        public List<MasterListDTO> GetList()
        {
            List<MasterListDTO> tempList = new List<MasterListDTO>();
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("MaterialType")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("CostCenter")));
            tempList.AddRange(Mapper.Map<List<MasterList>, List<MasterListDTO>>(_masterListRepo.GetMasterListByFieldName("Account")));
            return tempList;
        }

        public List<MasterCostCenterAccountDTO> GetALLMasterCostCenterAccount(MasterCostCenterAccountInput input)
        {
            var cca = Mapper.Map<List<MasterCostCenter>, List<MasterCostCenterAccountDTO>>(_masterCostCenterAccountRepo.GetAllMasterCostCenterAccount(input));

            foreach (var c in cca)
            {
                c.MasterLocation = Mapper.Map<MasterLocationDTO>(_masterLoc.Get(e => e.IDLocation == c.Sender).FirstOrDefault());
                c.MasterLocation1 = Mapper.Map<MasterLocationDTO>(_masterLoc.Get(e => e.IDLocation == c.Receiver).FirstOrDefault());
            }

            return cca;
        }

        public MasterCostCenterAccountDTO GetMasterCostCenterAccountBySenderReceiverMaterial(MasterCostCenterAccountInput input)
        {
            var cca = Mapper.Map<MasterCostCenterAccountDTO>(_masterCostCenterAccountRepo.GetCostCenterBySenderReceiverMaterial(input));

            return cca;
        }

        public int SaveData(MasterCostCenter input, bool status)
        {
            if (status == true)
            {
                var context = new TOMContextDB();
                // cek exist data utk di reject
                var rejectData = context.MasterCostCenters
                         .Where(m => m.Sender == input.Sender)
                         .Where(m => m.Receiver == input.Receiver)
                         .Where(m => m.EffectiveStartDate == input.EffectiveStartDate).FirstOrDefault();
                if (rejectData == null)
                {
                    // cek jika start date yg baru lebih kecil dari list data
                    var dbRes2 = context.MasterCostCenters
                             .Where(m => m.Sender == input.Sender)
                             .Where(m => m.Receiver == input.Receiver)
                             .Where(m => m.EffectiveStartDate > input.EffectiveStartDate).OrderBy(m => m.EffectiveStartDate).FirstOrDefault();
                    if (dbRes2 != null)
                    {
                        // insert new     
                        input.EffectiveEndDate = dbRes2.EffectiveStartDate.AddDays(-1);
                        _masterCostCenterAccountRepo.SaveData(input, status);
                        return 0;
                    }
                    else
                    {
                        // cek exist data update effective date
                        var dbRes = context.MasterCostCenters
                                 .Where(m => m.Sender == input.Sender)
                                 .Where(m => m.Receiver == input.Receiver)
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
                        _masterCostCenterAccountRepo.SaveData(input, status);
                        return 0;
                    }
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                var context = new TOMContextDB();
                // cek exist data apakah start datenya between data sebelumnya
                var prevData = context.MasterCostCenters
                         .Where(m => m.Sender == input.Sender)
                         .Where(m => m.Receiver == input.Receiver)
                         .Where(m => m.EffectiveStartDate < input.EffectiveStartDate)
                         .Where(m => m.EffectiveEndDate >= input.EffectiveStartDate)
                         .Where(m => m.IDCostCenter != input.IDCostCenter).ToList();
                if (prevData.Count > 0)
                {
                    return 1;
                }
                else
                {
                    _masterCostCenterAccountRepo.SaveData(input, status);
                    return 0;
                }
            }
        }

        public void InsertOrUpdate(MasterCostCenterAccountDTO value, bool canUpdate = true)
        {

            // filters out the same record
            var existingRecords = _generalRepo.Get(v =>
            (value.IDCostCenter == v.IDCostCenter)
            ||
            (value.Receiver == v.Receiver
            && value.Sender == v.Sender
            && value.MaterialType == v.MaterialType)
               );

            // create date range helper
            DateRangeHelper hlp = new DateRangeHelper();
            foreach (var rec in existingRecords)
                hlp.Add(new DateRange(rec.EffectiveStartDate, rec.EffectiveEndDate == null ? DateTime.Parse("2999-12-31") : (DateTime)rec.EffectiveEndDate, rec));

            value.UpdatedDate = DateTime.Now;
            if (existingRecords.Count() == 0)
            {
                // new data mode (type, sender or receiver is new)
                _generalRepo.Insert(MappingHelper.Map<MasterCostCenter>(value));
            }
            else
            {
                // update mode (type, sender and receiver are same)
                var record = existingRecords.Where(mtl => mtl.IDCostCenter == value.IDCostCenter).FirstOrDefault();
                if (record == null)
                {
                    // get all intersections
                    var itx = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate == null ? DateTime.Parse("2999-12-31") : (DateTime)value.EffectiveEndDate));
                    if (itx.Count == 0)
                    {
                        // no intersection, happily insert the data :D
                        _generalRepo.Insert(MappingHelper.Map<MasterCostCenter>(value));
                    }
                    else if (itx.Count == 1)
                    {
                        // the new data can only intersects the inf ended data
                        var itr = itx.ElementAt(0);
                        if (itr.SourceRange.DateEnd.ToString("yyyy-MM-dd") != "2999-12-31")
                            throw new Exception("Save failed, Effective Start Date / Effective End Date intersect with existing data.");
                        // but only when the new data starts after the start date of the intersected data
                        if (itr.SourceRange.DateStart >= value.EffectiveStartDate)
                            throw new Exception("Save failed, Effective Start Date / Effective End Date intersect with existing data.");
                        // update the old data
                        if (itr.SourceRange.Tag is MasterCostCenter)
                        {
                            var oldValue = itr.SourceRange.Tag as MasterCostCenter;
                            oldValue.EffectiveEndDate = value.EffectiveStartDate - new TimeSpan(1, 0, 0, 0);
                            _generalRepo.Update(oldValue);
                        }
                        // insert the new data
                        _generalRepo.Insert(MappingHelper.Map<MasterCostCenter>(value));
                    }
                    else
                        throw new Exception("Save failed, Effective Start Date / Effective End Date intersect with existing data.");

                }
                else
                {
                    // cannot intersect with anything but itself
                    var recs = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate == null ? DateTime.Parse("2999-12-31") : (DateTime)value.EffectiveEndDate));
                    if ((recs.Count == 1 && recs.ElementAt(0).SourceRange.Tag is MasterCostCenter &&
                        (recs.ElementAt(0).SourceRange.Tag as MasterCostCenter).IDCostCenter != value.IDCostCenter)
                        ||
                        recs.Count > 1)
                        throw new Exception("Save failed, Effective Start Date / Effective End Date intersect with existing data.");
                    // pure update
                    value.IDCostCenter = record.IDCostCenter;
                    MappingHelper.Map(value, record);
                    _generalRepo.Update(record);
                }
            }


            _generalRepo.Save();
        }

        public bool DelData(int keyID)
        {
            var ent = _generalRepo.Get(c => c.IDCostCenter == keyID);
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

        public int SaveData(MasterCostCenterAccountDTO input, bool status)
        {
            return SaveData(MappingHelper.Map<MasterCostCenter>(input), status);
        }
    }
}
