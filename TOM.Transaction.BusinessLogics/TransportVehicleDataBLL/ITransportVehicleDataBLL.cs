using System.Collections.Generic;
using DFIS.Universal.Domain.Outputs;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using DFIS.Utils;

namespace TOM.Transport.BusinessLogics
{
    public interface ITransportVehicleDataBLL
    {
        TransportVehicleDataDTO GetById(string id);
        List<TransportVehicleDataDTO> GetDataByCriteria(TransportVehicleDataInput criteria);
        List<TransportVehicleDataDTO> GetTransportVehicleDatas(TransportVehicleDataInput input);
        List<TransportVehicleDataDTO> GetTransportVehicleDatas2(TransportVehicleDataInput input);
        dynamic GetVehicleDataTable(TransportVehicleDataInput filter, DataTableModel model = null);
        int UpdateTransportVehicleData(TransportVehicleDataDTO input, string controller, string userid);
        int SetInactiveRecovery(TransportVehicleDataDTO input, string controller, string userid);
        List<TransportVehicleDataDTO> exportToExcel();
        int CountTransportVehicleDatas(TransportVehicleDataInput input, string category);
        List<MasterVendorTOMDTO> GetMasterVendor(List<UserRole> userRole);
    }
}
