using System;
using System.Diagnostics;
using Domain.Models;
using Microsoft.JSInterop;

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
       
       
        public static Picker.PickerSelected DateTimePicker { get; set; }
        
   
        [JSInvokable]
        public static void TokenSelected(string token)
        {
            //TODO implement communication interface that routes back the transactions object each time the user changes  a date.
            DateTimePicker?.Invoke(token);
        }


    }
}
