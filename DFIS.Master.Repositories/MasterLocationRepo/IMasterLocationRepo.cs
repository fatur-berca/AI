using System.Collections.Generic;

using DFIS.Contracts;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Master.Domain.Inputs;

namespace DFIS.Master.Repositories
{
    public interface IMasterLocationRepo : IGenericRepository<MasterLocation>
    {
        bool SetMasterLocationMapping(string source, string idLoc, bool value);
        List<MasterLocation> GetAllMasterLocationFromSource(string source);
        List<MasterLocation> GetAllMasterLocationActive();
        List<MasterLocation> GetAllMasterLocationActiveByType(string type);
        MasterLocation GetMasterLocationActiveByName(string locName);
        List<MasterLocation> GetAllMasterLocationActiveByParentLocation(string parloc);
        List<MasterLocation> GetAllMasterLocationActiveTRI();
        MasterLocation GetMasterLocationByID(string idLoc);
        MasterLocation GetMasterLocationByParentID(string parentID);

        /* Beetle Trap Definition Repository */
        List<MasterLocation> GetParentLocation();
        List<MasterLocation> GetLocationByWareFactTypeEachParentLocation(
            MasterLocationInput input);
        List<MasterLocation> GetLocationByWareFactType();
        string GetParentLocationEastWest(string idloc);
    }
}
