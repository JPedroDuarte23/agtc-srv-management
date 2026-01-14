using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using AgtcSrvManagement.Domain.Enums;

namespace AgtcSrvManagement.Domain.Models;

public class Sensor
{
    public Guid Id { get; set; }
    public string Serial { get; set; }
    public SensorType SensorType { get; set; }
    public Guid OwnerId { get; set; }     
    public Guid FieldId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
