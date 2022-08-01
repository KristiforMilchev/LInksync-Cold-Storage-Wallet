using Foundation;
using SYNCWallet;

namespace NFTLock;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp().GetAwaiter().GetResult();
}
