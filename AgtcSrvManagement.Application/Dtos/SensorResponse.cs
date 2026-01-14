using AgtcSrvManagement.Domain.Enums;

namespace AgtcSrvManagement.Application.Dtos;

public class SensorResponse
{
    public Guid Id { get; set; }
    public string Serial { get; set; }
    public SensorType SensorType { get; set; }
    public Guid OwnerId { get; set; }
    public Guid FieldId { get; set; }
    public DateTime CreatedAt { get; set; }
}