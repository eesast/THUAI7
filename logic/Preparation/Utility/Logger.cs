using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Timothy.FrameRateTask;

namespace Preparation.Utility.Logging;

public struct LogInfo
{
    public string FileName;
    public string Info;
}

public class LogQueue
{
    public static LogQueue Global { get; } = new();
    private static readonly object queueLock = new();

    private readonly Queue<LogInfo> logInfoQueue = new();

    public async Task Commit(LogInfo logInfo)
    {
        await Task.Run(() =>
        {
            lock (queueLock) logInfoQueue.Enqueue(logInfo);
        });
    }

    private LogQueue()
    {
        new Thread(() =>
        {
            new FrameRateTaskExecutor<int>(
                loopCondition: () => Global != null,
                loopToDo: () =>
                {
                    lock (queueLock)
                    {
                        while (logInfoQueue.Count != 0)
                        {
                            var logInfo = logInfoQueue.Dequeue();
                            File.AppendAllText(logInfo.FileName, logInfo.Info + Environment.NewLine);
                        }
                    }
                },
                timeInterval: 200,
                finallyReturn: () => 0
                ).Start();
        })
        { IsBackground = true }.Start();
    }
}

public class Logger(string module, string file)
{
    public void ConsoleLog(string msg, bool Duplicate = false)
    {
        var info = $"[{module}]{msg}";
        Console.WriteLine(info);
        if (Duplicate)
            _ = LogQueue.Global.Commit(new()
            {
                FileName = file,
                Info = info
            });
    }
    public void ConsoleLogDebug(string msg, bool Duplicate = false)
    {
#if DEBUG
        var info = $"[{module}]{msg}";
        Console.WriteLine(info);
        if (Duplicate)
            _ = LogQueue.Global.Commit(new()
            {
                FileName = file,
                Info = info
            });
#endif
    }
    public static string TypeName(object obj)
    {
        return obj.GetType().Name;
    }
    public static string ObjInfo(object obj, string msg)
    {
        return $"<{TypeName(obj)} {msg}>";
    }
}
