using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace hms_tom_dev.Code
{
    public interface ICustomPrincipal : IPrincipal
    {
        string Username { get; set; }
    }
}
