using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace installer
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault },
        DataMimeType = @"application/com.thueesast.thuaiplayback",
        DataPathSuffix = "thuaipb")]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}