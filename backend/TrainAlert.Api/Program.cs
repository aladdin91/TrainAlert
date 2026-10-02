using TrainAlert.Api.Providers;
using TrainAlert.Api.Services;
using TrainAlert.Api.Workers;
using Microsoft.AspNetCore.Identity;
using TrainAlert.Api.Models;
using Microsoft.EntityFrameworkCore;
using TrainAlert.Api.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
  {
    Title = "TrainAlert API",
    Version = "v1",
    Description = "TrainAlert backend API"
  });

  options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
  {
    Name = "Authorization",
    Type = Microsoft.OpenApi.SecuritySchemeType.Http,
    Scheme = "bearer",
    BearerFormat = "JWT",
    In = Microsoft.OpenApi.ParameterLocation.Header,
    Description = "Enter your JWT token"
  });

  options.AddSecurityRequirement(document =>
      new Microsoft.OpenApi.OpenApiSecurityRequirement
      {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
      });
});

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      var jwtKey =
          builder.Configuration["Jwt:Key"]
          ?? throw new InvalidOperationException(
              "JWT key is not configured.");

      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer =
              builder.Configuration["Jwt:Issuer"],

        ValidAudience =
              builder.Configuration["Jwt:Audience"],

        IssuerSigningKey =
              new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(jwtKey))
      };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<TrainService>();
builder.Services.AddScoped<TrainDestinationFilter>();

builder.Services.AddScoped<ITrainDataProvider, ViaggiaTrenoProvider>();
builder.Services.AddScoped<IStationSearchProvider, ViaggiaTrenoProvider>();
builder.Services.AddScoped<StationSearchService>();

builder.Services.AddHttpClient();

builder.Services.AddScoped<TrainChangeDetector>();

builder.Services.AddSingleton<TrainStateStore>();

builder.Services.AddScoped<TrainStateMapper>();

builder.Services.AddHostedService<TrainMonitoringWorker>();

builder.Services.AddScoped<AlertService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordHasher<User>>();

builder.Services.AddScoped<INotificationService, LogNotificationService>();

builder.Services.AddScoped<RouteStatusService>();

builder.Services.AddScoped<DisruptionAnalyzer>();

builder.Services.AddScoped<DisruptionRouteMatcher>();

builder.Services.AddScoped<
    IInfomobilityProvider,
    ViaggiaTrenoInfomobilityProvider>();

builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<NotificationDecisionService>();

builder.Services.AddScoped<TrenordStrikeParser>();
builder.Services.AddScoped<IStrikeProvider, TrenordStrikeProvider>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.Run();
