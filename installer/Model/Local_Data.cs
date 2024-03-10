using System;
using System.Collections.Concurrent;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace installer.Model
{
    public class Local_Data
    {
        public string ConfigPath;       // 标记路径记录文件THUAI7.json的路径
        public string MD5DataPath;      // 标记MD5本地缓存文件的路径
        public string UserCodePostfix;  // 用户文件后缀(.cpp/.py)
        public string UserCodePath
        {
            get => Path.Combine(InstallPath,
            $"???{Path.DirectorySeparatorChar}AI{UserCodePostfix}");
        }
        public string LogPath { get => Path.Combine(InstallPath, "Logs"); }
        public Dictionary<string, string> Config
        {
            get; protected set;
        } = new Dictionary<string, string>();
        public ConcurrentDictionary<string, string> MD5Data
        {
            get; protected set;
        } = new ConcurrentDictionary<string, string>();   // 路径为尽可能相对路径
        public ConcurrentBag<(DataRowState state, string name)> MD5Update
        {
            get; set;
        }                               // 路径为绝对路径
        public string InstallPath = ""; // 最后一级为THUAI7文件夹所在目录
        public bool Installed = false;  // 项目是否安装
        public bool RememberMe = false; // 是否记录账号密码
        public Logger Log;
        public Logger LogError;
        public ExceptionStack Exceptions;
        public Local_Data()
        {
            MD5Update = new ConcurrentBag<(DataRowState state, string name)>();
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            ConfigPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "THUAI7.json");
            if (File.Exists(ConfigPath))
            {
                ReadConfig();
                if (Config.ContainsKey("InstallPath") && Directory.Exists(Config["InstallPath"]))
                {
                    InstallPath = Config["InstallPath"];
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
                        MD5DataPath = Path.Combine(InstallPath, $".{Path.DirectorySeparatorChar}hash.json");
                        Config["MD5DataPath"] = $".{Path.DirectorySeparatorChar}hash.json";
                        SaveMD5Data();
                        SaveConfig();
                    }
                    RememberMe = (Config.ContainsKey("Remembered") && Convert.ToBoolean(Config["Remembered"]));
                    Installed = true;
                }
                else
                {
                    var dir = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "THUAI7"));
                    InstallPath = dir.FullName;
                    Config["InstallPath"] = InstallPath;
                    MD5DataPath = Path.Combine(InstallPath, $".{Path.DirectorySeparatorChar}hash.json");
                    Config["MD5DataPath"] = $".{Path.DirectorySeparatorChar}hash.json";
                    SaveMD5Data();
                    SaveConfig();
                }
            }
            else
            {
                Config = new Dictionary<string, string>
                {
                    { "THUAI7", "2024" },
                    { "MD5DataPath", $".{Path.DirectorySeparatorChar}hash.json" }
                };
                var dir = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "THUAI7"));
                InstallPath = dir.FullName;
                Config["InstallPath"] = InstallPath;
                MD5DataPath = Path.Combine(InstallPath, $".{Path.DirectorySeparatorChar}hash.json");
                SaveMD5Data();
                SaveConfig();
            }
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
            // DEBUG模式下每次启动清除日志
            if (Debugger.IsAttached && MauiProgram.RefreshLogs_WhileDebug)
            {
                foreach (var log in Directory.EnumerateFiles(LogPath))
                {
                    File.Delete(log);
                }
            }
            Log = LoggerProvider.FromFile(Path.Combine(LogPath, "LocalData.log"));
            LogError = LoggerProvider.FromFile(Path.Combine(LogPath, "LocalData.error.log"));
            Exceptions = new ExceptionStack(LogError, this);
        }

        ~Local_Data()
        {
            SaveMD5Data();
            SaveConfig();
        }

        public void ResetInstallPath(string newPath)
        {
            // 移动已有文件夹至新位置
            try
            {
                if (InstallPath != newPath)
                {
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    Log.LogInfo($"Move work started: {InstallPath} -> {newPath}");
                    Log.Dispose(); LogError.Dispose(); Exceptions.logger.Dispose();
                    Action<DirectoryInfo> action = (dir) => { };
                    var moveTask = (DirectoryInfo dir) =>
                    {
                        foreach (var file in dir.EnumerateFiles())
                        {
                            var newName = Path.Combine(newPath, Helper.ConvertAbsToRel(InstallPath, file.FullName));
                            file.MoveTo(newName);
                        }
                        foreach (var sub in dir.EnumerateDirectories())
                        {
                            var newName = Path.Combine(newPath, Helper.ConvertAbsToRel(InstallPath, sub.FullName));
                            if (!Directory.Exists(newName))
                            {
                                Directory.CreateDirectory(newName);
                            }
                            action(sub);
                        }
                    };
                    action = moveTask;
                    moveTask(new DirectoryInfo(InstallPath));
                    Directory.Delete(InstallPath, true);
                    InstallPath = newPath;
                }
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
            catch (Exception e)
            {
                Exceptions.Push(e);
            }
            finally
            {
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                Log = LoggerProvider.FromFile(Path.Combine(LogPath, "LocalData.log"));
                LogError = LoggerProvider.FromFile(Path.Combine(LogPath, "LocalData.error.log"));
                Exceptions = new ExceptionStack(LogError, this);
                Log.LogInfo($"Move work finished: {InstallPath} -> {newPath}");
            }
        }

        public void ReadConfig()
        {
            try
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
            catch (Exception e)
            {
                Exceptions.Push(e);
            }
        }

        public void SaveConfig()
        {
            try
            {
                using FileStream fs = new FileStream(ConfigPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                using StreamWriter sw = new StreamWriter(fs);
                fs.SetLength(0);
                sw.Write(JsonSerializer.Serialize(Config));
                sw.Flush();
            }
            catch (Exception e)
            {
                Exceptions.Push(e);
            }
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
                    newMD5Data = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                r.Close(); r.Dispose();
            }
            catch (JsonException e)
            {
                // Json反序列化失败，考虑重新创建MD5数据库
                newMD5Data = new Dictionary<string, string>();
                r.Close(); r.Dispose();
                File.Delete(MD5DataPath);
                File.Create(MD5DataPath);
            }
            catch (Exception e)
            {
                Exceptions.Push(e);
                newMD5Data = new Dictionary<string, string>();
                r.Close(); r.Dispose();
            }
            foreach (var item in newMD5Data)
            {
                var key = item.Key.Replace('/', Path.DirectorySeparatorChar);
                MD5Data.AddOrUpdate(key, (k) =>
                {
                    MD5Update.Add((DataRowState.Added, key));
                    return item.Value;
                }, (k, v) =>
                {
                    if (v != item.Value)
                        MD5Update.Add((DataRowState.Modified, key));
                    return item.Value;
                });
            }
        }

        public void SaveMD5Data()
        {
            try
            {
                using (FileStream fs = new FileStream(MD5DataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.SetLength(0);
                    var exp1 = from i in MD5Data
                               select new KeyValuePair<string, string>(i.Key.Replace(Path.DirectorySeparatorChar, '/'), i.Value);
                    sw.Write(JsonSerializer.Serialize(exp1.ToDictionary<string, string>()));
                    sw.Flush();
                }
            }
            catch (Exception e)
            {
                Exceptions.Push(e);
            }
        }

        public void ScanDir()
        {
            foreach (var _file in MD5Data.Keys)
            {
                if (_file is null)
                    continue;
                var file = _file.StartsWith('.') ?
                    Path.Combine(InstallPath, _file) : _file;
                if (!File.Exists(file) && MD5Data.TryRemove(_file, out _))
                {
                    MD5Update.Add((DataRowState.Deleted, _file));
                }
            }
            // 层序遍历文件树
            Stack<string> stack = new Stack<string>();
            List<string> files = new List<string>();
            stack.Push(InstallPath);
            while (stack.Count > 0)
            {
                string cur = stack.Pop();
                files.AddRange(from f in Directory.GetFiles(cur)
                               where !IsUserFile(f)
                               select f);
                foreach (var d in Directory.GetDirectories(cur))
                    stack.Push(d);
            }
            if (files.Count == 0)
            {
                MD5Data.Clear();
                SaveMD5Data();
                return;
            }
            // 并行计算hash值
            var partitioner = Partitioner.Create(0, files.Count);
            Parallel.ForEach(partitioner, (range, loopState) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    if (loopState.IsStopped)
                        break;
                    var file = files[i];
                    var relFile = Helper.ConvertAbsToRel(InstallPath, file);
                    var hash = Helper.GetFileMd5Hash(file);
                    MD5Data.AddOrUpdate(relFile, (k) =>
                    {
                        MD5Update.Add((DataRowState.Added, relFile));
                        return hash;
                    }, (k, v) =>
                    {
                        if (v != hash)
                            MD5Update.Add((DataRowState.Modified, relFile));
                        return hash;
                    });
                }
            });
            SaveMD5Data();
        }


        public static bool IsUserFile(string filename)
        {
            if (filename.Contains("git") || filename.Contains("bin") || filename.Contains("obj"))
                return true;
            if (filename.EndsWith("sh") || filename.EndsWith("cmd"))
                return true;
            if (filename.EndsWith("gz"))
                return true;
            if (filename.Contains("AI.cpp") || filename.Contains("AI.py"))
                return true;
            if (filename.Contains("hash.json"))
                return true;
            if (filename.EndsWith("log"))
                return true;
            return false;
        }

        public static int CountFile(string folder)
        {
            int result = (from f in Directory.EnumerateDirectories(folder)
                          where !IsUserFile(f)
                          select f).Count();
            foreach (var d in Directory.EnumerateDirectories(folder))
            {
                result += CountFile(d);
            }
            return result;
        }
    }
}
