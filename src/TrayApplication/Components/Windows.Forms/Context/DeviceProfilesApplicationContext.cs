﻿using Application.Features.HotKeys.Commands;
using Application.Features.HotKeys.Queries;
using Application.Features.Profiles.Commands.ActivateProfile;
using Application.Features.Profiles.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using TrayApplication.Components.Interfaces;
using TrayApplication.Extensions;

namespace TrayApplication.Components.Windows.Forms.Context;

public sealed class DeviceProfilesApplicationContext : ApplicationContext
{
    private readonly ISender _mediatR;
    private readonly ILogger<DeviceProfilesApplicationContext> _logger;
    private readonly ITrayIconProvider _trayIconProvider;
    private readonly IApplicationCancellationTokenSource _applicationCancellationTokenSource;

    //private readonly NotifyIcon _trayIcon;

    public DeviceProfilesApplicationContext(
        ISender mediatR,
        ILogger<DeviceProfilesApplicationContext> logger,
        ITrayIconProvider trayIconProvider, 
        IApplicationCancellationTokenSource applicationCancellationTokenSource)
    {
        _mediatR = mediatR;
        _logger = logger;
        _trayIconProvider = trayIconProvider;
        _applicationCancellationTokenSource = applicationCancellationTokenSource;
        _trayIconProvider.SetOnCloseCallback(CloseApplication); // Set the application to close when the tray icon closes.

        var profiles = _mediatR.Send(new GetProfilesQuery(), _applicationCancellationTokenSource.Token).GetAwaiter().GetResult();

        _trayIconProvider.UpdateTrayIconContents(profiles);
        _ = BackgroundLoop();
    }

    /// <summary>
    /// Background loop listening for key presses.
    /// </summary>
    private async Task BackgroundLoop()
    {
        await _mediatR.Send(new RegisterHotKeysCommand(), _applicationCancellationTokenSource.Token).ConfigureAwait(false);
        while (!_applicationCancellationTokenSource.Token.IsCancellationRequested)
        {
            var profile = await _mediatR.Send(new GetRegisteredHotKeyPressQuery(), _applicationCancellationTokenSource.Token).ConfigureAwait(false);
            await _mediatR.Send(new ActivateProfileCommand { ProfileId = profile.Id }, _applicationCancellationTokenSource.Token).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Closes the application.
    /// </summary>
    private void CloseApplication()
    {
        _logger.StartingShutdown();
        _applicationCancellationTokenSource.Cancel();
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
        _applicationCancellationTokenSource.Dispose();
        base.Dispose(disposing); // Dispose base.
    }
}