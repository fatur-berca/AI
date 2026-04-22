using System;
using System.Collections.Generic;
using System.Linq;
using DFIS.Utils;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.Repositories
{
    public class MasterVendorSuggestionRepo : TOMGenericRepository<MasterVendorSuggestion>, IMasterVendorSuggestionRepo
    {
        public MasterVendorSuggestionRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public List<MasterVendorSuggestion> GetAllMasterVendorSuggestion(MasterVendorSuggestionInput criteria)
        {
            var queryFilter = PredicateHelper.True<MasterVendorSuggestion>();

            if (criteria.filterSenderLocation != null && !String.IsNullOrEmpty(criteria.filterSenderLocation[0]))
            {
                queryFilter = queryFilter.And(x => criteria.filterSenderLocation.Contains(x.StartLocation));
            }

            return Get(queryFilter).ToList();
        }

        public MasterVendorSuggestion GetMasterVendorSuggestionByField(string idStartLoc, string idReceiveLoc, string orderType, string transCat, string Transmode, string vechType)
        {
            var queryFilter = PredicateHelper.True<MasterVendorSuggestion>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.StartLocation == idStartLoc);
            queryFilter = queryFilter.And(x => x.ReceiverIDLocation == idReceiveLoc);
            queryFilter = queryFilter.And(x => x.OrderType == orderType);
            queryFilter = queryFilter.And(x => x.TransportationCategory == transCat);
            queryFilter = queryFilter.And(x => x.TransportationMode == Transmode);
            queryFilter = queryFilter.And(x => x.VehicleType == vechType);
            return Get(queryFilter).FirstOrDefault();
        }

        public void SaveData(MasterVendorSuggestion input, bool status)
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

        public List<MasterVendorSuggestion> GetMasterVendorSuggestionListByField(string idStartLoc, string idReceiveLoc, string orderType, string transCat, string Transmode, string vechType)
        {
            var queryFilter = PredicateHelper.True<MasterVendorSuggestion>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.StartLocation == idStartLoc);
            queryFilter = queryFilter.And(x => x.ReceiverIDLocation == idReceiveLoc);
            queryFilter = queryFilter.And(x => x.OrderType == orderType);
            queryFilter = queryFilter.And(x => x.TransportationCategory == transCat);
            queryFilter = queryFilter.And(x => x.TransportationMode == Transmode);
            queryFilter = queryFilter.And(x => x.VehicleType == vechType);
            return Get(queryFilter).ToList();
        }

    }
}
