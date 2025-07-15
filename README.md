# Portfolio Performance API

- This project is a backend system for tracking investment portfolio performance, including asset allocation, total value over time, realized and unrealized gains.

---

## Tech Stack

- **C# / .NET Core**
- **RESTful API**
- **JSON file-based persistence**
- **xUnit + Moq test suite**
- **Swagger / OpenAPI documentation**

---

## Portfolio API Methods 
API Persists Portfolio along with Assets and Transaction record in Portfolio.json file. 

1. Create and manage investment portfolios (each with a unique CientId/Name) 
   Ex: Portfolios:  **C123, H123**
2. Add, update, and remove assets (e.g., stocks, bonds) within a portfolio.
3. Record transactions (buy/sell) for assets in a portfolio, including date, quantity, and price.

##  Portfolio Endpoints

### GET `/api/portfolios`
Returns: All portfolios

### GET `/api/portfolios/{id}`
Returns: Specific portfolios

### POST `/api/portfolios`
Creates New Portfolio

### DELETE `/api/portfolios/{id}`
Deletes Portfolio by Id

##  Asset Endpoints

### POST `/api/portfolios/{id}/assets`
Adds: New asset to portfolio

##  Transaction Endpoints

### POST `/api/portfolios/{id}/transactions`
Adds: Buy/Sell transaction

---

## PerformanceEngine 

The PerformanceEngine is a core service class that calculates financial analytics for investment portfolios. It consumes transaction and asset data, along with dynamic pricing from the IPriceProvider interface, to produce key performance metrics:

- **Value Over Time**: Tracks total portfolio valuation across a date range
- **Asset Allocation**: Breaks down holdings by percentage on a specific date
- **Realized Gains**: Calculates profit/loss from completed sales using average cost basis
- **Unrealized Gains**: Measures potential profit/loss of currently held assets using current prices

All methods are fully asynchronous (async/await) for scalability and integration flexibility. The engine is stateless and designed for testability, separation of concerns, and modular extension. All methods are asynchronous (`Task<T>`) and use `IPriceProvider` to fetch historical and current prices.

