namespace LInksync_Cold_Storage_Wallet.Pages
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System.Net;
    using System.Numerics;
    using NFTLock.Data;
    using Newtonsoft.Json;
    using SYNCWallet.Models;
    using SYNCWallet.Services;
    using SYNCWallet.Services.Definitions;
    public partial class LoginPanel
    {       
        
        
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        
        private string Password{ get; set; }
        IHardwareService HardwareService { get; set; }
        ICommunication Communication { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HardwareService = ServiceHelper.GetService<IHardwareService>();
            Communication = ServiceHelper.GetService<ICommunication>();
            Communication.LoginAttempt = Callback;
            if (Communication.IsLogged && !string.IsNullOrEmpty(Communication.PublicAddress))
            {
                NavigationManager.NavigateTo("Landing");

            }
        }

        void Callback(bool status)
        {
            Communication.TriggerLoader.Invoke("none");

            InvokeAsync(() =>
            {
                Communication.ShowLoader = "none";
                Communication.HideTokenList = "";
                Communication.HideTokenSend = "none";
                Communication.ShowPinPanel = "none";
                Communication.Receipt = "none";

                StateHasChanged();

            });

            if (Communication.IsLogged && !string.IsNullOrEmpty(Communication.PublicAddress))
            {
                NavigationManager.NavigateTo("Landing");
            }

        }



        private void LoginPublic()
        {
            if (Password.Length < 8)
            {
                //TODO send error message.
                return;
            }

            var passwrod = HardwareService.Encrypt("iV1z@$H8",Password);
            Communication.Pass = Password;
            if(Communication.TriggerLoader != null)
              Communication.TriggerLoader.Invoke("flex");

            switch(Communication.SoftwareType)
            {
                case Enums.ConfigMode.ColdWallet:
                    Communication.WriteState(JsonConvert.SerializeObject(new HardwareWallet
                        {
                            Cmd = "Login",
                            Password = passwrod,
                            PrivateKey = "3"
                        }));
                     break;
                case Enums.ConfigMode.HotWallet:
                    Communication.ReadInternalStorage(passwrod, HardwareService.Os);
                    break;
            }

        }
    }
}