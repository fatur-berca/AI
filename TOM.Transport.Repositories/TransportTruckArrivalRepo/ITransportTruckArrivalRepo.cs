using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories
{
    public interface ITransportTruckArrivalRepo : IGenericRepository<TransportTruckArrival>
    {
        List<TransportTruckArrivalDTO> GetAllTransportTruckArrival(TransportTruckArrivalInput filter);
        void UpdateCalculate(TransportTruckArrival input, string userid);
    }
}
