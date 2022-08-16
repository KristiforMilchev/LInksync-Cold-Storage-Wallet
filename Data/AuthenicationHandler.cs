using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using SYNCWallet;
using SYNCWallet.Models;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace NFTLock.Data;

public class AuthenicationHandler : IAuthenicationService
{
    ContractService _contractService { get; set; }
    IUtilities Utilities { get; set; }
    IContractService ContractService { get; set; }
    IHardwareService HardwareService { get; set; }
    public AuthenicationHandler()
    {
        Utilities = ServiceHelper.GetService<IUtilities>();
        ContractService = ServiceHelper.GetService<IContractService>();
        HardwareService = ServiceHelper.GetService<IHardwareService>();
    }

 
    public string GetDefault()
    {
        //WIP
        return MauiProgram.PublicAddress;
    }

    public bool SetupNetwork(string networkName, string networkSymbol, string rpcUrl, int chainID, string blockExplorer)
    {
        if (!File.Exists($"{Utilities.GetOsSavePath()}/LocalNetworks.json"))
            File.WriteAllText($"{Utilities.GetOsSavePath()}/LocalNetworks.json", "");

        var filesContent = File.ReadAllText($"{Utilities.GetOsSavePath()}/LocalNetworks.json");

        var convertedNetworkList = JsonConvert.DeserializeObject<List<NetworkSettings>>(filesContent);


        //Create the collection in case it's empty
        if (convertedNetworkList == null)
            convertedNetworkList = new List<NetworkSettings>();

        if (!convertedNetworkList.Any(x=>x.Chainid == chainID))
        {
            convertedNetworkList.Add(new NetworkSettings
            {
                Id = 22 + convertedNetworkList.Count + 1,
                Chainid = chainID,
                IsProduction = true,
                Endpoint = rpcUrl,
                Name = networkName,
                TokenSylmbol = networkSymbol
            });

            File.WriteAllText($"{Utilities.GetOsSavePath()}/LocalNetworks.json", JsonConvert.SerializeObject(convertedNetworkList));
            return true;
        }

        return false;
    }

    public bool ImportToken(string contractAddress, string symbol, int delimiter, int network)
    {
        if (!File.Exists($"{Utilities.GetOsSavePath()}/LocalTokens.json"))
            File.WriteAllText($"{Utilities.GetOsSavePath()}/LocalTokens.json", "");

        var filesContent = File.ReadAllText($"{Utilities.GetOsSavePath()}/LocalTokens.json");

        var tokenList = JsonConvert.DeserializeObject<List<Token>>(filesContent);
        
        //Create the collection in case it's empty
        if(tokenList == null)
            tokenList = new List<Token>();
        
        if(tokenList.Any(x=>x.Symbol == symbol && x.Name == x.Name))
            return false;
        else
        {
            tokenList.Add(new Token
            {
                Symbol = symbol,
                Name = symbol,
                IsChainCoin = false,
                Contracts = new List<TokenContract>
            {
                new TokenContract
                {
                    Decimals = delimiter,
                    ContractAddress = contractAddress,
                    Network = network
                }
            }
            });
            File.WriteAllText($"{Utilities.GetOsSavePath()}/LocalTokens.json", JsonConvert.SerializeObject(tokenList));
            return true;
        }
    }


    public Account  UnlockWallet(string pass)
    {
         var privateKey = HardwareService.DecryptAesEncoded(MauiProgram.PK, pass);

        if (string.IsNullOrEmpty(privateKey))
            return null;

        var chainId = 97;
        if (MauiProgram.ActiveNetwork != null)
            chainId = MauiProgram.ActiveNetwork.Chainid;
        
        
        var wallet = new Account(privateKey, chainId);

 
        return wallet;
    }

    public async Task<List<Token>> GetSupportedTokens(int networkId)
    {
         return await ContractService.GetNetworkTokens(networkId);
        
    }
}   


 