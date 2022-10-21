namespace Domain.Models
{
    public class UserAssetBalance
    {
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public decimal PevBalance { get; set; }
        public DateTime Date { get; set; }
        public string WalletAddress { get; set; }
        public int NetworkId { get; set; }
    }
}