namespace Domain.Models
{
    public class CurrencyDataSetting
    {
        public  string ContractAddress { get; set; }
        public DateTime LastUpdate { get; set; }
        public int IsEnabled { get; set; } = 1;

    }
}