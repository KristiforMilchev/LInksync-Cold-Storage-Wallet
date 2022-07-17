﻿ using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using NFTLock.Models;
using SYNCWallet;
using SYNCWallet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static SYNCWallet.Models.GithubTokensModel;

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

        public static async Task<decimal> GetAccountBalance(int network)
        {
            try
            {
                var publicKey = MauiProgram.PublicAddress;
                var web3 = new Nethereum.Web3.Web3("https://data-seed-prebsc-1-s1.binance.org:8545/");
                var balance = await web3.Eth.GetBalance.SendRequestAsync(publicKey);
                var etherAmount = Web3.Convert.FromWei(balance.Value);

                Console.WriteLine(web3);
                Console.WriteLine("Get txCount " + etherAmount);
                Console.ReadLine();
                return etherAmount;
            }
            catch (Exception e )
            {
                Debug.WriteLine(e);
                return 0;
            }
        }

        public static async Task<decimal> CheckUserBalanceForContract(string ownerAddress, string contract)
        {
 
            var balanceOfFunctionMessage = new BalanceOf()
            {
                Owner = ownerAddress,
            };
            var web3 = new Nethereum.Web3.Web3("https://data-seed-prebsc-1-s1.binance.org:8545/");

            var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOf>();
            var balance = await balanceHandler.QueryAsync<BigInteger>(contract, balanceOfFunctionMessage);
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



            var listedTokenData = await GetRequest<List<ListedToken>>($"https://api.github.com/repos/KristiforMilchev/LInksync-Cold-Storage-Wallet/contents/Models/Tokens");

            var tokens = new List<Token>();



            switch (networkId)
            {
                case 97:
                    tokens.Add(new Token
                    {
                        Symbol = "BNB",
                        Name = "BNB",
                        Logo = "/images/tokenLogos/bsc.png",
                        IsChainCoin = true
                    });
                    break;
                case 56:
                    tokens.Add(new Token
                    {
                        Symbol = "BNB",
                        Name = "BNB",
                        Logo = "/images/tokenLogos/bsc.png",
                        IsChainCoin = true
                    });
                    break;
                case 1:
                    tokens.Add(new Token
                    {
                        Symbol = "ETH",
                        Name = "ETH",
                        Logo = "/images/tokenLogos/eth.jpg",
                        IsChainCoin = true,
                        Contracts = new List<TokenContract>
                        {
                            new TokenContract
                            {
                                ContractAddress = "",
                                UserBalance = await GetAccountBalance(networkId)
                            }
                        }
                    }); ;
                    break;
                case 4:
                    tokens.Add(new Token
                    {
                        Symbol = "ETH",
                        Name = "ETH",
                        Logo = "/images/tokenLogos/eth.jpg",
                        IsChainCoin = true
                    });
                    break;
                default:
                    break;
            }
 
            tokens = await GetListedTokens(listedTokenData, tokens);

            return tokens;
        }

        private static async Task<List<Token>> GetListedTokens(List<ListedToken> listedTokenData, List<Token> tokens)
        {
            foreach(var token in listedTokenData)
            {
                var currentToken = await GetRequest<Token>($"https://raw.githubusercontent.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/main/Models/Tokens/{token.name}/token.json");
                var contracts = new List<TokenContract>();

                foreach (var getContract in currentToken.Contracts)
                {
                    getContract.UserBalance = await CheckUserBalanceForContract(MauiProgram.PublicAddress, getContract.ContractAddress);
                    contracts.Add(getContract);
                }
                currentToken.Contracts = contracts;

                tokens.Add(currentToken);
            }

            return tokens;
        }

        private static async Task<T> GetRequest<T>(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "request");
            HttpResponseMessage response = await client.GetAsync(url);


            // response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var listedTokenData = JsonConvert.DeserializeObject<T>(responseBody);
            
            return listedTokenData;
        }
    }
}
