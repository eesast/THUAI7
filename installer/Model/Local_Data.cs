using System;
using System.Collections.Concurrent;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using installer.Services;
using installer.Data;

using Command = installer.Data.Command;

namespace installer.Model
{
    public class Local_Data
    {
        public string MD5DataPath;      // 标记MD5本地缓存文件的路径
        public MD5DataFile FileHashData = new MD5DataFile();
        public ConfigData Config;
        public Version CurrentVersion;
        public Dictionary<LanguageOption, (bool, string)> LangEnabled;
        public string LogPath { get => Path.Combine(Config.InstallPath, "Logs"); }
        public ConcurrentDictionary<string, string> MD5Data
        {
            get; protected set;
        } = new ConcurrentDictionary<string, string>();   // 路径为尽可能相对路径
        public ConcurrentBag<(DataRowState state, string name)> MD5Update
        {
            get; set;
        }                               // 路径为绝对路径
        public bool Installed
        {
            get => Config.Installed;
            set => Config.Installed = value;
        }  // 项目是否安装
        public bool RememberMe = false; // 是否记录账号密码
        public Logger Log;
        public Local_Data()
        {
            MD5Update = new ConcurrentBag<(DataRowState state, string name)>();
            Config = new ConfigData();
            if (Directory.Exists(Config.InstallPath))
            {
                MD5DataPath = Config.MD5DataPath.StartsWith('.') ?
                    Path.Combine(Config.InstallPath, Config.MD5DataPath) :
                    Config.MD5DataPath;
                if (File.Exists(MD5DataPath))
                {
                    if (!File.Exists(MD5DataPath))
                        SaveMD5Data();
                    ReadMD5Data();
                    CurrentVersion = FileHashData.Version;
                    MD5Update.Clear();
                }
                else
                {
                    MD5DataPath = Path.Combine(Config.InstallPath, $"hash.json");
                    Config.MD5DataPath = $".{Path.DirectorySeparatorChar}hash.json";
                    CurrentVersion = FileHashData.Version;
                    SaveMD5Data();
                }
                RememberMe = (Config.Remembered && Convert.ToBoolean(Config.Remembered));
            }
            else
            {
                var dir = Directory.CreateDirectory(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    , "THUAI7", "Data"));
                Config.InstallPath = dir.FullName;
                MD5DataPath = Path.Combine(Config.InstallPath, "hash.json");
                Config.MD5DataPath = $".{Path.DirectorySeparatorChar}hash.json";
                CurrentVersion = FileHashData.Version;
                SaveMD5Data();
                Config.SaveFile();
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
            Log.PartnerInfo = "[LocalData]";
            LangEnabled = new Dictionary<LanguageOption, (bool, string)>();
            foreach (var a in typeof(LanguageOption).GetEnumValues())
            {
                LangEnabled.Add((LanguageOption)a, (false, string.Empty));
            }
        }

        ~Local_Data()
        {
            SaveMD5Data();
            Config.SaveFile();
        }

        public void ResetInstallPath(string newPath)
        {
            // 移动已有文件夹至新位置
            try
            {
                if (Config.InstallPath != newPath)
                {
                    var oldPath = Config.InstallPath;
                    Config.InstallPath = newPath;
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    Log.LogInfo($"Move work started: {oldPath} -> {newPath}");
                    Action<DirectoryInfo> action = (dir) => { };
                    var moveTask = (DirectoryInfo dir) =>
                    {
                        foreach (var file in dir.EnumerateFiles())
                        {
                            var newName = Path.Combine(newPath, FileService.ConvertAbsToRel(oldPath, file.FullName));
                            file.MoveTo(newName);
                        }
                        foreach (var sub in dir.EnumerateDirectories())
                        {
                            var newName = Path.Combine(newPath, FileService.ConvertAbsToRel(oldPath, sub.FullName));
                            if (!Directory.Exists(newName))
                            {
                                Directory.CreateDirectory(newName);
                            }
                            action(sub);
                        }
                    };
                    action = moveTask;
                    moveTask(new DirectoryInfo(oldPath));
                    Directory.Delete(oldPath, true);
                }
                MD5DataPath = Config.MD5DataPath.StartsWith('.') ?
                    Path.Combine(Config.InstallPath, Config.MD5DataPath) :
                    Config.MD5DataPath;
                SaveMD5Data();
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
            }
            finally
            {
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                if (Log is FileLogger) ((FileLogger)Log).Path = Path.Combine(LogPath, "LocalData.log");
                Log.LogInfo($"Move work finished: {Config.InstallPath} -> {newPath}");
            }
        }

