using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Master.Domain.DTOs;
using DFIS.Master.Domain.Inputs;

namespace DFIS.Master.BusinessLogics.UtilitiesBLL
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
