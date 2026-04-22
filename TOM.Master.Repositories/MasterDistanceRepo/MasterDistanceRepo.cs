using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.EntitiesDAL;
using DFIS.Utils;
using TOM.Master.Domain.Inputs;
using AutoMapper;

namespace TOM.Master.Repositories
{
    public class MasterDistanceRepo : TOMGenericRepository<MasterDistance>, IMasterDistanceRepo
    {
        private readonly ITOMGenericRepository<MasterDistance> _generalRepo;

        public MasterDistanceRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _generalRepo = new TOMGenericRepository<MasterDistance>(contextEntities);
        }

        public List<MasterDistance> GetAllMasterDistance(MasterDistanceInput input)
        {
            var queryFilter = PredicateHelper.True<MasterDistance>();
            if (!string.IsNullOrEmpty(input.DistanceType))
            {
                queryFilter = queryFilter.And(x => x.DistanceType.Equals(input.DistanceType, StringComparison.InvariantCultureIgnoreCase));
            }
            if (!string.IsNullOrEmpty(input.IDSender))
            {
                queryFilter = queryFilter.And(x => x.IDSender.Equals(input.IDSender, StringComparison.InvariantCultureIgnoreCase));
            }
            if (!string.IsNullOrEmpty(input.IDReceiver))
            {
                queryFilter = queryFilter.And(x => x.IDReceiver.Equals(input.IDReceiver, StringComparison.InvariantCultureIgnoreCase));
            }
            if (input.filterSenderLocation != null && !String.IsNullOrEmpty(input.filterSenderLocation[0]))
            {
                queryFilter = queryFilter.And(x => input.filterSenderLocation.Contains(x.IDSender));
            }
            /*if (input.EffectiveStartDate != null || input.EffectiveEndDate != null)
            {
                queryFilter = queryFilter.And(x => x.EffectiveStartDate >= input.EffectiveStartDate && x.EffectiveEndDate <= input.EffectiveEndDate);
            }*/
            if (!string.IsNullOrEmpty(input.filterDate))
            {
                DateTime filterDate = Convert.ToDateTime(input.filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveStartDate <= filterDate);
                queryFilter = queryFilter.And(x => x.EffectiveEndDate >= filterDate);
            }
            queryFilter = queryFilter.And(m => m.IsActive == true);
            var dists = Get(queryFilter).ToList();
            return dists;

            /*var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MasterDistance>();
            var dbResult = _generalRepo.Get(queryFilter, orderByFilter).ToList();
            var result = dbResult.OrderByDescending(x => x.IDDistance);

            return Mapper.Map<List<MasterDistanceDTO>>(result);*/
        }

        public MasterDistance GetMasterDistanceActiveByTypeSenderReceiverModeVia(string distanceType, DateTime transDate, string sender, string receiver, string transMode, string via)
        {
            var queryFilter = PredicateHelper.True<MasterDistance>();
            queryFilter = queryFilter.And(x => x.DistanceType == distanceType);
            queryFilter = queryFilter.And(x => x.IDSender == sender);
            queryFilter = queryFilter.And(x => x.IDReceiver == receiver);
            if (!string.IsNullOrEmpty(transMode))
            {
                queryFilter = queryFilter.And(x => x.TransportationMode == transMode);
            }
            if (!string.IsNullOrEmpty(via))
            {
                queryFilter = queryFilter.And(x => x.Via == via);
            }
            queryFilter = queryFilter.And(m => m.EffectiveStartDate <= transDate && m.EffectiveEndDate >= transDate);
            queryFilter = queryFilter.And(m => m.IsActive);
            var dbRes = Get(queryFilter).FirstOrDefault();

            if (dbRes != null) dbRes.Total = Math.Round((decimal)dbRes.Total);

            return dbRes;
        }

        private MasterDistanceDTO Fusion(MasterDistance dist, MasterLocation sender, MasterLocation recv, MasterLocation thr)
        {
            var dto = Mapper.Map<MasterDistanceDTO>(dist);
            if (sender != null)
                dto.SenderName = sender.LocationName;
            if (recv != null)
                dto.ReceiverName = recv.LocationName;
            if (thr != null)
                dto.ThroughName = thr.LocationName;
            return dto;
        }

        public List<MasterDistanceDTO> GetAllMasterDistanceDTO(MasterDistanceInput input)
        {
            var dists = GetAllMasterDistance(input);

            var context = new TOMContextDB();
            var locs = context.MasterLocations;

            var q = from dist in dists


                    select Fusion(dist,
                                (from locSender in locs
                                where dist.IDSender == locSender.IDLocation
                                select locSender).FirstOrDefault()
                                ,
                                (from locRecv in locs
                                where dist.IDReceiver == locRecv.IDLocation
                                select locRecv).FirstOrDefault()
                                ,
                                (from locThr in locs
                                where dist.Through == locThr.IDLocation || dist.Through == null
                                select locThr).FirstOrDefault()
                                );


            return q.ToList();
        }


        public void SaveData(MasterDistance input, bool status)
        {
            if (status)
            {
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(input);
            }
            else
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
            }
            Save();
        }
        public MasterDistance GetMasterDistanceByField(string idSenderLoc, string idReceiveLoc, DateTime? ShipmentDate)
        {
            var queryFilter = PredicateHelper.True<MasterDistance>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.IDSender == idSenderLoc);
            queryFilter = queryFilter.And(x => x.IDReceiver == idReceiveLoc);
            queryFilter = queryFilter.And(x => x.DistanceType == "KM Based");
            queryFilter = queryFilter.And(x => x.EffectiveStartDate <= ShipmentDate && x.EffectiveEndDate >= ShipmentDate);
            return Get(queryFilter).FirstOrDefault();
        }
    }
}

