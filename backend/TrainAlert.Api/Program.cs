using TrainAlert.Api.Providers;
using TrainAlert.Api.Services;
using TrainAlert.Api.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<TrainService>();

builder.Services.AddScoped<ITrainDataProvider, ViaggiaTrenoProvider>();

builder.Services.AddHttpClient();

builder.Services.AddScoped<TrainChangeDetector>();

builder.Services.AddSingleton<TrainStateStore>();

builder.Services.AddScoped<TrainStateMapper>();

builder.Services.AddHostedService<TrainMonitoringWorker>();

var app = builder.Build();

app.MapControllers();

app.Run();