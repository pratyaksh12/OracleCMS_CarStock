using System;
using CarStock.Models;

namespace CarStock.IRepositories;

public interface IDealerRepository
{
    Task<Dealer?> GetByUsernameAsync(string username);
    Task<int> CreateAsync(Dealer dealer);
}