## .Net Core Task Parallel Library (TPL) Stream Usage in PriceDataLoader
The PriceDataLoader utilizes `Task Parallel Library (TPL)` and streaming I/O to efficiently ingest large financial datasets from a JSON file (portfolio_prices.json from [kaggle] (https://www.kaggle.com/datasets/nikitamanaenkov/stock-portfolio-data-with-prices-and-indices/data?select=Portfolio_prices.csv). Instead of loading the entire file into memory at once, it processes records one-by-one using:

- StreamReader or FileStream with buffered reads
- `await` ReadLineAsync() or JsonSerializer.DeserializeAsync() for asynchronous file I/O
- ConcurrentDictionary for thread-safe accumulation of ticker-date price data

This approach enables:

- Scalability for large datasets (millions of records)
- Parallel record parsing without blocking the main thread
- Memory safety through controlled buffering and streaming

## Sample Test Data illustration for Portfolio Performance
|Date |Portfolio |Ticker   |Txn Type  |Quantity |Price  |Fees |Current Holdings |Average Cost |Total Cost |Market Price |Realized Gain/Loss |Unrealized Gain/Loss
|--------|------|-----|-----|----------|-------|------|------------------|----------------|-------------|--------------|--------------------|---------------------|
|Jan 1, 2023  |`C123`  |JPM     |Buy  |10 |$100.00  |5  |10 |($100*10+$5)/10=$100.50  |$1,005 |$100 |$0.00  |($100.00 - $100.50) * 10 = -$5.00|
|Jan 15, 2023 |`C123`  |JPM     |Buy  |5  |$110.00  |5  |15 |(($100.50*10) + ($110*5 + $5)) / 15 = ($1005 + $555) / 15 = $104.00  |$1,560 |$100 |$0.00  |($112.00 - $104.00) * 15 = $120.00|
|Feb 1, 2023  |`C123`  |JPM     |Sell |7  |$120.00  |5  |8  |$104.00 (remains)  |$832 |$125 |(($120.00 - $104.00) * 7) - $5.00 = ($16 * 7) - $5 = $112 - $5 = $107.00|  ($125.00 - $104.00) * 8 = $168.00|
---

## 1. Total portfolio value over time

1. **Task<Dictionary<DateTime, decimal>> `GetValueOverTimeAsync`(Portfolio p, DateTime start, DateTime end)**

#### Calculates portfolio value for each day by:
- Computing net quantity of each asset as of the date
- Fetching historical price for that asset
- Summing all asset values

### API Request/Response:
- **`GET /api/portfolios/{id}/performance/value-over-time`**

### Request
~~~ 
api/portfolios/C123/performance/value-over-time?portfolioId=C123&start=2023-01-01&end=2023-02-01
~~~ 
### Response
~~~ 
{
  "2023-01-01T00:00:00": 2416.300048828125,
  "2023-01-02T00:00:00": 2416.300048828125,
  "2023-01-03T00:00:00": 1351.199951171875,
  "2023-01-04T00:00:00": 1363.800048828125,
  "2023-01-05T00:00:00": 1353.5000610351562,
  "2023-01-06T00:00:00": 1379.4000244140625,
  "2023-01-07T00:00:00": 2416.300048828125,
  "2023-01-08T00:00:00": 2416.300048828125,
  "2023-01-09T00:00:00": 1373.699951171875,
  "2023-01-10T00:00:00": 1386.0000610351562,
  "2023-01-11T00:00:00": 1396.300048828125,
  "2023-01-12T00:00:00": 1394.9000549316406,
  "2023-01-13T00:00:00": 1430.0999450683594,
  "2023-01-14T00:00:00": 2416.300048828125,
  "2023-01-15T00:00:00": 3624.4500732421875,
  "2023-01-16T00:00:00": 3624.4500732421875,
  "2023-01-17T00:00:00": 2112.000045776367,
  "2023-01-18T00:00:00": 2048.5501098632812,
  "2023-01-19T00:00:00": 2021.25,
  "2023-01-20T00:00:00": 2026.2000274658205,
  "2023-01-21T00:00:00": 3624.4500732421875,
  "2023-01-22T00:00:00": 3624.4500732421875,
  "2023-01-23T00:00:00": 2059.050064086914,
  "2023-01-24T00:00:00": 2076.749954223633,
  "2023-01-25T00:00:00": 2086.7999267578125,
  "2023-01-26T00:00:00": 2099.699935913086,
  "2023-01-27T00:00:00": 2104.8001098632812,
  "2023-01-28T00:00:00": 3624.4500732421875,
  "2023-01-29T00:00:00": 3624.4500732421875,
  "2023-01-30T00:00:00": 2086.9500732421875,
  "2023-01-31T00:00:00": 2099.400100708008,
  "2023-02-01T00:00:00": 1116.719970703125
}
~~~ 

---
## 2. Asset allocation breakdown

2. **Task<Dictionary<string, decimal>> `GetAllocationAsync`(Portfolio p, DateTime date)**

#### Returns percentage breakdown of asset value:
- Value per asset = net quantity × historical price
- Allocation = (asset value / total portfolio value) × 100

### API Request/Response:
- **`GET /api/portfolios/{id}/performance/allocation`**

### Request
~~~ 
https://localhost:7244/api/portfolios/C123/performance/allocation?portfolioId =C123&date=2023-02-01
~~~ 
### Response
~~~ 
{
  "JPM": 100
}
~~~ 

---

## 3. Realized Gain/Loss

3. **Task<decimal> `GetRealizedGainsAsync`(IEnumerable<Transaction> txns, DateTime start, DateTime end)**

#### Calculates profit/loss from sold assets using Average Cost Basis:
- On each buy: update average cost
- On each sell: gain = (sell price − avg cost) × quantity

### API Request/Response:
- **`GET /api/portfolios/{id}/performance/realized`**

### Request
~~~ 
https://localhost:7244/api/portfolios/C123/performance/realized?start=2023-01-01&end=2023-02-01
~~~ 
### Response
~~~ 
{
  "JPM": 107
}
~~~ 

---

## 4. Unrealized gains/losses

4. **Task<decimal> `GetUnrealizedGainsAsync`(Portfolio p, DateTime date)


#### Calculates profit/loss of held assets (not yet sold):
- Total cost basis from buy/sell history
- Current value = quantity held × current price
- Gain = current value − cost basis

### API Request/Response:
- **`GET /api/portfolios/{id}/performance/unrealized`**

### Request
~~~ 
https://localhost:7244/api/portfolios/C123/performance/unrealized?date=2023-02-01
~~~ 
### Response
~~~ 
{
  "JPM": 168
}
~~~ 

---
## Notes

- All endpoints return application/json
- Ensure GsSecId (asset id) matches actual portfolio asset
- Dates must be in YYYY-MM-DD format

---

##  Design Patterns Used

| Pattern                   | Purpose                                                         |
|--------------------------|------------------------------------------------------------------|
| Repository Pattern        | Abstracts data access (e.g. `IRepository`, `JsonRepository`)     |
| Service Layer             | Encapsulates logic (`PortfolioService`, `PerformanceEngine`)     |
| Strategy Pattern          | Price retrieval via pluggable `IPriceProvider` implementations   |
| Dependency Injection      | All services are injected at runtime (`Program.cs`)              |
| DTO / Domain Separation   | Clean entity definitions for `Portfolio`, `Asset`, etc.          |
| Mocking via Moq           | Simulates repo and service behaviors for testing                 |
| Single Responsibility     | Each class focuses on one concern (e.g. pricing, logic, storage) |

---

## Test Coverage

- **Framework**: xUnit with Moq
- **Tests**: PortfolioService, JsonPriceProvider, PerformanceEngine
- **Goal**: ≥ 100% method coverage
- Sample data: `portfolio_prices.json`, `portfolios.json`

---
## Swagger Documentation

~~~ 
https://your-domain/swagger
~~~ 
### Includes
- Response types
- Parameter descriptions
- Testable interface
---

## Future Improvements

- Persist Data to Relational Database
- Add support for multiple currency denominations
- External market data via REST API
---
