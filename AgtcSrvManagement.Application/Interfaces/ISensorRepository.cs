using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvManagement.Domain.Models;

namespace AgtcSrvManagement.Application.Interfaces;

public interface ISensorRepository
{
    Task DeleteSensorById(Guid sensorId);
    Task<Sensor> GetSensorByIdAsync(Guid sensorId);
    Task<IEnumerable<Sensor>> GetSensorsByFarmerIdAsync(Guid farmerId);
}
