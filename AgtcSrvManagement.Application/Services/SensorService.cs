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

public class SensorService : ISensorService
{
    private readonly IFarmerRepository _farmerRepository;
    private readonly ISensorRepository _sensorRepository;
    private readonly ILogger<SensorService> _logger;

    public SensorService(IFarmerRepository farmerRepository, ILogger<SensorService> logger, ISensorRepository sensorRepository)
    {
        _farmerRepository = farmerRepository;
        _logger = logger;
        _sensorRepository = sensorRepository;
    }

    public async Task<IEnumerable<SensorResponse>> GetSensorsAsync(Guid farmerId)
    {
        await ValidateFarmerId(farmerId);

        try
        {
            var sensors = await _sensorRepository.GetSensorsByFarmerIdAsync(farmerId);


            return MapSensorToResponseDto(sensors);
        } catch(Exception ex)
        {
            throw new UnexpectedException(ex);
        }

        throw new NotImplementedException();
    }

    public async Task DeleteSensorAsync(Guid farmerId, Guid sensorId)
    {
        await ValidateFarmerId(farmerId);

        try
        {
            var sensor = await _sensorRepository.GetSensorByIdAsync(sensorId);

            if(sensor == null)
            {
                throw new NotFoundException("Sensor" + sensorId + " não encontrado");
            }

            await _sensorRepository.DeleteSensorById(sensorId);
        } catch (HttpException)
        {
            throw;
        } catch (Exception ex) {
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

    private List<SensorResponse> MapSensorToResponseDto(IEnumerable<Sensor> sensors)
    {
        var responseList = new List<SensorResponse>();
        foreach (var sensor in sensors)
        {
            var dto = new SensorResponse
            {
                Id = sensor.Id,
                Serial = sensor.Serial,
                SensorType = sensor.SensorType,
                OwnerId = sensor.OwnerId,
                FieldId = sensor.FieldId,
                CreatedAt = sensor.CreatedAt
            };
            responseList.Add(dto);
        }

        return responseList;
    }
}
