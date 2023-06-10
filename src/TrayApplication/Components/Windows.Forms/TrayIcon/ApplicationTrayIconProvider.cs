using System.Globalization;
using Application.Features.Devices.Queries;
using Application.Features.Profiles.Commands.ActivateProfile;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using TrayApplication.Components.Interfaces;
using TrayApplication.Extensions;
using TrayApplication.Resources.Text;

namespace TrayApplication.Components.Windows.Forms.TrayIcon;

/// <summary>
/// Class provides the tray icon functionality for the application context.
/// </summary>
public sealed class ApplicationTrayIconProvider : ITrayIconProvider
{
    private readonly ILogger<ApplicationTrayIconProvider> _logger;
    private readonly ISender _mediatR;
    private readonly CancellationToken _applicationCancellationToken;
    
    private Action? _onTrayIconClosed;

    public ApplicationTrayIconProvider(
        ISender mediatR, 
        IApplicationCancellationTokenSource applicationCancellationTokenSource,
        ILogger<ApplicationTrayIconProvider> logger
        )
    {
        ArgumentNullException.ThrowIfNull(applicationCancellationTokenSource);

        _mediatR = mediatR;
        _logger = logger;
        _applicationCancellationToken = applicationCancellationTokenSource.Token;
        TrayIcon = new NotifyIcon
        {
            Text = Strings.TrayIconTooltip,
            Visible = true,
            Icon = new Icon(typeof(Program), "Resources.Images.app.ico"),
        };

    }

    /// <inheritdoc cref="ITrayIconProvider.TrayIcon"/>
    public NotifyIcon TrayIcon { get; }

    /// <inheritdoc cref="ITrayIconProvider.SetOnCloseCallback"/>
    public void SetOnCloseCallback(Action closingCallback)
    {
        _onTrayIconClosed = closingCallback;
    }

    public void UpdateTrayIconContents(DeviceProfile[] profiles)
    {
        ArgumentNullException.ThrowIfNull(profiles);

        var profilesSection = BuildProfilesSection(profiles);

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add(Strings.TrayIconCopyDataToClipboard, null, OnCopyDataToClipboard);
        contextMenu.Items.Add(profilesSection); // Add the created profile container.
        contextMenu.Items.Add(new ToolStripSeparator()); // Add a separator.
        contextMenu.Items.Add(Strings.TrayIconExitText, null, OnExit); // Add an exit button.
        _logger.CreatedContextMenu();
        TrayIcon.ContextMenuStrip = contextMenu;
    }

    /// <summary>
    /// EventHandler for when an user clicks on the Exit button.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnExit(object? sender, EventArgs e)
    {
        TrayIcon.Visible = false; // Hide tray icon, so it won't remain there until hovered on.
        TrayIcon.Dispose();
        _onTrayIconClosed?.Invoke(); // Invoke the callback.
    }

    /// <summary>
    /// Sets the current device data to the clipboard so users can create profiles more easily.
    /// </summary>
    private void OnCopyDataToClipboard(object? sender, EventArgs e)
    {
        var deviceData = _mediatR.Send(new GetCurrentDeviceInformationQuery(), _applicationCancellationToken).GetAwaiter().GetResult();
        Clipboard.SetText(deviceData);
        _logger.CopiedInformationToClipboard(deviceData);
    }


    /// <summary>
    /// Create the profiles section for the TrayIcon menu.
    /// </summary>
    /// <param name="profiles">Current collection of profiles on the device.</param>
    /// <returns>A menu item containing the required profiles.</returns>
    private ToolStripMenuItem BuildProfilesSection(IReadOnlyCollection<DeviceProfile> profiles)
    {
        ToolStripMenuItem profileItem;
        if (profiles.Count == 0)
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
                var menuItem = new ToolStripMenuItem(profileText, null, OnProfileClick, profile.value.Id.ToString(CultureInfo.InvariantCulture)); // Set ProfileName as name.
                switchProfileDropDownMenu.Items.Add(menuItem);
                _logger.AddedProfileToContextMenu(profileText);
            }
            profileItem.DropDown = switchProfileDropDownMenu; // Set as dropdown menu for the main item.
        }

        return profileItem;
    }

    /// <summary>
    /// Change a profile based on the click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnProfileClick(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem menuItem)
        {
            _logger.EventHandlerTriggeredByWrongType(nameof(OnProfileClick));
            return; // not possible to process currently.
        }

        var profileId = int.Parse(menuItem.Name, CultureInfo.InvariantCulture);
        _mediatR.Send(new ActivateProfileCommand { ProfileId = profileId }, _applicationCancellationToken).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the context menu text for this profile based on the profile and the iteration. 
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="index"></param>
    private static string GetProfileContextMenuText(DeviceProfile profile, int index)
    {
        var hotkeyString = profile.HotKey != null ? $"({profile.HotKey}) | " : string.Empty;
        return $"{Strings.TrayIconProfile} #{index + 1}: {hotkeyString}{profile}";
    }
}