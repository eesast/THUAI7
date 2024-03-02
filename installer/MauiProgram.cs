using Microsoft.Extensions.Logging;
using System.Reflection;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using installer.ViewModel;
using System.Runtime.CompilerServices;
using installer.Model;

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