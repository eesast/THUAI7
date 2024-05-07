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

            InstallerHelp.AddRange(new[] {
                new HelpMessage
                {
                    Title = "选择安装路径",
                    Content = "> 下载功能需要选择空文件夹路径。"
                },
                new HelpMessage
                {
                    Title = "版本差异",
                    Content = "> 全新安装后请再次检查更新，全新安装的压缩包通常版本较旧。"
                },
                new HelpMessage
                {
                    Title = "更新",
                    Content = "> 更新前请先进行检查更新操作。"
                }
                });

            LauncherHelp.AddRange(new[] {
                new HelpMessage
                {
                    Title = "参数",
                    Content = "> 修改参数后需要进行保存操作，然后才可以启动。"
                },
                new HelpMessage
                {
                    Title = "CPP可执行文件构建",
                    Content = "> 目前的推荐方法是在VS中打开位于CAPI/cpp下的CAPI.sln，然后生成项目。"
                },
                new HelpMessage
                {
                    Title = "Python proto构建",
                    Content = "> 首先请确保电脑已经安装了python和pip，然后执行位于CAPI/python下的generate_proto.cmd(Windows)/generate_proto.sh(Mac/Linux)，等待protos文件夹生成完毕即可。"
                },
                new HelpMessage
                {
                    Title = "特殊权限",
                    Content = "> 启动操作可能需要管理员权限"
                }
                });


            OtherHelp.AddRange(new[] {
                new HelpMessage
                {
                    Title = "Other",
                    Content = "> 欢迎在比赛群中提出有关比赛的各种问题"
                }
                });
        }
    }

    public class HelpMessage
    {
        public HelpMessage(string? t = null, string? c = null)
        {
            Title = t; Content = c;
        }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string TitleSize { get; } = "18";
        public string ContentSize { get; } = "16";
    }
}
