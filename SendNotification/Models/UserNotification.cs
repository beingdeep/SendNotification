using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendNotification.Models
{
    public class UserNotification
    {
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string DataValue { get; set; }
        public bool NotificationFlag { get; set; } 
    }
}
