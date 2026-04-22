using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using System;

namespace TOM.Master.Repositories
{
    public class MasterServicePORepo : TOMGenericRepository<MasterServicePo>, IMasterServicePORepo
    {
        public MasterServicePORepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public MasterServicePo GetMasterServicePOByID(int IDServicePO)
        {
            var queryFilter = PredicateHelper.True<MasterServicePo>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.IDServicePO == IDServicePO);
            return Get(queryFilter).FirstOrDefault();
        }
        public MasterServicePo GetMasterServicePOByPONumber(string PONumber)
        {
            var queryFilter = PredicateHelper.True<MasterServicePo>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.ServicePONumber == PONumber);
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterServicePo GetMasterServicePOByVendorEffectiveDate(int idvendor, DateTime date)
        {
            var queryFilter = PredicateHelper.True<MasterServicePo>();
            queryFilter = queryFilter.And(x => x.IsActive);
            queryFilter = queryFilter.And(x => x.IDVendor == idvendor);
            queryFilter = queryFilter.And(x => date >= x.EffectiveStartDate && date <= x.EffectiveEndDate);
            return Get(queryFilter).FirstOrDefault();
        }
    }
}
