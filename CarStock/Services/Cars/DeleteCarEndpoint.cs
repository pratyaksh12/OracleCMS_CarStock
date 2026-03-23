using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using CarStock.IRepositories;
using FastEndpoints;

namespace CarStock.Features.Cars;
public class DeleteCarRequest
{
    public int CarId{get; set;}
}
public class DeleteCarEndpoint(ICarRepository _carRepository) : Endpoint<DeleteCarRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("car/{CarId}");
    }
    public override async Task HandleAsync(DeleteCarRequest req, CancellationToken ct)
    {
        var dealerId = int.Parse(User.Claims.First(c => c.Type == "DealerId").Value);
        bool deleted = await _carRepository.RemoveCarFromGarageAsync(dealerId, req.CarId);

        if (!deleted)
        {
            HttpContext.Response.StatusCode = 404;
        }else
        {
          HttpContext.Response.StatusCode = 204;  
        }

        await Task.CompletedTask;
    }
}
