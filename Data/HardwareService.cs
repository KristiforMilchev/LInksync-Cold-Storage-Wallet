
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
using RJCP.IO.Ports;
using ArduinoUploader.Hardware;
using ArduinoUploader;

namespace NFTLock.Data
{
    internal class HardwareService
{

  


        public string DeviceConnected()
        {
            string[] portNames = SerialPortStream.GetPortNames();

         //   string[] ports = SerialPort.GetPortNames();

            Debug.WriteLine("The following serial ports were found:");
            var current = string.Empty;
            // Display each port name to the console.
            foreach (string port in portNames)
            {
                Debug.WriteLine(port);
                MauiProgram.ComPort = port;
                current = port;
            }
            return current;
        }

        public List<ArduinoModel> GetSupportedDevices()
        {
            return new List<ArduinoModel> { ArduinoModel.Leonardo, ArduinoModel.Mega1284, ArduinoModel.Mega2560, ArduinoModel.Micro, ArduinoModel.NanoR2, ArduinoModel.NanoR3, ArduinoModel.UnoR3 };
        }

        public async Task<bool> CreateNewDevice(string port)
        {
            string userName = Environment.UserName;
            
           
            if (!File.Exists(@$"{Utilities.GetOsDatFolder()}\wallet.ino.standard.hex"))
            {
                using (WebClient wc = new WebClient())
                {

                        wc.DownloadFile(
                        // Param1 = Link of file
                        new System.Uri("https://raw.githubusercontent.com/KristiforMilchev/LInksync-Cold-Storage-Wallet/main/HardwareCode/ColdStorage/wallet.ino.standard.hex"),
                        // Param2 = Path to save
                        @$"{Utilities.GetOsDatFolder()}\wallet.ino.standard.hex"
                    );
                }
            }


            ConfigureHardware(MauiProgram.DeviceType, @$"{Utilities.GetOsDatFolder()}\wallet.ino.standard.hex", port);

            Debug.WriteLine("Device Updated");
            return true;

        }

        private void ConfigureHardware(ArduinoModel device,string path, string port)
        {
            var uploader = new ArduinoSketchUploader(
                  new ArduinoSketchUploaderOptions()
                  {
                      FileName = path,
                      PortName = port,
                      ArduinoModel = device
                  });

            uploader.UploadSketch();

        }
   

        public CryptoWallet ImportAccount(List<Word> words)
        {
            var Pk = string.Empty;
            var i = 0;

            words.ForEach(x =>
            {
                Pk += Utilities.BinaryToString(x.Name);
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
                string yourByteString = Utilities.StringToBinary(x);
 

 
                words.Add(new Word
                {
                    Name = yourByteString,
                    Index = i,
                });
                i++; 
               
            });
            return words;
        }

        public string GeneratePrivateKey(List<string> data)
        {
            byte[] bytes = new byte[data.Count+1];
            var i = 0;
            data.ForEach(x =>
            {
                var cByte = Convert.ToByte(x, 2);
                bytes[i] = cByte;
                i++;
            });

            var pk = bytes.ToHex();
            return pk;
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
