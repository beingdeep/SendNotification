using System;
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
            await _sqlConnection.OpenAsync();

            using (SqlCommand command = new("sp_UsersToNotify", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add the parameter to the command
                command.Parameters.Add("@lastExecutedTime", SqlDbType.DateTime2).Value = lastExecutedDateTime;

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
        catch (SqlException ex)
        {
            // Handle different SQL exception
            switch (ex.Number)
            {
                case 4060:
                    // Handle database not found or unavailable
                    _logger.LogError(ex, "An error occurred, database not found");
                    break;
                case 18456:
                    // Handle login failed
                    _logger.LogError(ex, "An error occurred, database login failed");
                    break;
                default:
                    // Handle other SQL exceptions or take generic actions
                    _logger.LogError(ex, $"An error occurred, while working in the database: {ex.Message}");
                    break;
            }
            throw;
        }
        catch { throw; }
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



