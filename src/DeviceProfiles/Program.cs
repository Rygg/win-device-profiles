using System;
using System.Windows.Forms;
using DeviceProfiles.App;

namespace DeviceProfiles
{
    /// <summary>
    /// DisplayController program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The program starting point.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.Run(new DeviceProfilesApplicationContext()); // TODO: is this properly disposed when exiting?
        }
    }
}
