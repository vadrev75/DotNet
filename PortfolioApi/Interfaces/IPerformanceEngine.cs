using PortfolioApi.Models;

namespace PortfolioApi.Interfaces
{
    public interface IPerformanceEngine
    {
        Task<Dictionary<DateTime, decimal>> GetValueOverTimeAsync(Portfolio portfolio, DateTime start, DateTime end);
        Task<Dictionary<string, decimal>> GetAllocationAsync(Portfolio portfolio, DateTime date);
        Task<Dictionary<string, decimal>> GetRealizedGainsAsync(Portfolio portfolio, DateTime start, DateTime end);
        Task<Dictionary<string, decimal>> GetUnrealizedGainsAsync(Portfolio portfolio, DateTime date);
    }
}
