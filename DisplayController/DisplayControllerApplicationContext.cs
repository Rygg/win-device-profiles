using System;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using DisplayController.Resources;

namespace DisplayController
{
    internal class DisplayControllerApplicationContext : ApplicationContext
    {
        private IConfigurationRoot _config;
        private readonly NotifyIcon _trayIcon;
        private readonly Controller _controller;

        public DisplayControllerApplicationContext()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build(); // Read config.

            LogManager.Configuration = new NLogLoggingConfiguration(_config.GetSection("NLog")); // Set log configuration.

            _trayIcon = CreateTrayIcon(); // Initialize Tray Icon
            
            _controller = new Controller(); // Create the actual controller class.
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
                Icon = null, // TODO: Get icon.
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
            return menu;
        }

        void OnContextMenuExit(object sender, EventArgs e)
        {
            _trayIcon.Visible = false; // Hide tray icon, so it won't remain there until hovered on.
            Application.Exit();
        }
    }
}
