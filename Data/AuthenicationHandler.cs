 using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using NFTLock.Models;
using SYNCWallet;
using SYNCWallet.Models;
using SYNCWallet.Services.Implementation;
using static SYNCWallet.Models.GithubTokensModel;

namespace NFTLock.Data;

public class AuthenicationHandler
{
    ContractService _contractService { get; set; }
  
    public string CreateAccountInitial()
	{
        //WIP
        return string.Empty;
    }

    public async Task<string> DeployContract()
    {
        //WIP
        return string.Empty;
    }

    public string GetDefault()
    {
        //WIP
        return MauiProgram.PublicAddress;
    }


    

    internal Account  UnlockWallet(string pass)
    {
        var hs = new HardwareService();
        var privateKey = hs.DecryptAesEncoded(MauiProgram.PK, pass);
        var wallet = new Account(privateKey, 97);
        return wallet;
    }

    public async Task<List<Token>> GetSupportedTokens(int networkId)
    {
         return await ContractService.GetNetworkTokens(networkId);
        
    }

   
}   


 