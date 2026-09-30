namespace TrainAlert.Api.Models;

public class Disruption
{
  public string Title { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  public DateTime? Date { get; set; }

  public string Status { get; set; } = "Informational";

  public string Scope { get; set; } = "Informational";

  public bool IsActive =>
      Status == "Active";
}