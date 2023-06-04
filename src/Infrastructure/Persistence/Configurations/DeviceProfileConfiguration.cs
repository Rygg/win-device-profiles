using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class DeviceProfileConfiguration : IEntityTypeConfiguration<DeviceProfile>
{
    public void Configure(EntityTypeBuilder<DeviceProfile> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(dp => dp.Id);
        builder.Property(dp => dp.Id).IsRequired();
        builder.Property(dp => dp.Name).IsRequired().HasMaxLength(256);
        builder.OwnsOne(dp => dp.HotKey, hotKeyCombinationBuilder =>
        {
            hotKeyCombinationBuilder.WithOwner();
            hotKeyCombinationBuilder.Property(hk => hk.Key).IsRequired();
        });
        builder.OwnsMany(dp => dp.DisplaySettings, displaySettingsBuilder =>
        {
            displaySettingsBuilder.WithOwner();
            displaySettingsBuilder.HasKey(ds => ds.Id);
            displaySettingsBuilder.Property(ds => ds.DisplayId).IsRequired();
        });
    }
}