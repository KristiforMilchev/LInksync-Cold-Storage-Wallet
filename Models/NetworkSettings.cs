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

    }
}
