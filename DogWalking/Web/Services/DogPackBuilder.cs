using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Web.Services
{
    public class DogPackBuilder : IDogPackBuilder
    {
        public IEnumerable<DogPack> Build(IEnumerable<Dog> dogs, IEnumerable<DogPackConfig> configs)
        {
            return dogs.GroupBy(
                d => new { d.Size, d.IsAggressive },
                (key, g) => new
                {
                    Size = key,
                    Dogs = g.ToList(),
                    config = configs.FirstOrDefault(c => c.DogSize == key.Size && c.IsAggressive == key.IsAggressive)
                }).Where(x => x.config != null)
                .SelectMany(x => BuildDogPacks(x.Dogs, x.config));

            IEnumerable<DogPack> BuildDogPacks(List<Dog> sameDogs, DogPackConfig config)
            {
                var packsCount = config.MaxPacksCount > 0 ? (int)Math.Ceiling((decimal)sameDogs.Count / config.MaxPacksCount) : 0;
                return Enumerable.Range(0, packsCount)
                    .Select(n => new DogPack
                    {
                        Dogs = sameDogs
                            .Skip(n * config.MaxPacksCount)
                            .Take(config.MaxPacksCount)
                            .ToList()
                    });
            }
        }
    }
}
