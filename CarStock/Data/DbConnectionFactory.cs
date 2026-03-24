using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace CarStock.Data;


public interface IDbConnectionFactory
{
    public IDbConnection CreateConnection();
}
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _config;

    public DbConnectionFactory(IConfiguration config)
    {
        _config = config;
    }
    // function to load connectionstring and to connect to the database
    public IDbConnection CreateConnection()
    {
        var connectionString = _config.GetConnectionString("DefaultConnection") ?? throw new Exception("Default connection not found. Please describe a connection in the appsetting.json file.");
        return new SqliteConnection(connectionString);
    }
}
