using DeviceProfiles.Application.Common.Interfaces;

namespace DeviceProfiles.Application.Features.Devices.Queries;

public sealed record GetCurrentDeviceInformationQuery : IRequest<string>;

public sealed class GetCurrentDevicesQueryHandler : IRequestHandler<GetCurrentDeviceInformationQuery,string>
{
    private readonly IDisplayDeviceController _displayDeviceController;

    public GetCurrentDevicesQueryHandler(IDisplayDeviceController displayDeviceController)
    {
        _displayDeviceController = displayDeviceController;
    }

    public async Task<string> Handle(GetCurrentDeviceInformationQuery request, CancellationToken cancellationToken)
    {
        return await _displayDeviceController.GetCurrentDisplayInformationString(cancellationToken).ConfigureAwait(false);
    }
}