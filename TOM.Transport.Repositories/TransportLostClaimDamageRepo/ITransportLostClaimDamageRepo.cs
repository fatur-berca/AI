using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Transport.Domain.DTOs;

namespace TOM.Transport.Repositories.TransportLostClaimDamageRepo
{
    public interface ITransportLostClaimDamageRepo
    {
        IEnumerable<string> GetListOriginWarehouse();
        IEnumerable<string> GetListDestinationWarehouse();
        List<string> GetListSTONumber(string transno);
        // IEnumerable<string> GetListSTONumber();
        IEnumerable<string> GetListDeliveryNoteNumber();
        IEnumerable<string> GetListPoliceRegNumber();
        IEnumerable<string> GetListVendor();
        IEnumerable<string> GetListClaimType();
        IEnumerable<string> GetListClaimExpense();
        IEnumerable<string> GetListClaimCategory();
        IEnumerable<string> GetListSJNumber();
        //IEnumerable<string> GetListGPNumber();

        TransportLostClaimDamageDTO GetNewDataDetail(string SJNumber);

        IEnumerable<TransportLostClaimDamage> GetListTransportLostClaimDamage(TransportLostClaimDamageInput input);
        TransportLostClaimDamage GetTransportLostClaimDamage(string GPNumber, string SJNumber);
        IEnumerable<TransportLostClaimFACode> GetListTransportLostClaimFACode(string GPNumber, string SJNumber);
        IEnumerable<TransportLostClaimUploadBox> GetListTransportLostClaimUploadBoxes(string GPNumber, string SJNumber);

        IEnumerable<TransportLostClaimDamage> GetListDefault();

        void SetActive(List<string> id, bool status);
    }
}
