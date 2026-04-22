using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;

namespace TOM.Transport.Repositories
{
    public class TransportOrderChangeLogRepo : TOMGenericRepository<TransportOrderChangeLog>, ITransportOrderChangeLogRepo
    {
        public TransportOrderChangeLogRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public TransportOrderChangeLog GetLatestVersionByTransportOrderID(int transOrderID)
        {
            var queryFilter = PredicateHelper.True<TransportOrderChangeLog>();
            queryFilter = queryFilter.And(x => x.IDTransportOrder == transOrderID);
            return Get(queryFilter).OrderByDescending(x => x.Version).FirstOrDefault();
        }
    }
}
