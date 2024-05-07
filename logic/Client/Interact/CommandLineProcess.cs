using System.Diagnostics;

namespace Client.Interact
{
    public static class CommandLineProcess
    {
        public static void StartProcess()
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process process = new() { StartInfo = startInfo };
            process.Start();

            // 写入命令行
            process.StandardInput.WriteLine("echo Hello, World!");

            // 读取命令行输出
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

            process.WaitForExit();
        }
    }
}
