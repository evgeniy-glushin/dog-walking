using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.DataLayer.Abstraction
{
    public interface IDogWalkersRepository
    {
        Task<DogWalker> GetByIdAsync(int id);
        Task<List<DogPack>> GetDogPacks(int userId);
        Task<List<Walk>> GetWalks(int userId);
        Task<bool> Save(DogWalker dogWalker);
    }
}
