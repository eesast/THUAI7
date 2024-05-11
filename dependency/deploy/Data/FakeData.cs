using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace installer.Data
{
    public enum DevicePlatform
    {
        WinUI
    }
    public static class DeviceInfo
    {
        public static DevicePlatform Platform { get; set; } = DevicePlatform.WinUI;
    }
}
