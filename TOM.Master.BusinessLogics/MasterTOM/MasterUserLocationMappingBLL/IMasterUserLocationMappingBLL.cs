using System.Collections.Generic;
using TOM.Master.Domain.DTOs;
using TOM.Master.Domain.Inputs;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using System.Linq.Expressions;
using System;

namespace TOM.Master.BusinessLogics
{
    public interface IMasterUserLocationMappingBLL
    {
        List<MasterUserLocationMappingDTO> GetMasterUserLocationMappingss(MasterUserLocationMappingInput input);
        List<MasterUserLocationMappingDTO> GetDistinctLocationMasterUserLocation();
        List<MasterUserLocationMappingDTO> GetMasterUserByLocations(List<string> locations);
        
        List<string> GetListIDLocationByIdUser(string id);
        List<MasterLocationDTO> GetMasterLocationLists(MasterLocationInput input);
        List<MasterUserDTO> GetMasterUserLists(MasterUserInput input);

        MasterUserLocationMappingDTO SaveData(MasterUserLocationMappingDTO input);

        MasterUserLocationMappingDTO EditData(MasterUserLocationMappingDTO input);

        MasterUserLocationMappingDTO GetById(string id);
        void UpdateIsActiveByIDUser(string iduser, bool status);
        List<MasterUserLocationMapTreeListView> GetMasterUserLocationMapTreeList();
        List<string> GetWarehouseNameByIdUser(string id);
        List<MasterUserLocationMappingDTO> GetMasterFunctionByIdUser(string id);
        List<MasterUserLocationMappingDTO> GetLocationsByIdUserAndWarehouseType(string id);
    }
}