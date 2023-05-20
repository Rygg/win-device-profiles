using Application.Common.Options;
using Domain.Enums;

namespace Application.UnitTests.Common.Options;

[TestFixture]
public sealed class ProfileOptionsTests // Testing general validation rules for Profile validation.
{
    [Test]
    public void Validate_Null_ThrowsArgumentNullException()
    {
        var action = () => ProfileOptions.Validate(null!);
        action.Should().Throw<ArgumentNullException>("Null value should not be allowed.");
    }

    [Test]
    public void Validate_DefaultOptions_ReturnsFalse()
    {
        var profile = new ProfileOptions();
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Default Options should not pass validation.");
    }

    [Test]
    public void Validate_DuplicateDisplayIds_ReturnsFalse()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = false,
                            Alt = true,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        },
                        new()
                        {
                            DisplayId = 0,
                            Primary = false,
                            EnableHdr = true,
                            RefreshRate = 60,
                        },
                        new()
                        {
                            DisplayId = 1,
                            Primary = false,
                            EnableHdr = false,
                            RefreshRate = 60,
                        }
                    }

                }
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Duplicate DisplayIds should not be allowed within a profile.");
    }

    [Test]
    public void Validate_DuplicateHotKeyBindings_ReturnsFalse()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    Name = "TestProfile2",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 1,
                            Primary = false,
                        }
                    }
                },
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Duplicate HotKey validations should not be allowed between profiles.");
    }

    [Test]
    public void Validate_DuplicateProfileIds_ReturnsFalse()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        }
                    }
                },
                new()
                {
                    Id = 1,
                    Name = "TestProfile2",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 1,
                            Primary = false,
                        }
                    }
                },
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Duplicate profile identifiers should not be allowed between profiles.");
    }

    [Test]
    public void Validate_MissingDisplaySettings_ReturnsFalse()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                },
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Missing DisplaySettings should not be allowed for profiles.");
    }

    [Test]
    public void Validate_MissingDisplayIdentifier_ReturnsFalse()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            Primary = true,
                            EnableHdr = true,
                            RefreshRate = 60,
                        }
                    }
                },
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Missing DisplayIdentifier should not be allowed for profiles.");
    }

    [Test]
    public void Validate_MissingHotKeys_ReturnsTrue()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                            EnableHdr = true,
                            RefreshRate = 60,
                        }
                    }
                },
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Missing HotKeys should be allowed for profiles.");
    }

    [Test]
    public void Validate_MissingProfileId_ReturnsFalse()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        }
                    }
                },
                new()
                {
                    Name = "TestProfile2",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 1,
                            Primary = false,
                        }
                    }
                },
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Missing profile identifiers should not be allowed.");
    }

    [Test]
    public void Validate_MissingProfileName_ReturnsFalse()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    Name = "TestProfile2",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 1,
                            Primary = false,
                        }
                    }
                },
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Missing profile names should not be allowed.");
    }

    [Test]
    public void Validate_DuplicatePrimaryDisplays_ReturnsFalse()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    Name = "TestProfile2",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 1,
                            Primary = true,
                        }
                    }
                },
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeFalse("Duplicate primary displays should not be allowed.");
    }

    [Test]
    public void Validate_ValidDisplaySettingConfigurations_ReturnsTrue()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    Name = "TestProfile2",
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        }
                    }
                },
                new()
                {
                    Id = 3,
                    Name = "TestProfile3",
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        },
                        new()
                        {
                            DisplayId = 1,
                            EnableHdr = false,
                            RefreshRate = 120,
                        },
                    }
                },
                new()
                {
                    Id = 4,
                    Name = "TestProfile4",
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        },
                        new()
                        {
                            DisplayId = 1,
                            EnableHdr = false,
                            Primary = false,
                            RefreshRate = 120,
                        },
                    }
                },
            }
        };
        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeTrue("ProfileSettings should be valid.");
    }

    [Test]
    public void Validate_ValidHotKeyConfigurations_ReturnsTrue()
    {
        var profile = new ProfileOptions
        {
            Profiles = new List<DeviceProfileOptions>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    Name = "TestProfile2",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        }
                    }
                },
                new()
                {
                    Id = 3,
                    Name = "TestProfile3",
                    HotKey = new HotKeyOptions
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierOptions
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                        }
                    },
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        }
                    }
                },
                new()
                {
                    Id = 4,
                    Name = "TestProfile4",
                    DisplaySettings = new List<DisplayOptions>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        },
                        new()
                        {
                            DisplayId = 1,
                            EnableHdr = false,
                            Primary = false,
                            RefreshRate = 120,
                        },
                    }
                },
            }
        };

        var validationResult = ProfileOptions.Validate(profile);
        validationResult.Should().BeTrue("ProfileSettings should be valid.");
    }
}