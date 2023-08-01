using SendNotification.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SendNotification.Database;

[ExcludeFromCodeCoverage]
public interface IDatabase
{
    Task<IEnumerable<UserNotification>> CallUsersToNotifyProc(DateTime lastExecutedDateTime);
}

