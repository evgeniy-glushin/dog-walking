using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Services.Abstraction;

namespace Web.Services
{
    public class DogPackBuilder : IDogPackBuilder
    {
        /// <summary>
        /// Creates dog packs for the provided dogs.
        /// </summary>
        public IEnumerable<DogPack> Build(IEnumerable<Dog> dogs, IEnumerable<PriceRate> rates)
        {
            return dogs.GroupBy(
                d => new { d.Size, d.IsAggressive },
                (key, g) => new
                {
                    Size = key,
                    Dogs = g.ToList(),
                    rate = rates.FirstOrDefault(c => c.DogSize == key.Size && c.IsAggressive == key.IsAggressive)
                }).Where(x => x.rate != null)
                .SelectMany(x => BuildDogPacks(x.Dogs, x.rate));

            // Generates a sequence of dog packs with specified number of dogs
            IEnumerable<DogPack> BuildDogPacks(List<Dog> sameDogs, PriceRate rate)
            {
                var packsCount = rate.MaxPackSize > 0 ? (int)Math.Ceiling((decimal)sameDogs.Count / rate.MaxPackSize) : 0;
                return Enumerable.Range(0, packsCount)
                    .Select(n => new DogPack
                    {
                        Dogs = sameDogs
                            .Skip(n * rate.MaxPackSize)
                            .Take(rate.MaxPackSize)
                            .ToList()
                    });
            }
        }
    }
}
