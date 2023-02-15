using System;
using HousingFinanceInterimApi.V1.Domain;
using HousingFinanceInterimApi.V1.Gateways;
using HousingFinanceInterimApi.V1.Gateways.Interface;
using HousingFinanceInterimApi.V1.Handlers;
using HousingFinanceInterimApi.V1.UseCase;
using HousingFinanceInterimApi.Tests.V1.TestHelpers;
using Moq;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Drive.v3.Data;

namespace HousingFinanceInterimApi.Tests.V1.UseCase
{
    public class GenerateRentPositionUseCaseTests
    {
        private readonly Mock<IRentPositionGateway> _mockRentPositionGateway;
        private readonly Mock<IBatchLogGateway> _mockBatchLogGateway;
        private readonly Mock<IBatchLogErrorGateway> _mockBatchLogErrorGateway;
        private readonly Mock<IGoogleFileSettingGateway> _mockGoogleFileSettingGateway;
        private readonly Mock<IGoogleClientService> _mockGoogleClientService;

        private GenerateRentPositionUseCase _classUnderTest;

        public GenerateRentPositionUseCaseTests()
        {
            _mockRentPositionGateway = new Mock<IRentPositionGateway>();
            _mockBatchLogGateway = new Mock<IBatchLogGateway>();
            _mockBatchLogErrorGateway = new Mock<IBatchLogErrorGateway>();
            _mockGoogleFileSettingGateway = new Mock<IGoogleFileSettingGateway>();
            _mockGoogleClientService = new Mock<IGoogleClientService>();

            var waitDurationEnvVar = 30;
            Environment.SetEnvironmentVariable("WAIT_DURATION", waitDurationEnvVar.ToString());

            _classUnderTest = new GenerateRentPositionUseCase
                (
                    _mockRentPositionGateway.Object,
                    _mockBatchLogGateway.Object,
                    _mockBatchLogErrorGateway.Object,
                    _mockGoogleFileSettingGateway.Object,
                    _mockGoogleClientService.Object
                );
        }

        [Fact]
        public async Task ExecuteAsyncThrowsErrorWhenFileSettingIsRentPositionLabelAndExceptionIsNotNull()
        {
            // Arrange
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<File>());

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(false);

            // Act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<Exception>().WithMessage("Failed to upload to Rent Position folder (Qlik)").ConfigureAwait(false);
        }

        [Fact]
        public async Task ExecuteAsyncThrowsErrorWhenFileSettingIsRentPositionBkpLabelAndExceptionIsNotNull()
        {
            // Arrange
            var rentPositionFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPositionBkpFileSettings = RandomGen.CreateMany<GoogleFileSettingDomain>(quantity: 1).ToList();
            var rentPosition = ConstantsGen.RentPositionLabel;
            var rentPositionBkp = ConstantsGen.RentPositionBkpLabel;

            _mockBatchLogGateway
                .Setup(g => g.CreateAsync(It.Is<string>(s => s == rentPosition), It.IsAny<bool>()))
                .ReturnsAsync(RandomGen.BatchLogDomain());

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPosition)))
                .ReturnsAsync(rentPositionFileSettings);

            _mockGoogleFileSettingGateway
                .Setup(g => g.GetSettingsByLabel(It.Is<string>(s => s == rentPositionBkp)))
                .ReturnsAsync(rentPositionBkpFileSettings);

            _mockRentPositionGateway
                .Setup(g => g.GetRentPosition())
                .ReturnsAsync(RandomGen.RentPositionCsvRepresentation());

            _mockGoogleClientService
                .Setup(x => x.GetFilesInDriveAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<File>());

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(true);

            _mockGoogleClientService
                .Setup(x => x.UploadCsvFile(
                    It.IsAny<List<string[]>>(),
                    It.IsAny<string>(),
                    It.Is<string>(s => s == rentPositionBkpFileSettings.First().GoogleIdentifier)))
                .ReturnsAsync(false);

            // Act
            Func<Task> useCaseCall = async () => await _classUnderTest.ExecuteAsync().ConfigureAwait(false);

            // Assert
            await useCaseCall.Should().ThrowAsync<Exception>().WithMessage("Failed to upload to Rent Position folder (Backup)").ConfigureAwait(false);
        }
    }
}
