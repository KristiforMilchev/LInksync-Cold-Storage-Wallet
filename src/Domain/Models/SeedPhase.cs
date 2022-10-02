using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Models
{
    public class SeedPhase
    {


        public List<Word> Words { get; set; }
 
    }

    public class Word
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Index { get; set; } 
    }
}
