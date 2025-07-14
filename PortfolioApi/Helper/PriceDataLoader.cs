using PortfolioApi.Models;
using System.Text.Json;

namespace PortfolioApi.Helper
{
    public static class PriceDataLoader
    {
        public static async Task<IEnumerable<PriceRecord>> LoadJsonAsync(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("Price data file not found.");

            await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var records = await JsonSerializer.DeserializeAsync<List<PriceRecord>>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return records ?? Enumerable.Empty<PriceRecord>();
        }
    }

}
