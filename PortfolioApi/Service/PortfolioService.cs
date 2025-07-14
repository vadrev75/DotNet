using PortfolioApi.Interfaces;
using PortfolioApi.Models;

namespace PortfolioApi.Service
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IRepository _repo;

        public PortfolioService(IRepository repo) => _repo = repo;

        public async Task<List<Portfolio>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Portfolio?> GetByIdAsync(string portfolioId) => await _repo.GetByIdAsync(portfolioId);

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _repo.SaveAsync(portfolio);
            await _repo.PersistAsync();
            return portfolio;
        }

        public async Task DeleteAsync(string portfolioId)
        {
            await _repo.DeleteAsync(portfolioId);
            await _repo.PersistAsync();
        }

        public async Task<Asset> AddAssetAsync(string portfolioId, Asset asset)
        {
            var portfolio = await _repo.GetByIdAsync(portfolioId);
            if (portfolio == null) throw new Exception("Portfolio not found");

            portfolio.Assets.Add(asset);
            await _repo.SaveAsync(portfolio);
            await _repo.PersistAsync();
            return asset;
        }

        public async Task<Transaction> AddTransactionAsync(string portfolioId, Transaction txn)
        {
            await _repo.AddTransactionAsync(portfolioId, txn);
            return txn;
        }

        public async Task<List<Transaction>> GetTransactionsAsync(string portfolioId) =>
            await _repo.GetTransactionsAsync(portfolioId);

        public async Task DeleteTransactionAsync(string PortfolioId, long txnId) =>
            await _repo.DeleteTransactionAsync(PortfolioId, txnId);
    }
}
