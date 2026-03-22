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

    public override async Task HandleAsync(LoginRequest request, CancellationToken stoppingToken)
    {
        var dealer = await _dealerRepository.GetByUsernameAsync(request.Username);

        if(dealer is null)
        {
            await _dealerRepository.CreateAsync(new Dealer{Username = request.Username, PaswordHash = request.Password});
            dealer = await _dealerRepository.GetByUsernameAsync(request.Username);
        }
        else if(dealer.PaswordHash != request.Password)
        {
            HttpContext.Response.StatusCode = 401;
            return;    
        }

        var jwtSecret = _config["Jwt:Secret"] ?? throw new Exception("Jwt secret missing. Set it in the appsetting");

        var token = JwtBearer.CreateToken(
            options =>
            {
                options.SigningKey = jwtSecret;
                options.ExpireAt = DateTime.UtcNow.AddDays(1);
                options.User.Claims.Add(("DealerId", dealer!.Id.ToString()));
            }
        );

        await HttpContext.Response.WriteAsJsonAsync(new LoginResponse { Token = token }, cancellationToken: stoppingToken);
    }

}
