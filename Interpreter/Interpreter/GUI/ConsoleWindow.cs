using System;
namespace GUI
{
    public partial class ConsoleWindow : Gtk.Window
    {

        private static readonly ConsoleWindow instance = new ConsoleWindow();

        static ConsoleWindow()
        {

        }

        private ConsoleWindow() : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }
        public static ConsoleWindow Instance
        {
            get
            {
                return instance;
            }
        }

    }
}
