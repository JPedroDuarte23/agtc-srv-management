using AgtcSrvManagement.Application.Interfaces;
using AgtcSrvManagement.Domain.Models;
using MongoDB.Driver;

namespace AgtcSrvManagement.Infrastructure.Repository;

public class SensorRepository : ISensorRepository
{
    private readonly IMongoCollection<Sensor> _collection;

    public SensorRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Sensor>("sensors");
    }

    public async Task DeleteSensorById(Guid sensorId)
    {
        await _collection.DeleteOneAsync(s => s.Id == sensorId);
    }

    public async Task<Sensor> GetSensorByIdAsync(Guid sensorId)
    {
        return await _collection
            .Find(s => s.Id == sensorId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Sensor>> GetSensorsByFarmerIdAsync(Guid farmerId)
    {
        return await _collection
            .Find(s => s.OwnerId == farmerId)
            .ToListAsync();
    }
}
