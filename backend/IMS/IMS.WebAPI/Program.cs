using IMS.Data.Data;
using IMS.Data.Interfaces;
using IMS.Data.Repositories;
using IMS.Services.Interfaces;
using IMS.Services.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Read connection string
var connectionString = builder.Configuration.GetConnectionString("DBConnectionString");

// Register DbContext with SQL Server provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(EfRepository<>));

// Register your service
builder.Services.AddScoped<IIncidentService, IncidentService>();

var blobConnection = builder.Configuration.GetConnectionString("AzureStorage");
if (!string.IsNullOrEmpty(blobConnection))
{
    builder.Services.AddSingleton(new Azure.Storage.Blobs.BlobServiceClient(blobConnection));
    builder.Services.AddScoped<IMS.Services.IBlobStorageService, IMS.Services.AzureBlobStorageService>();
}

builder.Services.AddSwaggerGen();   // Register Swagger
builder.Services.AddApplicationInsightsTelemetry(new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMS API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Conventional default route
app.MapControllerRoute(
    name: "default",
    pattern: "/api/v1/{controller}/{action}/{id?}");

app.MapGet("/", () => Results.Redirect("/api/v1/Incident/"));

app.MapControllers();

app.Run();