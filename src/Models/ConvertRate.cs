using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace SYNCWallet.Models
{
    [Function("getAmountsOut", "uint256[]")]
    public class ConvertRate : FunctionMessage
    {
        [Parameter("uint256", "amountIn", 1)]
        public BigInteger TokensToSell { get; set; }

        [Parameter("address[]", "path", 2)]
        public string[] Addresses { get; set; }
    }
}
