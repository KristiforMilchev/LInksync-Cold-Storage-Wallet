using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using NFTLock.Models;
using SYNCWallet;
using SYNCWallet.Models;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Domain.Models;
using static SYNCWallet.Models.GithubTokensModel;

namespace NFTLock.Data
{
    public class ContractService : IContractService
    {
       
        public IUtilities Utilities { get; set; }
        public ICommunication Communication { get; set; }
        public IHardwareService HardwareService { get; set; }
        public ICacheRepository<RangeBarModel> PriceCacheRepository { get; set; } 
        public ICacheRepository<CurrencyDataSetting> CurrencyCacheSettingRepository { get; set; }
        List<TokenContract> CachedTokenContracts { get; set; }
        decimal USDPrice { get; set; }
        
        public ContractService(IUtilities utilities, ICommunication communication, IHardwareService hardwareService,
            ICacheRepository<RangeBarModel> priceCacheRepository, ICacheRepository<CurrencyDataSetting> currencyCache)
        {
            Utilities = utilities;
            Communication = communication;
            HardwareService = hardwareService;
            PriceCacheRepository = priceCacheRepository;
            CurrencyCacheSettingRepository = currencyCache;

        }

        public async Task<decimal> GetAccountBalance(string endpoint)
        {
            try
            {
                var publicKey = Communication.PublicAddress;
                var web3 = new Nethereum.Web3.Web3(endpoint);
                var balance = await web3.Eth.GetBalance.SendRequestAsync(publicKey);
                var etherAmount = Web3.Convert.FromWei(balance.Value);

                Console.WriteLine(web3);
                Console.WriteLine("Get txCount " + etherAmount);

                return etherAmount;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 0;
            }
        }


