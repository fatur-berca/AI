using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterMappingRepo : IGenericRepository<MasterMapping>
    {
        MasterMapping CheckAvailabilityMasterMappingByMapFrom(string mapFrom);
        List<string> GetMapFromByMapTo(string mapTo);
    }
}
