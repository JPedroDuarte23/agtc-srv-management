using System.Diagnostics.CodeAnalysis;
using AgtcSrvManagement.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace AgtcSrvManagement.Infrastructure.Mappings;

[ExcludeFromCodeCoverage]
public static class PropertyMapping
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Field>(map =>
        {
            map.AutoMap();
            map.MapMember(u => u.FieldId)
                .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

            map.MapMember(u => u.Name).SetIsRequired(true);
            map.MapMember(u => u.CropType).SetIsRequired(true);
            map.MapMember(u => u.Area).SetIsRequired(true);
        });


        BsonClassMap.RegisterClassMap<Property>(map =>
        {
            map.AutoMap();

            map.MapIdMember(u => u.Id)
                .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

            map.MapMember(u => u.OwnerId)
                .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));

            map.MapMember(u => u.Name).SetIsRequired(true);
            map.MapMember(u => u.Location).SetIsRequired(true);

            map.MapMember(u => u.Fields).SetIsRequired(true);
        });
    }
}
