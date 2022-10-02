namespace LInksync_Cold_Storage_Wallet.Pages
{
    
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System.Net;
    using System.Timers;
    using System.Numerics;
    using ArduinoUploader.Hardware;
    using System.IO.Ports;
    using System.Diagnostics;
    using SYNCWallet.Models;
    using SYNCWallet.Services;
    using SYNCWallet.Services.Definitions;
    using static SYNCWallet.Models.Enums;
    
    public partial class HardwareSelect
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        
        private ICommunication Communication { get; set; }
        private IAuthenicationService AuthenicationHandler { get; set; }
        private IHardwareService HardwareService { get; set; }
        private IUtilities Utilities { get; set; }
        private string Address { get; set; }
        private string Port{ get; set; }
        private string ShowPicker { get; set; }
        int Attempts { get; set; }

        public ArduinoModel DeviceModel { get; set; }
        public List<ArduinoModel> Devices { get; set; }
        TriggerLoader CurrentLoader { get; set; }
        public  int OperatingSystem { get; set; }
        System.Timers.Timer aTimer { get; set; }

      
        protected override async Task OnInitializedAsync()
        {

            AuthenicationHandler = ServiceHelper.GetService<IAuthenicationService>();
            HardwareService = ServiceHelper.GetService<IHardwareService>();
            Communication = ServiceHelper.GetService<ICommunication>();
            Utilities = ServiceHelper.GetService<IUtilities>();
            Attempts = 0;
        

            Communication.Init();
            //Get Supported Devices
            Devices = HardwareService.GetSupportedDevices();
            Port = HardwareService.DeviceConnected();
     
            InitTimer();
        }

       
        void InitTimer()
        {
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 5000;
            aTimer.Start();
        }
        // Specify what you want to happen when the Elapsed event is raised.
        private async void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            Port = HardwareService.DeviceConnected();
            
            Attempts += 1;

            if(Attempts == 2 && string.IsNullOrEmpty(Port))
            {

                await InvokeAsync(() =>
                {
                    ShowPicker = "flex";
                    Communication.TriggerLoader.Invoke("none");
                    StateHasChanged();

                });

                Communication.PublishError("Port not found", "Please connect atmega328 compatible device <a href='' >click here for supported devices.</a>");
                KillTimer();
            }
            
            if(!string.IsNullOrEmpty(Port))
            {
                KillTimer();
            }
        }

        private async Task<bool> CheckDeviceConnected(string port)
        {

            try
            {
                if (!string.IsNullOrEmpty(port))
                {
                    var firmwareUpdated = HardwareService.CreateNewDevice(port, Communication.DeviceType);

                    if (!firmwareUpdated)
                    {

                        await InvokeAsync(() =>
                        {
                            ShowPicker = "flex";
                            Communication.TriggerLoader.Invoke("none");
                            StateHasChanged();

                        });
                        return false;
                    }

                    Communication.ComPort = port;
                    var configStatus = Communication.CheckConfigured(ConfigMode.ColdWallet, HardwareService.Os);
        
                    Communication.TriggerLoader.Invoke("none");
                    if (configStatus && firmwareUpdated)
                    {

                        KillTimer();
                        NavigationManager.NavigateTo("LoginPanel");
                    }
                    else
                    {
                        KillTimer();
                        NavigationManager.NavigateTo("Create");

                    }
                }

            }
            catch (Exception e)
            {

                throw;
            }

            return true;
        }


        private void KillTimer()
        {
            aTimer.Stop();
            aTimer.Dispose();
        }

        private async void DeviceChanged(ArduinoModel deviceType)
        {

            await InvokeAsync(() =>
            {   
                ShowPicker = "none";
                Communication.TriggerLoader.Invoke("flex");
                StateHasChanged();

            });


            InvokeAsync(async () =>
            {

                DeviceModel = deviceType;
                Communication.DeviceType = deviceType;
                Task.Run(() => CheckDeviceConnected(Port));
                StateHasChanged();

            });

            

        }
    }
}