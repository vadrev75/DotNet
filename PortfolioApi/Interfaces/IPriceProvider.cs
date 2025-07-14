namespace PortfolioApi.Interfaces
{
    public interface IPriceProvider
    {
        Task<decimal> GetCurrentPriceAsync(string symbol); //Latest Adjusted Price for Un-Realized Gain/Loss Calculation
        Task<decimal> GetHistoricalPriceAsync(string symbol, DateTime date); //Close Price for Realized Gain/Loss Calculation
        Task<decimal> GetHistoricalPriceAdjustedAsync(string symbol, DateTime date); //Historical Adjusted Price for Un-Realized Gain/Loss Calculation
    }
}
