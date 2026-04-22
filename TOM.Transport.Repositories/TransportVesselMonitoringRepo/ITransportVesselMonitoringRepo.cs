using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories.TransportVesselMonitoringRepo
{
    public interface ITransportVesselMonitoringRepo: IGenericRepository<TransportVesselMonitoring>
    {
    	TransportVesselMonitoring GetTransportVesselMonitoringByIDTransactionExecution(int idtransexe);
        int UpdateVessel(TransportMonitoringViewInput input, string userid);
        //int UpdateOrder(TransportMonitoringViewInput input, string userid);
        TransportVesselMonitoring GetVesselMonitoringByidTE(int idTE);
        void SaveData(TransportVesselMonitoring input, bool status);
    }
}
