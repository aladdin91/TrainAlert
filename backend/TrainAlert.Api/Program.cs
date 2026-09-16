using TrainAlert.Api.Providers;
using TrainAlert.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<TrainService>();

builder.Services.AddScoped<ITrainDataProvider, FakeTrainDataProvider>();

var app = builder.Build();

app.MapControllers();

app.Run();