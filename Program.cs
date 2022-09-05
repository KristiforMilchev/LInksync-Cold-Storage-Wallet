using LInksync_Cold_Storage_Wallet.Data;
using SYNCWallet.Services.Definitions;
using SYNCWallet.Services.Implementation;
using NFTLock.Data;
using SYNCWallet.Data;
using LInksync_Cold_Storage_Wallet;
using ElectronNET.API;
using Microsoft.AspNetCore.Server.Kestrel.Https;

var builder = WebApplication.CreateBuilder(args);
 // --- Add electron...


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddElectron();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped(typeof(IUtilities), typeof(Utilities));
builder.Services.AddScoped(typeof(IHardwareService), typeof(HardwareService));
builder.Services.AddScoped(typeof(IAuthenicationService), typeof(AuthenicationHandler));
builder.Services.AddScoped(typeof(IContractService), typeof(ContractService));
builder.Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
builder.Services.AddScoped(typeof(ICommunication), typeof(Communication));


 builder.WebHost.ConfigureKestrel(o => {
}).UseElectron(args);
 

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
// Open the Electron-Window here
System.Console.WriteLine("Starting Main window");


Task.Run(() => {
  var window =  Electron.WindowManager.CreateWindowAsync();
});

app.Run();