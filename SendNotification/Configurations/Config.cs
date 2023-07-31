using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendNotification.Configurations
{
    public class Config
    {
        public string SQLConnection { get; set; }
        public string QueueConnectionString { get; set; }
        public string QueueName { get; set; } 
    }
}
