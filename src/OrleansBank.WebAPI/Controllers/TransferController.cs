using Microsoft.AspNetCore.Mvc;
using OrleansBank.UseCases.Ports.In;

namespace OrleansBank.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly ILogger<TransferController> _logger;
        private readonly ITransferMoneyUseCase _useCase;

        public TransferController(ILogger<TransferController> logger,
            ITransferMoneyUseCase useCase)
        {
            _logger = logger;
            _useCase = useCase;
        }

        [HttpPost]
        public async Task<IActionResult> MakeTransfer(TransferRequest transfer)
        {
            var response = await _useCase.Execute(transfer);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}