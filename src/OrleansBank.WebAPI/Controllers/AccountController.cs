using Microsoft.AspNetCore.Mvc;
using OrleansBank.UseCases.Ports.In;

namespace OrleansBank.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IGetBalanceUseCase _useCase;

        public AccountController(ILogger<AccountController> logger,
            IGetBalanceUseCase useCase)
        {
            _logger = logger;
            _useCase = useCase;
        }

        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(string id)
        {
            double balance = await _useCase.Execute(id);
            return Ok(balance);
        }
    }
}
