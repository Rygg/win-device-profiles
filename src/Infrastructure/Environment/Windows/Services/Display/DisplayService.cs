using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Environment.Windows.Services.Display;

public sealed class DisplayService : IDisplayController
{
    private readonly ILogger<DisplayService> _logger;

    public DisplayService(ILogger<DisplayService> logger)
    {
        _logger = logger;
    }

    public void Foo()
    {
        throw new NotImplementedException();
    }
}