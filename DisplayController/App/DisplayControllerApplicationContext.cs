using System;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using DisplayController.App.Control;
using DisplayController.App.Resources.Text;

namespace DisplayController.App
{
    /// <summary>
    /// Application context for the DisplayController application.
    /// </summary>
    internal class DisplayControllerApplicationContext : ApplicationContext
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// The configuration file.
        /// </summary>
        private IConfigurationRoot _config;
        /// <summary>
        /// Applications tray icon.
        /// </summary>
        private readonly NotifyIcon _trayIcon;
        /// <summary>
        /// Controller responsible for application functionality.
        /// </summary>
        private readonly MainController _controller;
        /// <summary>
        /// Default constructor for the application context.
        /// </summary>
        public DisplayControllerApplicationContext()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build(); // Read config.
            LogManager.Configuration = new NLogLoggingConfiguration(_config.GetSection("NLog")); // Set log configuration.

            _trayIcon = CreateTrayIcon(); // Initialize Tray Icon            
            _controller = new MainController(); // Create the actual controller class.

            _controller.Start(_config);
        }

        /// <summary>
        /// Create the NotifyIcon for system tray.
        /// </summary>
        /// <returns></returns>
        private NotifyIcon CreateTrayIcon()
        {           
            var icon = new NotifyIcon()
            {
                Text = Strings.TrayIconTooltip,
                Icon = new System.Drawing.Icon(typeof(DisplayControllerApplicationContext), "Resources.Images.app.ico"),
                Visible = true,
                ContextMenuStrip = CreateTrayIconContextMenu()
            };
            return icon;
        }

        /// <summary>
        /// Create the context menu for the tray icon.
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip CreateTrayIconContextMenu()
        {
            var menu = new ContextMenuStrip()
            {
                Items =
                {
                    Strings.TrayIconExitText,
                }
            };
            menu.Items[0].Click += OnContextMenuExit;
            Log.Debug("Created ContextMenu for the tray icon.");
            return menu;
        }

        /// <summary>
        /// EventHandler for when the exit is pressed from the context menu of the tray icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContextMenuExit(object sender, EventArgs e)
        {
            Log.Info("Application close requested from tray icon context menu.");
            Exit();
        }
        /// <summary>
        /// Method closes and releases all required handles and closes the application.
        /// </summary>
        private void Exit()
        {
            Log.Debug("Shutting down the application..");
            _trayIcon.Visible = false; // Hide tray icon, so it won't remain there until hovered on.
            _controller.Dispose(); // // Dispose the main controller.
            Log.Info("Application shutting down.");
            Application.Exit();
        }
    }
}
