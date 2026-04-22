using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
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
