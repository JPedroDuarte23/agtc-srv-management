using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvManagement.Application.Interfaces;
using AgtcSrvManagement.Domain.Models;
using MongoDB.Driver;

namespace AgtcSrvManagement.Infrastructure.Repository;

public class FarmerRepository : IFarmerRepository
{
    private readonly IMongoCollection<Farmer> _collection;
    public FarmerRepository(IMongoDatabase database) 
    {
        _collection = database.GetCollection<Farmer>("farmer");
    }
    public async Task<Farmer> GetFarmerByIdAsync(Guid farmerId)
    {
        return await _collection.Find(f => f.Id == farmerId).FirstOrDefaultAsync();
    }
}
