using Microsoft.EntityFrameworkCore;
using TrainAlert.Api.Data;
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class AlertService
{
    private readonly ApplicationDbContext _db;

    public AlertService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<AlertConfiguration>> GetAllAsync(
        Guid userId)
    {
        return await _db.Alerts
            .AsNoTracking()
            .Where(alert => alert.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<AlertConfiguration>> GetAllForMonitoringAsync()
    {
        return await _db.Alerts
            .AsNoTracking()
            .Where(alert => alert.IsEnabled)
            .ToListAsync();
    }

    public async Task<AlertConfiguration> AddAsync(
        AlertConfiguration alert)
    {
        alert.Id = Guid.NewGuid();

        _db.Alerts.Add(alert);

        await _db.SaveChangesAsync();

        return alert;
    }

    public async Task<AlertConfiguration?> GetByIdAsync(
        Guid id,
        Guid userId)
    {
        return await _db.Alerts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                alert =>
                    alert.Id == id &&
                    alert.UserId == userId);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        Guid userId)
    {
        var alert = await _db.Alerts
            .FirstOrDefaultAsync(
                alert =>
                    alert.Id == id &&
                    alert.UserId == userId);

        if (alert is null)
        {
            return false;
        }

        _db.Alerts.Remove(alert);

        await _db.SaveChangesAsync();

        return true;
    }
}