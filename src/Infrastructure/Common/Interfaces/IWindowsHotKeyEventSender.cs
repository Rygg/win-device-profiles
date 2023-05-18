namespace Infrastructure.Common.Interfaces;

public interface IWindowsHotKeyEventSender : IHotKeyEventSender
{
    public nint Handle { get; }
}