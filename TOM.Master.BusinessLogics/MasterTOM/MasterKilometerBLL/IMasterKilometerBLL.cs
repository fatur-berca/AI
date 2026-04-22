using System.Collections.Generic;
using System.Web;
using TOM.Master.Domain.DTOs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterKilometerBLL
    {
        List<MasterKilometerDTO> GetAllMasterKilometer();
        List<string> SaveUpload(HttpPostedFileBase input, string userid);
    }
}
