
using System.Diagnostics.CodeAnalysis;

namespace AgtcSrvManagement.Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
