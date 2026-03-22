using System;
using CarStock.Models;

namespace CarStock.IRepositories;

public interface ICarRepository
{
    Task<int> AddOrUpdateCarInGarageAsync(int dealerId, Car car, int stockLevel);
    Task<bool> RemoveCarFromGarageAsync(int dealerId, int carId);
    Task<bool> UpdateStockLevelAsync(int dealerId, int carId, int stockLevel);
    Task<(IList<CarStockDto> Cars, int TotalCount)> GetAllForDealerAsync(int dealerId, int page, int pageSize);
    Task<(IList<CarStockDto> Cars, int TotalCount)> SearchAsync(int dealerId, string make, string model, int page, int pageSize);
}
