using Domain.Models;
using System.Collections.Generic;

namespace Web.Services
{
    public interface IDogPackBuilder
    {
        IEnumerable<DogPack> Build(IEnumerable<Dog> dogs, IEnumerable<DogPackConfig> configs);
    }
}
