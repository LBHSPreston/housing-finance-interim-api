using HousingFinanceInterimApi.V1.Boundary.Request;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HousingFinanceInterimApi.V1.Gateways
{
    public class UpdateTAGateway : IUpdateTAGateway
    {
        private readonly DatabaseContext _context;

        public UpdateTAGateway(DatabaseContext context)
        {
            _context = context;
        }

        public async Task UpdateTADetails(string tagRef, UpdateTADomain request)
        {
            try
            {
                var uhTenancyAgreement = _context.UHTenancyAgreement.SingleOrDefault(p => p.TenancyAgreementRef == tagRef);
                var maTenancyAgreement = _context.MATenancyAgreement.SingleOrDefault(p => p.TenancyAgreementRef == tagRef);

                LoggingHandler.LogInfo($"HELLO {uhTenancyAgreement}");
                if (uhTenancyAgreement is not null && maTenancyAgreement is not null)
                {
                    LoggingHandler.LogInfo($"uhTA eot value is {uhTenancyAgreement.EndOfTenancy}");
                    uhTenancyAgreement.EndOfTenancy = request.TenureEndDate;
                    maTenancyAgreement.EndOfTenancy = request.TenureEndDate;
                    LoggingHandler.LogInfo($"uhTA eot value has been changed to {uhTenancyAgreement.EndOfTenancy}");
                }

                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                LoggingHandler.LogError(ex.Message);
                LoggingHandler.LogError(ex.StackTrace);
                LoggingHandler.LogError($"Unable to upload tag_ref {tagRef} with this end date {request.TenureEndDate} as requested");
                throw;
            }
        }

    }
}
