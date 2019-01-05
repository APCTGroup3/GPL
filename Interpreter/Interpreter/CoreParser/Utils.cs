using System;
using System.IO;
using System.Linq;

namespace CoreParser
{
    public static class Utils
    {
        public static string ReadFile(string path)
        {
            string[] lines = File.ReadAllLines(path);
            if (lines.Length == 0)
            {
                return "";
            }
            var fileStr = "";
            for (var i = 0; i < lines.Length - 1; i++)
            {
                fileStr += lines[i] += "\n";
            }
            fileStr += lines.Last();
            return fileStr;
        }
    }
}
