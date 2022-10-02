using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Models
{
    public class Token
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public bool IsChainCoin { get; set; }
        public List<TokenContract> Contracts { get; set; }
        public List<TokenLink> Links { get; set; }
    }
}
