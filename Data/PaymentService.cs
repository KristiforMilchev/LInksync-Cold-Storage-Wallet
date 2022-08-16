﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NFTLock.Data;
using SYNCWallet.Models;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;
using SYNCWallet.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SYNCWallet.Data
{
    internal class PaymentService : IPaymentService
    {
        private TransactionResult TransactionResult { get; set; }
        IContractService ContractService { get; set; }
        IUtilities Utilities { get; set; }
        IAuthenicationService AuthenicationService { get; set; }
        ICommunication Communication { get; set; }

        //Init the constructor and inherit all dependencies.
        public PaymentService()
        {
            ContractService = ServiceHelper.GetService<IContractService>();
            Utilities = ServiceHelper.GetService<IUtilities>();
            AuthenicationService = ServiceHelper.GetService<IAuthenicationService>();
            Communication = ServiceHelper.GetService<ICommunication>();
        }

        public async Task<TransactionResult> BeginTransaction()
        {
            var wallet = AuthenicationService.UnlockWallet(Communication.Pass); //One of decrypt the PK encoded on the device and open the wallet.
           
            //If wallet doesn't exist return null
            if(wallet == null)
                return null; 

            //Check if a contract exists (Native currencies don't have a contract) if false, send native chain token.
            if (string.IsNullOrEmpty(Communication.SelectedContract.ContractAddress))
                await ContractService.ExecuteNative(Communication.ReceiverAddress, Communication.Amount, wallet, Communication.ActiveNetwork.Endpoint, Communication.ActiveNetwork.Chainid);
            else
                await ContractService.ExecutePayments(Communication.ReceiverAddress, Communication.SelectedContract, Communication.Amount, wallet, Communication.ActiveNetwork.Endpoint, Communication.ActiveNetwork.Chainid);

            //Defer next check, it will be validated after the next block regardless.
            var dateTime = DateTime.UtcNow.AddSeconds(30); 

            //Loop and wait till the transaction was valid, important no recursion here due to that the transaction hash has already been created we are only checking the status.
            while(TransactionResult == null)
            {
                //If time is over the check time validate if the transaction is validated
                if(DateTime.UtcNow > dateTime)
                {
                    await ValidateTransaction(Communication.TxHash);
                    dateTime = DateTime.UtcNow.AddSeconds(30);
                }
                
            }
            Communication.ClearCredentials();
            return TransactionResult;
        }

    
        public async Task<bool> ValidateTransaction(string txHash)
        {
            //We don't need a real wallet here, Pass is empty however due to how Nethereum operates we need to make the request from the object, so we can inherit the chain
            //That's why we just instance a object to interact with the chain.
            var account = AuthenicationService.UnlockWallet(Communication.Pass);

            //If Hash is empty return false. Return an empty object to avoid recursion in the loop.
            if (string.IsNullOrEmpty(txHash))
            {
                TransactionResult = new TransactionResult
                {
                    TransactionHash = "-1"
                };
                Communication.TxHash = string.Empty;
                return false;
            }    
            
            //If the transaction failed at creation, and there is no hash we return an error object.
            if(txHash == "-")
            {
                TransactionResult = new TransactionResult
                {
                    Amount = 0,
                    From = "--",
                    To = "--",
                    Timestamp = DateTime.UtcNow,
                    TransactionHash = "Transaction failed, internal error, insuficient balance or gas!"
                };
                Communication.TxHash = string.Empty;
                return false;
            }

            //Create a new web3 instance, using the account object, for the chain data and the current selected network. 
            //We send a request to the blockchain to check the receipt of the transaction.
            var web3 = new Web3(account, Communication.ActiveNetwork.Endpoint);
            var transactionReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
            if (transactionReceipt == null) //Check if transaction is processed in case it fails sent it to quoue;
            {

                return false;
            }
           
            //Decode events in case it's a proxy contract, ETC ETH smart contract
            //If transferEventOutput is 0, it's a native token transfer.
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
                    Communication.TxHash = string.Empty;
                    return true;
                }
                else if (transactionReceipt.Failed())
                {
                    TransactionResult = new TransactionResult
                    {
                        Amount = 0,
                        From = "--",
                        To = "--",
                        Timestamp = DateTime.UtcNow,
                        TransactionHash = "Transaction failed, internal error, insuficient balance or gas!"
                    };
                    Communication.TxHash = string.Empty;
                    return false;
                }
            }
                
            //Get the initial main trainsfer event.
            var transferEvent = transferEventOutput.FirstOrDefault().Event;

            //We get the ETH contract actual tokens by substracting the decimals that are obsolete
            var actualTransfer = Utilities.ConvertToDex((decimal)transferEvent.Value, Communication.SelectedContract.Decimals);
            if (transferEvent.From.ToUpper() != Communication.PublicAddress.ToUpper() || actualTransfer < 0)
                return false;

 
            TransactionResult = new TransactionResult
            {
                Amount = actualTransfer,
                From = transferEvent.From,
                To = transferEvent.To,
                Timestamp = DateTime.UtcNow,
                TransactionHash = transactionReceipt.TransactionHash
            };
            Communication.TxHash = string.Empty;

            return true;
        }
    }
}
