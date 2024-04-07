using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace installer.Data
{
    public class MD5DataFile
    {
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
        public Version Version { get; set; } = new Version(1, 0, 0, 0);
        public string Description { get; set; }
            = "The Description of the current version.";
        public string BugFixed { get; set; }
            = "Bugs had been fixed.";
        public string BugGenerated { get; set; }
            = "New bugs found in the new version.";
    }
}