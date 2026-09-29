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

    public async Task<List<AlertConfiguration>> GetAllAsync()
    {
        return await _db.Alerts
            .AsNoTracking()
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
        Guid id)
    {
        return await _db.Alerts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var alert = await _db.Alerts
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alert is null)
        {
            return false;
        }

        _db.Alerts.Remove(alert);

        await _db.SaveChangesAsync();

        return true;
    }
}