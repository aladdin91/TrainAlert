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

    public DbSet<Notification> Notifications => Set<Notification>();

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

        modelBuilder.Entity<User>()
    .HasMany(user => user.Notifications)
    .WithOne(notification => notification.User)
    .HasForeignKey(notification => notification.UserId)
    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AlertConfiguration>()
            .HasMany<Notification>()
            .WithOne(notification => notification.Alert)
            .HasForeignKey(notification => notification.AlertId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}