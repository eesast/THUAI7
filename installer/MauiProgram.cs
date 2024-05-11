using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using installer.ViewModel;
using installer.Model;
using System.Text;
using System.Diagnostics;
using Microsoft.Maui.LifecycleEvents;

namespace installer
{
    public static class MauiProgram
    {
        // public static Model.Logger logger = Model.LoggerProvider.FromFile(@"E:\bin\log\123.log");
        public static bool ErrorTrigger_WhileDebug = true;
        public static bool RefreshLogs_WhileDebug = false;
        public static string SecretID = "***";
        public static string SecretKey = "***";
        public static MauiApp CreateMauiApp()
        {
            // read SecretID & SecretKey from filePath for debug
            var filePath = Debugger.IsAttached ? "D:\\Secret.csv" : Path.Combine(AppContext.BaseDirectory, "Secret.csv");
            var lines = File.ReadAllLines(filePath);
            if (lines.Length >= 4)
            {
                lines = lines.Select(s => s.Trim().Trim('\r', '\n')).ToArray();
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Convert.FromBase64String(lines[0]);
                    aes.IV = Convert.FromBase64String(lines[1]);
                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream memory = new MemoryStream(Convert.FromBase64String(lines[2])))
                    {
                        using (CryptoStream crypto = new CryptoStream(memory, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(crypto, Encoding.ASCII))
                                SecretID = reader.ReadToEnd();
                        }
                    }
                    using (MemoryStream memory = new MemoryStream(Convert.FromBase64String(lines[3])))
                    {
                        using (CryptoStream crypto = new CryptoStream(memory, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(crypto, Encoding.ASCII))
                                SecretKey = reader.ReadToEnd();
                        }
                    }
                }
            }

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitCore()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var c = builder.Services.AddSingleton<Downloader>().First();

            builder.Services.AddSingleton(FolderPicker.Default);
            builder.Services.AddSingleton(FilePicker.Default);

            AddViewModelService(builder);
            AddPageService(builder);
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static void AddViewModelService(MauiAppBuilder builder)
        {
            var a = typeof(MauiProgram).Assembly;
            foreach (var t in a.GetTypes())
            {
                if ((t.FullName ?? string.Empty).StartsWith($"{a.GetName().Name}.ViewModel") && !t.IsAbstract)
                {
                    builder.Services.AddSingleton(t);
                }
            }
        }
        public static void AddPageService(MauiAppBuilder builder)
        {
            var a = typeof(MauiProgram).Assembly;
            foreach (var t in a.GetTypes())
            {
                if ((t.FullName ?? string.Empty).StartsWith($"{a.GetName().Name}.Page") && !t.IsAbstract)
                {
                    builder.Services.AddSingleton(t);
                }
            }
        }
    }
}