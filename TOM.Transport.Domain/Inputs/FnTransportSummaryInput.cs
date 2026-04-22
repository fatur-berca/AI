using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.Inputs;

namespace TOM.Transport.Domain.Inputs
{
    public class FnTransportSummaryInput : BaseInput
    {
        string FilterTab { get; set; }
        string FilterSort { get; set; }
        string FilterRole { get; set; }
        string FilterYear { get; set; }
    }
}
