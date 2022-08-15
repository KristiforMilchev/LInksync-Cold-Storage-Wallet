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
        public Task<decimal> GetAccountBalance(int network, string endpoint);
        public Task<decimal> ConvertTokenToUsd(decimal tokens, string[] addresses, string endpoint, string router);
        public Task<decimal> CheckUserBalanceForContract(string ownerAddress, string contract, string endpoint, int decimals);
        //TODO implement in V2
        //public Task<string> MintToken(); 
        public Task<List<SYNCWallet.Models.Token>> GetNetworkTokens(int networkId);
        public Task<bool> ExecutePayments(string receiver, TokenContract token, decimal amountToSend, Nethereum.Web3.Accounts.Account account, string endpoint, int chainId);
        public Task<bool> ExecuteNative(string receiver, decimal amountToSend, Nethereum.Web3.Accounts.Account account, string endpoint, int chainId);
        public Task<decimal> CheckContractPrice(string contractAddress, string token, string pair, int tokenDecimals, int pairDecimals, string endpoint);



        public Task<decimal> GetImportedData(NetworkSettings network, TokenContract getContract);
        public Task<List<Token>> GetListedTokens(List<ListedToken> listedTokenData, List<Token> tokens, NetworkSettings network);
        public Task<(decimal, decimal)> GetContractMarketCap(decimal supply, decimal getTokenPrice, string contractAddress, string endpoint, int decimals);
        public Task<string> CheckExchangelisting(string contractAddress, string pairToken, string endpoint, string factory);
        public Task<decimal> CheckExistingSupply(string contractAddress, string endpoint, int tokenDecimals);



    }
}
