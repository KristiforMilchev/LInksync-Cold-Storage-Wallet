using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            => Current.GetService<TService>();

        public static IServiceProvider Current => MauiProgram.MauiApp.Services;
      
    }
}
