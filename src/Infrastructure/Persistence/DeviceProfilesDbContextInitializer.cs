using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public sealed class DeviceProfilesDbContextInitializer
{
    private readonly ILogger<DeviceProfilesDbContext> _logger;
    private readonly DeviceProfilesDbContext _dbContext;

    public DeviceProfilesDbContextInitializer(ILogger<DeviceProfilesDbContext> logger, DeviceProfilesDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    /// <summary>
    /// Initialize the database.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken for the operation.</param>
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_dbContext.Database.IsSqlite())
            {
                await _dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.ErrorOccurredDuringMigration(ex);
            throw;
        }
    }
}