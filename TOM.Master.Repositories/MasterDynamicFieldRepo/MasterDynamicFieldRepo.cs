using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using System;

namespace TOM.Master.Repositories
{
    public class MasterDynamicFieldRepo : TOMGenericRepository<MasterDynamicField>, IMasterDynamicFieldRepo
    {
        public MasterDynamicFieldRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public List<MasterDynamicField> GetMasterDynamicFieldByPageName(string pagename)
        {
            var queryFilter = PredicateHelper.True<MasterDynamicField>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.PageName.Equals(pagename));
            return Get(queryFilter).OrderBy(p => p.Ordering).ToList();
        }

        public List<MasterDynamicField> GetMasterDynamicFieldByPageNameGroup(string pagename, string group)
        {
            var queryFilter = PredicateHelper.True<MasterDynamicField>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.GroupCollapse.Equals(group));
            queryFilter = queryFilter.And(p => p.PageName.Equals(pagename));
            return Get(queryFilter).OrderBy(p => p.Ordering).ToList();
        }

        public List<MasterDynamicField> GetMasterDynamicFieldByPageNameGroupListFieldName(string pagename, string group, List<int> listFieldName)
        {
            var queryFilter = PredicateHelper.True<MasterDynamicField>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.GroupCollapse.Equals(group));
            queryFilter = queryFilter.And(p => p.PageName.Equals(pagename));
            if (listFieldName != null)
            {
                if (listFieldName.Count > 0)
                    queryFilter = queryFilter.And(p => listFieldName.All(p2 => p2 != p.IDDynamicField));
            }
            return Get(queryFilter).OrderBy(p => p.Ordering).ToList();
        }

        public List<MasterDynamicField> GetMasterDynamicFieldByPageNameFieldType(string pagename, string fieldtype)
        {
            var queryFilter = PredicateHelper.True<MasterDynamicField>();
            queryFilter = queryFilter.And(p => p.IsActive);
            queryFilter = queryFilter.And(p => p.PageName.Equals(pagename));
            queryFilter = queryFilter.And(p => p.FieldType.Equals(fieldtype));
            return Get(queryFilter).ToList();
        }
    }
}
