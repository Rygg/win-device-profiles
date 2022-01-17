using DisplayController.App.Configuration;
using DisplayController.App.Control;
using DisplayController.App.Resources.Text;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DisplayController.App
{
    /// <summary>
    /// Application context for the DisplayController application.
    /// </summary>
    internal class DisplayControllerApplicationContext : ApplicationContext
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Profile configuration.
        /// </summary>
        private readonly AppConfiguration _config;
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
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: false).Build(); // Read config.
            _config = new AppConfiguration(config.GetRequiredSection("Configuration"));

            LogManager.Configuration = _config.NLog; // Set NLog configuration.
            
            _trayIcon = CreateTrayIcon(_config.Profiles); // Initialize Tray Icon            

            _controller = new MainController(); // Create the actual controller class.
            _controller.Start(_config); // Start controller functionality.
        }

        /// <summary>
        /// Create the NotifyIcon for system tray.
        /// </summary>
        /// <returns></returns>
        private NotifyIcon CreateTrayIcon(Profile[] profiles)
        {           
            var icon = new NotifyIcon()
            {
                Text = Strings.TrayIconTooltip,
                Icon = new System.Drawing.Icon(typeof(DisplayControllerApplicationContext), "Resources.Images.app.ico"),
                Visible = true,
                ContextMenuStrip = CreateTrayIconContextMenu(profiles)
            };
            return icon;
        }

        /// <summary>
        /// Create the context menu for the tray icon.
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip CreateTrayIconContextMenu(Profile[] profiles)
        {
            var contextMenu = new ContextMenuStrip();

            var switchProfileItem = new ToolStripMenuItem(Strings.TrayIconSwitchProfiles); // Create SwitchProfiles container.
            var switchProfileDropDownMenu = new ContextMenuStrip(); // Create the inner menu:
            foreach(var profile in profiles.Select((value, index) => new { value, index }))
            {
                var profileText = GetProfileContextMenuText(profile.value, profile.index);
                // Populate with menu items.
                var menuItem = new ToolStripMenuItem(profileText, null, OnProfileClick, profile.value.Name); // Set ProfileName as name.
                switchProfileDropDownMenu.Items.Add(menuItem);
                Log.Debug($"Added {profileText} to context menu profiles");
            }
            switchProfileItem.DropDown = switchProfileDropDownMenu; // Set as dropdown menu for the main item.
            contextMenu.Items.Add(switchProfileItem); // Add the created profile container.
            contextMenu.Items.Add(new ToolStripSeparator()); // Add a separator.
            contextMenu.Items.Add(Strings.TrayIconExitText, null, OnContextMenuExit); // Add an exit button.

            Log.Debug("Created ContextMenu for the tray icon.");
            return contextMenu;
        }

        /// <summary>
        /// Get the context menu text for this profile based on the profile and the iteration. 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string GetProfileContextMenuText(Profile profile, int index)
        {
            return $"{Strings.TrayIconProfile} #{index + 1}: {profile.Name}";
        }

        /// <summary>
        /// Method handles clicks from the SwitchProfile context menu from the tray.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProfileClick(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem menuItem)
            {
                Log.Error(nameof(OnProfileClick) + "Triggered by wrong type. Ignoring");
                return; // not possible.
            }
            var clickedProfile = _config.Profiles.FirstOrDefault(p => p.Name == menuItem.Name); // Get the clicked profile.
            if(clickedProfile == null)
            {
                Log.Error("Clicked profile not found from configuration.");
                return; // not possible.
            }
            Log.Info($"User selected profile {clickedProfile.Name} from the tray icon. Switching profile.");

            // TODO: Actually switch profile.
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
