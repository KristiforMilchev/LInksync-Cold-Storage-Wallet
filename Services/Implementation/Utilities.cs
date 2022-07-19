﻿using Newtonsoft.Json;
using SYNCWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Services.Implementation
{
    internal class Utilities
    {

        public List<NetworkSettings> SetupNetworks()
        {

            var currentToken = await GetRequest<Token>($"https://raw.githubusercontent.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/main/Models/Tokens/{token.name}/token.json");

            return new List<NetworkSettings> { 
            
            };
           
        }


        private static async Task<T> GetRequest<T>(string url)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "request");
            HttpResponseMessage response = await client.GetAsync(url);


            // response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var listedTokenData = JsonConvert.DeserializeObject<T>(responseBody);

            return listedTokenData;
        }
    }
}
