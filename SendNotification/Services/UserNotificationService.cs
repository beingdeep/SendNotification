using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using SendNotification.Database;
using SendNotification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendNotification.Utilities;

namespace SendNotification.Services
{
    /// <summary>
    /// Service responsible for handling user notifications and sending them to a Service Bus queue.
    /// </summary>
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

        /// <summary>
        /// Retrieves the list of user notifications that need to be sent based on the last execution time.
        /// </summary>
        /// <param name="lastExecutedDateTime">The last execution time of the function.</param>
        /// <returns>A collection of UserNotification objects.</returns>
        public async Task<IEnumerable<UserNotification>> GetUsersToNotify(DateTime lastExecutedDateTime)
        {
            _logger.LogInformation($"GetUsersToNotify method called with lastExecutedDateTime: {lastExecutedDateTime}");
            
            try
            {
                // Call the database to get the list of UserNotification objects
                IEnumerable<UserNotification> notifications = await _database.CallUsersToNotifyProc(lastExecutedDateTime);

                _logger.LogInformation($"GetUsersToNotify method executed successfully. Found {notifications.Count()} notifications.");

                return notifications;
            }
            catch
            {
                throw; // Rethrow the exception to the caller
            }
        }

        /// <summary>
        /// Processes the list of user notifications and sends them to the Service Bus queue.
        /// </summary>
        /// <param name="notifications">The collection of UserNotification objects to process and send.</param>
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
                catch (MessagingEntityNotFoundException ex)
                {
                    // This exception is thrown when the queue is not found
                    _logger.LogError(ex, $"Messaging Entity Not Found: {ex.Message}");
                    throw;
                }
                catch (ServiceBusException ex)
                {
                    // This exception is thrown for transient errors (e.g., network issues, server busy)
                    _logger.LogError(ex, $"Service Bus Exception: {ex.Message}");
                    throw;
                } 
                catch
                {
                    throw;
                }
            }
        }
    }
}
