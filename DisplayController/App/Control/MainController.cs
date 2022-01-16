using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DisplayController.App.Control
{
    /// <summary>
    /// MainController class responsible for the application functionality.
    /// </summary>
    internal class MainController : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// DisplayController responsible for the display operations.
        /// </summary>
        private DisplayController _displays;
        /// <summary>
        /// HotKeyController responsible for the hotkey operations.
        /// </summary>
        private HotKeyController _hotkeys;

        /// <summary>
        /// Default constructor for the MainController.
        /// </summary>
        internal MainController()
        {
            _displays = new DisplayController();
            _hotkeys = new HotKeyController();
        }

        /// <summary>
        /// Method starts the controller functionality with the configuration given as a parameter.
        /// </summary>
        /// <param name="config">Configuration for the controller.</param>
        internal void Start(IConfigurationRoot config)
        {
            Log.Debug("Starting MainController functionality.");
            // TODO: Create the functionality.
            Log.Info("Started");
        }

        /// <summary>
        /// Dispose the controller.
        /// </summary>
        public void Dispose()
        {
            Log.Trace("Disposing..");
            _hotkeys?.Dispose(); // Dispose the hotkeys handler.
            Log.Trace("Disposed.");
        }
    }
}
