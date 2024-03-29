﻿using Application.Common.Options;
using Domain.Enums;

namespace Application.UnitTests.Common.Options;

[TestFixture]
public sealed class DeviceProfileOptionsTests
{
    [Test]
    public void IsValid_DefaultObject_ReturnsFalse()
    {
        var options = new DeviceProfileOptions();
        var result = options.IsValid();
        result.Should().BeFalse("default object should not be valid.");
    }

    [Test]
    public void IsValid_NoIdentifier_ReturnsFalse()
    {
        var options = new DeviceProfileOptions
        {
            Id = null,
            Name = "TestProfile",
            HotKey = new HotKeyOptions
            {
                Key = SupportedKeys.D1
            },
            DisplaySettings = new List<DisplayOptions>
            {
                new()
                {
                    DisplayId = 0,
                    Hdr = true,
                    Primary = true,
                    RefreshRate = 60
                }
            }
        };
        var result = options.IsValid();
        result.Should().BeFalse("object should not be valid.");
    }

    [Test]
    public void IsValid_NoName_ReturnsFalse()
    {
        var options = new DeviceProfileOptions
        {
            Id = 0,
            Name = null,
            HotKey = new HotKeyOptions
            {
                Key = SupportedKeys.D1
            },
            DisplaySettings = new List<DisplayOptions>
            {
                new()
                {
                    DisplayId = 0,
                    Hdr = true,
                    Primary = true,
                    RefreshRate = 60
                }
            }
        };
        var result = options.IsValid();
        result.Should().BeFalse("object should not be valid.");
    }

    [Test]
    public void IsValid_NoHotKeys_ReturnsTrue()
    {
        var options = new DeviceProfileOptions
        {
            Id = 0,
            Name = "TestProfile",
            DisplaySettings = new List<DisplayOptions>
            {
                new()
                {
                    DisplayId = 0,
                    Hdr = true,
                    Primary = true,
                    RefreshRate = 60
                }
            }
        };
        var result = options.IsValid();
        result.Should().BeTrue("object should be valid.");
    }

    [Test]
    public void IsValid_NoDisplaySettings_ReturnsTrue()
    {
        var options = new DeviceProfileOptions
        {
            Id = 0,
            Name = "TestProfile",
            HotKey = new HotKeyOptions
            {
                Key = SupportedKeys.D1
            }
        };
        var result = options.IsValid();
        result.Should().BeTrue("object should be valid.");
    }

    [Test]
    public void ToDeviceProfile_InvalidProfileOptions_ReturnsCorrectObject()
    {
        var options = new DeviceProfileOptions();
        var act = options.ToDeviceProfile;
        act.Should().Throw<InvalidOperationException>("options should not be valid.");
    }

    [Test]
    public void ToDeviceProfile_ValidDeviceProfileOptions_ReturnsCorrectObject()
    {
        var options = new DeviceProfileOptions
        {
            Id = 2,
            Name = "TestProfile",
            HotKey = new HotKeyOptions
            {
                Key = SupportedKeys.D1
            },
            DisplaySettings = new List<DisplayOptions>
            {
                new()
                {
                    DisplayId = 0,
                    Hdr = true,
                    Primary = true,
                    RefreshRate = 60
                },
                new()
                {
                    DisplayId = 1,
                    Hdr = true,
                    Primary = false,
                    RefreshRate = 120
                }
            }
        };
        var result = options.ToDeviceProfile();
        result.Id.Should().Be(2);
        result.Name.Should().Be("TestProfile");
        result.HotKey.Should().NotBeNull();
        result.HotKey!.Key.Should().Be(SupportedKeys.D1);
        result.DisplaySettings.Should().NotBeEmpty();
        result.DisplaySettings.Should().HaveCount(2);
        result.DisplaySettings
            .Should()
            .Contain(ds => 
                ds.DisplayId == 0 
                && ds.PrimaryDisplay == true 
                && ds.EnableHdr == true 
                && ds.RefreshRate == 60
            );
        result.DisplaySettings
            .Should()
            .Contain(ds =>
                ds.DisplayId == 1
                && ds.PrimaryDisplay == false
                && ds.EnableHdr == true
                && ds.RefreshRate == 120
        );
    }

    [Test]
    public void ToDeviceProfile_ValidDeviceProfileOptionsWithoutHotKey_ReturnsCorrectObject()
    {
        var options = new DeviceProfileOptions
        {
            Id = 2,
            Name = "TestProfile",
            DisplaySettings = new List<DisplayOptions>
            {
                new()
                {
                    DisplayId = 0,
                    Hdr = true,
                    Primary = true,
                    RefreshRate = 60
                }
            }
        };
        var result = options.ToDeviceProfile();
        result.Id.Should().Be(2);
        result.Name.Should().Be("TestProfile");
        result.HotKey.Should().BeNull();
        result.DisplaySettings.Should().NotBeEmpty();
        result.DisplaySettings.Should().HaveCount(1);
        result.DisplaySettings
            .Should()
            .Contain(ds =>
                ds.DisplayId == 0
                && ds.PrimaryDisplay == true
                && ds.EnableHdr == true
                && ds.RefreshRate == 60
            );
    }

    [Test]
    public void ToDeviceProfile_ValidDeviceProfileOptionsWithoutDisplaySettings_ReturnsCorrectObject()
    {
        var options = new DeviceProfileOptions
        {
            Id = 2,
            Name = "TestProfile",
            HotKey = new HotKeyOptions
            {
                Key = SupportedKeys.D2
            },
        };
        var result = options.ToDeviceProfile();
        result.Id.Should().Be(2);
        result.Name.Should().Be("TestProfile");
        result.HotKey.Should().NotBeNull();
        result.HotKey!.Key.Should().Be(SupportedKeys.D2);
        result.DisplaySettings.Should().BeEmpty();
        
    }
}