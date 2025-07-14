using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Interfaces;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/portfolios/{portfolioId}/performance")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IPerformanceEngine _performanceEngine;

        public PerformanceController(IPortfolioService portfolioService, IPerformanceEngine performanceEngine)
        {
            _portfolioService = portfolioService;
            _performanceEngine = performanceEngine;
        }

        // GET Total Value Over Time
        [HttpGet("value-over-time")]
        public async Task<IActionResult> GetValueOverTime(string portfolioId, DateTime start, DateTime end)
        {
            var portfolio = await _portfolioService.GetByIdAsync(portfolioId);
            if (portfolio == null) return NotFound($"Portfolio {portfolioId} not found.");

            var result = await _performanceEngine.GetValueOverTimeAsync(portfolio, start, end);
            return Ok(result);
        }

        // GET Asset Allocation Breakdown
        [HttpGet("allocation")]
        public async Task<IActionResult> GetAllocation(string portfolioId, DateTime date)
        {
            var portfolio = await _portfolioService.GetByIdAsync(portfolioId);
            if (portfolio == null) return NotFound($"Portfolio {portfolioId} not found.");

            var allocation = await _performanceEngine.GetAllocationAsync(portfolio, date);
            return Ok(allocation);
        }

        // GET Realized Gain/Loss
        [HttpGet("realized")]
        public async Task<IActionResult> GetRealizedGain(string portfolioId, DateTime start, DateTime end)
        {
            var portfolio = await _portfolioService.GetByIdAsync(portfolioId);
            if (portfolio == null) return NotFound($"Portfolio {portfolioId} not found.");

            var gain = await _performanceEngine.GetRealizedGainsAsync(portfolio, start, end);
            return Ok(gain);
        }

        // GET Unrealized Gain/Loss
        [HttpGet("unrealized")]
        public async Task<IActionResult> GetUnrealizedGain(string portfolioId, DateTime date)
        {
            var portfolio = await _portfolioService.GetByIdAsync(portfolioId);
            if (portfolio == null) return NotFound($"Portfolio {portfolioId} not found.");

            var gain = await _performanceEngine.GetUnrealizedGainsAsync(portfolio, date);
            return Ok(gain);
        }
    }
}
