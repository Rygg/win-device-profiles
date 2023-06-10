using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TrayApplication.Components.Interfaces;

namespace TrayApplication.Components.Windows.Forms.Context;

/// <summary>
/// Implementation for the IRequestSender interface sending scoped requests to the application layer.
/// </summary>
public sealed class ScopedRequestSender : IRequestSender
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ScopedRequestSender(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <inheritdoc cref="IRequestSender.SendAsync"/>
    public async Task SendAsync(IRequest request, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IRequestSender.SendAsync"/>
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var response = await sender.Send(request, cancellationToken).ConfigureAwait(false);
        return response;
    }
}