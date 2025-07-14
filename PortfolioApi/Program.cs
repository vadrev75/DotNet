using PortfolioApi.Helper;
using PortfolioApi.Interfaces;
using PortfolioApi.Repository;
using PortfolioApi.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IRepository, JsonRepository>();
builder.Services.AddScoped<IPerformanceEngine, PerformanceEngine>();
builder.Services.AddSingleton<IPriceProvider>(new JsonPriceProvider("portfolio_prices.json"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
