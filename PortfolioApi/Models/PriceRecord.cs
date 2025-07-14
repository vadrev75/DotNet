namespace PortfolioApi.Models
{
    public class PriceRecord
    {
        public DateTime Date { get; set; }
        public string Ticker { get; set; } = "";
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Adjusted { get; set; }
        public decimal Returns { get; set; }
        public long Volume { get; set; }
    }
}
