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
 
        
        private ICommunication Communication { get; set; }
        private SYNCWallet.Services.Definitions.ITransactionRepository Repository { get; set; }
        private IUtilities Utilities { get; set; }
        public List<TranscationRecordDTO> Transactions { get; set; }
        protected override async Task OnInitializedAsync()
        {
            Communication = ServiceHelper.GetService<ICommunication>();
            Repository = ServiceHelper.GetService<SYNCWallet.Services.Definitions.ITransactionRepository>();;
            Utilities = ServiceHelper.GetService<IUtilities>();
            GetAllTransactions();
            TrackerHandler.NotifyChildren += ParentUpdated;
        }

        private void ParentUpdated()
        {
            InvokeAsync(() =>
            {
                GetAllTransactions();
                this.StateHasChanged();
            });
        }

        private void GetAllTransactions()
        {
            Transactions = Repository.GetAllTransactionsForAsset(Contract);
        }
    }
}