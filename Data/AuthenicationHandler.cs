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
        return string.Empty;
    }


    public async Task<int> CheckUserBalanceForContract(string address)
    {
        var result = 0;
        var path = AppDomain.CurrentDomain.BaseDirectory;

        if (!File.Exists($"{path}\\Settings.json"))
            return 0;


        var applicationSettings = JsonConvert.DeserializeObject<ApplicationSettings>(File.ReadAllText($"{path}\\Settings.json"));
        var account = applicationSettings.Wallets.FirstOrDefault(x=>x.Address == address);

        if (account == null)
            return 0;

        var provider = "https://data-seed-prebsc-1-s1.binance.org:8545/";
        var contractAddress = "0x92d5E3A2F20C5191E38eC8D950bb9591602753DB";
        var privateKey = account.PrivateKey;
        var abi = MauiProgram.ContractABI;

        try
        {
            ContractService contractService = new ContractService(provider, contractAddress, abi, privateKey);
            result = await contractService.CheckUserBalanceForContract(address);
        }
        catch (Exception e)
        {

            Console.WriteLine(e);
        }


        return result;
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


 