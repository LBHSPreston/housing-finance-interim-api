using System;
using AutoMapper;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Factories;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingFinanceInterimApi.V1.Boundary.Response;
using HousingFinanceInterimApi.V1.Handlers;

namespace HousingFinanceInterimApi.V1.UseCase
{
    public class RefreshManageArrearsUseCase : IRefreshManageArrearsUseCase
    {
        private readonly IManageArrearsGateway _manageArrearsGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        public RefreshManageArrearsUseCase(IManageArrearsGateway manageArrearsGateway)
        {
            _manageArrearsGateway = manageArrearsGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"STARTING REFRESH MANAGE ARREARS CURRENT BALANCE");
            try
            {
                await _manageArrearsGateway.RefreshManageArrearsTenancyAgreement().ConfigureAwait(false);

                LoggingHandler.LogInfo($"END REFRESH MANAGE ARREARS CURRENT BALANCE");
                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(HousingFinanceInterimApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                LoggingHandler.LogError($"{namespaceLabel} APPLICATION ERROR");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
