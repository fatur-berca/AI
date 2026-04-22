using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.Repositories
{
    public interface IMasterLeadTimeTomRepo
    {
        //List<MasterLeadTimeTOM> GetAllMasterLeadTimeTom();
        List<MasterLeadTime> GetAllMasterLeadTimeTOM(MasterLeadTimeTomInput input);
        MasterLeadTime GetMasterLeadTimeFilterByTOTE(int? idVendor, string sender, string receiver, DateTime ShipmentDate);
    }
}
