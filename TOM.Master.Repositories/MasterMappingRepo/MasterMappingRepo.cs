using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using System;
using System.Collections.Generic;

namespace TOM.Master.Repositories
{
    public class MasterMappingRepo : TOMGenericRepository<MasterMapping>, IMasterMappingRepo
    {
        public MasterMappingRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }
        
        public MasterMapping CheckAvailabilityMasterMappingByMapFrom(string mapFrom)
        {
            var queryFilter = PredicateHelper.True<MasterMapping>();
            queryFilter = queryFilter.And(x => x.MapFrom == mapFrom).And(x => x.IsActive);
            return Get(queryFilter).FirstOrDefault();
        }

        public List<string> GetMapFromByMapTo(string mapTo)
        {
            var queryFilter = PredicateHelper.True<MasterMapping>();
            queryFilter = queryFilter.And(x => x.MapTo == mapTo).And(x => x.IsActive);
            return Get(queryFilter).Select(x => x.MapFrom).ToList();
        }
    }
}
