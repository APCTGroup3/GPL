using System;
namespace GUI
{

    // Singleton Class only allowing one console window
    public partial class ConsoleWindow : Gtk.Window
    {
        // Do not allow set on instance
        private static readonly ConsoleWindow instance = new ConsoleWindow();



        static ConsoleWindow()
        {

        }

        private ConsoleWindow() : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }
        // Return Console Window instance
        public static ConsoleWindow Instance
        {
            get
            {
                return instance;
            }
        }

    }
}
