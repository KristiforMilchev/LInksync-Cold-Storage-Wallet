using LInksync_Cold_Storage_Wallet.Services.Implementation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace SYNCWallet.Components.PriceChart
{
    public partial class PriceChartComponent
    {
        [Inject]
        private IJSRuntime JS { get; set; }
        private IContractService ContractService { get; set; }
        public bool IsChartRendered { get; set; }
        
        [Parameter]
        public string ContractAddress { get; set; }

        protected override async Task<Task> OnAfterRenderAsync(bool firstRender)
        {
            if (!IsChartRendered)
            {
                var data = await ContractService.GetContractPriceData(ContractAddress, "", DateTime.UtcNow.AddMonths(-3), DateTime.Now);
                Task.Run(() => JS.InvokeAsync<string>("InitBalanceChart",data));
 
                IsChartRendered = !IsChartRendered;
            }

            return base.OnAfterRenderAsync(firstRender);
        }
        
        protected override async Task OnInitializedAsync()
        {
            ContractService = ServiceHelper.GetService<IContractService>();
            
        }

    }
}