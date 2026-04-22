using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.Repositories
{
    public interface IMasterUserRepo : IGenericRepository<MasterUser>
    {
        List<MasterUserDTO> GetUserInLocation(string[] locs);
        List<MasterUser> GettAllMasterUserAcive();
        List<MasterUserDTO> GetByLocation(params string[] locationIDs);
        MasterUser GetUserNameById(string id);
        List<string> GetEmailInLocation(string[] locs);
        List<string> GetIDUserInLocation(string[] locs);
    }
}
