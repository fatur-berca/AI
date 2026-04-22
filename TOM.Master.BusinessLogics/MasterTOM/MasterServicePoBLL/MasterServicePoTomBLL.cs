using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using DFIS.Utils;
using DFIS.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Repositories;
using DFIS.Universal;

namespace TOM.Master.BusinessLogics
{
    public class MasterServicePoTomBLL : IMasterServicePoTomBLL
    {
        private readonly IGenericRepository<MasterServicePo> _generalRepo;
        private readonly IMasterVendorTOMRepo _masterVendorTOMRepo;

        public MasterServicePoTomBLL(IGenericRepository<MasterServicePo> generalRepo, IMasterVendorTOMRepo masterVendorTOMRepo)
        {
            _generalRepo = generalRepo;
            _masterVendorTOMRepo = masterVendorTOMRepo;
        }
        public List<MasterServicePoTomDTO> GetData(MasterServicePoTomInput input)
        {
            var queryFilter = PredicateHelper.True<MasterServicePo>();
            if (input.IDVendor != 0 && input.IDVendor != null)
            {
                queryFilter = queryFilter.And(m => m.IDVendor == input.IDVendor);
            }

            if (!String.IsNullOrEmpty(input.ServicePONumber))
            {
                queryFilter = queryFilter.And(m => m.ServicePONumber.Equals(input.ServicePONumber, StringComparison.InvariantCultureIgnoreCase));
            }            

            if (!string.IsNullOrEmpty(input.filterDate))
            {
                DateTime filterDate = Convert.ToDateTime(input.filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveStartDate <= filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveEndDate >= filterDate);
            }
            queryFilter = queryFilter.And(m => m.IsActive == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterServicePo>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            //var result = dbResult.OrderByDescending(x => x.MasterVendor.VendorName);

            //return Mapper.Map<List<MasterServicePoTomDTO>>(result);

            var vsug = Mapper.Map<List<MasterServicePo>, List<MasterServicePoTomDTO>>(dbResult);
            foreach (var v in vsug)
            {
                v.MasterTransportVendor = Mapper.Map<MasterVendorTOMDTO>(_masterVendorTOMRepo.Get(c => c.IDVendor == v.IDVendor).FirstOrDefault());
            }
            return vsug;
        }

        public int SaveData(MasterServicePoTomDTO input)
        {
            var dbService = Mapper.Map<MasterServicePo>(input);
            var context = new TOMContextDB();
            DateTime maxDate = Convert.ToDateTime("2999-12-31");
            // cek if startdate = input.startdate. if exist then reject
            var condition1 = context.MasterServicePoes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.EffectiveStartDate == input.EffectiveStartDate).FirstOrDefault();
            // cek if input start date & end date di dalam range date data existing. then update end date existing
            var condition2 = context.MasterServicePoes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.EffectiveStartDate < input.EffectiveStartDate && m.EffectiveEndDate >= input.EffectiveEndDate && m.EffectiveEndDate == maxDate).FirstOrDefault();
            // cek if input start date & end date di luar range date data existing dan memotong range data existing. then reject
            var condition3 = context.MasterServicePoes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.EffectiveEndDate >= input.EffectiveStartDate && m.EffectiveStartDate <= input.EffectiveEndDate).ToList();
            // cek if input start date & end date di luar range date data existing tapi tidak memotong. if null then insert
            var condition4 = context.MasterServicePoes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.EffectiveStartDate > input.EffectiveStartDate && m.EffectiveEndDate < input.EffectiveEndDate).FirstOrDefault();
            /*if (condition1 != null)
                Debug.WriteLine("condition1:" + condition1.IDServicePO);
            if (condition2 != null)
                Debug.WriteLine("condition2:" + condition2.IDServicePO);
            if (condition3.Count > 0)
                Debug.WriteLine("condition3:" + condition3.Count);
            if (condition4 != null)
                Debug.WriteLine("condition4:" + condition4.IDServicePO);*/
            if (condition2 != null)
            {
                //Debug.WriteLine("masuk if condition2 != null");
                condition2.EffectiveEndDate = input.EffectiveStartDate.AddDays(-1);
                condition2.UpdatedDate = DateTime.Now;
                condition2.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();

                dbService.CreatedDate = DateTime.Now;
                dbService.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbService);
                _generalRepo.Save();
                return 0;
            }
            else if (condition3.Count > 0)
            {
                /*foreach (MasterServicePo record in condition3)
                {
                    Debug.WriteLine("ID Service PO:" + record.IDServicePO);
                }*/
                return 1;
            }
            else if ((condition1 == null) || (condition4 == null && condition3.Count == 0))
            {
                //Debug.WriteLine("masuk if (condition1 == null) || (condition4 == null && condition3.Count == 0)");
                dbService.CreatedDate = DateTime.Now;
                dbService.UpdatedDate = DateTime.Now;
                _generalRepo.Insert(dbService);
                _generalRepo.Save();
                return 0;
            }
            else
            {
                //Debug.WriteLine("masuk data reject");
                return 1;
            }
        }

        public int EditData(MasterServicePoTomDTO input)
        {
            var context = new TOMContextDB();
            MasterServicePo mstServicePo = context.MasterServicePoes
                     .Where(m => m.IDServicePO == input.IDServicePO).FirstOrDefault();
            DateTime maxDate = Convert.ToDateTime("2999-12-31");
            // cek if startdate = input.startdate. if exist then reject
            var condition1 = context.MasterServicePoes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.EffectiveStartDate == input.EffectiveStartDate)
                     .Where(m => m.IDServicePO != input.IDServicePO).FirstOrDefault();
            // cek if input start date & end date di dalam range date data existing. then update end date existing
            var condition2 = context.MasterServicePoes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.EffectiveStartDate < input.EffectiveStartDate && m.EffectiveEndDate >= input.EffectiveEndDate && m.EffectiveEndDate == maxDate)
                     .Where(m => m.IDServicePO != input.IDServicePO).FirstOrDefault();
            // cek if input start date & end date di luar range date data existing dan memotong range data existing. then reject
            var condition3 = context.MasterServicePoes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.EffectiveEndDate >= input.EffectiveStartDate && m.EffectiveStartDate <= input.EffectiveEndDate)
                     .Where(m => m.IDServicePO != input.IDServicePO).ToList();
            // cek if input start date & end date di luar range date data existing tapi tidak memotong. if null then insert
            var condition4 = context.MasterServicePoes
                     .Where(m => m.IDVendor == input.IDVendor)
                     .Where(m => m.EffectiveStartDate > input.EffectiveStartDate && m.EffectiveEndDate < input.EffectiveEndDate)
                     .Where(m => m.IDServicePO != input.IDServicePO).FirstOrDefault();
            if (condition2 != null)
            {
                //Debug.WriteLine("masuk if condition2 != null");
                condition2.EffectiveEndDate = input.EffectiveStartDate.AddDays(-1);
                condition2.UpdatedDate = DateTime.Now;
                condition2.UpdatedBy = input.UpdatedBy;
                context.SaveChanges();
               
                mstServicePo.EffectiveStartDate = input.EffectiveStartDate;
                mstServicePo.EffectiveEndDate = input.EffectiveEndDate;
                mstServicePo.CreatedDate = DateTime.Now;
                mstServicePo.UpdatedDate = DateTime.Now;
                context.SaveChanges();
                //_generalRepo.Update(mstServicePo);
                //_generalRepo.Save();                                
                return 0;
            }
            else if (condition3.Count > 0)
            {                
                return 1;
            }
            else if ((condition1 == null) || (condition4 == null && condition3.Count == 0))
            {
                mstServicePo.EffectiveStartDate = input.EffectiveStartDate;
                mstServicePo.EffectiveEndDate = input.EffectiveEndDate;
                mstServicePo.CreatedDate = DateTime.Now;
                mstServicePo.UpdatedDate = DateTime.Now;
                context.SaveChanges();
                //_generalRepo.Update(mstServicePo);
                //_generalRepo.Save();
                return 0;
            }
            else
            {               
                return 1;
            }       
        }

        public bool DelData(int keyID)
        {
            var ent = _generalRepo.Get(c => c.IDServicePO == keyID);
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

        public void InsertOrUpdate(MasterServicePoTomDTO value, bool canUpdate = true)
        {
            // filters out the same record
            var existingRecords = _generalRepo.Get(v =>
            (value.IDServicePO == v.IDServicePO)
            ||
            (value.IDVendor == v.IDVendor)
               );

            // create date range helper
            DateRangeHelper hlp = new DateRangeHelper();
            foreach (var rec in existingRecords)
                hlp.Add(new DateRange(rec.EffectiveStartDate, rec.EffectiveEndDate, rec));

            value.UpdatedDate = DateTime.Now;
            if (existingRecords.Count() == 0)
            {
                // new data mode (vendor, sender or receiver is new)
                value.CreatedDate = DateTime.Now;
                _generalRepo.Insert(Mapper.Map<MasterServicePo>(value));
            }
            else
            {
                // update mode (vendor, sender and receiver are same)
                var record = existingRecords.Where(mtl => mtl.IDServicePO == value.IDServicePO).FirstOrDefault();
                if (record == null)
                {
                    // get all intersections
                    var itx = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate));
                    if (itx.Count == 0)
                    {
                        // no intersection, happily insert the data :D
                        value.CreatedDate = DateTime.Now;
                        _generalRepo.Insert(Mapper.Map<MasterServicePo>(value));
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
                        if (itr.SourceRange.Tag is MasterServicePo)
                        {
                            var oldValue = itr.SourceRange.Tag as MasterServicePo;
                            oldValue.EffectiveEndDate = value.EffectiveStartDate - new TimeSpan(1, 0, 0, 0);
                            _generalRepo.Update(oldValue);
                        }
                        // insert the new data
                        value.CreatedDate = DateTime.Now;
                        _generalRepo.Insert(Mapper.Map<MasterServicePo>(value));
                    }
                    else
                        throw new EffectiveDateConflictException();

                }
                else
                {
                    // cannot intersect with anything but itself
                    var recs = hlp.IntersectWith(new DateRange(value.EffectiveStartDate, value.EffectiveEndDate));
                    if ((recs.Count == 1 && recs.ElementAt(0).SourceRange.Tag is MasterServicePo &&
                        (recs.ElementAt(0).SourceRange.Tag as MasterServicePo).IDServicePO != value.IDServicePO)
                        ||
                        recs.Count > 1)
                        throw new EffectiveDateConflictException();
                    // pure update
                    value.IDServicePO = record.IDServicePO;
                    value.CreatedDate = record.CreatedDate;
                    value.CreatedBy = record.CreatedBy;
                    MappingHelper.Map(value, record);
                    _generalRepo.Update(record);
                }
            }
            _generalRepo.Save();
        }
    }
}
