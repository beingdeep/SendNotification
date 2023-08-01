using SendNotification.Models;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace SendNotification.Database;

public interface IDatabase
{
    Task<IEnumerable<UserNotification>> CallUsersToNotifyProc(DateTime lastExecutedDateTime);
}

