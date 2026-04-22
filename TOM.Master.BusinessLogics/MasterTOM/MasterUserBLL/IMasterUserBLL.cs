using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics.UtilitiesBLL
{
    public interface IMasterUserBLL
    {
        MasterUser GetLogin(string iduser);
        List<MasterUserDTO> GetMasterUsers(MasterUserInput input);
        MasterUserDTO EditData(MasterUserDTO input, string controller, string userid);
        MasterUserDTO SaveData(MasterUserDTO input, string controller, string userid);
        List<MasterUserDTO> GetMasterUserViews(MasterUserInput input);
    }
}
