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
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserName TEXT NOT NULL UNIQUE,
            PasswordHash TEXT NOT NULL
            )
        ");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Cars(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Make VARCHAR(255) NOT NULL,
            Model VARCHAR(255) NOT NULL,
            Year INTEGER NOT NULL
            )
        ");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Garages(
            CarId INTEGER NOT NULL,
            DealerId INTEGER NOT NULL,
            StockLevel INTEGER NOT NULL,
            PRIMARY KEY(DealerId, CarId),
            FOREIGN KEY(DealerId) REFERENCES Dealers(Id) ON DELETE CASCADE,
            FOREIGN KEY(CarId) REFERENCES Cars(Id) ON DELETE CASCADE
            )
        ");

        connection.Execute(@"
            CREATE INDEX IF NOT EXISTS IDX_Cars_Make_Model_Year ON Cars(Make, Model, Year);
        ");
        connection.Execute(@"
            CREATE INDEX IF NOT EXISTS IDX_Garages_CarId ON Garages(CarId);
        ");
    }
}
