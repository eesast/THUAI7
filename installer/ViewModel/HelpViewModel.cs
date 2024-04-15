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
            LauncherHelp.Add(new HelpMessage { Title = "Launcher", Content = "> Playback File非空时为回放模式" });
            LauncherHelp.Add(new HelpMessage { Title = "Launcher", Content = "> 启动操作可能需要管理员权限" });

            OtherHelp.Add(new HelpMessage { Title = "Other", Content = "> 欢迎在比赛群中提出有关比赛的各种问题" });
        }
    }

    public class HelpMessage
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string TitleSize { get; } = "20";
        public string ContentSize { get; } = "16";
    }
}
