using System.ComponentModel;
using Application.Features.HotKeys.Commands;
using Domain.Models;

namespace Application.IntegrationTests.Features.HotKeys.Commands;

public sealed class RegisterHotKeysCommandTests : BaseTestFixture
{
    [Test]
    public async Task Handle_RegistrationSucceeds_RegistersHotKeysFromConfiguration()
    {
        var command = new RegisterHotKeysCommand();
        var act = async () => await SendAsync(command);
        await act.Should().NotThrowAsync("Command should not throw an exception.");

        // Validate the registration calls based on the test configuration.
        var registeredKeys = TestConfiguration.TestProfiles.Select(p => p.HotKey).ToList();
        foreach (var registeredKey in registeredKeys.Where(registeredKey => registeredKey != null))
        {
            hotKeyTriggerMock
                .Verify(m => m.RegisterHotKeyAsync(
                        It.Is<HotKeyCombination>(hotkey => hotkey == registeredKey),
                        It.IsAny<CancellationToken>()),
                    Times.Once
                );
        }
    }

    [Test]
    public async Task Handle_FirstRegistrationFails_ThrowsException()
    {
        hotKeyTriggerMock.Setup(m => m.RegisterHotKeyAsync(It.IsAny<HotKeyCombination>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Win32Exception("Something went wrong with NativeAPI."));

        var command = new RegisterHotKeysCommand();
        var act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<Exception>("Command should throw an exception.");

        // Validate the registration calls based on the test configuration.
        hotKeyTriggerMock
            .Verify(m => m.RegisterHotKeyAsync(
                    It.IsAny<HotKeyCombination>(),
                    It.IsAny<CancellationToken>()),
                Times.Once
            );
    }

    [Test]
    public async Task Handle_ThirdRegistrationFails_ThrowsException()
    {
        hotKeyTriggerMock.SetupSequence(m => m.RegisterHotKeyAsync(It.IsAny<HotKeyCombination>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new Win32Exception("Something went wrong with NativeAPI."));

        var command = new RegisterHotKeysCommand();
        var act = async () => await SendAsync(command);
        await act.Should().ThrowAsync<Exception>("Command should throw an exception.");

        // Validate the registration calls based on the test configuration.
        var registeredKeys = TestConfiguration.TestProfiles.Select(p => p.HotKey).ToList();
        foreach (var registeredKey in registeredKeys.Where(registeredKey => registeredKey != null))
        {
            hotKeyTriggerMock
                .Verify(m => m.RegisterHotKeyAsync(
                        It.Is<HotKeyCombination>(hotkey => hotkey == registeredKey),
                        It.IsAny<CancellationToken>()),
                    Times.Once
                );
        }
    }
}