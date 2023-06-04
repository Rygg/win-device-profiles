using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.IntegrationTests;

/// <summary>
/// Helper class contains DeviceProfile domain models based on the appsettings.json used in the tests.
/// </summary>
internal static class TestConfiguration
{
    internal static readonly DeviceProfile TestProfile1;
    internal static readonly DeviceProfile TestProfile2;
    internal static readonly DeviceProfile TestProfile3;
    internal static readonly ICollection<DeviceProfile> TestProfiles;

    static TestConfiguration()
    {
        TestProfile1 = new DeviceProfile
        {
            Id = 1,
            Name = "TestProfile1",
            HotKey = new HotKeyCombination
            {
                Key = SupportedKeys.NumPad1,
                Modifiers = SupportedKeyModifiers.Ctrl | SupportedKeyModifiers.Alt
            },
            DisplaySettings = new DisplaySettings[]
            {
                new()
                {
                    DisplayId = 0,
                    PrimaryDisplay = true,
                },
                new()
                {
                    DisplayId = 2,
                    PrimaryDisplay = false,
                    EnableHdr = false,
                    RefreshRate = 120
                }
            }
        };
        TestProfile2 = new DeviceProfile
        {
            Id = 2,
            Name = "TestProfile2",
            HotKey = new HotKeyCombination
            {
                Key = SupportedKeys.NumPad2,
                Modifiers = SupportedKeyModifiers.Ctrl | SupportedKeyModifiers.Alt
            },
            DisplaySettings = new DisplaySettings[]
            {
                new()
                {
                    DisplayId = 2,
                    PrimaryDisplay = true,
                    EnableHdr = true,
                    RefreshRate = 60
                }
            }
        };
        TestProfile3 = new DeviceProfile
        {
            Id = 3,
            Name = "TestProfile3",
            HotKey = new HotKeyCombination
            {
                Key = SupportedKeys.NumPad3,
                Modifiers = SupportedKeyModifiers.Ctrl | SupportedKeyModifiers.Alt
            },
            DisplaySettings = new DisplaySettings[]
            {
                new()
                {
                    DisplayId = 2,
                    PrimaryDisplay = true,
                    EnableHdr = false,
                    RefreshRate = 120
                }
            }
        };

        TestProfiles = new List<DeviceProfile> { TestProfile1, TestProfile2, TestProfile3 };
    }
}