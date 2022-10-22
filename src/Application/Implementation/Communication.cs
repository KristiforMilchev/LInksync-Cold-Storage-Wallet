using System.Diagnostics;
using System.IO.Ports;
using System.Net.Http.Headers;
using Application.Cache;
using ArduinoUploader.Hardware;
using Domain.Models;
using Newtonsoft.Json;
using NFTLock.Data;
using NFTLock.Models;
using SYNCWallet.Models;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;
using SYNCWallet.Services.Implementation;
using static SYNCWallet.Models.Enums;
using static SYNCWallet.Models.GithubTokensModel;

namespace LInksync_Cold_Storage_Wallet.Services.Implementation
{
    public class Communication : ICommunication
    {
        public ArduinoModel DeviceType { get; set; }
        public List<ListedToken> ListedTokens { get; set; }
        public List<Token> LoadedTokens { get; set; }
        public int RemainingAttempts { get; set; }
        public string DefaultPath { get; set; }
        public string ContractABI { get; set; }
        public string ComPort { get; set; }
        public bool ConfigResponse { get; set; }
        public bool IsConfigured { get; set; }
        public bool RecordPK { get; set; }
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
        public  Token SelectedToken { get; set; }
        public string TxHash { get; set; }
        public SerialPort _serialPort { get; set; }
        public IUtilities Utilities { get; set; }
        public IAuthenicationService AuthenicationService { get; set; }
        public string HideTokenList { get; set; }
        public string HideTokenSend { get; set; }
        public string ShowPinPanel { get; set; }
        public string ShowLoader { get; set; }
        public string Receipt { get; set; }
        public LoginCallback LoginAttempt { get; set; }
        public TriggerLoader TriggerLoader { get; set; }
        public ErrorCallback ErrorCallback {get; set;}
        public WidgetLoadedGeneric<List<UserAssetBalance>> ChartDataLoaded { get; set; }
        public ConfigMode SoftwareType { get; set; }
        public string DataApiEndpoint { get; set; }

        public Communication(IAuthenicationService authenicationService, IUtilities utilities)
        {
            Utilities = utilities;
            AuthenicationService = authenicationService;
        }
        
        
        public string GetDefault()
        {
            return PublicAddress;
        }
        
        public bool CheckConfigured(ConfigMode configMode, int os)
        {
            if (configMode == ConfigMode.ColdWallet)
                return CheckHardware();

            return CheckSoftware(os);
        }

        public void PublishError(string title, string message)
        {
            ErrorCallback?.Invoke(title, message);
        }

        bool CheckSoftware(int os)
        {
            var file = $"{Utilities.GetOsSavePath(os)}/wallet.json";
            if (File.Exists(file))
            {
                var wallet = JsonConvert.DeserializeObject<CryptoWallet>(File.ReadAllText(file));
                if (wallet != null)
                    return true;
            }

            return false;
        }

        public void WriteInternalStorage(HardwareWallet hardwareWallet, int os)
        {
            var file = $"{Utilities.GetOsSavePath(os)}/wallet.json";
            if (!File.Exists(file))
            {
                File.WriteAllText(file,JsonConvert.SerializeObject(hardwareWallet));
            }
        }
        public void ReadInternalStorage(string password,  int os)
        {
            
            var file = $"{Utilities.GetOsSavePath(os)}/wallet.json";
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
                        ErrorCallback?.Invoke("Authentication Error!", $"Self destruct activated, too many wrong pins, wallet removed!");
                        return;
                        
                        // Application.Current.Dispatcher.Dispatch(() =>
                        // {
                        //     Application.Current.Windows.ToList().ForEach(y =>
                        //     {
                        //         Application.Current.CloseWindow(y);
                        //     });
                        // });
                    }
                    ErrorCallback?.Invoke("Authentication Error!",$"Wrong pin, {RemainingAttempts} attempts remaining");

 
                    return;
                }

                if (convert == null)
                {
                    LoginAttempt?.Invoke(false);
                    return;
                 }
                
