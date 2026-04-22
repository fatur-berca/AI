using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;


namespace TOM.Master.BusinessLogics
{
    public interface IMasterLoadFactorCFPBLL
    {
        List<MasterLoadFactorCFPDTO> GetMasterLoadFactorCFPs(MasterLoadFactorCFPInput input);
        List<MasterListDTO> GetListForCPFs(MasterListInput input);
        List<MasterListDTO> GetListForModel(MasterListInput input);

        MasterLoadFactorCFPDTO SaveData(MasterLoadFactorCFPDTO input);

        string SaveAndUpdateData(MasterLoadFactorCFPDTO input);

        MasterLoadFactorCFPDTO EditData(MasterLoadFactorCFPDTO input);

        MasterLoadFactorCFPDTO GetById(int id);
    }
}
