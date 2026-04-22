using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Master.Domain.DTOs;
using DFIS.Master.Domain.Inputs;

namespace DFIS.Master.BusinessLogics
{
    public interface IMasterFABrandBLL
    {
        List<MasterFABrandDTO> GetAllMasterFABrands();
        List<MasterFABrandDTO> GetMasterFABrands(MasterFABrandInput input);

        MasterFABrandDTO SaveData(MasterFABrandDTO input);

        MasterFABrandDTO EditData(MasterFABrandDTO input);

        MasterFABrandDTO GetById(string id);
       
    }
}
