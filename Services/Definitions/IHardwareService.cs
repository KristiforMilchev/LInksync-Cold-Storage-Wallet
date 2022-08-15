using ArduinoUploader.Hardware;
using NFTLock.Models;
using SYNCWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYNCWallet.Services.Definitions
{
    public interface IHardwareService
    {
        public string DeviceConnected();
        public List<ArduinoModel> GetSupportedDevices();
        public bool CreateNewDevice(string port);
        public void ConfigureHardware(ArduinoModel device, string path, string port);
        public CryptoWallet ImportAccount(List<Word> words);
        public CryptoWallet CreateAccount();
        public List<Word> GenerateWords(List<string> chunks);
        public string GeneratePrivateKey(List<string> data);
        public string Encrypt(string data, string password);
        public string DecryptAesEncoded(string text, string password);


    }
}
