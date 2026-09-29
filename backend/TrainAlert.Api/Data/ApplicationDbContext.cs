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

  public DbSet<User> Users => Set<User>();

  public DbSet<AlertConfiguration> Alerts => Set<AlertConfiguration>();

  protected override void OnModelCreating(
      ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
        .HasIndex(user => user.Email)
        .IsUnique();

    modelBuilder.Entity<User>()
        .HasMany(user => user.Alerts)
        .WithOne(alert => alert.User)
        .HasForeignKey(alert => alert.UserId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}