using System.Collections.Generic;
using DFIS.Contracts;
using DFIS.Universal.Domain.DTOs;

namespace DFIS.Universal.Repositories
{
    public interface IMasterApprovalNewsRepo<T> : IGenericRepository<T> where T : class
    {
        List<MasterApprovalNewsDTO> GetAllNewsHighlights();
        List<MasterApprovalNewsDTO> GetAllNewsHighlightsByStatus(int status);
        MasterApprovalNewsDTO SaveData(MasterApprovalNewsDTO input, bool status);
        int UpdateSeen(MasterApprovalNewsDTO input, string controller, string userid);
    }
}
