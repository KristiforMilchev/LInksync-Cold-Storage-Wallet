using SYNCWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Services.Definitions
{
    public interface IPaymentService
    {
        public Task<TransactionResult> BeginTransaction();
        public Task<bool> ValidateTransaction(string txHash);


    }
}
