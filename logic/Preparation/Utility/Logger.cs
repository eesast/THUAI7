using System;
using System.IO;

namespace Preparation.Utility
{
    public class Logger
    {
        static public void Writelog(object current, string str)
        {
            string path = "log.txt";
            string log = $"[{DateTime.Now}] {current.GetType()} {current} {str}";
            File.AppendAllText(path, log + Environment.NewLine);
        }
        static public void Writelog(string str)
        {
            string path = "log.txt";
            string log = $"[{DateTime.Now}] {str}";
            File.AppendAllText(path, log + Environment.NewLine);
        }
    }
}
