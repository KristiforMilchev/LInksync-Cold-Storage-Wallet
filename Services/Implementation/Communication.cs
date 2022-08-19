
using ArduinoUploader.Hardware;
using Newtonsoft.Json;
using NFTLock.Models;
using SYNCWallet.Models;
using SYNCWallet.Services.Definitions;
using System.Diagnostics;
using System.IO.Ports;
using static SYNCWallet.Models.Enums;
using static SYNCWallet.Models.GithubTokensModel;

namespace SYNCWallet.Services.Implementation
{
    public class Communication : ICommunication
    {
        public ArduinoModel DeviceType { get; set; }
        public List<ListedToken> ListedTokens { get; set; }
        public int RemainingAttempts { get; set; }
        public int Os { get; set; }
        public string DefaultPath { get; set; }
        public string ContractABI { get; set; }
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
        public SerialPort _serialPort { get; set; }
        IUtilities Utilities { get; set; }
        IAuthenicationService AuthenicationService { get; set; }
        public string HideTokenList { get; set; }
        public string HideTokenSend { get; set; }
        public string ShowPinPanel { get; set; }
        public string ShowLoader { get; set; }
        public string Receipt { get; set; }
        public LoginCallback LoginAttempt { get; set; }
        public TriggerLoader TriggerLoader { get; set; }
        public ConfigMode SoftwareType { get; set; }

        public bool CheckConfigured(ConfigMode configMode)
        {
            if (configMode == ConfigMode.ColdWallet)
                return CheckHardware();

            return CheckSoftware();
        }

        bool CheckSoftware()
        {
            var file = $"{Utilities.GetOsSavePath()}/wallet.json";
            if (File.Exists(file))
            {
                var wallet = JsonConvert.DeserializeObject<CryptoWallet>(File.ReadAllText(file));
                if (wallet != null)
                    return true;
            }

            return false;
        }

        public void WriteInternalStorage(HardwareWallet hardwareWallet)
        {
            var file = $"{Utilities.GetOsSavePath()}/wallet.json";
            if (!File.Exists(file))
            {
                File.WriteAllText(file,JsonConvert.SerializeObject(hardwareWallet));
            }
        }

        public void ReadInternalStorage(string password)
        {
            
            var file = $"{Utilities.GetOsSavePath()}/wallet.json";
            if (File.Exists(file))
            {
                var readAll = File.ReadAllText(file);
                var convert = JsonConvert.DeserializeObject<HardwareWallet>(readAll);
                if (password != convert.Password)
                {
                    LoginAttempt?.Invoke(false);
                    RemainingAttempts -= 1;
                    if (RemainingAttempts <= 0)
                    {
                        //Since we don't have a hardware to handle file deletion we delete the file from the software.
                        //Only if the password is wrong 3 times in a row.
                        File.Delete(file);

                        Application.Current.Dispatcher.Dispatch(() =>
                        {
                            Application.Current.Windows.ToList().ForEach(y =>
                            {
                                Application.Current.CloseWindow(y);
                            });
                        });
                    }
                    Utilities.OpenErrorView($"Wrong pin, {RemainingAttempts} attempts remaining", RemainingAttempts);

                    return;
                }

                if (convert == null)
                {
                    //Todo
                    LoginAttempt?.Invoke(false);
                    return;
                 }

                PK = convert.PrivateKey;
                var wallet = AuthenicationService.UnlockWallet(Pass);

                PublicAddress = wallet.Address;

                if (PublicAddress == null)
                {

                    WriteState(JsonConvert.SerializeObject(new HardwareWallet
                    {
                        Cmd = "Login",
                        Password = Pass,
                        PrivateKey = "3"
                    }));
                    IsLogged = false;
                    LoginAttempt?.Invoke(false);

                }
                else
                {
                    Debug.WriteLine(PublicAddress);
                    IsLogged = true;
                    LoginAttempt?.Invoke(true);

                    RemainingAttempts = 3;
                    if (!KeepPrivateSingle)
                        PK = string.Empty;

                }
            }
        }

        bool CheckHardware()
        {
            try
            {
                StartSerial();

                var pingAgain = DateTime.UtcNow.AddSeconds(3);

                WriteState(JsonConvert.SerializeObject(new HardwareWallet
                {
                    Cmd = "CF",
                    Password = "",
                    PrivateKey = ""
                }));

                while (!ConfigResponse)
                {
                    if (DateTime.UtcNow > pingAgain)
                    {
                        WriteState(JsonConvert.SerializeObject(new HardwareWallet
                        {
                            Cmd = "CF",
                            Password = "",
                            PrivateKey = ""
                        }));
                        pingAgain = DateTime.UtcNow.AddSeconds(3);
                    }
                }

                return IsConfigured;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

                return false;
            }
        }


        public void ClearCredentials()
        {
            PK = string.Empty;
            Pass = string.Empty;
        }

