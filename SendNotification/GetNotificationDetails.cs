using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendNotification.Models;
using SendNotification.Services;

namespace SendNotification
{
    public class GetNotificationDetails
    {
        private readonly ILogger<GetNotificationDetails> _logger;
        private readonly IUserNotificationService _userNotificationService;

        public GetNotificationDetails(ILogger<GetNotificationDetails> logger, IUserNotificationService userNotificationService)
        {
            _logger = logger;
            _userNotificationService = userNotificationService;
        }

        /// <summary>
        /// Azure Function triggered by a timer at specified intervals.
        /// Fetches and processes notifications for users.
        /// </summary>
        /// <param name="timerInfo">Information about the timer trigger.</param>
        [FunctionName("GetNotificationDetails")]
        public async Task RunAsync([TimerTrigger("%TimerScheduleExpression%")] TimerInfo timerInfo)
        {
            // Log the execution time of the function
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                // Call the service to get the list of users to notify
                IEnumerable<UserNotification> notifications = await _userNotificationService.GetUsersToNotify(timerInfo.ScheduleStatus.Last);

                if (notifications.Any())
                {
                    // Process the notifications for the users
                    await _userNotificationService.ProcessNotificationsAsync(notifications);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the process
                _logger.LogError(ex, "An error occurred while processing notifications."); 
            }
        }
    }
}
