using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;

namespace TOM.Transport.BusinessLogics.TransportLostClaimDamageBLL
{
    public interface ITransportLostClaimDamageBLL
    {
        TransportLostClaimDamageDTO SaveInitiation(TransportLostClaimDamageDTO newData, string userID);
        void SaveInitiationAttachment(string pathFile, string SJNumber, string GPNumber);
        void SaveVerification(TransportLostClaimDamageDTO newData, string userID);
        void RevokeVerification(TransportLostClaimDamageDTO revokeData, string userID);

        IEnumerable<string> GetDropDownOriginWarehouse();
        IEnumerable<string> GetDropDownDestWarehouse();
        List<string> GetTransportationNumberFilter(string stono);
        // IEnumerable<string> GetDropDownSTONumber();
        IEnumerable<string> GetDropDownDeliveryNoteNumber();
        IEnumerable<string> GetDropDownPoliceRegNumber();
        IEnumerable<string> GetDropDownVendor();
        IEnumerable<string> GetDropDownClaimType();
        IEnumerable<string> GetDropDownClaimExpense();
        IEnumerable<string> GetDropDownClaimCategory();
        IEnumerable<string> GetDropDownSJNumber();
        //IEnumerable<string> GetDropDownGPNumber();

        TransportLostClaimDamageDTO GetAddNewDetail(string SJNumber);

        IEnumerable<TransportLostClaimDamageDTO> GetListLostClaimDamage(TransportLostClaimDamageInput input);
        TransportLostClaimDamageDTO GetTransportLostClaimDamageByKey(string GPNumber, string SJNumber);
        TransportLostClaimDamageDTO EditInitiation(TransportLostClaimDamageDTO newData, string userID);
        void SaveFile(string pathFile, string SJNumber, string GPNumber, string status, string userID);
        void SaveProcess(TransportLostClaimDamageDTO newData, string userID);
        void SaveInvoicing(TransportLostClaimDamageDTO newData, string userID);
        void SaveDispose(TransportLostClaimDamageDTO newData, string userID);
        void RevokeProcess(TransportLostClaimDamageDTO revokeData, string userID);
        void RevokeInvoicing(TransportLostClaimDamageDTO revokeData, string userID);
        void RevokeDispose(TransportLostClaimDamageDTO revokeData, string userID);
        void SetActive(List<string> id, bool status);

        void RemoveAttachment(string GPNumber, string SJNumber);
        void RemoveIncinerationReportFile(string GPNumber, string SJNumber);
        void RemovePaymentProof(string GPNumber, string SJNumber);
        void RemoveMemoFile(string GPNumber, string SJNumber);

        string GetSpeakingCode(string FACode);      
        IEnumerable<TransportLostClaimDamageDTO> GetListDefault();
        void SaveNotification(TransportLostClaimDamageDTO Dto, List<string> listuser, string username);
        string checkDataUpload(TransportLostClaimDamageDTO input, string sheetName);
        TransportLostClaimDamageDTO UpdateDataTransport(TransportLostClaimDamageDTO input, string sheetName);
        void UpdateCloneFile(string path);
        List<TransportLostClaimFACode> GetTransporftLostClaimFACodeBySTO(string transno, string stono);
    }
}
