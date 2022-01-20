# README #

This project is designed to create support for switching between different device profiles for Windows operating systems.

The application runs in the system tray, and profiles can be enabled either by pressing configured hotkey combinations or by clicking the desired profile from the tray icon context menu. The ideal functionality is achieved by setting the application to automatically start up with Windows.

The project is built using [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and it uses the user32.dll of the Win32 API to control the user devices. This project also uses the [NLog](https://nlog-project.org/) library for logging.

## Tools ##
Project is currently developed using [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/vs/).

## Configuration ##
The application is configured by changing the appsettings.json a fairly simple configuration file. The currently available configuration settings are: 

* LogLevel: [(NLog.LogLevel)](https://nlog-project.org/documentation/v4.4.0/html/T_NLog_LogLevel.htm) Sets the log level of the application. The supported values:
    * Trace 
    * Debug
    * Info
    * Warn
    * Error
    * Fatal
    * Off

* ProfilesFile: Application reads the user profiles from this file during application startup.

appsettings.json:
```
{
  "LogLevel": "Info",
  "ProfilesFile": "profiles.json"
}
```

## Profile configuration ##

Profiles are configured in separate json files. Profiles are configured inside an array inside a single "Profiles"-field. Each profile itself contains the following fields:

* Id: Identifier of the profile. Required.
* Name: Name of the profile. Not required.
* HotKey: Keyboard hotkey combination activating this profile. Not required.
* DisplaySettings: Display settings to be set when this profile is activated. Required.

### Id ###
Id is an integer value identifying the profile. Must be unique within the profiles.

### Name ###
Name of the profile shown in the tray menu alongside the setting summary.

### HotKey ###
HotKey configuration for triggering the profile change without accessing the tray icon. The HotKey is configured with following fields:

* Modifiers: An object containing boolean values for each modifier key in the keyboard. If a modifier is set to true, it is required to be pressed alongside the regular hotkey. Supported modifiers:

    * Ctrl
    * Alt
    * Shift
    * Win

* Key: The key to be pressed. Accepts names from the [System.Windows.Forms.Keys](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=windowsdesktop-6.0) enumeration. This is a required value if HotKey field is present in the profile configuration.
```
"HotKey": {
  "Modifiers": {
    "Ctrl": true,
    "Shift": false,
    "Alt": true,
    "Win": false
  },
  "Key": "NumPad1"
```

### DisplaySettings ###
DisplaySetting configuring is configured as an array of DisplaySetting-objects. Each DisplaySetting can contain the following fields:

* DisplayId
* Primary
* HDR
* RefreshRate

```
{
  "DisplayId": 0,
  "Primary": true,
  "HDR": false,
  "RefreshRate": 60
}
```

#### DisplayId ####
Required for every DisplaySetting. Field acts as the identifier of the display. **This Id might be different to the one visible in Windows display settings**.

The proper id can be retrieved by using the "Copy display information to clipboard."-option from the tray icon. The copied value contains a list of every display located in the system, along with their Ids andd other information.

#### Primary ####
A boolean value indicating whether this display should be set as the new primary display. Only one display can be set to primary within a profile. If the field is missing, no changes are made.

#### HDR ####
A boolean value indicating whether or not the Advanced Color State should be enabled for this display. In case the device does not support advanced color states, this should have no effect. If the field is missing, no changes to the advanced color state are made to this device.

#### RefreshRate ####
An integer value indicating the desired RefreshRate of the display. In case the device does not support the refresh rate, this should have no effect. If the field is missing, no changes to refresh rate are made to this device.

### Example of profiles.json

The following example configuration holds 2 different device profiles. Profiles can be triggered by hotkeys Ctrl+Alt+NumPad1 and Ctrl+Alt+NumPad2: 

Profile 1 sets Display 0 as the new primary display, and disables HDR from Display 1 and sets it's refresh rate to 120.

Profile 2 sets Display 1 as the new primary display, enables HDR, and sets its refresh rate to 60.

profiles.json:
```
{
  "Profiles": [
    {
      "Id":  1, 
      "Name": "Profile 1",
      "HotKey": {
        "Modifiers": {
          "Ctrl": true,
          "Shift": false,
          "Alt": true,
          "Win": false
        },
        "Key": "NumPad1"
      },
      "DisplaySettings": [
        {
          "DisplayId": 0,
          "Primary": true
        },
        {
          "DisplayId": 1,
          "Primary": false,
          "HDR": false,
          "RefreshRate": 120
        }
      ]
    },
    {
      "Id": 2, 
      "Name": "Profile 2",
      "HotKey": {
        "Modifiers": {
          "Ctrl": true,
          "Shift": false,
          "Alt": true,
          "Win": false
        },
        "Key": "NumPad2"
      },
      "DisplaySettings": [
        {
          "DisplayId": 1,
          "Primary": true,
          "HDR": true,
          "RefreshRate": 60
        }
      ]
    },
  ]
}
```

## Contribution guidelines ##
TBD
## Future considerations ##
* Support for switching Audio Devices?