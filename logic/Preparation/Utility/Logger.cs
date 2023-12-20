using System;
using System.IO;

namespace Preparation.Utility
{
    public class Logger
    {
        static public void Writelog(object current, string str)
        {
            string path = "log.txt";
            string log = string.Format("[{0}] {1} {2} {3}", DateTime.Now, current.GetType(), current.ToString(), str);
            File.AppendAllText(path, log + Environment.NewLine);
        }
        static public void Writelog(string str)
        {
            string path = "log.txt";
            string log = string.Format("[{0}] {1}", DateTime.Now, str);
            File.AppendAllText(path, log + Environment.NewLine);
        }
    }
}
