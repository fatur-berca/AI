using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterListRepo : IGenericRepository<MasterList>
    {
        List<MasterList> GetMasterListByFieldName(string fieldName);
        List<MasterList> GetMasterListByListFieldName(List<string> listFieldName);
        MasterList GetMasterListCustom(string fieldname, string value);
        List<MasterList> GetMasterListByFieldNameListFieldValue(string fieldName, List<string> fieldValue);
        List<string> GetMasterListFieldNameValue(string fieldname);
        bool IsMasterListValid(string fieldName, string fieldValue);
    }
}
