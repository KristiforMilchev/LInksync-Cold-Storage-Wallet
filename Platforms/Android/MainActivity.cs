using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;

namespace NFTLock;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Platform.Init(this, savedInstanceState);
        ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.CaptureVideoOutput, Manifest.Permission.CaptureAudioOutput, Manifest.Permission.CaptureSecureVideoOutput,  Manifest.Permission.Camera, Manifest.Permission.RecordAudio, Manifest.Permission.ModifyAudioSettings }, 0);
    }
}