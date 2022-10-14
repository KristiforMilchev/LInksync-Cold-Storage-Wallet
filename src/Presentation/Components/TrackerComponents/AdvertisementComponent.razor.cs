

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
        [Parameter]
        public bool Triggered { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        
        public  List<IncomingAdRequest> ServedAds { get; set; }
        public bool IsRendered { get; set; }
        private IUtilities Utilities { get; set; }
        
        protected override async Task<Task> OnAfterRenderAsync(bool firstRender)
        {
            if (!IsRendered)
            {
                
                if (Triggered)
                {
                    InvokeAsync(async () =>
                    {              
                        ServedAds = await Utilities.GetRequest<List<IncomingAdRequest>>("https://localhost:5001/Advertisments/new");
                        Task.Run(() => JS.InvokeVoidAsync("OpenAdModule"));
                        StateHasChanged();
                    });
                }
                
                IsRendered = !IsRendered;
            }

            return base.OnAfterRenderAsync(firstRender);
        }
        
        protected override async Task OnInitializedAsync()
        {
            Utilities = ServiceHelper.GetService<IUtilities>();
        }
        

    }
}