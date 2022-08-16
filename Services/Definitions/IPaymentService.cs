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
        /// <summary>
        /// Initializes a transaction on the blockchain. Returns a Transaction object verified. Waits for the confirmation
        /// <return>TransactionResult</return>
        /// </summary>
        public Task<TransactionResult> BeginTransaction();

        /// <summary>
        /// Using a transaction hash, it checks the condition of the transcation based on the selected network from the UI returns true only if confirmed and successful
        /// <return>Bool</return>
        /// </summary>
        public Task<bool> ValidateTransaction(string txHash);
    }
}
