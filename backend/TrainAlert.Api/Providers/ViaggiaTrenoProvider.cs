using System.Net.Http;

namespace TrainAlert.Api.Providers;

public class ViaggiaTrenoProvider : ITrainDataProvider
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ViaggiaTrenoProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<Models.Train>> GetTrainsAsync()
    {
        var client = _httpClientFactory.CreateClient();

        var response = await client.GetAsync(
            "http://www.viaggiatreno.it/infomobilita/resteasy/viaggiatreno/cercaStazione/Milano"
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        Console.WriteLine(json);

        return new List<Models.Train>();
    }
}