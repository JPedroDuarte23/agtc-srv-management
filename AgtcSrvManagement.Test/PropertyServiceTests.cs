using AgtcSrvManagement.Application.Dtos;
using AgtcSrvManagement.Application.Exceptions;
using AgtcSrvManagement.Application.Interfaces;
using AgtcSrvManagement.Application.Services;
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
    public class PropertyServiceTests
    {
        private readonly Mock<IFarmerRepository> _farmerRepositoryMock;
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<ILogger<PropertyService>> _loggerMock;
        private readonly PropertyService _propertyService;

        public PropertyServiceTests()
        {
            _farmerRepositoryMock = new Mock<IFarmerRepository>();
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _loggerMock = new Mock<ILogger<PropertyService>>();

            _propertyService = new PropertyService(
                _farmerRepositoryMock.Object,
                _loggerMock.Object,
                _propertyRepositoryMock.Object
            );
        }

        #region GetPropertiesAsync Tests

        [Fact]
        public async Task GetPropertiesAsync_WithValidFarmerId_ReturnsProperties()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var properties = new List<Property>
            {
                new Property { Id = Guid.NewGuid(), Name = "Property 1", Location = "Location 1", TotalArea = 100, OwnerId = farmerId, Fields = new List<Field>() },
                new Property { Id = Guid.NewGuid(), Name = "Property 2", Location = "Location 2", TotalArea = 200, OwnerId = farmerId, Fields = new List<Field>() }
            };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.GetPropertiesByOwnerId(farmerId)).ReturnsAsync(properties);

            // Act
            var result = await _propertyService.GetPropertiesAsync(farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Property 1", result.First().Name);
            _farmerRepositoryMock.Verify(x => x.GetFarmerByIdAsync(farmerId), Times.Once);
            _propertyRepositoryMock.Verify(x => x.GetPropertiesByOwnerId(farmerId), Times.Once);
        }

        [Fact]
        public async Task GetPropertiesAsync_WithInvalidFarmerId_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync((Farmer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyService.GetPropertiesAsync(farmerId));
        }

        [Fact]
        public async Task GetPropertiesAsync_WithEmptyProperties_ReturnsEmptyList()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.GetPropertiesByOwnerId(farmerId)).ReturnsAsync(new List<Property>());

            // Act
            var result = await _propertyService.GetPropertiesAsync(farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetPropertyAsync Tests

        [Fact]
        public async Task GetPropertyAsync_WithValidPropertyId_ReturnsProperty()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var property = new Property { Id = propertyId, Name = "Property 1", Location = "Location 1", TotalArea = 100, OwnerId = farmerId, Fields = new List<Field>() };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.GetPropertyByIdAsync(propertyId)).ReturnsAsync(property);

            // Act
            var result = await _propertyService.GetPropertyAsync(propertyId, farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(propertyId, result.Id);
            Assert.Equal("Property 1", result.Name);
        }

        [Fact]
        public async Task GetPropertyAsync_WithInvalidFarmerId_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync((Farmer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyService.GetPropertyAsync(propertyId, farmerId));
        }

        [Fact]
        public async Task GetPropertyAsync_WithNonExistentProperty_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.GetPropertyByIdAsync(propertyId)).ReturnsAsync((Property)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyService.GetPropertyAsync(propertyId, farmerId));
        }

        [Fact]
        public async Task GetPropertyAsync_WithDifferentOwner_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var otherFarmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var property = new Property { Id = propertyId, Name = "Property 1", Location = "Location 1", TotalArea = 100, OwnerId = otherFarmerId, Fields = new List<Field>() };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.GetPropertyByIdAsync(propertyId)).ReturnsAsync(property);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyService.GetPropertyAsync(propertyId, farmerId));
        }

        #endregion

        #region CreatePropertyAsync Tests

        [Fact]
        public async Task CreatePropertyAsync_WithValidRequest_CreatesProperty()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var request = new CreatePropertyRequest { Name = "New Property", Location = "New Location" };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.CreateProperty(It.IsAny<Property>())).Returns(Task.CompletedTask);

            // Act
            await _propertyService.CreatePropertyAsync(request, farmerId);

            // Assert
            _propertyRepositoryMock.Verify(x => x.CreateProperty(It.IsAny<Property>()), Times.Once);
            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Propriedade")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task CreatePropertyAsync_WithInvalidFarmerId_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var request = new CreatePropertyRequest { Name = "New Property", Location = "New Location" };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync((Farmer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyService.CreatePropertyAsync(request, farmerId));
        }

        [Fact]
        public async Task CreatePropertyAsync_WhenDatabaseThrowsException_ThrowsModifyDatabaseException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var request = new CreatePropertyRequest { Name = "New Property", Location = "New Location" };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.CreateProperty(It.IsAny<Property>())).ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            await Assert.ThrowsAsync<ModifyDatabaseException>(() => _propertyService.CreatePropertyAsync(request, farmerId));
            _loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Erro")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        #endregion

        #region AddFieldToPropertyAsync Tests

        [Fact]
        public async Task AddFieldToPropertyAsync_WithValidRequest_AddsField()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var property = new Property { Id = propertyId, Name = "Property 1", Location = "Location 1", TotalArea = 100, OwnerId = farmerId, Fields = new List<Field>() };
            var request = new CreateFieldRequest { Name = "Field 1", CropType = "Corn", Area = 50 };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.GetPropertyByIdAsync(propertyId)).ReturnsAsync(property);
            _propertyRepositoryMock.Setup(x => x.UpdateProperty(It.IsAny<Property>())).Returns(Task.CompletedTask);

            // Act
            await _propertyService.AddFieldToPropertyAsync(request, propertyId, farmerId);

            // Assert
            _propertyRepositoryMock.Verify(x => x.UpdateProperty(It.IsAny<Property>()), Times.Once);
            _loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Talhão")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task AddFieldToPropertyAsync_WithInvalidFarmerId_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var request = new CreateFieldRequest { Name = "Field 1", CropType = "Corn", Area = 50 };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync((Farmer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyService.AddFieldToPropertyAsync(request, propertyId, farmerId));
        }

        [Fact]
        public async Task AddFieldToPropertyAsync_WithNonExistentProperty_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var request = new CreateFieldRequest { Name = "Field 1", CropType = "Corn", Area = 50 };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.GetPropertyByIdAsync(propertyId)).ReturnsAsync((Property)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyService.AddFieldToPropertyAsync(request, propertyId, farmerId));
        }

        [Fact]
        public async Task AddFieldToPropertyAsync_WithDifferentOwner_ThrowsNotFoundException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var otherFarmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var property = new Property { Id = propertyId, Name = "Property 1", Location = "Location 1", TotalArea = 100, OwnerId = otherFarmerId, Fields = new List<Field>() };
            var request = new CreateFieldRequest { Name = "Field 1", CropType = "Corn", Area = 50 };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.GetPropertyByIdAsync(propertyId)).ReturnsAsync(property);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _propertyService.AddFieldToPropertyAsync(request, propertyId, farmerId));
        }

        [Fact]
        public async Task AddFieldToPropertyAsync_WhenDatabaseThrowsException_ThrowsModifyDatabaseException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var farmer = new Farmer { Id = farmerId, Name = "Test Farmer", Email = "test@test.com", PasswordHash = "hash" };
            var property = new Property { Id = propertyId, Name = "Property 1", Location = "Location 1", TotalArea = 100, OwnerId = farmerId, Fields = new List<Field>() };
            var request = new CreateFieldRequest { Name = "Field 1", CropType = "Corn", Area = 50 };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ReturnsAsync(farmer);
            _propertyRepositoryMock.Setup(x => x.GetPropertyByIdAsync(propertyId)).ReturnsAsync(property);
            _propertyRepositoryMock.Setup(x => x.UpdateProperty(It.IsAny<Property>())).ThrowsAsync(new Exception("DB Error"));

            // Act & Assert
            await Assert.ThrowsAsync<ModifyDatabaseException>(() => _propertyService.AddFieldToPropertyAsync(request, propertyId, farmerId));
        }

        [Fact]
        public async Task AddFieldToPropertyAsync_WhenFarmerValidationThrowsException_ThrowsUnexpectedException()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var propertyId = Guid.NewGuid();
            var request = new CreateFieldRequest { Name = "Field 1", CropType = "Corn", Area = 50 };

            _farmerRepositoryMock.Setup(x => x.GetFarmerByIdAsync(farmerId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            await Assert.ThrowsAsync<UnexpectedException>(() => _propertyService.AddFieldToPropertyAsync(request, propertyId, farmerId));
        }

        #endregion
    }
}
