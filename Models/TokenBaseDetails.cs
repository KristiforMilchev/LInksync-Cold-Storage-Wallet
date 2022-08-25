using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace SYNCWallet.Models
{


    public class TokenBaseDetails { 
    
        public string Address { get; set; }
        public int Decimals { get; set; }
        public string Symbol { get; set; }
        public decimal Supply { get; set; }
    }
}
