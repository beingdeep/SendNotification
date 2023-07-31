# Send Notification Assignment Solution

The Send Notification project is an Azure Function App that facilitates sending notifications to users by processing user notifications and sending them to a Service Bus queue. This project is built using Microsoft Azure Functions, Microsoft Azure Service Bus, and relies on an external database for retrieving user notifications. Detailed Assignment can be found here [a relative link](ASSIGNMENT.md)

## Table of Contents

- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Improvement](#improvement)

## Prerequisites

Before running the SendNotification project, ensure you have the following installed:

- .NET Core SDK
- Microsoft Azure Subscription
- Azure Service Bus Queue
- SQL Database (e.g., Azure SQL Database) with a stored procedure to retrieve user notifications.

## Getting Started

1. Clone this repository to your local machine:

```bash
git clone https://github.com/beingdeep/SendNotification.git
```
to run the solution in the local update please update the following value in local.settings.json (it is now checked-in) 
```json
{ 
  "Values": {
    "SQLConnection": "",
    "QueueConnectionString": "",
    "QueueName": "",
    "TimerScheduleExpression": "0 */15 * * * *"
  }
}
```
Assuming that you already have the required resources in place and also have a stored procedure "sp_UsersToNotify"

## Project Structure
The solution consists of 2 projects. 
  - SendNotification
  - SendNotification.Test

### Summary
 - The GetNotificationDetails class contains an Azure Function RunAsync that is triggered by a timer at specified intervals.
 - The function starts by logging the execution time using a logger.
 - It calls the GetUsersToNotify method of the IUserNotificationService to fetch a list of notifications for users based on the last execution time of the function.
 - If there are notifications to process (i.e., the list is not empty), the function calls the ProcessNotificationsAsync method of the IUserNotificationService to handle the notifications.
 - If any exceptions occur during the process, they are logged using the logger.
 - The GetNotificationDetails function is part of a notification system that relies on the IUserNotificationService to handle user notifications. The class is designed to fetch notifications from an external service, process them, and send them to users via a Service Bus queue or another means, which is not shown in this code snippet.

### SendNotification.Test
  - There are total of 7 Test Cases to test the functionality in the isolation
  - RunAsync_ValidNotifications_ProcessesNotifications
  - RunAsync_NoNotifications_DoesNotProcessNotifications
  - RunAsync_ProcessNotificationsError_ThrowsException
  - GetUsersToNotify_ValidInput_ReturnsNotifications
  - GetUsersToNotify_DatabaseError_ThrowsException
  - ProcessNotificationsAsync_ValidNotifications_LogsAndSendsMessages
  - ProcessNotificationsAsync_NullNotifications_LogsAndThrowsException

## Improvement

