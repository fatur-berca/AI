using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.BusinessLogics
{
    public interface IImporterBLL<DTOType>
    {
        void Import(IEnumerable<DTOType> newData);

        void BeginImport();
        void ImportRow(DTOType data);
        void FinalizeImport();
        void CancelImport();
    }

    public interface IInsertOrUpdateBLL<DTOType>
    {
        void InsertOrUpdate(DTOType value, bool canUpdate = true);
    }
}
