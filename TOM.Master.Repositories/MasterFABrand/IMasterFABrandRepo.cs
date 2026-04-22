using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterFABrandRepo : IGenericRepository<MasterFABrand>
    {
        MasterFABrand CheckBrandAvailabilityBySpeakingCode(string speakingCode);
        MasterFABrand GetMasterFABrandByFaCode(string facode);
        List<string> ListAvailableSpeakingCode();
        MasterFABrand CheckBrandAvailabilityByCode(string Code);
        MasterFABrand CheckBrand(string FACode, string LongSpeakingCode);
        MasterFABrand CheckBrandFACode(string FACode);
        MasterFABrand CheckBrandLONGCode(string LongSpeakingCode);
    }
}
