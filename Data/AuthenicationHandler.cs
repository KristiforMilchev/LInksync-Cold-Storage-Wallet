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

    public void SetupNetwork(string networkName, string networkSymbol, string rpcUrl, int chainID, string blockExplorer)
    {
        if (!File.Exists($"{MauiProgram.DefaultPath}/LocalNetworks.json"))
            File.WriteAllText($"{MauiProgram.DefaultPath}/LocalNetworks.json", "");

        var filesContent = File.ReadAllText($"{MauiProgram.DefaultPath}/LocalNetworks.json");

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

            File.WriteAllText($"{MauiProgram.DefaultPath}/LocalNetworks.json", JsonConvert.SerializeObject(convertedNetworkList));
        }
    }

    public void ImportToken(string contractAddress, string symbol, int delimiter, int network)
    {
        if (!File.Exists($"{MauiProgram.DefaultPath}/LocalTokens.json"))
            File.WriteAllText($"{MauiProgram.DefaultPath}/LocalTokens.json", "");

        var filesContent = File.ReadAllText($"{MauiProgram.DefaultPath}/LocalTokens.json");

        var tokenList = JsonConvert.DeserializeObject<List<Token>>(filesContent);
        
        //Create the collection in case it's empty
        if(tokenList == null)
            tokenList = new List<Token>();
        
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
        File.WriteAllText($"{MauiProgram.DefaultPath}/LocalTokens.json", JsonConvert.SerializeObject(tokenList));
    }


    internal Account  UnlockWallet(string pass)
    {
        var hs = new HardwareService();
        var privateKey = hs.DecryptAesEncoded(MauiProgram.PK, pass);
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


 