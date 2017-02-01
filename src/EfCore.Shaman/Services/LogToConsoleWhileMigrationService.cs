#region using

using System;

#endregion

namespace EfCore.Shaman.Services
{
    /// <summary>
    ///     Appends logging to console feature while running under migration
    /// </summary>
    public class LogToConsoleWhileMigrationService : IShamanOptionModificationService
    {
        #region Static Methods

        private static void LogInfoToConsole(ShamanLogMessage info)
        {
            Console.WriteLine($"SHAMAN: {info.Source}: {info.Message}");
        }

        #endregion

        #region Instance Methods

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

        #endregion
    }
}