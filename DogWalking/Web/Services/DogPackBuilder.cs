using System.Collections.Generic;
using System.Linq;
using Web.Models;


namespace Web.Services
{
    public class DogPackBuilder
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
                var packsCount = config.MaxPackCount > 0 ? sameDogs.Count / config.MaxPackCount + 1 : 0;
                return Enumerable.Range(0, packsCount)
                    .Select(n => new DogPack
                    {
                        Dogs = sameDogs
                            .Skip(n)
                            .Take(config.MaxPackCount)
                            .ToList()
                    });
            }
        }
    }
}
