using System;
using CarStock.Data;
using CarStock.IRepositories;
using CarStock.Models;
using Dapper;

namespace CarStock.Repositories;

public class CarRepository : ICarRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CarRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<int> AddOrUpdateCarInGarageAsync(int dealerId, Car car, int stockLevel)
    {
        using var connection = _connectionFactory.CreateConnection();

        var executable_query = @"
            SELECT Id FROM Cars WHERE Make = @Make AND Model = @Model AND Year = @Year
        ";

        var existingCarId = await connection.QueryFirstOrDefaultAsync<int?>(executable_query, car);
        int carId;

        if (existingCarId is not null)
        {
            carId = existingCarId.Value;
        }else
        {
            var create_car_query = @"
                INSERT INTO Cars(Make, Model, Year)
                VALUES (@Make, @Model, @Year);
                SELECT last_insert_rowid();
            ";

            carId = await connection.ExecuteScalarAsync<int>(create_car_query, car);
        }

        var handleGarage_query = @"
            INSERT OR REPLACE INTO Garages (DealerId, CarId, StockLevel)
            VALUES (@DealerId, @CarId, @StockLevel);
        ";

        await connection.ExecuteAsync(handleGarage_query, new {DealerId = dealerId, CarId = carId, StockLevel = stockLevel} );

        return carId;
        
    }

    public async Task<(IList<CarStockDto> Cars, int TotalCount)> GetAllForDealerAsync(int dealerId, int page, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();
        var offset = (page - 1) * pageSize;
        var executable_query = @"
            SELECT COUNT(*) FROM Garages WHERE DealerId = @DealerId;
        ";

        var totalCount = await connection.ExecuteScalarAsync<int>(executable_query, new {DealerId = dealerId});

        var getAllCars_query = @"
            SELECT c.Id AS CarId, c.Make, c.Model, c.Year, g.StockLevel FROM Garages AS g
            INNER JOIN Cars AS c ON g.CarId = c.Id
            WHERE g.DealerId = @DealerId
            ORDER BY c.Id LIMIT @Limit OFFSET @Offset;
        ";
        var cars = (await connection.QueryAsync<CarStockDto>(getAllCars_query, new {DealerId = dealerId, Limit = pageSize, Offset = offset})).ToList();

        return (cars, totalCount);
    }

    public async Task<bool> RemoveCarFromGarageAsync(int dealerId, int carId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var execute_query = @"
            DELETE FROM Garages
            WHERE CarId = @CarId AND DealerId = @DealerId;
            ";

        int rows = await connection.ExecuteAsync(execute_query, new {CarId = carId, DealerId = dealerId});

        return rows > 0;
    }

    public async Task<(IList<CarStockDto> Cars, int TotalCount)> SearchAsync(int dealerId, string make, string model, int page, int pageSize)
    {
        var offset = (page - 1) * pageSize;
        using var connection = _connectionFactory.CreateConnection();

        var count_query = @"
            SELECT COUNT(*) FROM Garages AS g
            INNER JOIN Cars AS c ON (c.Id = g.CarId)
            WHERE g.DealerId = @DealerId
            AND (
                (@Make IS NULL OR @Make = '' OR c.Make LIKE @Make || '%')
                OR (@Model IS NULL OR @Model = '' OR c.Model LIKE @Model || '%')
            );
        ";

        var carSearch_query = @"
            SELECT c.Id AS CarId, c.Make, c.Model, c.Year, g.StockLevel FROM Garages AS g
            INNER JOIN Cars AS c ON (g.CarId = c.Id)
            WHERE g.DealerId = @DealerId
            AND (
                (@Make IS NULL OR @Make = '' OR c.Make LIKE @Make || '%')
                OR (@Model IS NULL OR @Model = '' OR c.Model LIKE @Model || '%')
            )
            ORDER BY c.Id LIMIT @Limit OFFSET @Offset;
        ";

        var totalCount = await connection.ExecuteScalarAsync<int>(count_query, new {DealerId = dealerId, Make = make, Model = model});
        var cars = await connection.QueryAsync<CarStockDto>(carSearch_query, new {DealerId = dealerId, Make = make, Model = model, Limit = pageSize, Offset = offset});

        return (cars.ToList(), totalCount);
    }

    public async Task<bool> UpdateStockLevelAsync(int dealerId, int carId, int stockLevel)
    {
        using var connection = _connectionFactory.CreateConnection();

        var execute_query = @"
            UPDATE Garages SET StockLevel = @StockLevel WHERE CarId = @CarId AND DealerId = @DealerId;
        ";

        int rows = await connection.ExecuteAsync(execute_query, new {CarId = carId, DealerId = dealerId, StockLevel = stockLevel});

        return rows > 0;
    }
}
