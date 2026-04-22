using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Master.Repositories
{
    public interface IMasterUomRepo : IGenericRepository<MasterUom>
    {
        MasterUom GetMasterUomByUom(string Uom);
        List<MasterUom> GetMasterUomByMaterial(string MaterialType);
    }
}
