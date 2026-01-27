using AgtcSrvManagement.API.Controllers;
using AgtcSrvManagement.Application.Dtos;
using AgtcSrvManagement.Application.Exceptions;
using AgtcSrvManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AgtcSrvManagement.Test
{
    public class PropertyControllerTests
    {
        private readonly Mock<IPropertyService> _propertyServiceMock;
        private readonly Mock<ILogger<PropertyController>> _loggerMock;
        private readonly PropertyController _propertyController;

        public PropertyControllerTests()
        {
            _propertyServiceMock = new Mock<IPropertyService>();
            _loggerMock = new Mock<ILogger<PropertyController>>();

            _propertyController = new PropertyController(_loggerMock.Object, _propertyServiceMock.Object);
        }

        private void SetupControllerContext(Guid farmerId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, farmerId.ToString())
            }, "mock"));

            _propertyController.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };
        }

        #region GetPropertiesAsync Tests

        [Fact]
        public async Task GetPropertiesAsync_WithValidUser_ReturnsOkWithProperties()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var properties = new List<PropertyResponse>
            {
                new PropertyResponse { Id = Guid.NewGuid(), Name = "Property 1", Location = "Location 1", TotalArea = 100, OwnerId = farmerId, Fields = new List<FieldResponse>() },
                new PropertyResponse { Id = Guid.NewGuid(), Name = "Property 2", Location = "Location 2", TotalArea = 200, OwnerId = farmerId, Fields = new List<FieldResponse>() }
            };

            _propertyServiceMock.Setup(x => x.GetPropertiesAsync(farmerId)).ReturnsAsync(properties);

            // Act
            var result = await _propertyController.GetPropertiesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var returnedProperties = okResult.Value as IEnumerable<PropertyResponse>;
            Assert.Equal(2, returnedProperties.Count());
        }

        [Fact]
        public async Task GetPropertiesAsync_WithEmptyProperties_ReturnsOkWithEmptyList()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _propertyServiceMock.Setup(x => x.GetPropertiesAsync(farmerId)).ReturnsAsync(new List<PropertyResponse>());

            // Act
            var result = await _propertyController.GetPropertiesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProperties = okResult.Value as IEnumerable<PropertyResponse>;
            Assert.Empty(returnedProperties);
        }

        [Fact]
        public async Task GetPropertiesAsync_WhenServiceThrowsException_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _propertyServiceMock.Setup(x => x.GetPropertiesAsync(farmerId))
                .ThrowsAsync(new NotFoundException("Farmer not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyController.GetPropertiesAsync());
        }

        #endregion

        #region GetPropertyAsync Tests

        [Fact]
        public async Task GetPropertyAsync_WithValidPropertyId_ReturnsOkWithProperty()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var property = new PropertyResponse
            {
                Id = propertyId,
                Name = "Property 1",
                Location = "Location 1",
                TotalArea = 100,
                OwnerId = farmerId,
                Fields = new List<FieldResponse>()
            };

            _propertyServiceMock.Setup(x => x.GetPropertyAsync(farmerId, propertyId)).ReturnsAsync(property);

            // Act
            var result = await _propertyController.GetPropertyAsync(propertyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProperty = okResult.Value as PropertyResponse;
            Assert.NotNull(returnedProperty);
            Assert.Equal(propertyId, returnedProperty.Id);
            Assert.Equal("Property 1", returnedProperty.Name);
        }

        [Fact]
        public async Task GetPropertyAsync_WithNonExistentProperty_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _propertyServiceMock.Setup(x => x.GetPropertyAsync(farmerId, propertyId))
                .ThrowsAsync(new NotFoundException("Property not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyController.GetPropertyAsync(propertyId));
        }

        [Fact]
        public async Task GetPropertyAsync_WhenServiceThrowsException_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _propertyServiceMock.Setup(x => x.GetPropertyAsync(farmerId, propertyId))
                .ThrowsAsync(new NotFoundException("Not Found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyController.GetPropertyAsync(propertyId));
        }

        #endregion

        #region CreatePropertyAsync Tests

        [Fact]
        public async Task CreatePropertyAsync_WithValidRequest_ReturnsCreated()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var request = new CreatePropertyRequest
            {
                Name = "New Property",
                Location = "New Location"
            };

            _propertyServiceMock.Setup(x => x.CreatePropertyAsync(request, farmerId)).Returns(Task.CompletedTask);

            // Act
            var result = await _propertyController.CreatePropertyAsync(request);

            // Assert
            Assert.IsType<CreatedResult>(result);
            _propertyServiceMock.Verify(x => x.CreatePropertyAsync(request, farmerId), Times.Once);
        }

        [Fact]
        public async Task CreatePropertyAsync_WithInvalidRequest_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var request = new CreatePropertyRequest
            {
                Name = "New Property",
                Location = "New Location"
            };

            _propertyServiceMock.Setup(x => x.CreatePropertyAsync(request, farmerId))
                .ThrowsAsync(new NotFoundException("Farmer not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyController.CreatePropertyAsync(request));
        }

        [Fact]
        public async Task CreatePropertyAsync_WhenDatabaseError_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var request = new CreatePropertyRequest
            {
                Name = "New Property",
                Location = "New Location"
            };

            _propertyServiceMock.Setup(x => x.CreatePropertyAsync(request, farmerId))
                .ThrowsAsync(new ModifyDatabaseException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<ModifyDatabaseException>(() => _propertyController.CreatePropertyAsync(request));
        }

        #endregion

        #region AddFieldToPropertyAsync Tests

        [Fact]
        public async Task AddFieldToPropertyAsync_WithValidRequest_ReturnsCreated()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var request = new CreateFieldRequest
            {
                Name = "Field 1",
                CropType = "Corn",
                Area = 50
            };

            _propertyServiceMock.Setup(x => x.AddFieldToPropertyAsync(request, propertyId, farmerId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _propertyController.AddFieldToPropertyAsync(propertyId, request);

            // Assert
            Assert.IsType<CreatedResult>(result);
            _propertyServiceMock.Verify(x => x.AddFieldToPropertyAsync(request, propertyId, farmerId), Times.Once);
        }

        [Fact]
        public async Task AddFieldToPropertyAsync_WithNonExistentProperty_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var request = new CreateFieldRequest
            {
                Name = "Field 1",
                CropType = "Corn",
                Area = 50
            };

            _propertyServiceMock.Setup(x => x.AddFieldToPropertyAsync(request, propertyId, farmerId))
                .ThrowsAsync(new NotFoundException("Property not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyController.AddFieldToPropertyAsync(propertyId, request));
        }

        [Fact]
        public async Task AddFieldToPropertyAsync_WhenDatabaseError_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var request = new CreateFieldRequest
            {
                Name = "Field 1",
                CropType = "Corn",
                Area = 50
            };

            _propertyServiceMock.Setup(x => x.AddFieldToPropertyAsync(request, propertyId, farmerId))
                .ThrowsAsync(new ModifyDatabaseException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<ModifyDatabaseException>(() => _propertyController.AddFieldToPropertyAsync(propertyId, request));
        }

        [Fact]
        public async Task AddFieldToPropertyAsync_WhenUnexpectedError_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var request = new CreateFieldRequest
            {
                Name = "Field 1",
                CropType = "Corn",
                Area = 50
            };

            _propertyServiceMock.Setup(x => x.AddFieldToPropertyAsync(request, propertyId, farmerId))
                .ThrowsAsync(new UnexpectedException(new Exception("Unexpected error")));

            // Act & Assert
            await Assert.ThrowsAsync<UnexpectedException>(() => _propertyController.AddFieldToPropertyAsync(propertyId, request));
        }

        #endregion
    }

    public class SensorControllerTests
    {
        private readonly Mock<ISensorService> _sensorServiceMock;
        private readonly Mock<ILogger<SensorController>> _loggerMock;
        private readonly SensorController _sensorController;

        public SensorControllerTests()
        {
            _sensorServiceMock = new Mock<ISensorService>();
            _loggerMock = new Mock<ILogger<SensorController>>();

            _sensorController = new SensorController(_loggerMock.Object, _sensorServiceMock.Object);
        }

        private void SetupControllerContext(Guid farmerId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, farmerId.ToString())
            }, "mock"));

            _sensorController.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };
        }

        #region GetSensorsAsync Tests

        [Fact]
        public async Task GetSensorsAsync_WithValidUser_ReturnsOkWithSensors()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            var sensors = new List<SensorResponse>
            {
                new SensorResponse { Id = Guid.NewGuid(), Serial = "SENSOR001", SensorType = Domain.Enums.SensorType.Temperatura, OwnerId = farmerId, FieldId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new SensorResponse { Id = Guid.NewGuid(), Serial = "SENSOR002", SensorType = Domain.Enums.SensorType.Umidade, OwnerId = farmerId, FieldId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
            };

            _sensorServiceMock.Setup(x => x.GetSensorsAsync(farmerId)).ReturnsAsync(sensors);

            // Act
            var result = await _sensorController.GetSensorsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var returnedSensors = okResult.Value as IEnumerable<SensorResponse>;
            Assert.Equal(2, returnedSensors.Count());
        }

        [Fact]
        public async Task GetSensorsAsync_WithEmptySensors_ReturnsOkWithEmptyList()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _sensorServiceMock.Setup(x => x.GetSensorsAsync(farmerId)).ReturnsAsync(new List<SensorResponse>());

            // Act
            var result = await _sensorController.GetSensorsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSensors = okResult.Value as IEnumerable<SensorResponse>;
            Assert.Empty(returnedSensors);
        }

        [Fact]
        public async Task GetSensorsAsync_WhenServiceThrowsException_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _sensorServiceMock.Setup(x => x.GetSensorsAsync(farmerId))
                .ThrowsAsync(new NotFoundException("Farmer not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sensorController.GetSensorsAsync());
        }

        [Fact]
        public async Task GetSensorsAsync_WhenUnexpectedError_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _sensorServiceMock.Setup(x => x.GetSensorsAsync(farmerId))
                .ThrowsAsync(new UnexpectedException(new Exception("Database error")));

            // Act & Assert
            await Assert.ThrowsAsync<UnexpectedException>(() => _sensorController.GetSensorsAsync());
        }

        #endregion

        #region DeleteSensorsAsync Tests

        [Fact]
        public async Task DeleteSensorsAsync_WithValidSensorId_ReturnsNoContent()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _sensorServiceMock.Setup(x => x.DeleteSensorAsync(farmerId, sensorId)).Returns(Task.CompletedTask);

            // Act
            var result = await _sensorController.DeleteSensorsAsync(sensorId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _sensorServiceMock.Verify(x => x.DeleteSensorAsync(farmerId, sensorId), Times.Once);
        }

        [Fact]
        public async Task DeleteSensorsAsync_WithNonExistentSensor_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _sensorServiceMock.Setup(x => x.DeleteSensorAsync(farmerId, sensorId))
                .ThrowsAsync(new NotFoundException("Sensor not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sensorController.DeleteSensorsAsync(sensorId));
        }

        [Fact]
        public async Task DeleteSensorsAsync_WhenDatabaseError_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _sensorServiceMock.Setup(x => x.DeleteSensorAsync(farmerId, sensorId))
                .ThrowsAsync(new ModifyDatabaseException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<ModifyDatabaseException>(() => _sensorController.DeleteSensorsAsync(sensorId));
        }

        [Fact]
        public async Task DeleteSensorsAsync_WhenUnexpectedError_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _sensorServiceMock.Setup(x => x.DeleteSensorAsync(farmerId, sensorId))
                .ThrowsAsync(new UnexpectedException(new Exception("Unexpected error")));

            // Act & Assert
            await Assert.ThrowsAsync<UnexpectedException>(() => _sensorController.DeleteSensorsAsync(sensorId));
        }

        [Fact]
        public async Task DeleteSensorsAsync_WhenHttpExceptionThrown_PropagatesException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensorId = Guid.NewGuid();
            SetupControllerContext(farmerId);

            _sensorServiceMock.Setup(x => x.DeleteSensorAsync(farmerId, sensorId))
                .ThrowsAsync(new HttpException(500, "Error", "Something went wrong"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpException>(() => _sensorController.DeleteSensorsAsync(sensorId));
        }

        #endregion
    }
}
