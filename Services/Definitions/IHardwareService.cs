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
        /// <summary>
        /// Check if the hardware device is connected to the system
        /// </summary>
        public string DeviceConnected();

        /// <summary>
        /// Returns a list of supported arudino models.
        /// </summary>
        public List<ArduinoModel> GetSupportedDevices();

        /// <summary>
        /// Downloads the latest firmware from github, compiles the firmware and updates the devices with the latest changes from the main branch.
        /// Hangs if the wrong device is selected due to crystal bound rate issues.
        /// Parameters:
        /// <param name="port">Serial COM port that has arduino on it.</param>
        /// </summary>
        public bool CreateNewDevice(string port);

        /// <summary>
        /// Installs the latest firmware to an arduino device that matches the selected device.
        /// Parameters:
        /// <param name="device">Model of the device. ArduinoModel</param>
        /// <param name="path">Path to the latest firmware</param>
        /// <param name="port">COM port of the connected Arduino</param>
        /// </summary>
        public bool ConfigureHardware(ArduinoModel device, string path, string port);

        /// <summary>
        /// Imports an account using the private key of the device split in 12 unique chunks.
        /// Parameters:
        /// <param name="words">The list of words to reconstruct the wallet.</param>
        /// </summary>
        /// TODO: in V2, implement a mapping to a book reference so it can create a mnemonic alogirthm to make it more conviniant 
        public CryptoWallet ImportAccount(List<Word> words);


        /// <summary>
        /// Creates a new EVM compatible account, returns an object of type CryptoWallet
        /// </summary>
        public CryptoWallet CreateAccount();


        /// <summary>
        /// Takes in a list of string and converts the list to List of words, used for importing an existing wallet.
        /// Returns a LIst of Words
        /// </summary>
        public List<Word> GenerateWords(List<string> chunks);


        /// <summary>
        /// Encryps a PK of a wallet using a 8 a-z/1-0 password with AES 256 bit algorithm
        /// Parameters:
        /// <param name="data">The data to be encrypted</param>
        /// <param name="password">Password to encrypt the data with, not less or over 8 characters</param>
        /// </summary>
        public string Encrypt(string data, string password);


        /// <summary>
        /// Decrypts a PK of a wallet using a 8 a-z/1-0 password with AES 256 bit algorithm
        /// Parameters:
        /// <param name="text">AES 256 encrypted string</param>
        /// <param name="password">Password to encrypt the data with, not less or over 8 characters</param>
        /// </summary>
        public string DecryptAesEncoded(string text, string password);


    }
}
