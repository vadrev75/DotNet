using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Interfaces;
using PortfolioApi.Models;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/portfolios")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfolioService _service;
        private readonly IPerformanceEngine _engine;

        public PortfoliosController(IPortfolioService service, IPerformanceEngine engine)
        {
            _service = service;
            _engine = engine;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Portfolio portfolio)
        {
            var result = await _service.CreateAsync(portfolio);
            return CreatedAtAction(nameof(GetById), new { id = result.PortfolioId }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var p = await _service.GetByIdAsync(id);
            return p == null ? NotFound() : Ok(p);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/assets")]
        public async Task<IActionResult> AddAsset(string id, [FromBody] Asset asset) =>
            Ok(await _service.AddAssetAsync(id, asset));

        [HttpPost("{id}/transactions")]
        public async Task<IActionResult> AddTransaction(string id, [FromBody] Transaction txn) =>
            Ok(await _service.AddTransactionAsync(id, txn));

        [HttpGet("{id}/transactions")]
        public async Task<IActionResult> GetTransactions(string id) =>
            Ok(await _service.GetTransactionsAsync(id));

        [HttpDelete("{id}/transactions/{txnId}")]
        public async Task<IActionResult> DeleteTransaction(string id, long txnId)
        {
            await _service.DeleteTransactionAsync(id, txnId);
            return NoContent();
        }
    }
}
