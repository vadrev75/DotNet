using PortfolioApi.Interfaces;
using PortfolioApi.Models;

namespace PortfolioApi.Helper
{
    public class JsonPriceProvider : IPriceProvider
    {
        private readonly Dictionary<(string, string), PriceRecord> _records = new();
        private readonly Dictionary<string, PriceRecord> _latestByTicker = new();

        public JsonPriceProvider(string jsonPath)
        {
            LoadPricesAsync(jsonPath).Wait(); //  init
        }

        private async Task LoadPricesAsync(string path)
        {
            var records = await PriceDataLoader.LoadJsonAsync(path);

            foreach (var record in records)
            {
                var key = (record.Ticker, record.Date.ToString("yyyy-MM-dd"));
                _records[key] = record;

                if (!_latestByTicker.ContainsKey(record.Ticker) || record.Date > _latestByTicker[record.Ticker].Date)
                    _latestByTicker[record.Ticker] = record;
            }
        }

        public Task<decimal> GetCurrentPriceAsync(string symbol)
        {
            //return Task.FromResult(125m);
            if (_latestByTicker.TryGetValue(symbol, out var record))
                return Task.FromResult(record.Adjusted);

            Console.WriteLine($" Current price fallback for {symbol}");
            return Task.FromResult(100m); //This can be fetching record from http API or a default value
        }

        public Task<decimal> GetHistoricalPriceAdjustedAsync(string symbol, DateTime date)
        {
            Console.WriteLine("Key:" + symbol + " Date:" + date.ToString("yyyy-MM-dd"));
            var key = (symbol, date.ToString("yyyy-MM-dd"));
            if (_records.TryGetValue(key, out var record))
                return Task.FromResult(record.Adjusted);

            Console.WriteLine($" Historical price fallback for {symbol} on {date:yyyy-MM-dd}");
            return GetCurrentPriceAsync(symbol); //This can be fetching record from http API or a default value
        }

        public Task<decimal> GetHistoricalPriceAsync(string symbol, DateTime date)
        {
            Console.WriteLine("Key:" + symbol + " Date:" + date.ToString("yyyy-MM-dd"));
            var key = (symbol, date.ToString("yyyy-MM-dd"));
            if (_records.TryGetValue(key, out var record))
                return Task.FromResult(record.Close);

            Console.WriteLine($" Historical price fallback for {symbol} on {date:yyyy-MM-dd}");
            return GetCurrentPriceAsync(symbol); //This can be fetching record from http API or a default value
        }
    }
}
