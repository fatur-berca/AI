using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterVendorRepo : IGenericRepository<MasterVendor>
    {
        List<MasterVendor> GetAllMasterVendorActive();
        MasterVendor GetMasterVendorByIDVendor(string iDVendor);
        MasterVendor GetMasterVendorByVendorName(string namaVendor);
    }
}
