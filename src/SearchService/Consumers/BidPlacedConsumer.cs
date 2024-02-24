using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{


    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        var auction = await DB.Find<Items>().OneAsync(context.Message.AuctionId);

        if (context.Message.BidStatus.Contains("Accepted")
            && context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await DB.SaveAsync(auction);
        }


    }
}
