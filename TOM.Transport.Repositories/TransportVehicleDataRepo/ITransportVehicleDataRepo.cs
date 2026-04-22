using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public interface ITransportVehicleDataRepo
    {
        int Update(TransportVehicleDataDTO input, string controller, string userid);
        int SetInactiveRecovery(TransportVehicleDataDTO input, string controller, string userid);
        List<TransportVehicleDataDTO> GetRecordsForExcel();
        TransportVehicleData GetTransportVehicleData(string idVD);
        List<TransportVehicleData> GetALLTransportVehicleDataActiveList();
        List<TransportVehicleData> GetALLTransportVehicleDataList();
        List<TransportVehicleData> GetDataByCriteriaActive(TransportVehicleDataInput criteria);
    }
}
