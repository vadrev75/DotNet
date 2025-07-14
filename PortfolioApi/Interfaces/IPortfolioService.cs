using PortfolioApi.Models;

namespace PortfolioApi.Interfaces
{
    public interface IPortfolioService
    {
        Task<List<Portfolio>> GetAllAsync();
        Task<Portfolio?> GetByIdAsync(string id);
        Task<Portfolio> CreateAsync(Portfolio portfolio);
        Task DeleteAsync(string id);
        Task<Asset> AddAssetAsync(string portfolioId, Asset asset);
        Task<Transaction> AddTransactionAsync(string portfolioId, Transaction transaction);
        Task<List<Transaction>> GetTransactionsAsync(string portfolioId);
        Task DeleteTransactionAsync(string portfolioId, long txnId);
    }
}
