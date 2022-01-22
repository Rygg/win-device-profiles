using System.Collections.Generic;
using DeviceProfiles.Classes;
using DeviceProfiles.Configuration;
using NUnit.Framework;

namespace DeviceProfiles.Tests.ProfileTests
{
    /// <summary>
    /// Unit tests for loading profile configurations from json-files.
    /// </summary>
    public class ProfileConfigurationTests
    {
        /// <summary>
        /// Enumeration for test file types.
        /// </summary>
        private enum ETestProfileType
        {
            EmptyString,
            NoFile,
            NoProfiles,
            DuplicateProfileIds,
            DuplicateDisplayIds,
            MissingProfileId,
            MissingProfileName,
            MissingHotKeys,
            MissingDisplaySettings,
            ValidHotKeyConfigurations,
            InvalidHotKeyConfigurations,
            InvalidDisplaySettingConfigurations,
            MissingDisplayId,
            MultiplePrimaryDisplays,
            ValidDisplaySettingConfigurations,
            ValidProfileFile1,
            ValidProfileFile2,
            ValidProfileFile3,
            ValidProfileFile4,
            DuplicateHotKeyBindings
        }
        /// <summary>
        /// Contain all the test files name.
        /// </summary>
        private readonly Dictionary<ETestProfileType, string> _fileNames = new();

