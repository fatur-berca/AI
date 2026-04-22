using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace DFIS.Universal.Repositories
{
    public interface ICustomReportStateRepo
    {
        int Insert(CustomReportStateDTO input, string controller, string userid);
        int DeleteDataBase(string pagename, string layoutname, string controller, string userid);
        int SaveSelectedLayout(CustomReportStateDTO input, string controller, string userid);
    }
}
