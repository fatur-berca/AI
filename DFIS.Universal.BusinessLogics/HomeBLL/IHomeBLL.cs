using System.Collections.Generic;
using DFIS.Universal.Domain.DTOs;

namespace DFIS.Universal.BusinessLogics
{
    public interface IHomeBLL
    {
        List<NotificationDTO> ViewALLNotificationByUserId(string userid);
        List<NotificationDTO> GetNotificationByUserId(List<int> idnotif, bool reload, string userId);
        void UpdateNotification(List<int> idnotif, bool isOpen, string userId);
        void SaveData(string title, string file, string image, string userid);
        string GetUrlBestPracticeToolbox();
    }
}