        /// <summary>
        /// Sets up the required test information.
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Set up file names.
            _fileNames.Add(ETestProfileType.EmptyString, string.Empty);
            _fileNames.Add(ETestProfileType.NoFile, "FileWillNotBeFound");
            _fileNames.Add(ETestProfileType.NoProfiles, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\noProfiles.json");
            _fileNames.Add(ETestProfileType.DuplicateProfileIds, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\duplicateProfileIds.json");
            _fileNames.Add(ETestProfileType.DuplicateDisplayIds, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\duplicateDisplayIds.json");
            _fileNames.Add(ETestProfileType.MissingProfileId, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\missingProfileId.json");
            _fileNames.Add(ETestProfileType.MissingProfileName, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\missingProfileName.json");
            _fileNames.Add(ETestProfileType.MissingHotKeys, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\missingHotKeys.json");
            _fileNames.Add(ETestProfileType.MissingDisplaySettings, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\missingDisplaySettings.json");
            _fileNames.Add(ETestProfileType.InvalidHotKeyConfigurations, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\invalidHotKeyConfigurations.json");
            _fileNames.Add(ETestProfileType.DuplicateHotKeyBindings, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\duplicateHotKeyBindings.json");
            _fileNames.Add(ETestProfileType.InvalidDisplaySettingConfigurations, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\invalidDisplaySettingConfigurations.json");
            _fileNames.Add(ETestProfileType.MissingDisplayId, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\missingDisplayId.json");
            _fileNames.Add(ETestProfileType.MultiplePrimaryDisplays, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\multiplePrimaryDisplays.json");
            _fileNames.Add(ETestProfileType.ValidHotKeyConfigurations, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\validHotKeyConfigurations.json");
            _fileNames.Add(ETestProfileType.ValidDisplaySettingConfigurations, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\validDisplaySettingConfigurations.json");
            _fileNames.Add(ETestProfileType.ValidProfileFile1, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\validProfile1.json");
            _fileNames.Add(ETestProfileType.ValidProfileFile2, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\validProfile2.json");
            _fileNames.Add(ETestProfileType.ValidProfileFile3, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\validProfile3.json");
            _fileNames.Add(ETestProfileType.ValidProfileFile4, $"{TestContext.CurrentContext.WorkDirectory}\\Files\\Profiles\\validProfile4.json");
        }

        /// <summary>
        /// Test loading with invalid files.
        /// </summary>
        [Test]
        public void ReadProfileConfiguration_NoProfileFilePresent_SetsEmptyProfiles()
        {
            var profiles = LoadProfilesFromFile(ETestProfileType.EmptyString);
            Assert.IsEmpty(profiles, "Profiles were set when no file was given");

            profiles = LoadProfilesFromFile(ETestProfileType.NoFile);
            Assert.IsEmpty(profiles, "Profiles were set when an invalid filename was given");
        }

        /// <summary>
        /// Tests loading an empty profile file.
        /// </summary>
        [Test]
        public void ReadProfileConfiguration_NoProfilesInProfileFile_SetsEmptyProfiles()
        {
            var profiles = LoadProfilesFromFile(ETestProfileType.NoProfiles);
            Assert.IsEmpty(profiles, "Profiles were set when no profiles were given");
        }

        /// <summary>
        /// Tests that duplicate ids are not accepted.
        /// </summary>
        [Test]
        public void ReadProfileConfiguration_DuplicateIdsInProfileFile_SetsEmptyProfiles()
        {
            var profiles = LoadProfilesFromFile(ETestProfileType.DuplicateProfileIds);
            Assert.IsEmpty(profiles, "Profiles were set when duplicate profiles were configured.");

            profiles = LoadProfilesFromFile(ETestProfileType.DuplicateDisplayIds);
            Assert.IsEmpty(profiles, "Profiles were set when duplicate displays were configured.");
        }

        /// <summary>
        /// Tests that profiles with missing mandatory fields are not accepted.
        /// </summary>
        [Test]
        public void ReadProfileConfiguration_MissingMandatoryFieldsInProfileFile_SetsEmptyProfiles()
        {
            var profiles = LoadProfilesFromFile(ETestProfileType.MissingProfileId);
            Assert.IsEmpty(profiles, "Profiles were set when profile id was missing.");

            profiles = LoadProfilesFromFile(ETestProfileType.MissingDisplaySettings);
            Assert.IsEmpty(profiles, "Profiles were set when display settings were missing.");
        }

        /// <summary>
        /// Tests that profiles with missing optional fields are accepted.
        /// </summary>
        [Test]
        public void ReadProfileConfiguration_MissingOptionalFieldsInProfileFile_SetsProfiles()
        {
            var profiles = LoadProfilesFromFile(ETestProfileType.MissingProfileName);
            Assert.IsNotEmpty(profiles, "Profiles were not set when profile name was missing.");
            Assert.AreEqual(2, profiles.Length, "Incorrect amount of profiles set.");

            profiles = LoadProfilesFromFile(ETestProfileType.MissingHotKeys);
            Assert.IsNotEmpty(profiles, "Profiles were not set when hotkey was missing.");
            Assert.AreEqual(2, profiles.Length, "Incorrect amount of profiles set.");
        }

        /// <summary>
        /// Tests that profiles with invalid field contents are not supported.
        /// </summary>
        [Test]
        public void ReadProfileConfiguration_InvalidObjectFieldsInProfileFile_SetsEmptyProfiles()
        {
            var profiles = LoadProfilesFromFile(ETestProfileType.InvalidDisplaySettingConfigurations);
            Assert.IsEmpty(profiles, "Profiles were set with invalid display settings.");

            profiles = LoadProfilesFromFile(ETestProfileType.MissingDisplayId);
            Assert.IsEmpty(profiles, "Profiles were set with invalid display settings (missing displayId).");

            profiles = LoadProfilesFromFile(ETestProfileType.MultiplePrimaryDisplays);
            Assert.IsEmpty(profiles, "Profiles were set with invalid display settings (multiple primary displays).");

            profiles = LoadProfilesFromFile(ETestProfileType.InvalidHotKeyConfigurations);
            Assert.IsEmpty(profiles, "Profiles were set with invalid hotkey field.");

            profiles = LoadProfilesFromFile(ETestProfileType.DuplicateHotKeyBindings);
            Assert.IsEmpty(profiles, "Profiles were set with invalid hotkey field (Duplicate hot keys).");
        }

        /// <summary>
        /// Tests that profiles with all allowed hotkey combinations are are accepted.
        /// </summary>
        [Test]
        public void ReadProfileConfiguration_ValidHotKeyConfigurationsInProfileFile_SetsProfiles()
        {
            var profiles = LoadProfilesFromFile(ETestProfileType.ValidHotKeyConfigurations);
            Assert.IsNotEmpty(profiles, "Profiles were not set when testing different hot key configurations.");
            Assert.AreEqual(4, profiles.Length, "Incorrect amount of profiles set.");
        }

        /// <summary>
        /// Tests that profiles with different display setting combinations are are accepted.
        /// </summary>
        [Test]
        public void ReadProfileConfiguration_ValidDisplaySettingConfigurationsInProfileFile_SetsProfiles()
        {
            var profiles = LoadProfilesFromFile(ETestProfileType.ValidDisplaySettingConfigurations);
            Assert.IsNotEmpty(profiles, "Profiles were not set when testing different display setting configurations.");
            Assert.AreEqual(4, profiles.Length, "Incorrect amount of profiles set.");
        }

        /// <summary>
        /// Test numerous different valid profile files.
        /// </summary>
        [Test]
        public void ReadProfileConfiguration_ValidProfilesInProfileFile_SetsProfiles()
        {
            var profiles = LoadProfilesFromFile(ETestProfileType.ValidProfileFile1);
            Assert.IsNotEmpty(profiles, $"Profiles were not set when testing valid profile configurations. TestFile: {ETestProfileType.ValidProfileFile1}");
            Assert.AreEqual(3, profiles.Length, "Incorrect amount of profiles set.");

            profiles = LoadProfilesFromFile(ETestProfileType.ValidProfileFile2);
            Assert.IsNotEmpty(profiles, $"Profiles were not set when testing valid profile configurations. TestFile: {ETestProfileType.ValidProfileFile2}");
            Assert.AreEqual(3, profiles.Length, "Incorrect amount of profiles set.");

            profiles = LoadProfilesFromFile(ETestProfileType.ValidProfileFile3);
            Assert.IsNotEmpty(profiles, $"Profiles were not set when testing valid profile configurations. TestFile: {ETestProfileType.ValidProfileFile3}");
            Assert.AreEqual(3, profiles.Length, "Incorrect amount of profiles set.");

            profiles = LoadProfilesFromFile(ETestProfileType.ValidProfileFile4);
            Assert.IsNotEmpty(profiles, $"Profiles were not set when testing valid profile configurations. TestFile: {ETestProfileType.ValidProfileFile4}");
            Assert.AreEqual(4, profiles.Length, "Incorrect amount of profiles set.");
        }

        private DeviceProfile[] LoadProfilesFromFile(ETestProfileType fileType)
        {
            DeviceProfile[]? profiles = null;
            Assert.DoesNotThrow(() =>
            {
                profiles = ProfileConfiguration.GetConfiguredDeviceProfiles(_fileNames[fileType]); ;
            }, "Exception thrown from GetConfiguredDeviceProfiles");

            Assert.IsNotNull(profiles, "Profiles not set by GetConfiguredDeviceProfiles");
            return profiles!;
        }
    }
}