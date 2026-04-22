using System.Collections.Generic;
using System.Linq;

using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.Inputs;
using DFIS.Utils;
using DFIS.Universal.Domain.Inputs;
using System;

namespace TOM.Master.Repositories
{
    public class MasterLocationRepo : TOMGenericRepository<MasterLocation>, IMasterLocationRepo
    {
        public MasterLocationRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public List<MasterLocation> GetAllMasterLocation()
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.IsAssigned == true);
            return Get(queryFilter).ToList();
        }

        public List<MasterLocation> GetAllMasterLocationForDisplay()
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            return Get(queryFilter).ToList();
        }

        public List<MasterLocation> GetAllMasterLocationActive()
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.Type == "Warehouse");
            return Get(queryFilter).ToList();
        }

        public List<MasterLocation> GetAllMasterLocationActiveByType(string type)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.Type == type).And(p => p.IsAssigned == true);
            return Get(queryFilter).ToList();
        }

        public MasterLocation GetMasterLocationActiveByName(string locName)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.LocationName == locName);
            return Get(queryFilter).FirstOrDefault();
        }
        public List<MasterLocation> GetAllMasterLocationActiveByParentLocation(string parloc)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.ParentLocation == parloc);
            return Get(queryFilter).ToList();
        }

        public List<MasterLocation> GetAllMasterLocationActiveTRI()
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.Type == "Region").Or(p => p.Type == "RegionTRI");
            return Get(queryFilter).ToList();
        }

        public MasterLocation GetMasterLocationByID(string idLoc)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.IDLocation == idLoc);
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterLocation GetMasterLocationByParentID(string parentID)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.ParentLocation == parentID);
            return Get(queryFilter).FirstOrDefault();
        }

        public MasterLocation GetMasterLocationByLocationName(string locationName)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.LocationName == locationName);
            return Get(queryFilter).FirstOrDefault();
        }

        public List<MasterLocation> GetMasterLocationListByListParentLocation(List<string> parentList)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => parentList.Contains(p.ParentLocation));
            return Get(queryFilter).ToList();
        }

        /* Beetle Trap Definition Repository */
        public List<MasterLocation> GetParentLocation()
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();

            string[] allowedType = new string[] { "Warehouse", "Factory/SKT/MPS" };
            queryFilter = queryFilter.And(m => allowedType.Contains(m.Type));

            return Get(queryFilter).GroupBy(m => m.ParentLocation).
                Select(m => m.First()).ToList();
        }

        public List<MasterLocation> GetLocationByWareFactTypeEachParentLocation(
            MasterLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();

            string[] allowedType = new string[] { "Warehouse", "Factory/SKT/MPS" };
            queryFilter = queryFilter.And(m => allowedType.Contains(m.Type));

            queryFilter = queryFilter.And(p => (p.ParentLocation == input.ParentLocation));
            queryFilter = queryFilter.And(p => p.IsActive == true);

            return Get(queryFilter).ToList();
        }

        public List<MasterLocation> GetLocationByWareFactType()
        {
            var queryFilter = PredicateHelper.True<MasterLocation>();

            string[] allowedType = new string[] { "Warehouse", "Factory/SKT/MPS" };
            queryFilter = queryFilter.And(m => allowedType.Contains(m.Type));

            queryFilter = queryFilter.And(p => p.IsActive == true);

            return Get(queryFilter).ToList();
        }
        public string GetParentLocationEastWest(string idloc)
        {
            MasterLocation sender = GetMasterLocationByID(idloc);
            MasterLocation zoneBased = GetMasterLocationByID(sender.ParentLocation);
            return zoneBased.ParentLocation;            
        }
        public string GetEastWestLocation(string idloc)
        {
            string zone = null;
            MasterLocation loc1 = GetMasterLocationByID(idloc);
            if (loc1 != null)
            {
                if (loc1.ParentLocation != "East" && loc1.ParentLocation != "West")
                {
                    MasterLocation loc2 = GetMasterLocationByID(loc1.ParentLocation);
                    zone = loc2.ParentLocation;
                }
                else
                    zone = loc1.ParentLocation;
            }
            return zone;
        }
    }
}
