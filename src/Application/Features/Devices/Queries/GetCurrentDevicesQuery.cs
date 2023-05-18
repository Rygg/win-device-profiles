using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Devices.Queries;

public sealed record GetCurrentDevicesQuery : IRequest<string>;

public sealed class GetCurrentDevicesQueryHandler : IRequestHandler<GetCurrentDevicesQuery,string>
{
    private readonly IDisplayDeviceController _displayDeviceController;

    public GetCurrentDevicesQueryHandler(IDisplayDeviceController displayDeviceController)
    {
        _displayDeviceController = displayDeviceController;
    }

    public async Task<string> Handle(GetCurrentDevicesQuery request, CancellationToken cancellationToken)
    {
        return await _displayDeviceController.GetCurrentDisplayInformationString(cancellationToken).ConfigureAwait(false);
    }
}