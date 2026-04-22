using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterVendorTOMRepo : IGenericRepository<MasterVendor>
    {
        List<MasterVendor> GetAllMasterVendor();
        List<MasterVendor> GetAllMasterVendorActive();
        List<MasterVendor> GetAllMasterVendorByListVendorName(List<string> vendorName);
        List<MasterVendor> GetAllMasterVendorActiveNoChild();
        MasterVendor GetMasterVendorByIDVendor(string IDVendor);
        MasterVendor GetMasterVendorByID(int IDVendor);
        MasterVendor GetMasterVendorByZone(int IDVendor, string zoneBased = "");
        MasterVendor GetMasterVendorByVendorName(string namaVendor);
        void SaveData(MasterVendor input, bool status);
    }
}
