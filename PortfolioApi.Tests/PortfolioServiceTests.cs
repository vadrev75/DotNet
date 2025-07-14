using Moq;
using PortfolioApi.Interfaces;
using PortfolioApi.Models;
using PortfolioApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioApi.Tests
{
    public class PortfolioServiceTests
    {
        private readonly Mock<IRepository> _repoMock;
        private readonly PortfolioService _service;
        private readonly Portfolio _portfolio;

        public PortfolioServiceTests()
        {
            _repoMock = new Mock<IRepository>();
            _service = new PortfolioService(_repoMock.Object);

            _portfolio = new Portfolio
            {
                PortfolioId = "C123",
                Name = "Priority Portfolio"
            };

            _repoMock.Setup(x => x.GetByIdAsync(_portfolio.PortfolioId))
                     .ReturnsAsync(_portfolio);
        }

        [Fact]
        public async Task CreateAsync_AssignsNewId()
        {
            var result = await _service.CreateAsync(new Portfolio { PortfolioId="C123", Name = "New Fund" });
            Assert.NotEqual(string.Empty, result.PortfolioId);
        }

        [Fact]
        public async Task AddAssetAsync_AttachesAssetToPortfolio()
        {
            var asset = new Asset { Ticker = "JPM", AssetType = "Stock" };
            _repoMock.Setup(x => x.SaveAsync(It.IsAny<Portfolio>())).Returns(Task.CompletedTask);
            var result = await _service.AddAssetAsync(_portfolio.PortfolioId, asset);
            Assert.NotEqual(string.Empty, result.GsSecId);
        }

        [Fact]
        public async Task AddTransactionAsync_CreatesTransactionWithId()
        {
            var txn = new Transaction
            {
                TxnId= 12345678,
                TrdId = "C123.1.1",
                GsSecId = "123456",
                Quantity = 10,
                Price = 150m,
                Date = DateTime.Today,
                Type = TransactionType.Buy
            };
            _repoMock.Setup(x => x.AddTransactionAsync(_portfolio.PortfolioId, It.IsAny<Transaction>()))
                     .Returns(Task.CompletedTask);
            var result = await _service.AddTransactionAsync(_portfolio.PortfolioId, txn);
            Assert.NotEqual(0, result.TxnId);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsList()
        {
            _repoMock.Setup(x => x.GetTransactionsAsync(_portfolio.PortfolioId))
                     .ReturnsAsync(new List<Transaction>());
            var txns = await _service.GetTransactionsAsync(_portfolio.PortfolioId);
            Assert.NotNull(txns);
        }

        [Fact]
        public async Task DeleteTransactionAsync_CallsRepository()
        {
            _repoMock.Setup(x => x.DeleteTransactionAsync(_portfolio.PortfolioId, It.IsAny<long>()))
                     .Returns(Task.CompletedTask);
            await _service.DeleteTransactionAsync(_portfolio.PortfolioId, 0);
            _repoMock.Verify(x => x.DeleteTransactionAsync(_portfolio.PortfolioId, It.IsAny<long>()), Times.Once);
        }
    }


}
