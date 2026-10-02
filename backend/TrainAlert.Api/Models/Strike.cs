public class Strike
{
  public string Title { get; set; } = string.Empty;

  public DateTime? PublishedAt { get; set; }

  public DateTime? StartAt { get; set; }

  public DateTime? EndAt { get; set; }

  public string Description { get; set; } = string.Empty;

  public string? NoticeNumber { get; set; }

  public string SourceUrl { get; set; } = string.Empty;
  public string? NoticeUrl { get; set; }
}