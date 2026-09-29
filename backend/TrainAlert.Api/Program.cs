using TrainAlert.Api.Providers;
using TrainAlert.Api.Services;
using TrainAlert.Api.Workers;

using Microsoft.EntityFrameworkCore;
using TrainAlert.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddScoped<TrainService>();

builder.Services.AddScoped<ITrainDataProvider, ViaggiaTrenoProvider>();

builder.Services.AddHttpClient();

builder.Services.AddScoped<TrainChangeDetector>();

builder.Services.AddSingleton<TrainStateStore>();

builder.Services.AddScoped<TrainStateMapper>();

builder.Services.AddHostedService<TrainMonitoringWorker>();

builder.Services.AddScoped<AlertService>();

builder.Services.AddScoped<INotificationService, LogNotificationService>();

var app = builder.Build();

app.MapControllers();

app.Run();