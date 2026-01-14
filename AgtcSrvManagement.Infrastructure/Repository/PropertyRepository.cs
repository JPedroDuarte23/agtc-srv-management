using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvManagement.Application.Interfaces;
using AgtcSrvManagement.Domain.Models;
using MongoDB.Driver;

namespace AgtcSrvManagement.Infrastructure.Repository;

public class PropertyRepository : IPropertyRepository
{
    private readonly IMongoCollection<Property> _collection;

    public PropertyRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Property>("properties");
    }

    public async Task CreateProperty(Property property)
    {
        await _collection.InsertOneAsync(property);
    }

    public async Task<IEnumerable<Property>> GetPropertiesByOwnerId(Guid farmerId)
    {
        return await _collection
            .Find(p => p.OwnerId == farmerId)
            .ToListAsync();
    }

    public async Task<Property> GetPropertyByIdAsync(Guid propertyId)
    {
        return await _collection
            .Find(p => p.Id == propertyId)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateProperty(Property property)
    {
        await _collection.ReplaceOneAsync(p => p.Id == property.Id, property);
    }
}
