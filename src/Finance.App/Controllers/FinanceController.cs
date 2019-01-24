using System;
using System.Threading.Tasks;
using Akka.Actor;
using Finance.Core;
using Finance.Core.CardPayment;
using Finance.External.Ecom.Authorize;
using Microsoft.AspNetCore.Mvc;

namespace Finance.App.Controllers
{
    [ApiController]
    [Route("finance")]
    public class FinanceController : ControllerBase
    {
        private readonly ActorSystem system;

        public FinanceController(ActorSystem system)
        {
            this.system = system;
        }

        [HttpPost("cardpayment")]
        public async Task<IActionResult> Index()
        {
            var opId = Guid.NewGuid().ToString();
            var dto = await system.GetTransactionActor()
                .Ask<SuccessAuthorize>(new CardPaymentInit(opId, "card", DateTime.UtcNow), TimeSpan.FromSeconds(50));
            return Ok(dto);
        }
    }
}