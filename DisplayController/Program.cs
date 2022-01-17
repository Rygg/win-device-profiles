using System;
using System.Windows.Forms;
using DisplayController.App;

namespace DisplayController
{
    /// <summary>
    /// DisplayController program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The program starting point.
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new DisplayControllerApplicationContext()); // TODO: is this properly disposed when exiting?
        }
    }
}
