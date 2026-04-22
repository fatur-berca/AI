using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterTruckSealRepo : IGenericRepository<MasterTruckSealStock>
    {
        List<MasterTruckSealStock> GetAllMasterTruckSealActive();

        // MasterVendor GetMasterVendorByIDVendor(string iDVendor);
        // MasterVendor GetMasterVendorByVendorName(string namaVendor);
    }
}
