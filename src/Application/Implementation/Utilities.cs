using Newtonsoft.Json;
using SYNCWallet.Models;
using SYNCWallet.Services.Definitions;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
 
namespace SYNCWallet.Services.Implementation
{
    public class Utilities : IUtilities
    {
       
        public async Task<List<NetworkSettings>> SetupNetworks(int os)
        {

            if (!File.Exists($"{GetOsSavePath(os)}/LocalNetworks.json"))
                return await GetRequest<List<NetworkSettings>>(@"https://raw.githubusercontent.com/KristiforMilchev/LinkSync-Whitelistings/main/NetworkSettings.json");
            else
            {
                var whiteListedNetworks = await GetRequest<List<NetworkSettings>>(@"https://raw.githubusercontent.com/KristiforMilchev/LinkSync-Whitelistings/main/NetworkSettings.json");

                var filesContent = File.ReadAllText($"{GetOsSavePath(os)}/LocalNetworks.json");
                var convertedNetworkList = JsonConvert.DeserializeObject<List<NetworkSettings>>(filesContent);
                if(convertedNetworkList != null)
                    whiteListedNetworks.AddRange(convertedNetworkList);
                return whiteListedNetworks;
            }
        }

        
        public async Task<T> GetRequest<T>(string url)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                client.DefaultRequestHeaders.Add("User-Agent", "request");
                HttpResponseMessage response = await client.GetAsync(url);

                // response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var listedTokenData = JsonConvert.DeserializeObject<T>(responseBody); //Convert to T

                return listedTokenData;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e); //Show error in debug mode.
                return default(T); //Return the defaukt<T>
            }
           
        }

        public IEnumerable<string> Split(string str, int chunkSize)
        {
            for (int index = 0; index < str.Length; index += chunkSize)
            {
                yield return str.Substring(index, Math.Min(chunkSize, str.Length - index));
            }
        }

        public string BinaryToString(string data)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());
        }


        public string StringToBinary(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        public decimal SetDecimalPoint(decimal value, int delimeter)
        {

            var result = value * (decimal)Math.Pow(10, delimeter);
            return result;
        }



        public decimal ConvertToDex(decimal value, int delimeter)
        {

            var result = value / (decimal)Math.Pow(10, delimeter);
            return result;
        }

        public decimal ConvertToBigIntDex(BigInteger value, int delimeter)
        {
            var result = (decimal)value / (decimal)Math.Pow(10, delimeter);
            return result;
        }

        public decimal ConvertToDexDecimal(decimal number, int decimals)
        {
            var num = number / (decimal)Math.Pow(10, decimals);
            return num;
        }


        public int GetSystemOs()
        {

            if (SYNCWallet.Models.OperatingSystem.IsWindows())
                return 1;
            if (SYNCWallet.Models.OperatingSystem.IsLinux())
                return 2;
            if (SYNCWallet.Models.OperatingSystem.IsMacOS())
                return 3;

            return 0;
        }

        public string GetOsSavePath(int os)
        {
            var result = string.Empty;
            string userName = Environment.UserName;
            switch (os)
            {
                case 1:
                    result = $@"C:\Users\{userName}\Documents";
                    break;

                case 2:
                    result = $@"/home/{userName}/Downloads";
                    break;

                case 3:
                    result = $@"/home/{userName}/Downloads";
                    break;
            }

            return result;
        }



        public void OpenErrorView(string title, string msg, int attempts)
        {
            //TODO Extend this to work under Electron
            
            // //Call the event in a dispatcher, would get stuck and block the main thread otherwise leading to Thread collision exception
            // Application.Current.Dispatcher.Dispatch(() =>
            // {
            //     //TODO extend this method to be general exception handler, as well as the option to handle user input and return callbacks.
            //     Application.Current.MainPage.DisplayAlert(title, msg, "OK");
            // });
            
        }




        public string TruncateDecimals(decimal price)
        {
            var data = price.ToString();

            //Return the same value in case it's less than expected.
            if (data.Length < 14)
                return data;

            return data.Substring(0, 14); //Truncate to the 14th character
        }

        public void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
        
        public double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }


    }
}
