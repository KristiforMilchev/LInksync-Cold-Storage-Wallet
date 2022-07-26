using Nethereum.Contracts;
using Nethereum.Web3;
using NFTLock.Data;
using SYNCWallet.Models;
using SYNCWallet.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SYNCWallet.Data
{
    internal class PaymentService
    {
        private TransactionResult TransactionResult { get; set; }
        public async Task<TransactionResult> BeginTransaction()
        {

            MauiProgram.HideTokenList = "none";
            MauiProgram.HideTokenSend = "none";
            MauiProgram.ShowPinPanel = "none";
            MauiProgram.ShowLoader = "";

            var auth = new AuthenicationHandler();
            var wallet = auth.UnlockWallet(MauiProgram.Pass);
            var contractService = new ContractService();
            await contractService.ExecutePayments(MauiProgram.ReceiverAddress, MauiProgram.SelectedContract, MauiProgram.Amount, wallet, MauiProgram.ActiveNetwork.Endpoint, MauiProgram.ActiveNetwork.Chainid);

            MauiProgram.TransactionTimer = new System.Timers.Timer();
            MauiProgram.TransactionTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            MauiProgram.TransactionTimer.Interval = 5000;
            MauiProgram.TransactionTimer.Start();

            while(TransactionResult == null)
            {

            }

            return TransactionResult;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(MauiProgram.TxHash))
            {
                MauiProgram.TransactionTimer.Stop();
                MauiProgram.TransactionTimer.Dispose();
            }
            else
                await ValidateTransaction(MauiProgram.TxHash);
        }

        public async Task<bool> ValidateTransaction(string txHash)
        {

            var auth = new AuthenicationHandler();
            var Account = auth.UnlockWallet(MauiProgram.Pass);

            var web3 = new Web3(Account, MauiProgram.ActiveNetwork.Endpoint);
            var transactionReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
            if (transactionReceipt == null) //Check if transaction is processed in case it fails sent it to quoue;
            {

                return false;
            }

            var transferEventOutput = transactionReceipt.DecodeAllEvents<TransferEventDTO>();
            if (transferEventOutput.Count == 0)
                return false;
            var transferEvent = transferEventOutput.FirstOrDefault().Event;


            var actualTransfer = Utilities.ConvertToDex((decimal)transferEvent.Value, MauiProgram.SelectedContract.Decimals);
            if (transferEvent.From.ToUpper() != MauiProgram.PublicAddress.ToUpper() || actualTransfer < 0)
                return false;

            MauiProgram.TxHash = string.Empty;

            TransactionResult = new TransactionResult
            {
                Amount = actualTransfer,
                From = transferEvent.From,
                To = transferEvent.To,
                Timestamp = DateTime.UtcNow,
                TransactionHash = transactionReceipt.TransactionHash
            };
            return true;
        }
    }
}
