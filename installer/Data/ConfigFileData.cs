using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace installer.Data
{
    public enum LanguageOption
    {
        cpp = 0, python = 1
    }
    public record CommandFile
    {
        public bool Enabled { get; set; } = true;

        public string IP { get; set; } = "127.0.0.1";

        public string Port { get; set; } = "8888";

        public string TeamID { get; set; } = "0";

        public string PlayerID { get; set; } = "0";

        public string SweeperType { get; set; } = "0";

        public string PlaybackFile { get; set; } = "CLGG!@#$%^&*()_+";

        public double PlaybackSpeed { get; set; } = 2.0;

        public LanguageOption Language { get; set; } = LanguageOption.cpp;

        public int LaunchID { get; set; } = 0;

        public bool Launched { get; set; } = false;
    }

    public record ConfigDataFile
    {
        public string Description { get; set; } = "THUAI7-2024";
        public string MD5DataPath { get; set; } = ".\\hash.json";
        public string InstallPath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "THUAI7"
        );
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Remembered { get; set; } = false;
        public CommandFile Commands { get; set; } = new CommandFile();
    }

    public class Command
    {
        public Command(CommandFile? f = null) => file = f ?? new CommandFile();
        public event EventHandler? OnMemoryChanged;
        public CommandFile file;
        public bool Enabled
        {
            get => file.Enabled;
            set
            {
                var temp = file.Enabled;
                file.Enabled = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }


        public string IP
        {
            get => file.IP;
            set
            {
                var temp = file.IP;
                file.IP = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }

        public string Port
        {
            get => file.Port;
            set
            {
                var temp = file.Port;
                file.Port = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }


        public string TeamID
        {
            get => file.TeamID;
            set
            {
                var temp = file.TeamID;
                file.TeamID = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }


        public string PlayerID
        {
            get => file.PlayerID;
            set
            {
                var temp = file.PlayerID;
                file.PlayerID = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }


        public string SweeperType
        {
            get => file.SweeperType;
            set
            {
                var temp = file.SweeperType;
                file.SweeperType = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }


        public string PlaybackFile
        {
            get => file.PlaybackFile;
            set
            {
                var temp = file.PlaybackFile;
                file.PlaybackFile = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }


        public double PlaybackSpeed
        {
            get => file.PlaybackSpeed;
            set
            {
                var temp = file.PlaybackSpeed;
                file.PlaybackSpeed = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }


        public LanguageOption Language
        {
            get => file.Language;
            set
            {
                var temp = file.Language;
                file.Language = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }


        public int LaunchID
        {
            get => file.LaunchID;
            set
            {
                var temp = file.LaunchID;
                file.LaunchID = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }


        public bool Launched
        {
            get => file.Launched;
            set
            {
                var temp = file.Launched;
                file.Launched = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }

    }

    public class ConfigData
    {
        public ConfigData(string? p = null, bool autoSave = true)
        {
            path = string.IsNullOrEmpty(p) ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "THUAI7.json") : p;
            file = new ConfigDataFile();
            com = new Command(file.Commands);
            ReadFile();

            if (autoSave)
                OnMemoryChanged += (_, _) => SaveFile();
            watcher = new FileSystemWatcher(Directory.GetParent(path)?.FullName ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            watcher.Filter = "THUAI*.json";
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            watcher.Changed += (_, _) =>
            {
                ReadFile();
                OnFileChanged?.Invoke(this, new EventArgs());
            };
        }

        public void ReadFile()
        {
            try
            {
                using (FileStream s = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (StreamReader r = new StreamReader(s))
                {
                    var f = JsonSerializer.Deserialize<ConfigDataFile>(r.ReadToEnd());
                    if (f is null)
                        throw new JsonException();
                    else file = f;
                }
            }
            catch (Exception)
            {
                file = new ConfigDataFile();
            }
            com = new Command(file.Commands);
            com.OnMemoryChanged += (_, _) => OnMemoryChanged?.Invoke(this, new EventArgs());
        }

        public void SaveFile()
        {
            file.Commands = com.file;
            using FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using StreamWriter sw = new StreamWriter(fs);
            fs.SetLength(0);
            sw.Write(JsonSerializer.Serialize(file));
            sw.Flush();
        }

        public event EventHandler? OnMemoryChanged;
        public event EventHandler? OnFileChanged;

        protected string path;
        protected ConfigDataFile file;
        protected FileSystemWatcher watcher;
        protected Command com;

        public string Description
        {
            get => file.Description;
            set
            {
                var temp = file.Description;
                file.Description = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }

        public string MD5DataPath
        {
            get => file.MD5DataPath;
            set
            {
                var temp = file.MD5DataPath;
                file.MD5DataPath = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }

        public string InstallPath
        {
            get => file.InstallPath;
            set
            {
                var temp = file.InstallPath;
                file.InstallPath = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }

        public string Token
        {
            get => file.Token;
            set
            {
                var temp = file.Token;
                file.Token = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }

        public string UserName
        {
            get => file.UserName;
            set
            {
                var temp = file.UserName;
                file.UserName = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }

        public string Password
        {
            get => file.Password;
            set
            {
                var temp = file.Password;
                file.Password = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }

        public bool Remembered
        {
            get => file.Remembered;
            set
            {
                var temp = file.Remembered;
                file.Remembered = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }

        public Command Commands
        {
            get => com;
            set
            {
                var temp = com;
                com = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }
    }

}
