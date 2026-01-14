using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvManagement.Application.Dtos;
using AgtcSrvManagement.Application.Exceptions;
using AgtcSrvManagement.Application.Interfaces;
using AgtcSrvManagement.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AgtcSrvManagement.Application.Services;

public class PropertyService : IPropertyService
{

    private readonly IFarmerRepository _farmerRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<PropertyService> _logger;

    public PropertyService(IFarmerRepository farmerRepository, ILogger<PropertyService> logger, IPropertyRepository propertyRepository)
    {
        _farmerRepository = farmerRepository;
        _logger = logger;
        _propertyRepository = propertyRepository;
    }

    public async Task<IEnumerable<PropertyResponse>> GetPropertiesAsync(Guid farmerId)
    {
        await ValidateFarmerId(farmerId);

        var properties = await _propertyRepository.GetPropertiesByOwnerId(farmerId);

        List<PropertyResponse> responseList = new List<PropertyResponse>();

        foreach (var property in properties) {
            responseList.Add(MapPropertiesToResponseDto(property));
        }

        return responseList;
    }

    public async Task<PropertyResponse> GetPropertyAsync(Guid propertyId, Guid farmerId)
    {
        await ValidateFarmerId(farmerId);

        var property = await _propertyRepository.GetPropertyByIdAsync(propertyId);
        if (property == null || property.OwnerId != farmerId)
        {
            throw new Exceptions.NotFoundException("Propriedade não encontrada");
        }
        
        return MapPropertiesToResponseDto(property);
    }

    public async Task CreatePropertyAsync(CreatePropertyRequest request, Guid farmerId)
    {
        await ValidateFarmerId(farmerId);

        var property = new Property
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Location = request.Location,
            TotalArea = request.TotalArea,
            OwnerId = farmerId,
            Fields = new List<Field>()
        };

        try
        {
            await _propertyRepository.CreateProperty(property);
            _logger.LogInformation("Propriedade {Name} cadastrada com sucesso", request.Name);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar propriedade {Name} ao fazendeiro de Id {farmerId}", request.Name, farmerId);
            throw new ModifyDatabaseException(ex);
        }

    }

    public async Task AddFieldToPropertyAsync(CreateFieldRequest request, Guid propertyId, Guid farmerId)
    {
        await ValidateFarmerId(farmerId);

        try
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId);
            if (property == null || property.OwnerId != farmerId)
            {
                throw new Exceptions.NotFoundException("Propriedade não encontrada");
            }

            var field = new Field
            {
                FieldId = Guid.NewGuid(),
                Name = request.Name,
                CropType = request.CropType,
                Area = request.Area
            };

            property.Fields.Add(field);


            try
            {
                await _propertyRepository.UpdateProperty(property);
                _logger.LogInformation("Talhão {Name} cadastrada com sucesso no campo {propertyName}", request.Name, property.Name);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar talhão com nome {Name} à propriedade {propertyName}", request.Name, property.Name);
                throw new ModifyDatabaseException(ex);
            }
        } catch (HttpException)
        {
            throw;

        } catch(Exception ex)
        {
            throw new UnexpectedException(ex);
        }
    }

    private async Task ValidateFarmerId(Guid farmerId)
    {
        try
        {
            var farmer = await _farmerRepository.GetFarmerByIdAsync(farmerId);

            if (farmer == null)
            {
                throw new Exceptions.NotFoundException("Fazendeiro não encontrada");
            }
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exceptions.UnexpectedException(ex);
        }
    }

    private PropertyResponse MapPropertiesToResponseDto(Property property)
    {
        var propertyResponse = new PropertyResponse
        {
            Id = property.Id,
            Name = property.Name,
            Location = property.Location,
            TotalArea = property.TotalArea,
            OwnerId = property.OwnerId,
            Fields = [.. property.Fields.Select(f => new FieldResponse
            {
                FieldId = f.FieldId,
                Name = f.Name,
                Area = f.Area,
                CropType = f.CropType
            })]
        };

        return propertyResponse;
    }
}
