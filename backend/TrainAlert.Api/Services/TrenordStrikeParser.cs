
using System.Globalization;
using HtmlAgilityPack;
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public class TrenordStrikeParser
{
  public Strike? Parse(string html, string sourceUrl)
  {
    if (string.IsNullOrWhiteSpace(html))
    {
      return null;
    }

    var document = new HtmlDocument();
    document.LoadHtml(html);

    var title = ExtractTitle(document);

    if (string.IsNullOrWhiteSpace(title))
    {
      return null;
    }

    var publishedAt = ExtractPublishedAt(document);
    var articleNode = FindStrikeArticle(document);

    if (articleNode is null)
    {
      return null;
    }

    var description = CleanText(articleNode.InnerText);

    var startAt = ExtractStrikeStart(description);
    var endAt = ExtractStrikeEnd(description);

    var noticeLink = ExtractNoticeLink(document);
    return new Strike
    {
      Title = title,
      PublishedAt = publishedAt,
      StartAt = startAt,
      EndAt = endAt,
      Description = description,
      NoticeNumber = ExtractNoticeNumber(noticeLink?.InnerText),
      SourceUrl = sourceUrl,
      NoticeUrl = BuildAbsoluteUrl(noticeLink?.GetAttributeValue("href", null))
    };
  }

  private static string? ExtractTitle(HtmlDocument document)
  {
    var node = document.DocumentNode.SelectSingleNode(
        "//div[contains(@class,'container-foglia-news')]//h1[contains(@class,'uppercase')]");

    return node is null
        ? null
        : CleanText(node.InnerText);
  }

  private static DateTime? ExtractPublishedAt(HtmlDocument document)
  {
    var node = document.DocumentNode.SelectSingleNode(
        "//div[contains(@class,'container-foglia-news')]//p[contains(@class,'date-news')]");

    if (node is null)
    {
      return null;
    }

    var text = CleanText(node.InnerText);

    var formats = new[]
    {
            "dddd dd/MM/yyyy",
            "dd/MM/yyyy"
        };

    if (DateTime.TryParseExact(
            text,
            formats,
            new CultureInfo("it-IT"),
            DateTimeStyles.None,
            out var date))
    {
      return date;
    }

    return null;
  }

  private static HtmlNode? FindStrikeArticle(HtmlDocument document)
  {
    return document.DocumentNode.SelectSingleNode(
        "//div[contains(@class,'container-foglia-news')]"
        + "//div[contains(@class,'frame-type-trenordtheme_simpletextmedia')]"
        + "[1]");
  }

  private static DateTime? ExtractStrikeStart(string text)
  {
    var match = System.Text.RegularExpressions.Regex.Match(
        text,
        @"dalle\s+ore\s+(\d{1,2})[.:](\d{2})"
        + @".*?\b(?:del|di)\s+"
        + @"(\d{1,2})\s+"
        + @"(gennaio|febbraio|marzo|aprile|maggio|giugno|luglio|agosto|settembre|ottobre|novembre|dicembre)"
        + @"\s+(\d{4})",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    if (!match.Success)
    {
      return null;
    }

    return CreateDateTime(
        match.Groups[3].Value,
        match.Groups[4].Value,
        match.Groups[5].Value,
        match.Groups[1].Value,
        match.Groups[2].Value);
  }

  private static DateTime? ExtractStrikeEnd(string text)
  {
    var match = System.Text.RegularExpressions.Regex.Match(
        text,
        @"dalle\s+ore\s+\d{1,2}[.:]\d{2}"
        + @"\s+alle\s+ore\s+(\d{1,2})[.:](\d{2})"
        + @".*?\b(?:del|di)\s+"
        + @"(\d{1,2})\s+"
        + @"(gennaio|febbraio|marzo|aprile|maggio|giugno|luglio|agosto|settembre|ottobre|novembre|dicembre)"
        + @"\s+(\d{4})",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    if (!match.Success)
    {
      return null;
    }

    return CreateDateTime(
        match.Groups[3].Value,
        match.Groups[4].Value,
        match.Groups[5].Value,
        match.Groups[1].Value,
        match.Groups[2].Value);
  }

  private static DateTime? CreateDateTime(
      string day,
      string monthName,
      string year,
      string hour,
      string minute)
  {
    var months = new Dictionary<string, int>(
        StringComparer.OrdinalIgnoreCase)
    {
      ["gennaio"] = 1,
      ["febbraio"] = 2,
      ["marzo"] = 3,
      ["aprile"] = 4,
      ["maggio"] = 5,
      ["giugno"] = 6,
      ["luglio"] = 7,
      ["agosto"] = 8,
      ["settembre"] = 9,
      ["ottobre"] = 10,
      ["novembre"] = 11,
      ["dicembre"] = 12
    };

    if (!months.TryGetValue(monthName, out var month))
    {
      return null;
    }

    if (!int.TryParse(day, out var dayNumber) ||
        !int.TryParse(year, out var yearNumber) ||
        !int.TryParse(hour, out var hourNumber) ||
        !int.TryParse(minute, out var minuteNumber))
    {
      return null;
    }

    try
    {
      return new DateTime(
          yearNumber,
          month,
          dayNumber,
          hourNumber,
          minuteNumber,
          0);
    }
    catch
    {
      return null;
    }
  }

  private static HtmlNode? ExtractNoticeLink(HtmlDocument document)
  {
    return document.DocumentNode.SelectSingleNode(
        "//div[contains(@class,'container-foglia-news')]"
        + "//a[contains(normalize-space(.), 'Avviso')]");
  }

  private static string? ExtractNoticeNumber(string? text)
  {
    if (string.IsNullOrWhiteSpace(text))
    {
      return null;
    }

    var match = System.Text.RegularExpressions.Regex.Match(
        text,
        @"Avviso\s+(\d+/\d{4})",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    return match.Success
        ? match.Groups[1].Value
        : null;
  }
  private static string? BuildAbsoluteUrl(string? href) { if (string.IsNullOrWhiteSpace(href)) { return null; } href = href.Trim(); if (href.StartsWith("/")) { return $"https://www.trenord.it{href}"; } if (Uri.TryCreate(href, UriKind.Absolute, out var absoluteUri)) { return absoluteUri.ToString(); } return $"https://www.trenord.it/{href}"; }

  private static string CleanText(string text)
  {
    return HtmlEntity.DeEntitize(text)
        .Replace('\u00A0', ' ')
        .Replace("\r", " ")
        .Replace("\n", " ")
        .Replace("\t", " ")
        .Trim();
  }
}
