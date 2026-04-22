using System;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories.TransportVendorChangeLogRepo
{
    public class TransportVendorChangeLogRepo : TOMGenericRepository<TransportVendorChangeLog>, ITransportVendorChangeLogRepo
    {
        public TransportVendorChangeLogRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
        }

        public void InsertData(TransportVendorChangeLog input)
        {
            input.IsActive = true;
            input.CreatedDate = DateTime.Now;
            input.UpdatedDate = DateTime.Now;
            Insert(input);
            Save();
        }
    }
}
