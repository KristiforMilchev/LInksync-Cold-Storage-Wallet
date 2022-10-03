using SYNCWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static SYNCWallet.Models.GithubTokensModel;

namespace SYNCWallet.Services.Definitions
{
    public interface IContractService
    {
        /// <summary>
        /// Gets the native token balance of a wallet based on the supplied network RPC endpoint
        /// Parameters:
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>        
        /// </summary>
        public Task<decimal> GetAccountBalance(string endpoint);

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
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>        
        /// <param name="router">Public contract address of the router where we want to check the price.</param>
        /// </summary>
        public Task<decimal> ConvertTokenToUsd(decimal tokens, string[] addresses, string endpoint, string router);

        /// <summary>
        /// Interacts with a solidity smart contract to get the balance of the selected public address
        /// Parameters:
        /// <param name="ownerAddress">The public address that we want to check</param>
        /// <param name="contract">The contract address that we want to run the check against.</param>
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>        
        /// <param name="decimals">Delimiter of the decimals in case the contract has less then 18 decimals.</param>       
        /// </summary>
        public Task<decimal> CheckUserBalanceForContract(string ownerAddress, string contract, string endpoint, int decimals);

        /// <summary>
        /// Interacts with a solidity smart contract to get the base details of a token
        /// Parameters:
        /// <param name="contract">The contract address that we want to run the check against.</param>
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>       
        /// </summary>
        public Task<TokenBaseDetails> CheckTokenDetails(string contract, string endpoint);

        /// <summary>
        /// Interacts with a solidity smart contract to get the total supply of a token
        /// Parameters:
        /// <param name="contract">The contract address that we want to run the check against.</param>
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param> 
        /// <param name="delimiter">unit used for display to users (in any UI, from wallets to exchanges or any Dapp). This is comparable to Ether, which uses 18 decimals for display. When showing 1080250000000000000000 for an 18 decimals token, it is much more user-friendly to display it as 1'080.25 instead.</param>       
        /// </summary>
        Task<decimal> GetTotalSupply(string contract, string endpoint, int delimiter);
       
        /// <summary>
        /// Interacts with a solidity smart contract to get the symbol of a token
        /// Parameters:
        /// <param name="contract">The contract address that we want to run the check against.</param>
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>       
        /// </summary>
       
        Task<string> GetTokenSymbol(string contract, string endpoint);
        /// <summary>
        /// Interacts with a solidity smart contract to get the token Decimals 
        /// Parameters:
        /// <param name="contract">The contract address that we want to run the check against.</param>
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>       
        /// </summary>
        Task<int> GetDelimiter(string contract, string endpoint);
    
        //TODO implement in V2
        //public Task<string> MintToken(); 

        /// <summary>
        /// Gets the user imported and officially listed tokens for a given network
        /// Parameters:
        /// <param name="networkId">Internal ID of the selected network </param>        
        /// </summary>
        public Task<List<Token>> GetNetworkTokensIntial(int networkId);
        /// <summary>
        /// Gets the user imported and officially listed tokens for a given network
        /// Parameters:
        /// <param name="networkId">Internal ID of the selected network </param>        
        /// </summary>
        public Task<List<SYNCWallet.Models.Token>> GetNetworkTokens(int networkId);

        /// <summary>
        /// Checks the price of a given contract
        /// Parameters:
        /// <param name="contractAddress">The address of the router that holds the 2 pairs</param>
        /// <param name="token">The contract address.</param>
        /// <param name="pair">The contract liquidity pair.</param>
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>        
        /// <param name="pairDecimals">Delimiter of the decimals in case the contract has less then 18 decimals.</param>          
        /// </summary>
        public Task<decimal> CheckContractPrice(string contractAddress, string token, string pair, int tokenDecimals, int pairDecimals, string endpoint);

