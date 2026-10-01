namespace TrainAlert.Api.Models;

public class Notification
{
  public Guid Id { get; set; }

  public Guid UserId { get; set; }
  public User? User { get; set; }

  public Guid AlertId { get; set; }
  public AlertConfiguration? Alert { get; set; }

  public string TrainNumber { get; set; } = string.Empty;
  public string Origin { get; set; } = string.Empty;
  public string Destination { get; set; } = string.Empty;

  public int? PreviousDelayMinutes { get; set; }
  public int? CurrentDelayMinutes { get; set; }

  public string? PreviousPlatform { get; set; }
  public string? CurrentPlatform { get; set; }

  public bool? PreviousRunning { get; set; }
  public bool? CurrentRunning { get; set; }

  public bool? PreviousNotDeparted { get; set; }
  public bool? CurrentNotDeparted { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public bool IsRead { get; set; }
  public bool? PreviousCancelled { get; set; }

  public bool? CurrentCancelled { get; set; }

  public string? PreviousDisruptionReason { get; set; }

  public string? CurrentDisruptionReason { get; set; }
}