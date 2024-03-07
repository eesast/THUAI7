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
        public string CodeRoute { get; set; } = string.Empty;
        public string PlayerNum { get; set; } = "nSelect";
        public enum LaunchLanguage { cpp, python };
        public LaunchLanguage Language { get; set; } = LaunchLanguage.cpp;
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
                string tardir = Path.Combine(Data.InstallPath, "LogArchieved");
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
            Route = Data.InstallPath;
            Cloud = new Tencent_Cos("1319625962", "ap-beijing", "bucket1",
                LoggerProvider.FromFile(Path.Combine(Data.LogPath, "TencentCos.log")),
                LoggerProvider.FromFile(Path.Combine(Data.LogPath, "TencentCos.error.log")));
            Web = new EEsast(LoggerProvider.FromFile(Path.Combine(Data.LogPath, "EESAST.log")),
                LoggerProvider.FromFile(Path.Combine(Data.LogPath, "EESAST.error.log")));
            Web.Token_Changed += SaveToken;
            LoggerBinding();

            string? temp;
            if (Data.Config.TryGetValue("Remembered", out temp))
            {
                if (Convert.ToBoolean(temp))
                {
                    if (Data.Config.TryGetValue("Username", out temp))
                        Username = temp;
                    if (Data.Config.TryGetValue("Password", out temp))
                        Password = temp;
                }
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
        public void Install()
        {
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
            deleteTask(new DirectoryInfo(Data.InstallPath));

            Data.Installed = false;
            string zp = Path.Combine(Data.InstallPath, "THUAI7.tar.gz");
            Status = UpdateStatus.downloading;
            Cloud.DownloadFileAsync(zp, "THUAI7.tar.gz").Wait();
            Status = UpdateStatus.unarchieving;
            Cloud.ArchieveUnzip(zp, Data.InstallPath);
            File.Delete(zp);

            Data.ResetInstallPath(Data.InstallPath);
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
            var installPath = Data.InstallPath.EndsWith(Path.DirectorySeparatorChar) ? Data.InstallPath[0..-1] : Data.InstallPath;
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
            return Data.MD5Update.Count != 0;
        }

        /// <summary>
        /// 更新文件
        /// </summary>
        public void Update()
        {
            if (CheckUpdate())
            {
                Status = UpdateStatus.downloading;
                Cloud.DownloadQueueAsync(Data.InstallPath,
                    from item in Data.MD5Update where item.state != System.Data.DataRowState.Added select item.name,
                    downloadFailed).Wait();
                foreach (var item in Data.MD5Update.Where((s) => s.state == System.Data.DataRowState.Added))
                {
                    var _file = item.name;
                    var file = _file.StartsWith('.') ?
                        Path.Combine(Data.InstallPath, _file) : _file;
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
                        return;
                    }
                }
            }
            else
            {
                Status = UpdateStatus.success;
                return;
            }
            Status = UpdateStatus.error;
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
            if (Data.Config.ContainsKey("Token"))
                Data.Config["Token"] = Web.Token;
            else
                Data.Config.Add("Token", Web.Token);
            Data.SaveConfig();
        }

        public void RememberUser()
        {
            if (Data.Config.ContainsKey("Username"))
                Data.Config["Username"] = Username;
            else
                Data.Config.Add("Username", Username);

            if (Data.Config.ContainsKey("Password"))
                Data.Config["Password"] = Password;
            else
                Data.Config.Add("Password", Password);

            if (Data.Config.ContainsKey("Remembered"))
                Data.Config["Remembered"] = "true";
            else
                Data.Config.Add("Remembered", "true");

            Data.SaveConfig();
        }

        public void ForgetUser()
        {
            if (Data.Config.ContainsKey("Remembered"))
                Data.Config["Remembered"] = "false";

            if (Data.Config.ContainsKey("Username"))
                Data.Config["Username"] = string.Empty;

            if (Data.Config.ContainsKey("Password"))
                Data.Config["Password"] = string.Empty;

            Data.SaveConfig();
        }

        /// <summary>
        /// 上传选手代码
        /// </summary>
        /// <param name="player_id">对应玩家id</param>
        public void UploadFiles(int player_id)
        {
            Web.UploadFiles(Client, Path.Combine(Data.InstallPath, Data.UserCodePath),
                Language == LaunchLanguage.cpp ? "cpp" : "python",
                $"player_{player_id}").Wait();
        }
        #endregion
    }
}