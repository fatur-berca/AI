using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories
{
    public interface ITransportOrderChangeLogRepo : IGenericRepository<TransportOrderChangeLog>
    {
        TransportOrderChangeLog GetLatestVersionByTransportOrderID(int transOrderID);
    }
}
