using System.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LInksync_Cold_Storage_Wallet.Pages
{
    
    using NFTLock.Data;
    using Nethereum.Contracts;
    using Nethereum.Web3;
    using Newtonsoft.Json;
    using SYNCWallet.Data;
    using SYNCWallet.Models;
    using System.Security.Cryptography;
    using System.Text;
    using SYNCWallet.Services;
    using SYNCWallet.Services.Definitions;
    using SYNCWallet.Services.Implementation;
    using System.Timers;
    


    public partial class Landing
    {
        
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        public string TokenName { get; set; }
        private bool IsChartRendered { get; set; }
        List<Token> Tokens { get; set; }
        private List<NetworkSettings> Networks { get; set; }
        public string WalletAddress { get; set; }
        public NetworkSettings SelectedNetwork { get; set; }
        public Token SelectedToken{ get; set; }
        public TokenContract SelectedContract { get; set; }
        private decimal TokensToSend { get; set; }
        private string ReceiverAddress { get; set; }
        private string Password { get; set; }
        TransactionResult Receipt { get; set; }
        System.Timers.Timer BalanceCheck { get; set; }
        DateTime NextCheck { get; set; }
        IUtilities Utilities { get; set; }
        IAuthenicationService AuthenicationService { get; set; }
        IPaymentService PaymentService { get; set; }
        IHardwareService HardwareService { get; set; }
        ICommunication Communication { get; set; }
        IContractService ContractService { get; set; }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!IsChartRendered)
            {
                Task.Run(() => JS.InvokeAsync<string>("RenderChart"));

                Task.Run(() => JS.InvokeAsync<string>("requestMedia"));
                IsChartRendered = !IsChartRendered;
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {


            Utilities = ServiceHelper.GetService<IUtilities>();
            AuthenicationService = ServiceHelper.GetService<IAuthenicationService>();
            PaymentService = ServiceHelper.GetService<IPaymentService>();
            HardwareService = ServiceHelper.GetService<IHardwareService>();
            ContractService = ServiceHelper.GetService<IContractService>();
            Communication = ServiceHelper.GetService<ICommunication>();
            Communication.LoginAttempt = Callback;

            NextCheck = NextCheck.AddMinutes(1);
            Communication.HideTokenList = "";
            Communication.HideTokenSend = "none";
            Communication.ShowPinPanel = "none";
            Communication.ShowLoader = "none";
            Communication.Receipt = "none";


            BalanceCheck = new System.Timers.Timer();
            BalanceCheck= new System.Timers.Timer();
            BalanceCheck.Elapsed += new ElapsedEventHandler(OnBalanceUpdate);
            BalanceCheck.Interval = 5000;
            BalanceCheck.Start();


            TokenName = "SYNC";

            //Load Network Settings
            Communication.NetworkSettings = await Utilities.SetupNetworks(HardwareService.Os);
            Networks = Communication.NetworkSettings.Where(x=> x.IsProduction == Communication.IsDevelopment).ToList();
            SelectedNetwork = Networks.FirstOrDefault();
            Communication.ActiveNetwork = SelectedNetwork;
            WalletAddress = Communication.GetDefault();
            Tokens = await ContractService.GetNetworkTokensIntial(SelectedNetwork.Id); //Get All tokens and their balance
            Task.Run(() => DefaultToToken());
            NextCheck = DateTime.UtcNow.AddSeconds(20);
        }



        async void Callback(bool status)
        {
            if(status)
            {
                var result = await PaymentService.BeginTransaction();
                Communication.TriggerLoader.Invoke("none");

                if (result != null)
                {
                    await InvokeAsync(() =>
                    {
                        Communication.ShowLoader = "none";
                        Communication.HideTokenList = "none";
                        Communication.HideTokenSend = "none";
                        Communication.ShowPinPanel = "none";
                        Communication.Receipt = "flex";
                        Receipt = result;
                        StateHasChanged();

                    });
                }
            }
            else
                InvokeAsync(() =>
                {
                    Communication.ShowLoader = "none";
                    Communication.HideTokenList = "";
                    Communication.HideTokenSend = "none";
                    Communication.ShowPinPanel = "none";
                    Communication.Receipt = "none";

                    StateHasChanged();

                });

        }

        private void NetworkChanged(NetworkSettings network)
        {
            InvokeAsync(async () =>{
                SelectedNetwork = network;
                Communication.ActiveNetwork = SelectedNetwork;
                Tokens = await ContractService.GetNetworkTokensIntial(SelectedNetwork.Id);
                Communication.LoadedTokens = Tokens;
                StateHasChanged();
            });
        }


        private async void DefaultToToken()
        {
            try
            {
                await InvokeAsync(async () =>
                {
                    if (SelectedNetwork != null)
                    {
                        var updateTokens =  await ContractService.GetNetworkTokens(SelectedNetwork.Id); //Get All tokens and their balance
                        var getNetwork = updateTokens.FirstOrDefault().Contracts.FirstOrDefault().Network;
                        //We don't want  updates in case we have already switched network since last request.
                        if (getNetwork == SelectedNetwork.Id)
                            Tokens = updateTokens;

                        
                        NextCheck = DateTime.UtcNow.AddSeconds(20);
                        StateHasChanged();
                    }

                });
            }
            catch (Exception)
            {

                throw;
            }



        }

        private void SelectToken(TokenContract contract, Token token)
        {
            
            InvokeAsync(() =>
            {
                SelectedToken = token;
                SelectedContract = contract;
                Communication.SelectedToken = token;
                Communication.SelectedContract = contract;
                Communication.Receipt = "none";
                Communication.HideTokenList = "none";
                Communication.ShowPinPanel = "none";
                Communication.ShowLoader = "none";
                Communication.HideTokenSend = "";

                StateHasChanged();
            });
        }

        private void SetMaxAmount()
        {

            InvokeAsync(() =>
            {
                TokensToSend = SelectedContract.UserBalance;
            });
        }

        private void CancelSend()
        {
            InvokeAsync(() =>
            {
                Communication.HideTokenList = "";
                Communication.HideTokenSend = "none";
                Communication.ShowPinPanel = "none";
                Communication.ShowLoader = "none";
                Communication.Receipt = "none";


                StateHasChanged();
            });
        }

        private async void ToggleTokenImport()
        {


            await JS.InvokeVoidAsync("ImportTokens");
        }

        private async void OpenNetworkImport()
        {
            await JS.InvokeVoidAsync("ImportNewNetwork");
        }

        public async Task ImportNetwork()
        {


            await InvokeAsync(async () =>
            {
              
                Communication.NetworkSettings = await Utilities.SetupNetworks(HardwareService.Os);
                Networks = Communication.NetworkSettings.Where(x => x.IsProduction == Communication.IsDevelopment).ToList();
                StateHasChanged();

            });


        }

        public async Task ImportToken()
        {
            await InvokeAsync(async () =>
            {
                Tokens = await ContractService.GetNetworkTokensIntial(SelectedNetwork.Id); //Get All tokens and their balance
                StateHasChanged();
            });
        }


        private void SendToken()
        {
            InvokeAsync(() =>
            {
                Communication.Receipt = "none";
                Communication.HideTokenList = "none";
                Communication.HideTokenSend = "none";
                Communication.ShowLoader = "none";
                Communication.ShowPinPanel = "flex";
                StateHasChanged();

            });
        }



        private async void EnterPin()
        {
            if (Password.Length < 8)
            {
                Communication.PublishError("Requirements not met", "Password cannot be less then 8 characters");
                return;
            }
                

            Communication.TriggerLoader.Invoke("flex");
            await InvokeAsync(() =>
            {

                Communication.HideTokenList = "none";
                Communication.HideTokenSend = "none";
                Communication.ShowPinPanel = "none";
                Communication.Receipt = "none";

                StateHasChanged();

            });

            var passwrod = HardwareService.Encrypt("iV1z@$H8", Password);
            Communication.KeepPrivateSingle = true;
            Communication.Amount = TokensToSend;
            Communication.ReceiverAddress = ReceiverAddress;

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
            

            Communication.Pass = Password;
     
            InvokeAsync(() =>
            {
                Password = "";
                StateHasChanged();
            });

        }

       

        private void OnBalanceUpdate(object source, ElapsedEventArgs e)
        {

            if (DateTime.UtcNow > NextCheck)
            {
                Task.Run(() => DefaultToToken());
                NextCheck = DateTime.UtcNow.AddSeconds(20);
            }

        }


        private async void CloseReceipt()
        {
            await InvokeAsync(async () =>
            {
                Communication.ShowLoader = "none";
                Communication.HideTokenList = "";
                Communication.HideTokenSend = "none";
                Communication.ShowPinPanel = "none";
                Communication.Receipt = "none";
                SelectedContract.UserBalance -= TokensToSend;
                TokensToSend = 0;
                ReceiverAddress = "";
                Task.Run(() => DefaultToToken());
                StateHasChanged();

            });
        }

        private async void OpenLink(TokenLink link)
        {
            Utilities.OpenBrowser(link.Url);
        }


        private void OpenTracker()
        {
            Communication.TrackerOpenCounter += 1;
            NavigationManager.NavigateTo("Tracker", forceLoad:true);
        }
    }
}