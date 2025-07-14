using PortfolioApi.Interfaces;
using PortfolioApi.Models;

namespace PortfolioApi.Service
{
    public class PerformanceEngine : IPerformanceEngine
    {
        private readonly IPriceProvider _prices;
        public PerformanceEngine(IPriceProvider provider) => _prices = provider;

        public int GetNetQty(IEnumerable<Transaction> txns, DateTime date) =>
            txns.Where(t => t.Date <= date).Sum(t => t.Type == TransactionType.Buy ? t.Quantity : -t.Quantity);

        public async Task<Dictionary<DateTime, decimal>> GetValueOverTimeAsync(Portfolio p, DateTime start, DateTime end)
        {
            var result = new Dictionary<DateTime, decimal>();

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                decimal total = 0;
                foreach (var asset in p.Assets)
                {
                    var txns = p.Transactions.Where(t => t.GsSecId == asset.GsSecId);
                    var qty = txns.Where(t => t.Date <= date)
                                  .Sum(t => t.Type == TransactionType.Buy ? t.Quantity : -t.Quantity);


                    Console.WriteLine("Quantity: " + qty);
                    var price = await _prices.GetHistoricalPriceAsync(asset.Ticker, date);
                    total += qty * price;
                }
                result[date] = total; 
            }

            return result;
        }


        public async Task<Dictionary<string, decimal>> GetAllocationAsync(Portfolio p, DateTime date)
        {
            var allocations = new Dictionary<string, decimal>();
            decimal totalValue = 0;

            foreach (var asset in p.Assets)
            {
                var txns = p.Transactions.Where(t => t.GsSecId == asset.GsSecId && t.Date <= date);
                int netQty = txns.Sum(t => t.Type == TransactionType.Buy ? t.Quantity : -t.Quantity);
                if (netQty <= 0) continue;

                var price = await _prices.GetHistoricalPriceAsync(asset.Ticker, date);
                var value = netQty * price;

                allocations[asset.Ticker] = value;
                totalValue += value;
            }

            // Convert to percentages
            return allocations.ToDictionary(
                kvp => kvp.Key,
                kvp => totalValue == 0 ? 0 : Math.Round((kvp.Value / totalValue) * 100, 2));
        }


        public async Task<Dictionary<string, decimal>> GetRealizedGainsAsync(Portfolio portfolio, DateTime start, DateTime end)
        {
            var result = new Dictionary<string, decimal>();

            // Group transactions by asset ID
            var groupedTxns = portfolio.Transactions
                .Where(t => t.Date >= start && t.Date <= end)
                .OrderBy(t => t.Date)
                .GroupBy(t => t.GsSecId);

            foreach (var group in groupedTxns)
            {
                var GsSecId = group.Key;
                var asset = portfolio.Assets.FirstOrDefault(a => a.GsSecId == GsSecId);
                if (asset == null || string.IsNullOrWhiteSpace(asset.Ticker)) continue;

                decimal avgCost = 0;
                int heldQty = 0;
                decimal realized = 0;

                foreach (var txn in group)
                {
                    if (txn.Type == TransactionType.Buy)
                    {
                        avgCost = ((avgCost * heldQty) + ((txn.Price * txn.Quantity) + txn.Fees)) / (heldQty + txn.Quantity);
                        heldQty += txn.Quantity;
                    }
                    else if (txn.Type == TransactionType.Sell)
                    {
                        realized += ((txn.Price - avgCost) * txn.Quantity) - txn.Fees;
                        heldQty -= txn.Quantity;
                    }
                }

                result[asset.Ticker] = Math.Round(realized, 2);
            }

            return await Task.FromResult(result);
        }



        public async Task<Dictionary<string, decimal>> GetUnrealizedGainsAsync(Portfolio p, DateTime date)
        {
            decimal totalUnrealized = 0;
            var result = new Dictionary<string, decimal>();

            foreach (var asset in p.Assets)
            {
                var txns = p.Transactions
                    .Where(t => t.GsSecId == asset.GsSecId && t.Date <= date)
                    .OrderBy(t => t.Date)
                    .ToList();

                int netQty = txns.Sum(t => t.Type == TransactionType.Buy ? t.Quantity : -t.Quantity);
                if (netQty <= 0) continue;

                decimal totalCost = 0;
                int qtySoFar = 0;

                decimal avgCost = 0;

                foreach (var txn in txns)
                {
                     if (txn.Type == TransactionType.Buy)
                     {
                         avgCost = ((avgCost * qtySoFar) + ((txn.Price * txn.Quantity) + txn.Fees)) / (qtySoFar + txn.Quantity);
                        qtySoFar += txn.Quantity;
                     }
                     else if (txn.Type == TransactionType.Sell)
                     {
                        //totalCost -= (txn.Price * txn.Quantity) + txn.Fees ;
                        totalCost = avgCost; //remains
                        qtySoFar -= txn.Quantity;
                     }
                }

                // var currentPrice = await _prices.GetCurrentPriceAsync(asset.Ticker);
                var currentPrice = await _prices.GetHistoricalPriceAdjustedAsync(asset.Ticker, date);
                //var currentValue = netQty * currentPrice;
                var gain = (currentPrice - totalCost) * netQty;

                totalUnrealized += gain;

                result.Add(asset.Ticker, Math.Round(totalUnrealized, 2));
            }

            return result;
            //return Math.Round(totalUnrealized, 2);
        }
    }
}
