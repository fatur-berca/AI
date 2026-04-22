using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterTruckSealBLL
    {
        List<MasterTruckSealDTO> GetMasterTruckSeal();
        bool SaveData(MasterTruckSealDTO input);
        bool DelData(int keyID);

        bool CheckAvailSealNumber(string sealNumber);
    }
}
