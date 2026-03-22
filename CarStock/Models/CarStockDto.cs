using System;

namespace CarStock.Models;

public class CarStockDto
{
    public int CarId { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int StockLevel { get; set; }
}
