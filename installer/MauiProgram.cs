using Microsoft.Extensions.Logging;

namespace installer
{
    public static class MauiProgram
    {
        public static Model.Downloader Downloader = new Model.Downloader();
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // 此处填写Secret ID和Secret Key
            Downloader.Cloud.UpdateSecret("***",
                "***");;

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}