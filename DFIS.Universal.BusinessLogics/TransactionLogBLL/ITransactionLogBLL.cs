using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Inputs;

namespace DFIS.Universal.BusinessLogics
{
    public interface ITransactionLogBLL
    {
        List<TransactionLogDTO> GetLog(TransactionLogInput input);

        List<NotificationDTO> GetNotification(string username);

        void SetNotificationRead(string username);
    }
}
