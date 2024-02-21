using AutoMapper;
using Contracts;

namespace SearchService;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Items>();
        CreateMap<AuctionUpdated, Items>();
    }
}
