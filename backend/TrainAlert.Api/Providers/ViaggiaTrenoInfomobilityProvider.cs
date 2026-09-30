
using System.Text.RegularExpressions;

using TrainAlert.Api.Models;
using TrainAlert.Api.Services;

namespace TrainAlert.Api.Providers;

public class ViaggiaTrenoInfomobilityProvider
    : IInfomobilityProvider
{
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly DisruptionAnalyzer _analyzer;

  public ViaggiaTrenoInfomobilityProvider(
      IHttpClientFactory httpClientFactory,
      DisruptionAnalyzer analyzer)
  {
    _httpClientFactory = httpClientFactory;
    _analyzer = analyzer;
  }

  public async Task<List<Disruption>> GetDisruptionsAsync()
  {
    var client =
        _httpClientFactory.CreateClient();

    var url =
        "http://www.viaggiatreno.it/infomobilita/resteasy/viaggiatreno/infomobilitaRSS/false";

    var response =
        await client.GetAsync(url);

    response.EnsureSuccessStatusCode();

    var html =
        await response.Content.ReadAsStringAsync();

    return Parse(html);
  }

  private List<Disruption> Parse(string html)
  {
    var disruptions = new List<Disruption>();

    var matches = Regex.Matches(
        html,
        @"<a[^>]*class=""headingNewsAccordion[^""]*""[^>]*>(.*?)</a>\s*<div.*?<h4>(.*?)</h4>.*?<div class=""info-text[^""]*"">(.*?)</div>",
        RegexOptions.Singleline |
        RegexOptions.IgnoreCase);

    foreach (Match match in matches)
    {
      var title =
          CleanHtml(match.Groups[1].Value);

      var dateText =
          CleanHtml(match.Groups[2].Value);

      var description =
          CleanHtml(match.Groups[3].Value);

      DateTime.TryParse(
          dateText,
          out var date);

      var disruption = new Disruption
      {
        Title = title,
        Description = description,
        Date = date == default
              ? null
              : date
      };

      disruptions.Add(
          _analyzer.Analyze(disruption));
    }

    return disruptions;
  }

  private static string CleanHtml(string value)
  {
    var text =
        Regex.Replace(
            value,
            "<.*?>",
            " ");

    text =
        System.Net.WebUtility
            .HtmlDecode(text);

    return Regex.Replace(
            text,
            @"\s+",
            " ")
        .Trim();
  }
}
