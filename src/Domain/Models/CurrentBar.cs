namespace Domain.Models
{
    public class CurrentBar
    {
        public string s { get; set; }
        public List<decimal> t { get; set; }
        public List<decimal> c { get; set; }
        public List<decimal> o { get; set; }
        public List<decimal> h { get; set; }
        public List<decimal> l { get; set; }
        public List<decimal> v { get; set; }
        public decimal nextTime { get; set; }
    }
}