using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFTLock.Models
{
    internal class ApplicationSettings
    {
        public string PrivateKey { get; set; }
        public string ContractAddress { get; set; }
        public List<string> DeactivatedNfts { get; set; }
        public List<CryptoWallet> Wallets { get; set; }
    }
}
