using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Models
{
    public class NativePrice
    {
        public string value { get; set; }
        public int decimals { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    public class MoralisToken
    {
        public NativePrice nativePrice { get; set; }
        public decimal usdPrice { get; set; }
        public string exchangeAddress { get; set; }
        public string exchangeName { get; set; }
    }

}
