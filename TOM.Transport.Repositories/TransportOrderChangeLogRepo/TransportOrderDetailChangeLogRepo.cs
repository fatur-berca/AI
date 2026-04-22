using DFIS.Utils;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories
{
    public class TransportOrderDetailChangeLogRepo : TOMGenericRepository<TransportOrderDetailChangeLog>, ITransportOrderDetailChangeLogRepo
    {
        public TransportOrderDetailChangeLogRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public TransportOrderDetailChangeLog GetLatestVersionByTransportOrderDetailID(int transOrderDetailID)
        {
            var queryFilter = PredicateHelper.True<TransportOrderDetailChangeLog>();
            queryFilter = queryFilter.And(x => x.IDTransportOrderDetail == transOrderDetailID);
            return Get(queryFilter).OrderByDescending(x => x.Version).FirstOrDefault();
        }
    }
}
