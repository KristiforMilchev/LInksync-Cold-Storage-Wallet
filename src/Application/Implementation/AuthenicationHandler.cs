using LInksync_Cold_Storage_Wallet.Services.Implementation;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using SYNCWallet;
using SYNCWallet.Models;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace NFTLock.Data;

public class AuthenicationHandler : IAuthenicationService
{
    public IUtilities Utilities { get; set; }
    public IHardwareService HardwareService { get; set; }
    public string PK { get; set; }

    
    public AuthenicationHandler(IUtilities utilities, IHardwareService hardwareService)
    {
        Utilities = utilities;
        HardwareService = hardwareService;
         
    }

 


    public bool SetupNetwork(string networkName, string networkSymbol, string rpcUrl, int chainID, string blockExplorer)
    {
        if (!File.Exists($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalNetworks.json"))
            File.WriteAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalNetworks.json", "");

        if (string.IsNullOrEmpty(networkName))
            return false;
        if (string.IsNullOrEmpty(networkSymbol))
            return false;
        if (string.IsNullOrEmpty(rpcUrl))
            return false;
        if (chainID == 0)
            return false;
        
        var filesContent = File.ReadAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalNetworks.json");

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

            File.WriteAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalNetworks.json", JsonConvert.SerializeObject(convertedNetworkList));
            return true;
        }

        return false;
    }

    public bool ImportToken(string contractAddress, string symbol, int delimiter, decimal supply, int network)
    {
        if (!File.Exists($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json"))
            File.WriteAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json", "");

        var filesContent = File.ReadAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json");

        var tokenList = JsonConvert.DeserializeObject<List<Token>>(filesContent);
        
        //Create the collection in case it's empty
        if(tokenList == null)
            tokenList = new List<Token>();

        if (string.IsNullOrEmpty(symbol))
            return false;

        if (delimiter == 0)
            return false;
        
        
        
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
                        Network = network,
                        Supply = supply,
                    }
                }
            });
            File.WriteAllText($"{Utilities.GetOsSavePath(HardwareService.Os)}/LocalTokens.json", JsonConvert.SerializeObject(tokenList));
            return true;
        }
    }


    public Account  UnlockWallet(string pass, int chainId)
    {
         var privateKey = HardwareService.DecryptAesEncoded(PK, pass);

        if (string.IsNullOrEmpty(privateKey))
            return null;

       
        
        var wallet = new Account(privateKey, chainId);

 
        return wallet;
    }
    
}   


 