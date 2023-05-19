using System.Globalization;
using Domain.Models;
using Microsoft.Extensions.Logging;
using TrayApplication.Extensions;
using TrayApplication.Resources.Text;

namespace TrayApplication.Components.Windows.Forms.TrayIcon;

public sealed class TrayIconBuilder
{
    private readonly ILogger<TrayIconBuilder> _logger;

    public TrayIconBuilder(ILogger<TrayIconBuilder> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Create the NotifyIcon for system tray.
    /// </summary>
    /// <param name="onExit">EventHandler for when the user clicks Exit.</param>
    /// <param name="onCopy">EventHandler for when the user clicks Copy to Clipboard.</param>
    /// <param name="onProfileClick">EventHandler for when the user clicks a profile.</param>
    /// <param name="profiles">List of profiles to add to the context menu.</param>
    /// <returns>A functional tray icon for the application.</returns>
    public NotifyIcon BuildTrayIcon(
        EventHandler onExit,
        EventHandler onCopy,
        EventHandler onProfileClick,
        DeviceProfile[] profiles)
    {
        var icon = new NotifyIcon
        {
            Text = Strings.TrayIconTooltip,
            Visible = true,
            Icon = new Icon(typeof(Program), "Resources.Images.app.ico"),
            ContextMenuStrip = CreateTrayIconContextMenu(onExit, onCopy, onProfileClick, profiles)
        };
        return icon;
    }

    /// <summary>
    /// Create the context menu for the tray icon.
    /// </summary>
    /// <param name="onExit">EventHandler for when the user presses exit.</param>
    /// <param name="onCopy">EventHandler for when the user clicks copy to clipboard.</param>
    /// <param name="onProfileClick">EventHandler for when the user clicks a profile.</param>
    /// <param name="profiles">Profiles to add to the context menu.</param>
    /// <returns></returns>
    private ContextMenuStrip CreateTrayIconContextMenu(
        EventHandler onExit,
        EventHandler onCopy,
        EventHandler onProfileClick,
        DeviceProfile[] profiles
        )
    {
        var contextMenu = new ContextMenuStrip();
        ToolStripMenuItem profileItem;
        if (!profiles.Any())
        {
            profileItem = new ToolStripMenuItem(Strings.TrayIconNoProfilesAvailable)
            {
                Enabled = false
            };
        }
        else
        {
            profileItem = new ToolStripMenuItem(Strings.TrayIconSwitchProfiles); // Create SwitchProfiles container.
            var switchProfileDropDownMenu = new ContextMenuStrip(); // Create the inner menu:
            foreach (var profile in profiles.Select((value, index) => new { value, index })) // Populate with menu items.
            {
                var profileText = GetProfileContextMenuText(profile.value, profile.index); // Get text.
                var menuItem = new ToolStripMenuItem(profileText, null, onProfileClick, profile.value.Id.ToString(CultureInfo.InvariantCulture)); // Set ProfileName as name.
                switchProfileDropDownMenu.Items.Add(menuItem);
                _logger.AddedProfileToContextMenu(profileText);
            }
            profileItem.DropDown = switchProfileDropDownMenu; // Set as dropdown menu for the main item.
        }
        contextMenu.Items.Add(Strings.TrayIconCopyDataToClipboard, null, onCopy);
        contextMenu.Items.Add(profileItem); // Add the created profile container.
        contextMenu.Items.Add(new ToolStripSeparator()); // Add a separator.
        contextMenu.Items.Add(Strings.TrayIconExitText, null, onExit); // Add an exit button.

        _logger.CreatedContextMenu();
        return contextMenu;
    }

    /// <summary>
    /// Get the context menu text for this profile based on the profile and the iteration. 
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string GetProfileContextMenuText(DeviceProfile profile, int index)
    {
        var hotkeyString = profile.HotKey != null ? $"({profile.HotKey}) | " : string.Empty;
        return $"{Strings.TrayIconProfile} #{index + 1}: {hotkeyString}{profile}";
    }
}