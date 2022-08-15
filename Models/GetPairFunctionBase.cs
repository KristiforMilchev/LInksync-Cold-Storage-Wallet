using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace SYNCWallet.Models
{

    [Function("getPair", "address")]
    public class GetPairFunctionBase : FunctionMessage
    {
        [Parameter("address", "tokenA", 1)]
        public virtual string TokenA { get; set; }
        [Parameter("address", "tokenB", 2)]
        public virtual string TokenB { get; set; }
    }
}
