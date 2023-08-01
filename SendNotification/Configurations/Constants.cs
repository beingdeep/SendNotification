using System.Diagnostics.CodeAnalysis;

namespace SendNotification.Configurations;

[ExcludeFromCodeCoverage]
public static class Constants
{
    // Database Procedure Name
    public const string STORED_PROCEDURE_NAME = "sp_UsersToNotify";
}