        public async Task<decimal> ConvertTokenToUsd(decimal tokens, string[] addresses, string endpoint, string router)
        {
            try
            {
                //Sets up a conversion query for the solidity smart contract
                var convertRateFunction = new ConvertRate
                {
                    TokensToSell = 1,
                    Addresses = addresses
                };

                var web3 = new Nethereum.Web3.Web3(endpoint);
                
                var balanceHandler = web3.Eth.GetContractQueryHandler<ConvertRate>();
                
                //Attempts to interact with a solidity smart contract exchange router to convert the base 1 token compared to USD
                var balance = await balanceHandler.QueryAsync<List<BigInteger>>(router, convertRateFunction);
                
                //Returns the sum of the tokens that we want to convert over the price of the pair of Native/USD to get the smart contract / USD instead of it's pair token.
                return tokens * (decimal)balance.ElementAt(1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                

                return 0;
            }
         
        }

        public async Task<decimal> CheckUserBalanceForContract(string ownerAddress, string contract, string endpoint, int decimals)
        {
            try
            {
                //Construct a balanceOf query and assign the current address as the owner
                var balanceOfFunctionMessage = new BalanceOf()
                {
                    Owner = ownerAddress,
                };

                //Initialize web3
                var web3 = new Nethereum.Web3.Web3(endpoint);

                //Run the qeury against the endpoint and attempt to get the user bgalance for a given contract.
                var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOf>();
                var balance = await balanceHandler.QueryAsync<BigInteger>(contract, balanceOfFunctionMessage);
                var convert = Utilities.ConvertToBigIntDex(balance, decimals);
                return convert;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var dt = DateTime.UtcNow;
                Debug.WriteLine($"{dt.ToShortDateString()} {dt.ToShortTimeString()} -> Error checking account balance for   Owner: {ownerAddress} Contract: {contract} Network: {endpoint} ");
                return 0;
            }
         
        }

        public async Task<TokenBaseDetails> CheckTokenDetails(string contract, string endpoint)
        {
            var delimiter = await GetDelimiter(contract, endpoint);
            var symbol = await GetTokenSymbol(contract, endpoint);
            var supply = await GetTotalSupply(contract, endpoint, delimiter);

            return new TokenBaseDetails
            {
                Decimals = delimiter,
                Address = contract,
                Symbol = symbol,
                Supply = supply

            };
        }

        public async Task<decimal> GetTotalSupply(string contract, string endpoint, int delimiter)
        {
            try
            {

                //Construct a balanceOf query and assign the current address as the owner
                var supplyFunction = new GetSupply()
                {

                };
                //Initialize web3
                var web3 = new Nethereum.Web3.Web3(endpoint);

                var supplyHandler = web3.Eth.GetContractQueryHandler<GetSupply>();
                var supply = await supplyHandler.QueryAsync<BigInteger>(contract, supplyFunction);
                return Utilities.ConvertToBigIntDex(supply, delimiter);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public async Task<string> GetTokenSymbol(string contract, string endpoint)
        {
            try
            {

                //Construct a balanceOf query and assign the current address as the owner


                var symbolFunction = new GetSymbol()
                {
                };

                 
                //Initialize web3
                var web3 = new Nethereum.Web3.Web3(endpoint);



                var symbolHander = web3.Eth.GetContractQueryHandler<GetSymbol>();
                return await symbolHander.QueryAsync<string>(contract, symbolFunction);
 

 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }

        public async Task<int> GetDelimiter(string contract, string endpoint)
        {
            try
            {

                //Construct a balanceOf query and assign the current address as the owner
                var decimalsFunction = new GetDecimals()
                {
                };

                
                //Initialize web3
                var web3 = new Nethereum.Web3.Web3(endpoint);
 

                var decimalHandler = web3.Eth.GetContractQueryHandler<GetDecimals>();
                return await decimalHandler.QueryAsync<int>(contract, decimalsFunction);

 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 18;
            }

        }

        //public async Task<string> MintToken()
        //{
        //    var transactionReceipt = default(string);
        //    //TODO implement in v2
        //    //var addFactFunction = contract.GetFunction("mint");
        //    //var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

        //    //var transfer = new TransferFunction()
        //    //{

        //    //    AmountToSend = new BigInteger( 0.1  * Math.Pow(10,18)),
        //    //    _mintAmount = new HexBigInteger(1)
        //    //};
        //    //transfer.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(15, UnitConversion.EthUnit.Gwei);

        //    //try
        //    //{
        //    //    transfer.Gas = GAS;
        //    //}
        //    //catch (Exception e) 
        //    //{
        //    //    Console.WriteLine(e);
        //    //}



        //    // try
        //    //{
        //    //    transactionReceipt =   transferHandler.SendRequestAsync(Contract, transfer).GetAwaiter().GetResult();
        //    //}
        //    //catch(Exception e)
        //    //{
        //    //    Console.WriteLine(e);
        //    //}

        //    return transactionReceipt;
        //}


        public async Task<List<Token>> GetNetworkTokensIntial(int networkId)
        {
            //Define a local collection of token.
            var tokens = new List<Token>();
            try
            {
                if (CachedTokenContracts == null)
                {
                    //Checks if the price cache exist, delete and recreate it.
                    if (File.Exists($"{Utilities.GetOsSavePath(HardwareService.Os)}/CachePrices.json"))
                    {
                        var content = File.ReadAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/CachePrices.json");
                        CachedTokenContracts = JsonConvert.DeserializeObject<List<TokenContract>>(content);
                        CachedTokenContracts = CachedTokenContracts == null ? new List<TokenContract>() : CachedTokenContracts;
                    }
                    else
                        CachedTokenContracts = new List<TokenContract>();
                }

                // On startup, it gets thge list of officially supported tokens by running a query against githubs API
                if (Communication.ListedTokens == null)
                    Communication.ListedTokens = await Utilities.GetRequest<List<ListedToken>>($"https://api.github.com/repos/KristiforMilchev/LinkSync-Whitelistings/contents/Whitelist");

               

                //Check if the selected network exists
                var getNetworkData = Communication.NetworkSettings.FirstOrDefault(x => x.Id == networkId && x.IsProduction == Communication.IsDevelopment);

                //In case it exists we add the native token to the list and run a query to get the user balance of the token.
                if (getNetworkData != null)
                {
                    tokens.Add(new Token
                    {
                        Symbol = getNetworkData.TokenSylmbol,
                        Name = getNetworkData.Name,
                        Logo = getNetworkData.Logo,
                        IsChainCoin = true,
                        Contracts = new List<TokenContract>
                            {
                                new TokenContract
                                {
                                    ContractAddress = getNetworkData.CurrencyAddress,
                                    UserBalance = await GetAccountBalance(getNetworkData.Endpoint),
                                    Network = networkId,

                                }
                            }
                    });
                }

                //Gets the list of officially supported tokens on the selected network.
                tokens = await GetListedTokensInitial(Communication.ListedTokens, tokens, getNetworkData);

                //Checks if the user has imported tokens in case it the file doesn't exists it creates a blank one.
                if (!File.Exists($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json"))
                    File.WriteAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json", "");

                //Reads the content of the file.
                var filesContent = File.ReadAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json");



                //Converts the imported tokens to List<Token> 
                var tokenList = JsonConvert.DeserializeObject<List<Token>>(filesContent);

                //Checks if the tokens exist, in case the file is blank the collection is null
                if (tokenList != null)
                {
                    //Filter only by selected network
                    tokenList = tokenList.Where(x => x.Contracts.Any(y => y.Network == networkId)).ToList();

                    //Loop over the token list
                    foreach (var currentToken in tokenList)
                    {
                        var getContract = currentToken.Contracts.FirstOrDefault(x => x.Network == networkId);

                        var exists = CachedTokenContracts.FirstOrDefault(x => x.ContractAddress == getContract.ContractAddress);
                        if (exists != null)
                            currentToken.Contracts = new List<TokenContract>{
                                exists
                            };
                        tokens.Add(currentToken);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
        


            return tokens;
        }

        public async Task<List<Token>> GetListedTokensInitial(List<ListedToken> listedTokenData, List<Token> tokens, NetworkSettings network)
        {
            //Check if there are no listed tokens
            if (listedTokenData == null)
                return tokens;

            //In case tokens exist loop over the tokens and convert them to a system token
            foreach (var token in listedTokenData)
            {
                var currentToken = await Utilities.GetRequest<Token>($"https://raw.githubusercontent.com/KristiforMilchev/LinkSync-Whitelistings/main/Whitelist/{token.name}/token.json");
                var contracts = new List<TokenContract>();

                //Get the current contract on the network
                var getContract = currentToken.Contracts.FirstOrDefault(x => x.Network == network.Id);
                if (getContract != null)
                {
                    var exists = CachedTokenContracts.FirstOrDefault(x => x.ContractAddress == getContract.ContractAddress);

                    if (exists != null)
                    {
                        if (string.IsNullOrEmpty(exists.PairName))
                            exists.PairName = getContract.PairName;
                        
                        currentToken.Contracts = new List<TokenContract>{
                            exists
                        };
                    }
                    
                    tokens.Add(currentToken);
                }

            }

            return tokens;
        }

        public async Task<List<Token>> GetNetworkTokens(int networkId)
        {
            // On startup, it gets thge list of officially supported tokens by running a query against githubs API
            if(Communication.ListedTokens == null)
                Communication.ListedTokens = await Utilities.GetRequest<List<ListedToken>>($"https://api.github.com/repos/KristiforMilchev/LinkSync-Whitelistings/contents/Whitelist"); // 

            //Define a local collection of token.
            var tokens = new List<Token>();

            //Check if the selected network exists
            var getNetworkData = Communication.NetworkSettings.FirstOrDefault(x => x.Id == networkId && x.IsProduction == Communication.IsDevelopment);

            //In case it exists we add the native token to the list and run a query to get the user balance of the token.
            if(getNetworkData != null)
            {
                var pairExists = await CheckExchangelisting(getNetworkData.CurrencyAddress, getNetworkData.PairCurrency, getNetworkData.Endpoint, getNetworkData.Factory);
                var tokenPrice = await CheckContractPrice(pairExists, getNetworkData.CurrencyAddress, getNetworkData.PairCurrency, 18, 18, getNetworkData.Endpoint);
                USDPrice = tokenPrice;
                tokens.Add(new Token
                {
                    Symbol = getNetworkData.TokenSylmbol,
                    Name = getNetworkData.Name,
                    Logo = getNetworkData.Logo,
                    IsChainCoin = true,
                    Contracts = new List<TokenContract>
                        {
                            new TokenContract
                            {
                                ContractAddress = getNetworkData.CurrencyAddress,
                                UserBalance = await GetAccountBalance(getNetworkData.Endpoint),
                                Network = networkId,
                                CurrentPrice = tokenPrice,
                            }
                        }
                });
            }
            
            //Gets the list of officially supported tokens on the selected network, as it binds user balance, market cap, price, and circulating supply for each token.
            tokens = await GetListedTokens(Communication.ListedTokens, tokens, getNetworkData);

            //Checks if the user has imported tokens in case it the file doesn't exists it creates a blank one.
            if (!File.Exists($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json"))
                File.WriteAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json", "");

            //Reads the content of the file.
            var filesContent = File.ReadAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json");

            //Converts the imported tokens to List<Token> 
            var tokenList = JsonConvert.DeserializeObject<List<Token>>(filesContent);

            //Checks if the tokens exist, in case the file is blank the collection is null
            if(tokenList != null)
            {
                //Filter only by selected network
                tokenList = tokenList.Where(x => x.Contracts.Any(y => y.Network == networkId)).ToList();

                //Loop over the token list
                foreach (var currentToken in tokenList)
                {
                    //Current token in the quoue 
                    var current = currentToken; 
                    //Get the contract of the token (Contracts with the same name are grouped over different blockchains, some projects have multiple contracts) We only get the contract for the current network
                    var getContract = currentToken.Contracts.FirstOrDefault(x => x.Network == networkId);

                    //Attempt to update the user balance of the contract, but calling CheckUserBalanceForContract internally on the selected contract
                    current.Contracts.FirstOrDefault(x => x.Network == networkId).UserBalance = await GetImportedData(getNetworkData, currentToken.Contracts.FirstOrDefault(x => x.Network == networkId));
                    //Save reference to local varible with the contract address..
                    var contract = current.Contracts.FirstOrDefault(x => x.Network == networkId).ContractAddress;
                    Task.Run(async () =>
                    {
                        //Check if the network tha the contract belongs to has a defined factory adddress of a defi exchange
                        if (!string.IsNullOrEmpty(getNetworkData.Factory))
                        {
                            //Attempt to get the router of a contract using the contract address of the listed token,
                            //the native currency of the netowrk, the network endpoint and running the data as query agains the factory of the network
                            var pairExists = await CheckExchangelisting(contract, getNetworkData.CurrencyAddress, getNetworkData.Endpoint, getNetworkData.Factory);
                            // If Router exists and is found attempt to get price market cap, circulating supply of a given address.
                            if (!string.IsNullOrEmpty(pairExists))
                            {
                                //Run a query against the router of the token to get the price in the native token,
                                var getTokenPrice = default(decimal);

                                getTokenPrice = await CheckContractPrice(pairExists, getContract.ContractAddress, getNetworkData.CurrencyAddress, getContract.Decimals, 18, getNetworkData.Endpoint);

                                //If Native token pair doesn't exist, try most common pair
                                if(getTokenPrice == 0)
                                    getTokenPrice = await CheckContractPrice(pairExists, getContract.ContractAddress, getNetworkData.CurrencyAddress, getContract.Decimals, 18, getNetworkData.Endpoint);

                                var pairs = new string[2];
                                //Construct an array in order to convert the native price of the token to USD
                                pairs[0] = getContract.PairTokenAddress == null ? getNetworkData.CurrencyAddress : getContract.PairTokenAddress;
                                pairs[1] = getNetworkData.PairCurrency;
                                //Runs a query to conver the native token price to usd

                                //var tryParseToUsd = await ConvertTokenToUsd(getTokenPrice, pairs, getNetworkData.Endpoint, pairExists);
                                //if(tryParseToUsd > 0)
                                //    getTokenPrice = tryParseToUsd;

                                getTokenPrice = getTokenPrice * USDPrice;
                                //Sets the current price, as the main price in USD rather then the native token
                                current.Contracts.FirstOrDefault(x => x.Network == networkId).CurrentPrice = getTokenPrice;
                                //Naming convention is wrong here should be Worth or BalanceValue, but basically
                                //it takes the users token balance and multiplies the by the price in usd to estimate the portfolio balance at this time
                                current.Contracts.FirstOrDefault(x => x.Network == networkId).Price = current.Contracts.FirstOrDefault(x => x.Network == networkId).UserBalance * getTokenPrice;

                                //Gets the total supply of the token by running a query against the solidity smart contract.
                                var totalSupply = await this.CheckExistingSupply(getContract.ContractAddress, getNetworkData.Endpoint, getContract.Decimals);
                                getContract.Supply = totalSupply;

                                //Calculates the market capital and the ciruclating supply of the token. then assigns both varibles.
                                (decimal circulating, decimal mCap) tokenMarketData = await GetContractMarketCap(getContract.Supply, getTokenPrice, getContract.ContractAddress, getNetworkData.Endpoint, getContract.Decimals);
                          

                                
                                current.Contracts.FirstOrDefault(x => x.Network == networkId).MarketCap = tokenMarketData.mCap;
                                current.Contracts.FirstOrDefault(x => x.Network == networkId).CirculatingSupply = tokenMarketData.circulating;

                                if (CachedTokenContracts.FirstOrDefault(x => x.ContractAddress == contract) != null)
                                {
                                    CachedTokenContracts.Remove(CachedTokenContracts.FirstOrDefault(x => x.ContractAddress == contract));
                                }
                                CachedTokenContracts.Add(current.Contracts.FirstOrDefault(x => x.Network == networkId));
                            }

                        }
                    });

                    if (current.Contracts.FirstOrDefault(y => y.Network == networkId) != null)
                    {
                        var contractData = CachedTokenContracts.FirstOrDefault(x => x.ContractAddress == current.Contracts.FirstOrDefault(y => y.Network == networkId).ContractAddress);
                        if(contractData != null)
                            current.Contracts = new List<TokenContract>
                            {
                                contractData
                            };
                    }
                   
                    //We add the token to the UI list, regardless if internal information about the tokens has been found.
                    tokens.Add(current);
                }


                //Checks if the price cache exist, delete and recreate it.
                if (File.Exists($"{Utilities.GetOsSavePath(HardwareService.Os)}/CachePrices.json"))
                    File.Delete($"{Utilities.GetOsSavePath(HardwareService.Os)}/CachePrices.json");
                
                File.WriteAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/CachePrices.json", JsonConvert.SerializeObject(CachedTokenContracts));
            }


            return tokens;
        }


        //Calculates the price between the Liquidity pool of the main token X and the wrapped pair Y
        public async Task<decimal> CheckContractPrice(string contractAddress, string token, string pair, int tokenDecimals, int pairDecimals, string endpoint)
        {
            var x = await CheckUserBalanceForContract(contractAddress, token, endpoint, tokenDecimals); // Main token 
            var y = await CheckUserBalanceForContract(contractAddress, pair, endpoint, pairDecimals); //Pair Token

            //We cannot devide by zero.
            if (x == 0 || y == 0)
                return 0;

            var pairOverToken = (y / x); // We devide the pair over the main token to get the current token price in the native token pair.
            return pairOverToken; 
        }

     
        public async Task<(decimal, decimal)> GetContractMarketCap(decimal supply, decimal getTokenPrice, string contractAddress, string endpoint, int decimals)
        {
            //Check most common dead addresses used for token burning for any burned tokens out of the total supply.
            var getBurned1 = await CheckUserBalanceForContract("0x000000000000000000000000000000000000dead", contractAddress, endpoint, decimals);
            var getBurned2 = await CheckUserBalanceForContract("0x0000000000000000000000000000000000000001", contractAddress, endpoint, decimals);

            //Calculate the circulating supply by substracting the total supply from the net sum of both burned addresses.
            var circulatingSupply = supply - (getBurned1 + getBurned2);

            //Multiply the circulating supply over the current token price to calculate the market cap of an asset.
            var mCap = circulatingSupply * getTokenPrice;

            //We return a tuple, as it's more conviniant, no need to runt he query against for both cases.
            return (circulatingSupply, mCap);
        }

        public async Task<string> CheckExchangelisting(string contractAddress,string pairToken, string endpoint, string factory)
        {
            try
            {
                //Construct a function that calls solidity getPair in order to check if there is a listed token with that pair on the factory
                var balanceOfFunctionMessage = new GetPairFunctionBase()
                {
                    TokenA = contractAddress,
                    TokenB = pairToken

                };

                //Construct a web3 handler
                var web3 = new Nethereum.Web3.Web3(endpoint);

                //Run the qeury
                var routerResolver = web3.Eth.GetContractQueryHandler<GetPairFunctionBase>();
                var router = await routerResolver.QueryAsync<string>(factory, balanceOfFunctionMessage);

                //Return the router of the pair in case it exists
                return router;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return String.Empty;
            }
           
        }

        public async Task<decimal> CheckExistingSupply(string contractAddress, string endpoint, int tokenDecimals)
        {
            var totalSupplyFunction = new GetTokenSupply()
            {


            };
            var web3 = new Nethereum.Web3.Web3(endpoint);

            var routerResolver = web3.Eth.GetContractQueryHandler<GetTokenSupply>();
            var totalSupply = await routerResolver.QueryAsync<BigInteger>(contractAddress, totalSupplyFunction);

            return Utilities.ConvertToBigIntDex(totalSupply, tokenDecimals);
        }

        public async Task<decimal> GetImportedData(NetworkSettings network, TokenContract getContract)
        {
            return await CheckUserBalanceForContract(Communication.PublicAddress, getContract.ContractAddress, network.Endpoint, getContract.Decimals);
        }

        public async Task<List<Token>> GetListedTokens(List<ListedToken> listedTokenData, List<Token> tokens, NetworkSettings network)
        {
            //Check if there are no listed tokens
            if (listedTokenData == null)
                return tokens;

            //In case tokens exist loop over the tokens and convert them to a system token
            foreach (var token in listedTokenData)
            {
                var currentToken = await Utilities.GetRequest<Token>($"https://raw.githubusercontent.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/main/Models/Tokens/{token.name}/token.json");
                var contracts = new List<TokenContract>();
                //Get the current contract on the network
                var getContract = currentToken.Contracts.FirstOrDefault(x => x.Network == network.Id);

                Task.Run(async () =>
                {
           

                    if (getContract != null)
                    {
                        //Check the user balance of the given contract
                        getContract.UserBalance = await CheckUserBalanceForContract(Communication.PublicAddress, getContract.ContractAddress, network.Endpoint, getContract.Decimals);
                        //Get the contract native price
                        var getTokenPrice = await CheckContractPrice(getContract.MainLiquidityPool, getContract.ContractAddress, getContract.PairTokenAddress, getContract.Decimals, 18, network.Endpoint);

                        //Construct a query to conver the price to USD
                        var pairs = new string[2];
                        pairs[0] = getContract.PairTokenAddress;
                        pairs[1] = network.PairCurrency;
                        //Convert the price to USD
                        getTokenPrice = await ConvertTokenToUsd(getTokenPrice, pairs, network.Endpoint, getContract.ListedExchangeRouter); //Convert to USDT

                        //Bind the price values
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

                        //Get contract market cap, and circulating supply
                        (decimal circulating, decimal mCap) tokenMarketData = await GetContractMarketCap(getContract.Supply, getTokenPrice, getContract.ContractAddress, network.Endpoint, getContract.Decimals);
                        getContract.MarketCap = tokenMarketData.mCap;
                        getContract.CirculatingSupply = tokenMarketData.circulating;
                        
                        if (CachedTokenContracts.FirstOrDefault(x => x.ContractAddress == getContract.ContractAddress) != null)
                            CachedTokenContracts.Remove(CachedTokenContracts.FirstOrDefault(x => x.ContractAddress == getContract.ContractAddress));
                        CachedTokenContracts.Add(getContract);
                    }
                });
              
                if(getContract != null && CachedTokenContracts != null)
                {
                    var newCache = new List<TokenContract>();
                    CachedTokenContracts.ForEach(x =>
                    {
                        if(x != null)
                            newCache.Add(x);
                    });
                    CachedTokenContracts = newCache;
                    var contractData = CachedTokenContracts.FirstOrDefault(x => x.ContractAddress == getContract.ContractAddress);
                    if (contractData != null)
                    {
                        currentToken.Contracts = new List<TokenContract>
                        {
                            contractData
                        };
                            tokens.Add(currentToken);
                    }

                }
            }

            //Checks if the price cache exist, delete and recreate it.
            if (File.Exists($"{Utilities.GetOsSavePath(HardwareService.Os)}/CachePrices.json"))
                File.Delete($"{Utilities.GetOsSavePath(HardwareService.Os)}/CachePrices.json");

            File.WriteAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/CachePrices.json", JsonConvert.SerializeObject(CachedTokenContracts));

            return tokens;
        }

        public async Task<bool> ExecutePayments(string receiver, TokenContract token, decimal amountToSend, Nethereum.Web3.Accounts.Account account, string endpoint, int chainId)
        {
            //assign the wallet address as the payer
            var PayerAddress = account.Address;

            //copy the account
            var Account = account;
            //assign the network endpont and instatiate web3 
            var netowrkEndpoint = endpoint;
            var web3 = new Web3(Account, netowrkEndpoint);
            web3.TransactionManager.UseLegacyAsDefault = true; //Important otherwise lots of gas issues problems 

            //Define a query for a transfer event
            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferTokenFunction>();

            //Default to 0 tokens to transfer then attempt to set the tokens to transfer by multiplying by the power of the contract decimals
            var transferAmount = default(BigInteger);
            transferAmount = (BigInteger)Utilities.SetDecimalPoint(amountToSend, token.Decimals);

            //Assign all varibles to the transfer event.
            var transfer = new TransferTokenFunction()
            {
                FromAddress = PayerAddress,
                To = receiver,
                TokenAmount = transferAmount,
                Nonce = await Account.NonceService.GetNextNonceAsync()
            };

            //Define a receipt string
            var transactionReceipt = default(string);
            var logged = false;

            //Attempt to generate a tranasction hash
            try
            {
                transactionReceipt = await transferHandler.SendRequestAsync(token.ContractAddress, transfer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                logged = true;
                Communication.TxHash = "-";
            }

            //If transaction hash has been created, start monitoring for a receipt.
            if (transactionReceipt != null)
            {
                var txHash = string.Empty;
                if (!logged)
                {

                    txHash = transactionReceipt;
                    Communication.TxHash = txHash;
                }
            }
            return true;
        }

        public async Task<bool> ExecuteNative(string receiver, decimal amountToSend, Nethereum.Web3.Accounts.Account account, string endpoint, int chainId)
        {
            //Instantiate web3 and generate a transfer event.
            var web3 = new Web3(account, endpoint);

            var transaction = web3.Eth.GetEtherTransferService();
            //Get a nounce for the transaction
            var nounceVal = await account.NonceService.GetNextNonceAsync();

            //Set to use the legacy gas price model to ensure it wont fail.
            web3.TransactionManager.UseLegacyAsDefault = true;
            //Attempt to transfer the native token and await for the result.

            try
            {
                var trans = transaction.TransferEtherAsync(receiver, amountToSend, null, null, nounceVal).GetAwaiter().GetResult();
                Communication.TxHash = trans;
                return true;

            }
            catch (Exception e)
            {
                Communication.TxHash = "-";
                Console.WriteLine(e);
                return false;
            }
           

        }

        public async Task<(decimal,decimal)> GetContractLpSupply(TokenContract token)
        {
            var network = Communication.NetworkSettings.FirstOrDefault(x => x.Id == token.Network);
            var x = await CheckUserBalanceForContract(token.MainLiquidityPool, token.ContractAddress, network.Endpoint, token.Decimals); // Main token 
            var y = await CheckUserBalanceForContract(token.MainLiquidityPool, token.PairTokenAddress, network.Endpoint, 18); //Pair Token

            return (x, y);
        }

        public  async Task<(decimal lastDayPercentage, decimal lastMonthPercentage, decimal lastYearPercentage)> GetPriceChange(string symbol)
        {
            var lastMonth = DateTime.UtcNow.AddDays(-30);
            var current = DateTime.UtcNow;
            var lastDayTime = DateTime.UtcNow.AddDays(-1);
            var lastYear = DateTime.UtcNow.AddDays(-365);

            var lastWeekPercentage = await GetChangeForPeriod(lastMonth, current, symbol);
            var lastDayPercentage = await GetChangeForPeriod(lastDayTime, current, symbol);
            var lastYearPercentage = await GetChangeForPeriod(lastYear, current, symbol);
            
            return (lastDayPercentage, lastWeekPercentage, lastYearPercentage);
        }

        public async Task<List<RangeBarModel>> GetContractPriceData(string contractAddress, string pairCurrency, DateTime from, DateTime to)
        {
            var cacheCombined = new List<RangeBarModel>();

            PriceCacheRepository.SelectDatabase(contractAddress);
            CurrencyCacheSettingRepository.SelectDatabase(contractAddress);
            var cacheResult = PriceCacheRepository.GetAllRange("", from, to);
            var settings = CurrencyCacheSettingRepository.GetEntity(contractAddress);
            if (cacheResult != null && cacheResult.Count > 0)
            {
                if (settings != null && settings.LastUpdate.AddDays(1) < DateTime.UtcNow)
                {
                    cacheCombined = cacheResult;
                }
                else if(settings != null)
                {
                    return cacheResult;
                }
            }
            
            var network = Communication.ActiveNetwork;
            var result = await Utilities.GetRequest<List<RangeBarModel>>(
                $"{Communication.DataApiEndpoint}/home/GetRangeGeneric?currency={contractAddress}&&from={from.Ticks}&&to={to.Ticks}&&resolution=1&&networkId={network.Id}");

            AddCacheEntries(contractAddress, result, settings);
            cacheCombined.AddRange(result);
            return cacheCombined;
        }

        private void AddCacheEntries(string contractAddress, List<RangeBarModel> result, CurrencyDataSetting settings)
        {
            if (result != null)
            {
                Task.Run(() =>
                {
                    if (settings == null)
                    {
                        CurrencyCacheSettingRepository.CreateEntity(new CurrencyDataSetting
                        {
                            ContractAddress = contractAddress,
                            IsEnabled = 1,
                            LastUpdate = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        settings.LastUpdate = DateTime.UtcNow;
                        CurrencyCacheSettingRepository.UpdateEntity(settings);
                    }
                        
                    result.ForEach(x => { PriceCacheRepository.CreateEntity(x); });
                });
            }
        }

        private async Task<decimal> GetChangeForPeriod(DateTime from, DateTime to, string symbol)
        {
            PriceCacheRepository.SelectDatabase(symbol);
            CurrencyCacheSettingRepository.SelectDatabase(symbol);
            var cacheResult = PriceCacheRepository.GetAllRange("", from, to);
            var settings = CurrencyCacheSettingRepository.GetEntity(symbol);
            var diff = default(decimal);
            if (cacheResult != null && cacheResult.Count > 0)
            {
                var lastWeekData = cacheResult.FirstOrDefault().Close;
                var currentPrice = cacheResult.LastOrDefault().Close;
                var sevenDaysPriceChange = (currentPrice - lastWeekData);
                diff = (sevenDaysPriceChange.Value / lastWeekData.Value) * 100;
            }
            
            return diff;
        }

     
        
    }
}
