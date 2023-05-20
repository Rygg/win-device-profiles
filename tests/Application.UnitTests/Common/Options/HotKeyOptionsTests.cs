using Application.Common.Options;
using Domain.Enums;

namespace Application.UnitTests.Common.Options;

[TestFixture]
public sealed class HotKeyOptionsTests
{
    [Test]
    public void IsValid_DefaultObject_ReturnsFalse()
    {
        var options = new HotKeyOptions();
        var result = options.IsValid();
        result.Should().BeFalse("default object should not be valid.");
    }

    [Test]
    public void IsValid_ObjectWithNoKeys_ReturnsFalse()
    {
        var options = new HotKeyOptions
        {
            Key = SupportedKeys.None
        };
        var result = options.IsValid();
        result.Should().BeFalse("object should not be valid.");
    }

    [Test]
    public void IsValid_ObjectWithKeyOnly_ReturnsTrue()
    {
        var options = new HotKeyOptions
        {
            Key = SupportedKeys.D3
        };
        var result = options.IsValid();
        result.Should().BeTrue("object should be valid.");
    }

    [Test]
    public void IsValid_ObjectWithKeyAndDefaultModifiers_ReturnsTrue()
    {
        var options = new HotKeyOptions
        {
            Key = SupportedKeys.D3,
            Modifiers = new ModifierOptions()
        };
        var result = options.IsValid();
        result.Should().BeTrue("object should be valid.");
    }

    [Test]
    public void IsValid_ObjectWithKeyAndModifiers_ReturnsTrue()
    {
        var options = new HotKeyOptions
        {
            Key = SupportedKeys.D3,
            Modifiers = new ModifierOptions
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
        var options = new HotKeyOptions();
        var act = options.ToHotKeyCombination;
        act.Should().Throw<InvalidOperationException>("object should not be valid.");
    }

    [Test]
    public void ToHotKeyCombination_ValidObject_ProducesCorrectObject()
    {
        var options = new HotKeyOptions
        {
            Key = SupportedKeys.D3,
            Modifiers = new ModifierOptions
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