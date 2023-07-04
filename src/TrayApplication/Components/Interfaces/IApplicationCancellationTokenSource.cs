namespace DeviceProfiles.TrayApplication.Components.Interfaces;

public interface IApplicationCancellationTokenSource : IDisposable
{
    /// <summary>
    /// Provide access to the application's common cancellation token.
    /// </summary>
    CancellationToken Token { get; }
    /// <summary>
    /// Cancel the global cancellation token.
    /// </summary>
    void Cancel();
}