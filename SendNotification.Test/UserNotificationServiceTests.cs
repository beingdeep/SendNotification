using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Moq;
using SendNotification.Database;
using SendNotification.Models;
using SendNotification.Services;

namespace SendNotification.Test;

[TestClass]
public class UserNotificationServiceTests
{
    private Mock<ILogger<UserNotificationService>> _loggerMock;
    private Mock<IDatabase> _databaseMock;
    private Mock<IQueueClient> _serviceBusQueueClientMock;

    [TestInitialize]
    public void TestInitialize()
    {
        _loggerMock = new Mock<ILogger<UserNotificationService>>();
        _databaseMock = new Mock<IDatabase>();
        _serviceBusQueueClientMock = new Mock<IQueueClient>();
    }

    [TestMethod]
    public async Task GetUsersToNotify_ValidInput_ReturnsNotifications()
    {
        // Arrange
        DateTime lastExecutedDateTime = DateTime.UtcNow;
        var userNotifications = new List<UserNotification>
            {
                new UserNotification { UserId = 1, UserName = "John Doe", NotificationFlag = true },
                new UserNotification { UserId = 2, UserName = "Jane Smith", NotificationFlag = true }
            };

        _databaseMock.Setup(m => m.CallUsersToNotifyProc(lastExecutedDateTime)).ReturnsAsync(userNotifications);

        var userNotificationService = new UserNotificationService(_loggerMock.Object, _databaseMock.Object, _serviceBusQueueClientMock.Object);

        // Act
        var result = await userNotificationService.GetUsersToNotify(lastExecutedDateTime);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(userNotifications.Count, result.Count());
    }

    [TestMethod]
    public async Task GetUsersToNotify_DatabaseError_ThrowsException()
    {
        // Arrange
        DateTime lastExecutedDateTime = DateTime.UtcNow;

        _databaseMock.Setup(m => m.CallUsersToNotifyProc(lastExecutedDateTime)).ThrowsAsync(new Exception("Database error"));

        var userNotificationService = new UserNotificationService(_loggerMock.Object, _databaseMock.Object, _serviceBusQueueClientMock.Object);

        // Act & Assert 
        await Assert.ThrowsExceptionAsync<Exception>(() => userNotificationService.GetUsersToNotify(lastExecutedDateTime));

    }

    [TestMethod]
    public async Task ProcessNotificationsAsync_ValidNotifications_LogsAndSendsMessages()
    {
        // Arrange
        var notifications = new List<UserNotification>
            {
                new UserNotification { UserId = 1, UserName = "John Doe", NotificationFlag = true },
                new UserNotification { UserId = 2, UserName = "Jane Smith", NotificationFlag = true }
            };

        _serviceBusQueueClientMock.Setup(m => m.SendAsync(It.IsAny<Message>())).Returns(Task.CompletedTask);

        var userNotificationService = new UserNotificationService(_loggerMock.Object, _databaseMock.Object, _serviceBusQueueClientMock.Object);

        // Act
        await userNotificationService.ProcessNotificationsAsync(notifications);

        // Assert
        // Expects that messages are sent for each notification
        _serviceBusQueueClientMock.Verify(m => m.SendAsync(It.IsAny<Message>()), Times.Exactly(notifications.Count));
    }

    [TestMethod]
    public async Task ProcessNotificationsAsync_NullNotifications_LogsAndThrowsException()
    {
        // Arrange
        var userNotificationService = new UserNotificationService(_loggerMock.Object, _databaseMock.Object, _serviceBusQueueClientMock.Object);

        // Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => userNotificationService.ProcessNotificationsAsync(null));
    }
}
