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

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (!IsChartRendered)
            {
 
                Task.Run(() => JS.InvokeAsync<string>("InitDonut"));
                IsChartRendered = !IsChartRendered;
            }

            return base.OnAfterRenderAsync(firstRender);
        }

    }
}