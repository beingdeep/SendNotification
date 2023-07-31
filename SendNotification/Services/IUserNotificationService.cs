using SendNotification.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SendNotification.Services
{
    public interface IUserNotificationService
    { 
        Task<IEnumerable<UserNotification>> GetUsersToNotify(DateTime lastExecutedDateTime);
        Task ProcessNotificationsAsync(IEnumerable<UserNotification> notifications);
    }
}
