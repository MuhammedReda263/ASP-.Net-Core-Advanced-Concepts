using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using CRUDExample.Filters.ActionFilters;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();
//builder.Logging.AddEventLog(); // For traditinal logger

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

builder.Services.AddHttpLogging(logging => {
logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPropertiesAndHeaders
    | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders
    ;
});

//builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews(options => {
    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<PersonHeaderActionFilter>>();
    options.Filters.Add(new PersonHeaderActionFilter(logger, "CustomKeyFromGlobal", "CustomValueFromGlobal",2)); // We implement iorderdfilter to can add value to order in this global filter
    }
    //options.Filters.Add<PersonHeaderActionFilter>(5) if your filter dosn't have additional paramaters and you don't need to implement iorderdfilter
    );


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

app.UseSerilogRequestLogging();

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