using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Models
{
    public class TokenContract
    {
        public int Network { get; set; }
        public int Decimals { get; set; }
        public string ContractAddress { get; set; }
        public decimal UserBalance { get; set; }
        public decimal Price { get; set; }
        public decimal Supply { get; set; }
        public decimal CirculatingSupply { get; set; }
        public decimal MarketCap { get; set; }
        public decimal CurrentPrice { get; set; }
        public string MainLiquidityPool { get; set; }
        public string PairTokenAddress { get; set; }
        public string ListedExchangeRouter { get; set; }
        public string PairName { get; set; }
    }
}
