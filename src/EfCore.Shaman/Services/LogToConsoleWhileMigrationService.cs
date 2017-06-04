using System;

namespace EfCore.Shaman.Services
{
    /// <summary>
    ///     Appends logging to console feature while running under migration
    /// </summary>
    public class LogToConsoleWhileMigrationService : IShamanOptionModificationService
    {
        public static void LogInfoToConsole(ShamanLogMessage info)
        {
            try
            {
                Console.WriteLine($"SHAMAN: {info.Source}: {info.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"SHAMAN LogInfoToConsole exception : {e.Message}");
            }
        }

        public void ModifyShamanOptions(ShamanOptions options)
        {
            var callerInfo = ShamanCallstackSpy.CallerInfo;
            if (callerInfo != CallerInfoType.AddMigration && callerInfo != CallerInfoType.RemoveMigration) return;
            var consoleLogger = new MethodCallLogger(LogInfoToConsole);
            var message = callerInfo == CallerInfoType.AddMigration
                ? "Running under add-migration"
                : "Running under remove-migration";

            consoleLogger.Log(typeof(LogToConsoleWhileMigrationService), nameof(ModifyShamanOptions), message);
            options.WithLogger(options.Logger.Append(consoleLogger));
        }
    }
}