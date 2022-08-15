using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace SYNCWallet.Models
{

    [Function("totalSupply", "uint256")]
    public class GetTokenSupply : FunctionMessage
    {

    }
}
