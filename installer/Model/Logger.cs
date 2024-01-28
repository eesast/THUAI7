using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public abstract class Logger : IDisposable
    {
        private int jobID = 0;
        public abstract DateTime LastRecordTime { get; }
        protected abstract bool IsEnabled(LogLevel level);
        protected abstract void Log(LogLevel logLevel, int eventId, string message);
        protected abstract void Log(LogLevel logLevel, string message);
        public int StartNew() => (jobID++);
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
        protected override bool IsEnabled(LogLevel logLevel)
        {
            if (Debugger.IsAttached)
            {
                return logLevel >= LogLevel.Trace;
            }
            else
            {
                return logLevel >= LogLevel.Warning;
            }
        }
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
        }
    }
    public class FileLogger : Logger
    {
        private StreamWriter writer;
        private DateTime time = DateTime.MinValue;
        public override DateTime LastRecordTime => time;
        public FileLogger(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
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
            var option = new FileStreamOptions();
            option.Mode = FileMode.Append;
            option.Access = FileAccess.Write;
            option.Share = FileShare.Read;
            writer = new StreamWriter(path, Encoding.UTF8, option);
        }
        protected override bool IsEnabled(LogLevel logLevel)
        {
            if (Debugger.IsAttached)
            {
                return logLevel >= LogLevel.Trace;
            }
            else
            {
                return logLevel >= LogLevel.Warning;
            }
        }
        protected void LogDate()
        {
            if (DateTime.Now.Date != time.Date)
            {
                time = DateTime.Now;
                writer.WriteLine($"\nLogged on {time:yyyy-MM-dd}\n\n");
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
                    writer.WriteLine("\nError happens!");
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Error: {message}\n");
                    break;
                case LogLevel.Critical:
                    writer.WriteLine("\nCritical error happens!");
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, {eventId}] Critical: {message}\n");
                    break;
                default:
                    break;
            }
            writer.Flush();
        }

        protected override void Log(LogLevel logLevel, string message)
        {
            if (!IsEnabled(logLevel))
                return;
            LogDate();
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
                    writer.WriteLine("\nError happens!");
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Error: {message}\n");
                    break;
                case LogLevel.Critical:
                    writer.WriteLine("\nCritical error happens!");
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}, ~] Critical: {message}\n");
                    break;
                default:
                    break;
            }
            writer.Flush();
        }
        public override void Dispose()
        {
            writer.Dispose();
            base.Dispose();
        }
    }

    public class ExceptionStack
    {
        public Logger logger;
        protected ConcurrentStack<Exception> Exceptions;
        protected object? Source;
        public event EventHandler? OnFailed;
        public event EventHandler? OnFailProcessed;
        public event EventHandler? OnFailClear;
        public int Count { get => Exceptions.Count; }
        public ExceptionStack(Logger _logger, object? _source = null)
        {
            logger = _logger;
            Exceptions = new ConcurrentStack<Exception>();
            Source = _source;
        }
        public void Push(Exception e, int eventID = -1)
        {
            if (eventID > 0)
            {
                e.Data.Add("Event ID", eventID);
                logger.LogError(eventID, $"{e}: {e.Message}");
            }
            else
            {
                logger.LogError($"{e}: {e.Message}");
            }
            Exceptions.Push(e);
            OnFailed?.Invoke(Source, new EventArgs());
        }
        public Exception? Pop()
        {
            Exception? e;
            if (!Exceptions.TryPop(out e))
                e = null;
            else
                OnFailProcessed?.Invoke(Source, new EventArgs());
            return e;
        }
        public void Clear()
        {
            bool invoke = Count > 0;
            Exceptions.Clear();
            if (invoke)
                OnFailClear?.Invoke(Source, new EventArgs());
        }
    }
}