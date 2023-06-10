using Application.Features.Profiles.Commands.Common;
using Domain.Enums;

namespace Application.UnitTests.Features.Profiles.Commands.Common;

[TestFixture]
public sealed class ProfilesFileDtoTests // Testing general validation rules for Profile validation.
{
    [Test]
    public void Validate_DefaultDto_ReturnsFalse()
    {
        var profile = new ProfilesFileDto();
        var validationResult = profile.Validate();
        validationResult.Should().BeFalse();
    }

    [Test]
    public void Validate_MissingDisplayIdentifier_ReturnsFalse()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
                    {
                        new()
                        {
                            Primary = true,
                            Hdr = true,
                            RefreshRate = 60,
                        }
                    }
                },
            }
        };
        var validationResult = profile.Validate();
        validationResult.Should().BeFalse("Missing DisplayIdentifier should not be allowed for profiles.");
    }

    [Test]
    public void Validate_MissingProfileId_ReturnsFalse()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
        var validationResult = profile.Validate();
        validationResult.Should().BeFalse("Missing profile identifiers should not be allowed.");
    }

    [Test]
    public void Validate_MissingProfileName_ReturnsFalse()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
        var validationResult = profile.Validate();
        validationResult.Should().BeFalse("Missing profile names should not be allowed.");
    }

    [Test]
    public void Validate_MissingHotKeys_ReturnsTrue()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    DisplaySettings = new List<DisplaySettingsDto>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                            Hdr = true,
                            RefreshRate = 60,
                        }
                    }
                },
            }
        };
        var validationResult = profile.Validate();
        validationResult.Should().BeTrue("Missing HotKeys should be allowed for profiles.");
    }

    [Test]
    public void Validate_MissingDisplaySettings_ReturnsTrue()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                },
            }
        };
        var validationResult = profile.Validate();
        validationResult.Should().BeTrue("Missing DisplaySettings should be allowed for profiles.");
    }

    [Test]
    public void Validate_DuplicateProfileIds_ReturnsFalse()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
        var validationResult = profile.Validate();
        validationResult.Should().BeFalse("Duplicate profile identifiers should not be allowed between profiles.");
    }

    [Test]
    public void Validate_DuplicateHotKeyBindings_ReturnsFalse()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
        var validationResult = profile.Validate();
        validationResult.Should().BeFalse("Duplicate HotKey validations should not be allowed between profiles.");
    }

    [Test]
    public void Validate_DuplicateDisplayIds_ReturnsFalse()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile",
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = false,
                            Alt = true,
                            Win = false
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
                            Hdr = true,
                            RefreshRate = 60,
                        },
                        new()
                        {
                            DisplayId = 1,
                            Primary = false,
                            Hdr = false,
                            RefreshRate = 60,
                        }
                    }

                }
            }
        };
        var validationResult = profile.Validate();
        validationResult.Should().BeFalse("Duplicate DisplayIds should not be allowed within a profile.");
    }

    [Test]
    public void Validate_DuplicatePrimaryDisplays_ReturnsFalse()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        },
                        new()
                        {
                            DisplayId = 1,
                            Primary = true,
                        }
                    }
                }
            }
        };
        var validationResult = profile.Validate();
        validationResult.Should().BeFalse("Duplicate primary displays should not be allowed.");
    }

    [Test]
    public void Validate_ValidDisplaySettingConfigurations_ReturnsTrue()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    DisplaySettings = new List<DisplaySettingsDto>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                            RefreshRate = 60
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    Name = "TestProfile2",
                    DisplaySettings = new List<DisplaySettingsDto>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                            RefreshRate = 120
                        }
                    }
                },
                new()
                {
                    Id = 3,
                    Name = "TestProfile3",
                    DisplaySettings = new List<DisplaySettingsDto>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        },
                        new()
                        {
                            DisplayId = 1,
                            Hdr = false,
                            RefreshRate = 120,
                        },
                    }
                },
                new()
                {
                    Id = 4,
                    Name = "TestProfile4",
                    DisplaySettings = new List<DisplaySettingsDto>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        },
                        new()
                        {
                            DisplayId = 1,
                            Hdr = false,
                            Primary = false,
                            RefreshRate = 120,
                        },
                    }
                },
            }
        };
        var validationResult = profile.Validate();
        validationResult.Should().BeTrue("ProfileSettings should be valid.");
    }

    [Test]
    public void Validate_ValidHotKeyConfigurations_ReturnsTrue()
    {
        var profile = new ProfilesFileDto
        {
            Profiles = new List<DeviceProfileDto>
            {
                new()
                {
                    Id = 1,
                    Name = "TestProfile1",
                    DisplaySettings = new List<DisplaySettingsDto>
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
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
                    HotKey = new HotKeyDto
                    {
                        Key = SupportedKeys.NumPad1,
                        Modifiers = new ModifierDto
                        {
                            Ctrl = true,
                            Shift = true,
                            Alt = false,
                        }
                    },
                    DisplaySettings = new List<DisplaySettingsDto>
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
                    DisplaySettings = new List<DisplaySettingsDto>
                    {
                        new()
                        {
                            DisplayId = 0,
                            Primary = true,
                        },
                        new()
                        {
                            DisplayId = 1,
                            Hdr = false,
                            Primary = false,
                            RefreshRate = 120,
                        },
                    }
                },
            }
        };

        var validationResult = profile.Validate();
        validationResult.Should().BeTrue("ProfileSettings should be valid.");
    }
}