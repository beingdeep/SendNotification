using System.Data;
using System.Data.SqlClient;

var numberOfRecords = 100;
var connectionString = "Server=tcp:raboassignment.database.windows.net,1433;Initial Catalog=raboassignment;Persist Security Info=False;User ID=sadmin;Password=Winter@456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

Random random = new Random();

using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();

    using (SqlCommand command = connection.CreateCommand())
    {
        command.CommandType = CommandType.Text;
        command.CommandText = "Truncate table UserNotification; INSERT INTO UserNotification (RecordId, UserId, UserName, UserEmail, DataValue, NotificationFlag, NotificationTime) " +
                              "VALUES (@RecordId, @UserId, @UserName, @UserEmail, @DataValue, @NotificationFlag, @NotificationTime)";

        // Prepare parameters
        SqlParameter recordIdParam = command.Parameters.Add("@RecordId", SqlDbType.Int);
        SqlParameter userIdParam = command.Parameters.Add("@UserId", SqlDbType.Int);
        SqlParameter userNameParam = command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100);
        SqlParameter userEmailParam = command.Parameters.Add("@UserEmail", SqlDbType.NVarChar, 100);
        SqlParameter dataValueParam = command.Parameters.Add("@DataValue", SqlDbType.NVarChar, 200);
        SqlParameter notificationFlagParam = command.Parameters.Add("@NotificationFlag", SqlDbType.Bit);
        SqlParameter notificationTimeParam = command.Parameters.Add("@NotificationTime", SqlDbType.DateTime2);

        for (int i = 0; i < numberOfRecords; i++)
        {
            // Generate random data for each record
            int recordId = i + 1;
            int userId = random.Next(1000, 10000);
            string userName = "User_" + userId;
            string userEmail = "user" + userId + "@example.com";
            string dataValue = "Data_" + i;
            bool notificationFlag = random.Next(0, 2) == 1; // true or false
            DateTime notificationTime = DateTime.Now;

            // Set parameter values
            recordIdParam.Value = recordId;
            userIdParam.Value = userId;
            userNameParam.Value = userName;
            userEmailParam.Value = userEmail;
            dataValueParam.Value = dataValue;
            notificationFlagParam.Value = notificationFlag;
            notificationTimeParam.Value = notificationTime;

            // Execute the insert command
            command.ExecuteNonQuery();
        }
    }
}
