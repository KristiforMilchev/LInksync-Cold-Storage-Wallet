using Domain.Models;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using SQLite;
using SYNCWallet.Services.Definitions;

namespace Application.Implementation
{
    public class TransactionRepository : SYNCWallet.Services.Definitions.ITransactionRepository
    {
        private readonly SQLiteConnection _database;
        public IUtilities Utilities { get; set; }
        public IHardwareService HardwareService { get; set; }

        TransactionRepository()
        {
            var dbPath = Path.Combine(Utilities.GetOsSavePath(HardwareService.Os), "transactions.db");
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<TranscationRecordDTO>();
        }
        
        public int Create(TranscationRecordDTO entity)
        {
            return _database.Insert(entity);
        }

        public int Update(TranscationRecordDTO entity)
        {
            return _database.Update(entity);
        }

        public int Delete(TranscationRecordDTO entity)
        {
            return _database.Delete(entity);
        }

        public List<TranscationRecordDTO> GetAllTransactionsForAsset(string asset)
        {
            return _database.Table<TranscationRecordDTO>().Where(x => x.Asset == asset).ToList();
        }

        public List<TranscationRecordDTO> GetAll()
        {
            return _database.Table<TranscationRecordDTO>().ToList();
        }

        public TranscationRecordDTO Single(TranscationRecordDTO transcationRecordDto)
        {
            return _database.Table<TranscationRecordDTO>().FirstOrDefault(x => x == transcationRecordDto);
        }

        
    }
}