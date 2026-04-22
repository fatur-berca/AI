using System.Collections.Generic;

using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.Inputs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.Repositories
{
    public interface IMasterLocationRepo : IGenericRepository<MasterLocation>
    {
        List<MasterLocation> GetAllMasterLocation();
        List<MasterLocation> GetAllMasterLocationForDisplay();
        List<MasterLocation> GetAllMasterLocationActive();
        List<MasterLocation> GetAllMasterLocationActiveByType(string type);
        MasterLocation GetMasterLocationActiveByName(string locName);
        List<MasterLocation> GetAllMasterLocationActiveByParentLocation(string parloc);
        List<MasterLocation> GetAllMasterLocationActiveTRI();
        MasterLocation GetMasterLocationByID(string idLoc);
        MasterLocation GetMasterLocationByParentID(string parentID);
        MasterLocation GetMasterLocationByLocationName(string locationName);
        List<MasterLocation> GetMasterLocationListByListParentLocation(List<string> parentList);

        /* Beetle Trap Definition Repository */
        List<MasterLocation> GetParentLocation();
        List<MasterLocation> GetLocationByWareFactTypeEachParentLocation(
            MasterLocationInput input);
        List<MasterLocation> GetLocationByWareFactType();
        string GetParentLocationEastWest(string idloc);
        string GetEastWestLocation(string idloc);
    }
}
