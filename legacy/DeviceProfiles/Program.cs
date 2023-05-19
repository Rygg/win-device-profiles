using System;
using DeviceProfiles.Application;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace DeviceProfiles
{
    /// <summary>
    /// DisplayController program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// appsettings.json Configuration.
        /// </summary>
        private static readonly IConfiguration Configuration;
        /// <summary>
        /// Application context variable.
        /// </summary>
        private static DeviceProfilesApplicationContext? _appContext;

        /// <summary>
        /// Static constructor for reading configuration.
        /// </summary>
        static Program()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }

        /// <summary>
        /// The program starting point.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Set Logging.
            var logConfig = new LoggingConfiguration();
            var fileTarget = new FileTarget
            {
                Name = "file",
                FileName = "log/DeviceProfiles.log",
                ArchiveFileName = "log/archive/DeviceProfiles-{#}.log",
                ArchiveNumbering = ArchiveNumberingMode.Date,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveDateFormat = "yyyyMMdd",
                Layout = "${longdate}|${uppercase:${level}}|${logger}|${message}${onexception:${newline}${exception:format=tostring}"
            };
            var logLevelStr = Configuration.GetSection("LogLevel").Value ?? "Off";
            logConfig.AddRule(LogLevel.FromString(logLevelStr), LogLevel.Fatal, fileTarget);
            LogManager.Configuration = logConfig;

            // Create application context:
            _appContext = new DeviceProfilesApplicationContext(Configuration);
            // Run the application.
            System.Windows.Forms.Application.Run(_appContext); // TODO: is this properly disposed when exiting?
        }
    }
}
