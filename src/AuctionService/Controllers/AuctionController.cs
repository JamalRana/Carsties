using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;


[Route("api/auctions")]
[ApiController]
public class AuctionController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionController(AuctionDbContext context, IMapper mapper)
    {
        this._mapper = mapper;
        this._context = context;

    }

    // GET: api/Auction
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllAuctions(string date)
    {

        var query = _context.Auctions.OrderBy(a => a.Item.Make).AsQueryable();
        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(a => a.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        // var auctions = await _context.Auctions
        // .Include(a => a.Item)
        // .OrderBy(a => a.Item.Make)
        // .ToListAsync();
        // return _mapper.Map<List<AuctionDto>>(auctions);
        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    // GET: api/Auction/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
    {
        var auction = await _context.Auctions
        .Include(a => a.Item)
        .FirstOrDefaultAsync(a => a.Id == id);
        if (auction == null)
        {
            return NotFound();
        }
        return _mapper.Map<AuctionDto>(auction);
    }

    // POST: api/Auction
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction([FromBody] CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        // TODO: Add Current user as seller
        auction.Seller = "bob";
        _context.Auctions.Add(auction);
        var result = await _context.SaveChangesAsync() > 0;
        if (!result)
        {
            return BadRequest("Failed to create auction");
        }


        return CreatedAtAction(nameof(GetAuction), new { auction.Id }, _mapper.Map<AuctionDto>(auction)); // CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, auction);
    }

    // PUT: api/Auction/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, [FromBody] UpdateAuctionDto updateAuctionDto)
    {
        var auction = _context.Auctions.Include(a => a.Item).FirstOrDefault(a => a.Id == id);
        if (auction == null)
        {
            return NotFound();
        }

        // TODO: Add Current user as seller
        // auction.Seller = "bob";
        // auction.Status = updateAuctionDto.Status;
        // _context.Entry(auction).State = EntityState.Modified;

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.UpdatedAt = DateTime.UtcNow;


        var result = await _context.SaveChangesAsync() > 0;
        if (result)
        {
            return Ok();
        }

        return BadRequest("Failed to update auction");
    }

    // DELETE: api/Auction/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuction(Guid id)
    {
        var auction = _context.Auctions.FindAsync(id);


        _context.Auctions.Remove(await auction);


        var result = await _context.SaveChangesAsync() > 0;
        if (result)
        {
            return Ok();
        }

        return BadRequest("Failed to delete auction");
    }
}
// create
// read
// update
// delete

// GET: api/Auction
// GET: api/Auction/{id}
// POST: api/Auction
// PUT: api/Auction/{id}
// DELETE: api/Auction/{id}
