
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Services;

public class DisruptionAnalyzer
{
  public Disruption Analyze(Disruption disruption)
  {
    var title =
        disruption.Title.ToLowerInvariant();

    var description =
        disruption.Description.ToLowerInvariant();

    var text =
        $"{title} {description}";

    if (IsResolved(text))
    {
      disruption.Status = "Resolved";
    }
    else if (IsFuture(title))
    {
      disruption.Status = "Future";
    }
    else if (IsActive(text))
    {
      disruption.Status = "Active";
    }
    else
    {
      disruption.Status = "Informational";
    }



    disruption.Scope =
        DetermineScope(
            disruption.Title,
            disruption.Description);

    return disruption;
  }

  private static bool IsResolved(string text)
  {
    return
        text.Contains("circolazione è tornata regolare") ||
        text.Contains("circolazione regolare") ||
        text.Contains("fine evento") ||
        text.Contains("evento concluso") ||
        text.Contains("situazione risolta") ||
        text.Contains("servizio regolare");
  }
  private static bool IsFuture(string title)
  {
    return
        title.Contains("domenica") ||
        title.Contains("lunedì") ||
        title.Contains("martedì") ||
        title.Contains("mercoledì") ||
        title.Contains("giovedì") ||
        title.Contains("venerdì") ||
        title.Contains("sabato");
  }

  private static bool IsActive(string text)
  {
    return
        text.Contains("circolazione rallentata") ||
        text.Contains("circolazione sospesa") ||
        text.Contains("circolazione interrotta") ||
        text.Contains("ritardi") ||
        text.Contains("ritardo") ||
        text.Contains("inconveniente tecnico") ||
        text.Contains("guasto") ||
        text.Contains("investimento") ||
        text.Contains("cancellazioni") ||
        text.Contains("cancellazione") ||
        text.Contains("limitazioni di percorso") ||
        text.Contains("limitazione di percorso");
  }

  private static string DetermineScope(
      string title,
      string description)
  {
    var text =
        $"{title} {description}"
            .ToLowerInvariant();

    if (text.Contains("infotreni"))
    {
      return "Train";
    }

    if (text.Contains("linea "))
    {
      return "Route";
    }

    if (text.Contains("rete alta velocità") ||
        text.Contains("rete ferroviaria"))
    {
      return "Network";
    }

    return "Informational";
  }
}
