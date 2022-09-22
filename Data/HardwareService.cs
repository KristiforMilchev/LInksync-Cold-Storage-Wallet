
using SYNCWallet;
using System.Text;
using System.Diagnostics;
using NFTLock.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3.Accounts;
using System.Security.Cryptography;
using SYNCWallet.Models;
using SYNCWallet.Services.Implementation;
using System.Net;
using ArduinoUploader.Hardware;
using ArduinoUploader;
using SYNCWallet.Services.Definitions;
using SYNCWallet.Services;
using System.IO.Ports;
using ArduinoUploader.Config;
using Newtonsoft.Json;

namespace NFTLock.Data
{
    internal class HardwareService : IHardwareService
    { 
        IUtilities Utilities = ServiceHelper.GetService<IUtilities>();
        ICommunication Communication = ServiceHelper.GetService<ICommunication>();
     
        public string DeviceConnected()
        {
 
            
            //Get an array of com ports on the system
            string[] portNames = SerialPort.GetPortNames();

            var current = string.Empty;
            // Display each port name to the console.
            //Always assigns the last com port to the system, doesn't work with two devices.
            foreach (string port in portNames)
            {
                Debug.WriteLine(port);
                Communication.ComPort = port;
                current = port;
            }

            return current;
        }

        public List<ArduinoModel> GetSupportedDevices()
        {
            return new List<ArduinoModel> { ArduinoModel.Leonardo, ArduinoModel.Mega1284, ArduinoModel.Mega2560, ArduinoModel.Micro, ArduinoModel.NanoR2, ArduinoModel.NanoR3, ArduinoModel.UnoR3 };
        }

        public bool CreateNewDevice(string port)
        {
            string userName = Environment.UserName;
            
            //If file exists, delete it, we want the latest version of the file.
            if (File.Exists(@$"{Utilities.GetOsSavePath()}\wallet.ino.standard.hex"))
                File.Delete(@$"{Utilities.GetOsSavePath()}\wallet.ino.standard.hex");


            //Download the latest version of the file.
            using (WebClient wc = new WebClient())
            {

                wc.DownloadFile(
                // Param1 = Link of file
                new System.Uri("https://raw.githubusercontent.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/main/HardwareCode/ColdStorage/wallet.ino.standard.hex"),
                // Param2 = Path to save
                @$"{Utilities.GetOsSavePath()}\wallet.ino.standard.hex");
            }


            //Uploads the firmware to device.
            var result = false;

            //Run the update on a seprate thread
            Task.Run(() =>
            {
                result = ConfigureHardware(Communication.DeviceType, @$"{Utilities.GetOsSavePath()}\wallet.ino.standard.hex", port);
            });

            var timeout = DateTime.UtcNow.AddSeconds(30);

            //Adding a timeout, in case a user selects a device that's not plugged in.
            while(!result)
            {
                if(DateTime.UtcNow > timeout)
                {
                    Utilities.OpenErrorView("Connection timeout",$"Device not connected, please make sure you have connected device of type {Communication.DeviceType}",0);
                    return false;

                }
            }

            Debug.WriteLine("Device Updated");
            return result;
        }

        public bool ConfigureHardware(ArduinoModel device,string path, string port)
        {
            try
            {
                var uploader = new ArduinoSketchUploader(
                 new ArduinoSketchUploaderOptions()
                 {
                     FileName = path,
                     PortName = port,
                     ArduinoModel = device
                 });

                uploader.UploadSketch();
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
           
        }
   

        public CryptoWallet ImportAccount(List<Word> words)
        {
            var Pk = string.Empty;
            var i = 0;

            //Loop over the words and reconstruct the key
            words.ForEach(x =>
            {
                Pk += x.Name;
                i++;
            });

            var account = new Account(Pk, 56);

            return new CryptoWallet
            {
                Address = account.Address,
                Card = "",
                Name = "Account 1",
                PrivateKey = Pk,
                Words = words
            };
        }

        public CryptoWallet CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var recovered = ecKey.GetPrivateKey();
  
            var chunks = Utilities.Split(recovered,5).ToList();
            var account = new Account(recovered, 97);

            return new CryptoWallet
            {
                Address = account.Address,
                Card = "",
                Name = "Account 1",
                PrivateKey = recovered,
                Words = GenerateWords(chunks)
            };
        }


        public List<Word> GenerateWords(List<string> chunks)
        {
            var seeePhrase = new SeedPhase();
            var i = 0;
            List<Word> words = new List<Word>();
            chunks.ToList().ForEach(x =>
            {
                words.Add(new Word
                {
                    Name = x,
                    Index = i,
                });
                i++; 
               
            });
            return words;
        }
 
        public string Encrypt(string data, string password)
        {
            try
            {
                string textToEncrypt = data;
                string ToReturn = "";
                string publickey = password;
                string secretkey = password;
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.ToString());
                return string.Empty;

            }
        }


        public string DecryptAesEncoded(string text, string password)
        {
            try
            {
                string textToDecrypt = text;
                string ToReturn = "";
                string publickey = password;
                string secretkey = password;
                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return String.Empty;
            }
         
        }
    }
}
