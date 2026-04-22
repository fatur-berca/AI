using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace DFIS.Universal.BusinessLogics
{
    public interface IMasterUserBLL<TMasterUser>
    {
        TMasterUser GetLogin(string iduser);
        List<MasterUserDTO> GetMasterUsers(MasterUserInput input);
        MasterUserDTO EditData(MasterUserDTO input, string controller, string userid);
        MasterUserDTO SaveData(MasterUserDTO input, string controller, string userid);
        List<MasterUserDTO> GetMasterUserViews(MasterUserInput input);
        string GetFullName(string iduser);
    }
}
