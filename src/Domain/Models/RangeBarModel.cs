using System.Diagnostics;
using Domain.Handlers;

namespace Domain.Models
{
    [DebuggerDisplay("Bar {Index} {TimestampDate} {CurrentPrice}")]
    public class RangeBarModel
    {
        private DateTime? _date;
        public double Timestamp { get; set; }

        public DateTime? Date
        {
            get => _date;
            set
            {
                if (value == null)
                    return;

                _date = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                Timestamp = _date.Value.ToUnixSeconds();
            }
        }

        public decimal? Mid { get; set; }

        public decimal? Bid { get; set; }
        public decimal? Ask { get; set; }

        public decimal? Open { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
        public decimal? Close { get; set; }
        public decimal? Volume { get; set; }
 
    }
}