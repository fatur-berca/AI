using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.Repositories
{
    public interface IMasterIMDLRoleRepo : IGenericRepository<MasterIMDLRole>
    {
        List<MasterIMDLRole> GetAllMasterIMDLRoleActive();
        MasterIMDLRole GetUserNameById(string id);
    }
}
