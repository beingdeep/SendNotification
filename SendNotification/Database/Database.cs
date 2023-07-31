﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using SendNotification.Models;

namespace SendNotification.Database; 
public class Database : IDatabase
{
    private readonly SqlConnection _sqlConnection;
    private readonly ILogger<Database> _logger;

    public Database(SqlConnection sqlConnection, ILogger<Database> logger)
    {
        _sqlConnection = sqlConnection ?? throw new ArgumentNullException(nameof(sqlConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Call the stored procedure "sp_UsersToNotify" and get a list of UserNotification objects.
    public async Task<IEnumerable<UserNotification>> CallUsersToNotifyProc(DateTime lastExecutedDateTime)
    {
        _logger.LogInformation($"CallUsersToNotifyProc method called with lastExecutedDateTime: {lastExecutedDateTime}");

        List<UserNotification> notifications = new();

        try
        {
            // Open the SQL connection
            await _sqlConnection.OpenAsync();

            using (SqlCommand command = new("sp_UsersToNotify", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add the parameter to the command
                command.Parameters.Add("@LastExecutedTime", SqlDbType.DateTime2).Value = lastExecutedDateTime;

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    // Read the data from the SqlDataReader and populate the list of UserNotification objects
                    while (reader.Read())
                    {
                        UserNotification notification = new()
                        {
                            RecordId = Convert.ToInt32(reader["RecordId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            UserName = reader["UserName"].ToString(),
                            UserEmail = reader["UserEmail"].ToString(),
                            DataValue = reader["DataValue"].ToString(),
                            NotificationFlag = Convert.ToBoolean(reader["NotificationFlag"])
                        };

                        notifications.Add(notification);
                    }
                }
            }

            _logger.LogInformation($"CallUsersToNotifyProc method executed successfully. Found {notifications.Count} notifications.");

            return notifications;
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during the process
            _logger.LogError(ex, "An error occurred while executing the stored procedure.");
            throw; // Rethrow the exception to the caller
        }
        finally
        {
            // Close the SQL connection in a finally block to ensure it is closed, even if an exception occurs
            if (_sqlConnection.State == ConnectionState.Open)
            {
                await _sqlConnection.CloseAsync();
            }
        }
    }
}


