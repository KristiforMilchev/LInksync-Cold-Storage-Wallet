using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace NFTLock.Models
{
    [Function("balanceOf", "uint256")]
    internal class BalanceOf: FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public string Owner { get; set; }

    }
}
