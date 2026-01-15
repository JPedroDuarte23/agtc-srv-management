using AgtcSrvManagement.Application.Dtos;
using AgtcSrvManagement.Application.Exceptions;
using AgtcSrvManagement.Application.Interfaces;
using AgtcSrvManagement.Application.Services;
using AgtcSrvManagement.Domain.Enums;
using AgtcSrvManagement.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AgtcSrvManagement.Test
{
    public class SensorServiceTests
    {
        private readonly Mock<IFarmerRepository> _farmerRepositoryMock;
        private readonly Mock<ISensorRepository> _sensorRepositoryMock;
        private readonly Mock<ILogger<SensorService>> _loggerMock;
        private readonly SensorService _sensorService;

        public SensorServiceTests()
        {
            _farmerRepositoryMock = new Mock<IFarmerRepository>();
            _sensorRepositoryMock = new Mock<ISensorRepository>();
            _loggerMock = new Mock<ILogger<SensorService>>();

            _sensorService = new SensorService(
                _farmerRepositoryMock.Object,
                _loggerMock.Object,
                _sensorRepositoryMock.Object
            );
        }

        #region GetSensorsAsync Tests

        [Fact]
        public async Task GetSensorsAsync_WithValidFarmerId_ReturnsSensors()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var sensors = new List<Sensor>
            {
                new Sensor { Id = Guid.NewGuid(), Serial = "SENSOR001", SensorType = SensorType.Temperatura, OwnerId = farmerId, FieldId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new Sensor { Id = Guid.NewGuid(), Serial = "SENSOR002", SensorType = SensorType.Umidade, OwnerId = farmerId, FieldId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
            };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _sensorRepositoryMock.Setup(x => x.GetSensorsByFarmerIdAsync(farmerId)).ReturnsAsync(sensors);

            // Act
            var result = await _sensorService.GetSensorsAsync(farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("SENSOR001", result.First().Serial);
            _farmerRepositoryMock.Verify(x => x.GetFarmerByIdAsync(farmerId), Times.Once);
            _sensorRepositoryMock.Verify(x => x.GetSensorsByFarmerIdAsync(farmerId), Times.Once);
        }

        [Fact]
        public async Task GetSensorsAsync_WithInvalidFarmerId_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync((Farmer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sensorService.GetSensorsAsync(farmerId));
        }

        [Fact]
        public async Task GetSensorsAsync_WithEmptySensors_ReturnsEmptyList()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _sensorRepositoryMock.Setup(x => x.GetSensorsByFarmerIdAsync(farmerId)).ReturnsAsync(new List<Sensor>());

            // Act
            var result = await _sensorService.GetSensorsAsync(farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSensorsAsync_WhenRepositoryThrowsException_ThrowsUnexpectedException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _sensorRepositoryMock.Setup(x => x.GetSensorsByFarmerIdAsync(farmerId)).ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<UnexpectedException>(() => _sensorService.GetSensorsAsync(farmerId));
        }

        [Fact]
        public async Task GetSensorsAsync_WithMultipleSensorTypes_ReturnsSensorsDtos()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var fieldId = Guid.NewGuid();
            var sensors = new List<Sensor>
            {
                new Sensor { Id = Guid.NewGuid(), Serial = "SENSOR001", SensorType = SensorType.Temperatura, OwnerId = farmerId, FieldId = fieldId, CreatedAt = DateTime.UtcNow },
                new Sensor { Id = Guid.NewGuid(), Serial = "SENSOR002", SensorType = SensorType.Umidade, OwnerId = farmerId, FieldId = fieldId, CreatedAt = DateTime.UtcNow },
                new Sensor { Id = Guid.NewGuid(), Serial = "SENSOR003", SensorType = SensorType.Pressao, OwnerId = farmerId, FieldId = fieldId, CreatedAt = DateTime.UtcNow }
            };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _sensorRepositoryMock.Setup(x => x.GetSensorsByFarmerIdAsync(farmerId)).ReturnsAsync(sensors);

            // Act
            var result = await _sensorService.GetSensorsAsync(farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        #endregion

        #region DeleteSensorAsync Tests

        [Fact]
        public async Task DeleteSensorAsync_WithValidSensorId_DeletesSensor()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var sensor = new Sensor { Id = sensorId, Serial = "SENSOR001", SensorType = SensorType.Temperatura, OwnerId = farmerId, FieldId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _sensorRepositoryMock.Setup(x => x.GetSensorByIdAsync(sensorId)).ReturnsAsync(sensor);
            _sensorRepositoryMock.Setup(x => x.DeleteSensorById(sensorId)).Returns(Task.CompletedTask);

            // Act
            await _sensorService.DeleteSensorAsync(farmerId, sensorId);

            // Assert
            _sensorRepositoryMock.Verify(x => x.DeleteSensorById(sensorId), Times.Once);
        }

        [Fact]
        public async Task DeleteSensorAsync_WithInvalidFarmerId_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync((Farmer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sensorService.DeleteSensorAsync(farmerId, sensorId));
        }

        [Fact]
        public async Task DeleteSensorAsync_WithNonExistentSensor_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _sensorRepositoryMock.Setup(x => x.GetSensorByIdAsync(sensorId)).ReturnsAsync((Sensor)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sensorService.DeleteSensorAsync(farmerId, sensorId));
        }

        [Fact]
        public async Task DeleteSensorAsync_WhenRepositoryThrowsHttpException_ThrowsHttpException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var sensor = new Sensor { Id = sensorId, Serial = "SENSOR001", SensorType = SensorType.Temperatura, OwnerId = farmerId, FieldId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _sensorRepositoryMock.Setup(x => x.GetSensorByIdAsync(sensorId)).ReturnsAsync(sensor);
            _sensorRepositoryMock.Setup(x => x.DeleteSensorById(sensorId)).ThrowsAsync(new HttpException(500, "Error", "Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpException>(() => _sensorService.DeleteSensorAsync(farmerId, sensorId));
        }

        [Fact]
        public async Task DeleteSensorAsync_WhenRepositoryThrowsGeneralException_ThrowsUnexpectedException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var sensor = new Sensor { Id = sensorId, Serial = "SENSOR001", SensorType = SensorType.Temperatura, OwnerId = farmerId, FieldId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _sensorRepositoryMock.Setup(x => x.GetSensorByIdAsync(sensorId)).ReturnsAsync(sensor);
            _sensorRepositoryMock.Setup(x => x.DeleteSensorById(sensorId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            await Assert.ThrowsAsync<UnexpectedException>(() => _sensorService.DeleteSensorAsync(farmerId, sensorId));
        }

        [Fact]
        public async Task DeleteSensorAsync_WhenFarmerValidationThrowsUnexpectedException_ThrowsUnexpectedException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            await Assert.ThrowsAsync<UnexpectedException>(() => _sensorService.DeleteSensorAsync(farmerId, sensorId));
        }

        #endregion
    }
}
