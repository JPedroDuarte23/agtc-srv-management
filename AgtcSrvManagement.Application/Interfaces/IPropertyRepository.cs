using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvManagement.Domain.Models;

namespace AgtcSrvManagement.Application.Interfaces;

public interface IPropertyRepository
{
    Task CreateProperty(Property property);
    Task<IEnumerable<Property>> GetPropertiesByOwnerId(Guid farmerId);
    Task<Property> GetPropertyByIdAsync(Guid propertyId);
    Task UpdateProperty(Property property);
}
