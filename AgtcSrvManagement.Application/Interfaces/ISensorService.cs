using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvManagement.Application.Dtos;

namespace AgtcSrvManagement.Application.Interfaces;

public interface ISensorService
{
    Task<IEnumerable<SensorResponse>> GetSensorsAsync(Guid farmerId);
    Task DeleteSensorAsync(Guid farmerId, Guid sensorId);
}
