using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories
{
    public interface ITransportOrderDetailChangeLogRepo : IGenericRepository<TransportOrderDetailChangeLog>
    {
        TransportOrderDetailChangeLog GetLatestVersionByTransportOrderDetailID(int transOrderDetailID);
    }
}
