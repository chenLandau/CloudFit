using DB;
using NLog;
using NLog.Web;
using Server.Interfaces;
using Server.Models;

LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                      });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Host.UseNLog();
builder.Services.AddHostedService<InsertDataToDBService>();
builder.Services.AddSingleton<MongoHelper>();
builder.Services.AddTransient<IAzureCloud, AzureCloud>();
builder.Services.AddTransient<IAmazonCloud, AmazonCloud>();
builder.Services.AddTransient<IGoogleCloud, GoogleCloud>();
builder.Services.AddTransient<IMetricsResults, MetricsResults>();
var app = builder.Build();

app.UseCors("_myAllowSpecificOrigins");
app.UseAuthorization();
app.MapControllers();
app.Run("http://localhost:8496");
