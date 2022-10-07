using System.Drawing;
using LInksync_Cold_Storage_Wallet.Handlers;
using LInksync_Cold_Storage_Wallet.Services.Implementation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace SYNCWallet.Components.PriceChanges
{
    public partial class PriceChangeComponent
    {
        [Inject] private IJSRuntime JS { get; set; }

        private IContractService ContractService { get; set; }
        private ICommunication Communication { get; set; }
        public decimal DailyChange { get; set; }
        public string DailyBackground { get; set; }
        public decimal MonthlyChange { get; set; }
        public string MonthlyBackground { get; set; }
        public decimal YearlyChange { get; set; }
        public string YearlyBackground { get; set; }

        public (decimal lastDayPercentage, decimal lastMonthPercentage, decimal lastYearPercentage) _priceChanges
        {
            get;
            set;
        }

    protected override async Task OnInitializedAsync()
        {
            ContractService = ServiceHelper.GetService<IContractService>();
            Communication = ServiceHelper.GetService<ICommunication>();
            TrackerHandler.NotifyChildren += ParentUpdated;
            await LoadContractPriceChanges();
        }

        private async void ParentUpdated()
        {
            await LoadContractPriceChanges();
            InvokeAsync(() =>
            {
                this.StateHasChanged();
            });
        }

        private async Task LoadContractPriceChanges()
        {
            _priceChanges = await ContractService.GetPriceChange(Communication.SelectedContract.ContractAddress);
            DailyChange = _priceChanges.lastDayPercentage;
            MonthlyChange = _priceChanges.lastMonthPercentage;
            YearlyChange = _priceChanges.lastYearPercentage;
            
            if (DailyChange > 0)
                DailyBackground = "bg-success";
            else
            {
                DailyBackground = "sellBG";
                DailyChange = DailyChange * -1;
            }
            
            if (MonthlyChange > 0)
                MonthlyBackground = "bg-success";
            else
            {
                MonthlyBackground = "sellBG";
                MonthlyChange = MonthlyChange * -1;
            }
            
            if (YearlyChange > 0)
                YearlyBackground = "bg-success";
            else
            {
                YearlyBackground = "sellBG";
                YearlyChange = YearlyChange * -1;
            }
        }
        


    }
}