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
    public interface IMasterDynamicFieldBLL
    {
        List<MasterDynamicFieldDTO> GetMasterDynamicFields(MasterDynamicFieldInput input);

        MasterDynamicFieldDTO SaveData(MasterDynamicFieldDTO input);

        MasterDynamicFieldDTO EditData(MasterDynamicFieldDTO input);

        MasterDynamicFieldDTO GetById(int id);
    }
}
