
using AgtcSrvManagement.Domain.Models;

namespace AgtcSrvManagement.Application.Interfaces;

public interface IFarmerRepository
{
    Task<Farmer> GetFarmerByIdAsync(Guid farmerId);
}