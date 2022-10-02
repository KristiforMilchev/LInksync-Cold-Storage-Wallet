using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace SYNCWallet.Models
{
    [Function("mint", "string")]
    public class TransferFunction : FunctionMessage
    {
        [Parameter("uint256", "_mintAmount", 2)]
        public BigInteger _mintAmount { get; set; }
    }
}
