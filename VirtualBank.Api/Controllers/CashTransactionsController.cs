using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;
using VirtualBank.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CashTransactionsController : ControllerBase
    {
        private readonly ICashTransactionService _cashTransactionService;

        public CashTransactionsController(ICashTransactionService cashTransactionService)
        {
            _cashTransactionService = cashTransactionService;
        }

        // GET api/values/5
        [HttpGet("get/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCashTransactionsByAccountNo([FromRoute] string accountNo, [FromQuery] int lastDays,
                                                                        CancellationToken cancellationToken = default)
        {
            try
            {
               return Ok(await _cashTransactionService.GetCashTransactionsByAccountNo(accountNo, lastDays, cancellationToken));
                
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // POST api/values
        [HttpPost]
        [Route("post")]
        public async Task<IActionResult> PostCashTransaction([FromBody] CreateCashTransactionRequest request,
                                                                        CancellationToken cancellationToken = default)
        {
            try
            {
                await _cashTransactionService.CreateCashTransaction(request, cancellationToken);

                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest();
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
