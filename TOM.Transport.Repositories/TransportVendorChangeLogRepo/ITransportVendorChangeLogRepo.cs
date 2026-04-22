using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories.TransportVendorChangeLogRepo
{
    public interface ITransportVendorChangeLogRepo : IGenericRepository<TransportVendorChangeLog>
    {
        void InsertData(TransportVendorChangeLog input);
    }
}
