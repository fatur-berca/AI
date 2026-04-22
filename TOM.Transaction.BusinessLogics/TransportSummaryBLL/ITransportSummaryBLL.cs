using DFIS.Universal.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.Outputs;
using TOM.Master.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.BusinessLogics.TransportSummaryBLL
{
    public interface ITransportSummaryBLL
    {
        List<TransportRouteDTO> GetTransportRoute(string stono);

        List<TransportExecutionDTO> GetTransportList();
        List<TransportOrderDTO> GetSTONo();
        List<MasterListDTO> GetMstList();
        List<MasterVendorDTO> GetVendorName();
        List<MasterLocationDTO> GetLocationName();
        List<MasterListDTO> GetListTabName();

        List<TransportOrderDTO> GetSTONoFilter(string stono);
        List<TransportExecutionDTO> GetTransportationNumberFilter(string transno);
        List<TransportExecutionDTOMin> GetListView(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro, List<UserRole> userRole);

        List<TransportExecutionDTO> GetExportXlsDetailClass(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro, List<UserRole> userRole);
        List<TransportExecutionDTO> GetExportXlsCompactValidation(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro, List<UserRole> userRole);
        List<TransportExecutionDTO> GetCustomXls(string fltrdf, string fltrdt, string fltrtn, string fltron, string fltrtc, string fltrtm, string fltrvn, string fltrvt, string fltroc, string fltrzo, string fltrsl, string fltrse, string fltrre, string fltrth, string fltrro, List<UserRole> userRole);
        List<TransportExecutionDTO> GetImportValidateXls(string fltrtn, string fltron);
        List<FnTransportationSummaryDTO> GetSummaryVendor(string filtertab, string filterrole, string filtersort, string filteryear);
        List<KPILoadFactorAvgDTO> GetRecordsAvg(int year);
        void CalculateCost(List<string> idTransportExecution, string userId);
        IEnumerable<TransportSummaryReclassViewDTO> GetReclass(int year);
        IEnumerable<FnTransportationSummaryCRateDTO> GetCrashRate(string year);

        FnTransportationSummaryDTO GenerateChart(string filtertab, string filterrole, string filtersort, string filteryear);

        FnTransportationSummaryDTO GenerateChartDecimal(string filtertab, string filterrole, string filtersort,
            string filteryear);
    }
}
