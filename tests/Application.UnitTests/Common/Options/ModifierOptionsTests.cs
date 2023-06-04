using Application.Common.Options;
using Domain.Enums;

namespace Application.UnitTests.Common.Options;

[TestFixture]
public sealed class ModifierOptionsTests
{
    [Test]
    public void ToSupportedModifiers_NoModifiers_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = false,
            Ctrl = false,
            Shift = false,
            Win = false,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().Be(SupportedKeyModifiers.None);
    }

    [Test]
    public void ToSupportedModifiers_AltModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = true,
            Ctrl = false,
            Shift = false,
            Win = false,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Alt);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Win);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Shift);
    }

    [Test]
    public void ToSupportedModifiers_CtrlModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = false,
            Ctrl = true,
            Shift = false,
            Win = false,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Alt);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Win);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Shift);
    }

    [Test]
    public void ToSupportedModifiers_ShiftModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = false,
            Ctrl = false,
            Shift = true,
            Win = false,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Shift);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Alt);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_WinModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = false,
            Ctrl = false,
            Shift = false,
            Win = true,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Win);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Alt);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Shift);
    }

    [Test]
    public void ToSupportedModifiers_AltAndWinModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = true,
            Ctrl = false,
            Shift = false,
            Win = true,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().NotHaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Shift);
        result.Should().HaveFlag(SupportedKeyModifiers.Alt);
        result.Should().HaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_AltAndShiftModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = true,
            Ctrl = false,
            Shift = true,
            Win = false,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().NotHaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().HaveFlag(SupportedKeyModifiers.Shift);
        result.Should().HaveFlag(SupportedKeyModifiers.Alt);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_AltAndCtrlModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = true,
            Ctrl = true,
            Shift = false,
            Win = false,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Shift);
        result.Should().HaveFlag(SupportedKeyModifiers.Alt);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_CtrlAndShiftModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = false,
            Ctrl = true,
            Shift = true,
            Win = false,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().HaveFlag(SupportedKeyModifiers.Shift);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Alt);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_CtrlAndWinModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = false,
            Ctrl = true,
            Shift = false,
            Win = true,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Shift);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Alt);
        result.Should().HaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_ShiftAndWinModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = false,
            Ctrl = false,
            Shift = true,
            Win = true,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().NotHaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().HaveFlag(SupportedKeyModifiers.Shift);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Alt);
        result.Should().HaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_CtrlAndShiftAndWinModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = false,
            Ctrl = true,
            Shift = true,
            Win = true,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().HaveFlag(SupportedKeyModifiers.Shift);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Alt);
        result.Should().HaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_AltAndShiftAndWinModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = true,
            Ctrl = false,
            Shift = true,
            Win = true,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().NotHaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().HaveFlag(SupportedKeyModifiers.Shift);
        result.Should().HaveFlag(SupportedKeyModifiers.Alt);
        result.Should().HaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_AltAndShiftAndCtrlModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = true,
            Ctrl = true,
            Shift = false,
            Win = true,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Shift);
        result.Should().HaveFlag(SupportedKeyModifiers.Alt);
        result.Should().HaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_AltAndCtrlAndShiftModifier_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = true,
            Ctrl = true,
            Shift = true,
            Win = false,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().HaveFlag(SupportedKeyModifiers.Shift);
        result.Should().HaveFlag(SupportedKeyModifiers.Alt);
        result.Should().NotHaveFlag(SupportedKeyModifiers.Win);
    }

    [Test]
    public void ToSupportedModifiers_AllModifiers_ProducesCorrectResults()
    {
        var options = new ModifierOptions
        {
            Alt = true,
            Ctrl = true,
            Shift = true,
            Win = true,
        };
        var result = options.ToSupportedKeyModifiers();
        result.Should().HaveFlag(SupportedKeyModifiers.Ctrl);
        result.Should().HaveFlag(SupportedKeyModifiers.Shift);
        result.Should().HaveFlag(SupportedKeyModifiers.Alt);
        result.Should().HaveFlag(SupportedKeyModifiers.Win);
    }
}