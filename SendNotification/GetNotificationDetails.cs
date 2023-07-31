using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendNotification.Models;
using SendNotification.Services;

namespace SendNotification;
public class GetNotificationDetails
{
    private readonly ILogger<GetNotificationDetails> _logger;
    private readonly IUserNotificationService _userNotificationService;

    public GetNotificationDetails(ILogger<GetNotificationDetails> logger, IUserNotificationService userNotificationService)
    {
        _logger = logger;
        _userNotificationService = userNotificationService;
    }

    [FunctionName("GetNotificationDetails")]
    public async Task RunAsync([TimerTrigger("%TimerScheduleExpression%")] TimerInfo timerInfo)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {timerInfo.ScheduleStatus.Last}");

        try
        {
            // Call the GetUserToNotify method
            IEnumerable<UserNotification> notifications = await _userNotificationService.GetUsersToNotify(timerInfo.ScheduleStatus.Last);

            if (notifications.Any())
            {
                await _userNotificationService.ProcessNotificationsAsync(notifications);
            }
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during the process
            _logger.LogError(ex, "An error occurred while processing notifications.");
            throw; // Rethrow the exception to the Azure Functions runtime
        }
    }

}


