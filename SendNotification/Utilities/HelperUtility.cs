using System;
using System.Text.Json;

namespace SendNotification.Utilities;
public static class HelperUtility
{
    public static string SerializeNotification<T>(T notification)
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }
        try
        {
            // Serialize the UserNotification object to JSON
            string json = JsonSerializer.Serialize(notification);

            return json;
        }
        catch { throw; }
    }
}

