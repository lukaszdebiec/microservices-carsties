using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuctionService.Consumers
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        private readonly AuctionDbContext _dbContext; 
        public AuctionFinishedConsumer(AuctionDbContext context)
        {
            _dbContext = context;
        }
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("---> Consuming Auction Finished");

            var auction = await _dbContext.Auctions.FindAsync(context.Message.AuctionId);

            if (auction == null){
                Console.WriteLine("given aution not found");
                return;
            }

            if(context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount;
            }

            auction.Status = auction.SoldAmount > auction.ReservePrice ? Status.Finished : Status.ReserveNotMet;

            await _dbContext.SaveChangesAsync();
            
        }

    }
}