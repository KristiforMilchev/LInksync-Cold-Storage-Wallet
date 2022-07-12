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
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();

            var account = new Account(privateKey, 97);

            return new CryptoWallet
            {
                Address = account.Address,
                Card = "",
                Name = "Account 1",
                PrivateKey = privateKey
            };
        }
    }
}
