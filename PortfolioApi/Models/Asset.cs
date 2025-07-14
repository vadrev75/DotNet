namespace PortfolioApi.Models
{
    public class Asset
    {
        public string GsSecId { get; set; }
        public static string? Description { get; set; } // Static description for the asset  
        public string Ticker { get; set; } // Stock ticker symbol or bond identifier
        public string? Cusip { get; set; } // Optional: U.S. security identifier
        public string? Isin { get; set; }  // Optional: Global security identifier
        public string AssetType { get; set; } // Stock, Bond, etc.  
        public string? AssetClass { get; set; } // Optional: Asset class (e.g., Equity, Fixed Income)
    }
}
