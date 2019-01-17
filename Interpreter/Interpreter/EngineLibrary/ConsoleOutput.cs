using System;
using System.Collections.Generic;
namespace EngineLibrary
{
    // Singleton class to just handle execution output
    public sealed class ConsoleOutput
    {

        /* -------------------------------------------------- Singleton stuff -------------------------------------------------- */
        private static readonly ConsoleOutput instance = new ConsoleOutput();

        static ConsoleOutput()
        {

        }

        private ConsoleOutput()
        {

        }
        public static ConsoleOutput Instance
        {
            get
            {
                return instance;
            }
        }

        /* --------------------------------------------------------------------------------------------------------------------- */

        // String list containing console output
        private List<string> output = new List<string>();

        public void AddNewLine(string log)
        {
            this.output.Add(log);
        }
        public List<string> GetOutput()
        {
            if(this.output.Count > 0)
            {
                return this.output;
            } else {
                return null;
            }
        }

        public void Clear()
        {
            this.output.Clear();
        }

    }
}
