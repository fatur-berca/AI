using System.Collections.Generic;
using System.Linq;
using DFIS.Utils;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories.TransportRouteRepo
{
    public class TransportRouteRepo : TOMGenericRepository<TransportRoute>, ITransportRouteRepo
    {
        public TransportRouteRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public List<TransportRoute> GetTransportRouteByIDTransactionExecution(int idtransexe)
        {
            var queryFilter = PredicateHelper.True<TransportRoute>();
            queryFilter = queryFilter.And(x => x.IDTransportExecution == idtransexe);
            return Get(queryFilter).ToList();
        }

        public void SaveData(TransportRoute input)
        {
            Update(input);
            Save();
        }
    }
}
