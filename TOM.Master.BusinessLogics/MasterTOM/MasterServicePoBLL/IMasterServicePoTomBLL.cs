using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterServicePoTomBLL: IInsertOrUpdateBLL<MasterServicePoTomDTO>
    {
        List<MasterServicePoTomDTO> GetData(MasterServicePoTomInput criteria);
        //MasterServicePoTomDTO SaveData(MasterServicePoTomDTO input, string controller, string userid);
        int SaveData(MasterServicePoTomDTO input);
        //MasterServicePoTomDTO EditData(MasterServicePoTomDTO input, string controller, string userid);
        int EditData(MasterServicePoTomDTO input);
        bool DelData(int keyID);
    }
}
