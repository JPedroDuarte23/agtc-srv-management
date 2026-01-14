using System.Diagnostics.CodeAnalysis;
using AgtcSrvManagement.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace AgtcSrvManagement.Infrastructure.Mappings;

[ExcludeFromCodeCoverage]
public static class SensorMapping
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Sensor>(map =>
        {
            map.AutoMap();
            map.MapIdMember(u => u.Id)
                .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
            map.MapMember(u => u.Serial).SetIsRequired(true);
            map.MapMember(u => u.OwnerId).SetIsRequired(true).SetSerializer(new GuidSerializer(GuidRepresentation.Standard)); ;
            map.MapMember(u => u.FieldId).SetIsRequired(true).SetSerializer(new GuidSerializer(GuidRepresentation.Standard)); ;
            map.MapMember(u => u.SensorType).SetIsRequired(true);
            map.MapMember(u => u.CreatedAt).SetIsRequired(true);
        });
    }
}
