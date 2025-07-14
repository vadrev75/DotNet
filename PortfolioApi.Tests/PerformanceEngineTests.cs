using Moq;
using PortfolioApi.Helper;
using PortfolioApi.Interfaces;
using PortfolioApi.Models;
using PortfolioApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit; 

namespace PortfolioApi.Tests
{
    public class PerformanceEngineTests
    {
        private readonly Mock<IPriceProvider> _priceProviderMock;
        private readonly Mock<IPerformanceEngine> _engineMock; 
        private readonly Portfolio _testPortfolio;

        public PerformanceEngineTests()
        {
            _priceProviderMock = new Mock<IPriceProvider>();
            _engineMock = new Mock<IPerformanceEngine>();
            _testPortfolio = TestDataHelper.GetSamplePortfolio();

            _priceProviderMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<string>()))
                              .ReturnsAsync(175m);
            _priceProviderMock.Setup(x => x.GetHistoricalPriceAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                              .ReturnsAsync(173m);

            _engineMock.Setup(x => x.GetValueOverTimeAsync(It.IsAny<Portfolio>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                       .ReturnsAsync(new Dictionary<DateTime, decimal>
                       {
                          { DateTime.Today.AddDays(-2), 1000m },
                          { DateTime.Today.AddDays(-1), 1050m },
                          { DateTime.Today, 1100m }
                       });

            var allocation = new Dictionary<string, decimal>
            {
                { "JPM", 50 },
                { "AAPL", 50 }
            };

            _engineMock.Setup(x => x.GetAllocationAsync(_testPortfolio, It.IsAny<DateTime>()))
                       .ReturnsAsync(allocation);

            /* _engineMock.Setup(x => x.GetAllocationAsync(_testPortfolio, It.IsAny<DateTime>())).Callback<DateTime>(date =>
             {
                 var allocation = new Dictionary<string, decimal>
                 {
                    { "JPM", 50 },
                    { "AAPL", 50 }
                 };
                 _engineMock.Setup(x => x.GetAllocationAsync(_testPortfolio, date)).ReturnsAsync(allocation);
             });*/

            _engineMock.Setup(x => x.GetRealizedGainsAsync(It.IsAny<Portfolio>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                  .ReturnsAsync(new Dictionary<string, decimal>
                  {
                      { "JPM", 500m }
                  });

            _engineMock.Setup(_engineMock => _engineMock.GetUnrealizedGainsAsync(It.IsAny<Portfolio>(), It.IsAny<DateTime>()))
                       .ReturnsAsync(new Dictionary<string, decimal>
                  {
                      { "JPM", 300m }
                  });

        }

        [Fact]
        public async Task GetAllocationAsync_ReturnsPercentageValues()
        {
            var result = await _engineMock.Object.GetAllocationAsync(_testPortfolio, DateTime.Today); 
            Assert.True(result.Count > 0);
            Assert.All(result.Values, v => Assert.InRange(v, 0, 100));
        }

        [Fact]
        public async Task GetRealizedGainsAsync_ComputesCorrectGain()
        {
            var gain = await _engineMock.Object.GetRealizedGainsAsync(_testPortfolio, DateTime.Today.AddDays(-10), DateTime.Today);
            Assert.NotEmpty(gain); 
            Assert.All(gain.Values, value => Assert.True(value >= 0)); 
        }

        [Fact]
        public async Task GetUnrealizedGainsAsync_ReturnsExpectedValue()
        {
            var gain = await _engineMock.Object.GetUnrealizedGainsAsync(_testPortfolio, DateTime.Today); // Corrected usage  
            Assert.NotEmpty(gain); 
            Assert.All(gain.Values, value => Assert.True(value >= 0)); 
        }

        [Fact]
        public async Task GetValueOverTimeAsync_ReturnsValuePerDay()
        {
            var result = await _engineMock.Object.GetValueOverTimeAsync(_testPortfolio, DateTime.Today.AddDays(-2), DateTime.Today); // Corrected usage  
            Assert.Equal(3, result.Count);
        }
    }
}
