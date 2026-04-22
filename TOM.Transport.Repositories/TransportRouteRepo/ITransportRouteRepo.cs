using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories.TransportRouteRepo
{
    public interface ITransportRouteRepo : IGenericRepository<TransportRoute>
    {
        List<TransportRoute> GetTransportRouteByIDTransactionExecution(int idtransexe);
        void SaveData(TransportRoute input);
    }
}
