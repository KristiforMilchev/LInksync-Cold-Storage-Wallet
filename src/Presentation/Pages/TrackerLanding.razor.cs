using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SYNCWallet.Models;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace LInksync_Cold_Storage_Wallet.Pages
{
    public partial class TrackerLanding
    {  
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        private ICommunication Communication { get; set; }
        private IUtilities Utilities { get; set; }
        private IContractService ContractService { get; set; }
        public bool IsChartRendered { get; set; }
        private string TokenId { get; set; }
        public string TwitterVisible { get; set; } = "none";
        public string InstagramVisible { get; set; } = "block";
        public string YoutubeVisible { get; set; } = "none";
        public decimal SupplyDifference { get; set; }
        public decimal TokenPoolSupply { get; set; }
        public decimal PairPoolSupply { get; set; }
        
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!IsChartRendered)
            {
 
                Task.Run(() => JS.InvokeAsync<string>("InitDonut"));
                IsChartRendered = !IsChartRendered;
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            Communication = ServiceHelper.GetService<ICommunication>();
            Utilities = ServiceHelper.GetService<IUtilities>();
            ContractService = ServiceHelper.GetService<IContractService>();
            var difference = Communication.SelectedContract.Supply -  Communication.SelectedContract.CirculatingSupply;
            SupplyDifference = 0;
            if(difference != 0)
                SupplyDifference =  (difference / Communication.SelectedContract.Supply) * 100;
             
            var poolSupplyData = await ContractService.GetContractLpSupply(Communication.SelectedContract);
            TokenPoolSupply = poolSupplyData.Item1;
            PairPoolSupply = poolSupplyData.Item2;
        }
        
        public void SocialFeedSelected(int networkType)
        {
            switch (networkType)
            {
                case 1:
                    InvokeAsync(() =>
                    {
                        InstagramVisible = "block";
                        TwitterVisible = "none";
                        YoutubeVisible = "none";
                        this.StateHasChanged();
                    });
                break;
                case 2:
                    InvokeAsync(() =>
                    {
                        InstagramVisible = "none";
                        TwitterVisible = "block";
                        YoutubeVisible = "none";
                        this.StateHasChanged();
                    });
                    break;
                case 3:
                    InvokeAsync(() =>
                    {
                        InstagramVisible = "none";
                        TwitterVisible = "none";
                        YoutubeVisible = "block";
                        this.StateHasChanged();
                    });
                    break;

            }
        }
    }
}