using System.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using NBitcoin.Protocol.Behaviors;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;

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
         IUtilities Utilities { get; set; }
        IAuthenicationService AuthenicationService { get; set; }
        IPaymentService PaymentService { get; set; }
        IHardwareService HardwareService { get; set; }
        ICommunication Communication { get; set; }
        IContractService ContractService { get; set; }
        private IBlockProcessor BlockProcessor { get; set; }
        public string Chart { get; set; }
        public string TokenListPanel { get; set; }
        public string PortfolioBaseInfo { get; set; } = string.Empty;
        public string TokenBaseInfo { get; set; } = "none";
        public decimal PortfolioDailyChange { get; set; } = 0;
        public decimal PortfolioCurrentTotal { get; set; }
    
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
            BlockProcessor = ServiceHelper.GetService<IBlockProcessor>();
            
            //Register callbcaks
            Communication.LoginAttempt = Callback;
            Communication.IncomingBlockCallback = BlockCallback;
            
            //TODO Remove dependency on time, default to data on new blocks
             
            //Hide stuff that has to be hidden 
            //TODO Move out to a handler to make code more readable
            Communication.HideTokenList = "";
            Communication.HideTokenSend = "none";
            Communication.ShowPinPanel = "none";
            Communication.ShowLoader = "none";
            Communication.Receipt = "none";
            TokenListPanel = "";
            Chart = "none";
            TokenName = "SYNC";

            //Load Network Settings
            if(Communication.NetworkSettings == null)
                Communication.NetworkSettings = await Utilities.SetupNetworks(HardwareService.Os);
            
            Networks = Communication.NetworkSettings.Where(x=> x.IsProduction != Communication.IsDevelopment).ToList();
            
            if (Communication.ActiveNetwork == null)
                SelectedNetwork = Networks.FirstOrDefault();
            else
                SelectedNetwork = Communication.ActiveNetwork;
            
            Communication.ActiveNetwork = SelectedNetwork;
            WalletAddress = Communication.GetDefault();
            Tokens = await ContractService.GetNetworkTokensIntial(SelectedNetwork.Id); //Get All tokens and their balance
            Task.Run(() => DefaultToToken());
            Task.Run(() => GetAssetBalance());
            
            BlockProcessor.BeginProcessing();
         }

        private void BlockCallback()
        {
            Task.Run(() => DefaultToToken());
        }

        private void GetAssetBalance()
        {
            var data = ContractService.GetPortfolioBalance();
            data = data.OrderBy(x => x.Date).ToList();
            var lastBalance = data.LastOrDefault();
            
            try
            {
                if (lastBalance != null)
                {
                    InvokeAsync(() =>
                    {
                        PortfolioCurrentTotal = lastBalance.Balance;
                        var prev = data.IndexOf(lastBalance);
                        if (prev - 1 >= 0)
                        {
                            var prevDay = data.ElementAt(prev - 1);
                            if (prevDay != null && prevDay.Balance != lastBalance.Balance)
                                PortfolioDailyChange =
                                    ((decimal)lastBalance.Balance / prevDay.Balance) * 100;
                            else
                                PortfolioDailyChange = 0;
                        }
                        else
                            PortfolioDailyChange = 0;
                    
                        StateHasChanged();
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            // Bind to chart
            Communication.ChartDataLoaded?.Invoke(data, string.Empty, true);
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
                        TokenListPanel = "";
                        Chart = "none";
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
                    TokenListPanel = "";
                    Chart = "none";
                    StateHasChanged();

                });

        }

        private async void NetworkChanged(NetworkSettings network)
        {
            
            SelectedNetwork = network;
            Communication.ActiveNetwork = SelectedNetwork;
            BlockProcessor.Dispose();
            NavigationManager.NavigateTo("Landing", forceLoad:true);

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

                        
                         StateHasChanged();
                    }

                });
            }
            catch (Exception)
            {
                Console.WriteLine("Exception updating tokens.");
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
                TokenListPanel = "";
                Chart = "none";
                PortfolioBaseInfo = "none";
                TokenBaseInfo = "";
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
                TokenListPanel = "";
                Chart = "none";
                PortfolioBaseInfo = "";
                TokenBaseInfo = "none";

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
                Networks = Communication.NetworkSettings.Where(x => x.IsProduction != Communication.IsDevelopment).ToList();
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
            if (string.IsNullOrEmpty(ReceiverAddress))
            {
                Communication.PublishError("Validation failed.", "Receiver address cannot be empty!");
                return;
            }
            
            if (TokensToSend <= 0)
            {
                Communication.PublishError("Validation failed.", "Tokens have to be greater then zero!");
                return;
            }
            
            InvokeAsync(() =>
            {
                Communication.Receipt = "none";
                Communication.HideTokenList = "none";
                Communication.HideTokenSend = "none";
                Communication.ShowLoader = "none";
                Communication.ShowPinPanel = "flex";
                TokenListPanel = "";
                Chart = "none";
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
                TokenListPanel = "";
                Chart = "none";

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

       

     


        private async void CloseReceipt()
        {
            await InvokeAsync(async () =>
            {
                Communication.ShowLoader = "none";
                Communication.HideTokenList = "";
                Communication.HideTokenSend = "none";
                Communication.ShowPinPanel = "none";
                Communication.Receipt = "none";
                TokenListPanel = "";
                Chart = "none";
                SelectedContract.UserBalance -= TokensToSend;
                TokensToSend = 0;
                ReceiverAddress = "";
                Communication.TxHash = "";
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
             NavigationManager.NavigateTo("Tracker", forceLoad:true);
        }

        private void KeyUpPressed(KeyboardEventArgs obj)
        {
            if(obj.Key == "Enter")
                EnterPin();
        }

        private void OpenChart()
        {
            InvokeAsync(() =>
            {
 
                Chart = "";
                TokenListPanel = "none";

                StateHasChanged();
            });
        }

        private void HideChart()
        {
            InvokeAsync(() =>
            {
  
                Chart = "none";
                TokenListPanel = "";

                StateHasChanged();
            });
        }
    }
}