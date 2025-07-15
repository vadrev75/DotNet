using PortfolioApi.Interfaces;
using PortfolioApi.Models;
using System.Text.Json;

namespace PortfolioApi.Repository
{
    public class JsonRepository : IRepository
    {
        private const string FilePath = "portfolios.json";
        private List<Portfolio> _store = new();

        public JsonRepository()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                _store = JsonSerializer.Deserialize<List<Portfolio>>(json) ?? new();
            }
        }

        public Task<List<Portfolio>> GetAllAsync() => Task.FromResult(_store);

        public Task<Portfolio?> GetByIdAsync(string PortfolioId) =>
            Task.FromResult(_store.FirstOrDefault(p => p.PortfolioId == PortfolioId));

        public Task SaveAsync(Portfolio portfolio)
        {
            var existing = _store.FirstOrDefault(p => p.PortfolioId == portfolio.PortfolioId);
            if (existing != null) _store.Remove(existing);
            _store.Add(portfolio);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string PortfolioId)
        {
            _store.RemoveAll(p => p.PortfolioId == PortfolioId);
            return Task.CompletedTask;
        }

        public async Task PersistAsync()
        {
            await using var stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(stream, _store, new JsonSerializerOptions { WriteIndented = true });
        }

        // Transaction APIs

        public async Task<List<Transaction>> GetTransactionsAsync(string portfolioId)
        {
            var portfolio = _store.FirstOrDefault(p => p.PortfolioId == portfolioId);
            return await Task.FromResult(portfolio?.Transactions ?? new List<Transaction>());
        }

        public async Task AddTransactionAsync(string portfolioId, Transaction transaction)
        {
            var portfolio = _store.FirstOrDefault(p => p.PortfolioId == portfolioId);
            if (portfolio != null)
            {
                portfolio.Transactions.Add(transaction);
                await SaveAsync(portfolio);
                await PersistAsync();
            }
        }

        public async Task DeleteTransactionAsync(string portfolioId, long txnId)
        {
            var portfolio = _store.FirstOrDefault(p => p.PortfolioId == portfolioId);
            if (portfolio != null)
            {
                portfolio.Transactions.RemoveAll(t => t.TxnId == txnId);
                await SaveAsync(portfolio);
                await PersistAsync();
            }
        }
    }

}
