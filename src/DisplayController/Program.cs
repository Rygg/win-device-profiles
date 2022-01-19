using System;
using System.Windows.Forms;
using DisplayController.App;

namespace DisplayController
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
            Application.Run(new DisplayControllerApplicationContext()); // TODO: is this properly disposed when exiting?
        }
    }
}
