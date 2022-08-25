using Nethereum.Web3.Accounts;
using SYNCWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Services.Definitions
{
    public interface IAuthenicationService
    {
        /// <summary>
        /// Gets the public key of the connected wallet.
        /// </summary>
        public string GetDefault();
        /// <summary>
        /// Impoirts a custom EVM compatible blockchain network
        /// Parameters:
        /// <param name="networkName">Name of the network</param>            
        /// <param name="networkSymbol">SYMBOL of the network</param>        
        /// <param name="rpcUrl">rpc endpoint of the network</param>        
        /// <param name="chainID">chain idof the network</param>        
        /// <param name="blockExplorer">https block explorer</param>        
        /// </summary>
        public bool SetupNetwork(string networkName, string networkSymbol, string rpcUrl, int chainID, string blockExplorer);
        /// <summary>
        /// Imports custom token not listed on the official list.
        /// Parameters:
        /// <param name="contractAddress">Address of the contract that we import </param>        
        /// <param name="symbol">Token Symbol</param>        
        /// <param name="delimiter">Decimal count of the token</param>        
        /// <param name="supply">Total supply for tokens that have no minting</param>        
        /// <param name="network">Internal network id of the network under which we import the token</param>        
        /// </summary>
        public bool ImportToken(string contractAddress, string symbol, int delimiter, decimal supply, int network);
        /// <summary>
        /// Attepmts to decrypt and unlock a web3 wallet using a password
        /// Parameters:
        /// <param name="pass">Password used to encrypt the wallet.</param>        
        /// </summary>
        public Account UnlockWallet(string pass);
        /// <summary>
        /// Gets a list of supported tokens filtered by network id. Doesn't include market cap, price or supply
        /// Parameters:
        /// <param name="networkId">Blockchain endpoint, RPC address of the Blockchain network </param>        
        /// </summary>
        public Task<List<Token>> GetSupportedTokens(int networkId);
        /// <summary>
        /// Gets a list of supported tokens filtered by network id. Includes market cap price and supply.
        /// Parameters:
        /// <param name="networkId">Blockchain endpoint, RPC address of the Blockchain network </param>        
        /// </summary>
        public Task<List<Token>> GetTokenDetails(int networkId);
    }
}
