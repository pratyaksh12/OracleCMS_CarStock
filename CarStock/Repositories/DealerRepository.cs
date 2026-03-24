using System;
using CarStock.Data;
using CarStock.IRepositories;
using CarStock.Models;
using Dapper;

namespace CarStock.Repositories;

public class DealerRepository : IDealerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public DealerRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // simple cerate. Takes in username and password to create a dealer
    public async Task<int> CreateAsync(Dealer dealer)
    {
        using var connection = _connectionFactory.CreateConnection();

        var executable_query = @"
            INSERT INTO Dealers (Username, PasswordHash)
            VALUES (@Username, @PasswordHash);
            SELECT last_insert_rowid()
        ";

        return await connection.ExecuteScalarAsync<int>(executable_query, dealer);
    }

    // Fetch dealer details by username
    public async Task<Dealer?> GetByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();

        var executable_query = @"
            SELECT * FROM Dealers WHERE Username = @Username
        ";

        return await connection.QueryFirstOrDefaultAsync<Dealer>(executable_query, new {Username = username});
    }
}
