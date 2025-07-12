using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventLog(); // For logger

builder.Services.AddHttpLogging(logging => {
logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders
    | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders
    ;
});

builder.Services.AddControllersWithViews();

//add services into IoC container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False

var app = builder.Build();

 

if (builder.Environment.IsDevelopment())
{
 app.UseDeveloperExceptionPage();
}

app.Logger.LogDebug("debugggggggggggggggggggggggggggggggggggggggggg");
app.Logger.LogError("errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrror");
app.Logger.LogWarning("warninggggggggggggggggggggggggggggggggggggggggg");
app.Logger.LogCritical("Criticaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaal");
app.Logger.LogInformation("Informatiiiiiiiiiiiiiiiiiiiiiiiiiiiiion");

app.UseHttpLogging();
if (builder.Environment.IsEnvironment("Test") == false) 
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa"); // For Pdf

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }