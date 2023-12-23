using COSXML.CosException;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string ProgramName = "THUAI7";                     // 要运行或下载的程序名称
        public string StartName = "maintest.exe";          // 启动的程序名
        public Local_Data Data;
        public Tencent_Cos Cloud;

        public HttpClient Client = new HttpClient();
        public EEsast Web = new EEsast();

        public enum UpdateStatus
        {
            success, unarchieving, downloading, hash_computing, error
        } //{ newUser, menu, move, working, initializing, disconnected, error, successful, login, web, launch };
        public UpdateStatus Status;

        ConcurrentQueue<string> downloadFailed = new ConcurrentQueue<string>();  //更新失败的文件名
        public List<string> UpdateFailed
        {
            get { return downloadFailed.ToList(); }
        }
        public bool UpdatePlanned
        {
            get; set;
        }

        public void ResetDownloadFailedInfo()
        {
            downloadFailed.Clear();
        }
        public string Route { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserId { get => Web.ID; }
        public string UserEmail { get => Web.Email; }
        public string CodeRoute { get; set; } = string.Empty;
        public string? Language { get; set; } = null;
        public string PlayerNum { get; set; } = "nSelect";
        public enum LaunchLanguage { cpp, python };
        public LaunchLanguage launchLanguage { get; set; } = LaunchLanguage.cpp;
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
        public bool RememberMe { get; set; }

        #endregion

        #region 方法区
        public Downloader()
        {
            Data = new Local_Data();
            Route = Data.InstallPath;
            Cloud = new Tencent_Cos("1314234950", "ap-beijing", "thuai6");
            Web.Token_Changed += SaveToken;
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
        }

        public void UpdateMD5()
        {
            if (File.Exists(Data.MD5DataPath))
                File.Delete(Data.MD5DataPath);
            Status = UpdateStatus.downloading;
            Cloud.DownloadFileAsync(Data.MD5DataPath, "hash.json").Wait();
            if (Cloud.Exceptions.Count > 0)
            {
                Status = UpdateStatus.error;
                return;
            }
            Data.ReadMD5Data();
        }

        /// <summary>
        /// 全新安装
        /// </summary>
        public void Install()
        {
            UpdateMD5();
            if (Status == UpdateStatus.error) return;

            if (Directory.Exists(Data.InstallPath))
                Directory.Delete(Data.InstallPath, true);

            Data.Installed = false;
            string zp = Path.Combine(Data.InstallPath, "THUAI7.tar.gz");
            Status = UpdateStatus.downloading;
            Cloud.DownloadFileAsync(zp, "THUAI7.tar.gz").Wait();
            Status = UpdateStatus.unarchieving;
            Cloud.ArchieveUnzip(zp, Data.InstallPath);
            File.Delete(zp);

            Data.ResetInstallPath(Data.InstallPath);
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
                Cloud.DownloadQueueAsync(Data.InstallPath, new ConcurrentQueue<string>(Data.MD5Update), downloadFailed).Wait();
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

        public async Task Login()
        {
            await Web.LoginToEEsast(Client, Username, Password);
        }

        public void SaveToken(object? sender, EventArgs args)  // 保存token
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
        #endregion
    }


}
