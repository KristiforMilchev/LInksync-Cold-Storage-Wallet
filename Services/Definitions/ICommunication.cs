using ArduinoUploader.Hardware;
using Microsoft.AspNetCore.Components;
using Nethereum.Web3.Accounts;
using NFTLock.Data;
using SYNCWallet.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SYNCWallet.Models.Enums;
using static SYNCWallet.Models.GithubTokensModel;


namespace SYNCWallet.Services.Definitions
{
    internal interface ICommunication
    {
        public  ArduinoModel DeviceType { get; set; }
        public List<ListedToken> ListedTokens { get; set; }
        public int RemainingAttempts { get; set; }
        public int Os { get; set; }
        public string DefaultPath { get; set; }
        public string ContractABI { get; set; }
        public SerialPort _serialPort { get; set; }
        public string ComPort { get; set; }
        public bool ConfigResponse { get; set; }
        public bool IsConfigured { get; set; }
        public bool RecordPK { get; set; }
        public string PK { get; set; }
        public string Pass { get; set; }
        public bool IsLogged { get; set; }
        public string PublicAddress { get; set; }
        public List<NetworkSettings> NetworkSettings { get; set; }
        public bool IsDevelopment { get; set; }
        public NetworkSettings ActiveNetwork { get; set; }
        public bool KeepPrivateSingle { get; set; }
        public string ReceiverAddress { get; set; }
        public decimal Amount { get; set; }
        public TokenContract SelectedContract { get; set; }
        public string TxHash { get; set; }
        public string HideTokenList { get; set; }
        public string HideTokenSend { get; set; }
        public string ShowPinPanel { get; set; }
        public string ShowLoader { get; set; }
        public string Receipt { get; set; }
        public LoginCallback LoginAttempt { get; set; }
        public TriggerLoader TriggerLoader { get; set; }
        public ErrorCallback ErrorCallback {get; set;}

        public ConfigMode SoftwareType { get; set; }


        public void Init();
        public void StartSerial();
        public bool CheckConfigured(ConfigMode configMode);
        public void ReadSerial();
        public void ProcessData();
        void SetPublic();
        void ExecuteCmd(string currentCMD);
        public void WriteState(string value);
        public void ClearCredentials();
        public void WriteInternalStorage(HardwareWallet hardwareWallet);
        public void ReadInternalStorage(string password);
        public void PublishError(string title, string message);

    }
}
