using System.Collections.Generic;
using DFIS.Universal.Domain.DTOs;

namespace DFIS.Universal.BusinessLogics
{
    public interface IMasterApprovalNewsBLL
    {
        List<MasterApprovalNewsDTO> GetAllMasterApprovalNewsByStatus(int status);
        void SaveData(int id ,int status, string userid);
        int UpdateSeen(MasterApprovalNewsDTO input, string controller, string userid);
    }
}
