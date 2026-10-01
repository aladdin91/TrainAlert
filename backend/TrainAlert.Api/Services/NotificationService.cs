using Microsoft.EntityFrameworkCore;

using TrainAlert.Api.Data;
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class NotificationService
{
  private readonly ApplicationDbContext _dbContext;

  public NotificationService(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<List<Notification>> GetAllAsync(
      Guid userId)
  {
    return await _dbContext.Notifications
        .AsNoTracking()
        .Where(notification => notification.UserId == userId)
        .OrderByDescending(notification => notification.CreatedAt)
        .ToListAsync();
  }

  public async Task<bool> MarkAsReadAsync(
      Guid id,
      Guid userId)
  {
    var notification =
        await _dbContext.Notifications
            .FirstOrDefaultAsync(notification =>
                notification.Id == id &&
                notification.UserId == userId);

    if (notification is null)
    {
      return false;
    }

    if (!notification.IsRead)
    {
      notification.IsRead = true;
      await _dbContext.SaveChangesAsync();
    }

    return true;
  }
}