
using System.Timers;
using Nethereum.JsonRpc.Client.Streaming;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Subscriptions;
using Nethereum.Web3;
using Newtonsoft.Json;
using SYNCWallet.Services.Definitions;
using Timer = System.Timers.Timer;

namespace Application.Implementation
{
    public class BlockProcessor : IBlockProcessor
    {
        private ICommunication Communication { get; set; }
        private  DateTime NextCheck { get; set; }
        public Timer BalanceCheck { get; set; }

        public BlockProcessor(ICommunication communication)
        {
            Communication = communication;
        }
        
        public async void BeginProcessing()
        {
            NextCheck = NextCheck.AddMinutes(3);
            //TODO Remove dependency on time, default to data on new blocks
            BalanceCheck = new System.Timers.Timer();
            BalanceCheck= new System.Timers.Timer();
            BalanceCheck.Elapsed += new ElapsedEventHandler(OnBalanceUpdate);
            BalanceCheck.Interval = 5000;
            BalanceCheck.Start();
        }

        private void OnBalanceUpdate(object source, ElapsedEventArgs e)
        {
            if (DateTime.UtcNow > NextCheck)
            {
                Communication.IncomingBlockCallback?.Invoke();
                NextCheck = NextCheck.AddMinutes(3);
            }
        }
        
        public void Dispose()
        {
            BalanceCheck.Stop();
            BalanceCheck.Dispose();
        }
    }
}