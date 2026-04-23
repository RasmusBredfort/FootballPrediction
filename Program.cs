using FootballPrediction.Services;
using FootballPrediction.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<FootballDataService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var baseUrl = configuration["FootballData:BaseUrl"];
    var apiKey = configuration["FootballData:ApiKey"];

    client.BaseAddress = new Uri(baseUrl!);
    client.DefaultRequestHeaders.Add("X-Auth-Token", apiKey);
});

builder.Services.AddScoped<PredictionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();