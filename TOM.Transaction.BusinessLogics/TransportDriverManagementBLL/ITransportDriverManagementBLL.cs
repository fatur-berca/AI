using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.Outputs;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using DFIS.Utils;

namespace TOM.Transport.BusinessLogics
{
    public interface ITransportDriverManagementBLL
    {
        TransportDriverManagementDTO GetByIdAndVendor(string id, int vendorID);
        int SetInactive(TransportDriverManagementDTO input, string controller);
        TransportDriverManagementDTO GetById(string id);
        string GetNameById(string id);
        TransportDriverManagementDTO CheckDriver(string idcard, int idvendor);
        List<TransportDriverManagementDTO> GetDataByCriteria(TransportDriverManagementInput criteria);
        TransportDriverManagementDTO GetByIdCardNumberAndIDVendor(string idCardNumber, int idVendor);
        List<TransportDriverManagementDTO> GetTransportDriverManagements(TransportDriverManagementInput input);
        dynamic GetTransportationDriverManagementsDataTable(TransportDriverManagementInput filter, DataTableModel model = null);
        //int UpdateTransportDriverManagement(TransportDriverManagementDTO input, string controller, string userid);
        int UpdateTransportDriverManagement(TransportDriverManagementInput input, string controller, string userid);
        List<TransportDriverManagementDTO> exportToExcel();
        int CountTransportDriverManagements(TransportDriverManagementInput input, string criteria);

        //TransportDriverManagementDTO Save(TransportDriverManagementDTO input, string controller, string userid);
        int Save(TransportDriverManagementDTO input, string controller, string userid);

        List<TransportDriverManagementDTO> getSearchResult(TransportDriverManagerSearchnput input);

        void SetActive(List<string> id, bool status);
        List<MasterVendorTOMDTO> GetMasterVendor(List<UserRole> userRole);
        List<TransportDriverManagementDTO> GetTransportDriverManagements2(TransportDriverManagementInput input);
    }
}
