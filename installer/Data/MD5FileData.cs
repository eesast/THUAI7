using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace installer.Data
{
    public class MD5DataFile
    {
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
        [Obsolete]
        public Version Version { get; set; } = new Version(1, 0, 0, 0);
        [JsonInclude]
        public TVersion TVersion { get; set; } = new TVersion();
        public string Description { get; set; }
            = "The Description of the current version.";
        public string BugFixed { get; set; }
            = "Bugs had been fixed.";
        public string BugGenerated { get; set; }
            = "New bugs found in the new version.";
    }

    public class TVersion
    {
        // 代码库版本
        [JsonInclude]
        public Version LibVersion = new Version(1, 0, 2, 3);
        // 选手代码模板版本
        [JsonInclude]
        public Version TemplateVersion = new Version(1, 0, 0, 3);
        // 本体版本
        [JsonInclude]
        public Version InstallerVersion = new Version(1, 1, 0, 2);
        public static bool operator <(TVersion l, TVersion r)
        {
            return l.LibVersion < r.LibVersion || l.TemplateVersion < r.TemplateVersion || l.InstallerVersion < r.InstallerVersion;
        }
        public static bool operator >(TVersion l, TVersion r)
        {
            return l.LibVersion > r.LibVersion || l.TemplateVersion > r.TemplateVersion || l.InstallerVersion > r.InstallerVersion;
        }
    }
}