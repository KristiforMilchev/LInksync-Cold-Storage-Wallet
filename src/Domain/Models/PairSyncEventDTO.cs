using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace SYNCWallet.Models
{

    [Event("Sync")]
    class PairSyncEventDTO : IEventDTO
    {
        [Parameter("uint112", "reserve0")]
        public virtual BigInteger Reserve0 { get; set; }

        [Parameter("uint112", "reserve1", 2)]
        public virtual BigInteger Reserve1 { get; set; }
    }

}
