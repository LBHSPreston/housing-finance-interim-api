using System.Threading;
using HousingFinanceInterimApi.V1.UseCase;
using Bogus;
using FluentAssertions;
using HousingFinanceInterimApi.V1.Boundary;
using Microsoft.Extensions.HealthChecks;
using Moq;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class DbHealthCheckUseCaseTests
    {

        //private Mock<IHealthCheckService> _mockHealthCheckService;
        //private DbHealthCheckUseCase _classUnderTest;

        //private readonly Faker _faker = new Faker();
        //private string _description;

        //[SetUp]
        //public void SetUp()
        //{
        //    _description = _faker.Random.Words();

        //    _mockHealthCheckService = new Mock<IHealthCheckService>();
        //    CompositeHealthCheckResult compositeHealthCheckResult = new CompositeHealthCheckResult(CheckStatus.Healthy);
        //    compositeHealthCheckResult.Add("test", CheckStatus.Healthy, _description);


        //    _mockHealthCheckService.Setup(s =>
        //            s.CheckHealthAsync(It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(compositeHealthCheckResult);

        //    _classUnderTest = new DbHealthCheckUseCase(_mockHealthCheckService.Object);
        //}

        //[Test]
        //public void ReturnsResponseWithStatus()
        //{
        //    HealthCheckResponse response = _classUnderTest.Execute();

        //    response.Should().NotBeNull();
        //    response.Success.Should().BeTrue();
        //    response.Message.Should().BeEquivalentTo("test: " + _description);
        //}
    }
}
