using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System.Collections.Generic;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterFunctionBLL
    {
        List<MasterList> GetTypeList();
        List<MasterFunctionDTO> GetMasterFunctionTypeMenu(string type);
        List<MasterFunctionDTO> GetMasterFunctions(MasterFunctionInput input);
        List<MasterFunctionDTO> GetMasterFunctionForDynamics(MasterFunctionInput input);
        List<MasterFunctionDTO> GetMasterFunctionTypes(MasterFunctionInput input);


        List<MasterFunction> GetAllMasterFunction();
        MasterFunction SaveData(MasterFunction input, bool status);
    }
}
