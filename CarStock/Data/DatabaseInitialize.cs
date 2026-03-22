using System;
using Dapper;

namespace CarStock.Data;

public class DatabaseInitialize
{
    public readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitialize(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Initialize()
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Dealers(
            Id INT PRIMARY KEY AUTOINCREMENT,
            UserName TEXT NOT NULL UNIQUE,
            PasswordHash TEXT NOT NULL
            )
        ");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Cars(
            Id INT PRIMARY KEY AUTOINCREMENT,
            Make VARCHAR(255) NOT NULL,
            Model VARCHAR(255) NOT NULL,
            Year INT NOT NULL
            )
        ");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Garages(
            CarId INT NOT NULL,
            DealerId INT NOT NULL,
            StockLevel INT NOT NULL,
            PRIMARY KEY(DealerId, CarId),
            FOREIGN KEY(DealerId) REFERENCES Dealers(Id) ON DELETE CASCADE,
            FOREIGN KEY(CarId) REFERENCES Cars(Id) ON DELETE CASCADE
            )
        ");
    }
}
