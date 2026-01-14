using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvManagement.Application.Dtos;

namespace AgtcSrvManagement.Application.Interfaces;

public interface IPropertyService
{
    Task<IEnumerable<PropertyResponse>> GetPropertiesAsync(Guid farmerId);
    Task<PropertyResponse> GetPropertyAsync(Guid propertyId, Guid farmerId);
    Task CreatePropertyAsync(CreatePropertyRequest request, Guid farmerId);
    Task AddFieldToPropertyAsync(CreateFieldRequest request, Guid propertyId, Guid farmerId);
}
