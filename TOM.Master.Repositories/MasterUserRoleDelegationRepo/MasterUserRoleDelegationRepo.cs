using TOM.EntitiesDAL.EDMX;
using TOM.Master.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;

namespace TOM.Master.Repositories
{
    public class MasterUserRoleDelegationRepo : IMasterUserRoleDelegationRepo
    {
        public int Save(MasterUserRoleDelegationDTO Input)
        {
            //DFISContextDB context = new DFISContextDB();
            using (var context = new TOMContextDB())
            {
                MasterUserRoleDelegation mstData = new MasterUserRoleDelegation();
                if (Input.IDUserDelegation != 0)
                {
                    mstData = context.MasterUserRoleDelegations.FirstOrDefault(x => x.IDUserDelegation == Input.IDUserDelegation);
                }
                else if (Input.IDUserDelegation == 0)
                {
                    mstData = context.MasterUserRoleDelegations.FirstOrDefault(x => x.IDUserTo == Input.IDUserTo && x.DelegationIDRole == Input.DelegationIDRole &&
                    x.EffectiveStartDate.Equals(Input.EffectiveStartDate) && x.EffectiveEndDate.Equals(Input.EffectiveEndDate));
                }
                
                if (mstData != null)
                {
                    // update datas
                    //mstData.IDUserFrom = Input.IDUserFrom;
                    //mstData.IDUserTo = Input.IDUserTo;
                    //mstData.DelegationIDRole = Input.DelegationIDRole.Value;
                    //mstData.EffectiveStartDate = Input.EffectiveStartDate;
                    mstData.EffectiveEndDate = Input.EffectiveEndDate;
                    mstData.IsActive = Input.IsActive;
                    mstData.UpdatedDate = DateTime.Now;
                    mstData.UpdatedBy = Input.CurrentUser;

                    //if (!String.IsNullOrEmpty(Input.DelegationIDLocation))
                    //{
                    //    string[] locationDelegation = Input.DelegationIDLocation.Split(',');

                    //    // cek data userlocation sebelumnya by IDUserDelegation
                        
                    //    //List<UserLocationDelegation> prevUl = context.UserLocationDelegations.Where(x => x.IDUserDelegation == mstData.IDUserDelegation).
                    //    context.UserLocationDelegations.RemoveRange(
                    //        context.UserLocationDelegations.Where(x => x.IDUserDelegation == mstData.IDUserDelegation).ToList()
                    //    );

                    //    foreach (var id in locationDelegation)
                    //    {
                    //        // new data
                    //        UserLocationDelegation userLoc = new UserLocationDelegation();
                    //        userLoc.DelegationIDLocation = id;
                    //        userLoc.IsActive = Input.IsActive.Value;
                    //        userLoc.CreatedBy = Input.IDUserFrom;
                    //        userLoc.CreatedDate = DateTime.Now;
                    //        userLoc.UpdatedBy = Input.IDUserFrom;
                    //        userLoc.UpdatedDate = DateTime.Now;
                    //        mstData.UserLocationDelegations.Add(userLoc);
                    //        context.MasterUserRoleDelegations.Add(mstData);   
                    //    }
                    //}

                }else{
                    // set new Batch properties
                    // new data
                    MasterUserRoleDelegation dbIn = new MasterUserRoleDelegation();
                    dbIn.IDUserFrom = Input.IDUserFrom;
                    dbIn.IDUserTo = Input.IDUserTo;
                    dbIn.DelegationIDRole = Input.DelegationIDRole.Value;
                    dbIn.EffectiveStartDate = Input.EffectiveStartDate;
                    dbIn.EffectiveEndDate = Input.EffectiveEndDate;
                    dbIn.IsActive = Input.IsActive;
                    dbIn.CreatedBy = Input.IDUserFrom;
                    dbIn.CreatedDate = DateTime.Now;
                    dbIn.UpdatedBy = Input.IDUserFrom;
                    dbIn.UpdatedDate = DateTime.Now;

                    // bagian insert UserLocationDelegation
                    if (!String.IsNullOrEmpty(Input.DelegationIDLocation))
                    {
                        string[] locationDelegation = Input.DelegationIDLocation.Split(',');
                        foreach (var id in locationDelegation)
                        {
                            UserLocationDelegation ul = context.UserLocationDelegations.Where(x => x.IDUserDelegation == dbIn.IDUserDelegation && x.DelegationIDLocation == id).SingleOrDefault();
                            if (ul == null)
                            {
                                // new data
                                UserLocationDelegation userLoc = new UserLocationDelegation();
                                userLoc.IDUserDelegation = dbIn.IDUserDelegation;
                                userLoc.DelegationIDLocation = id;
                                userLoc.IsActive = Input.IsActive.Value;
                                userLoc.CreatedBy = Input.IDUserFrom;
                                userLoc.CreatedDate = DateTime.Now;
                                userLoc.UpdatedBy = Input.IDUserFrom;
                                userLoc.UpdatedDate = DateTime.Now;
                                dbIn.UserLocationDelegations.Add(userLoc);
                                context.MasterUserRoleDelegations.Add(dbIn);
                            }
                        }
                    } 

                }
                //return context.SaveChanges();
                try
                {
                    // Your code...
                    // Could also be before try if you know the exception occurs in SaveChanges

                    return context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
        }
    }
}