        public void ExecuteCmd(string currentCMD)
        {
            var lastCmd = currentCMD.Split(Environment.NewLine).LastOrDefault();
            switch (lastCmd)
            {

                case "#CFS1":
                    IsConfigured = true;
                    ConfigResponse = true;
                    break;
                case "#CFS2":
                    IsConfigured = false;
                    ConfigResponse = true;
                    break;
                case "#SR:":
                    RecordPK = true;
                    break;
                case "#SO:":
                    RecordPK = false;
                    PK = PK.Substring(0, PK.Length - 4);
                    break;
                default:
                    break;
            }
        }

        public void ProcessData()
        {
            try
            {
                #pragma warning disable CA1416 // Validate platform compatibility
                string a = _serialPort.ReadExisting();
                #pragma warning restore CA1416 // Validate platform compatibility
                var test = a.Split(Environment.NewLine).LastOrDefault();
                if (!string.IsNullOrEmpty(a))
                {

                    if (a.Contains("#SR:"))
                    {
                        var getPK = a.Split("#SR:");
                        if (getPK.Length > 1)
                        {
                            var tpm = getPK[1].Replace(Environment.NewLine, "");
                            if (getPK.Length > 2)
                            {
                                tpm += getPK[2].Replace(Environment.NewLine, "");
                            }
                            PK = tpm;
                            IsLogged = true;
                            SetPublic();
                        }

                    }
                    else if (a == "#ERL" || test == "#ERL")
                    {
                        //"Authenication Handler",null
                        LoginAttempt?.Invoke(false);
                        RemainingAttempts -= 1;
                        if (RemainingAttempts <= 0)
                        {
                            Application.Current.Dispatcher.Dispatch(() =>
                            {
                                Application.Current.Windows.ToList().ForEach(y =>
                                {
                                    Application.Current.CloseWindow(y);
                                });
                            });


                        }
                        Utilities.OpenErrorView($"Wrong pin, {RemainingAttempts} attempts remaining", RemainingAttempts);
                    }
                    else
                    {
                        ExecuteCmd(a);
                    }

                    Debug.WriteLine(a);
                }

                Thread.Sleep(200);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void ReadSerial()
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = ComPort;
            _serialPort.BaudRate = 115200;
            _serialPort.Open();

            var currentCMD = string.Empty;
            Task.Run(() =>
            {
                while (true)
                {
                    ProcessData();
                }
            });
        }

        public void SetPublic()
        {
            try
            {
                var wallet = AuthenicationService.UnlockWallet(Pass);

                if (wallet == null)
                    return;

                PublicAddress = wallet.Address;

                if (PublicAddress == null)
                {

                    WriteState(JsonConvert.SerializeObject(new HardwareWallet
                    {
                        Cmd = "Login",
                        Password = Pass,
                        PrivateKey = "3"
                    }));
                    IsLogged = false;
                    LoginAttempt?.Invoke(false);

                }
                else
                {
                    Debug.WriteLine(PublicAddress);
                    LoginAttempt?.Invoke(true);

                    RemainingAttempts = 3;
                    if (!KeepPrivateSingle)
                        PK = string.Empty;

                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void StartSerial()
        {
            if (_serialPort != null && _serialPort.IsOpen)
                _serialPort.Close();

            ReadSerial();
        }

        public void WriteState(string value)
        {
            #pragma warning disable CA1416 // Validate platform compatibility
            _serialPort.WriteLine(value);
            _serialPort.DiscardOutBuffer();
            _serialPort.DiscardInBuffer();
            #pragma warning restore CA1416 // Validate platform compatibility

        }

        public void Init()
        {
            Utilities = ServiceHelper.GetService<IUtilities>();
            AuthenicationService = ServiceHelper.GetService<IAuthenicationService>();

            HideTokenList = "none";
            HideTokenSend = "none";
            ShowPinPanel = "none";
            ShowLoader = "none";
            Receipt = "none";

            IsDevelopment = true;

            Utilities = new Utilities();
 
            Os = Utilities.GetSystemOs();
            RemainingAttempts = 3;


            DefaultPath = AppDomain.CurrentDomain.BaseDirectory;
            ContractABI = "[{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_name\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"_symbol\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"_initBaseURI\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"_initNotRevealedUri\",\"type\":\"string\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"baseExtension\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"cost\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"maxMintAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"maxSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_mintAmount\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"notRevealedUri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bool\",\"name\":\"_state\",\"type\":\"bool\"}],\"name\":\"pause\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"paused\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"reveal\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"revealed\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_newBaseExtension\",\"type\":\"string\"}],\"name\":\"setBaseExtension\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_newBaseURI\",\"type\":\"string\"}],\"name\":\"setBaseURI\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_newCost\",\"type\":\"uint256\"}],\"name\":\"setCost\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_notRevealedURI\",\"type\":\"string\"}],\"name\":\"setNotRevealedURI\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_newmaxMintAmount\",\"type\":\"uint256\"}],\"name\":\"setmaxMintAmount\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_owner\",\"type\":\"address\"}],\"name\":\"walletOfOwner\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"}]";

        }
    }
}
