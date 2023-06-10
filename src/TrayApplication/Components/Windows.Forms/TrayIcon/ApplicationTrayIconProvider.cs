using System.Globalization;
using System.Text.Json;
using Application.Features.Devices.Queries;
using Application.Features.Profiles.Commands.ActivateProfile;
using Application.Features.Profiles.Commands.Common;
using Application.Features.Profiles.Commands.ImportProfiles;
using Domain.Entities;
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
    private readonly IRequestSender _sender;
    private readonly CancellationToken _applicationCancellationToken;
    
    private Action? _onTrayIconClosed;

    public ApplicationTrayIconProvider(
        IRequestSender sender,
        IApplicationCancellationTokenSource applicationCancellationTokenSource,
        ILogger<ApplicationTrayIconProvider> logger
    )
    {
        ArgumentNullException.ThrowIfNull(applicationCancellationTokenSource);

        _logger = logger;
        _sender = sender;
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
        // Add Import/Export items:
        contextMenu.Items.Add(Strings.TrayIconImportProfiles, null, OnImportProfiles);
        contextMenu.Items.Add(Strings.TrayIconExportProfiles, null, OnExportProfiles);
        // Separator
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(Strings.TrayIconCopyDataToClipboard, null, OnCopyDataToClipboard);
        // Add the profiles section.
        contextMenu.Items.Add(profilesSection); // Add the created profile container.
        // Separator
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
        _onTrayIconClosed?.Invoke(); // Invoke the callback if it is set.
    }

    /// <summary>
    /// Event handler for importing profiles from a file.
    /// </summary>
    private void OnImportProfiles(object? sender, EventArgs e)
    {
        using var openFileDialog = new OpenFileDialog
        {
            Title = Strings.ImportProfilesDialogTitle,
            Filter = Strings.ImportProfilesDialogFilter,
            FileName = "profiles.json"
        };
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                using var reader = new StreamReader(openFileDialog.OpenFile());
                var file = JsonSerializer.Deserialize<ProfilesFileDto>(reader.BaseStream, JsonSerializerOptions.Default);
                if (file == null || !file.Profiles.Any())
                {
                    _logger.ProfilesCouldNotBeDeserialized();
                    return;
                }
                var command = new ImportProfilesCommand
                {
                    ProfileFile = file
                };
                _sender.SendAsync(command, _applicationCancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.ExceptionOnFileImport(ex);
                throw;
            }
        }
    }

    /// <summary>
    /// Event handler for exporting the current profiles to a file.
    /// </summary>
    private void OnExportProfiles(object? sender, EventArgs e)
    {
        // TODO:
    }

    /// <summary>
    /// Sets the current device data to the clipboard so users can create profiles more easily.
    /// </summary>
    private void OnCopyDataToClipboard(object? sender, EventArgs e)
    {
        var deviceData = _sender.SendAsync(new GetCurrentDeviceInformationQuery(), _applicationCancellationToken).GetAwaiter().GetResult();
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
    private void OnProfileClick(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem menuItem)
        {
            _logger.EventHandlerTriggeredByWrongType(nameof(OnProfileClick));
            return; // not possible to process currently.
        }

        var profileId = int.Parse(menuItem.Name, CultureInfo.InvariantCulture);
        _sender.SendAsync(new ActivateProfileCommand { ProfileId = profileId }, _applicationCancellationToken).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Get the context menu text for this profile based on the profile and the iteration. 
    /// </summary>
    private static string GetProfileContextMenuText(DeviceProfile profile, int index)
    {
        var hotkeyString = profile.HotKey != null ? $"({profile.HotKey}) | " : string.Empty; // TODO: Not working properly.
        return $"{Strings.TrayIconProfile} #{index + 1}: {hotkeyString}{profile}";
    }
}