﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Models
{
    public class TokenContract
    {
        public int Network { get; set; }
        public decimal Decimals { get; set; }
        public string ContractAddress { get; set; }
    }
}