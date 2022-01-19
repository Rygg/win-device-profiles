﻿using DisplayController.App.Configuration;
using DisplayController.App.Control;
using DisplayController.App.Resources.Text;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DisplayController.App
{
    /// <summary>
    /// Application context for the DisplayController application.
    /// </summary>
    internal sealed class DisplayControllerApplicationContext : ApplicationContext
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
            var icon = new NotifyIcon
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
            ToolStripMenuItem profileItem;
            if (!profiles.Any())
            {
                profileItem = new ToolStripMenuItem(Strings.TrayIconNoProfilesAvailable);
                profileItem.Enabled = false;
            }
            else
            {
                profileItem = new ToolStripMenuItem(Strings.TrayIconSwitchProfiles); // Create SwitchProfiles container.
                var switchProfileDropDownMenu = new ContextMenuStrip(); // Create the inner menu:
                foreach (var profile in profiles.Select((value, index) => new { value, index }))
                {
                    var profileText = GetProfileContextMenuText(profile.value, profile.index);
                    // Populate with menu items.
                    var menuItem = new ToolStripMenuItem(profileText, null, async (s, e) => await OnProfileClick(s, e), profile.value.Name); // Set ProfileName as name.
                    switchProfileDropDownMenu.Items.Add(menuItem);
                    Log.Debug($"Added {profileText} to context menu profiles");
                }
                profileItem.DropDown = switchProfileDropDownMenu; // Set as dropdown menu for the main item.
            }
            contextMenu.Items.Add(Strings.TrayIconCopyDataToClipboard, null, OnCopyDataToClipboard);
            contextMenu.Items.Add(profileItem); // Add the created profile container.
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
            var hotkeyString = profile.HotKey != null ? $"({profile.HotKey}) | " : string.Empty;
            return $"{Strings.TrayIconProfile} #{index + 1}: {hotkeyString}{profile.Name}";
        }

        /// <summary>
        /// Method handles clicks from the SwitchProfile context menu from the tray.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task OnProfileClick(object? sender, EventArgs e)
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

            if(await _controller.SetDisplayProfile(clickedProfile.Name, CancellationToken.None))
            {
                Log.Info("Profile changed successfully!");
            }
            else
            {
                Log.Error("Profile could not be changed.");
            }
        }

        /// <summary>
        /// Sets retrieved display data to the clipboard so users can create profiles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCopyDataToClipboard(object? sender, EventArgs e)
        {
            var displayData = _controller.GetRetrievedDisplayDataString();
            Clipboard.SetText(displayData);
        }

        /// <summary>
        /// EventHandler for when the exit is pressed from the context menu of the tray icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContextMenuExit(object? sender, EventArgs e)
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
