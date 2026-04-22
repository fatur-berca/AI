using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFIS.Contracts;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using TOM.Master.Repositories;
using DFIS.Utils;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.BusinessLogics
{
    public class MasterRoleFunctionBLL : IMasterRoleFunctionBLL
    {
        private readonly IMasterRoleFunctionRepo _masterRoleFunctionRepo;
        private readonly IMasterFunctionRepo _masterFunctionRepo;
        private readonly IGenericRepository<MasterRole> _generalRepo;
        private readonly IGenericRepository<MasterRolesFunctionMapping> _generalRepoRoleFunction;


        public MasterRoleFunctionBLL(IMasterRoleFunctionRepo masterRoleFunctionRepo, IMasterFunctionRepo masterFunctionRepo, IGenericRepository<MasterRole> generalRepo, IGenericRepository<MasterRolesFunctionMapping> generalRepoRoleFunction)
        {
            _masterRoleFunctionRepo = masterRoleFunctionRepo;
            _masterFunctionRepo = masterFunctionRepo;
            _generalRepo = generalRepo;
            _generalRepoRoleFunction = generalRepoRoleFunction;
        }

        public List<MasterRoleDTO> GetRoleList()
        {
            var queryFilter = PredicateHelper.True<MasterRole>();
            queryFilter = queryFilter.And(p => p.IsActive);
            return Mapper.Map < List < MasterRoleDTO >> (_generalRepo.Get(queryFilter).ToList());
        }

        public List<MasterRoleFunctionDTO> GetDistinctMasterRoleFunction()
        {
            return Mapper.Map<List<MasterRolesFunctionMapping>, List<MasterRoleFunctionDTO>>(_masterRoleFunctionRepo.GetDistinctRoleMasterRoleFunction());
        }

        public List<int> GetIDFunctionList(int idRole)
        {
            return _masterRoleFunctionRepo.GetListIDFunctionByIdRole(idRole);
        }

        public MasterRolesFunctionMapping SaveData(MasterRoleFunctionDTO input, bool status)
        {
            List<MasterRolesFunctionMapping> listTempIDForm = new List<MasterRolesFunctionMapping>();
            List<int> tempCheckParentID = new List<int>();
            foreach (int temp in input.ListIDFunction)
            {
                MasterRolesFunctionMapping masterRoleFunction = _masterRoleFunctionRepo.GetMasterRoleFunctionByIDRoleIDFunction(input.IDRole, temp);
                if (masterRoleFunction != null)
                {
                    if (masterRoleFunction.MasterFunction.Type == "Button") { 
                        if(status)
                            masterRoleFunction.IsActive = true;
                        else
                            masterRoleFunction.IsActive = !masterRoleFunction.IsActive;
                        _masterRoleFunctionRepo.SaveData(masterRoleFunction, false);
                    }
                    else if(masterRoleFunction.MasterFunction.Type == "Form")
                        listTempIDForm.Add(masterRoleFunction);
                    else //tipenya module pasti selalu true
                    {
                        masterRoleFunction.IsActive = true;
                        _masterRoleFunctionRepo.SaveData(masterRoleFunction, false);
                    }
                    //bagian untuk ngecek parentnya supaya aktif/tidak aktif
                    if(!tempCheckParentID.Any(x => x == masterRoleFunction.MasterFunction.ParentIDFunction))
                        tempCheckParentID.Add(masterRoleFunction.MasterFunction.ParentIDFunction??0);
                }
                else
                {
                    masterRoleFunction = new MasterRolesFunctionMapping();
                    masterRoleFunction.IDRole = input.IDRole;
                    masterRoleFunction.IDFunction = temp;
                    masterRoleFunction.IsActive = true;
                    masterRoleFunction.CreatedBy = input.CreatedBy;
                    masterRoleFunction.UpdatedBy = input.UpdatedBy;
                    masterRoleFunction.Remarks = " - ";
                    _masterRoleFunctionRepo.SaveData(masterRoleFunction, true);
                    MasterFunction tempMasterFunction = _masterFunctionRepo.GetMasterFunctionByIDFunction(temp);
                    //bagian untuk ngecek parentnya supaya aktif/tidak aktif
                    if (!tempCheckParentID.Any(x => x == tempMasterFunction.ParentIDFunction))
                        tempCheckParentID.Add(tempMasterFunction.ParentIDFunction??0);
                }
            }
            List<MasterRolesFunctionMapping> tempListMasterRolesFunction = new List<MasterRolesFunctionMapping>();
            MasterRolesFunctionMapping tempCheck = new MasterRolesFunctionMapping();
            foreach (int parentid in tempCheckParentID.OrderByDescending(x => x))
            {
                tempListMasterRolesFunction = _masterRoleFunctionRepo.GetMasterRoleFunctionActiveByIDRoleParentIDFunction(input.IDRole, parentid);//cek semua anak2 yang isactive true
                tempCheck = _masterRoleFunctionRepo.GetMasterRoleFunctionByIDRoleIDFunction(input.IDRole, parentid);
                if (tempListMasterRolesFunction.Count > 0) //kalau ada, maka parent is active di true kan
                {
                    if (parentid != 0) { 
                        if (tempCheck == null) //kalau parent nya tidak ada
                        {
                            tempCheck = new MasterRolesFunctionMapping();
                            tempCheck.IDRole = input.IDRole;
                            tempCheck.IDFunction = parentid;
                            tempCheck.IsActive = true;
                            tempCheck.CreatedBy = input.CreatedBy;
                            tempCheck.UpdatedBy = input.UpdatedBy;
                            tempCheck.Remarks = " - ";
                            _masterRoleFunctionRepo.SaveData(tempCheck, true);
                        }
                        else
                        {
                            tempCheck.IsActive = true;
                            _masterRoleFunctionRepo.SaveData(tempCheck, false);
                        }
                    }
                }
                else//kalau anak2nya tidak ada yang aktif maka parentnya di falsekan
                {
                    tempCheck.IsActive = false;
                    _masterRoleFunctionRepo.SaveData(tempCheck, false);
                }
            }
            
            //foreach (MasterRolesFunctionMapping tempForm in listTempIDForm)
            //{
            //    List<MasterFunction> checkMasterFunctionFormChildren = _masterFunctionRepo.GetMasterFunctionByParentID(tempForm.IDFunction);
            //    if (checkMasterFunctionFormChildren.Count > 0)
            //    {
            //        tempForm.IsActive = true;
            //        _masterRoleFunctionRepo.SaveData(tempForm, false);
            //    }
            //    else
            //    {
            //        tempForm.IsActive = false;
            //        _masterRoleFunctionRepo.SaveData(tempForm, false);
            //    }
            //}

            return new MasterRolesFunctionMapping();
        }

        public void UpdateIsActiveByIDRole(int idrole, bool status)
        {
            List<MasterRolesFunctionMapping> listMasterRoleFunction = _masterRoleFunctionRepo.GetAllMasterRoleFunctionByIDRole(idrole);
            foreach (MasterRolesFunctionMapping temp in listMasterRoleFunction)
            {
                temp.IsActive = status;
                _masterRoleFunctionRepo.SaveData(temp, false);
            }
        }

        public List<MasterRoleFunctionTreeListView> GetMasterRoleFunctionTreeList()
        {
            return _masterFunctionRepo.GetMasterRoleFunctionTreeList();
        }
    }
}
