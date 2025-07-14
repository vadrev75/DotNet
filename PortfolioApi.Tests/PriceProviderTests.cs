using PortfolioApi.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioApi.Tests
{
    public class PriceProviderTests
    {
        private readonly JsonPriceProvider _provider;

        public PriceProviderTests()
        {
            _provider = new JsonPriceProvider("portfolio_prices.json");
        }

        [Fact]
        public async Task GetCurrentPriceAsync_ReturnsPrice_WhenTickerExists()
        {
            var price = await _provider.GetCurrentPriceAsync("JPM");
            Assert.True(price > 0);
        }

        [Fact]
        public async Task GetHistoricalPriceAsync_ReturnsPrice_WhenDataExists()
        {
            var date = new DateTime(2025, 07, 01);
            var price = await _provider.GetHistoricalPriceAsync("JPM", date);
            Assert.True(price > 0);
        }

        [Fact]
        public async Task GetHistoricalPriceAsync_FallsBack_WhenDateMissing()
        {
            var fallback = await _provider.GetHistoricalPriceAsync("XYZ", new DateTime(2010, 01, 01));
            Assert.Equal(100m, fallback);
        }
    }
}
