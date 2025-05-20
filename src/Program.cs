// Program.cs
using FlightQualityAnalyzer.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Qoco_Airlines.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFlightService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<FlightsController>>();
    return new FlightService(null, null, logger);
}); 

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
