using Application.Features.HotKeys.Commands;
using Application.Features.HotKeys.Queries;
using Application.Features.Profiles.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using WindowsTrayApplication.Components.TrayIcon;

namespace WindowsTrayApplication.Components;

public sealed class DeviceProfilesApplicationContext : ApplicationContext
{
    private readonly ISender _mediatR;
    private readonly ILogger<DeviceProfilesApplicationContext> _logger;
    private readonly CancellationTokenSource _applicationCts;

    private readonly NotifyIcon _trayIcon;

    public DeviceProfilesApplicationContext(
        ISender mediatR,
        ILogger<DeviceProfilesApplicationContext> logger,
        ApplicationTrayIconBuilder trayIconBuilder
        )
    {
        _mediatR = mediatR;
        _logger = logger;
        _applicationCts = new CancellationTokenSource();

        var profiles = _mediatR.Send(new GetProfilesQuery(), _applicationCts.Token).GetAwaiter().GetResult();
        _trayIcon = trayIconBuilder.BuildApplicationTrayIcon(OnExit, OnCopyDataToClipboard, OnProfileClick, profiles);

        _ = BackgroundLoop(_applicationCts.Token);
    }

    /// <summary>
    /// Background loop listening for key presses.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken to stop the loop.</param>
    private async Task BackgroundLoop(CancellationToken cancellationToken)
    {
        await _mediatR.Send(new RegisterHotKeysCommand(), cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            var profile = await _mediatR.Send(new GetHotKeyPressProfileChangeQuery(), cancellationToken);
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Change a profile based on the click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnProfileClick(object? sender, EventArgs e)
    {

    }

    /// <summary>
    /// Sets retrieved display data to the clipboard so users can create profiles more easily.
    /// </summary>
    private static void OnCopyDataToClipboard(object? sender, EventArgs e)
    {
        var displayData = "";
        //var displayData = _application.GetRetrievedDisplayDataString();
        Clipboard.SetText(displayData);
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
        _logger.LogDebug("Shutting down the application..");
        _trayIcon.Visible = false; // Hide tray icon, so it won't remain there until hovered on.
        _applicationCts.Cancel();
        
        //_application.Dispose(); // TODO: Dispose the main controller.
        _logger.LogInformation("Application shutting down.");
        System.Windows.Forms.Application.Exit();
    }
}