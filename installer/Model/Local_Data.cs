using System;
using System.Collections.Concurrent;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace installer.Model
{
    public class Local_Data
    {
        public string ConfigPath;      // 标记路径记录文件THUAI7.json的路径
        public string MD5DataPath;     // 标记MD5本地文件缓存值
        public Dictionary<string, string> Config
        {
            get; protected set;
        } = new Dictionary<string, string>();
        public Dictionary<string, string> MD5Data
        {
            get; protected set;
        } = new Dictionary<string, string>();   // 路径为尽可能相对路径
        public ConcurrentBag<(DataRowState state, string name)> MD5Update
        {
            get; set;
        }// 路径为绝对路径
        public string InstallPath = ""; // 最后一级为THUAI7文件夹所在目录
        public bool Installed = false;  // 项目是否安装
        protected Logger Log = LoggerProvider.FromConsole();

        public Local_Data()
        {
            MD5Update = new ConcurrentBag<(DataRowState state, string name)>();
            ConfigPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "THUAI7.json");
            if (File.Exists(ConfigPath))
            {
                ReadConfig();
                if (Config.ContainsKey("InstallPath") && Directory.Exists(Config["InstallPath"]))
                {
                    InstallPath = Config["InstallPath"].Replace(Path.DirectorySeparatorChar, '/');
                    if (Config.ContainsKey("MD5DataPath"))
                    {
                        MD5DataPath = Config["MD5DataPath"].StartsWith('.') ?
                            Path.Combine(InstallPath, Config["MD5DataPath"]) :
                            Config["MD5DataPath"];
                        if (!File.Exists(MD5DataPath))
                            SaveMD5Data();
                        ReadMD5Data();
                        MD5Update.Clear();
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
            SaveMD5Data();
            SaveConfig();
        }

        public void ResetInstallPath(string newPath)
        {
            string? dirName = Path.GetDirectoryName(newPath);
            if (dirName is null)
            {
                return;
            }
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            if (Installed)
            {
                // 移动已有文件夹至新位置
                Directory.Move(newPath, InstallPath);
            }
            InstallPath = newPath.Replace(Path.DirectorySeparatorChar, '/');
            if (Config.ContainsKey("InstallPath"))
                Config["InstallPath"] = InstallPath;
            else
                Config.Add("InstallPath", InstallPath);
            MD5DataPath = Config["MD5DataPath"].StartsWith('.') ?
                Path.Combine(InstallPath, Config["MD5DataPath"]) :
                Config["MD5DataPath"];
            SaveConfig();
            SaveMD5Data();
            Installed = true;
        }

        public static bool IsUserFile(string filename)
        {
            if (filename.Contains("git") || filename.Contains("bin") || filename.Contains("obj"))
                return true;
            if (filename.EndsWith("sh") || filename.EndsWith("cmd"))
                return true;
            if (filename.Contains("AI.cpp") || filename.Contains("AI.py"))
                return true;
            if (filename.Contains("hash.json"))
                return true;
            return false;
        }

        public void ReadConfig()
        {
            using (StreamReader r = new StreamReader(ConfigPath))
            {
                string json = r.ReadToEnd();
                if (json is null || json == "")
                {
                    json += @"{""THUAI7""" + ":" + @"""2024""}";
                }
                Config = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
        }

        public void SaveConfig()
        {
            using FileStream fs = new FileStream(ConfigPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using StreamWriter sw = new StreamWriter(fs);
            fs.SetLength(0);
            sw.Write(JsonSerializer.Serialize(Config));
            sw.Flush();
        }

        public void ReadMD5Data()
        {
            Dictionary<string, string> newMD5Data;
            StreamReader r = new StreamReader(MD5DataPath);
            try
            {
                string json = r.ReadToEnd();
                if (json is null || json == "")
                {
                    newMD5Data = new Dictionary<string, string>();
                }
                else
                {
                    newMD5Data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                }
                r.Close(); r.Dispose();
            }
            catch (JsonException)
            {
                // Json反序列化失败，考虑重新创建MD5数据库
                newMD5Data = new Dictionary<string, string>();
                r.Close(); r.Dispose();
                File.Delete(MD5DataPath);
                File.Create(MD5DataPath);
            }
            foreach (var item in newMD5Data)
            {
                if (MD5Data.ContainsKey(item.Key))
                {
                    if (MD5Data[item.Key] != item.Value)
                    {
                        MD5Data[item.Key] = item.Value;
                        MD5Update.Add((DataRowState.Modified, item.Key));
                    }
                }
                else
                {
                    MD5Data.Add(item.Key, item.Value);
                    MD5Update.Add((DataRowState.Added, item.Key));
                }
            }
        }

        public void SaveMD5Data()
        {
            using (FileStream fs = new FileStream(MD5DataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                fs.SetLength(0);
                sw.Write(JsonSerializer.Serialize(MD5Data));
                sw.Flush();
            }
        }

        public void ScanDir()
        {
            foreach (var _file in MD5Data.Keys)
            {
                if (_file is null)
                    continue;
                var file = _file.StartsWith('.') ?
                    Path.Combine(InstallPath.Replace('/', Path.DirectorySeparatorChar), _file.Replace('/', Path.DirectorySeparatorChar)) :
                    _file;
                if (!File.Exists(file))
                {
                    MD5Data.Remove(_file);
                    MD5Update.Add((DataRowState.Deleted, _file));
                }
            }
            ScanDir(InstallPath);
            SaveMD5Data();
        }

        public void ScanDir(string dir)
        {
            var d = new DirectoryInfo(dir);
            foreach (var file in d.GetFiles())
            {
                var relFile = Helper.ConvertAbsToRel(InstallPath, file.FullName);
                // 用户自己的文件不会被计入更新hash数据中
                if (IsUserFile(relFile))
                    continue;
                var hash = Helper.GetFileMd5Hash(file.FullName);
                if (MD5Data.Keys.Contains(relFile))
                {
                    if (MD5Data[relFile] != hash)
                    {
                        MD5Data[relFile] = hash;
                        MD5Update.Add((DataRowState.Modified, relFile));
                    }
                }
                else
                {
                    MD5Data.Add(relFile, hash);
                    MD5Update.Add((DataRowState.Added, relFile));
                }
            }
            foreach (var d1 in d.GetDirectories()) { ScanDir(d1.FullName); }
        }
    }
}
