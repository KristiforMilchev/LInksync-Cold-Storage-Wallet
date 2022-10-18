

using Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NBitcoin.Protocol.Behaviors;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace SYNCWallet.Components.Advertisement
{
    public partial class AdvertisementComponent
    {

        
        [Inject]
        private IJSRuntime JS { get; set; }
        
        public  List<IncomingAdRequest> ServedAds { get; set; }
        public bool IsRendered { get; set; }
        private IUtilities Utilities { get; set; }
        private ICommunication Communication { get; set; }
    
        protected override async Task OnInitializedAsync()
        {
            Utilities = ServiceHelper.GetService<IUtilities>();
            Communication = ServiceHelper.GetService<ICommunication>();
            Communication.AdEnabled = AdTriggered;
        }

        private void AdTriggered()
        {
            if (!IsRendered)
            {
                InvokeAsync(async () =>
                {
                    ServedAds = await Utilities.GetRequest<List<IncomingAdRequest>>("https://services.linksync.tech/Advertisments/new");
                    StateHasChanged();
                });
            
                JS.InvokeVoidAsync("OpenAdModule");
                IsRendered = true;
            }
 
        }
    }
}