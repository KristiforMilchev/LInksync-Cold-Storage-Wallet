using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
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
         

            var auth = new AuthenicationHandler();
            var wallet = auth.UnlockWallet(MauiProgram.Pass);
            var contractService = new ContractService();

            if(wallet == null)
                return null; 

            if (string.IsNullOrEmpty(MauiProgram.SelectedContract.ContractAddress))
                await contractService.ExecuteNative(MauiProgram.ReceiverAddress, MauiProgram.Amount, wallet, MauiProgram.ActiveNetwork.Endpoint, MauiProgram.ActiveNetwork.Chainid);
            else
                await contractService.ExecutePayments(MauiProgram.ReceiverAddress, MauiProgram.SelectedContract, MauiProgram.Amount, wallet, MauiProgram.ActiveNetwork.Endpoint, MauiProgram.ActiveNetwork.Chainid);

            //Clear PK, password etc.

          

            var dateTime = DateTime.UtcNow.AddSeconds(30);

            while(TransactionResult == null)
            {
                if(DateTime.UtcNow > dateTime)
                {
                    await ValidateTransaction(MauiProgram.TxHash);
                    dateTime = DateTime.UtcNow.AddSeconds(30);
                }
                
            }
            MauiProgram.ClearCredentials();
            return TransactionResult;
        }

    
        public async Task<bool> ValidateTransaction(string txHash)
        {

            var auth = new AuthenicationHandler();
            var account =  auth.UnlockWallet(MauiProgram.Pass);

            if (string.IsNullOrEmpty(txHash))
                return false;

            if(txHash == "-")
            {
                TransactionResult = new TransactionResult
                {
                    Amount = 0,
                    From = "--",
                    To = "--",
                    Timestamp = DateTime.UtcNow,
                    TransactionHash = "Transaction failed, internal error, inssuficient balance or gas!"
                };
                MauiProgram.TxHash = String.Empty;
                return false;
            }

            var web3 = new Web3(account, MauiProgram.ActiveNetwork.Endpoint);
            var transactionReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
            if (transactionReceipt == null) //Check if transaction is processed in case it fails sent it to quoue;
            {

                return false;
            }
           
            var transferEventOutput = transactionReceipt.DecodeAllEvents<TransferEventDTO>();
            if (transferEventOutput.Count == 0)
            {
                if (transactionReceipt.Succeeded())
                {
                    TransactionResult = new TransactionResult
                    {
                        Amount = 0,
                        From = transactionReceipt.From,
                        To = transactionReceipt.To,
                        Timestamp = DateTime.UtcNow,
                        TransactionHash = transactionReceipt.TransactionHash
                    };
                    MauiProgram.TxHash = String.Empty;
                    return true;
                }
                else if (transactionReceipt.Failed())
                {
                    
                    MauiProgram.TxHash = String.Empty;
                    return false;
                }
            }
                

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
            MauiProgram.TxHash = String.Empty;

            return true;
        }
    }
}
