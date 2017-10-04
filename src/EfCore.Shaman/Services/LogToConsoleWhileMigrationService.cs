using System;
using JetBrains.Annotations;

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
            catch (Exception e) // no exception logging
            {
                Console.WriteLine($"SHAMAN LogInfoToConsole exception : {e.Message}");
            }
        }

        private static void LogExceptionToConsole(Guid locationId, Exception exception)
        {
            try
            {
                Console.WriteLine($"SHAMAN EXCEPTION: {locationId}: {exception.Message}");
            }
            catch // no exception logging
            {
            }
        }

        public void ModifyShamanOptions([NotNull] ShamanOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var callerInfo = ShamanCallstackSpy.GetCallerInfo(options.Logger);
            if (callerInfo != CallerInfoType.AddMigration && callerInfo != CallerInfoType.RemoveMigration) return;
            var consoleLogger = new MethodCallLogger(LogInfoToConsole, LogExceptionToConsole);
            var message = callerInfo == CallerInfoType.AddMigration
                ? "Running under add-migration"
                : "Running under remove-migration";

            consoleLogger.Log(typeof(LogToConsoleWhileMigrationService), nameof(ModifyShamanOptions), message);
            options.WithLogger(options.Logger.Append(consoleLogger));
        }
    }
}