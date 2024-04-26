using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

#pragma warning disable CA1416

namespace installer.Model
{
    public static class LoggerProvider
    {
        public static Logger FromConsole() => new ConsoleLogger();
        public static Logger FromFile(string path) => new FileLogger(path);
    }
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6,
    }
    public class LogRecord
    {
        public LogLevel Level { get; set; }
        public string Color
        {
            get
            {
                return Level switch
                {
                    LogLevel.Trace => "Black",
                    LogLevel.Debug => "Black",
                    LogLevel.Information => "Black",
                    LogLevel.Warning => "Tan",
                    LogLevel.Error => "Red",
                    LogLevel.Critical => "DarkRed",
                    _ => "White",
                };
            }
        }
        public string Message { get; set; } = string.Empty;
    }

    public abstract class Logger : IDisposable
    {
        private int jobID = 0;
        public List<Logger> Partner = [];
        public string PartnerInfo = string.Empty;
        public Dictionary<LogLevel, int> CountDict = new()
        {
            { LogLevel.Trace, 0 }, { LogLevel.Debug, 1 },
            { LogLevel.Information, 2 }, { LogLevel.Warning, 3 },
            { LogLevel.Error, 4 }, { LogLevel.Critical, 5 },
            { LogLevel.None, 6 }
        };
        public abstract DateTime LastRecordTime { get; }
        protected virtual bool IsEnabled(LogLevel level)
        {
            if (Debugger.IsAttached)
            {
                return level >= LogLevel.Trace;
            }
            else
            {
                return level >= LogLevel.Information;
            }
        }
        protected virtual void Log(LogLevel logLevel, int eventId, string message)
        {
            CountDict[logLevel] += 1;
            Partner.ForEach(i => i.Log(logLevel, eventId, PartnerInfo + message));
        }
        protected virtual void Log(LogLevel logLevel, string message)
        {
            CountDict[logLevel] += 1;
            Partner.ForEach(i => i.Log(logLevel, PartnerInfo + message));
        }
        public int StartNew() => (jobID++);
        public void LogDebug(int eventId, string message)
            => Log(LogLevel.Debug, eventId, message);
        public void LogDebug(string message)
            => Log(LogLevel.Debug, message);
        public void LogInfo(int eventId, string message)
            => Log(LogLevel.Information, eventId, message);
        public void LogInfo(string message)
            => Log(LogLevel.Information, message);
        public void LogWarning(int eventId, string message)
            => Log(LogLevel.Warning, eventId, message);
        public void LogWarning(string message)
            => Log(LogLevel.Warning, message);
        public void LogError(int eventId, string message)
            => Log(LogLevel.Error, eventId, message);
        public void LogError(string message)
            => Log(LogLevel.Error, message);
        public virtual void Dispose() { }
    }
    public class ConsoleLogger : Logger
    {
        private DateTime time = DateTime.MinValue;
        public override DateTime LastRecordTime => time;
        protected void LogDate()
        {
            if (DateTime.Now.Date != time.Date)
            {
                time = DateTime.Now;
                Console.WriteLine($"\nLogged on {time:yyyy-MM-dd}\n\n");
            }

        }
        protected override void Log(LogLevel logLevel, int eventId, string message)
        {
            if (!IsEnabled(logLevel))
                return;
            LogDate();
            switch (logLevel)
            {
                case LogLevel.Trace:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Trace: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Debug: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Info: {message}");
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Warning: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Error: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Critical: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    break;
            }
            base.Log(logLevel, eventId, message);
        }
        protected override void Log(LogLevel logLevel, string message)
        {
            if (!IsEnabled(logLevel))
                return;
            LogDate();
            switch (logLevel)
            {
                case LogLevel.Trace:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Trace: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Debug: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Info: {message}");
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Warning: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Error: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Critical: {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    break;
            }
            base.Log(logLevel, message);
        }
    }
    public class FileLogger : Logger
    {
        private DateTime time = DateTime.MinValue;
        private string path;
        private readonly Mutex mutex = new();
        public string Path
        {
            get => path; set
            {
                path = value;
                LoadTime();
            }
        }
        public override DateTime LastRecordTime => time;
        public FileLogger(string _path)
        {
            path = _path;
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            LoadTime();
        }

        public void LoadTime()
        {
            mutex.WaitOne();
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("Logged on"))
                    {
                        time = Convert.ToDateTime(line.Split(' ').Last());
                    }
                }
            }
            mutex.ReleaseMutex();
        }
        protected void LogDate()
        {
            mutex.WaitOne();
            var writer = new StreamWriter(path, Encoding.UTF8, new FileStreamOptions()
            {
                Mode = FileMode.Append,
                Access = FileAccess.Write
            });
            if (DateTime.Now.Date != time.Date)
            {
                time = DateTime.Now;
                writer.WriteLine($"\nLogged on {time:yyyy-MM-dd}\n");
            }
            writer.Dispose();
            mutex.ReleaseMutex();
        }
        protected override void Log(LogLevel logLevel, int eventId, string message)
        {
            if (!IsEnabled(logLevel))
                return;
            LogDate();
            mutex.WaitOne();
            using var writer = new StreamWriter(path, Encoding.UTF8, new FileStreamOptions()
            {
                Mode = FileMode.Append,
                Access = FileAccess.Write
            });
            switch (logLevel)
            {
                case LogLevel.Trace:
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Trace: {message}");
                    break;
                case LogLevel.Debug:
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Debug: {message}");
                    break;
                case LogLevel.Information:
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Info: {message}");
                    break;
                case LogLevel.Warning:
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Warning: {message}");
                    break;
                case LogLevel.Error:
                    writer.WriteLine("\nError occurred!");
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Error: {message}\n");
                    break;
                case LogLevel.Critical:
                    writer.WriteLine("\nCritical error occurred!");
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Critical: {message}\n");
                    break;
                default:
                    break;
            }
            writer.Flush();
            writer.Dispose();
            mutex.ReleaseMutex();
            base.Log(logLevel, eventId, message);
        }

        protected override void Log(LogLevel logLevel, string message)
        {
            if (!IsEnabled(logLevel))
                return;
            LogDate();
            mutex.WaitOne();
            using var writer = new StreamWriter(path, Encoding.UTF8, new FileStreamOptions()
            {
                Mode = FileMode.Append,
                Access = FileAccess.Write
            });
            switch (logLevel)
            {
                case LogLevel.Trace:
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Trace: {message}");
                    break;
                case LogLevel.Debug:
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Debug: {message}");
                    break;
                case LogLevel.Information:
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Info: {message}");
                    break;
                case LogLevel.Warning:
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Warning: {message}");
                    break;
                case LogLevel.Error:
                    writer.WriteLine("\nError occurred!");
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Error: {message}\n");
                    break;
                case LogLevel.Critical:
                    writer.WriteLine("\nCritical error occurred!");
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Critical: {message}\n");
                    break;
                default:
                    break;
            }
            writer.Flush();
            writer.Dispose();
            mutex.ReleaseMutex();
            base.Log(logLevel, message);
        }
    }
    public class ListLogger : Logger
    {
        protected ConcurrentQueue<LogRecord> Queue = new();
        public ObservableCollection<LogRecord> List = [];
        public override DateTime LastRecordTime => DateTime.Now;
        private Timer timer;
        private DateTime time;
        private int ind;
        public ListLogger()
        {
            ind = 0;
            timer = new Timer(_ =>
            {
                for (int i = ind; i < Queue.Count; i++)
                {
                    List.Add(Queue.ElementAt(i));
                }
                ind = Queue.Count;
            }, null, 0, 100);
        }
        protected override void Log(LogLevel logLevel, int eventId, string message)
        {
            Queue.Enqueue(new LogRecord { Level = logLevel, Message = message });
            base.Log(logLevel, eventId, message);
        }
        protected override void Log(LogLevel logLevel, string message)
        {
            Queue.Enqueue(new LogRecord { Level = logLevel, Message = message });
            base.Log(logLevel, message);
        }
    }
}