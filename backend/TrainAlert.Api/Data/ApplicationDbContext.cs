using Microsoft.EntityFrameworkCore;
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(
      DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {
  }

  public DbSet<AlertConfiguration> Alerts => Set<AlertConfiguration>();
}