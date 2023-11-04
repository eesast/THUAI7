using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Platform;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace installer.Model
{
    class Local_Data
    {
        public string ConfigPath;      // 标记路径记录文件THUAI7.json的路径
        public string MD5DataPath;     // 标记MD5本地文件缓存值
        public Dictionary<string, string> Config
        {
            get; protected set;
        }
        public Dictionary<string, string> MD5Data
        {
            get; protected set;
        }// 路径为尽可能相对路径
        public ConcurrentBag<string> MD5Update
        {
            get; set;
        }// 路径为绝对路径
        public string InstallPath = ""; // 最后一级为THUAI7文件夹所在目录
        public bool Installed = false;  // 项目是否安装
        public Local_Data()
        {
            MD5Update = new ConcurrentBag<string>();
            ConfigPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                "THUAI7.json");
            if (File.Exists(ConfigPath))
            {
                ReadConfig();
                if (Config.ContainsKey("InstallPath") && Directory.Exists(Config["InstallPath"]))
                {
                    InstallPath = Config["InstallPath"].Replace('\\', '/');
                    if (Config.ContainsKey("MD5DataPath"))
                    {
                        MD5DataPath = Config["MD5DataPath"].StartsWith('.') ?
                            Path.Combine(InstallPath, Config["MD5DataPath"]) :
                            Config["MD5DataPath"];
                        ReadMD5Data();
                    }
                    else
                    {
                        MD5DataPath = Path.Combine(InstallPath, "./hash.json");
                        Config["MD5DataPath"] = "./hash.json";
                        SaveMD5Data();
                        SaveConfig();
                    }
                    Installed = true;
                }
                else
                {
                    MD5DataPath = Path.Combine(InstallPath, "./hash.json");
                    Config["MD5DataPath"] = "./hash.json";
                    SaveMD5Data();
                    SaveConfig();
                }
            }
            else
            {
                Config = new Dictionary<string, string>
                {
                    { "THUAI7", "2024" },
                    { "MD5DataPath", "./hash.json" }
                };
                MD5DataPath = Path.Combine(InstallPath, "./hash.json");
                SaveMD5Data();
                SaveConfig();
            }
        }

        ~Local_Data()
        {
            SaveConfig();
        }

        public void ResetInstallPath(string newPath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(newPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            }
            if (Installed)
            {
                // 移动已有文件夹至新位置
                Directory.Move(newPath, InstallPath);
            }
            InstallPath = newPath.Replace('\\', '/');
            if (Config.ContainsKey("InstallPath"))
                Config["InstallPath"] = InstallPath;
            else
                Config.Add("InstallPath", InstallPath);
            SaveConfig();
        }

        public static bool IsUserFile(string filename)
        {
            if (filename.Substring(filename.Length - 3, 3).Equals(".sh") || filename.Substring(filename.Length - 4, 4).Equals(".cmd"))
                return true;
            if (filename.Equals("AI.cpp") || filename.Equals("AI.py"))
                return true;
            return false;
        }

        public void ReadConfig()
        {
            using (StreamReader r = new StreamReader(ConfigPath))
            {
                string json = r.ReadToEnd();
                if (json == null || json == "")
                {
                    json += @"{""THUAI6""" + ":" + @"""2023""}";
                }
                Config = Helper.TryDeserializeJson<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
        }

        public void SaveConfig()
        {
            using FileStream fs = new FileStream(ConfigPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using StreamWriter sw = new StreamWriter(fs);
            fs.SetLength(0);
            sw.Write(JsonConvert.SerializeObject(Config));
            sw.Flush();
        }

        public void ReadMD5Data()
        {
            var newMD5Data = new Dictionary<string, string>();
            using (StreamReader r = new StreamReader(MD5DataPath))
            {
                string json = r.ReadToEnd();
                if (json == null || json == "")
                {
                    newMD5Data = new Dictionary<string, string>();
                }
                else
                {
                    newMD5Data = Helper.TryDeserializeJson<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            foreach(var item in newMD5Data)
            {
                if(MD5Data.ContainsKey(item.Key))
                {
                    if (MD5Data[item.Key] != newMD5Data[item.Value])
                    {
                        MD5Data[item.Key] = newMD5Data[item.Value];
                        MD5Update.Add(Path.Combine(InstallPath, item.Key));
                    }
                }
                else
                {
                    MD5Data.Add(item.Key, item.Value);
                    MD5Update.Add(Path.Combine(InstallPath, item.Key));
                }
            }
        }

        public void SaveMD5Data()
        {
            using FileStream fs = new FileStream(MD5DataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using StreamWriter sw = new StreamWriter(fs);
            fs.SetLength(0);
            sw.Write(JsonConvert.SerializeObject(MD5Data));
            sw.Flush();
        }

        public void ScanDir() => ScanDir(InstallPath);
        
        public void ScanDir(string dir)
        {
            var d = new DirectoryInfo(dir);
            foreach(var file in d.GetFiles()) 
            {
                var relFile = Helper.ConvertAbsToRel(InstallPath, file.FullName);
                // 用户自己的文件不会被计入更新hash数据中
                if (IsUserFile(file.Name))
                    continue;
                var hash = Helper.GetFileMd5Hash(file.FullName);
                if (MD5Data.Keys.Contains(relFile))
                {
                    if (MD5Data[relFile] != hash)
                    {
                        MD5Data[relFile] = hash;
                        MD5Update.Add(file.FullName);
                    }
                }
                else
                {
                    MD5Data.Add(relFile, hash);
                    MD5Update.Add(file.FullName);
                }
            }
            foreach(var d1 in d.GetDirectories()) { ScanDir(d1.FullName); }
        }
    }
}
