using System.Collections.Generic;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Repositories;
using TOM.Master.Repositories;

namespace DFIS.Universal.BusinessLogics
{
    public class MasterConfigurationEmailBLL : IMasterConfigurationEmailBLL
    {
        private readonly IMasterConfigurationEmailRepo _masterConfigurationEmailRepo;
        private readonly IMasterFunctionRepo _masterFunctionRepo;

        public MasterConfigurationEmailBLL(IMasterConfigurationEmailRepo masterConfigurationEmailRepo, IMasterFunctionRepo masterFunctionRepo)
        {
            _masterConfigurationEmailRepo = masterConfigurationEmailRepo;
            _masterFunctionRepo = masterFunctionRepo;
        }

        public List<MasterFunctionDTO> GetMasterFunctionTypeMenu(string type)
        {
            var temp = Mapper.Map<List<MasterFunction>, List<MasterFunctionDTO>>(_masterFunctionRepo.GetMasterFunctionByType("Menu"));
            return temp;
        }
        public List<MasterConfigurationEmail> GetAllMasterConfigurationEmail()
        {
            return _masterConfigurationEmailRepo.GetAllMasterConfigurationEmail();
        }

        public MasterConfigurationEmail SaveData(MasterConfigurationEmail input, bool status)
        {
            return _masterConfigurationEmailRepo.SaveData(input, status);
        }
    }
}
