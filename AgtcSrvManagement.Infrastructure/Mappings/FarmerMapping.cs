using System.Diagnostics.CodeAnalysis;
using AgtcSrvManagement.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace AgtcSrvManagement.Infrastructure.Mappings;

[ExcludeFromCodeCoverage]
public static class FarmerMapping
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Farmer>(map =>
        {
            map.AutoMap();
            map.MapIdMember(u => u.Id)
                .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
            map.MapMember(u => u.Name).SetIsRequired(true);
            map.MapMember(u => u.Email).SetIsRequired(true);
            map.MapMember(u => u.PasswordHash).SetIsRequired(true);
        });
    }
}
