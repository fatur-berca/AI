using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using DFIS.Universal.Domain.DTOs;
using TOM.Transport.Domain.DTOs;
using TOM.Transport.Domain.Inputs;

namespace TOM.Transport.BusinessLogics
{
    public interface ITransportUnitFrequentBLL
    {
        List<TransportUnitFrequentDTO> GetViewUnitFrequent(TransportUnitFrequentInput input);
        List<MasterListDTO> GetMasterLists();

    }
}
