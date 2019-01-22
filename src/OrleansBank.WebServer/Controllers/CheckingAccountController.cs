using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrleansBank.WebServer.Models;
using Orleans.Hosting;
using Orleans;
using OrleansBank.Contracts;

namespace OrleansBank.WebServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CheckingAccountController : Controller
    {
        private IClusterClient _client;
        public CheckingAccountController(IClusterClient client) 
        {
            _client = client;
        }
        [HttpGet("{accountId}/balance")]
        public async Task<IActionResult> GetBalance(string accountId)
        {
            var account = _client.GetGrain<ICheckingAccount>(accountId);
            return Ok(await account.GetBalance());
        }

        [HttpGet("{accountId}/transactions")]
        public async Task<IActionResult> GetTransactions(string accountId)
        {
            var account = _client.GetGrain<ICheckingAccount>(accountId);
            return Ok(await account.GetTransactions());
        }

        [HttpPost("test")]
        public async Task<IActionResult> Test()
        {
            for(int i = 0; i < 10000; i++)
            {
                var account = _client.GetGrain<ICheckingAccount>(i.ToString());
                account.GetTransactions();
            }
            return Ok();
        }
    }
}
