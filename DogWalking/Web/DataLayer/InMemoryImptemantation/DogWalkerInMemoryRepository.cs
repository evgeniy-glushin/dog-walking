using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.DataLayer.Abstraction;

namespace Web.DataLayer.InMemoryImptemantation
{
    class DogWalkerInMemoryRepository : IDogWalkersRepository
    {
        public Task<DogWalker> GetByIdAsync(int id)
        {
            var dogWalker = InMemoryStorage
                .DogWalkers.FirstOrDefault(d => d.Id == id);

            return Task.FromResult(dogWalker);
        }

        public Task<List<DogPack>> GetDogPacks(int userId)
        {
            var dogWalker = InMemoryStorage
                .DogWalkers.FirstOrDefault(d => d.Id == userId);

            return Task.FromResult(dogWalker?.DogPacks ?? new List<DogPack>());
        }

        public Task<List<Walk>> GetWalks(int userId)
        {
            var dogWalker = InMemoryStorage
                .DogWalkers.FirstOrDefault(d => d.Id == userId);

            return Task.FromResult(dogWalker.BookedWalks ?? new List<Walk>());
        }

        public Task<bool> Save(DogWalker dogWalker)
        {
            return Task.FromResult(true);
        }
    }
}
