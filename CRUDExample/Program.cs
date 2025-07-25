using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.StartUpExtensions;

var services = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();
//builder.Logging.AddEventLog(); // For traditinal logger

//Serilog
services.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

services.Services.ConfigureServices(services.Configuration);

var app = services.Build();

 

if (services.Environment.IsDevelopment())
{
 app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

app.Logger.LogDebug("debugggggggggggggggggggggggggggggggggggggggggg");
app.Logger.LogError("errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrror");
app.Logger.LogWarning("warninggggggggggggggggggggggggggggggggggggggggg");
app.Logger.LogCritical("Criticaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaal");
app.Logger.LogInformation("Informatiiiiiiiiiiiiiiiiiiiiiiiiiiiiion");

app.UseHttpLogging();
if (services.Environment.IsEnvironment("Test") == false) 
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa"); // For Pdf

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }