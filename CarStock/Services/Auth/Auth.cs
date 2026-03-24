using System;
using CarStock.IRepositories;
using CarStock.Models;
using FastEndpoints;
using FastEndpoints.Security;

namespace CarStock.Features;

public class LoginRequest
{
    public string Username{get; set;} = "";
    public string Password{get; set;} = "";
}

public class LoginResponse
{
    public string Token{get; set;} = "";
}
public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IDealerRepository _dealerRepository;
    private readonly IConfiguration _config;

    public LoginEndpoint(IDealerRepository dealerRepository, IConfiguration config)
    {
        _dealerRepository = dealerRepository;
        _config = config;
    }

    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
    }

    // login and signup for the dealer. Dealer needs to have a unique name. For simplicity signup and login is using the same endpoint
    public override async Task HandleAsync(LoginRequest request, CancellationToken stoppingToken)
    {
        var username = request.Username.ToLowerInvariant();
        var dealer = await _dealerRepository.GetByUsernameAsync(username);

        if(dealer is null)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 10);
            await _dealerRepository.CreateAsync(new Dealer{Username = username, PasswordHash = passwordHash});
            dealer = await _dealerRepository.GetByUsernameAsync(username);
        }
        else if(!BCrypt.Net.BCrypt.Verify(request.Password, dealer.PasswordHash))
        {
            HttpContext.Response.StatusCode = 401;
            await HttpContext.Response.WriteAsJsonAsync("Incorrect Username or Password", cancellationToken: stoppingToken);
            return;    
        }

        
        // JwtBearer token for authorization
        var token = JwtBearer.CreateToken(
            options =>
            {
                options.SigningKey = _config["Jwt:Secret"]!;
                options.Issuer = _config["Jwt:Issuer"];
                options.Audience = _config["Jwt:Audience"];
                options.ExpireAt = DateTime.UtcNow.AddDays(1);
                options.User.Claims.Add(("DealerId", dealer!.Id.ToString()));
            }
        );

        await HttpContext.Response.WriteAsJsonAsync(new LoginResponse { Token = token }, cancellationToken: stoppingToken);
    }

}
