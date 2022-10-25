using Domain.Models;
using LInksync_Cold_Storage_Wallet.Handlers;
using Microsoft.AspNetCore.Components;
using NBitcoin;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace SYNCWallet.Components.Transactions
{
    public partial class TransactionsGridComponent
    {
        [Parameter]
        public string Contract { get; set; }
        [Parameter]
        public EventCallback<decimal> TotalSpendingCalculated { get; set; }

        
        private ICommunication Communication { get; set; }
        private SYNCWallet.Services.Definitions.ITransactionRepository Repository { get; set; }
        private IUtilities Utilities { get; set; }
        public List<TranscationRecordDTO> Transactions { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Communication = ServiceHelper.GetService<ICommunication>();
                Repository = ServiceHelper.GetService<SYNCWallet.Services.Definitions.ITransactionRepository>();;
                Utilities = ServiceHelper.GetService<IUtilities>();
                Task.Run(() => GetAllTransactions());
                TrackerHandler.NotifyChildren += ParentUpdated;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }

        }

        private void ParentUpdated()
        {
            InvokeAsync(() =>
            {
                GetAllTransactions();
                this.StateHasChanged();
            });
        }

        private async void GetAllTransactions()
        {
            Transactions = Repository.GetAllTransactionsForAsset(Contract);
            if (Transactions != null && Transactions.Count > 0)
            {
                
                await TotalSpendingCalculated.InvokeAsync(Transactions.Sum(x=>x.Value));
            }
        }
    }
}