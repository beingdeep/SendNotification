﻿using System.Diagnostics.CodeAnalysis;

namespace SendNotification.Configurations;

[ExcludeFromCodeCoverage]
public class Config
{
    public string SQLConnection { get; set; }
    public string QueueConnectionString { get; set; }
    public string QueueName { get; set; }
}

