using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApi;

namespace DisplayController.App.Control
{
    internal class MainController
    {
        private DisplayWrapper _displayApi;
        // TODO: Trigger the changing with something.

        internal MainController()
        {
            _displayApi = new DisplayWrapper();
        }
    }
}
