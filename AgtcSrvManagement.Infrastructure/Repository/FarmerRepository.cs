using AgtcSrvManagement.Application.Interfaces;
using AgtcSrvManagement.Domain.Models;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace AgtcSrvManagement.Infrastructure.Repository;

public class FarmerRepository : IFarmerRepository
{
    private readonly IMongoCollection<Farmer> _collection;
    private readonly ILogger<FarmerRepository> _logger;
    public FarmerRepository(IMongoDatabase database, ILogger<FarmerRepository> logger) 
    {
        _collection = database.GetCollection<Farmer>("farmers");
        _logger = logger;
    }
    public async Task<Farmer> GetFarmerByIdAsync(Guid farmerId)
    {
        // [LOG 1] O que chegou para buscar?
        _logger.LogWarning($"[DEBUG] Buscando Fazendeiro com ID: {farmerId}");

        try
        {
            var todos = await _collection.Find(_ => true).Limit(5).ToListAsync();
            _logger.LogWarning($"[DEBUG] Total de fazendeiros encontrados na coleção: {todos.Count}");

            foreach (var f in todos)
            {
                _logger.LogWarning($"[DEBUG] Encontrado no Banco: ID={f.Id} | Nome={f.Name}");

                // Verifica se os GUIDs batem visualmente
                if (f.Id.ToString() == farmerId.ToString())
                {
                    _logger.LogWarning($"[DEBUG] 🚨 OS IDS BATEM COMO STRING! Mas a query direta falhou? (Problema de Serialização)");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[DEBUG] Erro ao tentar listar fazendeiros: {ex.Message}");
        }

        return await _collection.Find(f => f.Id == farmerId).FirstOrDefaultAsync();
    }
}
