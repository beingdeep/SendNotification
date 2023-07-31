using SendNotification.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SendNotification.Utilities
{
    public static class HelperUtility
    {
        public static string SerializeNotification(UserNotification notification)
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
}
