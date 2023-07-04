using MediatR;

namespace DeviceProfiles.TrayApplication.Components.Interfaces;

/// <summary>
/// Interface for sending requests in scopes.
/// </summary>
public interface IRequestSender
{
    /// <summary>
    /// Sends a request with no expected responses.
    /// </summary>
    Task SendAsync(IRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Sends a request with an expected response type and returns the response.
    /// </summary>
    /// <returns>The response to the request.</returns>
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
}