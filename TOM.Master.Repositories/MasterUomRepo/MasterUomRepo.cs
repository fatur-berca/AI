using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;

namespace TOM.Master.Repositories
{
    public class MasterUomRepo : TOMGenericRepository<MasterUom>, IMasterUomRepo
    {
        public MasterUomRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }
        
        public MasterUom GetMasterUomByUom(string Uom)
        {
            var queryFilter = PredicateHelper.True<MasterUom>();
            queryFilter = queryFilter.And(p => p.UoM.Equals(Uom));
            return Get(queryFilter).SingleOrDefault();
        }
        public List<MasterUom> GetMasterUomByMaterial(string MaterialType)
        {
            var queryFilter = PredicateHelper.True<MasterUom>();
            queryFilter = queryFilter.And(p => p.MaterialType.Equals(MaterialType)).And(x => x.IsActive);
            return Get(queryFilter).ToList();
        }
    }
}
