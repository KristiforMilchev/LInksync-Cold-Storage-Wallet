using LInksync_Cold_Storage_Wallet.Data;
using SYNCWallet.Services.Definitions;
using SYNCWallet.Services.Implementation;
using NFTLock.Data;
using SYNCWallet.Data;
using LInksync_Cold_Storage_Wallet;
using ElectronNET.API;
using ElectronNET.API.Entities;
using LInksync_Cold_Storage_Wallet.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


try
{
  //  builder.WebHost.UseElectron(args);
   // Electron.ReadAuth();

}
catch (Exception e)
{
    Console.WriteLine(e);
    
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped(typeof(IUtilities), typeof(Utilities));
builder.Services.AddScoped(typeof(IHardwareService), typeof(HardwareService));
builder.Services.AddScoped(typeof(IAuthenicationService), typeof(AuthenicationHandler));
builder.Services.AddScoped(typeof(IContractService), typeof(ContractService));
builder.Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
builder.Services.AddScoped(typeof(ICommunication), typeof(Communication));

 
 

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
var scope = app.Services.CreateScope();

Initializer.Provider = scope.ServiceProvider;

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

try
{
    //Add the Electron 
    if (HybridSupport.IsElectronActive)
    {
        CreateWindow();
    }

}
catch (Exception e)
{
    Console.WriteLine(e);
    
}

async void CreateWindow()
{
    BrowserWindowOptions bo = new BrowserWindowOptions();
    bo.AutoHideMenuBar = true;
    var window = await Electron.WindowManager.CreateWindowAsync(bo);
        window.OnClosed += () => {
        Electron.App.Quit();
    };
}

void Window_OncClosed()
{
    Electron.App.Exit();
}//end  Window_OncClosed

void Window_OnMaximize()
{
    Electron.Dialog.ShowErrorBox("Info Box", "Hi, you Just maimixed you Electron App");
}//end  Window_OncClosed

void Window_OnMinimize()
{
    Electron.Dialog.ShowMessageBoxAsync("Application minimized");
}//end  Window_OncClosed

app.Run();