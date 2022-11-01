using System.Security.Cryptography.Xml;
using Application.Cache;
using Domain.Models;
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
        private ICommunication Communication { get; set; }
        private ICacheRepository<UserAssetBalance> BalanceRepository { get; set; }

        public bool IsChartRendered { get; set; }
        
        [Parameter]
        public string ContractAddress { get; set; }
        public  bool IsEmpty { get; set; } = true;


        protected override async Task OnInitializedAsync()
        {
            ContractService = ServiceHelper.GetService<IContractService>();
            Communication = ServiceHelper.GetService<ICommunication>();
            BalanceRepository = ServiceHelper.GetService<ICacheRepository<UserAssetBalance>>();
            Communication.ChartDataLoaded = ChartLoadUserBalance;

        }

        private async void ChartLoadUserBalance(List<UserAssetBalance> data, string contractAddress, bool isGlobal)
        {
            try
            {
                if (data != null && !isGlobal)
                {
                    BindUserBalance(data, contractAddress);
                }
                else if (data != null && isGlobal)
                {
                    BindUserBalance(data, "PortfolioBalance");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void BindUserBalance(List<UserAssetBalance> data, string contract)
        {
            var bindingData = new List<UserAssetBalance>();
            //Fill in the gaps from missing balance updates due to the app not running/

            var iterationIndex = 0;
            data.ForEach(x =>
            {
                if (x.Balance > 0)
                {
                    var currentActiveDate = x.Date;
                    var nextEntry = data.ElementAt(iterationIndex);
                    while (currentActiveDate <= nextEntry.Date && currentActiveDate <= DateTime.UtcNow)
                    {
                        currentActiveDate = currentActiveDate.AddMinutes(5);
                        if (currentActiveDate <= DateTime.Now)
                        {
                            var newEntity = new UserAssetBalance
                            {
                                Balance = x.Balance,
                                PevBalance = x.Balance,
                                Currency = contract,
                                Date = currentActiveDate,
                                WalletAddress = Communication.PublicAddress
                            };
                            bindingData.Add(newEntity);
                            if (!string.IsNullOrEmpty(contract))
                            {
                                BalanceRepository.SelectDatabase("UserBalanceHistory");
                                BalanceRepository.CreateEntity(newEntity);
                            }
                        }
                        else
                        {
                            bindingData.Add(x);
                        }
                    
                    }
                }
                iterationIndex++;
            });
            
            InvokeAsync(() =>
            {

                if (bindingData.Count > 0 && bindingData.Any(x => x.Balance > 0))
                {
                    IsEmpty = false;
                    StateHasChanged();
                }
                else
                {
                    IsEmpty = true;
                    StateHasChanged();
                }
            });

            if (bindingData.Count > 0 && bindingData.Any(x => x.Balance > 0))
            {
                JS.InvokeVoidAsync("InitBalanceChart", bindingData);
            }
        }
    }
}