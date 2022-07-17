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
        public decimal Decimals { get; set; }
        public string ContractAddress { get; set; }
        public decimal UserBalance { get; set; }
        public decimal Price { get; set; }
    }
}
