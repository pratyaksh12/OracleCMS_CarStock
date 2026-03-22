using System.Security.Cryptography;
using CarStock.Data;
using CarStock.IRepositories;
using CarStock.Repositories;
using FastEndpoints;
using FastEndpoints.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddSingleton<DatabaseInitialize>();
builder.Services.AddScoped<IDealerRepository, DealerRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddOpenApi();

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new Exception("Jwt secret not found. Set a secret key in the appsetting file.");
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = jwtSecret);
builder.Services.AddAuthentication();
builder.Services.AddFastEndpoints();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

using(var services = app.Services.CreateScope())
{
    var dbInit = services.ServiceProvider.GetRequiredService<DatabaseInitialize>();
    dbInit.Initialize();
}

app.UseCors("AllowAll");
app.UseAuthentication().UseAuthorization().UseFastEndpoints();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();

