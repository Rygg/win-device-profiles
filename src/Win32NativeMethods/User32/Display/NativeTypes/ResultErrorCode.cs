namespace Win32NativeMethods.User32.Display.NativeTypes
{
	/// <summary>
	/// Enum containing the required result errror codes. Selected the required values from <see href="https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-">here</see>
	/// </summary>
	public enum ResultErrorCode : int
    {
		/// <summary>
		/// The function succeeded.
		/// </summary>
		ERROR_SUCCESS = 0x0,
		/// <summary>
		/// The caller does not have access to the console session. This error occurs if the calling process does not have access to the current desktop or is running on a remote session.
		/// </summary>
		ERROR_ACCESS_DENIED = 0x5,
		/// <summary>
		/// The combination of parameters and flags that are specified is invalid.
		/// </summary>
		ERROR_INVALID_PARAMETER = 0x57,
		/// <summary>
		/// The system is not running a graphics driver that was written according to the Windows Display Driver Model (WDDM). The function is only supported on a system with a WDDM driver running.
		/// </summary>
		ERROR_NOT_SUPPORTED = 0xAE,	
		/// <summary>
		/// An unspecified error occurred. 
		/// </summary>
		ERROR_GEN_FAILURE = 0x1F,
		/// <summary>
		/// The supplied path and mode buffer are too small.
		/// </summary>
		ERROR_INSUFFICIENT_BUFFER = 0x7A,
	}
}
