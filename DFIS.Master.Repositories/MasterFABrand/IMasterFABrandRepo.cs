using System.Collections.Generic;
using DFIS.Contracts;
using DFIS.EntitiesDAL.EDMX;

namespace DFIS.Master.Repositories
{
    public interface IMasterFABrandRepo : IGenericRepository<MasterFABrand>
    {
        MasterFABrand CheckBrandAvailabilityBySpeakingCode(string speakingCode);
        List<string> ListAvailableSpeakingCode();
    }
}
