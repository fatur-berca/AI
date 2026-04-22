using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using System;

namespace TOM.Master.Repositories
{
    public class MasterTruckSealRepo : TOMGenericRepository<MasterTruckSealStock>, IMasterTruckSealRepo
    {
        public MasterTruckSealRepo(TOMContextDB contextEntities) : base(contextEntities)
        {

        }
        
        public List<MasterTruckSealStock> GetAllMasterTruckSealActive()
        {
            var queryFilter = PredicateHelper.True<MasterTruckSealStock>();
            queryFilter = queryFilter.And(p => p.IsActive);
            return Get(queryFilter).ToList();
        }
    }
}
