using System;
using System.Diagnostics;

namespace LInksync_Cold_Storage_Wallet
{
    public class Initializer
    {
        public static IServiceProvider Provider {get; set;}

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
       
       



    }
}
