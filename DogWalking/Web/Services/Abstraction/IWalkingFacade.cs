using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Services
{
    public interface IWalkingFacade
    {
        Task<(StatusCode, IEnumerable<DogPack>)> BuildDogPacksAsync(int userId);
        Task<(StatusCode, IEnumerable<Walk>)> BuildWalksAsync(int userId, BuildWalksPayload payload);
    }
}
