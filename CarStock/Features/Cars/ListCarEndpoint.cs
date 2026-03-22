using System;
using CarStock.IRepositories;
using FastEndpoints;

namespace CarStock.Features.Cars;

public class ListCarsRequest
{
    [QueryParam] public int Page{get; set;} = 1;
    [QueryParam] public int Size{get; set;} = 20;
}
public class ListCarEndpoint(ICarRepository _carRepository) : Endpoint<ListCarsRequest, object>
{
    public override void Configure()
    {
        Get("cars/");
    }

    public override async Task HandleAsync(ListCarsRequest req, CancellationToken ct)
    {
        var dealerId = int.Parse(User.Claims.First(c => c.Type ==  "DealerId").Value);
        var result = await _carRepository.GetAllForDealerAsync(dealerId, req.Page, req.Size);

        await HttpContext.Response.WriteAsJsonAsync(new {Data = result}, cancellationToken: ct);
    }
}
