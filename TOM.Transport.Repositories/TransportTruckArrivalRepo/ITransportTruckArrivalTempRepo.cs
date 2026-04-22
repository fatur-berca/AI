using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.Repositories
{
    public interface ITransportTruckArrivalTempRepo : IGenericRepository<TruckArrivalTemp>
    {
        void DeleteTruckArrivalTemp();
        void ReseedTruckArrivalTemp();
        void SaveData(TruckArrivalTemp save);
    }
}
