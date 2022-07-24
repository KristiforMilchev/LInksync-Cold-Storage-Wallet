using Newtonsoft.Json;
using SYNCWallet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Services.Implementation
{
    internal class Utilities
    {

        public async Task<List<NetworkSettings>> SetupNetworks()
        {
            return await GetRequest<List<NetworkSettings>>(@"https://raw.githubusercontent.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/main/NetworkSettings.json");
        }

        private static async Task<T> GetRequest<T>(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "request");
                HttpResponseMessage response = await client.GetAsync(url);

                // response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var listedTokenData = JsonConvert.DeserializeObject<T>(responseBody);

                return listedTokenData;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return default(T);
            }
           
        }

        /// <summary>
        /// Truncates a price to the 14th decimal to make it more user friendly
        /// Example 0.00000000275465465654 ->  0.0000000027
        /// Parameters:
        /// <param name="price">The input amount to be truncated, returns the same value if it's less then 14 characters.</param>
        /// </summary>
        public static string TruncateDecimals(decimal price)
        {
            var data = price.ToString();

            //Return the same value in case it's less than expected.
            if (data.Length < 14)
                return data; 

            return data.Substring(0, 14); //Truncate to the 14th character
        }
    }
}
