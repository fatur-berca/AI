using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterUomBLL
    {
        List<MasterUomDTO> GetMasterUom(MasterUomInput criteria);
        MasterUomDTO GetById(int id);
        MasterUomDTO SaveData(MasterUomDTO input, string controller, string userid);
        MasterUomDTO EditData(MasterUomDTO input, string controller, string userid);
    }
}
