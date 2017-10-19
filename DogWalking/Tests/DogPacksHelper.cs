using Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    class DogPacksHelper
    {
        public static bool HaveSameType(IEnumerable<Dog> dogsInPack)
        {
            var first = dogsInPack.FirstOrDefault();
            return dogsInPack.All(d => d.Size == first.Size && d.IsAggressive == first.IsAggressive);
        }

        public static (DogSize, bool) GetDogPackType(DogPack pack)
        {
            var firstDog = pack.Dogs.FirstOrDefault();
            return (firstDog.Size, firstDog.IsAggressive);
        }
    }
}
