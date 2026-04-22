using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using DFIS.EntitiesDAL;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Master.Domain.Inputs;
using DFIS.Utils;

namespace DFIS.Master.Repositories
{
    public class MasterLocationRepo : GenericRepository<MasterLocation>, IMasterLocationRepo
    {
        public MasterLocationRepo(DFISContextDB contextEntities) : base(contextEntities)
        {
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
            queryFilter = queryFilter.And(p => p.IsActive).And(p => p.Type == type);
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

        public List<MasterLocation> GetAllMasterLocationFromSource(string source)
        {
            DFISContextDB context = new DFISContextDB();
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@DBMappingName",  Value =source , Direction = System.Data.ParameterDirection.Input}
            };

            return context.Database.SqlQuery<MasterLocation>("GetMasterLocation @DBMappingName", sqlParams).ToList();
        }

        public bool SetMasterLocationMapping(string source, string idLoc, bool value)
        {
            DFISContextDB context = new DFISContextDB();
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@DBMappingName",  Value =source , Direction = System.Data.ParameterDirection.Input},
                new SqlParameter { ParameterName = "@IDLocation",  Value =idLoc , Direction = System.Data.ParameterDirection.Input},
                new SqlParameter { ParameterName = "@Status",  Value =value , Direction = System.Data.ParameterDirection.Input}
            };
            try
            {
                context.Database.ExecuteSqlCommand("SetMasterLocation @DBMappingName, @IDLocation, @Status", sqlParams);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }            
        }
    }
}
