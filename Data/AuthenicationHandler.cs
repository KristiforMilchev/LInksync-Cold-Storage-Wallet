 using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using NFTLock.Models;
using SYNCWallet;
using SYNCWallet.Models;
using SYNCWallet.Services.Implementation;
using System.Diagnostics;
using System.Text;
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

    public bool SetupNetwork(string networkName, string networkSymbol, string rpcUrl, int chainID, string blockExplorer)
    {
        if (!File.Exists($"{Utilities.GetOsDatFolder()}/LocalNetworks.json"))
            File.WriteAllText($"{Utilities.GetOsDatFolder()}/LocalNetworks.json", "");

        var filesContent = File.ReadAllText($"{Utilities.GetOsDatFolder()}/LocalNetworks.json");

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

            File.WriteAllText($"{Utilities.GetOsDatFolder()}/LocalNetworks.json", JsonConvert.SerializeObject(convertedNetworkList));
            return true;
        }

        return false;
    }

    public bool ImportToken(string contractAddress, string symbol, int delimiter, int network)
    {
        if (!File.Exists($"{Utilities.GetOsDatFolder()}/LocalTokens.json"))
            File.WriteAllText($"{Utilities.GetOsDatFolder()}/LocalTokens.json", "");

        var filesContent = File.ReadAllText($"{Utilities.GetOsDatFolder()}/LocalTokens.json");

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
            File.WriteAllText($"{Utilities.GetOsDatFolder()}/LocalTokens.json", JsonConvert.SerializeObject(tokenList));
            return true;
        }
    }


    internal Account  UnlockWallet(string pass)
    {
        var hs = new HardwareService();
        var privateKey = hs.DecryptAesEncoded(MauiProgram.PK, pass);

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

    private string ToHex(byte[] value, bool prefix = false)
    {
        return System.Text.Encoding.UTF8.GetString(value);
    }

    static byte ConvertBinaryToText(string seq)
    {
        return Convert.ToByte(seq, 2);

    }
}   


 