using System.Collections.Generic;
using AutoMapper;
using TOM.EntitiesDAL.EDMX;
using DFIS.Universal.Domain.DTOs;
using System.Linq;
using DFIS.Universal.BusinessLogics;
using TOM.Master.Repositories;

namespace TOM.Master.BusinessLogics
{
    public class HomeBLL : IHomeBLL
    {
        //private readonly IMasterApprovalNewsRepo _masterApprovalNewsRepo;
        private readonly INotificationSystemRepo _notificationSystemRepo;
        private readonly IMasterConfigurationBLL _masterConfigurationBll;

        public HomeBLL(INotificationSystemRepo notificationSystemRepo,IMasterConfigurationBLL masterConfigurationBll)
        {
            _notificationSystemRepo = notificationSystemRepo;
            _masterConfigurationBll = masterConfigurationBll;
        }

        public List<NotificationDTO> ViewALLNotificationByUserId(string userid)
        {
            return Mapper.Map<List<NotificationDTO>>(_notificationSystemRepo.GetALLNotificationByIDUser(userid));
        }

        public List<NotificationDTO> GetNotificationByUserId(List<int> idnotif, bool reload, string userId)
        {
            List<NotificationSystem> listNotification = new List<NotificationSystem>();
            List<NotificationSystem> tempListNotification = _notificationSystemRepo.GetNotificationSystemsByIdUserIsOpenFalse(idnotif, userId);
            if (!reload)
            {
                NotificationSystem tempNotification = new NotificationSystem();
                string notif = tempListNotification.Count > 1 ? "notifications" : "notification";
                tempNotification.Description = "You have " + tempListNotification.Count + " " + notif;
                //tempNotification.Description = "You have <span id='countnotif'>" + tempListNotification.Count + "</span> " + notif;
                listNotification.Add(tempNotification);
            }
            if (tempListNotification.Count > 0)
                listNotification.AddRange(tempListNotification);
            return Mapper.Map<List<NotificationDTO>>(listNotification);
        }

        public string GetUrlBestPracticeToolbox()
        {
            MasterConfiguration result = _masterConfigurationBll.GetAllMasterConfiguration().Where(x => x.PageName.ToLower() == "general" && x.Description.ToLower() == "additionalurl1").FirstOrDefault();
            string value = result != null ? result.Value : "";
            return value;
        }

        public void UpdateNotification(List<int> idnotif, bool isOpen, string userId)
        {
            foreach (int id in idnotif)
            {
                NotificationSystem notification = _notificationSystemRepo.GetNotificationSystemByID(id);
                if (isOpen){
                    notification.IsRead = true;
                    notification.IsOpen = true;
                }
                else
                    notification.IsRead = true;
                notification.UpdatedBy = userId;
                _notificationSystemRepo.SaveData(notification, false);
            }
        }

        public void SaveData(string title, string file, string image, string userid)
        {
            /*NewsHighlight input = new NewsHighlight()
            {
                Title = title,
                FileUpload = file,
                Image = image,
                Status = 1,
                //input.IsPublished = false;
                IsActive = true,
                CreatedBy = userid,
                UpdatedBy = userid
            };
            _masterApprovalNewsRepo.SaveData(input, true);
            */
        }
    }
}