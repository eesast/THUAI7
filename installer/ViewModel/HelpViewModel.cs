using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace installer.ViewModel
{
    public class HelpViewModel : BaseViewModel
    {

        public List<HelpMessage> InstallerHelp { get; }
        public List<HelpMessage> LauncherHelp { get; }
        public List<HelpMessage> OtherHelp { get; }

        public HelpViewModel()
        {
            InstallerHelp = new List<HelpMessage>();
            LauncherHelp = new List<HelpMessage>();
            OtherHelp = new List<HelpMessage>();

            InstallerHelp.Add(new HelpMessage { Title = "Installer", Content = "> 下载功能需要选择空文件夹路径" });
            InstallerHelp.Add(new HelpMessage { Title = "Installer", Content = "> 更新前请先进行检查更新操作" });

            LauncherHelp.Add(new HelpMessage { Title = "Launcher", Content = "> 修改参数后需要进行保存操作" });

            OtherHelp.Add(new HelpMessage { Title = "Other", Content = "> 欢迎在比赛群中提出有关比赛的各种问题" });
        }
    }

    public class HelpMessage
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string FontSize { get; } = "18";
    }
}
