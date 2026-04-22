using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.Repositories.TransportTicketNCRRepo
{
    public interface ITransportTicketNCRRepo : ITOMGenericRepository<TransportTicketNCR>
    {
        List<TransportTicketNCR> GetTransportTicketNCRActive(TransportTicketNCRInput criteria);
        List<string> GetFieldTransportTicketNCR();
        void SetActiveTransportTicketNCR(List<string> ticketNumber);
        TransportTicketNCR SaveData(TransportTicketNCR input, bool status);
        void SetActive(List<string> id, bool status);
    }
}
