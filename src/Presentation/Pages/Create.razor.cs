

namespace LInksync_Cold_Storage_Wallet.Pages
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System.Net;
    using System.Timers;
    using System.Numerics;
    using NFTLock.Data;
    using NFTLock.Models;
    using Newtonsoft.Json;
    using SYNCWallet.Models;
    using SYNCWallet.Services;
    using SYNCWallet.Services.Definitions;

    
    public partial class Create
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }
        private string Password{ get; set; }
        public List<Word> Words { get; set; }
        public CryptoWallet Wallet { get; set; }
        public Word Selected { get; set; }
        public int Validated { get; set; }
        public int WordNum { get; set; }
        public string Word { get; set; }
        public string PassCode { get; set; }
        IHardwareService HardwareService { get; set; }
        IUtilities Utilities { get; set; }
        ICommunication Communication { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HardwareService = ServiceHelper.GetService<IHardwareService>();
            Utilities = ServiceHelper.GetService<IUtilities>();
            Communication = ServiceHelper.GetService<ICommunication>();
        }

        private void CreateNew()
        {
            Validated = 0;
            Wallet = HardwareService.CreateAccount();

            InvokeAsync(() =>
            {
                Words = Wallet.Words;
                StateHasChanged();
            });
            JS.InvokeVoidAsync("OpenSeedConfimr");
        }


        private void ImportAccountScreen()
        {
            JS.InvokeVoidAsync("OpenImportConfirm");

        }

        private async void CheckWallet()
        {
            var getWords = await JS.InvokeAsync<List<string>>("GetImportWords");
            var words = new List<Word>();



            getWords.ForEach(x =>
            {
                words.Add(new Word
                {
                    Name = x
                });
            });
            Wallet = HardwareService.ImportAccount(words);

            SetPin();
        }

        private void ConfirmWallet()
        {
            JS.InvokeVoidAsync("ConfirmSeed");


            if(Selected == null)
            {
                var random = new Random();
                WordNum = random.Next(0, Words.Count -1);
                Selected = Words.ElementAt(WordNum);

            }
            else
            {
                var last = Words.ElementAt(WordNum);

                if (Selected.Name == Word)
                {

                    Validated++;
                    Word = "";
                }

                var random = new Random();
                WordNum = random.Next(0, Words.Count -1);
                Selected = Words.ElementAt(WordNum);


            }

            if (Validated > 2)
            {
                SetPin();
            }
        }

        private void SetPin()
        {
            JS.InvokeVoidAsync("SetPin");
        }

        private void CreateWallet()
        {

            try
            {
                if(PassCode.Length < 8)
                {
                    Communication.PublishError("Lenght requirement violated.", "The pin code has to be equal to 8 characters!");
                    return;
                }

                var passwrod = HardwareService.Encrypt("iV1z@$H8",PassCode);
                var PK = HardwareService.Encrypt(Wallet.PrivateKey, PassCode);

                switch (Communication.SoftwareType)
                {
                    case Enums.ConfigMode.ColdWallet:
                        Communication.WriteState(JsonConvert.SerializeObject(new HardwareWallet
                            {
                                Cmd = "NEW",
                                Password = passwrod,
                                PrivateKey = PK
                            }));

                        break;
                    case Enums.ConfigMode.HotWallet:

                        Communication.WriteInternalStorage(new HardwareWallet
                            {
                                Cmd = "NEW",
                                Password = passwrod,
                                PrivateKey = PK
                            }, HardwareService.Os);
                        break;
                }

                Communication.IsConfigured = true;
                Communication.ConfigResponse = true;
                NavigationManager.NavigateTo("/LoginPanel");
            
            }
            catch (Exception e)
            {
                
                
            }

        }
    }
}
