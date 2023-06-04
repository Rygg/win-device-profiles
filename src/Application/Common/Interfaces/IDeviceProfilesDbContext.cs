using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IDeviceProfilesDbContext
{
    public DbSet<DeviceProfile> DeviceProfiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}