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
   public interface IMasterMappingBLL
    {
       List<MasterMappingDTO> GetMasterMappings(MasterMappingInput input);
       MasterMappingDTO SaveData(MasterMappingDTO input);
       MasterMappingDTO EditData(MasterMappingDTO input);
       MasterMappingDTO GetById(int id);
       MasterMappingDTO Import(MasterMappingDTO input);
    }
}
