using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistence;

public sealed class DeviceProfilesDbContext : DbContext, IDeviceProfilesDbContext
{
    public DeviceProfilesDbContext(DbContextOptions<DeviceProfilesDbContext> options) : base(options) { }

    public DbSet<DeviceProfile> DeviceProfiles => Set<DeviceProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

}