using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LInksync_Cold_Storage_Wallet.Pages
{
    public partial class TrackerLanding
    {  
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        public bool IsChartRendered { get; set; }
        private  string TokenId { get; set; }
        public string TwitterVisible { get; set; } = "none";
        public  string InstagramVisible { get; set; } = "block";
        public  string YoutubeVisible { get; set; } = "none";
        
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!IsChartRendered)
            {
 
                Task.Run(() => JS.InvokeAsync<string>("InitDonut"));
                IsChartRendered = !IsChartRendered;
            }

            return base.OnAfterRenderAsync(firstRender);
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