using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterDynamicFieldRepo : IGenericRepository<MasterDynamicField>
    {
        List<MasterDynamicField> GetMasterDynamicFieldByPageName(string pagename);
        List<MasterDynamicField> GetMasterDynamicFieldByPageNameGroup(string pagename, string group);

        List<MasterDynamicField> GetMasterDynamicFieldByPageNameGroupListFieldName(string pagename, string group, List<int> listFieldName);
        List<MasterDynamicField> GetMasterDynamicFieldByPageNameFieldType(string pagename, string fieldtype);
    }
}