        /// <summary>
        /// Gets the market cap and the ciruclating supply of a token.
        /// Parameters:
        /// <param name="supply">The total supply of the contract</param>
        /// <param name="getTokenPrice">Current price of the token in USD</param>
        /// <param name="contractAddress">The contract that we try to find the market capital and circulating supply</param>
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>        
        /// <param name="decimals">Delimiter of the decimals in case the contract has less then 18 decimals.</param>          
        /// </summary>
        public Task<(decimal, decimal)> GetContractMarketCap(decimal supply, decimal getTokenPrice, string contractAddress, string endpoint, int decimals);

        /// <summary>
        /// Check if a contract is listed on the supported defi exchange
        /// Parameters:
        /// <param name="contractAddress">The address of the contract that we try to check</param>
        /// <param name="pairToken">The liquidity pair</param>
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>        
        /// <param name="factory">The address of the exchange</param>          
        /// </summary>
        public Task<string> CheckExchangelisting(string contractAddress, string pairToken, string endpoint, string factory);


        /// <summary>
        /// Gets the total supply of a given asset.
        /// Parameters:
        /// <param name="contractAddress">The address of the contract that we try to check</param>
        /// <param name="endpoint">Blockchain endpoint, RPC address of the Blockchain network </param>        
        /// <param name="tokenDecimals">Decimal points of the contract that we check</param>          
        /// </summary>
        public Task<decimal> CheckExistingSupply(string contractAddress, string endpoint, int tokenDecimals);


        /// <summary>
        /// Internal method that calls CheckUserBalanceForContract so it can retrive token information for mcap, supply and price.
        /// Parameters:
        /// <param name="network">Selected network object</param>
        /// <param name="getContract">imported token object.</param>
        /// </summary>
        public Task<decimal> GetImportedData(NetworkSettings network, TokenContract getContract);

        /// <summary>
        /// Returns a list of tokens from the official repository main branch.
        /// Parameters:
        /// <param name="network">Selected network object</param>
        /// <param name="listedTokenData">Already listed assets collection</param>
        /// <param name="tokens">The local collection of imported tokens</param>
        /// </summary>
        public Task<List<Token>> GetListedTokensInitial(List<ListedToken> listedTokenData, List<Token> tokens, NetworkSettings network);
        /// <summary>
        /// Returns a list of tokens from the official repository main branch.
        /// Parameters:
        /// <param name="network">Selected network object</param>
        /// <param name="listedTokenData">Already listed assets collection</param>
        /// <param name="tokens">The local collection of imported tokens</param>

        /// </summary>
        public Task<List<Token>> GetListedTokens(List<ListedToken> listedTokenData, List<Token> tokens, NetworkSettings network);

        /// <summary>
        /// Interacts with a smart contract and tries to transfer tokens from wallet address to another wallet address
        /// Parameters:
        /// <param name="receiver">The address that has to receive the funds</param>
        /// <param name="token">Selected token object.</param>
        /// <param name="amountToSend">The amount to send.</param>
        /// <param name="account">Decrypted web3 wallet.</param>
        /// <param name="endpoint">The network RPC endpoint where to send the request.</param>
        /// <param name="chainId">The chain ID of the blockchain</param>
        /// </summary>
        public Task<bool> ExecutePayments(string receiver, TokenContract token, decimal amountToSend, Nethereum.Web3.Accounts.Account account, string endpoint, int chainId);

        /// <summary>
        /// Send native blockchain tokens to a given address
        /// Parameters:
        /// <param name="receiver">The address that has to receive the funds</param>
        /// <param name="amountToSend">The amount to send.</param>
        /// <param name="account">Decrypted web3 wallet.</param>
        /// <param name="endpoint">The network RPC endpoint where to send the request.</param>
        /// <param name="chainId">The chain ID of the blockchain</param>
        /// </summary>
        public Task<bool> ExecuteNative(string receiver, decimal amountToSend, Nethereum.Web3.Accounts.Account account, string endpoint, int chainId);
        
        /// <summary>
        /// Returns the amount of pair token in a given liquidity pool
        /// Parameters:
        /// <param name="token">TokenContract object containing all the market data provider properties to query the LP holdings.</param>
        /// </summary>
        public Task<(decimal,decimal)> GetContractLpSupply(TokenContract token);

    }
}
