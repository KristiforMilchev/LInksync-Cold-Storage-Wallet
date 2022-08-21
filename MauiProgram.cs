using ArduinoUploader.Hardware;
using Newtonsoft.Json;
using NFTLock.Data;
using SYNCWallet.Data;
using SYNCWallet.Models;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;
using SYNCWallet.Services.Implementation;
using System.Diagnostics;
using System.IO.Ports;
using System.Numerics;
using static SYNCWallet.Models.GithubTokensModel;

namespace SYNCWallet;

public static class MauiProgram
{
     public static MauiApp MauiApp { get; set; }


    //TODO Clean all global dependebcuesm create a wraooer for all varibles and extract the hardware reading code to a serivce.
    public static MauiApp CreateMauiApp()
    {

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddLogging();
        builder.Services.AddScoped(typeof(IUtilities), typeof(Utilities));
        builder.Services.AddScoped(typeof(IHardwareService), typeof(HardwareService));
        builder.Services.AddScoped(typeof(IAuthenicationService), typeof(AuthenicationHandler));
        builder.Services.AddScoped(typeof(IContractService), typeof(ContractService));
        builder.Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
        builder.Services.AddScoped(typeof(ICommunication), typeof(Communication));

    
        #if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        #endif
       

        MauiApp = builder.Build();

        return MauiApp;
        
    }

    
}
