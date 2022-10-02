namespace LInksync_Cold_Storage_Wallet.Pages
{
    using System.Net;
    using System.Timers;
    using System.Numerics;
    using ArduinoUploader.Hardware;
    using System.IO.Ports;
    using System.Diagnostics;
    using SYNCWallet.Services;
    using SYNCWallet.Services.Definitions;
    using static SYNCWallet.Models.Enums;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    
    public partial class Index
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        
        private ICommunication Communication { get; set; }
        private IAuthenicationService AuthenicationHandler { get; set; }
        private IHardwareService HardwareService { get; set; }
        private IUtilities Utilities {get; set;}
        private string Port{ get; set; }



        protected override Task OnAfterRenderAsync(bool firstRender)
        {

            return base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {

            AuthenicationHandler = ServiceHelper.GetService<IAuthenicationService>();
            HardwareService = ServiceHelper.GetService<IHardwareService>();
            Communication = ServiceHelper.GetService<ICommunication>();
            Utilities =  ServiceHelper.GetService<IUtilities>();
            Communication.Init();
       
            Port = HardwareService.DeviceConnected();
        }

        private void LoadHotWallet()
        {
            var configStatus = Communication.CheckConfigured(ConfigMode.HotWallet);
            if(configStatus)
                NavigationManager.NavigateTo("LoginPanel");

            else
                NavigationManager.NavigateTo("Create");

        }

        private void LoadColdWallet()
        {
            // if(Communication.Os == 1)
            // {
            //   
            // }
            // else
            // {
            //     Communication.PublishError("Not supported",$"This feature is not supported under Linux and Mac due to kernal compatabilities.");
            // }
            Communication.SoftwareType = ConfigMode.ColdWallet;
            NavigationManager.NavigateTo("HardwareSelect");
        }
    }
}