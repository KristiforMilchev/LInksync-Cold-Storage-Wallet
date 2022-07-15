 using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NFTLock.Models;
using SYNCWallet;
using SYNCWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
 
namespace NFTLock.Data
{
    internal class ContractService
    {

        private readonly Web3 web3;
        private readonly Contract contract;
        private readonly Account account;
        string Contract { get; set; }
        private static readonly HexBigInteger GAS = new HexBigInteger(4600000);

        public ContractService(string provider, string contractAddress, string abi, string privateKey)
        {
            Contract = contractAddress;
            this.account = new Account(privateKey,97);
            this.web3 = new Web3(account, provider);
            this.contract = web3.Eth.GetContract(abi, contractAddress);
        }

        [Function("balanceOf", "uint256")]
        public class BalanceOfFunction : FunctionMessage
        {
            [Parameter("address", "owner", 1)]
            public string Owner { get; set; }

        }

        [Function("mint", "string")]
        public class TransferFunction : FunctionMessage
        {

            
            [Parameter("uint256", "_mintAmount", 2)]
            public BigInteger _mintAmount { get; set; }
        }

        public async Task<int> CheckUserBalanceForContract(string ownerAddress)
        {
 
            var balanceOfFunctionMessage = new BalanceOf()
            {
                Owner = ownerAddress,
            };

            var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOf>();
            var balance = await balanceHandler.QueryAsync<BigInteger>(Contract, balanceOfFunctionMessage);
            return int.Parse(balance.ToString());
        }

        public async Task<string> MintNftKey()
        {
            var addFactFunction = contract.GetFunction("mint");
            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

            var transfer = new TransferFunction()
            {
                
                AmountToSend = new BigInteger( 0.1  * Math.Pow(10,18)),
                _mintAmount = new HexBigInteger(1)
            };
            transfer.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(15, UnitConversion.EthUnit.Gwei);

            try
            {
                transfer.Gas = GAS;
            }
            catch (Exception e) 
            {
                Console.WriteLine(e);
            }

      
            var transactionReceipt = default(string);

            var logged = false;
            try
            {
                transactionReceipt =   transferHandler.SendRequestAsync(Contract, transfer).GetAwaiter().GetResult();
            }
            catch(Exception e)
            {
                logged = true;
            }



            return transactionReceipt;
        }

        public static async Task<List<Token>> GetNetworkTokens(int networkId)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("http://www.contoso.com/");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return new List<Token>();
        }
    }
}
