using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace DFIS.Universal.BusinessLogics
{
    public interface ICustomReportStateBLL
    {
        CustomReportStateDTO SelectLayout(string userid, string pageName, string layoutName);
        bool SaveLayouts(string userid, string pageName, params CustomReportStateDTO[] states);
        List<CustomReportStateDTO> GetSavedLayouts(string userid, string pageName, string layoutName = null);
        int InsertCustomReportState(CustomReportStateDTO input, string controller, string userid);
        int SaveSelectedLayout(CustomReportStateDTO input, string controller, string userid);
        int DeleteDataBase(string pagename, string layoutname, string controller, string userid);
        List<CustomReportStateDTO> GetLayouts(CustomReportStateInput input, string userid);
        List<CustomReportStateDTO> GetLayoutsJoin(CustomReportStateInput input, string userid);
        List<CustomReportStateDTO> GetSelectedLayout(CustomReportStateInput input, string userid);
        List<CustomReportStateDTO> GetSavedSelectedLayout(CustomReportStateInput input, string userid);
    }
}
