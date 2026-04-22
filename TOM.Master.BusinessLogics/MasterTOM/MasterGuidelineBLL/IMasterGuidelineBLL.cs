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
    public interface IMasterGuidelineBLL
    {
        List<MasterGuidelineDTO> GetMasterGuidelines(MasterGuidelineInput input);
        List<MasterFunctionDTO> GetListForGuidelines(MasterFunctionInput input);

        List<MasterGuidelineDTO> GetListForGuidelinesByDescAndKeyword(MasterGuidelineInput input);

        MasterGuidelineDTO SaveData(MasterGuidelineDTO input);

        MasterGuidelineDTO EditData(MasterGuidelineDTO input);

        MasterGuidelineDTO GetById(int id);
    }
}
