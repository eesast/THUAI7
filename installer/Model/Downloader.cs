using COSXML.CosException;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Formats.Tar;
using installer.Data;
using installer.Services;

namespace installer.Model
{
    public struct UpdateInfo                                         // 更新信息，包括新版本版本号、更改文件数和新文件数
    {
        public string status;
        public int changedFileCount;
        public int newFileCount;
    }

    public class Downloader
    {
        #region 属性区
        public class UserInfo
        {
            public string _id = "";
            public string email = "";
        }
        public string ProgramName = "THUAI7";               // 要运行或下载的程序名称
        public string StartName = "maintest.exe";           // 启动的程序名
        public Local_Data Data;                             // 本地文件管理器
        public Tencent_Cos Cloud;                           // THUAI7 Cos桶
        public Version CurrentVersion { get => Data.CurrentVersion; set => Data.CurrentVersion = value; }

        public HttpClient Client = new HttpClient();
        public EEsast Web;                                  // EEsast服务器
        public Logger Log;                                  // 日志管理器
        public ListLogger LogList = new ListLogger();
        public enum UpdateStatus
        {
            success, unarchieving, downloading, hash_computing, exiting, error
        }
        public UpdateStatus Status = UpdateStatus.success;                         // 当前工作状态

        public string Route { get; set; }
        public string Username { get => Web.Username; set { Web.Username = value; } }
        public string Password { get => Web.Password; set { Web.Password = value; } }
        public string UserId { get => Web.ID; }
        public string UserEmail { get => Web.Email; }
        public Data.Command Commands
        {
            get => Data.Config.Commands;
            set
            {
                Data.Config.Commands = value;
            }
        }
        public enum UsingOS { Win, Linux, OSX };
        public UsingOS usingOS { get; set; }
        public class Updater
        {
            public string Message = string.Empty;
            public bool Working { get; set; }
            public bool CombatCompleted { get => false; }
            public bool UploadReady { get; set; } = false;
            public bool ProfileAvailable { get; set; }
        }
        public bool LoginFailed { get; set; } = false;
        public bool RememberMe { get => Data.RememberMe; set { Data.RememberMe = value; } }

        public DownloadReport CloudReport { get => Cloud.Report; }
        #endregion

        #region 方法区
        public Downloader()
        {
            Data = new Local_Data();
            Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "Main.log"));
            if (Log.LastRecordTime != DateTime.MinValue && DateTime.Now.Month != Log.LastRecordTime.Month)
            {
                string tardir = Path.Combine(Data.Config.InstallPath, "LogArchieved");
                if (!Directory.Exists(tardir))
                    Directory.CreateDirectory(tardir);
                string tarPath = Path.Combine(tardir, $"Backup-{Log.LastRecordTime.Year}-{Log.LastRecordTime.Month}.tar");
                if (File.Exists(tarPath))
                    File.Delete(tarPath);
                if (File.Exists(tarPath + ".gz"))
                    File.Delete(tarPath + ".gz");
                TarFile.CreateFromDirectory(Data.LogPath, tarPath, false);
                using (FileStream tar = File.Open(tarPath, FileMode.Open))
                using (FileStream gz = File.Create(tarPath + ".gz"))
                using (var compressor = new GZipStream(gz, CompressionMode.Compress))
                {
                    tar.CopyTo(compressor);
                }
                File.Delete(tarPath);
                foreach (var log in Directory.EnumerateFiles(Data.LogPath))
                {
                    File.Delete(log);
                }
            }
            Route = Data.Config.InstallPath;
            Cloud = new Tencent_Cos("1319625962", "ap-beijing", "thuai7",
                LoggerProvider.FromFile(Path.Combine(Data.LogPath, "TencentCos.log")));
            Web = new EEsast(LoggerProvider.FromFile(Path.Combine(Data.LogPath, "EESAST.log")));
            Web.Token_Changed += SaveToken;

            Data.Log.Partner.Add(Log);
            Cloud.Log.Partner.Add(Log);
            Web.Log.Partner.Add(Log);
            Log.Partner.Add(LogList);

