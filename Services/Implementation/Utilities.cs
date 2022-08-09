using Newtonsoft.Json;
using SYNCWallet.Models;
using System.Diagnostics;
using System.Text;

namespace SYNCWallet.Services.Implementation
{
    internal class Utilities
    {

        public async Task<List<NetworkSettings>> SetupNetworks()
        {

            if (!File.Exists($"{GetOsSavePath()}/LocalNetworks.json"))
                return await GetRequest<List<NetworkSettings>>(@"https://raw.githubusercontent.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/main/NetworkSettings.json");
            else
            {
                var whiteListedNetworks = await GetRequest<List<NetworkSettings>>(@"https://raw.githubusercontent.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/main/NetworkSettings.json");

                var filesContent = File.ReadAllText($"{GetOsSavePath()}/LocalNetworks.json");
                var convertedNetworkList = JsonConvert.DeserializeObject<List<NetworkSettings>>(filesContent);
                if(convertedNetworkList != null)
                    whiteListedNetworks.AddRange(convertedNetworkList);
                return whiteListedNetworks;
            }
       
        }

        public static async Task<T> GetRequest<T>(string url)
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

        public static IEnumerable<string> Split(string str, int chunkSize)
        {
            for (int index = 0; index < str.Length; index += chunkSize)
            {
                yield return str.Substring(index, Math.Min(chunkSize, str.Length - index));
            }
        }

        public static string BinaryToString(string data)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }

        public static string StringToBinary(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        public static decimal SetDecimalPoint(decimal value, int delimeter)
        {

            var result = value * (decimal)Math.Pow(10, delimeter);
            return result;
        }

        public static decimal ConvertToDex(decimal value, int delimeter)
        {

            var result = value / (decimal)Math.Pow(10, delimeter);
            return result;
        }

        public static int GetSystemOs()
        {

            if (SYNCWallet.Models.OperatingSystem.IsWindows())
                return 1;
            if (SYNCWallet.Models.OperatingSystem.IsLinux())
                return 2;
            if (SYNCWallet.Models.OperatingSystem.IsMacOS())
                return 3;

            return 0;
        }


        public static string GetOsSavePath()
        {
            var result = string.Empty;
            string userName = Environment.UserName;
            switch (MauiProgram.Os)
            {
                case 1:
                    result = $@"C:\Users\{userName}\Documents";
                    break;

                case 2:
                    result = $@"home/{userName}";
                    break;

                case 3:
                    result = $@"home/{userName}";
                    break;
            }

            return result;
        }


        public static void OpenErrorView(string msg, int attempts)
        {
            Application.Current.Dispatcher.Dispatch(() =>
            {
                Application.Current.MainPage.DisplayAlert("Authenication Error!", msg, "OK");
            });

        }
    }
}
