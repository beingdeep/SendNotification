using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Moq;
using SendNotification.Models;
using SendNotification.Services;

namespace SendNotification.Test;

[TestClass]
public class GetNotificationDetailsTests
{
    private Mock<ILogger<GetNotificationDetails>> _loggerMock;
    private Mock<IUserNotificationService> _userNotificationServiceMock;
    private GetNotificationDetails _notificationDetails;
    private TimerInfo _timerInfo;

    [TestInitialize]
    public void TestInitialize()
    {
        _loggerMock = new Mock<ILogger<GetNotificationDetails>>();
        _userNotificationServiceMock = new Mock<IUserNotificationService>();
        _notificationDetails = new GetNotificationDetails(_loggerMock.Object, _userNotificationServiceMock.Object);
        _timerInfo = new TimerInfo(default, new ScheduleStatus() { Last = DateTime.Now });
    }

    [TestMethod]
    public async Task RunAsync_ValidNotifications_ProcessesNotifications()
    {
        // Arrange 

        var notifications = new List<UserNotification>
            {
                new UserNotification { UserId = 1, UserName = "John Doe", NotificationFlag = true },
                new UserNotification { UserId = 2, UserName = "Jane Smith", NotificationFlag = true }
            };

        _userNotificationServiceMock.Setup(m => m.GetUsersToNotify(It.IsAny<DateTime>())).ReturnsAsync(notifications);



        // Act
        await _notificationDetails.RunAsync(_timerInfo);

        // Assert
        // Expects that the GetUserToNotify and ProcessNotificationsAsync methods are called
        _userNotificationServiceMock.Verify(m => m.GetUsersToNotify(It.IsAny<DateTime>()), Times.Once);
        _userNotificationServiceMock.Verify(m => m.ProcessNotificationsAsync(It.IsAny<IEnumerable<UserNotification>>()), Times.Once);
    }

    [TestMethod]
    public async Task RunAsync_NoNotifications_DoesNotProcessNotifications()
    {
        // Arrange
        var timerInfo = new TimerInfo(default, default);
        var emptyNotifications = Enumerable.Empty<UserNotification>();

        _userNotificationServiceMock.Setup(m => m.GetUsersToNotify(It.IsAny<DateTime>())).ReturnsAsync(emptyNotifications);

        // Act
        await _notificationDetails.RunAsync(_timerInfo);

        // Assert
        // Expects that the GetUserToNotify method is called, but ProcessNotificationsAsync is not called
        _userNotificationServiceMock.Verify(m => m.GetUsersToNotify(It.IsAny<DateTime>()), Times.Once);
        _userNotificationServiceMock.Verify(m => m.ProcessNotificationsAsync(It.IsAny<IEnumerable<UserNotification>>()), Times.Never);
    }

    [TestMethod] 
    public async Task RunAsync_ProcessNotificationsError_ThrowsException()
    {
        // Arrange 
        var timerInfo = new TimerInfo(default, default);
        var notifications = new List<UserNotification>
            {
                new UserNotification { UserId = 1, UserName = "John Doe", NotificationFlag = true },
                new UserNotification { UserId = 2, UserName = "Jane Smith", NotificationFlag = true }
            };

        _userNotificationServiceMock.Setup(m => m.GetUsersToNotify(It.IsAny<DateTime>())).ReturnsAsync(notifications);
        _userNotificationServiceMock.Setup(m => m.ProcessNotificationsAsync(It.IsAny<IEnumerable<UserNotification>>())).ThrowsAsync(new Exception("Process error"));

        // Act & Assert
        await _notificationDetails.RunAsync(_timerInfo);
    }
}
