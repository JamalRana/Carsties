using AutoMapper;
using Contracts;
using MassTransit;
using MassTransit.Internals;
using MongoDB.Entities;

namespace SearchService;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper;

    public AuctionUpdatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("--> Consuming Auction Updated." + context.Message.Id);

        var item = _mapper.Map<Items>(context.Message);


        var result = DB.Update<Items>()
        .Match(a => a.ID == context.Message.Id)
        .ModifyOnly(x => new
        {
            x.Color,
            x.Model,
            x.Make,
            x.Year,
            x.Mileage
        }, item)
        .ExecuteAsync();

        if (!result.IsCompletedSuccessfully) throw new MessageException(typeof(AuctionUpdated), "Could not update item");

    }
}
