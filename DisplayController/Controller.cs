using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApi;

namespace DisplayController
{
    internal class Controller
    {
        private IConfigurationRoot _config;
        private DisplayWrapper _displayApi;

        internal Controller()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            LogManager.Configuration = new NLogLoggingConfiguration(_config.GetSection("NLog"));

            _displayApi = new DisplayWrapper();
        }
    }
}
