using DeviceProfiles.Application.Features.Profiles.Commands.Common;
using DeviceProfiles.Domain.Enums;

namespace DeviceProfiles.Application.UnitTests.Features.Profiles.Commands.Common;

[TestFixture]
public sealed class HotKeyDtoTests
{
    [Test]
    public void IsValid_DefaultObject_ReturnsFalse()
    {
        var options = new HotKeyDto();
        var result = options.IsValid();
        result.Should().BeFalse("default object should not be valid.");
    }

    [Test]
    public void IsValid_ObjectWithNoKeys_ReturnsFalse()
    {
        var options = new HotKeyDto()
        {
            Key = SupportedKeys.None
        };
        var result = options.IsValid();
        result.Should().BeFalse("object should not be valid.");
    }

    [Test]
    public void IsValid_ObjectWithKeyOnly_ReturnsTrue()
    {
        var options = new HotKeyDto
        {
            Key = SupportedKeys.D3
        };
        var result = options.IsValid();
        result.Should().BeTrue("object should be valid.");
    }

    [Test]
    public void IsValid_ObjectWithKeyAndDefaultModifiers_ReturnsTrue()
    {
        var options = new HotKeyDto
        {
            Key = SupportedKeys.D3,
            Modifiers = new ModifierDto()
        };
        var result = options.IsValid();
        result.Should().BeTrue("object should be valid.");
    }

    [Test]
    public void IsValid_ObjectWithKeyAndModifiers_ReturnsTrue()
    {
        var options = new HotKeyDto
        {
            Key = SupportedKeys.D3,
            Modifiers = new ModifierDto
            {
                Ctrl = true,
                Shift = true,
            }
        };
        var result = options.IsValid();
        result.Should().BeTrue("object should be valid.");
    }

    [Test]
    public void ToHotKeyCombination_InvalidObject_ThrowsException()
    {
        var options = new HotKeyDto();
        var act = options.ToHotKeyCombination;
        act.Should().Throw<InvalidOperationException>("object should not be valid.");
    }

    [Test]
    public void ToHotKeyCombination_ValidObject_ProducesCorrectObject()
    {
        var options = new HotKeyDto
        {
            Key = SupportedKeys.D3,
            Modifiers = new ModifierDto
            {
                Ctrl = true,
                Shift = true,
            }
        };
        var result = options.ToHotKeyCombination();
        result.Should().NotBeNull();
        result.Key.Should().Be(SupportedKeys.D3);
        result.Modifiers.Should().HaveFlag(SupportedKeyModifiers.Ctrl);
        result.Modifiers.Should().HaveFlag(SupportedKeyModifiers.Shift);
        result.Modifiers.Should().NotHaveFlag(SupportedKeyModifiers.Alt);
        result.Modifiers.Should().NotHaveFlag(SupportedKeyModifiers.NoRepeat);
        result.Modifiers.Should().NotHaveFlag(SupportedKeyModifiers.Win);
    }
}