using System;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterServicePORepo : IGenericRepository<MasterServicePo>
    {
        MasterServicePo GetMasterServicePOByID(int IDServicePO);
        MasterServicePo GetMasterServicePOByPONumber(string PONumber);
        MasterServicePo GetMasterServicePOByVendorEffectiveDate(int idvendor, DateTime date);
    }
}
