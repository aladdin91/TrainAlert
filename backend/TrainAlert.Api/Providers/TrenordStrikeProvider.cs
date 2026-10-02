
using TrainAlert.Api.Models;

namespace TrainAlert.Api.Providers;

public class TrenordStrikeProvider : IStrikeProvider
{
  private const string StrikeUrl =
      "https://www.trenord.it/news/trenord-informa/avvisi/sciopero/";

  private readonly IHttpClientFactory _httpClientFactory;
  private readonly TrenordStrikeParser _parser;

  public TrenordStrikeProvider(
      IHttpClientFactory httpClientFactory,
      TrenordStrikeParser parser)
  {
    _httpClientFactory = httpClientFactory;
    _parser = parser;
  }

  public async Task<Strike?> GetCurrentStrikeAsync()
  {
    var client = _httpClientFactory.CreateClient();

    var response = await client.GetAsync(StrikeUrl);

    response.EnsureSuccessStatusCode();

    var html = await response.Content.ReadAsStringAsync();

    return _parser.Parse(html, StrikeUrl);
  }
}
