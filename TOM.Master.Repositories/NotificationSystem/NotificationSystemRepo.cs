using System;
using System.Collections.Generic;
using System.Linq;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using DFIS.Utils;
using DFIS.Universal.Repositories;

namespace TOM.Master.Repositories
{
    public interface INotificationSystemRepo : INotificationSystemRepo<NotificationSystem> { }

    public class NotificationSystemRepo : TOMGenericRepository<NotificationSystem>, INotificationSystemRepo
    {
        private TOMContextDB _context;

        public NotificationSystemRepo(TOMContextDB contextEntities) : base(contextEntities)
        {
            _context = contextEntities;
        }

        public NotificationSystem GetNotificationSystemByID(int idnotif)
        {
            var queryFilter = PredicateHelper.True<NotificationSystem>();
            queryFilter = queryFilter.And(p => p.IDNotification == idnotif);
            return Get(queryFilter).First();
        }

        public List<NotificationSystem> GetALLNotificationByIDUser(string iduser)
        {
            var queryFilter = PredicateHelper.True<NotificationSystem>();
            queryFilter = queryFilter.And(p => p.IDUser == iduser);
            return Get(queryFilter).OrderByDescending(x => x.CreatedDate).ToList();
        }

        public List<NotificationSystem> GetNotificationSystemsByIdUserIsOpenFalse(List<int> idnotif, string iduser)
        {
            var queryFilter = PredicateHelper.True<NotificationSystem>();
            queryFilter = queryFilter.And(p => p.IsOpen == false);
            queryFilter = queryFilter.And(p => p.IDUser == iduser);
            if (idnotif != null) 
                queryFilter = queryFilter.And(p => !idnotif.Contains(p.IDNotification));
            return Get(queryFilter).OrderByDescending(x => x.CreatedDate).ToList();
        }

        public NotificationSystem SaveData(NotificationSystem input, bool status)
        { 
            if (status)
            {
                var result = from hrd in _context.HRD_EMP_V2
                    join mu in _context.MasterUsers
                    on hrd.FULL_NAME equals mu.IDUser
                    where hrd.FULL_NAME == input.IDUser
                    select hrd;
                if (result != null)
                {
                    input.IsRead = false;
                    input.IsOpen = false;
                    input.CreatedDate = DateTime.Now;
                    input.UpdatedDate = DateTime.Now;
                    Insert(input);
                    Save();
                }
            }
            else
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
                Save();
            }   
            return input;
        }

        public void SaveData(NotificationSystem input)
        {
            Insert(input);
            Save();
        }
    }
}
