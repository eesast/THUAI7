using installer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public string? PlaybackFile { get; set; } = "";

        public double PlaybackSpeed { get; set; } = 2.0;

        public LanguageOption Language { get; set; } = LanguageOption.cpp;

        public int LaunchID { get; set; } = 0;

        public bool Launched { get; set; } = false;

        public int TeamID { get; set; } = 0;

        public int PlayerID { get; set; } = 2024;

        public int ShipType { get; set; } = 0;
    }

    public class Player : NotificationObject
    {
        protected int teamID = 0;
        protected int playerID = 0;
        protected string playerMode = "API";
        protected bool shipTypePickerEnabled = false;
        protected int shipType = 0;
        [JsonInclude]
        public int TeamID
        {
            get => teamID;
            set
            {
                teamID = value;
                OnPropertyChanged();
            }
        }
        [JsonInclude]
        public int PlayerID
        {
            get => playerID;
            set
            {
                playerID = value;
                OnPropertyChanged();
            }
        }
        [JsonInclude]
        public string PlayerMode
        {
            get => playerMode;
            set
            {
                playerMode = value;
                if (playerMode == "API")
                {
                    ShipTypePickerEnabled = false;
                    ShipType = 0;
                }
                else
                {
                    ShipTypePickerEnabled = true;
                }
                OnPropertyChanged();
            }
        }
        [JsonInclude]
        public bool ShipTypePickerEnabled
        {
            get => shipTypePickerEnabled;
            set
            {
                shipTypePickerEnabled = value;
                OnPropertyChanged();
            }
        }
        [JsonInclude]
        public int ShipType
        {
            get => shipType;
            set
            {
                shipType = value;
                OnPropertyChanged();
            }
        }
    }

    public record ConfigDataFile
    {
        public string Description { get; set; } = "THUAI7-2024";
        public string MD5DataPath { get; set; } = ".\\hash.json";
        public bool Installed { get; set; } = false;
        public string InstallPath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "THUAI7", "Data"
        );
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Remembered { get; set; } = false;
        public CommandFile Commands { get; set; } = new CommandFile();
        public List<Player> Players { get; set; } = new List<Player>();
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


        public string? PlaybackFile
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


        public int TeamID
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


        public int PlayerID
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


        public int ShipType
        {
            get => file.ShipType;
            set
            {
                var temp = file.ShipType;
                file.ShipType = value;
                if (temp != value)
                    OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }
    }

    public class ConfigData
    {
        public ConfigData(string? p = null, bool autoSave = true)
        {
            var dataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "THUAI7");
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);
            path = string.IsNullOrEmpty(p) ? Path.Combine(dataDir, "config.json") : p;
            file = new ConfigDataFile();
            com = new Command(file.Commands);
            Players = new ObservableCollection<Player>();
            Players.CollectionChanged += (sender, args) =>
            {
                if (args.NewItems is not null)
                {
                    foreach (var item in args.NewItems)
                    {
                        ((Player)item).PropertyChanged += (_, _) => OnMemoryChanged?.Invoke(this, new EventArgs());
                    }
                }
                OnMemoryChanged?.Invoke(this, new EventArgs());
            };
            ReadFile();

            if (autoSave)
                OnMemoryChanged += (_, _) => SaveFile();
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
            Players.Clear();
            file.Players.ForEach(p => Players.Add(p));
            com.OnMemoryChanged += (_, _) => OnMemoryChanged?.Invoke(this, new EventArgs());
        }

        public void SaveFile()
        {
            file.Commands = com.file;
            file.Players = new List<Player>(Players);
            using FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using StreamWriter sw = new StreamWriter(fs);
            fs.SetLength(0);
            sw.Write(JsonSerializer.Serialize(file));
            sw.Flush();
        }

        public event EventHandler? OnMemoryChanged;
        public event EventHandler? OnFileChanged;
        public ObservableCollection<Player> Players;

        protected string path;
        protected ConfigDataFile file;
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

        public bool Installed
        {
            get => file.Installed;
            set
            {
                var temp = file.Installed;
                file.Installed = value;
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
                com.OnMemoryChanged += (_, _) => OnMemoryChanged?.Invoke(this, new EventArgs());
            }
        }
    }

}
