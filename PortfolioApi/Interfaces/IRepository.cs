using PortfolioApi.Models;

namespace PortfolioApi.Interfaces
{
    public interface IRepository
    {
        Task<List<Portfolio>> GetAllAsync();
        Task<Portfolio?> GetByIdAsync(string portfolioId);
        Task SaveAsync(Portfolio portfolio);
        Task DeleteAsync(string portfolioId);
        Task PersistAsync();

        //Trransactions
        Task<List<Transaction>> GetTransactionsAsync(string portfolioId);
        Task AddTransactionAsync(string portfolioId, Transaction transaction);
        Task DeleteTransactionAsync(string portfolioId, long txnId);
    }
}
