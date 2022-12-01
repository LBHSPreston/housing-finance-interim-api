using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Http;
using HousingFinanceInterimApi.V1.Infrastructure;
using System.Collections.Generic;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tenancy")]
    [ApiVersion("1.0")]
    public class TenancyController : BaseController
    {

        private readonly ITenancyGateway _gateway;
        private readonly IUPHousingCashLoadGateway _uPHousingCashLoadGateway;

        public TenancyController(ITenancyGateway gateway, IUPHousingCashLoadGateway uPHousingCashLoadGateway)
        {
            _gateway = gateway;
            _uPHousingCashLoadGateway = uPHousingCashLoadGateway;
        }

        [ProducesResponseType(typeof(List<TenancyTransaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(string tenancyAgreementRef, string rentAccount, string householdRef)
        {
            var data = await _gateway.GetAsync(tenancyAgreementRef, rentAccount, householdRef).ConfigureAwait(false);
            if (data == null)
                return NotFound();

            var academyClaimRefs = await _uPHousingCashLoadGateway.GetAcademyRefByRentAccount(data.RentAccount).ConfigureAwait(false);
            data.AcademyClaimRefs = academyClaimRefs;

            return Ok(data);
        }

        [ProducesResponseType(typeof(List<TenancyTransaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("transaction")]
        public async Task<IActionResult> GetTransactions(string tenancyAgreementRef, string rentAccount, string householdRef,
            int count, string order, DateTime startDate, DateTime endDate)
        {
            var data = new List<TenancyTransaction>();

            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                data = (List<TenancyTransaction>) await _gateway
                    .GetTransactionsByDateAsync(tenancyAgreementRef, rentAccount, householdRef, startDate, endDate)
                    .ConfigureAwait(false);
            else
                data = (List<TenancyTransaction>) await _gateway.GetTransactionsAsync(tenancyAgreementRef, rentAccount, householdRef, count, order)
                .ConfigureAwait(false);

            if (data == null)
                return NotFound();
            return Ok(data);
        }

    }

}
