using Amazon.SQS;
using AspNetCore.DataProtection.Aws.S3;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Prometheus;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using AgtcSrvManagement.Infrastructure.Configuration;
using Amazon.SimpleNotificationService;
using Amazon.S3;
using AgtcSrvManagement.Infrastructure.Mappings;
using AgtcSrvManagement.Application.Interfaces;
using AgtcSrvManagement.Application.Services;
using AgtcSrvManagement.Infrastructure.Repository;
using AgtcSrvManagement.Infrastructure.Middleware;

[assembly: ExcludeFromCodeCoverage]

var builder = WebApplication.CreateBuilder(args);

Log.Logger = SerilogConfiguration.ConfigureSerilog();
builder.Host.UseSerilog();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();
builder.Services.AddAWSService<IAmazonSQS>();

var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDbConnection")
    ?? throw new InvalidOperationException("Connection string MongoDbConnection not found.");

var jwtSigningKey = builder.Configuration["Jwt:SigningKey"]
    ?? builder.Configuration["Jwt:DevKey"]
    ?? throw new InvalidOperationException("JWT Signing Key not found.");

var databaseName = builder.Configuration["MongoDbSettings:DatabaseName"]
    ?? throw new InvalidOperationException("Database Name not found.");

if (!builder.Environment.IsDevelopment())
{
    var s3Bucket = builder.Configuration["DataProtection:S3BucketName"];
    var s3KeyPrefix = builder.Configuration["DataProtection:S3KeyPrefix"];

    if (!string.IsNullOrEmpty(s3Bucket) && !string.IsNullOrEmpty(s3KeyPrefix))
    {
        var s3DataProtectionConfig = new S3XmlRepositoryConfig(s3Bucket) { KeyPrefix = s3KeyPrefix };
        builder.Services.AddDataProtection()
            .SetApplicationName("AgroTech")
            .PersistKeysToAwsS3(s3DataProtectionConfig);
    }
}

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
MongoMappings.ConfigureMappings();

builder.Services.AddScoped<IFarmerRepository, FarmerRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<IPropertyService>(sp =>
    new PropertyService(
        sp.GetRequiredService<IFarmerRepository>(),
        sp.GetRequiredService<ILogger<PropertyService>>(),
        sp.GetRequiredService<IPropertyRepository>()
    ));

builder.Services.AddScoped<ISensorService>(sp =>
    new SensorService(
        sp.GetRequiredService<IFarmerRepository>(),
        sp.GetRequiredService<ILogger<SensorService>>(),
        sp.GetRequiredService<ISensorRepository>()
    ));

builder.Services.ConfigureJwtBearer(builder.Configuration, jwtSigningKey);
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "AgroTech Management API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
   {
       {
           new OpenApiSecurityScheme
           {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "Bearer"
               }
           },
           Array.Empty<string>()
       }
   });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.UseHttpMetrics();

app.MapMetrics();
app.MapControllers();

app.Run();
