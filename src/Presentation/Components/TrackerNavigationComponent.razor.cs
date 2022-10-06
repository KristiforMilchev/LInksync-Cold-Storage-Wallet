using Domain.Models;
using Microsoft.AspNetCore.Components;
using NFTLock.Data;
using SYNCWallet.Models;
using SYNCWallet.Services;
using SYNCWallet.Services.Definitions;

namespace SYNCWallet.Components.Navigation
{
    public partial class TrackerNavigationComponent
    {
        [Parameter]
        public Token CurrentToken { get; set; }
        
        private ICommunication _Communication { get; set; }
        private IContractService _contractService { get; set; }
        
        public List<NetworkTokenGroup> ListedTokens { get; set; }
        public object ItemChanged { get; set; }


        protected override async Task OnInitializedAsync()
        {
            ListedTokens = new List<NetworkTokenGroup>();
            _contractService = ServiceHelper.GetService<IContractService>();
            _Communication = ServiceHelper.GetService<ICommunication>();
            LoadAllNetworkTokens();
          
        }

        private async void LoadAllNetworkTokens()
        {
            var tokens = await _contractService.GetNetworkTokensIntial(_Communication.ActiveNetwork.Id);
            ListedTokens.Add(new NetworkTokenGroup
            {
                Network = _Communication.ActiveNetwork,
                Tokens = tokens
            });
        }

    
        private void TokenSelected(Token token)
        {
            throw new NotImplementedException();
        }
    }
}