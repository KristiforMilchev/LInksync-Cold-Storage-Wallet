using Domain.Models;
using LInksync_Cold_Storage_Wallet;
using LInksync_Cold_Storage_Wallet.Services.Implementation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NFTLock.Data;
using SYNCWallet.Models;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace SYNCWallet.Components.Navigation
{
    public partial class TrackerNavigationComponent
    {
                
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        
        [Parameter]
        public Token CurrentToken { get; set; }
        [Parameter]
        public EventCallback AssetChanged { get; set; }

        private ICommunication _Communication { get; set; }
        private IContractService _contractService { get; set; }
        private IUtilities Utilities { get; set; }
        public bool IsChartRendered { get; set; }


        public List<NetworkTokenGroup> ListedTokens { get; set; }
        public object ItemChanged { get; set; }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!IsChartRendered)
            {
                Task.Run(() => JS.InvokeAsync<string>("InitDropdown"));
                IsChartRendered = !IsChartRendered;
            }

            return base.OnAfterRenderAsync(firstRender);
        }



        protected override async Task OnInitializedAsync()
        {
            ListedTokens = new List<NetworkTokenGroup>();
            _contractService = ServiceHelper.GetService<IContractService>();
            _Communication = ServiceHelper.GetService<ICommunication>();
            Utilities = ServiceHelper.GetService<IUtilities>();
            Initializer.DateTimePicker += RegisterPickerCallback;
            LoadAllNetworkTokens();
          
        }

        private async void RegisterPickerCallback(string token)
        {
            
            //TODO add an abstraction to the delegate to pass a new network and bind that to the communication service in order to make the search more robust
            var networkGroup = ListedTokens.FirstOrDefault(x => x.Network == _Communication.ActiveNetwork);
            if(networkGroup == null)
                return;
            
            var changedToken = networkGroup.Tokens.FirstOrDefault(x => x.Name == token); 
            if(changedToken == null)
                return;

            var contractDetails = changedToken.Contracts.FirstOrDefault(x => x.Network == _Communication.ActiveNetwork.Id);
            if(contractDetails == null)
                return;

            InvokeAsync(async () =>
            {
                _Communication.SelectedToken = changedToken;
                _Communication.SelectedContract = contractDetails;
                await AssetChanged.InvokeAsync();
            });

            //   NavigationManager.NavigateTo("Tracker");
        }

        private async void LoadAllNetworkTokens()
        {
            var tokens = await _contractService.GetNetworkTokensIntial(_Communication.ActiveNetwork.Id);
            ListedTokens.Add(new NetworkTokenGroup
            {
                Network = _Communication.ActiveNetwork,
                Tokens = tokens
            });
        }

        private void ReturnToLanding()
        {
            NavigationManager.NavigateTo("Landing");
        }
    }
}