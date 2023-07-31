using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using SendNotification.Database;
using SendNotification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using SendNotification.Utilities;
using Microsoft.Extensions.Options;
using SendNotification.Configurations;

namespace SendNotification.Services;

public class UserNotificationService : IUserNotificationService
{
    private readonly ILogger<UserNotificationService> _logger;
    private readonly IDatabase _database;
    private readonly IQueueClient _serviceBusQueueClient; 

    public UserNotificationService(ILogger<UserNotificationService> logger, IDatabase database, IQueueClient serviceBusQueueClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _serviceBusQueueClient = serviceBusQueueClient ?? throw new ArgumentNullException(nameof(serviceBusQueueClient)); 
    }

    public async Task<IEnumerable<UserNotification>> GetUsersToNotify(DateTime lastExecutedDateTime)
    {
        _logger.LogInformation($"GetUsersToNotify method called with lastExecutedDateTime: {lastExecutedDateTime}");

        // Add necessary null checks for parameters and handle any validation logic

        try
        {
            // Call the database to get the list of UserNotification objects
            IEnumerable<UserNotification> notifications = await _database.CallUsersToNotifyProc(lastExecutedDateTime);

            _logger.LogInformation($"GetUsersToNotify method executed successfully. Found {notifications.Count()} notifications.");

            return notifications;
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during the process
            _logger.LogError(ex, "An error occurred while getting notifications for users.");
            throw; // Rethrow the exception to the caller
        }
    }

    public async Task ProcessNotificationsAsync(IEnumerable<UserNotification> notifications)
    {
        if (notifications == null)
        {
            _logger.LogError($"Argument Null Exception in nameof({ProcessNotificationsAsync})");
            throw new ArgumentNullException(nameof(notifications));
        }
        foreach (var notification in notifications)
        {
            try
            {
                // Convert the UserNotification object to a message and send it to the Service Bus queue
                string messageBody = HelperUtility.SerializeNotification(notification);
                Message message = new(System.Text.Encoding.UTF8.GetBytes(messageBody));

                await _serviceBusQueueClient.SendAsync(message);

                // Log successful message sending
                _logger.LogInformation($"Message sent to Service Bus queue for UserId: {notification.UserId}");
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur while sending the message
                _logger.LogError(ex, $"Error sending message to Service Bus queue for UserId: {notification.UserId}");
                throw;
            }
        }
    }
}

