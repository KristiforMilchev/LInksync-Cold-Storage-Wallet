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
        public string CreateAccountInitial();
        public Task<string> DeployContract();
        public string GetDefault();
        public bool SetupNetwork(string networkName, string networkSymbol, string rpcUrl, int chainID, string blockExplorer);
        public bool ImportToken(string contractAddress, string symbol, int delimiter, int network);
        public Account UnlockWallet(string pass);
        public Task<List<Token>> GetSupportedTokens(int networkId);
        string ToHex(byte[] value, bool prefix = false);
        byte ConvertBinaryToText(string seq);



    }
}
