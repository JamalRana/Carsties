using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;

namespace SearchService;

public class DbInitializer
{
    public static async Task InitAsync(WebApplication app)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Items>()
        .Key(x => x.Make, KeyType.Text)
        .Key(x => x.Model, KeyType.Text)
        .Key(x => x.Color, KeyType.Text)
        .CreateAsync();

        var count = await DB.CountAsync<Items>();

        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var items = await httpClient.GetItemsForSearchDb();


        if (items.Count > 0) await DB.SaveAsync(items);



        //  Console.WriteLine("No - Data For Seeding.");
        //     var itemData = await File.ReadAllTextAsync("Data/auctions.json");

        //     var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        //     var items = JsonSerializer.Deserialize<List<Items>>(itemData, options);

        //     await DB.SaveAsync(items);



    }
}
