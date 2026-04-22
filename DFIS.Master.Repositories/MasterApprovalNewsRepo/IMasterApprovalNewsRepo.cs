using System.Collections.Generic;
using DFIS.Contracts;
using DFIS.EntitiesDAL.EDMX;
using DFIS.Master.Domain.DTOs;
using DFIS.Master.Domain.DTOs;

namespace DFIS.Master.Repositories.MasterApprovalNewsRepo
{
    public interface IMasterApprovalNewsRepo : IGenericRepository<NewsHighlight>
    {
        List<NewsHighlight> GetAllNewsHighlights();
        List<NewsHighlight> GetAllNewsHighlightsByStatus(int status);
        NewsHighlight SaveData(NewsHighlight input, bool status);
        int UpdateSeen(MasterApprovalNewsDTO input, string controller, string userid);
    }
}
