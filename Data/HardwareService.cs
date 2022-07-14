using ArduinoUploader.Hardware;
using ArduinoUploader;
using SYNCWallet;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NFTLock.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3.Accounts;
using System.Security.Cryptography;
using SYNCWallet.Models;

namespace NFTLock.Data
{
    internal class HardwareService
    {

        public void Lock()
        {
            MauiProgram.WriteState("1");
        }

        public void Unlock()
        {
            MauiProgram.WriteState("0");
        }

        public string DeviceConnected()
        {
            string[] ports = SerialPort.GetPortNames();

            Debug.WriteLine("The following serial ports were found:");
            var current = string.Empty;
            // Display each port name to the console.
            foreach (string port in ports)
            {
                Debug.WriteLine(port);
                MauiProgram.ComPort = port;
                current = port;
            }
            return current;
        }

        public void CreateNewDevice()
        {
            string[] hexFileContents;
            hexFileContents = File.ReadAllLines(@"C:\Users\krisk\source\repos\SYNCWallet\SYNCWallet\HardwareCode\wallet\wallet.ino");

            try
            {

                
               var uploader = new ArduinoSketchUploader(
               new ArduinoSketchUploaderOptions()
               {
                   FileName = @"C:\Users\krisk\source\repos\SYNCWallet\SYNCWallet\HardwareCode\ColdStorage\wallet.ino.hex",
                   PortName = "COM3",
                   ArduinoModel = ArduinoModel.UnoR3
               });

                uploader.UploadSketch();
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.ToString());
            }
           
        }

        public CryptoWallet CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var recovered = ecKey.GetPrivateKeyAsBytes();
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            var privateKey2 = recovered.ToHex();

            var account = new Account(privateKey, 97);

            return new CryptoWallet
            {
                Address = account.Address,
                Card = "",
                Name = "Account 1",
                PrivateKey = privateKey,
                Words = GenerateWords(recovered)
            };
        }


        public List<Word> GenerateWords(byte[] bytes)
        {
            var seeePhrase = new SeedPhase();
            var i = 0;
            List<Word> words = new List<Word>();
            bytes.ToList().ForEach(x =>
            {
               var word = seeePhrase.Words.FirstOrDefault(y => y.Id == x);
                if(word != null)
                {
                    word.Index = i;
                    words.Add(word);
                }
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


        private string DecryptAesEncoded(string text)
        {
            string textToDecrypt = text;
            string ToReturn = "";
            string publickey = "12345678";
            string secretkey = "87654321";
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
    }
}
