namespace Domain.Models
{
    public class TranscationRecordDTO
    {
        public string TransactionHash { get; set; }
        public string Asset { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public decimal Value { get; set; }
    }
}