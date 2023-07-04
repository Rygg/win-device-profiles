using DeviceProfiles.Application.Features.Profiles.Commands.Common;

namespace DeviceProfiles.Application.UnitTests.Features.Profiles.Commands.Common;

[TestFixture]
public sealed class DisplaySettingsDtoTests
{
    [Test]
    public void IsValid_DefaultObject_ReturnsFalse()
    {
        var displayOptions = new DisplaySettingsDto();
        var result = displayOptions.IsValid();
        result.Should().BeFalse("default value should not be valid.");
    }

    [Test]
    public void IsValid_OnlyIdentifier_ReturnsFalse()
    {
        var displayOptions = new DisplaySettingsDto
        {
            DisplayId = 0
        };
        var result = displayOptions.IsValid();
        result.Should().BeFalse("object contains no configuration options.");
    }

    [Test]
    public void IsValid_OnlyPrimaryField_ReturnsFalse()
    {
        var displayOptions = new DisplaySettingsDto
        {
            Primary = true,
        };
        var result = displayOptions.IsValid();
        result.Should().BeFalse("object should not be valid.");
    }

    [Test]
    public void IsValid_OnlyEnableHdr_ReturnsFalse()
    {
        var displayOptions = new DisplaySettingsDto
        {
            Hdr = false,
        };
        var result = displayOptions.IsValid();
        result.Should().BeFalse("object should not be valid.");
    }

    [Test]
    public void IsValid_OnlyRefreshRate_ReturnsFalse()
    {
        var displayOptions = new DisplaySettingsDto
        {
            RefreshRate = 120
        };
        var result = displayOptions.IsValid();
        result.Should().BeFalse("object should not be valid.");
    }

    [Test]
    public void IsValid_IdAndPrimaryField_ReturnsTrue()
    {
        var displayOptions = new DisplaySettingsDto
        {
            DisplayId = 1,
            Primary = true,
        };
        var result = displayOptions.IsValid();
        result.Should().BeTrue("object should not valid.");
    }

    [Test]
    public void IsValid_IdAndHdrField_ReturnsTrue()
    {
        var displayOptions = new DisplaySettingsDto
        {
            DisplayId = 1,
            Hdr = true,
        };
        var result = displayOptions.IsValid();
        result.Should().BeTrue("object should not valid.");
    }

    [Test]
    public void IsValid_IdAndRefreshRateField_ReturnsTrue()
    {
        var displayOptions = new DisplaySettingsDto
        {
            DisplayId = 1,
            RefreshRate = 120
        };
        var result = displayOptions.IsValid();
        result.Should().BeTrue("object should not valid.");
    }

    [Test]
    public void IsValid_IdAndPrimaryAndRefreshRateField_ReturnsTrue()
    {
        var displayOptions = new DisplaySettingsDto
        {
            DisplayId = 1,
            Primary = false,
            RefreshRate = 120
        };
        var result = displayOptions.IsValid();
        result.Should().BeTrue("object should not valid.");
    }

    [Test]
    public void IsValid_IdAndPrimaryAndHdrField_ReturnsTrue()
    {
        var displayOptions = new DisplaySettingsDto
        {
            DisplayId = 1,
            Primary = false,
            Hdr = true
        };
        var result = displayOptions.IsValid();
        result.Should().BeTrue("object should not valid.");
    }

    [Test]
    public void IsValid_IdAndRefreshRateAndHdrField_ReturnsTrue()
    {
        var displayOptions = new DisplaySettingsDto
        {
            DisplayId = 1,
            Hdr = true,
            RefreshRate = 60,
        };
        var result = displayOptions.IsValid();
        result.Should().BeTrue("object should not valid.");
    }

    [Test]
    public void IsValid_AllFieldsPresent_ReturnsTrue()
    {
        var displayOptions = new DisplaySettingsDto
        {
            DisplayId = 1,
            Primary = true,
            Hdr = true,
            RefreshRate = 60,
        };
        var result = displayOptions.IsValid();
        result.Should().BeTrue("object should not valid.");
    }

    [Test]
    public void ToDisplaySettings_InvalidConfiguration_ThrowsException()
    {
        var displayOptions = new DisplaySettingsDto();
        var act = displayOptions.ToDisplaySettings;
        act.Should().Throw<InvalidOperationException>("the converted object is not valid.");
    }

    [Test]
    public void ToDisplaySettings_ValidConfigurations_ReturnValidObjects()
    {
        var displayOptions = new DisplaySettingsDto
        {
            DisplayId = 2,
            Primary = true,
        };
        var result = displayOptions.ToDisplaySettings();
        result.Should().NotBeNull();
        result.DisplayId.Should().Be(2);
        result.PrimaryDisplay.Should().BeTrue();
        result.EnableHdr.Should().BeNull();
        result.RefreshRate.Should().BeNull();

        var displayOptions2 = new DisplaySettingsDto
        {
            DisplayId = 1,
            Primary = true,
            Hdr = false,
            RefreshRate = 60,
        };
        var result2 = displayOptions2.ToDisplaySettings();
        result2.Should().NotBeNull();
        result2.DisplayId.Should().Be(1);
        result2.PrimaryDisplay.Should().BeTrue();
        result2.EnableHdr.Should().BeFalse();
        result2.RefreshRate.Should().Be(60);
    }
}