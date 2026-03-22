using System;
using System.Threading;
using CarStock.IRepositories;
using CarStock.Models;
using FastEndpoints;
using Microsoft.VisualBasic;

namespace CarStock.Features.Cars;


public class AddCarRequest
{
    public string Make{get; set;} = "";
    public string Model{get; set;} = "";
    public int Year {get; set;}
    public int StockLevel{get; set;}
}
public class AddCarEndpoint(ICarRepository _carRepoitory) : Endpoint<AddCarRequest, object>
{    
    
    public override void Configure()
    {
        Post("/cars");
    }

    public override async Task HandleAsync(AddCarRequest req, CancellationToken ct)
    {
        int dealerId = int.Parse(User.Claims.First(c => c.Type == "DealerId").Value);

        var car = new Car{Make = req.Make, Model = req.Model, Year = req.Year};
        int id = await _carRepoitory.AddOrUpdateCarInGarageAsync(dealerId, car, req.StockLevel);
        
        HttpContext.Response.StatusCode = 201;
        await HttpContext.Response.WriteAsJsonAsync(new {CarId = id}, cancellationToken : ct);
    }
}
