using System.Runtime.CompilerServices;
using Domain.Models;
using LInksync_Cold_Storage_Wallet.Handlers;
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
        public string TwitterVisible { get; set; } = "block";
        public string InstagramVisible { get; set; } = "none";
        public string YoutubeVisible { get; set; } = "none";
        public decimal SupplyDifference { get; set; }
        public decimal TokenPoolSupply { get; set; }
        public decimal PairPoolSupply { get; set; }
        public bool TriggerAd { get; set; } = false;
        public bool TwitterFeedEmpty { get; set; } = true;
        public bool InstagramFeedEmpty { get; set; } = true;
        public bool YoutubeFeedEmpty { get; set; } = true;
        public string EmptySocialError { get; set; }
        public decimal TotalBinding { get; set; }
        
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!IsChartRendered)
            {
                LoadSocialData();
                IsChartRendered = !IsChartRendered;
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        private void LoadSocialData()
        {
            var anyExist = false;
            if (!string.IsNullOrEmpty(Communication.SelectedToken.TwitterFeed))
            {
                Task.Run(() => JS.InvokeVoidAsync("LoadSocialMediaFeed", "TwitterFeed", 
                    Communication.SelectedToken.TwitterFeed));
                TwitterFeedEmpty = false;
                anyExist = true;
            }

            if (!string.IsNullOrEmpty(Communication.SelectedToken.InstagramFeed))
            {
                Task.Run(() => JS.InvokeVoidAsync("LoadSocialMediaFeed", "InstagramFeed",
                    Communication.SelectedToken.InstagramFeed));
                InstagramFeedEmpty = false;
                anyExist = true;
            }

            if (!string.IsNullOrEmpty(Communication.SelectedToken.YoutubeFeed))
            {
                Task.Run(() => JS.InvokeVoidAsync("LoadSocialMediaFeed", "YoutubeFeed",
                    Communication.SelectedToken.YoutubeFeed));
                YoutubeFeedEmpty = false;
                anyExist = true;
            }

            InvokeAsync(() =>
            {
                if (anyExist)
                    EmptySocialError = "There is no data provided.";
                else
                    EmptySocialError = " Feed is only enabled for whitelisted assets.";
                
                StateHasChanged();
            });

        }

        protected override async Task OnInitializedAsync()
        {
            Communication = ServiceHelper.GetService<ICommunication>();
            Utilities = ServiceHelper.GetService<IUtilities>();
            ContractService = ServiceHelper.GetService<IContractService>();
            await CalculateCirculatingSupply();
            await GetPoolQuantities();
            GetAssetBalance();
        }

        private void GetAssetBalance()
        {
            var data = ContractService.GetContractPriceData(Communication.SelectedContract.ContractAddress);
            Communication.ChartDataLoaded?.Invoke(data, Communication.SelectedContract.ContractAddress, false);
        }


        private async Task CalculateCirculatingSupply()
        {
            var difference = Communication.SelectedContract.Supply - Communication.SelectedContract.CirculatingSupply;
            SupplyDifference = 0;
            if (difference != 0)
                SupplyDifference = (difference / Communication.SelectedContract.Supply) * 100;
        }

        private async Task GetPoolQuantities()
        {
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
                        InstagramVisible = "none";
                        TwitterVisible = "block";
                        YoutubeVisible = "none";
                        this.StateHasChanged();
                    });
                break;
                case 2:
                    InvokeAsync(() =>
                    {
                        InstagramVisible = "block";
                        TwitterVisible = "none";
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

        private async void AssetUpdatedHandler()
        {
            NavigationManager.NavigateTo("Tracker", forceLoad:true);
        }

        private void TotalCalculated(decimal total)
        {
            InvokeAsync(() =>
            {
                TotalBinding = total;
                StateHasChanged();
            });
        }
    }
}