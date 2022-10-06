using Domain.Models;
using Microsoft.AspNetCore.Components;
using NBitcoin;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace SYNCWallet.Components.Navigation
{
    public partial class TransactionsGridComponent
    {
        [Parameter]
        public string Contract { get; set; }
        private ICommunication Communication { get; set; }
        private SYNCWallet.Services.Definitions.ITransactionRepository Repository { get; set; }
        public List<TranscationRecordDTO> Transactions { get; set; }
        
        
        private async void LoadAllNetworkTokens()
        {
            Communication = ServiceHelper.GetService<ICommunication>();
            Repository = ServiceHelper.GetService<SYNCWallet.Services.Definitions.ITransactionRepository>();
            Repository.GetAllTransactionsForAsset(Contract);
        }
    }
}