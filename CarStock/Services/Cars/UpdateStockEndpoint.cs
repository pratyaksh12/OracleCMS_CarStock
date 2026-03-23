using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using CarStock.IRepositories;
using FastEndpoints;

namespace CarStock.Features.Cars;

public class UpdateStockRequest
{
    public int CarId{get; set;}
    public int StockLevel{get; set;}
}
public class UpdateStockEndpoint(ICarRepository _carRepository) : Endpoint<UpdateStockRequest, EmptyResponse>
{
    public override void Configure()
    {
        Patch("cars/{CarId}/stock");
    }
    public override async Task HandleAsync(UpdateStockRequest req, CancellationToken ct)
    {
        int dealerId = int.Parse(User.Claims.First(c => c.Type == "DealerId").Value);
        bool updated = await _carRepository.UpdateStockLevelAsync(dealerId, req.CarId, req.StockLevel);

        if (!updated)
        {
            HttpContext.Response.StatusCode = 404;
        }
        else
        {
            HttpContext.Response.StatusCode = 200;
        }

        await Task.CompletedTask;
    }
    
}
