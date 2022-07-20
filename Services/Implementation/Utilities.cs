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

    }
}
