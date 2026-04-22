using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DFIS.Utils;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories.TransportTicketNCRRepo
{
    public class TransportTicketNCRRepo : TOMGenericRepository<TransportTicketNCR>, ITransportTicketNCRRepo
    {
        public TransportTicketNCRRepo(TOMContextDB contextEntities)
            : base(contextEntities)
        {
        }

        public List<TransportTicketNCR> GetTransportTicketNCRActive(TransportTicketNCRInput criteria)
        {
            var queryFilter = PredicateHelper.True<TransportTicketNCR>();
            //if (criteria.filterDate)
            //    queryFilter = queryFilter.And(p => p.Date >= criteria.DateF).And(p => p.Date <= criteria.DateT);
            queryFilter = queryFilter.And(p => p.IsActive);
            return Get(queryFilter).ToList();
        }

        public List<string> GetFieldTransportTicketNCR()
        {
            List<string> temp = new List<string>();
            var properties = typeof(TransportTicketNCR).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in properties)
            {
                temp.Add(item.Name);
            }
            return temp;
        }

        public void SetActiveTransportTicketNCR(List<string> ticketNumber)
        {
            var queryFilter = PredicateHelper.True<TransportTicketNCR>();
            queryFilter = queryFilter.And(p => ticketNumber.Contains(p.TicketNumber));
            List<TransportTicketNCR> transTicket = Get(queryFilter).ToList();
            foreach (TransportTicketNCR temp in transTicket)
            {
                temp.IsActive = !temp.IsActive;
                Update(temp);
            }
            Save();
        }

        public TransportTicketNCR SaveData(TransportTicketNCR input, bool status)
        {
            if (status)
            {
                input.CreatedDate = DateTime.Now;
                input.UpdatedDate = DateTime.Now;
                Insert(input);
            }
            else
            {
                input.UpdatedDate = DateTime.Now;
                Update(input);
            }
            Save();
            return input;
        }

        public void SetActive(List<string> id, bool status)
        {
            var queryFilter = PredicateHelper.True<TransportTicketNCR>();
            queryFilter = queryFilter.And(p => id.Contains(p.TicketNumber));
            List<TransportTicketNCR> dbResult = Get(queryFilter).ToList();
            foreach (TransportTicketNCR data in dbResult)
            {
                data.IsActive = status;
                Update(data);
            }
            Save();
        }
    }
}
