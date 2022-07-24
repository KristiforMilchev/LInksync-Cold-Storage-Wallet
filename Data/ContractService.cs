using NBitcoin;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
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
using System.Net;
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


        [Function("getAmountsOut", "uint256[]")]
        public class ConvertRate : FunctionMessage
        {
            [Parameter("uint256", "amountIn", 1)]
            public BigInteger TokensToSell { get; set; }
            
            [Parameter("address[]", "path", 2)]
            public string[] Addresses { get; set; }
        }


        
        
        public static async Task<decimal> GetAccountBalance(int network, string endpoint)
        {
            try
            {
                var publicKey = MauiProgram.PublicAddress;
                var web3 = new Nethereum.Web3.Web3(endpoint);
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



        /// <summary>
        /// This is a base method for a price converter, it's main purpose is to convert the input amount of tokens from the native token to USD
        /// In order to perform the conversion it creates the call to the trading router taking the value of 1 of the pair token to USD 
        /// Then multiply the tokens over the price of 1 Native token to USD
        /// 
        /// Example 1 BNB at the time of writing this method 262 USD if we input 0.00000000000032 bnb we will get a conversion rate of 0.00000000008384 USD
        /// 
        /// Parameters:
        /// <param name="tokens">The input amount of tokens that has to be converted, example 0.0000000032 bnb to usd</param>
        /// <param name="addresses">Array of contract addresses, first is the token that we are converting, second contract is in the currency that we are converting to.</param>
        /// <param name="endpoint">Blockchain endpoint, https address of the Blockchain network </param>        
        /// <param name="router">Public contract address of the router where we want to check the price.</param>
        /// </summary>
        public static async Task<decimal> ConvertTokenToUsd(decimal tokens, string[] addresses, string endpoint, string router)
        {
            try
            {
                var convertRateFunction = new ConvertRate
                {
                    TokensToSell = 1,
                    Addresses = addresses
                };

                var web3 = new Nethereum.Web3.Web3(endpoint);

                var balanceHandler = web3.Eth.GetContractQueryHandler<ConvertRate>();
                
                var balance = await balanceHandler.QueryAsync<List<BigInteger>>(router, convertRateFunction);
                //var convert = ConvertToDex(balance, 18);
                return tokens * (decimal)balance.ElementAt(1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
         
        }


        public static async Task<decimal> CheckUserBalanceForContract(string ownerAddress, string contract, string endpoint, int decimals)
        {
 
            var balanceOfFunctionMessage = new BalanceOf()
            {
                Owner = ownerAddress,
            };
            var web3 = new Nethereum.Web3.Web3(endpoint);

            var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOf>();
            var balance = await balanceHandler.QueryAsync<BigInteger>(contract, balanceOfFunctionMessage);
            var convert = ConvertToDex(balance, decimals);
            return convert;
        }

       
        public static decimal ConvertToDex(BigInteger blockNumber, int decimals)
        {
            var convert = decimal.Parse(blockNumber.ToString());
            var num =  convert / (decimal) Math.Pow(10, decimals);
            return num;
        }


        public static decimal ConvertToDexDecimal(decimal number, int decimals)
        {
            var num = number / (decimal)Math.Pow(10, decimals);
            return num;
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




            var getNetworkData = MauiProgram.NetworkSettings.FirstOrDefault(x => x.Id == networkId && x.IsProduction == MauiProgram.IsDevelopment);

            if(getNetworkData != null)
            {
                tokens.Add(new Token
                {
                    Symbol = getNetworkData.TokenSylmbol,
                    Name = getNetworkData.Name,
                    Logo = "/images/tokenLogos/bsc.png",
                    IsChainCoin = true,
                    Contracts = new List<TokenContract>
                        {
                            new TokenContract
                            {
                                ContractAddress = getNetworkData.CurrencyAddress,
                                UserBalance = await GetAccountBalance(networkId, getNetworkData.Endpoint),
                               // Price =  await GetTokenPrice(getNetworkData.Factory, getNetworkData.CurrencyAddress, getNetworkData.PairCurrency, getNetworkData.Endpoint, getNetworkData.WS)
                            }
                        }
                });
            }

          
 
            tokens = await GetListedTokens(listedTokenData, tokens, getNetworkData);

            return tokens;
        }

        private static async Task<List<Token>> GetListedTokens(List<ListedToken> listedTokenData, List<Token> tokens, NetworkSettings network)
        {
            foreach(var token in listedTokenData)
            {
                var currentToken = await GetRequest<Token>($"https://raw.githubusercontent.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/main/Models/Tokens/{token.name}/token.json");
                var contracts = new List<TokenContract>();

                foreach (var getContract in currentToken.Contracts)
                {
                    getContract.UserBalance = await CheckUserBalanceForContract(MauiProgram.PublicAddress, getContract.ContractAddress, network.Endpoint, getContract.Decimals);
                    var getTokenPrice = await CheckContractPrice(getContract.MainLiquidityPool, getContract.ContractAddress, getContract.PairTokenAddress, 9, 18, network.Endpoint);
                    var pairs = new string[2];
                    pairs[0] = getContract.PairTokenAddress;
                    pairs[1] = network.PairCurrency;
                    getTokenPrice = await ConvertTokenToUsd(getTokenPrice, pairs, network.Endpoint, getContract.ListedExchangeRouter); //Convert to USDT

                    if (getContract.UserBalance > 0)
                    {
                        getContract.Price = getContract.UserBalance * getTokenPrice;
                        getContract.CurrentPrice = getTokenPrice;
                    }  
                    else
                    {
                        getContract.Price = 0;
                        getContract.CurrentPrice = getTokenPrice;
                    }

                    (decimal circulating, decimal mCap) tokenMarketData = await GetContractMarketCap(getContract.Supply, getTokenPrice, getContract.ContractAddress, network.Endpoint, getContract.Decimals);
                    getContract.MarketCap = tokenMarketData.mCap;
                    getContract.CirculatingSupply = tokenMarketData.circulating;

                    contracts.Add(getContract);
                }
                currentToken.Contracts = contracts;

                tokens.Add(currentToken);
            }

            return tokens;
        }


        //Calculates the price between the Liquidity pool of the main token X and the wrapped pair Y
        public static async Task<decimal> CheckContractPrice(string contractAddress, string token, string pair, int tokenDecimals, int pairDecimals, string endpoint)
        {
            var x = await CheckUserBalanceForContract(contractAddress, token, endpoint, tokenDecimals); // Main token 
            var y = await CheckUserBalanceForContract(contractAddress, pair, endpoint, pairDecimals); //Pair Token
            var pairOverToken = (y / x); // We devide the pair over the main token to get the current token price in the native token pair.
            return pairOverToken; 
        }


        private static async Task<(decimal, decimal)> GetContractMarketCap(decimal supply, decimal getTokenPrice, string contractAddress, string endpoint, int decimals)
        {
            var getBurned1 = await CheckUserBalanceForContract("0x000000000000000000000000000000000000dead", contractAddress, endpoint, decimals);
            var getBurned2 = await CheckUserBalanceForContract("0x0000000000000000000000000000000000000001", contractAddress, endpoint, decimals);

            var circulatingSupply = supply - (getBurned1 + getBurned2);

            var mCap = circulatingSupply * getTokenPrice;


            return (circulatingSupply, mCap);
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

        [Event("Sync")]
        class PairSyncEventDTO : IEventDTO
        {
            [Parameter("uint112", "reserve0")]
            public virtual BigInteger Reserve0 { get; set; }

            [Parameter("uint112", "reserve1", 2)]
            public virtual BigInteger Reserve1 { get; set; }
        }


        public partial class GetPairFunction : GetPairFunctionBase { }

        [Function("getPair", "address")]
        public class GetPairFunctionBase : FunctionMessage
        {
            [Parameter("address", "tokenA", 1)]
            public virtual string TokenA { get; set; }
            [Parameter("address", "tokenB", 2)]
            public virtual string TokenB { get; set; }
        }
        

        private async static Task<decimal> GetTokenPrice(string factory, string baseCurrency, string ws)
        {
            try
            {
                var result = default(decimal);
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-API-Key", "0pEwf8PIgFMnSTgRV7UcAd1nn8lRUmmXoDoMqg8LpbIP2Aj0pvs9jWnkwZ93EoNp");
                var url = $"https://deep-index.moralis.io/api/v2/erc20/{baseCurrency}/price?chain={ws}&exchange={factory}";
                HttpResponseMessage response = await client.GetAsync(url);

                // response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var listedTokenData = JsonConvert.DeserializeObject<MoralisToken>(responseBody);

               

                return listedTokenData.usdPrice;
            }
            catch (Exception e)
            {

                Debug.WriteLine(e);
                return 0;
            }
           
        }
    }
}
