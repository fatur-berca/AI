using System.Collections.Generic;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;
using TOM.Transport.Domain.DTOs;
using System.Collections;

namespace TOM.Transport.Repositories.TransportExecutionRepo
{
    public interface ITransportExecutionRepo : IGenericRepository<TransportExecution>
    {
        List<TransportExecution> GetALLTransportExecutionActive();
        TransportExecution GetTransportExecutionByID(int idexe);
        TransportExecution GetTransportExecutionActiveByID(int idexe);
        void RefreshTransportStatus(int idExecution);
        TransportExecution GetTransportExecutionByTN(string TN);
        TransportExecution GetTransportExecutionByIDDriver(string idDriver, int idtranexe);
        List<TransportExecution> GetTransportExecutionFilter(string stono);
        List<TransportExecution> GetTransportationNumberFilterByUserLocations(string stono, string userid);
        List<TransportExecution> GetListTransportExecutionByID(int idTE);
        List<FN_TRANSPORT_EXECUTION_LIST_ADD_Result> GetTransportExecutionAddNew(TransportOrderInput input);
        void CalculateCost(string idTransportExecution, string userid);
        void CalculateCost(string idTransportExecution, string userid, int recalculate);
        void CalculateLoadFactorCFP(int idTransportExecution);
        List<string> GenerateTransportationNumber(List<TransportExecutionDTO> saveList, List<TransportExecutionAddNewDTO> toList, Hashtable HashSeq);
        int SaveData(TransportExecution input);
        TransportExecutionPrintMemoDTO GetTransportExecutionByidTE(int idTE);
        List<TransportExecutionVendorDTO> GetTotalVendorTE(List<int?> idTE);

        IEnumerable<FnTransportationSummaryCRate_Result> GetCrashRate(string year);
        List<string> GetDistinctUserByRegional(List<string> regional);
        IEnumerable<object> GetTNCreatorShipping(string tncreator);
    }
}
