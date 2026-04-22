using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Repositories
{
    public class MasterIMDLRoleRepo : TOMGenericRepository<MasterIMDLRole>, IMasterIMDLRoleRepo
    {
        public MasterIMDLRoleRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }
        
        public List<MasterIMDLRole> GetAllMasterIMDLRoleActive()
        {
            var queryFilter = PredicateHelper.True<MasterIMDLRole>();
            queryFilter = queryFilter.And(p => p.IsActive);
            return Get(queryFilter).ToList();
        }

        public MasterIMDLRole GetUserNameById(string id)
        {
            var queryFilter = PredicateHelper.True<MasterIMDLRole>();
            queryFilter = queryFilter.And(p => p.IsActive);
            if (!String.IsNullOrEmpty(id))
                queryFilter = queryFilter.And(p => p.IMDLRole == id);
            return Get(queryFilter).FirstOrDefault();
        }
    }
}
