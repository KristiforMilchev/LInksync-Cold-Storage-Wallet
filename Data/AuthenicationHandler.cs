 using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using NFTLock.Models;
using SYNCWallet;
using SYNCWallet.Models;

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

    public async Task<TokenContract> GetUserTokenBalance(int network, TokenContract current, bool isCoin)
    {
        var provider = "https://data-seed-prebsc-1-s1.binance.org:8545/";

        var abi = MauiProgram.ContractABI;

        try
        {
            ContractService contractService = new ContractService(provider, current.ContractAddress, abi, MauiProgram.PublicAddress);
            var result = default(decimal);
            if (isCoin)
                result = await contractService.GetAccountBalance(network);
            else
                result = await contractService.CheckUserBalanceForContract(MauiProgram.PublicAddress);
       
            current.UserBalance = result;
            return current;
        }
        catch (Exception e)
        {

            Console.WriteLine(e);
            return null;
        }

    }
}


 