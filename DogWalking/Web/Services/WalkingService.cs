using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.DataLayer.Abstraction;

namespace Web.Services
{
    public interface IWalkingService
    {
        Task<IEnumerable<DogPack>> BuildDogPacksAsync(int userId);
        Task<IEnumerable<Walk>> BuildWalksAsync(int userId, BuildWalksPayload payload);
    }

    public class WalkingService : IWalkingService
    {
        private IDogPackBuilder _dogPackBuilder;
        private IWalksBuilder _walksBuilder;
        private IDogWalkersRepository _dogWalkersRepo;

        public WalkingService(IDogPackBuilder dogPackBuilder, 
            IWalksBuilder walksBuilder, 
            IDogWalkersRepository dogWalkersRepo)
        {
            _dogPackBuilder = dogPackBuilder;
            _walksBuilder = walksBuilder;
            _dogWalkersRepo = dogWalkersRepo;
        }

        // TODO: add error paths
        public async Task<IEnumerable<DogPack>> BuildDogPacksAsync(int userId)
        {
            var dogWalker = await _dogWalkersRepo.GetByIdAsync(userId);
            
            var dogs = dogWalker
                .Customers
                .SelectMany(c => c.Dogs);

            var congigs = dogWalker
                .PriceRates
                .Select(pr => new DogPackConfig { DogSize = pr.DogSize, IsAggressive = pr.IsAggressive, MaxPacksCount = pr.MaxPacksCount });

            dogWalker.DogPacks = _dogPackBuilder
                .Build(dogs, congigs)
                .ToList();

            var isSaved = await _dogWalkersRepo.Save(dogWalker);

            return dogWalker.DogPacks;
        }

        public async Task<IEnumerable<Walk>> BuildWalksAsync(int userId, BuildWalksPayload payload)
        {
            var dogWalker = await _dogWalkersRepo.GetByIdAsync(userId);
            
            if (dogWalker.BookedWalks == null)
            {
                var dogPacks = await BuildDogPacksAsync(userId);
                dogWalker.DogPacks = dogPacks.ToList();
            }

            var walks = _walksBuilder.Build(new WeeklySchedulePayload
            {
                DateFrom = payload.DateFrom,
                DateTo = payload.DateTo,
                Now = DateTime.Now,
                DayStartsAt = TimeSpan.FromHours(10), // TODO: find out where to take this from
                DayEndsAt = TimeSpan.FromHours(16),   // TODO: find out where to take this from
                DogPacks = dogWalker.DogPacks,
                WalkDuration = dogWalker.DefaultWalkDuration,
                WorkingDays = dogWalker.WorkingDays
            }).ToList();

            dogWalker.BookedWalks = walks;
            // TODO: add error path
            var isSaved = await _dogWalkersRepo.Save(dogWalker);

            return dogWalker.BookedWalks;
        }
    }

    public class BuildWalksPayload
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
