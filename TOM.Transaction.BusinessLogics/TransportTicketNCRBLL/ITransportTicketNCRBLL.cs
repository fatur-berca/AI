using System.Collections.Generic;
using DFIS.Universal.Domain.DTOs;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using DFIS.Utils;

namespace TOM.Transport.BusinessLogics.TransportTicketNCRBLL
{
    public interface ITransportTicketNCRBLL
    {
        List<MasterList> GetMasterListByFieldName(string fieldName);
        List<MasterLocationDTO> GetMasterLocation();
        List<MasterVendorTOMDTO> GetMasterVendor();
        List<MasterListDTO> GetTicketCategory();
        string getTicketnumbers();
        TransportTicketNCRDTO SaveData(TransportTicketNCRDTO input);
        TransportTicketNCRDTO EditData(TransportTicketNCRDTO input);
        List<MasterListDTO> Getvehicletypes();
        List<MasterListDTO> GetLocationTypes();
        List<MasterListDTO> GetLocationCategorys();
        List<MasterListDTO> GetRoadConditions();
        List<MasterListDTO> GetWeatherConditions();
        List<MasterListDTO> GetAccidentCategorys();
        TransportTicketNCRDTO EditDataCirf(TransportTicketNCRDTO input);
        List<TransportTicketNCRDTO> GetTicketNCRs(TransportTicketNCRInput input);
        dynamic GetTicketNCRsTable(TransportTicketNCRInput filter, DataTableModel model = null);
        string GetSumTickets(TransportTicketNCRInput input);
        string GetCountNRCNumbers(TransportTicketNCRInput input);
        string GetTotalAncidents(TransportTicketNCRInput input);
        string GetSumAccidents(TransportTicketNCRInput input);
        TransportTicketNCRDTO SetInactives(TransportTicketNCRDTO input);
        List<MasterListDTO> GetOffenderRoles();
        List<TransportTicketNCRDTO> GetDataByIDs(string id);
        List<TransportTicketNCRDTO> GetTicketNCRPDFs(string id);
        List<MasterListDTO> GetAnalystProblems();
        List<MasterListDTO> GetMainProblems();
        string getNonTicketnumbers();
        void SetActive(List<string> id, bool status);
        List<MasterListDTO> GetOffenderYOS();
    }
}