        public void ReadMD5Data()
        {
            FileHashData = new MD5DataFile();
            StreamReader r = new StreamReader(MD5DataPath);
            try
            {
                string json = r.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                {
                    FileHashData = JsonSerializer.Deserialize<MD5DataFile>(json) ?? new MD5DataFile();
                }
                r.Close(); r.Dispose();
            }
            catch (JsonException)
            {
                // Json反序列化失败，考虑重新创建MD5数据库
                r.Close(); r.Dispose();
                File.Delete(MD5DataPath);
                File.Create(MD5DataPath);
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
                r.Close(); r.Dispose();
            }
            foreach (var item in FileHashData.Data)
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

        public void SaveMD5Data(bool VersionRefresh = true)
        {
            try
            {
                if (VersionRefresh)
                    FileHashData.Version = CurrentVersion;
                using (FileStream fs = new FileStream(MD5DataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.SetLength(0);
                    var exp1 = from i in MD5Data
                               select new KeyValuePair<string, string>(i.Key.Replace(Path.DirectorySeparatorChar, '/'), i.Value);
                    FileHashData.Data = exp1.ToDictionary();
                    sw.Write(JsonSerializer.Serialize(FileHashData, new JsonSerializerOptions
                    {
                        WriteIndented = Debugger.IsAttached
                    }));
                    sw.Flush();
                }
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
            }
        }

        public void ScanDir(bool VersionRefresh = true)
        {
            foreach (var _file in MD5Data.Keys)
            {
                if (_file is null)
                    continue;
                var file = _file.StartsWith('.') ?
                    Path.Combine(Config.InstallPath, _file) : _file;
                if (!File.Exists(file) && MD5Data.TryRemove(_file, out _))
                {
                    MD5Update.Add((DataRowState.Deleted, _file));
                }
                if (IsUserFile(_file) && MD5Data.TryRemove(_file, out _))
                {
                    MD5Update.Add((DataRowState.Deleted, _file));
                }
            }
            // 层序遍历文件树
            Stack<string> stack = new Stack<string>();
            List<string> files = new List<string>();
            stack.Push(Config.InstallPath);
            while (stack.Count > 0)
            {
                string cur = stack.Pop();
                files.AddRange(from f in Directory.GetFiles(cur)
                               where !IsUserFile(f, LangEnabled)
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
                    var relFile = FileService.ConvertAbsToRel(Config.InstallPath, file);
                    var hash = FileService.GetFileMd5Hash(file);
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
            SaveMD5Data(VersionRefresh);
        }

        public static bool IsUserFile(string filename)
        {
            filename = filename.Replace(Path.DirectorySeparatorChar, '/');
            if (filename.Contains("/git/") || filename.Contains("bin/") || filename.Contains("/obj/") || filename.Contains("/x64/"))
                return true;
            if (filename.Contains("/vs/") || filename.Contains("/.vs/") || filename.Contains("/.vscode/"))
                return true;
            if (filename.EndsWith("gz") || filename.EndsWith("log") || filename.EndsWith("csv"))
                return true;
            if (filename.EndsWith(".gitignore") || filename.EndsWith(".gitattributes"))
                return true;
            if (filename.EndsWith("AI.cpp") || filename.EndsWith("AI.py"))
                return true;
            if (filename.EndsWith("hash.json"))
                return true;
            return false;
        }

        public static bool IsUserFile(string filename, Dictionary<LanguageOption, (bool, string)> dict)
        {
            if (filename.Contains("AI.cpp"))
                dict[LanguageOption.cpp] = (true, filename);
            if (filename.Contains("AI.py"))
                dict[LanguageOption.python] = (true, filename);
            return IsUserFile(filename);
        }

        public static int CountFile(string folder, string? root = null)
        {
            int result = (from f in Directory.EnumerateFiles(folder)
                          let t = FileService.ConvertAbsToRel(root ?? folder, f)
                          where !IsUserFile(t)
                          select f).Count();
            foreach (var d in Directory.EnumerateDirectories(folder))
            {
                result += CountFile(d, root ?? folder);
            }
            return result;
        }
    }
}
