using HousingFinanceInterimApi.V1.Gateways.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Factories;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http;
using HousingFinanceInterimApi.V1.Infrastructure;
using HousingFinanceInterimApi.V1.Domain;

namespace HousingFinanceInterimApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/report")]
    [ApiVersion("1.0")]
    public class ReportController : BaseController
    {

        private readonly IReportChargesGateway _reportChargesGateway;
        private readonly IReportSuspenseAccountGateway _reportSuspenseAccountGateway;
        private readonly IReportCashImportGateway _reportCashImportGateway;
        private readonly IBatchReportAccountBalanceGateway _batchReportAccountBalanceGateway;

        public ReportController(IReportChargesGateway reportChargesGateway,
            IReportSuspenseAccountGateway reportSuspenseAccountGateway,
            IReportCashImportGateway reportCashImportGateway,
            IBatchReportAccountBalanceGateway batchReportAccountBalanceGateway)
        {
            _reportChargesGateway = reportChargesGateway;
            _reportSuspenseAccountGateway = reportSuspenseAccountGateway;
            _reportCashImportGateway = reportCashImportGateway;
            _batchReportAccountBalanceGateway = batchReportAccountBalanceGateway;
        }

        [ProducesResponseType(typeof(List<dynamic>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("charges")]
        public async Task<IActionResult> ListChargesByYearAndRentGroup(int year, string rentGroup, string group)
        {
            var data = new List<dynamic>();

            if (!string.IsNullOrEmpty(rentGroup))
            {
                data = (List<dynamic>) await _reportChargesGateway.ListByYearAndRentGroupAsync(year, rentGroup).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(group))
            {
                data = (List<dynamic>) await _reportChargesGateway.ListByGroupTypeAsync(year, group).ConfigureAwait(false);
            }
            else
            {
                data = (List<dynamic>) await _reportChargesGateway.ListByYearAsync(year).ConfigureAwait(false);
            }

            if (data.Count == 0)
                return NotFound();
            return Ok(data);
        }

        [ProducesResponseType(typeof(List<ReportCashSuspenseAccount>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("cash/suspense")]
        public async Task<IActionResult> ListCashSuspenseByYearAndType(int year, string suspenseAccountType)
        {
            var data = await _reportSuspenseAccountGateway
                .ListCashSuspenseByYearAndTypeAsync(year, suspenseAccountType).ConfigureAwait(false);

            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [ProducesResponseType(typeof(List<ReportCashImport>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("cash/import")]
        public async Task<IActionResult> ListCashImportByDate(DateTime startDate, DateTime endDate)
        {
            var data = await _reportCashImportGateway
                .ListCashImportByDateAsync(startDate, endDate).ConfigureAwait(false);

            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [ProducesResponseType(typeof(List<BatchReportAccountBalanceDomain>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("balance")]
        public async Task<IActionResult> CreateReportAccountBalance([FromBody] BatchReportAccountBalanceRequest request)
        {
            var data = await _batchReportAccountBalanceGateway
                .CreateAsync(request.ToDomain()).ConfigureAwait(false);

            return Created("Report request created",
                           data);
        }

        [ProducesResponseType(typeof(List<BatchReportAccountBalanceDomain>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("balance")]
        public async Task<IActionResult> ListReportAccountBalance()
        {
            var data = await _batchReportAccountBalanceGateway
                .ListAsync().ConfigureAwait(false);

            if (data == null)
                return NotFound();
            return Ok(data);
        }
    }
}
