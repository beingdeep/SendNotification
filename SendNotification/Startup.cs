using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendNotification.Configurations;
using Db = SendNotification.Database;
using SendNotification.Services;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;

[assembly: FunctionsStartup(typeof(SendNotification.Startup))]
namespace SendNotification;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        // Register configuration options
        builder.Services.AddOptions<Config>()
           .Configure<IConfiguration>((settings, configuration) =>
           {
               configuration.Bind(settings);
           });

        // Retrieve the configuration options
        Config configuration = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<Config>>().Value;
        ILogger logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Startup>>();

        // Register services with DI
        builder.Services.AddScoped<IUserNotificationService, UserNotificationService>();
        builder.Services.AddScoped<Db.IDatabase, Db.Database>();

        builder.Services.AddScoped(c => new SqlConnection(configuration.SQLConnection)); 

        // Register the IQueueClient
        builder.Services.AddSingleton<IQueueClient>(c =>
        {
            // Get the Service Bus connection string and queue name from the configuration
            string serviceBusConnectionString = configuration.QueueConnectionString;
            string queueName = configuration.QueueName;

            // Create a new instance of the QueueClient using the connection string and queue name
            return new QueueClient(serviceBusConnectionString, queueName);
        });
    }
}

