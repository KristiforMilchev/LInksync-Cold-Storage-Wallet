using Domain.Models;
using SQLite;

namespace SYNCWallet.Services.Definitions
{
    public interface ITransactionRepository

    {
        public int Create(TranscationRecordDTO entity);
        public int Update(TranscationRecordDTO entity);
        public int Delete(TranscationRecordDTO entity);
        public List<TranscationRecordDTO> GetAllTransactionsForAsset(string asset);
        public List<TranscationRecordDTO> GetAll();
        public TranscationRecordDTO Single(TranscationRecordDTO transcationRecordDto);
        
    }
}