            if (Data.Config.Remembered)
            {
                Username = Data.Config.UserName;
                Password = Data.Config.Password;
            }
            Cloud.UpdateSecret(MauiProgram.SecretID, MauiProgram.SecretKey);
        }

        public void UpdateMD5()
        {
            if (File.Exists(Data.MD5DataPath))
                File.Delete(Data.MD5DataPath);
            Log.LogInfo($"正在下载校验文件……");
            Status = UpdateStatus.downloading;
            Log.CountDict[LogLevel.Error] = 0;
            Cloud.DownloadFileAsync(Data.MD5DataPath, "hash.json").Wait();
            if (Log.CountDict[LogLevel.Error] > 0)
            {
                Status = UpdateStatus.error;
                return;
            }
            Data.ReadMD5Data();
            Status = UpdateStatus.success;
        }

        /// <summary>
        /// 全新安装
        /// </summary>
        public void Install(string? path = null)
        {
            Data.Installed = false;
            Data.Config.InstallPath = path ?? Data.Config.InstallPath;
            UpdateMD5();
            if (Status == UpdateStatus.error)
            {
                Log.LogError($"校验文件下载失败，退出安装。");
                return;
            }

            Log.CountDict[LogLevel.Error] = 0;
            Action<DirectoryInfo> action = (dir) => { };
            var deleteTask = (DirectoryInfo dir) =>
            {
                foreach (var file in dir.EnumerateFiles())
                {
                    if (!Local_Data.IsUserFile(file.FullName))
                        file.Delete();
                }
                foreach (var sub in dir.EnumerateDirectories())
                {
                    action(sub);
                }
            };
            action = deleteTask;
            Log.LogWarning($"全新安装开始，所有位于{Data.Config.InstallPath}的文件都将被删除。");
            deleteTask(new DirectoryInfo(Data.Config.InstallPath));

            Data.ResetInstallPath(Data.Config.InstallPath);

            string zp = Path.Combine(Data.Config.InstallPath, "THUAI7.tar.gz");
            Status = UpdateStatus.downloading;
            (CloudReport.Count, CloudReport.ComCount) = (1, 1);
            Log.LogInfo($"正在下载安装包……");
            Cloud.DownloadFileAsync(zp, "THUAI7.tar.gz").Wait();
            Status = UpdateStatus.unarchieving;
            Log.LogInfo($"安装包下载完毕，正在解压……");
            Cloud.ArchieveUnzip(zp, Data.Config.InstallPath);
            Log.LogInfo($"解压完成");
            File.Delete(zp);

            CurrentVersion = Data.FileHashData.Version;
            Log.LogInfo("正在下载选手代码……");
            Status = UpdateStatus.downloading;
            var c = $"{CurrentVersion.Major}_{CurrentVersion.Minor}";
            var tocpp = Cloud.DownloadFileAsync(Path.Combine(Data.Config.InstallPath, "CAPI", "cpp", "API", "src", "AI.cpp"),
                $"./Templates/t.{c}.cpp").Result;
            var topy = Cloud.DownloadFileAsync(Path.Combine(Data.Config.InstallPath, "CAPI", "python", "PyAPI", "AI.py"),
                $"./Templates/t.{c}.py").Result;
            if (tocpp >= 0 && topy >= 0)
            {
                Log.LogInfo("选手代码下载成功！");
            }
            else
            {
                Log.LogError("选手代码下载失败，选手可自行下载，网址：https://github.com/eesast/THUAI7/tree/dev/CAPI/cpp/API/src/AI.cpp，https://github.com/eesast/THUAI7/tree/dev/CAPI/python/PyAPI/AI.py");
            }

            Status = UpdateStatus.hash_computing;
            Log.LogInfo($"正在校验……");
            Data.MD5Update.Clear();
            Data.ScanDir();
            if (Data.MD5Update.Count != 0)
            {
                Status = UpdateStatus.error;
                Log.LogInfo($"校验失败，试图进行升级以修复……");
                Update();
            }
            else
            {
                Status = UpdateStatus.success;
                Log.LogInfo($"安装成功！开始您的THUAI7探索之旅吧！");
                Data.Installed = true;
                if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        Arguments = Data.Config.InstallPath,
                        FileName = "explorer.exe"
                    });
                }
            }
        }

        /// <summary>
        /// 已有安装目录时移动安装目录到其他位置
        /// </summary>
        /// <param name="newPath">新的THUAI7根目录</param>
        public void ResetInstallPath(string newPath)
        {
            newPath = newPath.EndsWith(Path.DirectorySeparatorChar) ? newPath[0..-1] : newPath;
            var installPath = Data.Config.InstallPath.EndsWith(Path.DirectorySeparatorChar) ? Data.Config.InstallPath[0..-1] : Data.Config.InstallPath;
            if (newPath != installPath)
            {
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (Directory.Exists(Path.Combine(newPath, "Logs")))
                {
                    Directory.Delete(Path.Combine(newPath, "Logs"), true);
                }
                Directory.CreateDirectory(Path.Combine(newPath, "Logs"));
                foreach (var f1 in Directory.EnumerateFiles(Path.Combine(installPath, "Logs")))
                {
                    var m = FileService.ConvertAbsToRel(installPath, f1);
                    var n = Path.Combine(newPath, m);
                    File.Move(f1, n);
                }
                if (Cloud.Log is FileLogger) ((FileLogger)Cloud.Log).Path = Path.Combine(newPath, "Logs", "TencentCos.log");
                if (Web.Log is FileLogger) ((FileLogger)Web.Log).Path = Path.Combine(newPath, "Logs", "EESAST.log");
                if (Data.Log is FileLogger) ((FileLogger)Data.Log).Path = Path.Combine(newPath, "Logs", "Local_Data.log");
                if (Log is FileLogger) ((FileLogger)Log).Path = Path.Combine(newPath, "Logs", "Main.log");
                Data.ResetInstallPath(newPath);
            }
            Update();
        }

        /// <summary>
        /// 检测是否需要进行更新
        /// 返回真时则表明需要更新
        /// </summary>
        /// <returns></returns>
        public bool CheckUpdate()
        {
            UpdateMD5();
            Data.MD5Update.Clear();
            Log.LogInfo("校验文件中……");
            Status = UpdateStatus.hash_computing;
            Data.ScanDir(false);
            Status = UpdateStatus.success;
            if (Data.MD5Update.Count != 0 || CurrentVersion < Data.FileHashData.Version)
            {
                Log.LogInfo("需要更新，请点击更新按钮以更新。");
                return true;
            }
            else
            {
                Log.LogInfo("您的版本已经是最新版本！");
                return false;
            }
        }

        /// <summary>
        /// 更新文件
        /// </summary>
        public int Update()
        {
            int result = 0;
            if (CheckUpdate())
            {
                // 如果Major版本号不一致，说明启动器本身需要更新，返回结果为16
                if (CurrentVersion.Major < Data.FileHashData.Version.Major)
                {
                    var dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "THUAI7");
                    var local = Path.Combine(dataDir, "Cache", $"setup_thuai7installer_{Data.FileHashData.Version.Major}.exe");
                    Log.LogWarning("启动器即将升级，正在下载安装程序……");
                    Status = UpdateStatus.downloading;
                    Log.CountDict[LogLevel.Error] = 0;
                    var i = Cloud.DownloadFileAsync(local, $"Setup/{Data.FileHashData.Version.Major}.exe").Result;
                    if (i >= 0)
                    {
                        Log.LogWarning("下载完成，安装程序即将启动，请手动关闭程序，否则本程序将于10s后自动关闭。");
                        Status = UpdateStatus.exiting;
                        Process.Start(local);
                        Task.Run(() =>
                        {
                            var t = DateTime.Now;
                            while ((DateTime.Now - t).TotalSeconds < 10) ;
                            Application.Current?.Quit();
                        });
                        return 16;
                    }
                    else
                    {
                        // 下载失败
                        Log.LogError("启动器下载失败。");
                        return -1;
                    }
                }
                // 如果Minor版本号不一致，说明AI.cpp/AI.py有改动
                // 返回结果为Flags，1: AI.cpp升级；2: AI.py升级
                else if (CurrentVersion.Minor < Data.FileHashData.Version.Minor)
                {
                    var c = $"{CurrentVersion.Major}_{CurrentVersion.Minor}";
                    var v = $"{Data.FileHashData.Version.Major}_{Data.FileHashData.Version.Minor}";
                    Log.LogWarning("检测到选手代码升级，即将下载选手代码模板……");
                    Status = UpdateStatus.downloading;
                    var p = Path.Combine(Data.Config.InstallPath, "Templates");
                    if (!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }
                    Log.CountDict[LogLevel.Error] = 0;
                    var tocpp = Cloud.DownloadFileAsync(Path.Combine(p, $"t.{c}.cpp"),
                        $"./Templates/t.{c}.cpp");
                    var topy = Cloud.DownloadFileAsync(Path.Combine(p, $"t.{c}.py"),
                        $"./Templates/t.{c}.py");
                    var tncpp = Cloud.DownloadFileAsync(Path.Combine(p, $"t.{v}.cpp"),
                        $"./Templates/t.{v}.cpp");
                    var tnpy = Cloud.DownloadFileAsync(Path.Combine(p, $"t.{v}.py"),
                        $"./Templates/t.{v}.py");
                    Task.WaitAll(tocpp, topy, tncpp, tnpy);
                    if (tocpp.Result >= 0 && topy.Result >= 0 && tncpp.Result >= 0 && tnpy.Result >= 0)
                    {
                        Log.LogWarning("下载完毕，即将合并模板与用户代码，结果可能出现问题，请务必确认正确后用同名temp文件覆盖源文件");
                        if (Data.LangEnabled[LanguageOption.cpp].Item1)
                        {
                            var so = FileService.ReadToEnd(Path.Combine(p, $"t.{c}.cpp"));
                            var sn = FileService.ReadToEnd(Path.Combine(p, $"t.{v}.cpp"));
                            var sa = FileService.ReadToEnd(Data.LangEnabled[LanguageOption.cpp].Item2);
                            var s = FileService.MergeUserCode(sa, so, sn);
                            using (var f = new FileStream(Data.LangEnabled[LanguageOption.cpp].Item2 + ".temp", FileMode.Create))
                            using (var w = new StreamWriter(f))
                            {
                                w.Write(s);
                                w.Flush();
                            }
                            result |= 1;
                            if (DeviceInfo.Platform == DevicePlatform.WinUI)
                            {
                                Process.Start(new ProcessStartInfo()
                                {
                                    Arguments = Directory.GetParent(Data.LangEnabled[LanguageOption.cpp].Item2)?.FullName,
                                    FileName = "explorer.exe"
                                });
                            }
                        }
                        if (Data.LangEnabled[LanguageOption.python].Item1)
                        {
                            var so = FileService.ReadToEnd(Path.Combine(p, $"t.{c}.py"));
                            var sn = FileService.ReadToEnd(Path.Combine(p, $"t.{v}.py"));
                            var sa = FileService.ReadToEnd(Data.LangEnabled[LanguageOption.python].Item2);
                            var s = FileService.MergeUserCode(sa, so, sn);
                            using (var f = new FileStream(Data.LangEnabled[LanguageOption.python].Item2 + ".temp", FileMode.Create))
                            using (var w = new StreamWriter(f))
                            {
                                w.Write(s);
                                w.Flush();
                            }
                            result |= 2;
                            if (DeviceInfo.Platform == DevicePlatform.WinUI)
                            {
                                Process.Start(new ProcessStartInfo()
                                {
                                    Arguments = Directory.GetParent(Data.LangEnabled[LanguageOption.python].Item2)?.FullName,
                                    FileName = "explorer.exe"
                                });
                            }
                        }
                    }
                }
                Log.CountDict[LogLevel.Error] = 0;

                // 后两位留给其他更新，更新成功后返回值Flags增加0x8
                Status = UpdateStatus.downloading;
                Log.LogInfo("正在更新……");
                Cloud.DownloadQueueAsync(Data.Config.InstallPath,
                    from item in Data.MD5Update where item.state != System.Data.DataRowState.Added select item.name).Wait();
                Log.LogWarning("正在删除冗余文件……");
                foreach (var item in Data.MD5Update.Where((s) => s.state == System.Data.DataRowState.Added))
                {
                    var _file = item.name;
                    var file = _file.StartsWith('.') ?
                        Path.Combine(Data.Config.InstallPath, _file) : _file;
                    File.Delete(file);
                }
                if (Log.CountDict[LogLevel.Error] == 0)
                {
                    Data.MD5Update.Clear();
                    CurrentVersion = Data.FileHashData.Version;
                    Status = UpdateStatus.hash_computing;
                    Log.LogInfo("正在校验……");
                    if (!CheckUpdate())
                    {
                        Log.LogInfo("更新成功！");
                        Status = UpdateStatus.success;
                        Data.Installed = true;
                        result |= 8;
                        return result;
                    }
                }
            }
            else
            {
                Log.LogInfo("已经是最新版本啦！");
                Data.Installed = true;
                Status = UpdateStatus.success;
                return 0;
            }
            Log.LogError("更新出问题了 -_-b");
            Status = UpdateStatus.error;
            return -1;
        }

        /// <summary>
        /// 登录到EEsast
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task Login(string username = "", string password = "")
        {
            Username = string.IsNullOrEmpty(username) ? Username : username;
            Password = string.IsNullOrEmpty(password) ? Password : password;
            await Web.LoginToEEsast(Client, Username, Password);
        }

        /// <summary>
        /// 存储EEsast身份标识（修改Token时自动触发）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SaveToken(object? sender, EventArgs args)
        {
            Data.Config.Token = Web.Token;
        }

        public void RememberUser()
        {
            Data.Config.UserName = Username;
            Data.Config.Password = Password;
            Data.Config.Remembered = true;
        }

        public void ForgetUser()
        {
            Data.Config.UserName = string.Empty;
            Data.Config.Password = string.Empty;
            Data.Config.Remembered = false;
        }

        /// <summary>
        /// 上传选手代码
        /// </summary>
        /// <param name="player_id">对应玩家id</param>
        public void UploadFiles(int player_id)
        {
            string lang;
            switch (Commands.Language)
            {
                case LanguageOption.cpp:
                    lang = "cpp";
                    break;
                case LanguageOption.python:
                    lang = "python";
                    break;
                default:
                    lang = "unknown";
                    break;
            }
            Web.UploadFiles(Client, Data.LangEnabled[Commands.Language].Item2, lang, $"player_{player_id}").Wait();
        }
        #endregion
    }
}