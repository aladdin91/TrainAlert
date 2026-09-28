using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class AlertService
{
    private readonly List<AlertConfiguration> _alerts = new();

    public List<AlertConfiguration> GetAll()
    {
        return _alerts.ToList();
    }

    public AlertConfiguration Add(AlertConfiguration alert)
    {
        alert.Id = Guid.NewGuid();

        _alerts.Add(alert);

        return alert;
    }

    public AlertConfiguration? GetById(Guid id)
    {
        return _alerts.FirstOrDefault(a => a.Id == id);
    }

    public bool Delete(Guid id)
    {
        var alert = GetById(id);

        if (alert is null)
        {
            return false;
        }

        _alerts.Remove(alert);

        return true;
    }
}