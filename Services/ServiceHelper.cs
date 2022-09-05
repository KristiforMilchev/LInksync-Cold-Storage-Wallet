using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LInksync_Cold_Storage_Wallet;

namespace SYNCWallet.Services
{

    public static class ServiceHelper
    {
        /// <summary>
        /// Returns a DI TService based on the reference model
        /// IService service = SerbiceHelper.GetService<T>
        /// Parameters:
        /// <param name="TService">The type of service to retrive</param>
        /// </summary>
        public static TService GetService<TService>()
            => Current.GetRequiredService<TService>();

        public static IServiceProvider Current => Initializer.Wallet;
      
    }
}
