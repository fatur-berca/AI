using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using TOM.Master.Domain.Inputs;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterVendorBLL
    {
        List<MasterVendorTOMDTO> GetMasterVendors(MasterVendorTOMInput input);

        MasterVendorTOMDTO SaveData(MasterVendorTOMDTO input, string controller, string userid);

        MasterVendorTOMDTO EditData(MasterVendorTOMDTO input, string controller, string userid);

        MasterVendorTOMDTO GetById(int id);
        MasterVendorTOMDTO GetVendorByName(string name, bool mustBeAParent = true);
    }
}
