﻿using System.Globalization;
using Application.Features.Devices.Queries;
using Application.Features.HotKeys.Commands;
using Application.Features.HotKeys.Queries;
using Application.Features.Profiles.Commands;
using Application.Features.Profiles.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using TrayApplication.Components.Windows.Forms.TrayIcon;
using TrayApplication.Extensions;

namespace TrayApplication.Components.Windows.Forms;

public sealed class DeviceProfilesApplicationContext : ApplicationContext
{
    private readonly ISender _mediatR;
    private readonly ILogger<DeviceProfilesApplicationContext> _logger;
    private readonly CancellationTokenSource _applicationCts;

    private readonly NotifyIcon _trayIcon;

    public DeviceProfilesApplicationContext(
        ISender mediatR,
        ILogger<DeviceProfilesApplicationContext> logger,
        TrayIconBuilder trayIconBuilder
        )
    {
        if (trayIconBuilder == null)
        {
            throw new ArgumentNullException(nameof(trayIconBuilder));
        }
        _mediatR = mediatR;
        _logger = logger;
        _applicationCts = new CancellationTokenSource();

        var profiles = _mediatR.Send(new GetProfilesQuery(), _applicationCts.Token).GetAwaiter().GetResult();
        _trayIcon = trayIconBuilder.BuildTrayIcon(OnExit, OnCopyDataToClipboard, OnProfileClick, profiles);

        _ = BackgroundLoop(_applicationCts.Token);
    }

    /// <summary>
    /// Background loop listening for key presses.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken to stop the loop.</param>
    private async Task BackgroundLoop(CancellationToken cancellationToken)
    {
        await _mediatR.Send(new RegisterHotKeysCommand(), cancellationToken).ConfigureAwait(false);
        while (!cancellationToken.IsCancellationRequested)
        {
            var profile = await _mediatR.Send(new GetRegisteredHotKeyPressQuery(), cancellationToken).ConfigureAwait(false);
            await _mediatR.Send(new ActivateProfileCommand { ProfileId = profile.Id }, cancellationToken).ConfigureAwait(false);
        }
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
        _mediatR.Send(new ActivateProfileCommand { ProfileId = profileId }, _applicationCts.Token).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Sets the current device data to the clipboard so users can create profiles more easily.
    /// </summary>
    private void OnCopyDataToClipboard(object? sender, EventArgs e)
    {
        var deviceData = _mediatR.Send(new GetCurrentDeviceInformationQuery(), _applicationCts.Token).GetAwaiter().GetResult();
        Clipboard.SetText(deviceData);
        _logger.CopiedInformationToClipboard(deviceData);
    }

    /// <summary>
    /// EventHandler for TrayIcon Exit click.
    /// </summary>
    private void OnExit(object? sender, EventArgs args) => Exit();
    /// <summary>
    /// Method closes and releases all required handles and closes the application.
    /// </summary>
    private void Exit()
    {
        _logger.StartingShutdown();
        _trayIcon.Visible = false; // Hide tray icon, so it won't remain there until hovered on.
        _applicationCts.Cancel();
        _logger.ApplicationShuttingDown();
        Dispose();
        System.Windows.Forms.Application.Exit();
    }

    /// <summary>
    /// Dispose the context.
    /// </summary>
    public new void Dispose()
    {
        Dispose(true);
        base.Dispose();
    }
    /// <summary>
    /// Override for base class disposer to dispose all disposable fields from this file.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        _applicationCts.Dispose();
        _trayIcon.Dispose();
        base.Dispose(disposing); // Dispose base.
    }
}