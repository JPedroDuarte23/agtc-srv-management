using AgtcSrvManagement.Domain.Models;
using AgtcSrvManagement.Infrastructure.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AgtcSrvManagement.Test
{
    #region PropertyRepository Tests

    public class PropertyRepositoryTests
    {
        private readonly Mock<IMongoDatabase> _databaseMock;
        private readonly Mock<IMongoCollection<Property>> _collectionMock;
        private readonly PropertyRepository _repository;

        public PropertyRepositoryTests()
        {
            _databaseMock = new Mock<IMongoDatabase>();
            _collectionMock = new Mock<IMongoCollection<Property>>();

            _databaseMock.Setup(db => db.GetCollection<Property>("properties", null))
                .Returns(_collectionMock.Object);

            _repository = new PropertyRepository(_databaseMock.Object);
        }

        [Fact]
        public async Task CreateProperty_WithValidProperty_InsertsProperty()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(),
                Name = "Test Property",
                Location = "Test Location",
                TotalArea = 100,
                OwnerId = Guid.NewGuid(),
                Fields = new List<Field>()
            };

            // Act
            await _repository.CreateProperty(property);

            // Assert
            _collectionMock.Verify(
                c => c.InsertOneAsync(property, It.IsAny<InsertOneOptions>(), default),
                Times.Once);
        }

        [Fact]
        public async Task GetPropertiesByOwnerId_WithValidFarmerId_ReturnsProperties()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var properties = new List<Property>
            {
                new Property { Id = Guid.NewGuid(), Name = "Property 1", Location = "Location 1", TotalArea = 100, OwnerId = farmerId, Fields = new List<Field>() },
                new Property { Id = Guid.NewGuid(), Name = "Property 2", Location = "Location 2", TotalArea = 200, OwnerId = farmerId, Fields = new List<Field>() }
            };

            var cursorMock = new Mock<IAsyncCursor<Property>>();
            cursorMock.Setup(c => c.Current).Returns(properties);
            cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property, Property>>(),
                default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetPropertiesByOwnerId(farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetPropertiesByOwnerId_WithNoProperties_ReturnsEmptyList()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var emptyList = new List<Property>();

            var cursorMock = new Mock<IAsyncCursor<Property>>();
            cursorMock.Setup(c => c.Current).Returns(emptyList);
            cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property, Property>>(),
                default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetPropertiesByOwnerId(farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_WithValidPropertyId_ReturnsProperty()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var property = new Property
            {
                Id = propertyId,
                Name = "Test Property",
                Location = "Test Location",
                TotalArea = 100,
                OwnerId = Guid.NewGuid(),
                Fields = new List<Field>()
            };

            var cursorMock = new Mock<IAsyncCursor<Property>>();
            cursorMock.Setup(c => c.Current).Returns(new List<Property> { property });
            cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property, Property>>(),
                default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetPropertyByIdAsync(propertyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(propertyId, result.Id);
            Assert.Equal("Test Property", result.Name);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_WithNonExistentPropertyId_ReturnsNull()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            var emptyList = new List<Property>();

            var cursorMock = new Mock<IAsyncCursor<Property>>();
            cursorMock.Setup(c => c.Current).Returns(emptyList);
            cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Property>>(),
                It.IsAny<FindOptions<Property, Property>>(),
                default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetPropertyByIdAsync(propertyId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProperty_WithValidProperty_UpdatesProperty()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(),
                Name = "Updated Property",
                Location = "Updated Location",
                TotalArea = 150,
                OwnerId = Guid.NewGuid(),
                Fields = new List<Field>()
            };

            var replaceResultMock = new Mock<ReplaceOneResult>();
            _collectionMock.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Property>>(),
                property,
                It.IsAny<ReplaceOptions>(),
                default))
                .ReturnsAsync(replaceResultMock.Object);

            // Act
            await _repository.UpdateProperty(property);

            // Assert
            _collectionMock.Verify(
                c => c.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<Property>>(),
                    property,
                    It.IsAny<ReplaceOptions>(),
                    default),
                Times.Once);
        }
    }

    #endregion

    #region SensorRepository Tests

    public class SensorRepositoryTests
    {
        private readonly Mock<IMongoDatabase> _databaseMock;
        private readonly Mock<IMongoCollection<Sensor>> _collectionMock;
        private readonly SensorRepository _repository;

        public SensorRepositoryTests()
        {
            _databaseMock = new Mock<IMongoDatabase>();
            _collectionMock = new Mock<IMongoCollection<Sensor>>();

            _databaseMock.Setup(db => db.GetCollection<Sensor>("sensors", null))
                .Returns(_collectionMock.Object);

            _repository = new SensorRepository(_databaseMock.Object);
        }

        [Fact]
        public async Task GetSensorByIdAsync_WithValidSensorId_ReturnsSensor()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var sensor = new Sensor
            {
                Id = sensorId,
                Serial = "SENSOR001",
                SensorType = Domain.Enums.SensorType.Temperatura,
                OwnerId = Guid.NewGuid(),
                FieldId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };

            var cursorMock = new Mock<IAsyncCursor<Sensor>>();
            cursorMock.Setup(c => c.Current).Returns(new List<Sensor> { sensor });
            cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Sensor>>(),
                It.IsAny<FindOptions<Sensor, Sensor>>(),
                default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetSensorByIdAsync(sensorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sensorId, result.Id);
            Assert.Equal("SENSOR001", result.Serial);
        }

        [Fact]
        public async Task GetSensorByIdAsync_WithNonExistentSensorId_ReturnsNull()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var emptyList = new List<Sensor>();

            var cursorMock = new Mock<IAsyncCursor<Sensor>>();
            cursorMock.Setup(c => c.Current).Returns(emptyList);
            cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Sensor>>(),
                It.IsAny<FindOptions<Sensor, Sensor>>(),
                default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetSensorByIdAsync(sensorId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSensorsByFarmerIdAsync_WithValidFarmerId_ReturnsSensors()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var sensors = new List<Sensor>
            {
                new Sensor { Id = Guid.NewGuid(), Serial = "SENSOR001", SensorType = Domain.Enums.SensorType.Temperatura, OwnerId = farmerId, FieldId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                new Sensor { Id = Guid.NewGuid(), Serial = "SENSOR002", SensorType = Domain.Enums.SensorType.Umidade, OwnerId = farmerId, FieldId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
            };

            var cursorMock = new Mock<IAsyncCursor<Sensor>>();
            cursorMock.Setup(c => c.Current).Returns(sensors);
            cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Sensor>>(),
                It.IsAny<FindOptions<Sensor, Sensor>>(),
                default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetSensorsByFarmerIdAsync(farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetSensorsByFarmerIdAsync_WithNoSensors_ReturnsEmptyList()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var emptyList = new List<Sensor>();

            var cursorMock = new Mock<IAsyncCursor<Sensor>>();
            cursorMock.Setup(c => c.Current).Returns(emptyList);
            cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Sensor>>(),
                It.IsAny<FindOptions<Sensor, Sensor>>(),
                default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetSensorsByFarmerIdAsync(farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteSensorById_WithValidSensorId_DeletesSensor()
        {
            // Arrange
            var sensorId = Guid.NewGuid();
            var deleteResultMock = new Mock<DeleteResult>();

            _collectionMock.Setup(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Sensor>>(),
                default))
                .ReturnsAsync(deleteResultMock.Object);

            // Act
            await _repository.DeleteSensorById(sensorId);

            // Assert
            _collectionMock.Verify(
                c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Sensor>>(), default),
                Times.Once);
        }

        [Fact]
        public async Task DeleteSensorById_WithMultipleSensors_DeletesEach()
        {
            // Arrange
            var sensorIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var deleteResultMock = new Mock<DeleteResult>();

            _collectionMock.Setup(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<Sensor>>(),
                default))
                .ReturnsAsync(deleteResultMock.Object);

            // Act
            foreach (var sensorId in sensorIds)
            {
                await _repository.DeleteSensorById(sensorId);
            }

            // Assert
            _collectionMock.Verify(
                c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Sensor>>(), default),
                Times.Exactly(3));
        }
    }

    #endregion

    #region FarmerRepository Tests

}
    #endregion
