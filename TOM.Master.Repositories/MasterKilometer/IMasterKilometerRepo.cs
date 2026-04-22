using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterKilometerRepo : IGenericRepository<MasterKilometer>
    {
        List<MasterKilometer> GetAllMasterKilometer();
        void DeleteData();
        MasterKilometer SaveData(MasterKilometer input, bool status);
    }
}
