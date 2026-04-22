using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;
using DFIS.Universal.Repositories;
using DFIS.Contracts;
using TOM.EntitiesDAL;
using TOM.EntitiesDAL.EDMX;
using AutoMapper;
using DFIS.Universal.BusinessLogics;

namespace TOM.Master.BusinessLogics
{
    public class TransactionLogBLL : ITransactionLogBLL
    {
        private readonly ITransactionLogRepo _transactionLogRepo;
        private readonly IGenericRepository<TransactionLog> _generalRepo;
        private readonly IGenericRepository<NotificationSystem> _notificationRepo;

        public TransactionLogBLL(ITransactionLogRepo transactionLogRepo, IGenericRepository<TransactionLog> generalRepo, IGenericRepository<NotificationSystem> notificationRepo)
        {
            _transactionLogRepo = transactionLogRepo;
            _generalRepo = generalRepo;
            _notificationRepo = notificationRepo;
        }

        public List<TransactionLogDTO> GetLog(TransactionLogInput input)
        {
            //sementara ctr di harcode dulu, nnti tingal di buka saja
            //var dbResult = _generalRepo.Get(x => x.MasterFunction.FunctionName == input.ctr).ToList();
            string ctr = "MstList";
            var dbResult = _generalRepo.Get(x => x.MasterFunction.FunctionName == ctr).ToList();
            List<TransactionLogDTO> transactionLogDTOs = dbResult.Select(x => new TransactionLogDTO() { Action = x.Action, ControllerName = x.MasterFunction.FunctionName, CreatedDate = x.CreatedDate, IdLog = x.IDLog, Remark = x.Remarks, UserName = x.MasterUser.FullName }).OrderByDescending(x => x.CreatedDate).ToList();
            return Mapper.Map<List<TransactionLogDTO>>(transactionLogDTOs);
        }

        public List<NotificationDTO> GetNotification(string username)
        {
            //var dbResult = _notificationRepo.Get(x => x.IDUser == "1").ToList();
            var dbResult = _notificationRepo.Get().Where(x => x.IDUser == username).Take(10).ToList();
            List<NotificationDTO> notificationDTOs = dbResult.Select(x => new NotificationDTO() { CreatedBy = x.CreatedBy, CreatedDate = x.CreatedDate, Description = x.Description, IDNotification = x.IDNotification, IDUser = x.IDUser, IsOpen = x.IsOpen, IsRead = x.IsRead, PageName = x.PageName, UpdatedBy = x.UpdatedBy, UpdatedDate = x.UpdatedDate}).OrderByDescending(x => x.CreatedDate).ToList();
            return Mapper.Map<List<NotificationDTO>>(notificationDTOs);
        }

        public void SetNotificationRead(string username)
        {
            List<NotificationSystem> notificationSystems = _notificationRepo.Get().Where(x => x.IDUser == username && x.IsRead == false).ToList();
            foreach (NotificationSystem notificationSystem in notificationSystems)
            {
                notificationSystem.IsRead = true;
                _notificationRepo.Update(notificationSystem);
            }
            _notificationRepo.Save();
        }
    }
}
