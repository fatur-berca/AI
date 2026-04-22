using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterUserRoleDelegationBLL:IInsertOrUpdateBLL<MasterUserRoleDelegationDTO>
    {
        List<MasterUserRoleDelegationViewDTO> GetData(MasterUserRoleDelegationViewInput criteria);
        int Save(MasterUserRoleDelegationDTO Input);
        List<UserLocationDelegationDTO> GetDataByIdParent(int id);
    }
}
