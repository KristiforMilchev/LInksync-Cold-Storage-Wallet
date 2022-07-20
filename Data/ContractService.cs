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
            var num =  convert * (decimal) Math.Pow(10, decimals);
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
                    Logo = "/images/tokenLogos/eth.jpg",
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

                    if (getContract.UserBalance > 0)
                        getContract.Price = getContract.UserBalance * await GetTokenPrice(network.Factory, getContract.ContractAddress, network.WS);
                    else
                        getContract.Price = 0;

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
