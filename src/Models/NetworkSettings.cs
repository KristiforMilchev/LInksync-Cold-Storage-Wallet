using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Models
{
    public class NetworkSettings
    {
        public bool IsProduction { get; set; }
        public int Id { get; set; }
        public int Chainid { get; set; }
        public string Endpoint { get; set; }
        public string WS { get; set; }
        public string Factory { get; set; }
        public string Name { get; set; }
        public string TokenSylmbol { get; set; }
        public string CurrencyAddress { get; set; }
        public string PairCurrency { get; set; }
        public string Logo { get; set; }

        
    }
}
