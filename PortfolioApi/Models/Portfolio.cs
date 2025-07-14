using System.Transactions;

namespace PortfolioApi.Models
{
    public class Portfolio
    {
        public required string PortfolioId { get; set; } //portfolio id C123, H123, etc.
        public string? Name { get; set; }
        public string? ParentId { get; set; } //parent portfolio id, if any
        public string? Classification { get; set; } // e.g., "Policy Portfolio - C",  "Active Portfolio - H"
        public List<Asset> Assets { get; set; } = new(); // List of assets in the portfolio
        public List<Transaction> Transactions { get; set; } = new(); // List of transactions in the portfolio
    }
}
