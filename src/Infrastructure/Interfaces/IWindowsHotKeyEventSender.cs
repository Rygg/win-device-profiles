namespace Infrastructure.Interfaces;

public interface IWindowsHotKeyEventSender : IHotKeyEventSender
{
    public nint Handle { get; }
}