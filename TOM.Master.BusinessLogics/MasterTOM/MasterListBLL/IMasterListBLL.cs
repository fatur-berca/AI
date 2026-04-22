using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterListBLL
    {
        List<MasterListDTO> GetMasterLists(MasterListInput input);
        List<MasterListDTO> GetMasterListDrops(MasterListInput input);
        List<MasterListDTO> GetMasterListValues(MasterListInput input);

        List<MasterListDTO> GetListForBrandCategorys(MasterListInput input);

        List<MasterListDTO> GetListForEmployees(MasterListInput input);

        List<MasterListDTO> GetListForFABrands(MasterListInput input);
        List<MasterListDTO> GetListForLocations(MasterListInput input);

        List<MasterListDTO> GetInputSvListView(MasterListInput input);

       // MasterListDTO SaveData(MasterListDTO input);

        MasterListDTO SaveData(MasterListDTO input, string controller, string userid);

      //  MasterListDTO EditData(MasterListDTO input);

        MasterListDTO EditData(MasterListDTO input, string controller, string userid);

        MasterListDTO GetById(int id);
        List<MasterListDTO> GetMasterListByFieldName(MasterListInput input);
        List<string> GetMasterListByFieldName(string fieldName);
        bool IsMasterListValid(string fieldName, string fieldValue);
    }
}
