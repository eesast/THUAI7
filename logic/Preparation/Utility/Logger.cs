using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Timothy.FrameRateTask;

namespace Preparation.Utility.Logging;

public class LogQueue
{
    public static LogQueue Global { get; } = new();
    private static uint logNum = 0;
    private static uint logCopyNum = 0;
    private static readonly object queueLock = new();

    private readonly Queue<string> logInfoQueue = new();

    public void Commit(string info)
    {
        lock (queueLock) logInfoQueue.Enqueue(info);
    }

    public static bool IsClosed { get; private set; } = false;
    public static void Close()
    {
        if (IsClosed) return;
        LogWrite();
        LogCopy();
        IsClosed = true;
    }

    static void LogCopy()
    {
        if (IsClosed) return;
        string copyPath = $"{LoggingData.ServerLogPath}-copy{logCopyNum}.txt";
        if (File.Exists(copyPath))
            File.Delete(copyPath);
        File.Copy(LoggingData.ServerLogPath, copyPath);
        logCopyNum++;
        File.Delete(LoggingData.ServerLogPath);
        logNum = 0;
    }
    static void LogWrite()
    {
        if (IsClosed) return;
        lock (queueLock)
        {
            while (Global.logInfoQueue.Count != 0)
            {
                var info = Global.logInfoQueue.Dequeue();
                File.AppendAllText(LoggingData.ServerLogPath, info + Environment.NewLine);
                logNum++;
                if (logNum >= LoggingData.MaxLogNum)
                    LogCopy();
            }
        }
    }

    private LogQueue()
    {
        if (File.Exists(LoggingData.ServerLogPath))
            File.Delete(LoggingData.ServerLogPath);
        File.AppendAllText(LoggingData.ServerLogPath, $"[{Logger.NowDate()}]" + Environment.NewLine);
        new Thread(() =>
        {
            new FrameRateTaskExecutor<int>(
                loopCondition: () => Global != null,
                loopToDo: LogWrite,
                timeInterval: 100,
                finallyReturn: () =>
                {
                    Close();
                    return 0;
                }
                ).Start();
        })
        { IsBackground = true }.Start();
    }
}

public class Logger(string module)
{
    public readonly string Module = module;
    public bool Enable { get; set; } = true;
    public bool Background { get; set; } = false;

    public void ConsoleLog(string msg, bool Duplicate = true)
    {
        var info = $"[{NowTime()}][{Module}] {msg}";
        if (Enable)
        {
            if (!Background)
                Console.WriteLine(info);
            if (Duplicate)
                LogQueue.Global.Commit(info);
        }
    }
    public void ConsoleLogDebug(string msg, bool Duplicate = true)
    {
#if DEBUG
        var info = $"[{NowTime()}][{Module}] {msg}";
        if (Enable)
        {
            if (!Background)
                Console.WriteLine(info);
            if (Duplicate)
                LogQueue.Global.Commit(info);
        }
#endif
    }
    public static void RawConsoleLog(string msg, bool Duplicate = true)
    {
        Console.WriteLine(msg);
        if (Duplicate)
            LogQueue.Global.Commit(msg);
    }
    public static void RawConsoleLogDebug(string msg, bool Duplicate = true)
    {
#if DEBUG
        Console.WriteLine(msg);
        if (Duplicate)
            LogQueue.Global.Commit(msg);
#endif
    }

    public static string TypeName(object obj)
        => obj.GetType().Name;
    public static string TypeName(Type tp)
        => tp.Name;
    public static string ObjInfo(object obj, string msg = "")
        => msg == "" ? $"<{TypeName(obj)}>"
                     : $"<{TypeName(obj)} {msg}>";
    public static string ObjInfo(Type tp, string msg = "")
        => msg == "" ? $"<{TypeName(tp)}>"
                     : $"<{TypeName(tp)} {msg}>";

    public static string NowTime()
    {
        DateTime now = DateTime.Now;
        return $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D3}";
    }
    public static string NowDate()
    {
        DateTime now = DateTime.Today;
        return $"{now.Year:D4}/{now.Month:D2}/{now.Day:D2} {now.DayOfWeek}";
    }
}

public static class LoggingData
{
    public const string ServerLogPath = "log.txt";
    public const uint MaxLogNum = 5000;
}
