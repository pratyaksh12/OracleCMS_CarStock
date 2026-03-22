using System;
using System.Globalization;
using CarStock.IRepositories;
using FastEndpoints;

namespace CarStock.Features.Cars;
public class SearchCarRequest
{
    [QueryParam] public string Make{get; set;} = "";
    [QueryParam] public string Model{get; set;} = "";
    [QueryParam] public int Page{get; set;}
    [QueryParam] public int Size{get; set;}
}
public class SearchCarEndpoint(ICarRepository _carRepository) : Endpoint<SearchCarRequest, object>
{
    public override void Configure()
    {
        Get("/cars/search");
    }

    public override async Task HandleAsync(SearchCarRequest req, CancellationToken ct)
    {
        var dealerId = int.Parse(User.Claims.First(c => c.Type == "DealerId").Value);
        var result = await _carRepository.SearchAsync(dealerId, req.Make, req.Model, req.Page, req.Size);

        await HttpContext.Response.WriteAsJsonAsync(new {Data = result.Cars, result.TotalCount, req.Page}, cancellationToken: ct);
    }
}
