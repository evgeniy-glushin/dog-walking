using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.DataLayer.Abstraction;

namespace Web.Services
{    
    public class WalkingFacade : IWalkingFacade
    {
        private IDogPackBuilder _dogPackBuilder;
        private IScheduleBuilder _walksBuilder;
        private IDogWalkersRepository _dogWalkersRepo;

        public WalkingFacade(IDogPackBuilder dogPackBuilder, 
            IScheduleBuilder walksBuilder, 
            IDogWalkersRepository dogWalkersRepo)
        {
            _dogPackBuilder = dogPackBuilder;
            _walksBuilder = walksBuilder;
            _dogWalkersRepo = dogWalkersRepo;
        }

        /// <summary>
        /// Creates and saves dog packs for provided dog walker.
        /// </summary>
        /// <param name="userId">The dog walker's ID.</param>
        /// <returns>Tuple of two values where left item is a status of 
        /// the operation and the right one is dog packs created.</returns>
        public async Task<(StatusCode, IEnumerable<DogPack>)> BuildDogPacksAsync(int userId)
        {
            var dogWalker = await _dogWalkersRepo.GetByIdAsync(userId);

            if (dogWalker == null)
                return (StatusCode.DogWalkerNotFound, null);

            var dogs = dogWalker
                .Customers
                .SelectMany(c => c.Dogs);
            
            dogWalker.DogPacks = _dogPackBuilder
                .Build(dogs, dogWalker.PriceRates)
                .ToList();

            var isSaved = await _dogWalkersRepo.Save(dogWalker);
            if (isSaved)
                return (StatusCode.Ok, dogWalker.DogPacks);
            else
                return (StatusCode.SaveDogWalkerDbError, null);
        }

        /// <summary>
        /// Creates and saves walks and dog pack if needed for provided dog walker.
        /// </summary>
        /// <param name="userId">The dog walker's ID.</param>
        /// <returns>Tuple of two values where left item is a status of 
        /// the operation and the right one is walks created.</returns>
        public async Task<(StatusCode, IEnumerable<Walk>)> BuildWalksAsync(int userId, BuildWalksPayload payload)
        {
            var dogWalker = await _dogWalkersRepo.GetByIdAsync(userId);

            if (dogWalker == null)
                return (StatusCode.DogWalkerNotFound, null);

            if (dogWalker.BookedWalks == null)
            {
                var (status, dogPacks) = await BuildDogPacksAsync(userId);
                if (status == StatusCode.Ok)
                    dogWalker.DogPacks = dogPacks.ToList();
                else
                    return (status, null);
            }

            dogWalker.BookedWalks = _walksBuilder.Build(new WeekDaysSchedulePayload
            {
                DateFrom = payload.DateFrom,
                DateTo = payload.DateTo,
                Now = DateTime.Now,
                DogPacks = dogWalker.DogPacks,
                WalkDuration = dogWalker.DefaultWalkDuration,
                WorkingDays = dogWalker.WorkingDays,
                PriceRates = dogWalker.PriceRates
            }).ToList();
                        
            var isSaved = await _dogWalkersRepo.Save(dogWalker);

            if (isSaved)
                return (StatusCode.Ok, dogWalker.BookedWalks);
            else
                return (StatusCode.SaveDogWalkerDbError, null);
        }
    }

    public enum StatusCode
    {
        Ok = 1,
        DogWalkerNotFound = 2,
        SaveDogWalkerDbError = 3
    }
}
