using System.ComponentModel;
using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Environment.Windows.Services.Keyboard;

/// <summary>
/// Infrastructure for subscribing and receiving hot key values. This has to be registered as a Singleton.
/// </summary>
internal sealed class KeyboardHotKeyService : IHotKeyTrigger, IDisposable
{
    private readonly ILogger<KeyboardHotKeyService> _logger;
    private readonly IWindowsHotKeyEventSender _eventSender;
    private readonly IHotKeyService _hotKeyService;

    private readonly Dictionary<int, HotKeyCombination> _registeredCombinations = new();
    private int _currentKeyRegistrationId;

    private readonly SemaphoreSlim _registrationLock = new(1);

    public KeyboardHotKeyService(
        ILogger<KeyboardHotKeyService> logger, 
        IWindowsHotKeyEventSender eventSender, 
        IHotKeyService hotKeyService)
    {
        _logger = logger;
        _eventSender = eventSender;
        _hotKeyService = hotKeyService;
    }

    /// <summary>
    /// Register a KeyCombination that can be listened to by this implementation.
    /// </summary>
    /// <param name="hotKey">Key combination to register to the listener.</param>
    /// <param name="ct">CancellationToken for the operation.</param>
    /// <exception cref="Win32Exception">Native API caused an exception.</exception>
    /// <exception cref="InvalidOperationException">HotKey could not be registered.</exception>
    public async Task RegisterHotKeyAsync(HotKeyCombination hotKey, CancellationToken ct)
    {
        if (hotKey == null)
        {
            throw new ArgumentNullException(nameof(hotKey));
        }

        if (hotKey.Key == SupportedKeys.None)
        {
            throw new InvalidOperationException("Key not defined.");
        }
        const int timeoutMs = 1000;

        var fsModifiers = (FsModifiers)hotKey.Modifiers;
        var key = (uint)hotKey.Key;

        if (await _registrationLock.WaitAsync(timeoutMs, ct).ConfigureAwait(false))
        {
            try
            {
                var alreadyRegistered = _registeredCombinations.Values.Any(r => (uint)r.Key == key && (int)r.Modifiers == (int)fsModifiers); // Check if key combination is already registered.
                if (alreadyRegistered)
                {
                    _logger.KeyCombinationAlreadyRegistered(hotKey);
                    return;
                }

                _currentKeyRegistrationId++; // Increment the operation counter. Registration starts from 1 -->
                _logger.RegisteringGlobalHotKey(_currentKeyRegistrationId, hotKey);
                if (!_hotKeyService.RegisterHotKeyToHandle(_eventSender.Handle, _currentKeyRegistrationId, fsModifiers, key)) // Register the hot key.
                {
                    throw new Win32Exception($"Key combination {hotKey} could not be registered due to Native Windows error.");
                }

                _registeredCombinations.Add(_currentKeyRegistrationId, hotKey); // Add to registered hot keys.
                _logger.KeyCombinationRegistered(hotKey);
                return;
            }
            finally
            {
                _registrationLock.Release();
            }
        }

        throw new InvalidOperationException($"HotKey {hotKey} could not be registered due to another hot key being registered.");
    }

    /// <summary>
    /// Waits until a registered HotKey is pressed and returns the pressed key combination.
    /// </summary>
    /// <param name="ct">CancellationToken for the operation.</param>
    /// <returns>The registered key combination.</returns>
    public Task<HotKeyCombination> GetHotKeyPressAsync(CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<HotKeyCombination>(); // Create tcs.

        // local function as action for the event.
        void RequestAction(object? sender, HotKeyEventArgs args)
        {
            if(!_registeredCombinations.ContainsKey(args.RegistrationId))
            {
                _logger.ReceivedHotKeyEventNotRegistered(args);
                tcs.TrySetException(new ArgumentOutOfRangeException(nameof(args.RegistrationId),"HotKeyRegistration not found for the received event."));
                return;
            }
            tcs.TrySetResult(_registeredCombinations[args.RegistrationId]);
        }

        _eventSender.OnRegisteredHotKeyPressed += RequestAction; // Subscribe to event.

        var ctRegistration = ct.Register(() => tcs.TrySetCanceled()); // Register the CT cancellation to cancel the Tcs.

        return tcs.Task
            .ContinueWith(
                async t =>
                {
                    await ctRegistration.DisposeAsync().ConfigureAwait(false); // Dispose the ct registration when completed.
                    _eventSender.OnRegisteredHotKeyPressed -= RequestAction; // Remove event listener.
                    return await t.ConfigureAwait(false);
                }, 
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.Current)
            .Unwrap(); // Return unwrapped task.
    }

    /// <summary>
    /// Dispose all registrations.
    /// </summary>
    public void Dispose()
    {
        _logger.StartingDispose();
        foreach (var registration in _registeredCombinations)
        {
            _logger.UnregisterGlobalHotKey(registration.Key, registration.Value);
            _hotKeyService.UnregisterHotKeyFromHandle(_eventSender.Handle, registration.Key);
        }
        _registrationLock.Dispose();
        _logger.DisposingCompleted();
    }
}