                AuthenicationService.PK = convert.PrivateKey;
                var wallet = AuthenicationService.UnlockWallet(Pass, 97);

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
                        AuthenicationService.PK = string.Empty;

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
            AuthenicationService.PK = string.Empty;
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
                  AuthenicationService.PK =  AuthenicationService.PK.Substring(0,AuthenicationService.PK.Length - 4);
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
                            AuthenicationService.PK = tpm;
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
                            ErrorCallback?.Invoke("Authentication Error!", $"Self destruct activated, too many wrong pins, wallet removed!");
                            return;
                            // Application.Current.Dispatcher.Dispatch(() =>
                            // {
                            //     Application.Current.Windows.ToList().ForEach(y =>
                            //     {
                            //         Application.Current.CloseWindow(y);
                            //     });
                            // });
                            

                        }

                        ErrorCallback?.Invoke("Authenication Error!",$"Wrong pin, {RemainingAttempts} attempts remaining");
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
                var wallet = AuthenicationService.UnlockWallet(Pass,97);

             
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
                        AuthenicationService.PK = string.Empty;

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
            if(_serialPort == null)
                StartSerial();
                
            _serialPort.WriteLine(value);
            _serialPort.DiscardOutBuffer();
            _serialPort.DiscardInBuffer();
        }

        public void Init()
        {
            
            //    DataApiEndpoint = "http://data.uksouth.cloudapp.azure.com";
            DataApiEndpoint = "http://data.uksouth.cloudapp.azure.com";
            
            HideTokenList = "none";
            HideTokenSend = "none";
            ShowPinPanel = "none";
            ShowLoader = "none";
            Receipt = "none";

            IsDevelopment = false;
            RemainingAttempts = 3;


            DefaultPath = AppDomain.CurrentDomain.BaseDirectory;
            ContractABI = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"enabled\",\"type\":\"bool\"}],\"name\":\"BuyBackEnabledUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"tokenAmount\",\"type\":\"uint256\"}],\"name\":\"RewardLiquidityProviders\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"tokensSwapped\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"ethReceived\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"tokensIntoLiqudity\",\"type\":\"uint256\"}],\"name\":\"SwapAndLiquify\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"enabled\",\"type\":\"bool\"}],\"name\":\"SwapAndLiquifyEnabledUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"amountIn\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address[]\",\"name\":\"path\",\"type\":\"address[]\"}],\"name\":\"SwapETHForTokens\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"amountIn\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address[]\",\"name\":\"path\",\"type\":\"address[]\"}],\"name\":\"SwapTokensForETH\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"_liquidityFee\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"_maxTxAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"_taxFee\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"buyBackEnabled\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"buyBackUpperLimitAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"deadAddress\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"subtractedValue\",\"type\":\"uint256\"}],\"name\":\"decreaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tAmount\",\"type\":\"uint256\"}],\"name\":\"deliver\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"excludeFromFee\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"excludeFromReward\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getUnlockTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"includeInFee\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"includeInReward\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"addedValue\",\"type\":\"uint256\"}],\"name\":\"increaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"isExcludedFromFee\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"isExcludedFromReward\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"time\",\"type\":\"uint256\"}],\"name\":\"lock\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"marketingAddress\",\"outputs\":[{\"internalType\":\"address payable\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"marketingDivisor\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"minimumTokensBeforeSwapAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tAmount\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"deductTransferFee\",\"type\":\"bool\"}],\"name\":\"reflectionFromToken\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bool\",\"name\":\"_enabled\",\"type\":\"bool\"}],\"name\":\"setBuyBackEnabled\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"buyBackLimit\",\"type\":\"uint256\"}],\"name\":\"setBuybackUpperLimit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"liquidityFee\",\"type\":\"uint256\"}],\"name\":\"setLiquidityFeePercent\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_marketingAddress\",\"type\":\"address\"}],\"name\":\"setMarketingAddress\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"divisor\",\"type\":\"uint256\"}],\"name\":\"setMarketingDivisor\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"maxTxAmount\",\"type\":\"uint256\"}],\"name\":\"setMaxTxAmount\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minimumTokensBeforeSwap\",\"type\":\"uint256\"}],\"name\":\"setNumTokensSellToAddToLiquidity\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bool\",\"name\":\"_enabled\",\"type\":\"bool\"}],\"name\":\"setSwapAndLiquifyEnabled\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"taxFee\",\"type\":\"uint256\"}],\"name\":\"setTaxFeePercent\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"swapAndLiquifyEnabled\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"rAmount\",\"type\":\"uint256\"}],\"name\":\"tokenFromReflection\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalFees\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"uniswapV2Pair\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"uniswapV2Router\",\"outputs\":[{\"internalType\":\"contract IUniswapV2Router02\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unlock\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]\r\n";

        }
    }
}
