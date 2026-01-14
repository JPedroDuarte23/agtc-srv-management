using System.Diagnostics.CodeAnalysis;
using AgtcSrvManagement.Infrastructure.Mappings;

namespace AgtcSrvManagement.Infrastructure.Mappings;

[ExcludeFromCodeCoverage]
public static class MongoMappings
{
    public static void ConfigureMappings() 
    {
        FarmerMapping.Configure();
        SensorMapping.Configure();
        PropertyMapping.Configure();
    }
}