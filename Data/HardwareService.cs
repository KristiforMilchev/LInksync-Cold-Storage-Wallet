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
using SYNCWallet.Services.Implementation;

namespace NFTLock.Data
{
internal class HardwareService
{

  


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
                CreateNewDevice(current);
            }
            return current;
        }

        public void CreateNewDevice(string port)
        {
            string[] hexFileContents;
  
            try
            { 
               var uploader = new ArduinoSketchUploader(
               new ArduinoSketchUploaderOptions()
               {
                   FileName = @$"{MauiProgram.DefaultPath}\HardwareCode\ColdStorage\wallet.ino.standard.hex",
                   PortName = port,
                   ArduinoModel = ArduinoModel.UnoR3
               });

               uploader.UploadSketch();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
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
