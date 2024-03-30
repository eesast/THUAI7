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
        public Logger Log;                               // 日志管理器
        public Logger LogError;
        public enum UpdateStatus
        {
            success, unarchieving, downloading, hash_computing, error
        }
        public UpdateStatus Status = UpdateStatus.success;                         // 当前工作状态

        public ConcurrentQueue<string> downloadFailed = new ConcurrentQueue<string>();
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
        public ExceptionStack Exceptions;
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

        #endregion

        #region 方法区
        public Downloader()
        {
            Data = new Local_Data();
            Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "Main.log"));
            LogError = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "Main.error.log"));
            if ((Log.LastRecordTime != DateTime.MinValue && DateTime.Now.Month != Log.LastRecordTime.Month)
                || (LogError.LastRecordTime != DateTime.MinValue && DateTime.Now.Month != LogError.LastRecordTime.Month))
            {
                string tardir = Path.Combine(Data.Config.InstallPath, "LogArchieved");
                if (!Directory.Exists(tardir))
                    Directory.CreateDirectory(tardir);
                string tarPath = Path.Combine(tardir, $"Backup-{Log.LastRecordTime.Year}-{Log.LastRecordTime.Month}.tar");
                if (File.Exists(tarPath))
                    File.Delete(tarPath);
                if (File.Exists(tarPath + ".gz"))
                    File.Delete(tarPath + ".gz");
                Data.Log.Dispose();
                Data.LogError.Dispose();
                Data.Exceptions.logger.Dispose();
                Log.Dispose();
                LogError.Dispose();
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
                Data.Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "LocalData.log"));
                Data.LogError = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "LocalData.error.log"));
                Data.Exceptions = new ExceptionStack(Data.LogError);
                Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "Main.log"));
                LogError = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "Main.error.log"));
            }
            Exceptions = new ExceptionStack(LogError, this);
            Route = Data.Config.InstallPath;
            Cloud = new Tencent_Cos("1319625962", "ap-beijing", "bucket1",
                LoggerProvider.FromFile(Path.Combine(Data.LogPath, "TencentCos.log")),
                LoggerProvider.FromFile(Path.Combine(Data.LogPath, "TencentCos.error.log")));
            Web = new EEsast(LoggerProvider.FromFile(Path.Combine(Data.LogPath, "EESAST.log")),
                LoggerProvider.FromFile(Path.Combine(Data.LogPath, "EESAST.error.log")));
            Web.Token_Changed += SaveToken;
            LoggerBinding();

            if (Data.Config.Remembered)
            {
                Username = Data.Config.UserName;
                Password = Data.Config.Password;
            }
            Cloud.UpdateSecret(MauiProgram.SecretID, MauiProgram.SecretKey);
        }

        public void LoggerBinding()
        {
            // Debug模式下将Exceptions直接抛出触发断点
            if (Debugger.IsAttached && MauiProgram.ErrorTrigger_WhileDebug)
            {
                Exceptions.OnFailed += (obj, _) =>
                {
                    var e = Exceptions.Pop();
                    if (e is not null)
                        throw e;
                };
            }
            Data.Exceptions.OnFailed += (obj, _) =>
            {
                var e = Data.Exceptions.Pop();
                if (e is null) return;
                if (obj is not null)
                    e.Data["Source"] = obj.ToString();
                LogError.LogError($"从Downloader.Data处提取的错误。");
                Exceptions.Push(e);
            };
            Cloud.Exceptions.OnFailed += (obj, _) =>
            {
                var e = Cloud.Exceptions.Pop();
                if (e is null) return;
                if (obj is not null)
                    e.Data["Source"] = obj.ToString();
                LogError.LogError($"从Downloader.Cloud处提取的错误。");
                Exceptions.Push(e);
            };
            Web.Exceptions.OnFailed += (obj, _) =>
            {
                var e = Web.Exceptions.Pop();
                if (e is null) return;
                if (obj is not null)
                    e.Data["Source"] = obj.ToString();
                LogError.LogError($"从Downloader.Web处提取的错误。");
                Exceptions.Push(e);
            };
            Exceptions.OnFailClear += (_, _) =>
            {
                Status = UpdateStatus.success;
            };
        }

        public void UpdateMD5()
        {
            if (File.Exists(Data.MD5DataPath))
                File.Delete(Data.MD5DataPath);
            Status = UpdateStatus.downloading;
            Cloud.DownloadFileAsync(Data.MD5DataPath, "hash.json").Wait();
            if (Exceptions.Count > 0)
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
            if (Status == UpdateStatus.error) return;

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
            deleteTask(new DirectoryInfo(Data.Config.InstallPath));

            Data.ResetInstallPath(Data.Config.InstallPath);
            Cloud.Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "TencentCos.log"));
            Cloud.LogError = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "TencentCos.error.log"));
            Cloud.Exceptions = new ExceptionStack(Cloud.LogError, Cloud);
            Web.Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "EESAST.log"));
            Web.LogError = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "EESAST.error.log"));
            Web.Exceptions = new ExceptionStack(Web.LogError, Web);
            Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "Main.log"));
            LogError = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "Main.error.log"));
            Exceptions = new ExceptionStack(LogError, this);
            LoggerBinding();

            string zp = Path.Combine(Data.Config.InstallPath, "THUAI7.tar.gz");
            Status = UpdateStatus.downloading;
            Cloud.DownloadFileAsync(zp, "THUAI7.tar.gz").Wait();
            Status = UpdateStatus.unarchieving;
            Cloud.ArchieveUnzip(zp, Data.Config.InstallPath);
            File.Delete(zp);

            Status = UpdateStatus.hash_computing;
            Data.ScanDir();
            if (Data.MD5Update.Count != 0)
            {
                // TO DO: 下载文件与hash校验值不匹配修复
                Status = UpdateStatus.error;
                Update();
            }
            else
            {
                Status = UpdateStatus.success;
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
                Log.Dispose(); LogError.Dispose(); Exceptions.logger.Dispose();
                Cloud.Log.Dispose(); Cloud.LogError.Dispose(); Cloud.Exceptions.logger.Dispose();
                Web.Log.Dispose(); Web.LogError.Dispose(); Web.Exceptions.logger.Dispose();
                Data.ResetInstallPath(newPath);

                Cloud.Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "TencentCos.log"));
                Cloud.LogError = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "TencentCos.error.log"));
                Cloud.Exceptions = new ExceptionStack(Cloud.LogError, Cloud);
                Web.Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "EESAST.log"));
                Web.LogError = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "EESAST.error.log"));
                Web.Exceptions = new ExceptionStack(Web.LogError, Web);
                Log = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "Main.log"));
                LogError = LoggerProvider.FromFile(Path.Combine(Data.LogPath, "Main.error.log"));
                Exceptions = new ExceptionStack(LogError, this);
                LoggerBinding();
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
            Status = UpdateStatus.hash_computing;
            Data.ScanDir();
            Status = UpdateStatus.success;
            return Data.MD5Update.Count != 0 || CurrentVersion < Data.FileHashData.Version;
        }

        /// <summary>
        /// 更新文件
        /// </summary>
        public int Update()
        {
            int result = 0;
            if (CheckUpdate())
            {
                // 处理AI.cpp/AI.py合并问题
                if (CurrentVersion < Data.FileHashData.Version)
                {
                    var c = CurrentVersion;
                    var v = Data.FileHashData.Version;
                    Status = UpdateStatus.downloading;
                    var p = Path.Combine(Data.Config.InstallPath, "Templates");
                    var tocpp = Cloud.DownloadFileAsync(Path.Combine(p, $"v{c}.cpp.t"),
                        $"./Template/v{c}.cpp.t");
                    var topy = Cloud.DownloadFileAsync(Path.Combine(p, $"v{c}.py.t"),
                        $"./Template/v{c}.py.t");
                    var tncpp = Cloud.DownloadFileAsync(Path.Combine(p, $"v{v}.cpp.t"),
                        $"./Template/v{v}.cpp.t");
                    var tnpy = Cloud.DownloadFileAsync(Path.Combine(p, $"v{v}.py.t"),
                        $"./Template/v{v}.py.t");
                    Task.WaitAll(tocpp, topy, tncpp, tnpy);
                    if (Directory.GetFiles(p).Count() == 4)
                    {
                        if (Data.LangEnabled[LanguageOption.cpp].Item1)
                        {
                            var so = FileService.ReadToEnd(Path.Combine(p, $"v{c}.cpp.t"));
                            var sn = FileService.ReadToEnd(Path.Combine(p, $"v{v}.cpp.t"));
                            var sa = FileService.ReadToEnd(Data.LangEnabled[LanguageOption.cpp].Item2);
                            var s = FileService.MergeUserCode(sa, so, sn);
                            using (var f = new FileStream(Data.LangEnabled[LanguageOption.cpp].Item2 + ".temp", FileMode.Create))
                            using (var w = new StreamWriter(f))
                            {
                                w.Write(s);
                                w.Flush();
                            }
                            result |= 1;
                        }
                        if (Data.LangEnabled[LanguageOption.python].Item1)
                        {
                            var so = FileService.ReadToEnd(Path.Combine(p, $"v{c}.py.t"));
                            var sn = FileService.ReadToEnd(Path.Combine(p, $"v{v}.py.t"));
                            var sa = FileService.ReadToEnd(Data.LangEnabled[LanguageOption.python].Item2);
                            var s = FileService.MergeUserCode(sa, so, sn);
                            using (var f = new FileStream(Data.LangEnabled[LanguageOption.python].Item2 + ".temp", FileMode.Create))
                            using (var w = new StreamWriter(f))
                            {
                                w.Write(s);
                                w.Flush();
                            }
                            result |= 2;
                        }
                    }
                }
                downloadFailed.Clear();

                Status = UpdateStatus.downloading;
                Cloud.DownloadQueueAsync(Data.Config.InstallPath,
                    from item in Data.MD5Update where item.state != System.Data.DataRowState.Added select item.name,
                    downloadFailed).Wait();
                foreach (var item in Data.MD5Update.Where((s) => s.state == System.Data.DataRowState.Added))
                {
                    var _file = item.name;
                    var file = _file.StartsWith('.') ?
                        Path.Combine(Data.Config.InstallPath, _file) : _file;
                    File.Delete(file);
                }
                if (downloadFailed.Count == 0)
                {
                    Data.MD5Update.Clear();
                    Status = UpdateStatus.hash_computing;
                    Data.ScanDir();
                    if (Data.MD5Update.Count == 0)
                    {
                        Status = UpdateStatus.success;
                        return result;
                    }
                }
            }
            else
            {
                Status = UpdateStatus.success;
                return result;
            }
            Status = UpdateStatus.error;
            return result;
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