using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        private readonly AuctionDbContext _dbContext;
        public BidPlacedConsumer(AuctionDbContext context)
        {
            _dbContext = context;
        }
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("---> Consuming Bid placed");

            var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);

            if (auction == null){
                Console.WriteLine("given aution not found");
                return;
            }

            if(auction.CurrentHighBid == null 
            || 
            context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}