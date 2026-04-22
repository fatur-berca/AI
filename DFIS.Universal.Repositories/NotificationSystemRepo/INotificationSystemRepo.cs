using System.Collections.Generic;
using DFIS.Contracts;

namespace DFIS.Universal.Repositories
{
    public interface INotificationSystemRepo<TNotificationSystem> : IGenericRepository<TNotificationSystem> where TNotificationSystem:class
    {
        TNotificationSystem GetNotificationSystemByID(int idnotif);
        List<TNotificationSystem> GetALLNotificationByIDUser(string iduser);
        List<TNotificationSystem> GetNotificationSystemsByIdUserIsOpenFalse(List<int> idnotif, string iduser);
        TNotificationSystem SaveData(TNotificationSystem input, bool status);
        void SaveData(TNotificationSystem input);
    }
}
