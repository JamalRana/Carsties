using MongoDB.Entities;

namespace SearchService;

public class AuctionSvcHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<Items>> GetItemsForSearchDb()
    {
        var lastUpdated = await DB.Find<Items, string>()
                            .Sort(x => x.Descending(a => a.UpdatedAt))
                            .Project(x => x.UpdatedAt.ToString())
                            .ExecuteFirstAsync();

        return await _httpClient.GetFromJsonAsync<List<Items>>(_config["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated);

    }

}
