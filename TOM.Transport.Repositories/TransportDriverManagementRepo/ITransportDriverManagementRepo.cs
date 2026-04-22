using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public interface ITransportDriverManagementRepo
    {
        //int Insert(CustomReportStateDTO input, string controller, string userid);
        int UpdateInactive(TransportDriverManagementDTO input, string controller);
        int Update(TransportDriverManagementDTO input, string controller, string userid);
        List<TransportDriverManagementDTO> GetRecordsForExcel();
        void SetActive(List<string> id, bool status);
        TransportDriverManagement GetTransportDriverManagement(string IDCard);
        List<TransportDriverManagement> GetALLTransportDriverManagementActiveList();
        List<TransportDriverManagement> GetALLTransportDriverManagementList();
        List<TransportDriverManagement> GetDataByCriteriaActive(TransportDriverManagementInput criteria);
    }
}
