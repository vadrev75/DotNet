using PortfolioApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioApi.Tests
{
    public static class TestDataHelper
    {
        public static Portfolio GetSamplePortfolio()
        {
            var asset = new Asset
            {
                GsSecId = "123456",
                Ticker = "JPM",
                AssetType = "Stock"
            };

            return new Portfolio
            {
                PortfolioId = "C123",
                Name = "Mock Portfolio",
                Assets = new List<Asset> { asset },
                Transactions = new List<Transaction> 
                {
                    new Transaction {
                        TxnId = 12345678,
                        TrdId = "C123.1.1",
                        GsSecId = asset.GsSecId,
                        Date = DateTime.Today.AddDays(-2),
                        Quantity = 10,
                        Price = 170m,
                        Type = TransactionType.Buy
                    },
                    new Transaction {
                        TxnId = 12345679,
                        TrdId = "C123.2.1",
                        GsSecId = asset.GsSecId,
                        Date = DateTime.Today,
                        Quantity = 5,
                        Price = 174m,
                        Type = TransactionType.Sell
                    }
                }
            };
        }
    }

}
