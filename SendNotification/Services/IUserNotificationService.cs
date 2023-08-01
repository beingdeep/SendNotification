using SendNotification.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SendNotification.Services;

[ExcludeFromCodeCoverage]
public interface IUserNotificationService
{
    Task<IEnumerable<UserNotification>> GetUsersToNotify(DateTime lastExecutedDateTime);
    Task ProcessNotificationsAsync(IEnumerable<UserNotification> notifications);
}
