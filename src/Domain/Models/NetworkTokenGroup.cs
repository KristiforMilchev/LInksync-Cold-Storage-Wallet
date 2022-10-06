using SYNCWallet.Models;

namespace Domain.Models
{
    public class NetworkTokenGroup
    {
        public List<Token> Tokens { get; set; }
        public NetworkSettings Network { get; set; }

        public NetworkTokenGroup()
        {
            Tokens = new List<Token>();
        }
    }
}